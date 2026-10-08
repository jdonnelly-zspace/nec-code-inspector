using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// One code profile folder under StreamingAssets/Codes, read from disk the way the app reads it.
    /// </summary>
    public class LoadedProfile
    {
        public string folder;
        public CodeProfileManifest manifest;
        public CodeArticleData[] articles;
        public ElectricalTables tables;         // null when the folder has no tables.json
        public CodeTerminology terminology;
        public ProtectionScope scope;           // null when the folder has no scope.json

        public string Id => manifest?.id ?? folder;

        public DataCodeProfile Build()
        {
            return new DataCodeProfile(manifest, articles, tables, terminology) { Scope = scope };
        }

        public bool HasReference(string reference)
        {
            return articles.Any(a => string.Equals(a.reference, reference, StringComparison.OrdinalIgnoreCase))
                   || articles.Any(a => CitationMatcher.IsParentOf(reference, a.reference));
        }
    }

    public static class ProfileFiles
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true };

        public static string CodesDir(string root)
        {
            return Path.Combine(root, "Assets/_Project/StreamingAssets/Codes");
        }

        public static List<LoadedProfile> LoadAll(string root)
        {
            var profiles = new List<LoadedProfile>();

            foreach (string dir in Directory.GetDirectories(CodesDir(root)).OrderBy(d => d, StringComparer.Ordinal))
            {
                profiles.Add(new LoadedProfile
                {
                    folder = Path.GetFileName(dir),
                    manifest = Read<CodeProfileManifest>(Path.Combine(dir, "profile.json")),
                    articles = Read<CodeArticleFile>(Path.Combine(dir, "articles.json"))?.articles,
                    tables = Read<ElectricalTables>(Path.Combine(dir, "tables.json")),
                    terminology = Read<CodeTerminology>(Path.Combine(dir, "terminology.json")),
                    scope = Read<ProtectionScope>(Path.Combine(dir, "scope.json"))
                });
            }

            return profiles;
        }

        public static LoadedProfile Load(string root, string id)
        {
            return LoadAll(root).FirstOrDefault(p => p.folder == id);
        }

        private static T Read<T>(string path) where T : class
        {
            return File.Exists(path) ? JsonSerializer.Deserialize<T>(File.ReadAllText(path), Options) : null;
        }
    }
}
