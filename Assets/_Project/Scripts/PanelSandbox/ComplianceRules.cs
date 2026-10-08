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
    /// The panel design compliance checks, with no Unity types, so they are tested without Unity.
    /// A code profile chooses its rule set in tables.json: which kinds of check run (<see cref="ComplianceRuleKinds"/>),
    /// in what order, how many of each, and each rule's id, name, citation and parameters. The numeric limits
    /// come from the same tables. Names and messages use the code's own words through terminology tokens
    /// ({term:breaker}, {term:shock-protection-device}, ...), so a code that says MCB, RCD or consumer unit reads
    /// correctly. `ComplianceChecker` reads the scene objects and calls this.
    /// </summary>
    public static class ComplianceRules
    {
        private static readonly CodeTerminology DefaultTerms = CodeTerminology.CreateNecDefaults();

        // Fills the tokens with the code's words; NEC words when no terminology is given
        private static string Fmt(CodeTerminology terms, string text) => (terms ?? DefaultTerms).Format(text);

        private static ComplianceResult Result(CodeTerminology terms, ComplianceRuleConfig config, string defaultId, string defaultName,
            string fallbackReference, bool passed, string message)
        {
            string id = !string.IsNullOrEmpty(config?.ruleId) ? config.ruleId : defaultId;
            string name = !string.IsNullOrEmpty(config?.name) ? config.name : defaultName;
            string reference = !string.IsNullOrEmpty(config?.reference) ? config.reference : fallbackReference;
            return new ComplianceResult(id, Fmt(terms, name), reference, passed, Fmt(terms, message));
        }

        /// <summary>
        /// Run the profile's rule set: every enabled rule in <c>complianceRules</c>, in order, each by its kind.
        /// A profile with no rule set runs the NEC's ten.
        /// </summary>
        public static List<ComplianceResult> RunAll(ElectricalTables tables, PanelRuleInput input, CodeTerminology terms = null)
        {
            var rules = tables.complianceRules != null && tables.complianceRules.Length > 0
                ? tables.complianceRules
                : ElectricalTables.CreateNecDefaults().complianceRules;

            var results = new List<ComplianceResult>();
            foreach (var rule in rules)
            {
                if (rule == null || !rule.enabled) continue;

                var result = Run(rule, tables, input, terms);
                if (result != null)
                    results.Add(result);
            }

            return results;
        }

        /// <summary>Run one rule by its kind. Null for a kind this build does not know (the profile validator rejects those).</summary>
        public static ComplianceResult Run(ComplianceRuleConfig rule, ElectricalTables tables, PanelRuleInput input, CodeTerminology terms = null)
        {
            switch (ComplianceRuleKinds.Resolve(rule))
            {
                case ComplianceRuleKinds.BreakerConductorMatch: return CheckBreakerConductorMatch(rule, tables, input.breakers, terms);
                case ComplianceRuleKinds.RequiredCircuits: return CheckRequiredCircuits(rule, tables, input.requiredCircuits, input.breakers, terms);
                case ComplianceRuleKinds.ProtectionRequired: return CheckProtection(rule, tables, input.requiredCircuits, input.breakers, terms);
                case ComplianceRuleKinds.LoadBalance: return CheckLoadBalance(rule, tables, input.breakers, terms);
                case ComplianceRuleKinds.MainBreakerSizing: return CheckMainBreakerSizing(rule, tables, input.totalAmps, input.breakers, terms);
                case ComplianceRuleKinds.DoubleTap: return CheckDoubleTap(rule, tables, input.slotUse, terms);
                case ComplianceRuleKinds.ConductorAmpacity: return CheckConductorAmpacity(rule, tables, input.breakers, terms);
                case ComplianceRuleKinds.PanelSpaces: return CheckPanelSpaces(rule, tables, input.totalSlots, input.breakers, terms);
                case ComplianceRuleKinds.WireConnections: return CheckWireConnections(rule, tables, input.breakers, terms);
                default: return null;
            }
        }

        private static float Ratio(ComplianceRuleConfig rule) => rule != null && rule.maxRatio > 0f ? rule.maxRatio : 1f;

        // ---- The built-in rules by their original ids: the same checks with the profile's settings for that id

        public static ComplianceResult BreakerConductorMatch(ElectricalTables t, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckBreakerConductorMatch(t.GetRuleConfig("RULE-01"), t, b, terms);
        public static ComplianceResult RequiredCircuits(ElectricalTables t, RequiredCircuit[] r, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckRequiredCircuits(t.GetRuleConfig("RULE-02"), t, r, b, terms);
        public static ComplianceResult GfciProtection(ElectricalTables t, RequiredCircuit[] r, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckProtection(t.GetRuleConfig("RULE-03") ?? new ComplianceRuleConfig { ruleId = "RULE-03" }, t, r, b, terms);
        public static ComplianceResult AfciProtection(ElectricalTables t, RequiredCircuit[] r, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckProtection(t.GetRuleConfig("RULE-04") ?? new ComplianceRuleConfig { ruleId = "RULE-04" }, t, r, b, terms);
        public static ComplianceResult LoadBalance(ElectricalTables t, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckLoadBalance(t.GetRuleConfig("RULE-05"), t, b, terms);
        public static ComplianceResult MainBreakerSizing(ElectricalTables t, int mainAmps, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckMainBreakerSizing(t.GetRuleConfig("RULE-06"), t, mainAmps, b, terms);
        public static ComplianceResult DoubleTap(ElectricalTables t, List<SlotUse> s, CodeTerminology terms = null) => CheckDoubleTap(t.GetRuleConfig("RULE-07"), t, s, terms);
        public static ComplianceResult ConductorAmpacity(ElectricalTables t, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckConductorAmpacity(t.GetRuleConfig("RULE-08"), t, b, terms);
        public static ComplianceResult PanelSpaces(ElectricalTables t, int totalSlots, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckPanelSpaces(t.GetRuleConfig("RULE-09"), t, totalSlots, b, terms);
        public static ComplianceResult WireConnections(ElectricalTables t, List<PanelBreakerState> b, CodeTerminology terms = null) => CheckWireConnections(t.GetRuleConfig("RULE-10"), t, b, terms);

        // ---- The kinds

        /// <summary>A breaker's rating must not exceed its wire's ampacity (times maxRatio).</summary>
        private static ComplianceResult CheckBreakerConductorMatch(ComplianceRuleConfig rule, ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            float ratio = Ratio(rule);
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire) continue;

                int wireMax = tables.GetMaxAmps(breaker.wireGauge);
                if (breaker.data.ampRating > wireMax * ratio + 1e-4f)
                {
                    return Result(terms, rule, "RULE-01", "{Term:breaker}/Conductor Match", "240.4", false,
                        $"{breaker.circuitName}: {breaker.data.ampRating}A {{term:breaker}} exceeds {breaker.wireGauge} capacity ({wireMax}A).");
                }
            }

            return Result(terms, rule, "RULE-01", "{Term:breaker}/Conductor Match", "240.4", true,
                "All {term:breaker}s match their conductor ampacity.");
        }

        /// <summary>Every required branch circuit is present at its required rating or higher.</summary>
        private static ComplianceResult CheckRequiredCircuits(ComplianceRuleConfig rule, ElectricalTables tables, RequiredCircuit[] required, List<PanelBreakerState> breakers, CodeTerminology terms)
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
                return Result(terms, rule, "RULE-02", "Required Branch Circuits", "210.11", false,
                    $"Missing required circuits: {string.Join(", ", missing)}.");

            return Result(terms, rule, "RULE-02", "Required Branch Circuits", "210.11", true, "All required branch circuits are present.");
        }

        /// <summary>
        /// Circuits that need ground-fault or arc-fault protection (the rule's <c>protection</c>) use a breaker or
        /// device that provides it, or a dual-function one.
        /// </summary>
        private static ComplianceResult CheckProtection(ComplianceRuleConfig rule, ElectricalTables tables, RequiredCircuit[] required, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            bool gfci = ComplianceRuleKinds.ResolveProtection(rule) != "afci";
            string term = gfci ? "shock-protection-device" : "arc-fault-device";
            string defaultId = gfci ? "RULE-03" : "RULE-04";
            string fallbackReference = gfci ? "210.8" : "210.12";
            string defaultName = "{term:" + term + "} Protection";

            var violations = new List<string>();
            foreach (var req in required ?? new RequiredCircuit[0])
            {
                if (!(gfci ? req.requiresGFCI : req.requiresAFCI)) continue;

                var breaker = breakers.FirstOrDefault(b => b.circuitName == req.circuitName);
                if (breaker != null && !(gfci ? breaker.data.SatisfiesGFCI : breaker.data.SatisfiesAFCI))
                    violations.Add(req.circuitName);
            }

            if (violations.Count > 0)
                return Result(terms, rule, defaultId, defaultName, fallbackReference, false,
                    $"Missing {{term:{term}}} protection: {string.Join(", ", violations)}.");

            return Result(terms, rule, defaultId, defaultName, fallbackReference, true,
                $"All required circuits have {{term:{term}}} protection.");
        }

        /// <summary>Load is balanced between the left and right bus (general practice).</summary>
        private static ComplianceResult CheckLoadBalance(ComplianceRuleConfig rule, ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms)
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
                return Result(terms, rule, "RULE-05", "Load Balance", "General Practice", true, "No load to balance.");

            float maxImbalance = rule != null && rule.maxImbalance > 0f ? rule.maxImbalance : tables.loadBalanceMaxImbalance;
            float imbalance = Math.Abs(leftLoad - rightLoad) / totalLoad;
            bool balanced = imbalance <= maxImbalance;

            return Result(terms, rule, "RULE-05", "Load Balance", "General Practice", balanced,
                balanced
                    ? $"Load is balanced ({imbalance:P0} imbalance)."
                    : $"Load imbalance is {imbalance:P0} (max {maxImbalance:P0}). Left: {leftLoad:N0} VA, Right: {rightLoad:N0} VA.");
        }

        /// <summary>The main breaker is sized for the total of the breaker loads (times the rule's margin).</summary>
        private static ComplianceResult CheckMainBreakerSizing(ComplianceRuleConfig rule, ElectricalTables tables, int mainAmps, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            float totalLoadVA = 0f;
            foreach (var breaker in breakers)
                totalLoadVA += breaker.data != null ? breaker.data.GetLoadVA(tables) : 0f;

            float margin = rule != null && rule.margin > 0f ? rule.margin : 1f;
            float loadAmps = LoadCalculator.ConvertVAToAmps(totalLoadVA, tables.serviceVoltage);
            bool adequate = mainAmps >= loadAmps * margin;

            return Result(terms, rule, "RULE-06", "Main {Term:breaker} Sizing", "230.79", adequate,
                adequate
                    ? $"Main {{term:breaker}} ({mainAmps}A) adequate for {loadAmps:N0}A calculated load."
                    : $"Main {{term:breaker}} ({mainAmps}A) undersized for {loadAmps:N0}A calculated load.");
        }

        /// <summary>No breaker takes more slots than it has poles (a double-pole breaker takes two).</summary>
        private static ComplianceResult CheckDoubleTap(ComplianceRuleConfig rule, ElectricalTables tables, List<SlotUse> slotUse, CodeTerminology terms)
        {
            foreach (var use in slotUse)
            {
                if (use.slotsOccupied > use.poleCount)
                    return Result(terms, rule, "RULE-07", "No Double-Tapped {Term:breaker}s", "110.14", false,
                        $"{{Term:breaker}} '{use.circuitName}' occupies {use.slotsOccupied} slots but is only {use.poleCount}-pole.");
            }

            return Result(terms, rule, "RULE-07", "No Double-Tapped {Term:breaker}s", "110.14", true, "No double-tapped {term:breaker}s found.");
        }

        /// <summary>Each wire's conductor size carries its breaker's rating (times maxRatio).</summary>
        private static ComplianceResult CheckConductorAmpacity(ComplianceRuleConfig rule, ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            float ratio = Ratio(rule);
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire) continue;

                bool valid = breaker.wireLinked && breaker.data.ampRating <= tables.GetMaxAmps(breaker.wireGauge) * ratio + 1e-4f;
                if (!valid)
                    return Result(terms, rule, "RULE-08", "Conductor Ampacity", "310.14", false,
                        $"{breaker.circuitName}: {breaker.wireGauge} insufficient for {breaker.data.ampRating}A {{term:breaker}}.");
            }

            return Result(terms, rule, "RULE-08", "Conductor Ampacity", "310.14", true, "All conductor ampacities match {term:breaker} ratings.");
        }

        /// <summary>The breakers fit in the panel's spaces.</summary>
        private static ComplianceResult CheckPanelSpaces(ComplianceRuleConfig rule, ElectricalTables tables, int totalSlots, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            int usedSlots = 0;
            foreach (var breaker in breakers)
                usedSlots += breaker.data.poleCount;

            bool withinLimit = usedSlots <= totalSlots;

            return Result(terms, rule, "RULE-09", "{Term:panel} Spaces", "408.54", withinLimit,
                withinLimit
                    ? $"Using {usedSlots} of {totalSlots} {{term:panel}} spaces."
                    : $"{{Term:panel}} exceeded: {usedSlots} spaces used, {totalSlots} available.");
        }

        /// <summary>Every placed breaker has a wire connected.</summary>
        private static ComplianceResult CheckWireConnections(ComplianceRuleConfig rule, ElectricalTables tables, List<PanelBreakerState> breakers, CodeTerminology terms)
        {
            var unwired = new List<string>();
            foreach (var breaker in breakers)
            {
                if (!breaker.hasWire)
                    unwired.Add(breaker.circuitName ?? breaker.data.GetDisplayName(terms));
            }

            if (unwired.Count > 0)
                return Result(terms, rule, "RULE-10", "Wire Connections", "General Practice", false,
                    $"{{Term:breaker}}s without wire connections: {string.Join(", ", unwired)}.");

            return Result(terms, rule, "RULE-10", "Wire Connections", "General Practice", true, "All {term:breaker}s have wire connections.");
        }
    }
}
