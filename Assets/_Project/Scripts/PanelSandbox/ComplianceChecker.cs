using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.PanelSandbox
{
    [Serializable]
    public class ComplianceResult
    {
        public string ruleId;
        public string ruleName;
        public string necReference;
        public bool passed;
        public string message;

        public ComplianceResult(string ruleId, string ruleName, string necReference, bool passed, string message)
        {
            this.ruleId = ruleId;
            this.ruleName = ruleName;
            this.necReference = necReference;
            this.passed = passed;
            this.message = message;
        }
    }

    /// <summary>
    /// Validates a panel design against 10 NEC compliance rules.
    /// </summary>
    public class ComplianceChecker
    {
        /// <summary>
        /// Run all 10 compliance checks against the current panel state.
        /// </summary>
        public List<ComplianceResult> RunAllChecks(
            PanelDesignDefinitionSO definition,
            BreakerSlot[] slots,
            List<PlacedBreaker> placedBreakers)
        {
            var results = new List<ComplianceResult>();

            results.Add(CheckBreakerConductorMatch(placedBreakers));
            results.Add(CheckRequiredCircuits(definition, placedBreakers));
            results.Add(CheckGFCIProtection(definition, placedBreakers));
            results.Add(CheckAFCIProtection(definition, placedBreakers));
            results.Add(CheckLoadBalance(slots, placedBreakers));
            results.Add(CheckMainBreakerSizing(definition, placedBreakers));
            results.Add(CheckDoubleTap(slots));
            results.Add(CheckConductorAmpacity(placedBreakers));
            results.Add(CheckPanelSpaces(definition, slots, placedBreakers));
            results.Add(CheckWireConnections(placedBreakers));

            return results;
        }

        /// <summary>
        /// Rule 1: Breaker amperage must not exceed wire ampacity (Art. 240.4).
        /// </summary>
        public ComplianceResult CheckBreakerConductorMatch(List<PlacedBreaker> breakers)
        {
            foreach (var breaker in breakers)
            {
                if (breaker.ConnectedWire == null) continue;

                int wireMax = WireConnection.GetMaxAmpsForGauge(breaker.ConnectedWire.WireGauge);
                if (breaker.BreakerData.ampRating > wireMax)
                {
                    return new ComplianceResult(
                        "RULE-01", "Breaker/Conductor Match", "240.4",
                        false,
                        $"{breaker.AssignedCircuitName}: {breaker.BreakerData.ampRating}A breaker exceeds {breaker.ConnectedWire.WireGauge} capacity ({wireMax}A)."
                    );
                }
            }

            return new ComplianceResult(
                "RULE-01", "Breaker/Conductor Match", "240.4",
                true, "All breakers match their conductor ampacity."
            );
        }

        /// <summary>
        /// Rule 2: All required branch circuits must be present (Art. 210.11).
        /// </summary>
        public ComplianceResult CheckRequiredCircuits(PanelDesignDefinitionSO definition, List<PlacedBreaker> breakers)
        {
            var missing = new List<string>();
            foreach (var req in definition.requiredCircuits)
            {
                if (!req.isRequired) continue;

                bool found = breakers.Any(b =>
                    b.AssignedCircuitName == req.circuitName &&
                    b.BreakerData.ampRating >= req.ampsRequired);

                if (!found)
                    missing.Add(req.circuitName);
            }

            if (missing.Count > 0)
            {
                return new ComplianceResult(
                    "RULE-02", "Required Branch Circuits", "210.11",
                    false,
                    $"Missing required circuits: {string.Join(", ", missing)}."
                );
            }

            return new ComplianceResult(
                "RULE-02", "Required Branch Circuits", "210.11",
                true, "All required branch circuits are present."
            );
        }

        /// <summary>
        /// Rule 3: Circuits requiring GFCI must use GFCI or dual-function breakers (Art. 210.8).
        /// </summary>
        public ComplianceResult CheckGFCIProtection(PanelDesignDefinitionSO definition, List<PlacedBreaker> breakers)
        {
            var violations = new List<string>();
            foreach (var req in definition.requiredCircuits)
            {
                if (!req.requiresGFCI) continue;

                var breaker = breakers.FirstOrDefault(b => b.AssignedCircuitName == req.circuitName);
                if (breaker != null && !breaker.BreakerData.SatisfiesGFCI)
                {
                    violations.Add(req.circuitName);
                }
            }

            if (violations.Count > 0)
            {
                return new ComplianceResult(
                    "RULE-03", "GFCI Protection", "210.8",
                    false,
                    $"Missing GFCI protection: {string.Join(", ", violations)}."
                );
            }

            return new ComplianceResult(
                "RULE-03", "GFCI Protection", "210.8",
                true, "All required circuits have GFCI protection."
            );
        }

        /// <summary>
        /// Rule 4: Circuits requiring AFCI must use AFCI or dual-function breakers (Art. 210.12).
        /// </summary>
        public ComplianceResult CheckAFCIProtection(PanelDesignDefinitionSO definition, List<PlacedBreaker> breakers)
        {
            var violations = new List<string>();
            foreach (var req in definition.requiredCircuits)
            {
                if (!req.requiresAFCI) continue;

                var breaker = breakers.FirstOrDefault(b => b.AssignedCircuitName == req.circuitName);
                if (breaker != null && !breaker.BreakerData.SatisfiesAFCI)
                {
                    violations.Add(req.circuitName);
                }
            }

            if (violations.Count > 0)
            {
                return new ComplianceResult(
                    "RULE-04", "AFCI Protection", "210.12",
                    false,
                    $"Missing AFCI protection: {string.Join(", ", violations)}."
                );
            }

            return new ComplianceResult(
                "RULE-04", "AFCI Protection", "210.12",
                true, "All required circuits have AFCI protection."
            );
        }

        /// <summary>
        /// Rule 5: Load balance between left and right bus sides (general practice, ≤20% imbalance).
        /// </summary>
        public ComplianceResult CheckLoadBalance(BreakerSlot[] slots, List<PlacedBreaker> breakers)
        {
            float leftLoad = 0f, rightLoad = 0f;

            foreach (var breaker in breakers)
            {
                if (breaker.CurrentSlot == null) continue;
                float load = breaker.GetLoadVA();

                if (breaker.CurrentSlot.BusSide == BusSide.Left)
                    leftLoad += load;
                else
                    rightLoad += load;
            }

            float totalLoad = leftLoad + rightLoad;
            if (totalLoad <= 0f)
            {
                return new ComplianceResult(
                    "RULE-05", "Load Balance", "General Practice",
                    true, "No load to balance."
                );
            }

            float imbalance = Math.Abs(leftLoad - rightLoad) / totalLoad;
            bool balanced = imbalance <= 0.2f;

            return new ComplianceResult(
                "RULE-05", "Load Balance", "General Practice",
                balanced,
                balanced
                    ? $"Load is balanced ({imbalance:P0} imbalance)."
                    : $"Load imbalance is {imbalance:P0} (max 20%). Left: {leftLoad:N0} VA, Right: {rightLoad:N0} VA."
            );
        }

        /// <summary>
        /// Rule 6: Main breaker must be sized for calculated load (Art. 230.79).
        /// </summary>
        public ComplianceResult CheckMainBreakerSizing(PanelDesignDefinitionSO definition, List<PlacedBreaker> breakers)
        {
            float totalLoadVA = 0f;
            foreach (var breaker in breakers)
                totalLoadVA += breaker.GetLoadVA();

            float loadAmps = LoadCalculator.ConvertVAToAmps(totalLoadVA, 240f);
            bool adequate = definition.totalAmps >= loadAmps;

            return new ComplianceResult(
                "RULE-06", "Main Breaker Sizing", "230.79",
                adequate,
                adequate
                    ? $"Main breaker ({definition.totalAmps}A) adequate for {loadAmps:N0}A calculated load."
                    : $"Main breaker ({definition.totalAmps}A) undersized for {loadAmps:N0}A calculated load."
            );
        }

        /// <summary>
        /// Rule 7: No double-tapped breakers — one circuit per breaker terminal.
        /// </summary>
        public ComplianceResult CheckDoubleTap(BreakerSlot[] slots)
        {
            // In our model, each slot can hold only one breaker, so double-tap
            // would mean multiple wires on one breaker. Check PlacedBreaker references.
            // This is inherently prevented by the data model but we validate anyway.
            var breakerCounts = new Dictionary<PlacedBreaker, int>();
            foreach (var slot in slots)
            {
                if (!slot.IsOccupied) continue;
                if (!breakerCounts.ContainsKey(slot.PlacedBreaker))
                    breakerCounts[slot.PlacedBreaker] = 0;
                breakerCounts[slot.PlacedBreaker]++;
            }

            // A double-pole breaker occupies 2 slots, which is valid
            foreach (var kvp in breakerCounts)
            {
                if (kvp.Value > kvp.Key.BreakerData.poleCount)
                {
                    return new ComplianceResult(
                        "RULE-07", "No Double-Tapped Breakers", "110.14",
                        false,
                        $"Breaker '{kvp.Key.AssignedCircuitName}' occupies {kvp.Value} slots but is only {kvp.Key.BreakerData.poleCount}-pole."
                    );
                }
            }

            return new ComplianceResult(
                "RULE-07", "No Double-Tapped Breakers", "110.14",
                true, "No double-tapped breakers found."
            );
        }

        /// <summary>
        /// Rule 8: Conductor ampacity must match breaker rating (Art. 310.14).
        /// </summary>
        public ComplianceResult CheckConductorAmpacity(List<PlacedBreaker> breakers)
        {
            foreach (var breaker in breakers)
            {
                if (breaker.ConnectedWire == null) continue;
                if (!breaker.ConnectedWire.Validate())
                {
                    return new ComplianceResult(
                        "RULE-08", "Conductor Ampacity", "310.14",
                        false,
                        $"{breaker.AssignedCircuitName}: {breaker.ConnectedWire.WireGauge} insufficient for {breaker.BreakerData.ampRating}A breaker."
                    );
                }
            }

            return new ComplianceResult(
                "RULE-08", "Conductor Ampacity", "310.14",
                true, "All conductor ampacities match breaker ratings."
            );
        }

        /// <summary>
        /// Rule 9: Panel spaces must not be exceeded (Art. 408.36).
        /// </summary>
        public ComplianceResult CheckPanelSpaces(PanelDesignDefinitionSO definition, BreakerSlot[] slots, List<PlacedBreaker> breakers)
        {
            int usedSlots = 0;
            foreach (var breaker in breakers)
            {
                usedSlots += breaker.BreakerData.poleCount;
            }

            bool withinLimit = usedSlots <= definition.totalSlots;

            return new ComplianceResult(
                "RULE-09", "Panel Spaces", "408.36",
                withinLimit,
                withinLimit
                    ? $"Using {usedSlots} of {definition.totalSlots} panel spaces."
                    : $"Panel exceeded: {usedSlots} spaces used, {definition.totalSlots} available."
            );
        }

        /// <summary>
        /// Rule 10: All placed breakers must have wire connections.
        /// </summary>
        public ComplianceResult CheckWireConnections(List<PlacedBreaker> breakers)
        {
            var unwired = new List<string>();
            foreach (var breaker in breakers)
            {
                if (!breaker.IsWired)
                    unwired.Add(breaker.AssignedCircuitName ?? breaker.BreakerData.DisplayName);
            }

            if (unwired.Count > 0)
            {
                return new ComplianceResult(
                    "RULE-10", "Wire Connections", "General Practice",
                    false,
                    $"Breakers without wire connections: {string.Join(", ", unwired)}."
                );
            }

            return new ComplianceResult(
                "RULE-10", "Wire Connections", "General Practice",
                true, "All breakers have wire connections."
            );
        }
    }
}
