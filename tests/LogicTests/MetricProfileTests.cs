using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;
using NECInspector.PanelSandbox;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// A synthetic 230 V, mm2, non-North-American code (tests/fixtures/codes/demo-metric) run through the engine.
    /// It checks the acceptance test in TODO.md at the level of the engine: a code with different voltages,
    /// units, conductor names, terminology and rule settings works with data files only. The fixture is never
    /// shipped; it is not any real code.
    /// </summary>
    public static class MetricProfileTests
    {
        public static void Run(TestContext t)
        {
            var fixture = ProfileFiles.LoadFixture(TestContext.RepoRoot(), "demo-metric");
            t.IsTrue(fixture != null, "the demo-metric fixture exists");
            if (fixture == null) return;

            FixtureIsValid(t, fixture);

            var profile = fixture.Build();
            CodeProfiles.SetActive(profile);
            try
            {
                UnitsAndVoltages(t, profile);
                Terminology(t, profile);
                ComplianceWithMetricData(t, profile);
                NothingLeaksFromNec(t, profile);
            }
            finally
            {
                CodeProfiles.SetActive(null);
            }

            PickerAndScenarios(t, profile);
        }

        private static void FixtureIsValid(TestContext t, LoadedProfile fixture)
        {
            t.Begin("metric fixture files");

            foreach (string error in CodeProfileValidator.Validate(fixture.manifest, fixture.articles, fixture.tables, fixture.terminology))
                t.IsTrue(false, $"demo-metric: {error}");
            t.IsTrue(fixture.tables.GetMaxAmps(fixture.tables.defaultConductor) > 0, "the default conductor is in the metric table");
            t.IsTrue(!ProfileFiles.LoadAll(TestContext.RepoRoot()).Any(p => p.folder == "demo-metric"), "the fixture is not in the shipped codes folder");
        }

        private static void UnitsAndVoltages(TestContext t, DataCodeProfile profile)
        {
            t.Begin("metric units and voltages");

            var tables = CodeProfiles.Tables;
            t.Equal("m²", tables.areaUnitLabel, "the area unit comes from the profile");
            t.IsTrue(profile.HasOwnTables, "the profile has its own tables, so the sandbox would count as evidence");
            t.Equal(3200f, LoadCalculator.CalculateGeneralLighting(100f), "lighting load per m² comes from the profile");
            t.Equal(10f, LoadCalculator.ConvertVAToAmps(2300f), "volt-amps to amps uses the 230 V service voltage");
            t.Equal(3680f, new BreakerData { ampRating = 16, poleCount = 1 }.GetLoadVA(tables), "a 16 A breaker is 3680 VA at 230 V");
            t.Equal(3680f, new BreakerData { ampRating = 16, poleCount = 2 }.GetLoadVA(tables), "a two-pole breaker uses the profile's two-pole voltage");
            t.Equal(24, tables.GetMaxAmps("2.5 mm²"), "conductors are looked up by their mm² names");
            t.Equal(0, tables.GetMaxAmps("12 AWG"), "an AWG name is not in a metric table");
            t.Equal(0.005f, tables.GetWireWidth("2.5 mm²", 0.1f), "wire widths come from the table");
            t.Equal(2, tables.generalLoadDemandTiers.Length, "the demand tiers are the profile's two, not the NEC's three");
        }

        private static void Terminology(TestContext t, DataCodeProfile profile)
        {
            t.Begin("metric terminology");

            var terms = CodeProfiles.Terminology;
            t.Equal("Reg. 411.3.3", terms.ReferenceLabel("411.3.3"), "the reference prefix comes from the profile");
            t.Equal("earthing and MCB", terms.Format("{term:grounding} and {term:breaker}"), "terms use the code's words");
            t.Equal("Consumer unit", terms.Format("{Term:panel}"), "a capitalised token starts with a capital");
            t.Equal("the code", terms.Format("{code}"), "the code token is neutral, never the code name");
            t.Equal("no-such-key stays", terms.Format("{term:no-such-key} stays"), "an unknown key falls back to the key");
        }

        private static PanelRuleInput Input(params (string name, int amps, int poles, string gauge)[] circuits)
        {
            var input = new PanelRuleInput { totalAmps = 80, totalSlots = 12, requiredCircuits = new RequiredCircuit[0] };
            foreach (var c in circuits)
            {
                input.breakers.Add(new PanelBreakerState
                {
                    circuitName = c.name, data = new BreakerData { ampRating = c.amps, poleCount = c.poles },
                    hasWire = true, wireGauge = c.gauge, side = BusSide.Left
                });
                input.slotUse.Add(new SlotUse { circuitName = c.name, poleCount = c.poles, slotsOccupied = c.poles });
            }
            return input;
        }

        private static void ComplianceWithMetricData(TestContext t, DataCodeProfile profile)
        {
            t.Begin("compliance rules with metric data");

            var tables = CodeProfiles.Tables;
            var terms = CodeProfiles.Terminology;

            var clean = ComplianceRules.RunAll(tables, Input(("Lights", 6, 1, "1.5 mm²"), ("Sockets", 20, 1, "2.5 mm²")), terms);
            t.Equal(10, clean.Count, "nine built-in rules run (the profile switched off the balance rule) plus the code's own margin rule");
            t.IsTrue(clean.All(r => r.ruleId != "RULE-05"), "the balance rule is the one missing");
            t.IsTrue(clean.All(r => r.passed), "a design that fits the metric table passes");
            t.Equal("433.1", clean.First(r => r.ruleId == "RULE-01").codeReference, "citations come from the profile");

            var over = ComplianceRules.RunAll(tables, Input(("Sockets", 32, 1, "2.5 mm²")), terms);
            var rule1 = over.First(r => r.ruleId == "RULE-01");
            t.IsTrue(!rule1.passed, "a 32 A breaker on 2.5 mm² (24 A) fails");
            t.Equal("Sockets: 32A MCB exceeds 2.5 mm² capacity (24A).", rule1.message, "the message uses the code's words and conductor names");
            t.Equal("MCB/Conductor Match", rule1.ruleName, "the rule name uses the code's word for breaker");

            t.Equal("RCD Protection", over.First(r => r.ruleId == "RULE-03").ruleName, "the shock protection rule says RCD");
            t.Equal("AFDD Protection", over.First(r => r.ruleId == "RULE-04").ruleName, "the arc-fault rule says AFDD");
            t.Equal("Consumer unit Spaces", over.First(r => r.ruleId == "RULE-09").ruleName, "the panel rule says consumer unit");

            var needsRcd = Input(("Bathroom", 16, 1, "1.5 mm²"));
            needsRcd.requiredCircuits = new[] { new RequiredCircuit { circuitName = "Bathroom", ampsRequired = 16, requiresGFCI = true } };
            var rcd = ComplianceRules.RunAll(tables, needsRcd, terms).First(r => r.ruleId == "RULE-03");
            t.Equal("Missing RCD protection: Bathroom.", rcd.message, "a missing RCD is named as one");

            var rcdBreaker = new BreakerData { ampRating = 16, isGFCI = true };
            t.IsTrue(rcdBreaker.GetDisplayName(terms).Contains("RCD") && !rcdBreaker.GetDisplayName(terms).Contains("GFCI"), "a breaker label uses the code's word for the protection");
            t.IsTrue(rcdBreaker.DisplayName.Contains("GFCI"), "the default breaker label keeps the NEC word");

            // Without terminology the rules fall back to NEC words, as before
            var nec = ComplianceRules.RunAll(tables, needsRcd).First(r => r.ruleId == "RULE-03");
            t.Equal("Missing GFCI protection: Bathroom.", nec.message, "no terminology means NEC words");
        }

        // What a metric code must NOT inherit from the NEC when its files are complete
        private static void NothingLeaksFromNec(TestContext t, DataCodeProfile profile)
        {
            t.Begin("no NEC values leak into a complete metric profile");

            var tables = CodeProfiles.Tables;
            var nec = ElectricalTables.CreateNecDefaults();
            t.IsTrue(tables.singlePoleVoltage != nec.singlePoleVoltage, "voltage is not the NEC's");
            t.IsTrue(tables.conductorSizes.All(c => !c.name.Contains("AWG")), "no AWG conductor sizes came in");
            t.IsTrue(tables.standardServiceSizes.SequenceEqual(new[] { 60, 80, 100 }), "service sizes are the profile's");
            t.IsTrue(tables.complianceRules.All(r => r.reference != "240.4" && r.reference != "310.14"), "rule citations are the profile's");
            t.IsTrue(profile.GetArticle("250.24(A)(1)") == null, "NEC articles are not reachable");
        }

        private static void PickerAndScenarios(TestContext t, DataCodeProfile profile)
        {
            t.Begin("a metric code in the picker and scenario list");

            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).Build();
            var choices = RegionChoices.Build(new ICodeProfile[] { nec, profile }, CodeProfileIds.Nec, id => id == CodeProfileIds.Nec ? 6 : 6, 6);
            var metric = choices.Single(c => c.profileId == "demo-metric");

            t.Equal("United Kingdom", metric.regionName, "the region is shown by name");
            t.IsTrue(!metric.artAvailable, "its art set has no art yet");
            t.Equal(0, metric.availableScenarios, "no scenarios are offered without art");
            t.IsTrue(ScenarioListMessage.For(profile, 0).Contains("no scene art"), "the menu explains the empty list");
            t.Equal("metric", metric.units, "it is shown as metric");
            t.IsTrue(metric.SandboxText == "", "its own tables make the sandbox available");
        }
    }
}
