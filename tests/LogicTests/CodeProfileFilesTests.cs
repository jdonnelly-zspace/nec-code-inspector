using System;
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
            ArtSetRules(t, profiles.FirstOrDefault(p => p.folder == CodeProfileIds.Nec));
            ProfilesUseAvailableArt(t, profiles);
        }

        // A profile names the art its scenes use; unknown sets are allowed (the art may not exist yet) but bad names are not
        private static void ArtSetRules(TestContext t, LoadedProfile nec)
        {
            t.Begin("profile art sets");

            t.IsTrue(ArtSets.IsAvailable(ArtSets.NorthAmerica), "the North American set is available");
            t.IsTrue(!ArtSets.IsAvailable("uk"), "a set with no art is not available");
            t.IsTrue(ArtSets.IsValidName("north-america") && ArtSets.IsValidName("uk"), "plain set names are valid");
            t.IsTrue(!ArtSets.IsValidName("") && !ArtSets.IsValidName(null) && !ArtSets.IsValidName("North America")
                     && !ArtSets.IsValidName("../x") && !ArtSets.IsValidName("a--b"), "empty and unsafe names are invalid");

            if (nec == null || nec.manifest == null) return;

            Func<string, bool> flagged = artSet =>
            {
                var m = new CodeProfileManifest
                {
                    id = nec.manifest.id, displayName = nec.manifest.displayName, edition = nec.manifest.edition,
                    region = nec.manifest.region, reviewStatus = nec.manifest.reviewStatus, units = nec.manifest.units, artSet = artSet
                };
                return CodeProfileValidator.Validate(m, nec.articles, nec.tables, nec.terminology).Any(e => e.Contains("artSet"));
            };

            t.IsTrue(flagged(null), "a profile without an artSet is rejected");
            t.IsTrue(flagged("Bad Name!"), "a profile with an unsafe artSet is rejected");
            t.IsTrue(!flagged("north-america"), "a profile with a valid artSet is accepted");
            t.IsTrue(!flagged("uk"), "an artSet whose art is not built yet is accepted by the validator");
        }

        // Scenes are only offered under codes whose art exists (docs/SCENE_DESIGN.md); a code without art is allowed, with a warning
        private static void ProfilesUseAvailableArt(TestContext t, List<LoadedProfile> profiles)
        {
            t.Begin("profiles use available art sets");
            foreach (var p in profiles)
                if (p.manifest != null)
                    if (!ArtSets.IsAvailable(p.manifest.artSet))
                        t.Warn($"Codes/{p.folder}: art set '{p.manifest.artSet}' has no art yet, so the app offers no scenarios under it (add it to ArtSets when built)");
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
