using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NECInspector.Codes
{
    /// <summary>
    /// Loads every code profile folder under StreamingAssets/Codes/ (profile.json, articles.json,
    /// terminology.json, optional tables.json). Invalid folders are logged and skipped.
    /// Adding a code means adding a folder, not changing code.
    /// </summary>
    public static class CodeProfileLibrary
    {
        public const string DefaultProfileId = CodeProfileIds.Nec;

        private static List<DataCodeProfile> _all;

        public static IReadOnlyList<DataCodeProfile> All
        {
            get
            {
                if (_all == null) Load();
                return _all;
            }
        }

        public static DataCodeProfile Get(string id)
        {
            return All.FirstOrDefault(p => p.ProfileId == id);
        }

        /// <summary>The loaded code that belongs to a region, or null.</summary>
        public static DataCodeProfile ForRegion(string region)
        {
            var profile = RegionChoices.ProfileForRegion(All, region);
            return profile == null ? null : Get(profile.ProfileId);
        }

        /// <summary>Make a profile the active one. Returns false if it is not loaded.</summary>
        public static bool Activate(string id)
        {
            var profile = Get(id);
            if (profile == null)
            {
                Debug.LogWarning($"[CodeProfileLibrary] Cannot activate unknown code profile '{id}'");
                return false;
            }

            CodeProfiles.SetActive(profile);
            Debug.Log($"[CodeProfileLibrary] Active code profile: {profile.DisplayName} {profile.Edition}");
            return true;
        }

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
            _all = new List<DataCodeProfile>();
            string root = Path.Combine(Application.streamingAssetsPath, "Codes");

            if (!Directory.Exists(root))
            {
                Debug.LogWarning($"[CodeProfileLibrary] Codes folder not found at {root}");
                return;
            }

            foreach (string dir in Directory.GetDirectories(root).OrderBy(d => d, StringComparer.Ordinal))
            {
                string name = Path.GetFileName(dir);

                try
                {
                    var manifest = ReadJson<CodeProfileManifest>(Path.Combine(dir, "profile.json"));
                    var articles = ReadJson<CodeArticleFile>(Path.Combine(dir, "articles.json"));
                    var terminology = ReadJson<CodeTerminology>(Path.Combine(dir, "terminology.json"));
                    var tables = ReadJson<ElectricalTables>(Path.Combine(dir, "tables.json"));
                    var scope = ReadJson<ProtectionScope>(Path.Combine(dir, "scope.json"));

                    var errors = CodeProfileValidator.Validate(manifest, articles?.articles, tables, terminology);
                    if (manifest != null && manifest.id != name)
                        errors.Add($"id '{manifest.id}' must match the folder name '{name}'");
                    if (scope != null)
                        errors.AddRange(ProtectionScopeValidator.Validate(scope).Select(e => "scope.json: " + e));

                    if (errors.Count > 0)
                    {
                        foreach (string error in errors)
                            Debug.LogError($"[CodeProfileLibrary] Codes/{name}: {error}");
                        continue;
                    }

                    if (terminology.terms == null)
                        terminology.terms = new TermEntry[0];

                    var profile = new DataCodeProfile(manifest, articles.articles, tables, terminology) { Scope = scope };
                    if (!profile.License.IsCleared)
                        Debug.LogWarning($"[CodeProfileLibrary] Codes/{name}: licence status is '{profile.License.status}'. Settle it before shipping this code (docs/CONTENT_POLICY.md, Licensing).");
                    _all.Add(profile);
                    Debug.Log($"[CodeProfileLibrary] Loaded {profile.DisplayName} {profile.Edition}: {profile.ArticleCount} entries, " +
                              $"{(profile.HasOwnTables ? "own tables" : "no tables")}, status {manifest.reviewStatus}");
                }
                catch (Exception e)
                {
                    Debug.LogError($"[CodeProfileLibrary] Codes/{name}: could not load: {e.Message}");
                }
            }
        }

        // Returns null if the file does not exist
        private static T ReadJson<T>(string path) where T : class
        {
            return File.Exists(path) ? JsonUtility.FromJson<T>(File.ReadAllText(path)) : null;
        }
    }
}
