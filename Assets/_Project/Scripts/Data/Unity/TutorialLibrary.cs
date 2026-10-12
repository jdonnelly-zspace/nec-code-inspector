using System.IO;
using UnityEngine;

namespace NECInspector.Data
{
    /// <summary>
    /// Loads the guided tutorial from StreamingAssets/Tutorial/tutorial.json (see <see cref="TutorialFileData"/>).
    /// An invalid file is logged and gives no steps, so the app still runs without the tutorial.
    /// </summary>
    public static class TutorialLibrary
    {
        private static TutorialStepData[] _steps;
        private static bool _loaded;

        public static TutorialStepData[] Steps
        {
            get
            {
                if (!_loaded) Load();
                return _steps ?? new TutorialStepData[0];
            }
        }

        public static void Reload()
        {
            _loaded = false;
            _steps = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad() => Reload();

        private static void Load()
        {
            _loaded = true;
            string path = Path.Combine(Application.streamingAssetsPath, "Tutorial", "tutorial.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[TutorialLibrary] No tutorial file at {path}");
                return;
            }

            try
            {
                var file = JsonUtility.FromJson<TutorialFileData>(File.ReadAllText(path));
                var errors = TutorialValidator.Validate(file);
                if (errors.Count > 0)
                {
                    foreach (string error in errors) Debug.LogError($"[TutorialLibrary] tutorial.json: {error}");
                    return;
                }

                _steps = file.steps;
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[TutorialLibrary] Could not read tutorial.json: {e.Message}");
            }
        }
    }
}
