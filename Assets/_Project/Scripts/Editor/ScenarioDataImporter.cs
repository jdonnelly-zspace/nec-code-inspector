using System;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    /// <summary>
    /// Builds ScenarioDefinitionSO and ViolationDefinitionSO assets from the JSON files in
    /// Assets/_Project/Content/Scenarios. Scenario content lives in data, so adding or changing
    /// a scenario does not require editing C#.
    /// Files are checked by ScenarioFileValidator before any asset is touched.
    /// Asset paths match the earlier per-scenario generators, so existing references stay valid.
    /// </summary>
    public static class ScenarioDataImporter
    {
        private const string CONTENT_DIR = "Assets/_Project/Content/Scenarios";
        private const string VIOLATION_ROOT = "Assets/_Project/ScriptableObjects/Violations";
        private const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

        [MenuItem("NEC Inspector/Import Scenario Data")]
        public static void ImportAll()
        {
            if (!Directory.Exists(CONTENT_DIR))
            {
                Debug.LogError($"[NEC Inspector] Scenario data folder not found: {CONTENT_DIR}");
                return;
            }

            string[] files = Directory.GetFiles(CONTENT_DIR, "*.json").OrderBy(f => f, StringComparer.Ordinal).ToArray();
            int imported = 0;
            int failed = 0;

            foreach (string file in files)
            {
                if (ImportFile(file)) imported++;
                else failed++;
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string summary = $"[NEC Inspector] Scenario data import finished: {imported} imported, {failed} failed ({files.Length} files).";
            if (failed > 0) Debug.LogError(summary);
            else Debug.Log(summary);
        }

        private static bool ImportFile(string path)
        {
            ScenarioFileData data;
            try
            {
                data = JsonUtility.FromJson<ScenarioFileData>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"[NEC Inspector] {path}: could not parse JSON: {e.Message}");
                return false;
            }

            var errors = ScenarioFileValidator.Validate(data);
            if (errors.Count > 0)
            {
                foreach (string error in errors)
                    Debug.LogError($"[NEC Inspector] {path}: {error}");
                return false;
            }

            string violationDir = $"{VIOLATION_ROOT}/{data.violationFolder}";
            EnsureFolder("Assets/_Project/ScriptableObjects", "Violations");
            EnsureFolder(VIOLATION_ROOT, data.violationFolder);
            EnsureFolder("Assets/_Project/ScriptableObjects", "Scenarios");

            AssetDatabase.StartAssetEditing();
            try
            {
                var violationAssets = new ViolationDefinitionSO[data.violations.Length];
                for (int i = 0; i < data.violations.Length; i++)
                {
                    var entry = data.violations[i];
                    string assetPath = $"{violationDir}/VD_{data.assetPrefix}_{entry.violationId}.asset";

                    // Load existing or create new, so references to the asset survive re-imports
                    var asset = AssetDatabase.LoadAssetAtPath<ViolationDefinitionSO>(assetPath);
                    if (asset == null)
                    {
                        asset = ScriptableObject.CreateInstance<ViolationDefinitionSO>();
                        AssetDatabase.CreateAsset(asset, assetPath);
                    }

                    asset.violationId = entry.violationId;
                    asset.conceptId = entry.conceptId;
                    asset.description = entry.description;
                    asset.citations = entry.citations;
                    asset.severity = Enum.Parse<ViolationSeverity>(entry.severity);
                    asset.minimumDifficulty = Enum.Parse<DifficultyLevel>(entry.minimumDifficulty);
                    asset.isSubtle = entry.isSubtle;
                    asset.componentObjectName = entry.componentObjectName;
                    asset.highlightOffset = Vector3.zero;
                    asset.hintText = entry.hintText;
                    asset.componentType = entry.componentType;
                    asset.inspectionNote = entry.inspectionNote;

                    EditorUtility.SetDirty(asset);
                    violationAssets[i] = asset;
                }

                string scenarioPath = $"{SCENARIO_DIR}/ScenarioDefinition_{data.scenarioAssetName}.asset";
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(scenarioPath);
                if (scenario == null)
                {
                    scenario = ScriptableObject.CreateInstance<ScenarioDefinitionSO>();
                    AssetDatabase.CreateAsset(scenario, scenarioPath);
                }

                scenario.id = data.id;
                scenario.sceneName = data.sceneName;
                scenario.displayName = data.displayName;
                scenario.description = data.description;
                scenario.availableDifficulties = data.availableDifficulties
                    .Select(d => Enum.Parse<DifficultyLevel>(d))
                    .ToArray();
                scenario.violations = violationAssets;
                scenario.sessionSize = data.sessionSize ?? new SessionSize();
                scenario.expertTimeLimit = data.expertTimeLimit;
                scenario.environmentDescription = data.environmentDescription;

                EditorUtility.SetDirty(scenario);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            Debug.Log($"[NEC Inspector] Imported '{data.displayName}': {data.violations.Length} violations + 1 scenario definition.");
            return true;
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
