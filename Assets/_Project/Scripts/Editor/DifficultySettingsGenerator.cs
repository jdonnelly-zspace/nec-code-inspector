using UnityEngine;
using UnityEditor;
using NECInspector.Core;

namespace NECInspector.Editor
{
    public static class DifficultySettingsGenerator
    {
        [MenuItem("NEC Inspector/Generate Difficulty Settings")]
        public static void Generate()
        {
            const string SETTINGS_DIR = "Assets/_Project/ScriptableObjects/Settings";

            if (!AssetDatabase.IsValidFolder(SETTINGS_DIR))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects"))
                    AssetDatabase.CreateFolder("Assets/_Project", "ScriptableObjects");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Settings");
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                // === Beginner ===
                CreateOrUpdate($"{SETTINGS_DIR}/DifficultySettings_Beginner.asset",
                    level: DifficultyLevel.Beginner,
                    displayName: "Beginner",
                    showHighlightHints: true,
                    showScaffolding: true,
                    scaffoldingTimeoutSeconds: 60f,
                    hintCooldownSeconds: 15f,
                    citationMode: NECCitationMode.Dropdown,
                    enableTimeLimit: false,
                    timeLimitSeconds: 0,
                    penalizeFalsePositives: false,
                    falsePositivePenalty: 0f,
                    showSimplifiedTerminology: true,
                    highlight2026Changes: true,
                    includeSubtleViolations: false
                );

                // === Standard ===
                CreateOrUpdate($"{SETTINGS_DIR}/DifficultySettings_Standard.asset",
                    level: DifficultyLevel.Standard,
                    displayName: "Standard",
                    showHighlightHints: false,
                    showScaffolding: false,
                    scaffoldingTimeoutSeconds: -1f,
                    hintCooldownSeconds: 30f,
                    citationMode: NECCitationMode.SearchableDropdown,
                    enableTimeLimit: false,
                    timeLimitSeconds: 0,
                    penalizeFalsePositives: false,
                    falsePositivePenalty: 0.1f,
                    showSimplifiedTerminology: false,
                    highlight2026Changes: true,
                    includeSubtleViolations: false
                );

                // === Expert ===
                CreateOrUpdate($"{SETTINGS_DIR}/DifficultySettings_Expert.asset",
                    level: DifficultyLevel.Expert,
                    displayName: "Expert",
                    showHighlightHints: false,
                    showScaffolding: false,
                    scaffoldingTimeoutSeconds: -1f,
                    hintCooldownSeconds: -1f,
                    citationMode: NECCitationMode.FreeText,
                    enableTimeLimit: true,
                    timeLimitSeconds: 1200,
                    penalizeFalsePositives: true,
                    falsePositivePenalty: 0.25f,
                    showSimplifiedTerminology: false,
                    highlight2026Changes: false,
                    includeSubtleViolations: true
                );
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log("[NEC Inspector] Difficulty settings generated: Beginner, Standard, Expert.");
        }

        private static void CreateOrUpdate(string path, DifficultyLevel level, string displayName,
            bool showHighlightHints, bool showScaffolding, float scaffoldingTimeoutSeconds,
            float hintCooldownSeconds, NECCitationMode citationMode, bool enableTimeLimit,
            int timeLimitSeconds, bool penalizeFalsePositives, float falsePositivePenalty,
            bool showSimplifiedTerminology, bool highlight2026Changes, bool includeSubtleViolations)
        {
            var asset = AssetDatabase.LoadAssetAtPath<DifficultySettingsSO>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<DifficultySettingsSO>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.level = level;
            asset.displayName = displayName;
            asset.showHighlightHints = showHighlightHints;
            asset.showScaffolding = showScaffolding;
            asset.scaffoldingTimeoutSeconds = scaffoldingTimeoutSeconds;
            asset.hintCooldownSeconds = hintCooldownSeconds;
            asset.citationMode = citationMode;
            asset.enableTimeLimit = enableTimeLimit;
            asset.timeLimitSeconds = timeLimitSeconds;
            asset.penalizeFalsePositives = penalizeFalsePositives;
            asset.falsePositivePenalty = falsePositivePenalty;
            asset.showSimplifiedTerminology = showSimplifiedTerminology;
            asset.highlight2026Changes = highlight2026Changes;
            asset.includeSubtleViolations = includeSubtleViolations;

            EditorUtility.SetDirty(asset);
        }
    }
}
