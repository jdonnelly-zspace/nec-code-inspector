using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NECInspector.Credentials
{
    /// <summary>
    /// Loads credential profiles from StreamingAssets/Credentials/*.json. Invalid files are
    /// logged and skipped. Adding a credential means adding a file, not changing code.
    /// </summary>
    public static class CredentialLibrary
    {
        public const string DefaultCredentialId = "core-skills";

        private static List<CredentialProfile> _all;

        public static IReadOnlyList<CredentialProfile> All
        {
            get
            {
                if (_all == null) Load();
                return _all;
            }
        }

        public static CredentialProfile Get(string id)
        {
            return All.FirstOrDefault(c => c.id == id);
        }

        /// <summary>The app-defined default credential, or the first one loaded, or null if none.</summary>
        public static CredentialProfile Default => Get(DefaultCredentialId) ?? All.FirstOrDefault();

        public static void Reload()
        {
            _all = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad()
        {
            _all = null;
        }

        private static void Load()
        {
            _all = new List<CredentialProfile>();
            string dir = Path.Combine(Application.streamingAssetsPath, "Credentials");

            if (!Directory.Exists(dir))
            {
                Debug.LogWarning($"[CredentialLibrary] Credentials folder not found at {dir}");
                return;
            }

            foreach (string file in Directory.GetFiles(dir, "*.json").OrderBy(f => f, StringComparer.Ordinal))
            {
                try
                {
                    var profile = JsonUtility.FromJson<CredentialProfile>(File.ReadAllText(file));
                    var errors = CredentialValidator.Validate(profile);

                    if (errors.Count > 0)
                    {
                        foreach (string error in errors)
                            Debug.LogError($"[CredentialLibrary] {Path.GetFileName(file)}: {error}");
                        continue;
                    }

                    if (_all.Any(c => c.id == profile.id))
                    {
                        Debug.LogError($"[CredentialLibrary] {Path.GetFileName(file)}: duplicate credential id '{profile.id}'");
                        continue;
                    }

                    _all.Add(profile);
                }
                catch (Exception e)
                {
                    Debug.LogError($"[CredentialLibrary] {Path.GetFileName(file)}: could not load: {e.Message}");
                }
            }

            Debug.Log($"[CredentialLibrary] Loaded {_all.Count} credential(s)");
        }
    }
}
