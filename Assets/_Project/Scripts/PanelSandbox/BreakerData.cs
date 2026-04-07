using System;

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

        public string DisplayName => $"{ampRating}A {PoleLabel}{ProtectionLabel}";

        private string PoleLabel => poleCount == 2 ? "2P " : "";

        private string ProtectionLabel
        {
            get
            {
                if (isDualFunction) return " DF";
                if (isGFCI && isAFCI) return " GFCI/AFCI";
                if (isGFCI) return " GFCI";
                if (isAFCI) return " AFCI";
                return "";
            }
        }

        public bool SatisfiesGFCI => isGFCI || isDualFunction;
        public bool SatisfiesAFCI => isAFCI || isDualFunction;
    }
}
