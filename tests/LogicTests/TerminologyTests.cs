using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    public static class TerminologyTests
    {
        public static void Run(TestContext t)
        {
            NecLabels(t);
            OtherCodesLabelThemselves(t);
            FormatTokens(t);
            JsonFileMatchesDefaults(t);
            ProfileFallback(t);
        }

        private static CodeTerminology Uk()
        {
            return new CodeTerminology
            {
                codeName = "BS 7671",
                referencePrefix = "Reg. ",
                referenceNoun = "Regulation",
                sectionLabel = "Part",
                terms = new[] { new TermEntry { key = "grounding", text = "earthing" } }
            };
        }

        private static void NecLabels(TestContext t)
        {
            t.Begin("terminology NEC labels");
            var nec = CodeTerminology.CreateNecDefaults();

            t.Equal("Art. 250.24(A)(1)", nec.ReferenceLabel("250.24(A)(1)"), "reference label");
            t.Equal("General practice", nec.ReferenceLabel("General Practice"), "general practice rules are not shown as an article");
            t.Equal("General practice", nec.ReferenceLabel("general practice"), "general practice match ignores case");
            t.Equal("", nec.ReferenceLabel(""), "empty reference");
            t.Equal("", nec.ReferenceLabel(null), "null reference");

            t.Equal("NEC Citations", nec.CitationsHeading, "citations heading");
            t.Equal("NEC Chapters", nec.SectionsHeading, "sections heading");
            t.Equal("Select NEC Article...", nec.SelectReferencePrompt, "dropdown prompt");
            t.Equal("Chapter 2", nec.SectionName(2), "section name");
        }

        private static void OtherCodesLabelThemselves(TestContext t)
        {
            t.Begin("terminology other codes");
            var uk = Uk();

            t.Equal("Reg. 411.3", uk.ReferenceLabel("411.3"), "reference label uses the code's prefix");
            t.Equal("BS 7671 Citations", uk.CitationsHeading, "citations heading uses the code name");
            t.Equal("BS 7671 Parts", uk.SectionsHeading, "sections heading uses the code's section name");
            t.Equal("Select BS 7671 Regulation...", uk.SelectReferencePrompt, "dropdown prompt uses the code's noun");
            t.Equal("Part 4", uk.SectionName(4), "section name");
        }

        private static void FormatTokens(TestContext t)
        {
            t.Begin("terminology format");
            var nec = CodeTerminology.CreateNecDefaults();
            var uk = Uk();

            t.Equal("NEC uses grounding", nec.Format("{code} uses {term:grounding}"), "NEC tokens");
            t.Equal("BS 7671 uses earthing", uk.Format("{code} uses {term:grounding}"), "UK tokens");
            t.Equal("ground-rod stays", uk.Format("{term:ground-rod} stays"), "unknown term falls back to its key");
            t.Equal("no tokens here", nec.Format("no tokens here"), "text without tokens is unchanged");
            t.Equal("", nec.Format(""), "empty text");
            t.IsTrue(nec.Format(null) == null, "null text");

            t.Equal("earthing", uk.Term("grounding"), "term lookup");
            t.Equal("fallback", uk.Term("missing", "fallback"), "term fallback argument");
            t.Equal("missing", uk.Term("missing"), "term fallback is the key");
        }

        private static void JsonFileMatchesDefaults(TestContext t)
        {
            t.Begin("terminology json == defaults");

            var fromFile = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).terminology;
            var defaults = CodeTerminology.CreateNecDefaults();

            t.IsTrue(fromFile != null, "the NEC profile has a terminology.json that parses");
            if (fromFile == null) return;

            t.Equal(defaults.codeName, fromFile.codeName, "codeName");
            t.Equal(defaults.referencePrefix, fromFile.referencePrefix, "referencePrefix");
            t.Equal(defaults.referenceNoun, fromFile.referenceNoun, "referenceNoun");
            t.Equal(defaults.sectionLabel, fromFile.sectionLabel, "sectionLabel");
            t.Equal(defaults.generalPracticeLabel, fromFile.generalPracticeLabel, "generalPracticeLabel");

            t.Equal(defaults.terms.Length, fromFile.terms.Length, "term count");
            for (int i = 0; i < defaults.terms.Length && i < fromFile.terms.Length; i++)
            {
                t.Equal(defaults.terms[i].key, fromFile.terms[i].key, $"term {i} key");
                t.Equal(defaults.terms[i].text, fromFile.terms[i].text, $"term {i} text");
            }
        }

        private static void ProfileFallback(TestContext t)
        {
            t.Begin("terminology profile fallback");

            CodeProfiles.SetActive(null);
            t.Equal("NEC", CodeProfiles.Terminology.codeName, "falls back to NEC labels when no profile is loaded");

            var fake = new FakeTerminologyProfile(Uk());
            CodeProfiles.SetActive(fake);
            t.Equal("BS 7671", CodeProfiles.Terminology.codeName, "follows the active profile");
            CodeProfiles.Clear(fake);
            t.Equal("NEC", CodeProfiles.Terminology.codeName, "clearing the profile restores the fallback");
        }

        private class FakeTerminologyProfile : ICodeProfile
        {
            public FakeTerminologyProfile(CodeTerminology terminology) { Terminology = terminology; }

            public string ProfileId => "uk";
            public string DisplayName => "UK";
            public string Edition => "test";
            public string Region => "UK";
            public string ArtSet => "north-america";
            public string ReviewStatus => "draft";
            public bool IsLoaded => true;
            public int ArticleCount => 0;
            public bool HasOwnTables => true;
            public ElectricalTables Tables => ElectricalTables.CreateNecDefaults();
            public CodeTerminology Terminology { get; }
            public CodeArticle GetArticle(string reference) => null;
            public System.Collections.Generic.List<CodeArticle> Search(string query, int maxResults = 20) => new System.Collections.Generic.List<CodeArticle>();
            public System.Collections.Generic.List<string> GetAllReferences() => new System.Collections.Generic.List<string>();
            public System.Collections.Generic.List<string> GetAllDisplayStrings() => new System.Collections.Generic.List<string>();
            public bool CitationMatches(string cited, string expected) => cited == expected;
        }
    }
}
