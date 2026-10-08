using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;

namespace NECInspector.PanelSandbox
{
    /// <summary>One placed breaker as the rules see it: plain data copied from the scene objects.</summary>
    public class PanelBreakerState
    {
        public string circuitName;
        public BreakerData data;
        public bool hasWire;
        public string wireGauge;        // the connected wire's conductor size; null when there is no wire
        public bool wireLinked = true;  // the wire points back at a breaker that has data
        public BusSide? side;           // null when the breaker is not in a slot
    }

    /// <summary>How many slots one breaker occupies (a double-pole breaker takes two).</summary>
    public class SlotUse
    {
        public string circuitName;
        public int poleCount;
        public int slotsOccupied;
    }

    /// <summary>Everything the compliance rules read about a panel design.</summary>
    public class PanelRuleInput
    {
        public int totalAmps;
        public int totalSlots;
        public RequiredCircuit[] requiredCircuits;
        public List<PanelBreakerState> breakers = new List<PanelBreakerState>();
        public List<SlotUse> slotUse = new List<SlotUse>();
    }

    /// <summary>
    /// The 10 panel design compliance rules, with no Unity types, so they are tested without Unity.
    /// Which rules run, the citation shown for each, and the numeric limits come from the tables
    /// passed in (the active code profile's). Names and messages use the code's own words through
    /// terminology tokens ({term:breaker}, {term:shock-protection-device}, ...), so a code that says
    /// MCB, RCD or consumer unit reads correctly. `ComplianceChecker` reads the scene objects
    /// and calls this.
    /// </summary>
    public static class ComplianceRules
    {
        private static readonly CodeTerminology DefaultTerms = CodeTerminology.CreateNecDefaults();

        // Fills the tokens with the code's words; NEC words when no terminology is given
        private static string Fmt(CodeTerminology terms, string text) => (terms ?? DefaultTerms).Format(text);

        private static ComplianceResult Result(CodeTerminology terms, string ruleId, string ruleName, string reference, bool passed, string message)
        {
            return new ComplianceResult(ruleId, Fmt(terms, ruleName), reference, passed, Fmt(terms, message));
        }

        /// <summary>Run every enabled rule, in rule order.</summary>
        public static List<ComplianceResult> RunAll(ElectricalTables tables, PanelRuleInput input, CodeTerminology terms = null)
        {
            var checks = new (string ruleId, Func<ComplianceResult> run)[]
            {
                ("RULE-01", () => BreakerConductorMatch(tables, input.breakers, terms)),
                ("RULE-02", () => RequiredCircuits(tables, input.requiredCircuits, input.breakers, terms)),
                ("RULE-03", () => GfciProtection(tables, input.requiredCircuits, input.breakers, terms)),
                ("RULE-04", () => AfciProtection(tables, input.requiredCircuits, input.breakers, terms)),
                ("RULE-05", () => LoadBalance(tables, input.breakers, terms)),
                ("RULE-06", () => MainBreakerSizing(tables, input.totalAmps, input.breakers, terms)),
                ("RULE-07", () => DoubleTap(tables, input.slotUse, terms)),
                ("RULE-08", () => ConductorAmpacity(tables, input.breakers, terms)),
                ("RULE-09", () => PanelSpaces(tables, input.totalSlots, input.breakers, terms)),
                ("RULE-10", () => WireConnections(tables, input.breakers, terms))
            };

            var results = new List<ComplianceResult>();
            foreach (var check in checks)
            {
                if (IsEnabled(tables, check.ruleId))
                    results.Add(check.run());
            }

            return results;
        }

        /// <summary>Citation for a rule from the profile's tables, or the built-in NEC citation if none is configured.</summary>
        private static string Ref(ElectricalTables tables, string ruleId, string fallback)
        {
            var config = tables.GetRuleConfig(ruleId);
            return config != null && !string.IsNullOrEmpty(config.reference) ? config.reference : fallback;
        }

        private static bool IsEnabled(ElectricalTables tables, string ruleId)
        {
            var config = tables.GetRuleConfig(ruleId);
            return config == null || config.enabled;
        }

        /// <summary>Rule 1: a breaker's rating must not exceed its wire's ampacity.</summary>
        public static ComplianceResult BreakerConductorMatch(ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire) continue;

