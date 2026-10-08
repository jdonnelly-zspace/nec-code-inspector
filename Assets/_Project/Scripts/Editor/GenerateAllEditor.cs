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
            Debug.Log("[NEC Inspector] [1/5] Generating difficulty settings...");
            DifficultySettingsGenerator.Generate();

            // 2. Inspection scenarios and violations, from the JSON files in Content/Scenarios
            Debug.Log("[NEC Inspector] [2/5] Importing scenario data...");
            ScenarioDataImporter.ImportAll();

            // 3. Panel sandbox (independent)
            Debug.Log("[NEC Inspector] [3/5] Generating Panel Sandbox data...");
            PanelDesignSandboxGenerator.Generate();

            // 4. Quick Reference Cards (independent)
            Debug.Log("[NEC Inspector] [4/5] Generating Quick Reference Cards...");
            QuickReferenceCardGenerator.Generate();

            // 5. Certificate templates (independent)
            Debug.Log("[NEC Inspector] [5/5] Generating Certificate Templates...");
            CertificateTemplateGenerator.Generate();

            // 6. Catalog (must run last — discovers scenario assets)
            Debug.Log("[NEC Inspector] [FINAL] Generating Scenario Catalog...");
            ScenarioCatalogGenerator.Generate();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NEC Inspector] === All data generation complete! ===");
        }
    }
}
