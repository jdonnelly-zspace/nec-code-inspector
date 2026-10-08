using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    public static class ElectricalTablesTests
    {
        public static void Run(TestContext t)
        {
            JsonFileMatchesBuiltInDefaults(t);
            LookupsWork(t);
            EnsureCompleteFillsMissingArrays(t);
        }

        // The JSON file and ElectricalTables.CreateNecDefaults() are two copies of the same numbers.
        private static void JsonFileMatchesBuiltInDefaults(TestContext t)
        {
            t.Begin("tables json == built-in defaults");

            var fromFile = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).tables;
            var defaults = ElectricalTables.CreateNecDefaults();

            t.IsTrue(fromFile != null, "the NEC profile has a tables.json that parses");
            if (fromFile == null) return;

            t.Equal(defaults.singlePoleVoltage, fromFile.singlePoleVoltage, "singlePoleVoltage");
            t.Equal(defaults.doublePoleVoltage, fromFile.doublePoleVoltage, "doublePoleVoltage");
            t.Equal(defaults.serviceVoltage, fromFile.serviceVoltage, "serviceVoltage");
            t.Equal(defaults.areaUnitLabel, fromFile.areaUnitLabel, "areaUnitLabel");
            t.Equal(defaults.defaultConductor, fromFile.defaultConductor, "defaultConductor");
            t.Equal(defaults.lightingVAPerArea, fromFile.lightingVAPerArea, "lightingVAPerArea");
            t.Equal(defaults.smallApplianceVA, fromFile.smallApplianceVA, "smallApplianceVA");
            t.Equal(defaults.laundryVA, fromFile.laundryVA, "laundryVA");
            t.Equal(defaults.dryerVA, fromFile.dryerVA, "dryerVA");
            t.Equal(defaults.rangeDemandVA, fromFile.rangeDemandVA, "rangeDemandVA");
            t.Equal(defaults.fixedApplianceDemandThreshold, fromFile.fixedApplianceDemandThreshold, "fixedApplianceDemandThreshold");
            t.Equal(defaults.fixedApplianceDemandFactor, fromFile.fixedApplianceDemandFactor, "fixedApplianceDemandFactor");
            t.Equal(defaults.loadBalanceMaxImbalance, fromFile.loadBalanceMaxImbalance, "loadBalanceMaxImbalance");

            t.IsTrue(defaults.standardServiceSizes.SequenceEqual(fromFile.standardServiceSizes), "standardServiceSizes");

            t.Equal(defaults.generalLoadDemandTiers.Length, fromFile.generalLoadDemandTiers.Length, "demand tier count");
            for (int i = 0; i < defaults.generalLoadDemandTiers.Length && i < fromFile.generalLoadDemandTiers.Length; i++)
            {
                t.Equal(defaults.generalLoadDemandTiers[i].upToVA, fromFile.generalLoadDemandTiers[i].upToVA, $"tier {i} upToVA");
                t.Equal(defaults.generalLoadDemandTiers[i].factor, fromFile.generalLoadDemandTiers[i].factor, $"tier {i} factor");
            }

            t.Equal(defaults.conductorSizes.Length, fromFile.conductorSizes.Length, "conductor count");
            for (int i = 0; i < defaults.conductorSizes.Length && i < fromFile.conductorSizes.Length; i++)
            {
                t.Equal(defaults.conductorSizes[i].name, fromFile.conductorSizes[i].name, $"conductor {i} name");
                t.Equal(defaults.conductorSizes[i].maxAmps, fromFile.conductorSizes[i].maxAmps, $"conductor {i} maxAmps");
                t.Equal(defaults.conductorSizes[i].visualWidth, fromFile.conductorSizes[i].visualWidth, $"conductor {i} visualWidth");
            }

            t.Equal(defaults.complianceRules.Length, fromFile.complianceRules.Length, "rule count");
            for (int i = 0; i < defaults.complianceRules.Length && i < fromFile.complianceRules.Length; i++)
            {
                t.Equal(defaults.complianceRules[i].ruleId, fromFile.complianceRules[i].ruleId, $"rule {i} id");
                t.Equal(defaults.complianceRules[i].enabled, fromFile.complianceRules[i].enabled, $"rule {i} enabled");
                t.Equal(defaults.complianceRules[i].reference, fromFile.complianceRules[i].reference, $"rule {i} reference");
                t.Equal(defaults.complianceRules[i].conceptId, fromFile.complianceRules[i].conceptId, $"rule {i} conceptId");
                t.Equal(defaults.complianceRules[i].kind, fromFile.complianceRules[i].kind, $"rule {i} kind");
                t.Equal(defaults.complianceRules[i].protection ?? "", fromFile.complianceRules[i].protection ?? "", $"rule {i} protection");
            }
        }

        private static void LookupsWork(TestContext t)
        {
            t.Begin("tables lookups");
            var tables = ElectricalTables.CreateNecDefaults();

            // NEC Table 310.16 copper ampacities the app relied on before the refactor
            t.Equal(15, tables.GetMaxAmps("14 AWG"), "14 AWG ampacity");
            t.Equal(20, tables.GetMaxAmps("12 AWG"), "12 AWG ampacity");
            t.Equal(30, tables.GetMaxAmps("10 AWG"), "10 AWG ampacity");
            t.Equal(195, tables.GetMaxAmps("4/0 AWG"), "4/0 AWG ampacity");
            t.Equal(20, tables.GetMaxAmps(tables.defaultConductor), "the default conductor exists in the conductor table");
            t.Equal(0, tables.GetMaxAmps("99 AWG"), "unknown conductor returns 0");
            t.Equal(0, tables.GetMaxAmps(null), "null conductor returns 0");

            t.Near(0.003, tables.GetWireWidth("14 AWG", 0.005f), "14 AWG width", 0.00001);
            t.Near(0.012, tables.GetWireWidth("6 AWG", 0.005f), "6 AWG width", 0.00001);
            t.Near(0.005, tables.GetWireWidth("4 AWG", 0.005f), "no width falls back", 0.00001);
            t.Near(0.005, tables.GetWireWidth("unknown", 0.005f), "unknown falls back", 0.00001);

            t.Equal("240.4", tables.GetRuleConfig("RULE-01").reference, "RULE-01 reference");
            t.IsTrue(tables.GetRuleConfig("RULE-99") == null, "unknown rule has no config");
        }

        private static void EnsureCompleteFillsMissingArrays(TestContext t)
        {
            t.Begin("tables EnsureComplete");
            var partial = new ElectricalTables { serviceVoltage = 230f };
            partial.EnsureComplete();

            t.IsTrue(partial.conductorSizes.Length > 0, "conductor sizes filled");
            t.IsTrue(partial.generalLoadDemandTiers.Length > 0, "demand tiers filled");
            t.IsTrue(partial.standardServiceSizes.Length > 0, "standard sizes filled");
            t.IsTrue(partial.complianceRules.Length > 0, "rule configs filled");
            t.IsTrue(partial.complianceRules.All(r => !string.IsNullOrEmpty(r.conceptId)), "every rule maps to a concept");
            t.IsTrue(partial.complianceRules.All(r => NECInspector.Data.ConceptIds.IsKnown(r.conceptId)), "rule concepts are known concepts");

            // A rules file written before rules carried a concept still gets one
            var legacy = new ElectricalTables { complianceRules = new[] { new ComplianceRuleConfig { ruleId = "RULE-03", reference = "x" } } };
            legacy.EnsureComplete();
            t.Equal(NECInspector.Data.ConceptIds.ShockProtection, legacy.GetRuleConfig("RULE-03").conceptId, "missing concept is filled from the defaults");
            t.Equal(230f, partial.serviceVoltage, "explicit values are kept");
        }
    }
}
