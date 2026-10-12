using System.IO;
using System.Linq;
using UnityEngine;

namespace NECInspector.Data
{
    /// <summary>
    /// Loads the skill lessons from StreamingAssets/Lessons/lessons.json (see <see cref="LessonFileData"/>).
    /// Invalid files are logged and give no lessons, so a broken lesson file never stops the app. The text is shown through
    /// the active code's terminology (<c>CodeProfiles.Terminology.Format</c>) so words follow the student's region.
    /// </summary>
    public static class LessonLibrary
    {
        private static LessonFileData _file;
        private static bool _loaded;

        /// <summary>The lesson for a skill, or null if there is none.</summary>
        public static SkillLessonData Get(string skillId)
        {
            if (!_loaded) Load();
            return _file?.lessons?.FirstOrDefault(l => l != null && l.skillId == skillId);
        }

        public static void Reload()
        {
            _loaded = false;
            _file = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() => Reload();

        private static void Load()
        {
            _loaded = true;
            string path = Path.Combine(Application.streamingAssetsPath, "Lessons", "lessons.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[LessonLibrary] No lessons file at {path}");
                return;
            }

            try
            {
                var file = JsonUtility.FromJson<LessonFileData>(File.ReadAllText(path));
                var errors = LessonValidator.Validate(file);
                if (errors.Count > 0)
                {
                    foreach (string error in errors) Debug.LogError($"[LessonLibrary] lessons.json: {error}");
                    return;
                }

                _file = file;
                Debug.Log($"[LessonLibrary] Loaded {file.lessons.Length} lessons");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LessonLibrary] Could not read lessons.json: {e.Message}");
            }
        }
    }
}
