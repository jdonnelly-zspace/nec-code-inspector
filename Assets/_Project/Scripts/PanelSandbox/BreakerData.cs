using System;
using NECInspector.Codes;

namespace NECInspector.PanelSandbox
{
    [Serializable]
    public class BreakerData
    {
        public string breakerName;
        public int ampRating;          // 15, 20, 30, 40, 50, etc.
        public int poleCount = 1;      // 1 = single pole (120V), 2 = double pole (240V)
        public bool isGFCI;
        public bool isAFCI;
        public bool isDualFunction;    // Combined GFCI + AFCI
        public string wireGauge;       // Expected wire gauge, e.g., "12 AWG"

        /// <summary>Label with the NEC words for the protection kinds (GFCI, AFCI).</summary>
        public string DisplayName => GetDisplayName(null);

        /// <summary>Label with the code's own words for the protection kinds (for example RCD and AFDD).</summary>
        public string GetDisplayName(CodeTerminology terms)
        {
            terms = terms ?? CodeTerminology.CreateNecDefaults();
            string gfci = terms.Term("shock-protection-device");
            string afci = terms.Term("arc-fault-device");

            string protection = "";
            if (isDualFunction) protection = " DF";
            else if (isGFCI && isAFCI) protection = $" {gfci}/{afci}";
            else if (isGFCI) protection = $" {gfci}";
            else if (isAFCI) protection = $" {afci}";

            return $"{ampRating}A {(poleCount == 2 ? "2P " : "")}{protection}";
        }

        public bool SatisfiesGFCI => isGFCI || isDualFunction;
        public bool SatisfiesAFCI => isAFCI || isDualFunction;

        /// <summary>
        /// Load in VA for this breaker's circuit: rating times the single-pole or double-pole voltage
        /// from the profile's tables (120 V and 240 V for the NEC).
        /// </summary>
        public float GetLoadVA(ElectricalTables tables)
        {
            float voltage = poleCount == 2 ? tables.doublePoleVoltage : tables.singlePoleVoltage;
            return ampRating * voltage;
        }
    }
}
