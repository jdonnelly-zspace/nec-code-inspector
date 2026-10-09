using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Concept-aware scoring: citing a different entry of the violation's own skill earns partial credit
    /// (docs/SKILL_SCORING.md), and every entry carries the skill it belongs to.
    /// </summary>
    public static class CitationCreditTests
    {
        public static void Run(TestContext t)
        {
            Outcomes(t);
            UntaggedProfiles(t);
            TagsAreComplete(t);
            CitedEntriesMatchTheirViolation(t);
            ValidatorRejectsUnknownSkill(t);
        }

        private static DataCodeProfile Nec => ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).Build();

        private static void Outcomes(TestContext t)
        {
            t.Begin("same-skill citation credit");

            t.IsTrue(SkillOutcomes.Missed < SkillOutcomes.FoundWrongCitation
                     && SkillOutcomes.FoundWrongCitation < SkillOutcomes.FoundSameSkillCitation
                     && SkillOutcomes.FoundSameSkillCitation < SkillOutcomes.FoundCorrectCitation,
                "credit rises: missed, wrong, same skill, correct");
            t.Near(0.75, SkillOutcomes.FoundSameSkillCitation, "a same-skill citation earns three quarters");

            var nec = Nec;
            string shock = ConceptIds.ShockProtection;

            t.Near(1, CitationCredit.Outcome(nec, shock, "210.8(A)(1)", "210.8(A)(1)"), "the expected citation earns full credit");
            t.Near(0.75, CitationCredit.Outcome(nec, shock, "210.8(A)(6)", "210.8(A)(1)"), "another GFCI entry (kitchens for bathrooms) earns three quarters");
            t.Near(0.75, CitationCredit.Outcome(nec, shock, "210.8(A)(6)(a)", "210.8(A)(1)"), "a sub-item of a same-skill entry earns three quarters too");
            t.Near(0.5, CitationCredit.Outcome(nec, shock, "210.12(A)", "210.8(A)(1)"), "an entry of another skill earns half");
            t.Near(0.5, CitationCredit.Outcome(nec, shock, "250.50", "210.8(A)(1)"), "a grounding entry for a GFCI violation earns half");
            t.Near(0.5, CitationCredit.Outcome(nec, shock, "999.9", "210.8(A)(1)"), "a reference that is not in the code earns half");
            t.Near(0.5, CitationCredit.Outcome(nec, shock, "", "210.8(A)(1)"), "no citation earns half");
            t.Near(0.5, CitationCredit.Outcome(nec, "", "210.8(A)(6)", "210.8(A)(1)"), "a violation with no skill gives no same-skill credit");

            t.Equal(ConceptIds.ArcFaultProtection, CitationCredit.ConceptOf(nec, "210.12(A)"), "the skill of an entry");
            t.Equal("", CitationCredit.ConceptOf(nec, "999.9"), "an unknown reference has no skill");
            t.Equal("", CitationCredit.ConceptOf(null, "210.12(A)"), "no profile, no skill");
            t.Near(1, CitationCredit.Outcome(null, shock, "210.8(A)(1)", "210.8(A)(1)"), "without a profile the default matcher still gives full credit");
            t.Near(0.5, CitationCredit.Outcome(null, shock, "210.8(A)(6)", "210.8(A)(1)"), "without a profile there is no same-skill credit");

            // The same rule under the Canadian code
            var cec = ProfileFiles.Load(TestContext.RepoRoot(), "cec").Build();
            t.IsTrue(CitationCredit.Outcome(cec, ConceptIds.BranchCircuitRequirements, "26-722", "26-722(d)(iii)") >= 0.75f, "CEC: the dwelling receptacle rule for the counter rule earns at least three quarters");
            t.Near(0.5, CitationCredit.Outcome(cec, ConceptIds.ShockProtection, "26-722", "26-704"), "CEC: a branch circuit rule for a GFCI violation earns half");
        }

        private static void UntaggedProfiles(TestContext t)
        {
            t.Begin("profiles without skill tags");

            var manifest = new CodeProfileManifest { id = "plain", displayName = "Plain", edition = "1", region = "US", units = "imperial", artSet = "north-america", reviewStatus = "draft" };
            var terminology = new CodeTerminology { codeName = "PL", terms = new TermEntry[0] };
            var articles = new[]
            {
                new CodeArticleData { reference = "1.1", title = "A", text = "a", section = 1 },
                new CodeArticleData { reference = "1.2", title = "B", text = "b", section = 1 }
            };
            var profile = new DataCodeProfile(manifest, articles, null, terminology);

            t.Near(0.5, CitationCredit.Outcome(profile, ConceptIds.ShockProtection, "1.2", "1.1"), "a code with no tags gives the old half credit");
            t.Near(1, CitationCredit.Outcome(profile, ConceptIds.ShockProtection, "1.1", "1.1"), "and full credit for the right citation");
        }

        private static void TagsAreComplete(TestContext t)
        {
            t.Begin("every entry has a skill");

            foreach (var p in ProfileFiles.LoadAll(TestContext.RepoRoot()))
            {
                foreach (var a in p.articles)
                {
                    t.IsTrue(!string.IsNullOrEmpty(a.conceptId), $"Codes/{p.folder}: {a.reference} has a conceptId");
                    t.IsTrue(string.IsNullOrEmpty(a.conceptId) || ConceptIds.IsKnown(a.conceptId), $"Codes/{p.folder}: {a.reference} names a known skill");
                }
            }
        }

        // A violation's citation must belong to the violation's own skill, or the credit rule would contradict the content
        private static void CitedEntriesMatchTheirViolation(TestContext t)
        {
            t.Begin("cited entries belong to their violation's skill");

            var options = new JsonSerializerOptions { IncludeFields = true };
            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot()).ToDictionary(p => p.folder, p => p.Build());
            string dir = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Scenarios");

            foreach (string file in Directory.GetFiles(dir, "*.json"))
            {
                foreach (var v in JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options).violations)
                {
                    foreach (var c in v.citations)
                    {
                        if (!profiles.TryGetValue(c.profileId, out var profile)) continue;
                        t.Equal(v.conceptId, CitationCredit.ConceptOf(profile, c.reference),
                            $"{v.violationId}: {c.profileId} {c.reference} belongs to the violation's skill");
                    }
                }
            }
        }

        private static void ValidatorRejectsUnknownSkill(TestContext t)
        {
            t.Begin("article skill validation");

            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec);
            var articles = nec.articles.Select(a => new CodeArticleData
            {
                reference = a.reference, title = a.title, text = a.text, section = a.section,
                keywords = a.keywords, related = a.related, conceptId = a.conceptId
            }).ToArray();
            t.Equal(0, CodeProfileValidator.Validate(nec.manifest, articles, nec.tables, nec.terminology).Count, "the shipped NEC tags are valid");

            articles[0].conceptId = "not-a-skill";
            t.IsTrue(CodeProfileValidator.Validate(nec.manifest, articles, nec.tables, nec.terminology).Any(e => e.Contains("unknown conceptId")),
                "an unknown skill tag is rejected");
        }
    }
}
