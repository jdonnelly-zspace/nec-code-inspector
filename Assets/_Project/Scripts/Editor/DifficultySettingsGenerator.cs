using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    /// <summary>
    /// Creates or updates the difficulty settings assets from
    /// Assets/_Project/Content/Difficulty/difficulty-settings.json.
    /// </summary>
    public static class DifficultySettingsGenerator
    {
        private const string CONTENT_PATH = "Assets/_Project/Content/Difficulty/difficulty-settings.json";
        private const string SETTINGS_DIR = "Assets/_Project/ScriptableObjects/Settings";

        [MenuItem("NEC Inspector/Generate Difficulty Settings")]
        public static void Generate()
        {
            var file = ContentFileLoader.Load<DifficultySettingsFile>(CONTENT_PATH, ContentFileValidator.Validate);
            if (file == null) return;

            ContentFileLoader.EnsureFolder("Assets/_Project", "ScriptableObjects");
            ContentFileLoader.EnsureFolder("Assets/_Project/ScriptableObjects", "Settings");

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var entry in file.settings)
                    CreateOrUpdate($"{SETTINGS_DIR}/DifficultySettings_{entry.level}.asset", entry);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Difficulty settings generated: {file.settings.Length} levels.");
        }

        private static void CreateOrUpdate(string path, DifficultySettingsEntry entry)
        {
            var asset = AssetDatabase.LoadAssetAtPath<DifficultySettingsSO>(path);
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<DifficultySettingsSO>();
                AssetDatabase.CreateAsset(asset, path);
            }

            asset.level = ContentFileLoader.ParseEnum<DifficultyLevel>(entry.level);
            asset.displayName = entry.displayName;
            asset.showHighlightHints = entry.showHighlightHints;
            asset.showScaffolding = entry.showScaffolding;
            asset.scaffoldingTimeoutSeconds = entry.scaffoldingTimeoutSeconds;
            asset.hintCooldownSeconds = entry.hintCooldownSeconds;
            asset.citationMode = ContentFileLoader.ParseEnum<CitationMode>(entry.citationMode);
            asset.enableTimeLimit = entry.enableTimeLimit;
            asset.timeLimitSeconds = entry.timeLimitSeconds;
            asset.penalizeFalsePositives = entry.penalizeFalsePositives;
            asset.falsePositivePenalty = entry.falsePositivePenalty;
            asset.showSimplifiedTerminology = entry.showSimplifiedTerminology;
            asset.highlightNewInEdition = entry.highlightNewInEdition;
            asset.includeSubtleViolations = entry.includeSubtleViolations;

            EditorUtility.SetDirty(asset);
        }
    }
}
