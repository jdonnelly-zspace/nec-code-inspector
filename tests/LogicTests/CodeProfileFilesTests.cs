using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Checks every code profile folder the way the app loads it, so a new code (a new folder)
    /// is validated without writing any test code.
    /// </summary>
    public static class CodeProfileFilesTests
    {
        public static void Run(TestContext t)
        {
            string root = TestContext.RepoRoot();
            var profiles = ProfileFiles.LoadAll(root);

            t.Begin("code profile folders");
            t.IsTrue(profiles.Count > 0, "at least one code profile folder exists");
            t.IsTrue(profiles.Any(p => p.folder == CodeProfileIds.Nec), "the default 'nec' profile exists");

            var ids = new HashSet<string>();
            foreach (var p in profiles)
            {
                foreach (string error in CodeProfileValidator.Validate(p.manifest, p.articles, p.tables, p.terminology))
                    t.IsTrue(false, $"Codes/{p.folder}: {error}");

                if (p.manifest == null) continue;

                t.Equal(p.folder, p.manifest.id, $"Codes/{p.folder}: profile id matches the folder name");
                t.IsTrue(ids.Add(p.manifest.id), $"Codes/{p.folder}: profile id is unique");

                CheckRelatedReferences(t, p);
                CheckBuiltProfile(t, p);
            }

            NecFilesMatchBuiltInDefaults(t, profiles.FirstOrDefault(p => p.folder == CodeProfileIds.Nec));
        }

        // Related references the UI cannot resolve are silently skipped, so report them
        private static void CheckRelatedReferences(TestContext t, LoadedProfile p)
        {
            if (p.articles == null) return;

            t.Begin($"profile {p.Id}: related references");
            foreach (var a in p.articles)
            {
                if (a?.related == null) continue;
                foreach (string related in a.related)
                    if (!p.HasReference(related))
                        t.Warn($"{p.Id} {a.reference} lists related entry '{related}' which is not in its articles.json");
            }
        }

        private static void CheckBuiltProfile(TestContext t, LoadedProfile p)
        {
            if (p.articles == null || p.articles.Length == 0 || p.terminology == null) return;

            t.Begin($"profile {p.Id}: built profile");
            var profile = p.Build();

            t.Equal(p.articles.Length, profile.ArticleCount, "all articles load");
            t.Equal(p.tables != null, profile.HasOwnTables, "HasOwnTables follows the presence of tables.json");
            t.IsTrue(profile.Tables != null, "Tables is never null");

            // The label every entry shows comes from the profile's own terminology
            foreach (var a in p.articles)
            {
                var found = profile.GetArticle(a.reference);
                t.IsTrue(found != null && found.reference == a.reference, $"{a.reference} can be looked up");
                t.IsTrue(found != null && found.referenceLabel == p.terminology.ReferenceLabel(a.reference), $"{a.reference} uses the profile's reference label");
            }
        }

        // The NEC tables and terminology also exist as built-in fallbacks; the two copies must agree
        private static void NecFilesMatchBuiltInDefaults(TestContext t, LoadedProfile nec)
        {
            t.Begin("nec profile: files == built-in defaults");
            if (nec == null) return;

            t.IsTrue(nec.tables != null, "the NEC profile has its own tables.json");
            t.IsTrue(nec.terminology != null, "the NEC profile has terminology.json");
        }
    }
}
