using System.Collections.Generic;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    public static class CodeProfileTests
    {
        public static void Run(TestContext t)
        {
            ActiveProfileRegistry(t);
            CitationLookup(t);
        }

        private static void ActiveProfileRegistry(TestContext t)
        {
            t.Begin("code profile registry");

            CodeProfiles.SetActive(null);
            t.IsTrue(CodeProfiles.Active == null, "no active profile by default");
            t.Equal(CodeProfileIds.Nec, CodeProfiles.ActiveId, "ActiveId falls back to the NEC");
            t.Near(240, CodeProfiles.Tables.serviceVoltage, "Tables fall back to the NEC defaults");

            var fake = new FakeProfile("cec", 230f);
            CodeProfiles.SetActive(fake);
            t.Equal("cec", CodeProfiles.ActiveId, "ActiveId follows the active profile");
            t.Near(230, CodeProfiles.Tables.serviceVoltage, "Tables follow the active profile");

            CodeProfiles.Clear(new FakeProfile("other", 0f));
            t.Equal("cec", CodeProfiles.ActiveId, "clearing a different profile leaves the active one");

            CodeProfiles.Clear(fake);
            t.Equal(CodeProfileIds.Nec, CodeProfiles.ActiveId, "clearing the active profile restores the fallback");
        }

        private static void CitationLookup(TestContext t)
        {
            t.Begin("violation citations");

            var citations = new[]
            {
                new ViolationCitation { profileId = "nec", reference = "210.8(A)(1)", text = "nec text" },
                new ViolationCitation { profileId = "cec", reference = "26-700", text = "cec text" }
            };

            t.Equal("210.8(A)(1)", ViolationCitations.Find(citations, "nec").reference, "finds the NEC citation");
            t.Equal("26-700", ViolationCitations.Find(citations, "cec").reference, "finds the CEC citation");
            t.IsTrue(ViolationCitations.Find(citations, "bs7671") == null, "no citation means the violation does not apply");
            t.IsTrue(ViolationCitations.Find(null, "nec") == null, "null citations");
            t.IsTrue(ViolationCitations.Find(citations, null) == null, "null profile id");
            t.IsTrue(ViolationCitations.Find(citations, "") == null, "empty profile id");
            t.IsTrue(ViolationCitations.Find(new ViolationCitation[] { null }, "nec") == null, "null entries are skipped");
        }

        private class FakeProfile : ICodeProfile
        {
            private readonly ElectricalTables _tables;

            public FakeProfile(string id, float serviceVoltage)
            {
                ProfileId = id;
                _tables = ElectricalTables.CreateNecDefaults();
                _tables.serviceVoltage = serviceVoltage;
            }

            public string ProfileId { get; }
            public string DisplayName => ProfileId;
            public string Edition => "test";
            public string Region => "US";
            public string ArtSet => "north-america";
            public string Units => "imperial";
            public string ReviewStatus => "draft";
            public bool IsLoaded => true;
            public int ArticleCount => 0;
            public bool HasOwnTables => true;
            public ElectricalTables Tables => _tables;
            public CodeTerminology Terminology => CodeTerminology.CreateNecDefaults();
            public CodeArticle GetArticle(string reference) => null;
            public List<CodeArticle> Search(string query, int maxResults = 20) => new List<CodeArticle>();
            public List<string> GetAllReferences() => new List<string>();
            public List<string> GetAllDisplayStrings() => new List<string>();
            public bool CitationMatches(string cited, string expected) => cited == expected;
        }
    }
}
