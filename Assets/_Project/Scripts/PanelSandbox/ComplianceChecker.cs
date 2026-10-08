using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// Validates a panel design against the 10 compliance rules. This class only reads the scene
    /// objects (slots, placed breakers, wires) into plain data; the rules themselves are in
    /// <see cref="ComplianceRules"/>, which is tested without Unity. Which rules run, the citation
    /// shown for each, and numeric limits come from the active code profile's tables
    /// (NEC values by default).
    /// </summary>
    public class ComplianceChecker
    {
        /// <summary>
        /// Run every enabled compliance check against the current panel state.
        /// </summary>
        public List<ComplianceResult> RunAllChecks(
            PanelDesignDefinitionSO definition,
            BreakerSlot[] slots,
            List<PlacedBreaker> placedBreakers)
        {
            var input = new PanelRuleInput
            {
                totalAmps = definition.totalAmps,
                totalSlots = definition.totalSlots,
                requiredCircuits = definition.requiredCircuits,
                breakers = placedBreakers.Select(ToState).ToList(),
                slotUse = BuildSlotUse(slots)
            };

            return ComplianceRules.RunAll(CodeProfiles.Tables, input);
        }

        private static PanelBreakerState ToState(PlacedBreaker breaker)
        {
            var wire = breaker.ConnectedWire;
            return new PanelBreakerState
            {
                circuitName = breaker.AssignedCircuitName,
                data = breaker.BreakerData,
                hasWire = wire != null,
                wireGauge = wire != null ? wire.WireGauge : null,
                wireLinked = wire != null && wire.ConnectedBreaker?.BreakerData != null,
                side = breaker.CurrentSlot != null ? breaker.CurrentSlot.BusSide : (BusSide?)null
            };
        }

        // A double-pole breaker occupies two slots, which is valid; the rule compares that count to its poles
        private static List<SlotUse> BuildSlotUse(BreakerSlot[] slots)
        {
            var order = new List<PlacedBreaker>();
            var counts = new Dictionary<PlacedBreaker, int>();

            foreach (var slot in slots)
            {
                if (!slot.IsOccupied || slot.PlacedBreaker == null) continue;

                if (!counts.ContainsKey(slot.PlacedBreaker))
                {
                    counts[slot.PlacedBreaker] = 0;
                    order.Add(slot.PlacedBreaker);
                }
                counts[slot.PlacedBreaker]++;
            }

            return order.Select(b => new SlotUse
            {
                circuitName = b.AssignedCircuitName,
                poleCount = b.BreakerData.poleCount,
                slotsOccupied = counts[b]
            }).ToList();
        }
    }
}
