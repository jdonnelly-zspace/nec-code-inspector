using UnityEngine;
using UnityEditor;

namespace NECInspector.Editor
{
    public static class GenerateAllEditor
    {
        [MenuItem("NEC Inspector/Generate All Data")]
        public static void GenerateAll()
        {
            Debug.Log("[NEC Inspector] === Starting full data generation ===");

            // 1. Settings (no dependencies)
            Debug.Log("[NEC Inspector] [1/8] Generating difficulty settings...");
            DifficultySettingsGenerator.Generate();

            // 2-5. Scenario generators (independent of each other)
            Debug.Log("[NEC Inspector] [2/8] Generating Residential Panel scenario...");
            ResidentialPanelScenarioGenerator.Generate();

            Debug.Log("[NEC Inspector] [3/8] Generating Branch Circuit scenario...");
            BranchCircuitScenarioGenerator.Generate();

            Debug.Log("[NEC Inspector] [4/8] Generating Grounding scenario...");
            GroundingScenarioGenerator.Generate();

            Debug.Log("[NEC Inspector] [5/8] Generating Commercial scenario...");
            CommercialScenarioGenerator.Generate();

            // 6. Panel sandbox (independent)
            Debug.Log("[NEC Inspector] [6/8] Generating Panel Sandbox data...");
            PanelDesignSandboxGenerator.Generate();

            // 7. Quick Reference Cards (independent)
            Debug.Log("[NEC Inspector] [7/8] Generating Quick Reference Cards...");
            QuickReferenceCardGenerator.Generate();

            // 8. Certificate templates (independent)
            Debug.Log("[NEC Inspector] [8/8] Generating Certificate Templates...");
            CertificateTemplateGenerator.Generate();

            // 9. Catalog (must run last — discovers scenario assets)
            Debug.Log("[NEC Inspector] [FINAL] Generating Scenario Catalog...");
            ScenarioCatalogGenerator.Generate();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NEC Inspector] === All data generation complete! ===");
        }
    }
}