                int wireMax = tables.GetMaxAmps(breaker.wireGauge);
                if (breaker.data.ampRating > wireMax)
                {
                    return Result(terms, "RULE-01", "{Term:breaker}/Conductor Match", Ref(tables, "RULE-01", "240.4"),
                        false,
                        $"{breaker.circuitName}: {breaker.data.ampRating}A {{term:breaker}} exceeds {breaker.wireGauge} capacity ({wireMax}A).");
                }
            }

            return Result(terms, "RULE-01", "{Term:breaker}/Conductor Match", Ref(tables, "RULE-01", "240.4"),
                true, "All {term:breaker}s match their conductor ampacity.");
        }

        /// <summary>Rule 2: every required branch circuit is present at its required rating or higher.</summary>
        public static ComplianceResult RequiredCircuits(ElectricalTables tables, RequiredCircuit[] required, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            var missing = new List<string>();
            foreach (var req in required ?? new RequiredCircuit[0])
            {
                if (!req.isRequired) continue;

                bool found = breakers.Any(b =>
                    b.circuitName == req.circuitName &&
                    b.data.ampRating >= req.ampsRequired);

                if (!found)
                    missing.Add(req.circuitName);
            }

            if (missing.Count > 0)
            {
                return Result(terms, "RULE-02", "Required Branch Circuits", Ref(tables, "RULE-02", "210.11"),
                    false,
                    $"Missing required circuits: {string.Join(", ", missing)}.");
            }

            return Result(terms, "RULE-02", "Required Branch Circuits", Ref(tables, "RULE-02", "210.11"),
                true, "All required branch circuits are present.");
        }

        /// <summary>Rule 3: circuits that need ground-fault protection use a GFCI or dual-function breaker.</summary>
        public static ComplianceResult GfciProtection(ElectricalTables tables, RequiredCircuit[] required, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            var violations = new List<string>();
            foreach (var req in required ?? new RequiredCircuit[0])
            {
                if (!req.requiresGFCI) continue;

                var breaker = breakers.FirstOrDefault(b => b.circuitName == req.circuitName);
                if (breaker != null && !breaker.data.SatisfiesGFCI)
                    violations.Add(req.circuitName);
            }

            if (violations.Count > 0)
            {
                return Result(terms, "RULE-03", "{term:shock-protection-device} Protection", Ref(tables, "RULE-03", "210.8"),
                    false,
                    $"Missing {{term:shock-protection-device}} protection: {string.Join(", ", violations)}.");
            }

            return Result(terms, "RULE-03", "{term:shock-protection-device} Protection", Ref(tables, "RULE-03", "210.8"),
                true, "All required circuits have {term:shock-protection-device} protection.");
        }

        /// <summary>Rule 4: circuits that need arc-fault protection use an AFCI or dual-function breaker.</summary>
        public static ComplianceResult AfciProtection(ElectricalTables tables, RequiredCircuit[] required, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            var violations = new List<string>();
            foreach (var req in required ?? new RequiredCircuit[0])
            {
                if (!req.requiresAFCI) continue;

                var breaker = breakers.FirstOrDefault(b => b.circuitName == req.circuitName);
                if (breaker != null && !breaker.data.SatisfiesAFCI)
                    violations.Add(req.circuitName);
            }

            if (violations.Count > 0)
            {
                return Result(terms, "RULE-04", "{term:arc-fault-device} Protection", Ref(tables, "RULE-04", "210.12"),
                    false,
                    $"Missing {{term:arc-fault-device}} protection: {string.Join(", ", violations)}.");
            }

            return Result(terms, "RULE-04", "{term:arc-fault-device} Protection", Ref(tables, "RULE-04", "210.12"),
                true, "All required circuits have {term:arc-fault-device} protection.");
        }

        /// <summary>Rule 5: load is balanced between the left and right bus (general practice).</summary>
        public static ComplianceResult LoadBalance(ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            float leftLoad = 0f, rightLoad = 0f;

            foreach (var breaker in breakers)
            {
                if (breaker.side == null) continue;
                float load = breaker.data != null ? breaker.data.GetLoadVA(tables) : 0f;

                if (breaker.side == BusSide.Left)
                    leftLoad += load;
                else
                    rightLoad += load;
            }

            float totalLoad = leftLoad + rightLoad;
            if (totalLoad <= 0f)
            {
                return Result(terms, "RULE-05", "Load Balance", Ref(tables, "RULE-05", "General Practice"),
                    true, "No load to balance.");
            }

            float maxImbalance = tables.loadBalanceMaxImbalance;
            float imbalance = Math.Abs(leftLoad - rightLoad) / totalLoad;
            bool balanced = imbalance <= maxImbalance;

            return Result(terms, "RULE-05", "Load Balance", Ref(tables, "RULE-05", "General Practice"),
                balanced,
                balanced
                    ? $"Load is balanced ({imbalance:P0} imbalance)."
                    : $"Load imbalance is {imbalance:P0} (max {maxImbalance:P0}). Left: {leftLoad:N0} VA, Right: {rightLoad:N0} VA.");
        }

        /// <summary>Rule 6: the main breaker is sized for the total of the breaker loads.</summary>
        public static ComplianceResult MainBreakerSizing(ElectricalTables tables, int mainAmps, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            float totalLoadVA = 0f;
            foreach (var breaker in breakers)
                totalLoadVA += breaker.data != null ? breaker.data.GetLoadVA(tables) : 0f;

            float loadAmps = LoadCalculator.ConvertVAToAmps(totalLoadVA, tables.serviceVoltage);
            bool adequate = mainAmps >= loadAmps;

            return Result(terms, "RULE-06", "Main {Term:breaker} Sizing", Ref(tables, "RULE-06", "230.79"),
                adequate,
                adequate
                    ? $"Main {{term:breaker}} ({mainAmps}A) adequate for {loadAmps:N0}A calculated load."
                    : $"Main {{term:breaker}} ({mainAmps}A) undersized for {loadAmps:N0}A calculated load.");
        }

        /// <summary>Rule 7: no breaker takes more slots than it has poles (a double-pole breaker takes two).</summary>
        public static ComplianceResult DoubleTap(ElectricalTables tables, List<SlotUse> slotUse, CodeTerminology terms = null)
        {
            foreach (var use in slotUse)
            {
                if (use.slotsOccupied > use.poleCount)
                {
                    return Result(terms, "RULE-07", "No Double-Tapped {Term:breaker}s", Ref(tables, "RULE-07", "110.14"),
                        false,
                        $"{{Term:breaker}} '{use.circuitName}' occupies {use.slotsOccupied} slots but is only {use.poleCount}-pole.");
                }
            }

            return Result(terms, "RULE-07", "No Double-Tapped {Term:breaker}s", Ref(tables, "RULE-07", "110.14"),
                true, "No double-tapped {term:breaker}s found.");
        }

        /// <summary>Rule 8: each wire's conductor size carries its breaker's rating.</summary>
        public static ComplianceResult ConductorAmpacity(ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire) continue;

                bool valid = breaker.wireLinked && breaker.data.ampRating <= tables.GetMaxAmps(breaker.wireGauge);
                if (!valid)
                {
                    return Result(terms, "RULE-08", "Conductor Ampacity", Ref(tables, "RULE-08", "310.14"),
                        false,
                        $"{breaker.circuitName}: {breaker.wireGauge} insufficient for {breaker.data.ampRating}A {{term:breaker}}.");
                }
            }

            return Result(terms, "RULE-08", "Conductor Ampacity", Ref(tables, "RULE-08", "310.14"),
                true, "All conductor ampacities match {term:breaker} ratings.");
        }

        /// <summary>Rule 9: the breakers fit in the panel's spaces.</summary>
        public static ComplianceResult PanelSpaces(ElectricalTables tables, int totalSlots, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            int usedSlots = 0;
            foreach (var breaker in breakers)
                usedSlots += breaker.data.poleCount;

            bool withinLimit = usedSlots <= totalSlots;

            return Result(terms, "RULE-09", "{Term:panel} Spaces", Ref(tables, "RULE-09", "408.54"),
                withinLimit,
                withinLimit
                    ? $"Using {usedSlots} of {totalSlots} {{term:panel}} spaces."
                    : $"{{Term:panel}} exceeded: {usedSlots} spaces used, {totalSlots} available.");
        }

        /// <summary>Rule 10: every placed breaker has a wire connected.</summary>
        public static ComplianceResult WireConnections(ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms = null)
        {
            var unwired = new List<string>();
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire)
                    unwired.Add(breaker.circuitName ?? breaker.data.GetDisplayName(terms));
            }

            if (unwired.Count > 0)
            {
                return Result(terms, "RULE-10", "Wire Connections", Ref(tables, "RULE-10", "General Practice"),
                    false,
                    $"{{Term:breaker}}s without wire connections: {string.Join(", ", unwired)}.");
            }

            return Result(terms, "RULE-10", "Wire Connections", Ref(tables, "RULE-10", "General Practice"),
                true, "All {term:breaker}s have wire connections.");
        }
    }
}
