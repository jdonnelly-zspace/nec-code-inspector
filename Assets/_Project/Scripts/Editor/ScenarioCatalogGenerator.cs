using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class ScenarioCatalogGenerator
    {
        [MenuItem("NEC Inspector/Generate Scenario Catalog")]
        public static void Generate()
        {
            const string CATALOG_PATH = "Assets/_Project/ScriptableObjects/ScenarioCatalog.asset";

            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects"))
                AssetDatabase.CreateFolder("Assets/_Project", "ScriptableObjects");

            // Find all ScenarioDefinitionSO assets
            string[] guids = AssetDatabase.FindAssets("t:ScenarioDefinitionSO",
                new[] { "Assets/_Project/ScriptableObjects/Scenarios" });

            var scenarios = new List<ScenarioDefinitionSO>();
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(path);
                if (scenario != null)
                {
                    scenarios.Add(scenario);
                    Debug.Log($"[NEC Inspector] Found scenario: {scenario.displayName} ({scenario.sceneName})");
                }
            }

            // Create or update catalog
            var catalog = AssetDatabase.LoadAssetAtPath<ScenarioCatalogSO>(CATALOG_PATH);
            if (catalog == null)
            {
                catalog = ScriptableObject.CreateInstance<ScenarioCatalogSO>();
                AssetDatabase.CreateAsset(catalog, CATALOG_PATH);
            }

            catalog.scenarios = scenarios;
            EditorUtility.SetDirty(catalog);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Scenario catalog generated with {scenarios.Count} scenarios.");
        }
    }
}
