using System.Linq;
using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.PanelSandbox;

namespace NECInspector.Editor
{
    /// <summary>
    /// Creates or updates the panel design definitions from
    /// Assets/_Project/Content/Sandbox/panel-designs.json.
    /// </summary>
    public static class PanelDesignSandboxGenerator
    {
        private const string CONTENT_PATH = "Assets/_Project/Content/Sandbox/panel-designs.json";
        private const string ASSET_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

        [MenuItem("NEC Inspector/Generate Panel Sandbox Data")]
        public static void Generate()
        {
            var file = ContentFileLoader.Load<SandboxFile>(CONTENT_PATH, ContentFileValidator.Validate);
            if (file == null) return;

            ContentFileLoader.EnsureFolder("Assets/_Project", "ScriptableObjects");
            ContentFileLoader.EnsureFolder("Assets/_Project/ScriptableObjects", "Scenarios");

            foreach (var entry in file.designs)
            {
                string assetPath = $"{ASSET_DIR}/PanelDesign_{entry.assetName}.asset";
                var definition = AssetDatabase.LoadAssetAtPath<PanelDesignDefinitionSO>(assetPath);
                if (definition == null)
                {
                    definition = ScriptableObject.CreateInstance<PanelDesignDefinitionSO>();
                    AssetDatabase.CreateAsset(definition, assetPath);
                }

                definition.panelType = entry.panelType;
                definition.displayName = entry.displayName;
                definition.description = entry.description;
                definition.totalAmps = entry.totalAmps;
                definition.totalSlots = entry.totalSlots;
                definition.dwellingArea = entry.dwellingArea;
                definition.targetLoadVA = entry.targetLoadVA;
                definition.loadCalcTolerancePercent = entry.loadCalcTolerancePercent;
                definition.expertTimeLimit = entry.expertTimeLimit;
                definition.availableDifficulties = entry.availableDifficulties
                    .Select(ContentFileLoader.ParseEnum<DifficultyLevel>).ToArray();
                definition.requiredCircuits = entry.requiredCircuits;

                EditorUtility.SetDirty(definition);
                Debug.Log($"[NEC Inspector] Panel sandbox data generated: {definition.requiredCircuits.Length} required circuits for {definition.panelType}.");
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
