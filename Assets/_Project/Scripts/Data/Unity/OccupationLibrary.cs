using System.IO;
using System.Linq;
using UnityEngine;

namespace NECInspector.Data
{
    /// <summary>
    /// Loads the occupations from StreamingAssets/Occupations/occupations.json (see <see cref="OccupationFileData"/>).
    /// Invalid files are logged and give no occupations, so a broken file never stops the app.
    /// </summary>
    public static class OccupationLibrary
    {
        private static OccupationFileData _file;
        private static bool _loaded;

        public static OccupationData[] All
        {
            get
            {
                if (!_loaded) Load();
                return _file?.occupations ?? new OccupationData[0];
            }
        }

        public static OccupationData Get(string id) => All.FirstOrDefault(o => o != null && o.id == id);

        public static ProcedureData GetProcedure(string procedureId)
        {
            foreach (var o in All)
            {
                var p = o?.procedures?.FirstOrDefault(x => x != null && x.id == procedureId);
                if (p != null) return p;
            }
            return null;
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
            string path = Path.Combine(Application.streamingAssetsPath, "Occupations", "occupations.json");
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[OccupationLibrary] No occupations file at {path}");
                return;
            }

            try
            {
                var file = JsonUtility.FromJson<OccupationFileData>(File.ReadAllText(path));
                var errors = OccupationValidator.Validate(file);
                if (errors.Count > 0)
                {
                    foreach (string error in errors) Debug.LogError($"[OccupationLibrary] occupations.json: {error}");
                    return;
                }

                _file = file;
                Debug.Log($"[OccupationLibrary] Loaded {file.occupations.Length} occupations");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[OccupationLibrary] Could not read occupations.json: {e.Message}");
            }
        }
    }
}
