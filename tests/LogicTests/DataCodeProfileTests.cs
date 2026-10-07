using System.Linq;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Behavior of the data-driven profile, using a small in-memory code that is nothing like the NEC
    /// (hyphenated numbering, lowercase subrules) plus the real NEC data.
    /// </summary>
    public static class DataCodeProfileTests
    {
        public static void Run(TestContext t)
        {
            SmallCode(t);
            RealNecData(t);
        }

        private static DataCodeProfile MakeSmallCode(ElectricalTables tables)
        {
            var manifest = new CodeProfileManifest { id = "demo", displayName = "Demo Code", edition = "2030", reviewStatus = "draft" };
            var terminology = new CodeTerminology
            {
                codeName = "DEMO", referencePrefix = "Rule ", referenceNoun = "Rule", sectionLabel = "Section",
                terms = new TermEntry[0]
            };
            var articles = new[]
            {
                new CodeArticleData { reference = "26-700", title = "Sinks and GFCI", text = "Ground-fault protection near sinks.", section = 26, keywords = new[] { "gfci", "sink" }, related = new[] { "26-712" } },
                new CodeArticleData { reference = "26-712", title = "Dwelling receptacles", text = "Receptacle spacing in dwellings.", section = 26 },
                new CodeArticleData { reference = "26-712(d)(iii)", title = "Counter receptacles", text = "Counter spacing.", section = 26, isNewInEdition = true },
                new CodeArticleData { reference = "10-700", title = "Grounding electrodes", text = "Rods and pipes.", section = 10 }
            };
            return new DataCodeProfile(manifest, articles, tables, terminology);
        }

        private static void SmallCode(TestContext t)
        {
            t.Begin("data profile: small code");
            var profile = MakeSmallCode(null);

            t.Equal("demo", profile.ProfileId, "id");
            t.Equal("Demo Code", profile.DisplayName, "display name");
            t.Equal(4, profile.ArticleCount, "article count");
            t.IsTrue(profile.IsLoaded, "loaded");
            t.IsTrue(!profile.HasOwnTables, "no tables.json means no own tables");
            t.Near(240, profile.Tables.serviceVoltage, "tables fall back to NEC values");

            t.Equal("Rule 26-700", profile.GetArticle("26-700").referenceLabel, "label uses the profile's prefix");
            t.Equal("Rule 26-700 - Sinks and GFCI", profile.GetArticle("26-700").DisplayString, "display string");

            t.IsTrue(profile.GetArticle("26-712(D)(III)") != null, "lookup ignores letter case");
            t.Equal("26-712", profile.GetArticle("26-712").reference, "exact reference wins over a longer one");
            t.Equal("26-712(d)(iii)", profile.GetArticle("26-712(d)").reference, "a parent reference leads to the first child");
            t.IsTrue(profile.GetArticle("26-71") == null, "a partial number is not a parent");
            t.IsTrue(profile.GetArticle("26") != null, "a section number leads into the section");
            t.IsTrue(profile.GetArticle("") == null && profile.GetArticle(null) == null, "empty lookups");

            t.Equal(3, profile.GetSection(26).Count, "section lookup");
            t.Equal(1, profile.GetNewInEditionArticles().Count, "new-in-edition lookup");

            var refs = profile.GetAllReferences();
            t.IsTrue(refs.SequenceEqual(refs.OrderBy(r => r)), "references are sorted");
            t.Equal(4, profile.GetAllDisplayStrings().Count, "display strings");

            t.Equal("26-700", profile.Search("gfci").First().reference, "keyword search");
            t.Equal("26-700", profile.Search("26-700").First().reference, "reference search ranks first");
            t.Equal(0, profile.Search("").Count, "empty search");
            t.Equal(1, profile.Search("counter", 1).Count, "result limit");

            t.IsTrue(profile.CitationMatches("Rule 26-712", "26-712(d)(iii)"), "a parent citation matches");
            t.IsTrue(!profile.CitationMatches("26-71", "26-712"), "a partial number does not match");

            var withTables = MakeSmallCode(ElectricalTables.CreateNecDefaults());
            t.IsTrue(withTables.HasOwnTables, "tables.json means own tables");
        }

        private static void RealNecData(TestContext t)
        {
            t.Begin("data profile: NEC data");
            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec);
            var profile = nec.Build();

            t.IsTrue(profile.HasOwnTables, "NEC has its own tables");
            t.Equal("Art. 250.24(A)(1)", profile.GetArticle("250.24(A)(1)").referenceLabel, "NEC reference label");
            t.IsTrue(profile.GetArticle("250.24") != null, "parent lookup finds the first child");
            t.Equal("250.52(A)(5)", profile.GetArticle("250.52(A)(5)").reference, "exact lookup");
            t.IsTrue(profile.GetArticle("250.5") == null, "250.5 is not a parent of 250.50");
            t.IsTrue(profile.Search("ground rod").Count > 0, "search finds the ground rod entry");
            t.IsTrue(profile.GetNewInEditionArticles().Count > 0, "some entries are flagged new in the edition");
        }
    }
}
