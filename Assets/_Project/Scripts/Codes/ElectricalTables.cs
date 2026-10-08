using System;
using System.Linq;
using NECInspector.Data;

namespace NECInspector.Codes
{
    [Serializable]
    public class ConductorSize
    {
        public string name;         // e.g., "12 AWG" (or "2.5 mm2" in a metric profile)
        public int maxAmps;         // allowable ampacity for the profile's reference conditions
        public float visualWidth;   // line width used when drawing the wire in the sandbox
    }

    [Serializable]
    public class DemandTier
    {
        public float upToVA;        // upper bound of this tier in VA; negative = no upper limit
        public float factor;        // demand factor applied to the slice of load in this tier
    }

    [Serializable]
    public class ComplianceRuleConfig
    {
        public string ruleId;       // e.g., "RULE-01"
        public bool enabled = true;
        public string reference;    // citation shown for this rule in the active code
        public string conceptId;    // skill (concept) this rule gives evidence of
    }

    /// <summary>
    /// Numbers and rule settings that belong to a code profile rather than to the app logic:
    /// system voltages, conductor ampacities, load-calculation constants, demand factors,
    /// standard sizes and compliance rule settings. Loaded from JSON by the profile.
    /// </summary>
    [Serializable]
    public class ElectricalTables
    {
        // System
        public float singlePoleVoltage = 120f;
        public float doublePoleVoltage = 240f;
        public float serviceVoltage = 240f;

        // Units and defaults shown to students
        public string areaUnitLabel = "sq ft";        // unit of dwelling area used by the lighting load constant
        public string defaultConductor = "12 AWG";    // conductor used when a wire has none set

        // Dwelling load calculation constants
        public float lightingVAPerArea = 3f;
        public float smallApplianceVA = 1500f;
        public float laundryVA = 1500f;
        public float dryerVA = 5000f;
        public float rangeDemandVA = 8000f;
        public DemandTier[] generalLoadDemandTiers;
        public int fixedApplianceDemandThreshold = 4;
        public float fixedApplianceDemandFactor = 0.75f;
        public int[] standardServiceSizes;

        // Panel compliance
        public float loadBalanceMaxImbalance = 0.2f;
        public ConductorSize[] conductorSizes;
        public ComplianceRuleConfig[] complianceRules;

        /// <summary>Allowable ampacity for a conductor size name, or 0 if unknown.</summary>
        public int GetMaxAmps(string conductorName)
        {
            var size = FindConductor(conductorName);
            return size != null ? size.maxAmps : 0;
        }

        public float GetWireWidth(string conductorName, float fallback)
        {
            var size = FindConductor(conductorName);
            return size != null && size.visualWidth > 0f ? size.visualWidth : fallback;
        }

        public ComplianceRuleConfig GetRuleConfig(string ruleId)
        {
            return complianceRules?.FirstOrDefault(r => r.ruleId == ruleId);
        }

        private ConductorSize FindConductor(string conductorName)
        {
            return conductorSizes?.FirstOrDefault(c => c.name == conductorName);
        }

        /// <summary>
        /// Fill any array the JSON left out with the NEC defaults, so partial files still work.
        /// </summary>
        public void EnsureComplete()
        {
            var defaults = CreateNecDefaults();
            if (generalLoadDemandTiers == null || generalLoadDemandTiers.Length == 0)
                generalLoadDemandTiers = defaults.generalLoadDemandTiers;
            if (standardServiceSizes == null || standardServiceSizes.Length == 0)
                standardServiceSizes = defaults.standardServiceSizes;
            if (conductorSizes == null || conductorSizes.Length == 0)
                conductorSizes = defaults.conductorSizes;
            if (complianceRules == null || complianceRules.Length == 0)
                complianceRules = defaults.complianceRules;

            // Files written before rules carried a concept still work: take it from the defaults
            foreach (var rule in complianceRules)
            {
                if (!string.IsNullOrEmpty(rule.conceptId)) continue;
                var fallback = defaults.GetRuleConfig(rule.ruleId);
                if (fallback != null) rule.conceptId = fallback.conceptId;
            }
        }

        /// <summary>
        /// NEC values (copper conductors, NEC Table 310.16 ampacities, Art. 220 dwelling standard method).
        /// Used when the profile has no tables file. Keep in sync with StreamingAssets/Codes/nec/tables.json.
        /// </summary>
        public static ElectricalTables CreateNecDefaults()
        {
            return new ElectricalTables
            {
                generalLoadDemandTiers = new[]
                {
                    new DemandTier { upToVA = 3000f, factor = 1f },
                    new DemandTier { upToVA = 120000f, factor = 0.35f },
                    new DemandTier { upToVA = -1f, factor = 0.25f }
                },
                standardServiceSizes = new[] { 60, 100, 125, 150, 200, 225, 300, 400 },
                conductorSizes = new[]
                {
                    Conductor("14 AWG", 15, 0.003f),
                    Conductor("12 AWG", 20, 0.005f),
                    Conductor("10 AWG", 30, 0.007f),
                    Conductor("8 AWG", 40, 0.009f),
                    Conductor("6 AWG", 55, 0.012f),
                    Conductor("4 AWG", 70, 0f),
                    Conductor("3 AWG", 85, 0f),
                    Conductor("2 AWG", 95, 0f),
                    Conductor("1 AWG", 110, 0f),
                    Conductor("1/0 AWG", 125, 0f),
                    Conductor("2/0 AWG", 145, 0f),
                    Conductor("3/0 AWG", 165, 0f),
                    Conductor("4/0 AWG", 195, 0f)
                },
                complianceRules = new[]
                {
                    Rule("RULE-01", "240.4", ConceptIds.OvercurrentProtection),
                    Rule("RULE-02", "210.11", ConceptIds.BranchCircuitRequirements),
                    Rule("RULE-03", "210.8", ConceptIds.ShockProtection),
                    Rule("RULE-04", "210.12", ConceptIds.ArcFaultProtection),
                    Rule("RULE-05", "General Practice", ConceptIds.LoadCalculation),
                    Rule("RULE-06", "230.79", ConceptIds.LoadCalculation),
                    Rule("RULE-07", "110.14", ConceptIds.EquipmentInstallation),
                    Rule("RULE-08", "310.14", ConceptIds.ConductorSizing),
                    Rule("RULE-09", "408.54", ConceptIds.EquipmentInstallation),
                    Rule("RULE-10", "General Practice", ConceptIds.EquipmentInstallation)
                }
            };
        }

        private static ConductorSize Conductor(string name, int maxAmps, float visualWidth)
        {
            return new ConductorSize { name = name, maxAmps = maxAmps, visualWidth = visualWidth };
        }

        private static ComplianceRuleConfig Rule(string ruleId, string reference, string conceptId)
        {
            return new ComplianceRuleConfig { ruleId = ruleId, enabled = true, reference = reference, conceptId = conceptId };
        }
    }
}
