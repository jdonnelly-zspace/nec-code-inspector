using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Companion standards (fire alarm, line work): they supplement a code, so their entries and violations apply wherever that code is the student's.
    /// Uses the real profile folders.
    /// </summary>
    public static class SupplementTests
    {
        public static void Run(TestContext t)
        {
            var loaded = ProfileFiles.LoadAll(TestContext.RepoRoot());
            var profiles = loaded.Select(p => (ICodeProfile)p.Build()).ToList();
            var byId = profiles.ToDictionary(p => p.ProfileId);

            t.Begin("companion profiles: the real files");
            foreach (string id in new[] { "fire-alarm", "utility-safety" })
            {
                t.IsTrue(byId.ContainsKey(id), $"{id} is loaded");
                var companion = byId[id];
                t.IsTrue(companion.Supplements.Contains("nec-2023") && companion.Supplements.Contains("nec-2026"), $"{id} supplements both NEC profiles");
                t.IsTrue(!companion.Supplements.Contains(id), $"{id} does not supplement itself");
            }
            t.IsTrue(byId["nec-2023"].Supplements.Length == 0 && byId["cec"].Supplements.Length == 0, "an ordinary code supplements nothing");

            t.Begin("companion profiles: who applies under which code");
            var map = ProfileSupplements.Build(profiles);
            t.Equal("nec-2023,fire-alarm,utility-safety", string.Join(",", ProfileSupplements.CitationIds(map, "nec-2023")), "the NEC first, then its companions in id order");
            t.Equal("nec-2026,fire-alarm,utility-safety", string.Join(",", ProfileSupplements.CitationIds(map, "nec-2026")), "the same under the 2026 profile");
            t.Equal("cec", string.Join(",", ProfileSupplements.CitationIds(map, "cec")), "a code with no companions is just itself");
            t.Equal("", string.Join(",", ProfileSupplements.CitationIds(map, "")), "no id, no citation ids");
            t.Equal("x", string.Join(",", ProfileSupplements.CitationIds(null, "x")), "no map, just the id");
            t.Equal(0, ProfileSupplements.Build(null).Count, "no profiles, an empty map");

            t.Begin("companion profiles: a violation citing a companion applies under the code");
            var citations = new[] { new ViolationCitation { profileId = "fire-alarm", reference = "10.6.5", text = "x" } };
            t.Equal("fire-alarm", ViolationCitations.FindAny(citations, ProfileSupplements.CitationIds(map, "nec-2023"))?.profileId, "found under the NEC 2023 profile");
            t.Equal("fire-alarm", ViolationCitations.FindAny(citations, ProfileSupplements.CitationIds(map, "nec-2026"))?.profileId, "found under the NEC 2026 profile");
            t.IsTrue(ViolationCitations.FindAny(citations, ProfileSupplements.CitationIds(map, "cec")) == null, "not found under the CEC, which has no fire alarm companion");
            var both = new[]
            {
                new ViolationCitation { profileId = "fire-alarm", reference = "10.6.5", text = "x" },
                new ViolationCitation { profileId = "nec-2023", reference = "760.41", text = "y" }
            };
            t.Equal("nec-2023", ViolationCitations.FindAny(both, ProfileSupplements.CitationIds(map, "nec-2023"))?.profileId, "the code's own citation wins over a companion's");
            t.IsTrue(ViolationCitations.FindAny(both, null) == null, "no ids, no citation");

            t.Begin("companion profiles: the student's code with its companions");
            var composite = new CompositeCodeProfile(byId["nec-2023"], new[] { byId["fire-alarm"], byId["utility-safety"] });
            t.Equal("nec-2023", composite.ProfileId, "identity is the main code's");
            t.Equal(byId["nec-2023"].Tables, composite.Tables, "tables are the main code's");
            t.Equal(byId["nec-2023"].Region, composite.Region, "region is the main code's");
            t.Equal(byId["nec-2023"].ArticleCount + byId["fire-alarm"].ArticleCount + byId["utility-safety"].ArticleCount, composite.ArticleCount, "entries add up");
            t.IsTrue(composite.GetArticle("10.6.5") != null && composite.GetArticle("10.6.5").conceptId == ConceptIds.LowVoltageAndFireAlarm, "a fire alarm entry can be looked up");
            t.IsTrue(composite.GetArticle("1910.269(l)(2)") != null, "a line work entry can be looked up");
            t.IsTrue(composite.GetArticle("250.24") != null, "an NEC entry can still be looked up");
            t.IsTrue(composite.GetArticle("no-such-entry") == null, "an unknown reference finds nothing");

            var found = composite.Search("battery", 6);
            t.IsTrue(found.Any(a => a.conceptId == ConceptIds.LowVoltageAndFireAlarm), "searching finds fire alarm entries");
            t.IsTrue(composite.Search("battery", 3).Count <= 3, "search respects the limit");
            t.IsTrue(composite.GetAllReferences().Contains("10.6.5") && composite.GetAllReferences().Contains(byId["nec-2023"].GetAllReferences()[0]), "the reference list covers both");
            t.Equal(composite.GetAllReferences().Count, composite.GetAllReferences().Distinct().Count(), "no reference is listed twice");

            t.IsTrue(composite.CitationMatches("10.6.5", "10.6.5"), "a student citing the expected fire alarm entry is right");
            t.IsTrue(!composite.CitationMatches("250.24", "10.6.5"), "citing a different entry is not");
            t.IsTrue(composite.CitationMatches("1910.269(l)(2)", "1910.269(l)(2)"), "the same for a line work entry");

            t.Begin("companion profiles: manifest checks");
            var manifest = new CodeProfileManifest
            {
                id = "x", displayName = "X", edition = "1", region = "US", units = "imperial", artSet = "north-america", reviewStatus = "draft",
                license = loaded.First().manifest.license, supplements = new[] { "x" }
            };
            var articles = loaded.First(p => p.folder == "fire-alarm").articles;
            var terminology = loaded.First(p => p.folder == "fire-alarm").terminology;
            t.IsTrue(CodeProfileValidator.Validate(manifest, articles, null, terminology).Any(e => e.Contains("supplement itself")), "a profile cannot supplement itself");
            manifest.supplements = new[] { "bad id!" };
            t.IsTrue(CodeProfileValidator.Validate(manifest, articles, null, terminology).Any(e => e.Contains("invalid profile id")), "a bad id is rejected");
            manifest.supplements = new[] { "nec-2023" };
            t.Equal(0, CodeProfileValidator.Validate(manifest, articles, null, terminology).Count, "a valid companion passes");
        }
    }
}
