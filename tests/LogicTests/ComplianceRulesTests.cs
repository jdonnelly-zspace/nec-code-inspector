using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;
using NECInspector.PanelSandbox;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The 10 panel design compliance rules, run on plain data with no Unity objects.
    /// </summary>
    public static class ComplianceRulesTests
    {
        public static void Run(TestContext t)
        {
            BreakerAndConductor(t);
            RequiredAndProtection(t);
            LoadAndSpaces(t);
            DoubleTapAndWiring(t);
            RunAllAndConfig(t);
        }

        private static ElectricalTables Tables => ElectricalTables.CreateNecDefaults();

        private static BreakerData Breaker(int amps, int poles = 1, bool gfci = false, bool afci = false, bool dual = false)
        {
            return new BreakerData { ampRating = amps, poleCount = poles, isGFCI = gfci, isAFCI = afci, isDualFunction = dual };
        }

        private static PanelBreakerState State(string name, BreakerData data, string gauge = "12 AWG",
            BusSide? side = BusSide.Left, bool wired = true)
        {
            return new PanelBreakerState
            {
                circuitName = name, data = data, hasWire = wired, wireGauge = wired ? gauge : null,
                wireLinked = true, side = side
            };
        }

        private static RequiredCircuit Required(string name, int amps, bool gfci = false, bool afci = false, bool isRequired = true)
        {
            return new RequiredCircuit { circuitName = name, ampsRequired = amps, requiresGFCI = gfci, requiresAFCI = afci, isRequired = isRequired };
        }

        private static List<PanelBreakerState> Many(params PanelBreakerState[] states) => states.ToList();

        private static void BreakerAndConductor(TestContext t)
        {
            t.Begin("rules 1 and 8: breaker and conductor");

            var ok = ComplianceRules.BreakerConductorMatch(Tables, Many(State("Kitchen", Breaker(20), "12 AWG")));
            t.IsTrue(ok.passed && ok.ruleId == "RULE-01" && ok.codeReference == "240.4", "a 20 A breaker on 12 AWG passes rule 1");

            var over = ComplianceRules.BreakerConductorMatch(Tables, Many(State("Kitchen", Breaker(30), "12 AWG")));
            t.IsTrue(!over.passed, "a 30 A breaker on 12 AWG fails rule 1");
            t.Equal("Kitchen: 30A breaker exceeds 12 AWG capacity (20A).", over.message, "rule 1 names the circuit, rating and capacity");

            var unknown = ComplianceRules.BreakerConductorMatch(Tables, Many(State("X", Breaker(15), "99 AWG")));
            t.IsTrue(!unknown.passed, "an unknown conductor size has no capacity, so it fails");

            t.IsTrue(ComplianceRules.BreakerConductorMatch(Tables, Many(State("Spare", Breaker(60), wired: false))).passed,
                "a breaker with no wire is not judged by rule 1 (rule 10 covers it)");

            var first = ComplianceRules.BreakerConductorMatch(Tables, Many(State("A", Breaker(30), "12 AWG"), State("B", Breaker(50), "12 AWG")));
            t.IsTrue(first.message.StartsWith("A:"), "rule 1 reports the first offender");

            var amp = ComplianceRules.ConductorAmpacity(Tables, Many(State("Dryer", Breaker(30, 2), "12 AWG")));
            t.IsTrue(!amp.passed && amp.ruleId == "RULE-08" && amp.codeReference == "310.14", "rule 8 fails when the conductor cannot carry the rating");
            t.Equal("Dryer: 12 AWG insufficient for 30A breaker.", amp.message, "rule 8 message");

            var good = ComplianceRules.ConductorAmpacity(Tables, Many(State("Dryer", Breaker(30, 2), "10 AWG")));
            t.IsTrue(good.passed, "rule 8 passes on 10 AWG for 30 A");

            var unlinked = State("Range", Breaker(20), "12 AWG");
            unlinked.wireLinked = false;
            t.IsTrue(!ComplianceRules.ConductorAmpacity(Tables, Many(unlinked)).passed,
                "rule 8 fails when the wire does not point back at a breaker");
        }

        private static void RequiredAndProtection(TestContext t)
        {
            t.Begin("rules 2 to 4: required circuits and protection");

            var required = new[] { Required("Kitchen 1", 20), Required("Kitchen 2", 20), Required("Spare", 15, isRequired: false) };
            var breakers = Many(State("Kitchen 1", Breaker(20)), State("Kitchen 2", Breaker(20)));
            t.IsTrue(ComplianceRules.RequiredCircuits(Tables, required, breakers).passed, "all required circuits present passes rule 2 (optional ones are ignored)");

            var missing = ComplianceRules.RequiredCircuits(Tables, required, Many(State("Kitchen 1", Breaker(20))));
            t.IsTrue(!missing.passed && missing.message == "Missing required circuits: Kitchen 2.", "a missing circuit is named");

            var small = ComplianceRules.RequiredCircuits(Tables, new[] { Required("Range", 40) }, Many(State("Range", Breaker(30))));
            t.IsTrue(!small.passed, "a breaker below the required rating does not count");
            t.IsTrue(ComplianceRules.RequiredCircuits(Tables, new[] { Required("Range", 40) }, Many(State("Range", Breaker(50)))).passed,
                "a breaker above the required rating counts");
            t.IsTrue(ComplianceRules.RequiredCircuits(Tables, null, breakers).passed, "no required circuits passes");

            var gfciNeeded = new[] { Required("Bath", 20, gfci: true) };
            var plain = ComplianceRules.GfciProtection(Tables, gfciNeeded, Many(State("Bath", Breaker(20))));
            t.IsTrue(!plain.passed && plain.message == "Missing GFCI protection: Bath.", "a plain breaker on a GFCI circuit fails rule 3");
            t.IsTrue(ComplianceRules.GfciProtection(Tables, gfciNeeded, Many(State("Bath", Breaker(20, gfci: true)))).passed, "a GFCI breaker passes rule 3");
            t.IsTrue(ComplianceRules.GfciProtection(Tables, gfciNeeded, Many(State("Bath", Breaker(20, dual: true)))).passed, "a dual-function breaker passes rule 3");
            t.IsTrue(ComplianceRules.GfciProtection(Tables, gfciNeeded, Many()).passed, "a circuit with no breaker yet is left to rule 2");
            t.IsTrue(!ComplianceRules.GfciProtection(Tables, gfciNeeded, Many(State("Bath", Breaker(20, afci: true)))).passed, "an AFCI-only breaker does not satisfy GFCI");

            var afciNeeded = new[] { Required("Bedroom", 15, afci: true) };
            var noAfci = ComplianceRules.AfciProtection(Tables, afciNeeded, Many(State("Bedroom", Breaker(15))));
            t.IsTrue(!noAfci.passed && noAfci.message == "Missing AFCI protection: Bedroom.", "a plain breaker on an AFCI circuit fails rule 4");
            t.IsTrue(ComplianceRules.AfciProtection(Tables, afciNeeded, Many(State("Bedroom", Breaker(15, afci: true)))).passed, "an AFCI breaker passes rule 4");
            t.IsTrue(ComplianceRules.AfciProtection(Tables, afciNeeded, Many(State("Bedroom", Breaker(15, dual: true)))).passed, "a dual-function breaker passes rule 4");
            t.IsTrue(!ComplianceRules.AfciProtection(Tables, afciNeeded, Many(State("Bedroom", Breaker(15, gfci: true)))).passed, "a GFCI-only breaker does not satisfy AFCI");
        }

        private static void LoadAndSpaces(TestContext t)
        {
            t.Begin("rules 5, 6 and 9: load and spaces");
            var tables = Tables;

            t.Equal(1800f, Breaker(15).GetLoadVA(tables), "a 15 A single-pole breaker is 1800 VA");
            t.Equal(7200f, Breaker(30, 2).GetLoadVA(tables), "a 30 A double-pole breaker is 7200 VA");

            t.IsTrue(ComplianceRules.LoadBalance(tables, Many()).passed, "no load passes rule 5");
            t.IsTrue(ComplianceRules.LoadBalance(tables, Many(State("A", Breaker(20), side: null))).passed, "breakers outside a slot add no load");
            t.IsTrue(ComplianceRules.LoadBalance(tables, Many(State("A", Breaker(20), side: BusSide.Left), State("B", Breaker(20), side: BusSide.Right))).passed,
                "equal loads on each side pass");

            var skewed = ComplianceRules.LoadBalance(tables, Many(State("A", Breaker(20), side: BusSide.Left), State("B", Breaker(20), side: BusSide.Left)));
            t.IsTrue(!skewed.passed && skewed.message.Contains("imbalance"), "everything on one side fails");

            // 20 A left, 15 A right: 2400 VA against 1800 VA is about 14% imbalance, within the default 20%
            t.IsTrue(ComplianceRules.LoadBalance(tables, Many(State("A", Breaker(20), side: BusSide.Left), State("B", Breaker(15), side: BusSide.Right))).passed,
                "a small imbalance within the limit passes");

            var strict = Tables;
            strict.loadBalanceMaxImbalance = 0.05f;
            t.IsTrue(!ComplianceRules.LoadBalance(strict, Many(State("A", Breaker(20), side: BusSide.Left), State("B", Breaker(15), side: BusSide.Right))).passed,
                "the imbalance limit comes from the tables");

            var load = Many(State("A", Breaker(20)), State("B", Breaker(20)));   // 4800 VA = 20 A at 240 V
            t.IsTrue(ComplianceRules.MainBreakerSizing(tables, 100, load).passed, "a 100 A main carries 20 A of load");
            var tiny = ComplianceRules.MainBreakerSizing(tables, 15, load);
            t.IsTrue(!tiny.passed && tiny.message.Contains("undersized"), "a 15 A main is undersized for 20 A");
            t.IsTrue(ComplianceRules.MainBreakerSizing(tables, 20, load).passed, "a main equal to the load passes");

            var spaces = Many(State("A", Breaker(20)), State("B", Breaker(30, 2)));
            t.IsTrue(ComplianceRules.PanelSpaces(tables, 3, spaces).passed, "3 spaces used of 3 passes");
            var full = ComplianceRules.PanelSpaces(tables, 2, spaces);
            t.IsTrue(!full.passed && full.message == "Panel exceeded: 3 spaces used, 2 available.", "too many spaces fails and says how many");
        }

        private static void DoubleTapAndWiring(TestContext t)
        {
            t.Begin("rules 7 and 10: slots and wiring");

            var tables = Tables;
            t.IsTrue(ComplianceRules.DoubleTap(tables, new List<SlotUse>
            {
                new SlotUse { circuitName = "A", poleCount = 1, slotsOccupied = 1 },
                new SlotUse { circuitName = "Dryer", poleCount = 2, slotsOccupied = 2 }
            }).passed, "a double-pole breaker in two slots is valid");

            var bad = ComplianceRules.DoubleTap(tables, new List<SlotUse> { new SlotUse { circuitName = "A", poleCount = 1, slotsOccupied = 2 } });
            t.IsTrue(!bad.passed && bad.message == "Breaker 'A' occupies 2 slots but is only 1-pole.", "a breaker in more slots than poles fails");

            t.IsTrue(ComplianceRules.WireConnections(tables, Many(State("A", Breaker(20)))).passed, "wired breakers pass rule 10");
            var unwired = ComplianceRules.WireConnections(tables, Many(State("A", Breaker(20), wired: false), State(null, Breaker(15), wired: false)));
            t.IsTrue(!unwired.passed, "an unwired breaker fails rule 10");
            t.Equal($"Breakers without wire connections: A, {Breaker(15).DisplayName}.", unwired.message, "an unnamed breaker is listed by its display name");
        }

        private static void RunAllAndConfig(TestContext t)
        {
            t.Begin("running all rules");

            var input = new PanelRuleInput
            {
                totalAmps = 200, totalSlots = 40,
                requiredCircuits = new[] { Required("Kitchen", 20) },
                breakers = Many(State("Kitchen", Breaker(20), "12 AWG", BusSide.Left), State("Bath", Breaker(20), "12 AWG", BusSide.Right)),
                slotUse = new List<SlotUse>
                {
                    new SlotUse { circuitName = "Kitchen", poleCount = 1, slotsOccupied = 1 },
                    new SlotUse { circuitName = "Bath", poleCount = 1, slotsOccupied = 1 }
                }
            };

            var all = ComplianceRules.RunAll(Tables, input);
            t.Equal(10, all.Count, "all ten rules run with default tables");
            t.IsTrue(all.Select(r => r.ruleId).SequenceEqual(Enumerable.Range(1, 10).Select(i => $"RULE-{i:00}")), "rules run in order");
            t.IsTrue(all.All(r => r.passed), "a clean design passes every rule");

            // A different code: one rule off, one citation changed, a different conductor table
            var other = Tables;
            other.complianceRules.First(r => r.ruleId == "RULE-05").enabled = false;
            other.complianceRules.First(r => r.ruleId == "RULE-01").reference = "14-104";
            other.conductorSizes.First(c => c.name == "12 AWG").maxAmps = 15;

            var results = ComplianceRules.RunAll(other, input);
            t.Equal(9, results.Count, "a disabled rule does not run");
            t.IsTrue(results.All(r => r.ruleId != "RULE-05"), "the disabled rule is the one missing");
            var rule1 = results.First(r => r.ruleId == "RULE-01");
            t.Equal("14-104", rule1.codeReference, "the citation comes from the profile");
            t.IsTrue(!rule1.passed, "the conductor table comes from the profile: 20 A on a 15 A conductor fails");
            t.IsTrue(results.First(r => r.ruleId == "RULE-08").passed == false, "rule 8 uses the same table");

            var noConfig = Tables;
            noConfig.complianceRules = null;
            t.Equal(10, ComplianceRules.RunAll(noConfig, input).Count, "tables with no rule settings run every rule");
            t.Equal("240.4", ComplianceRules.RunAll(noConfig, input)[0].codeReference, "the built-in citation is used when none is configured");
        }
    }
}
