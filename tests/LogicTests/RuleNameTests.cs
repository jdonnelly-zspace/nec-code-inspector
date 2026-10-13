using System.Linq;
using NECInspector.Codes;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The Master level asks the student to name the rule in their own words. These tests check that naming works the way a student would,
    /// against the real NEC, CEC and BS 7671 entries, and that scoring (full, same skill, wrong) follows.
    /// </summary>
    public static class RuleNameTests
    {
        public static void Run(TestContext t)
        {
            var nec = ProfileFiles.Load(TestContext.RepoRoot(), "nec-2023").Build();
            var articles = RuleNameMatcher.ArticlesOf(nec);

            t.Begin("rule naming: students name a rule in their own words");
            Names(t, articles, "GFCI protection bathroom", "210.8(A)(1)");
            Names(t, articles, "bathroom receptacles need GFCI", "210.8(A)(1)");
            Names(t, articles, "gfci in the garage", "210.8(A)(2)");
            Names(t, articles, "ground fault protection kitchen counter", "210.8(A)(6)");
            Names(t, articles, "kitchen small appliance circuits", "210.11(C)(1)");
            Names(t, articles, "working space width", "110.26(A)(2)");
            Names(t, articles, "Working Space - Height", "110.26(A)(3)");
            Names(t, articles, "GFCI Protection - Laundry Areas", "210.8(A)(11)");

            t.Begin("rule naming: it does not accept the wrong rule or too little");
            NotNames(t, articles, "GFCI protection bathroom", "210.8(A)(6)");
            NotNames(t, articles, "working space width", "110.26(A)(3)");
            NotNames(t, articles, "terminals", "110.14");
            NotNames(t, articles, "wiring", "110.14(A)");
            NotNames(t, articles, "banana smoothie", "210.8(A)(1)");
            NotNames(t, articles, "", "210.8(A)(1)");
            t.IsTrue(!RuleNameMatcher.Names(articles, "GFCI protection bathroom", ""), "no expected rule, no match");

            t.Begin("rule naming: general words still show the skill");
            t.Equal(ConceptIds.ShockProtection, RuleNameMatcher.ConceptOf(articles, "gfci"), "gfci is about shock protection");
            t.Equal(ConceptIds.WorkingSpaceAccess, RuleNameMatcher.ConceptOf(articles, "working space clearance"), "working space is about working space and access");
            t.Equal("", RuleNameMatcher.ConceptOf(articles, "banana smoothie"), "nonsense is about no skill");
            t.Equal("", RuleNameMatcher.ConceptOf(articles, ""), "empty is about no skill");

            t.Begin("rule naming: every entry can be named by its own title");
            int unnamed = 0, wrongSkill = 0;
            var wrongRefs = new System.Collections.Generic.List<string>();
            var unnamedRefs = new System.Collections.Generic.List<string>();
            foreach (var a in articles)
            {
                if (!RuleNameMatcher.Names(articles, a.title, a.reference)) { unnamed++; unnamedRefs.Add(a.reference + " '" + a.title + "'"); }
                string concept = RuleNameMatcher.ConceptOf(articles, a.title);
                if (!string.IsNullOrEmpty(a.conceptId) && concept != a.conceptId) { wrongSkill++; wrongRefs.Add(a.reference + " '" + a.title + "' -> '" + concept + "' not '" + a.conceptId + "'"); }
            }
            t.Equal(0, unnamed, $"{unnamed} of {articles.Count} entries are not named by their own title: {string.Join("; ", unnamedRefs)}");
            t.IsTrue(wrongSkill * 20 <= articles.Count, $"typing an entry's title points to its skill ({wrongSkill} of {articles.Count} do not: {string.Join("; ", wrongRefs.Take(8))})");

            t.Begin("rule naming: it feeds the code profile and the skill credit");
            t.IsTrue(nec.CitationMatches("GFCI protection bathroom", "210.8(A)(1)"), "the profile accepts the rule named in words");
            t.IsTrue(nec.CitationMatches("210.8(A)(1)", "210.8(A)(1)"), "the profile still accepts a reference number");
            t.IsTrue(nec.CitationMatches("Art. 210.8(A)", "210.8(A)(1)"), "and a parent reference");
            t.IsTrue(!nec.CitationMatches("GFCI protection kitchen", "210.8(A)(1)"), "but not another rule in words");
            t.Near(1.0, CitationCredit.Outcome(nec, ConceptIds.ShockProtection, "GFCI protection bathroom", "210.8(A)(1)"), "the rule named in words earns full credit");
            t.Near(0.75, CitationCredit.Outcome(nec, ConceptIds.ShockProtection, "GFCI protection kitchen", "210.8(A)(1)"), "another rule of the same skill earns three quarters");
            t.Near(0.75, CitationCredit.Outcome(nec, ConceptIds.ShockProtection, "gfci", "210.8(A)(1)"), "a general word about the skill earns three quarters");
            t.Near(0.5, CitationCredit.Outcome(nec, ConceptIds.ShockProtection, "grounding electrode", "210.8(A)(1)"), "a rule of another skill earns half");
            t.Near(0.5, CitationCredit.Outcome(nec, ConceptIds.ShockProtection, "banana smoothie", "210.8(A)(1)"), "nonsense earns half, as a wrong citation does");

            t.Begin("rule naming: the other codes name rules too");
            foreach (string id in new[] { "cec", "bs7671" })
            {
                var profile = ProfileFiles.Load(TestContext.RepoRoot(), id).Build();
                var list = RuleNameMatcher.ArticlesOf(profile);
                int missed = list.Count(a => !RuleNameMatcher.Names(list, a.title, a.reference));
                t.Equal(0, missed, $"{id}: every entry is named by its own title ({missed} of {list.Count} are not)");
            }
        }

        private static void Names(TestContext t, System.Collections.Generic.List<CodeArticle> articles, string text, string reference)
        {
            t.IsTrue(RuleNameMatcher.Names(articles, text, reference), $"'{text}' names {reference}");
        }

        private static void NotNames(TestContext t, System.Collections.Generic.List<CodeArticle> articles, string text, string reference)
        {
            t.IsTrue(!RuleNameMatcher.Names(articles, text, reference), $"'{text}' does not name {reference}");
        }
    }
}
