using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;
using NECInspector.Data;
using NECInspector.PanelSandbox;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// A code profile chooses its own panel compliance rule set in tables.json: which kinds of check run, in what
    /// order, how many of each, with its own ids, names, citations and parameters.
    /// </summary>
    public static class ComplianceRuleSetTests
    {
        public static void Run(TestContext t)
        {
            LegacyFilesStillWork(t);
            ValidatorCatchesBadRuleSets(t);
            RuleSetsAreData(t);
            Parameters(t);
            RealProfiles(t);
        }

        private static ComplianceRuleConfig Rule(string id, string kind, string reference = "X", float maxRatio = 0, float margin = 0,
            float maxImbalance = 0, string protection = null, string name = null)
        {
            return new ComplianceRuleConfig
            {
                ruleId = id, kind = kind, reference = reference, conceptId = ConceptIds.EquipmentInstallation, enabled = true,
                maxRatio = maxRatio, margin = margin, maxImbalance = maxImbalance, protection = protection, name = name
            };
        }

        private static ElectricalTables WithRules(params ComplianceRuleConfig[] rules)
        {
            var tables = ElectricalTables.CreateNecDefaults();
            tables.complianceRules = rules;
            return tables;
        }

        private static PanelBreakerState State(string name, int amps, string gauge, int poles = 1, BusSide side = BusSide.Left)
        {
            return new PanelBreakerState
            {
                circuitName = name, data = new BreakerData { ampRating = amps, poleCount = poles }, hasWire = true, wireGauge = gauge, side = side
            };
        }

        private static PanelRuleInput Input(int mainAmps, params PanelBreakerState[] breakers)
        {
            return new PanelRuleInput { totalAmps = mainAmps, totalSlots = 40, requiredCircuits = new RequiredCircuit[0], breakers = breakers.ToList() };
        }

        private static void LegacyFilesStillWork(TestContext t)
        {
            t.Begin("rule kinds for files written before kinds existed");

            t.Equal("breaker-conductor-match", ComplianceRuleKinds.Infer("RULE-01"), "RULE-01 is the breaker and conductor check");
            t.Equal("protection-required", ComplianceRuleKinds.Infer("RULE-03"), "RULE-03 is a protection rule");
            t.Equal("gfci", ComplianceRuleKinds.InferProtection("RULE-03"), "RULE-03 checks GFCI");
            t.Equal("afci", ComplianceRuleKinds.InferProtection("RULE-04"), "RULE-04 checks AFCI");
            t.Equal("", ComplianceRuleKinds.Infer("MY-RULE"), "a custom id implies nothing");

            var legacy = new ElectricalTables { complianceRules = new[] { new ComplianceRuleConfig { ruleId = "RULE-04", reference = "x" } } };
            legacy.EnsureComplete();
            t.Equal("protection-required", legacy.complianceRules[0].kind, "a rule with only an id gets its kind");
            t.Equal("afci", legacy.complianceRules[0].protection, "and its protection");
            t.Equal(0, CodeProfileValidator.ValidateRules(legacy).Count, "so it validates");
        }

        private static void ValidatorCatchesBadRuleSets(TestContext t)
        {
            t.Begin("rule set validator");

            Func<ComplianceRuleConfig, string> first = r => CodeProfileValidator.ValidateRules(WithRules(r)).FirstOrDefault() ?? "";

            t.Equal(0, CodeProfileValidator.ValidateRules(WithRules(Rule("DM-1", "double-tap"))).Count, "a custom id with a known kind is valid");
            t.IsTrue(first(Rule("DM-1", "")).Contains("needs a kind"), "a custom id with no kind is rejected");
            t.IsTrue(first(Rule("DM-1", "teleport")).Contains("unknown kind"), "an unknown kind is rejected");
            t.IsTrue(first(Rule("DM-1", "protection-required")).Contains("protection"), "a protection rule with no protection is rejected");
            t.IsTrue(first(Rule("DM-1", "protection-required", protection: "rcd")).Contains("protection"), "a protection rule with an unknown protection is rejected");
            t.IsTrue(first(Rule("DM-1", "double-tap", margin: -1)).Contains("margin"), "a negative margin is rejected");
            t.IsTrue(first(Rule("DM-1", "double-tap", maxRatio: -1)).Contains("maxRatio"), "a negative ratio is rejected");
            t.IsTrue(first(Rule("DM-1", "load-balance", maxImbalance: 2)).Contains("maxImbalance"), "an imbalance above 1 is rejected");
            t.IsTrue(first(new ComplianceRuleConfig { ruleId = "", kind = "double-tap" }).Contains("no ruleId"), "a rule without an id is rejected");

            var dup = WithRules(Rule("DM-1", "double-tap"), Rule("DM-1", "panel-spaces"));
            t.IsTrue(CodeProfileValidator.ValidateRules(dup).Any(e => e.Contains("duplicate")), "a duplicate id is rejected");

            var badConcept = Rule("DM-1", "double-tap"); badConcept.conceptId = "not-a-skill";
            t.IsTrue(first(badConcept).Contains("conceptId"), "an unknown skill is rejected");
        }

        private static void RuleSetsAreData(TestContext t)
        {
            t.Begin("a profile's rule set");

            var input = Input(100, State("A", 20, "12 AWG"));

            var custom = WithRules(Rule("X-WIRES", "wire-connections", "9.9", name: "Every {term:breaker} wired"), Rule("X-SPACES", "panel-spaces", "9.8"));
            var results = ComplianceRules.RunAll(custom, input);
            t.Equal(2, results.Count, "only the rules the profile lists run");
            t.IsTrue(results.Select(r => r.ruleId).SequenceEqual(new[] { "X-WIRES", "X-SPACES" }), "they run in the profile's order, with the profile's ids");
            t.Equal("Every breaker wired", results[0].ruleName, "a custom name uses terminology tokens");
            t.Equal("9.9", results[0].codeReference, "the citation is the profile's");
            t.Equal("Panel Spaces", results[1].ruleName, "a rule with no name gets its kind's default name");

            var reversed = WithRules(Rule("X-SPACES", "panel-spaces"), Rule("X-WIRES", "wire-connections"));
            t.IsTrue(ComplianceRules.RunAll(reversed, input).Select(r => r.ruleId).SequenceEqual(new[] { "X-SPACES", "X-WIRES" }), "the order is the profile's");

            var twice = WithRules(Rule("MAIN-A", "main-breaker-sizing"), Rule("MAIN-B", "main-breaker-sizing", margin: 1.25f));
            var load = Input(100, State("A", 20, "12 AWG", 2), State("B", 20, "12 AWG", 2));   // 2 x 4800 VA = 40 A at 240 V
            t.Equal(2, ComplianceRules.RunAll(twice, load).Count, "the same kind can run twice under different ids");

            var disabled = Rule("X-OFF", "double-tap"); disabled.enabled = false;
            t.Equal(0, ComplianceRules.RunAll(WithRules(disabled), input).Count, "a disabled rule does not run");

            t.IsTrue(ComplianceRules.Run(Rule("X-NEW", "teleport"), ElectricalTables.CreateNecDefaults(), input) == null, "an unknown kind is skipped, not a crash");
            t.Equal(10, ComplianceRules.RunAll(new ElectricalTables(), input).Count, "a profile with no rule set runs the NEC's ten");
        }

        private static void Parameters(TestContext t)
        {
            t.Begin("rule parameters");

            // 32 A breaker on a 20 A conductor: fails at the default ratio, passes if the code allows 1.6 times
            var input = Input(100, State("A", 32, "12 AWG"));
            var strict = WithRules(Rule("R", "breaker-conductor-match"));
            var loose = WithRules(Rule("R", "breaker-conductor-match", maxRatio: 1.6f));
            t.IsTrue(!ComplianceRules.RunAll(strict, input)[0].passed, "a 32 A breaker on 20 A fails at the default ratio");
            t.IsTrue(ComplianceRules.RunAll(loose, input)[0].passed, "it passes when the rule allows 1.6 times the ampacity");
            var ampacity = ComplianceRules.RunAll(WithRules(Rule("R", "conductor-ampacity", maxRatio: 1.6f)), input)[0];
            t.IsTrue(ampacity.passed, "the conductor ampacity check takes the same ratio");

            // 40 A of load: a 50 A main passes with no margin, fails with a 1.5 margin (needs 60 A)
            var load = Input(50, State("A", 20, "12 AWG", 2), State("B", 20, "12 AWG", 2));
            t.IsTrue(ComplianceRules.RunAll(WithRules(Rule("M", "main-breaker-sizing")), load)[0].passed, "a 50 A main covers 40 A with no margin");
            t.IsTrue(!ComplianceRules.RunAll(WithRules(Rule("M", "main-breaker-sizing", margin: 1.5f)), load)[0].passed, "but not with a 1.5 margin");
            t.IsTrue(ComplianceRules.RunAll(WithRules(Rule("M", "main-breaker-sizing", margin: 1.25f)), load)[0].passed, "and passes at a 1.25 margin (50 A needed)");

            // Imbalance: 20 A against 15 A is about 14%; the table allows 20%, the rule can tighten it
            var balanced = Input(100, State("A", 20, "12 AWG", 1, BusSide.Left), State("B", 15, "14 AWG", 1, BusSide.Right));
            t.IsTrue(ComplianceRules.RunAll(WithRules(Rule("B", "load-balance")), balanced)[0].passed, "the table's 20% allows a 14% imbalance");
            t.IsTrue(!ComplianceRules.RunAll(WithRules(Rule("B", "load-balance", maxImbalance: 0.05f)), balanced)[0].passed, "a rule's own 5% limit does not");

            // Two protection rules, one per kind, each with its own id
            var circuits = new[] { new RequiredCircuit { circuitName = "A", ampsRequired = 20, requiresGFCI = true, requiresAFCI = true } };
            var protection = new PanelRuleInput { totalAmps = 100, totalSlots = 40, requiredCircuits = circuits, breakers = new List<PanelBreakerState> { State("A", 20, "12 AWG") } };
            var both = ComplianceRules.RunAll(WithRules(Rule("P-GF", "protection-required", protection: "gfci"), Rule("P-AF", "protection-required", protection: "afci")), protection);
            t.IsTrue(both.Count == 2 && both.All(r => !r.passed), "a circuit needing both has both protections reported missing");
            t.IsTrue(both[0].message.Contains("GFCI") && both[1].message.Contains("AFCI"), "each rule names its own protection");
        }

        private static void RealProfiles(TestContext t)
        {
            t.Begin("the shipped rule sets");

            foreach (var p in ProfileFiles.LoadAll(TestContext.RepoRoot()).Where(p => p.tables != null))
            {
                p.tables.EnsureComplete();
                t.Equal(0, CodeProfileValidator.ValidateRules(p.tables).Count, $"Codes/{p.folder}: the rule set is valid");
                t.IsTrue(p.tables.complianceRules.All(r => !string.IsNullOrEmpty(ComplianceRuleKinds.Resolve(r))), $"Codes/{p.folder}: every rule has a kind");
            }

            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).tables;
            t.IsTrue(nec.complianceRules.All(r => !string.IsNullOrEmpty(r.kind)), "the NEC file names every rule's kind explicitly");

            // The metric fixture adds a rule of its own (a main with a margin) and drops one
            var demo = ProfileFiles.LoadFixture(TestContext.RepoRoot(), "demo-metric").tables;
            t.IsTrue(demo.complianceRules.Any(r => r.ruleId == "DM-MAIN-25"), "the fixture has a rule of its own");
            t.IsTrue(demo.GetRuleConfig("DM-MAIN-25").conceptId == ConceptIds.LoadCalculation, "which gives evidence for a skill");
        }
    }
}
