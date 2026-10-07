using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    /// <summary>
    /// Builds ScenarioDefinitionSO and ViolationDefinitionSO assets from the JSON files in
    /// Assets/_Project/Content/Scenarios. Scenario content lives in data, so adding or changing
    /// a scenario (or a credential-specific content pack) does not require editing C#.
    /// Asset paths match the earlier per-scenario generators, so existing references stay valid.
    /// </summary>
    public static class ScenarioDataImporter
    {
        private const string CONTENT_DIR = "Assets/_Project/Content/Scenarios";
        private const string VIOLATION_ROOT = "Assets/_Project/ScriptableObjects/Violations";
        private const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

        [Serializable]
        private class ScenarioFile
        {
            public string id;
            public string sceneName;
            public string displayName;
            public string description;
            public string environmentDescription;
            public string[] necChapters;
            public int expertTimeLimit;
            public string[] availableDifficulties;
            public string assetPrefix;         // violation asset names: VD_{assetPrefix}_{violationId}
            public string violationFolder;     // under ScriptableObjects/Violations
            public string scenarioAssetName;   // ScenarioDefinition_{scenarioAssetName}.asset
            public ViolationEntry[] violations;
        }

        [Serializable]
        private class ViolationEntry
        {
            public string violationId;
            public string conceptId;
            public string description;
            public string necArticle;
            public string necArticleText;
            public string severity;            // Minor | Major | Critical
            public string minimumDifficulty;   // Beginner | Standard | Expert
            public bool isSubtle;
            public string componentObjectName;
            public string hintText;
            public string componentType;
            public string inspectionNote;
        }

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
            ScenarioFile data;
            try
            {
                data = JsonUtility.FromJson<ScenarioFile>(File.ReadAllText(path));
            }
            catch (Exception e)
            {
                Debug.LogError($"[NEC Inspector] {path}: could not parse JSON: {e.Message}");
                return false;
            }

            var errors = Validate(data);
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
                    asset.necArticle = entry.necArticle;
                    asset.necArticleText = entry.necArticleText;
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
                scenario.necChapters = data.necChapters ?? new string[0];
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

        /// <summary>
        /// Check a scenario file before touching any assets. Returns one message per problem.
        /// </summary>
        private static List<string> Validate(ScenarioFile data)
        {
            var errors = new List<string>();

            if (data == null)
            {
                errors.Add("file is empty or not a scenario");
                return errors;
            }

            RequireText(errors, data.id, "id");
            RequireText(errors, data.sceneName, "sceneName");
            RequireText(errors, data.displayName, "displayName");
            RequireName(errors, data.assetPrefix, "assetPrefix");
            RequireName(errors, data.violationFolder, "violationFolder");
            RequireName(errors, data.scenarioAssetName, "scenarioAssetName");

            if (data.availableDifficulties == null || data.availableDifficulties.Length == 0)
                errors.Add("availableDifficulties is empty");
            else
                foreach (string d in data.availableDifficulties)
                    if (!Enum.TryParse<DifficultyLevel>(d, out _))
                        errors.Add($"unknown difficulty '{d}' in availableDifficulties");

            if (data.violations == null || data.violations.Length == 0)
            {
                errors.Add("violations is empty");
                return errors;
            }

            var seen = new HashSet<string>();
            foreach (var v in data.violations)
            {
                string label = string.IsNullOrEmpty(v.violationId) ? "(violation without id)" : v.violationId;

                RequireName(errors, v.violationId, $"{label}: violationId");
                if (!string.IsNullOrEmpty(v.violationId) && !seen.Add(v.violationId))
                    errors.Add($"duplicate violationId '{v.violationId}'");

                if (!ConceptIds.IsKnown(v.conceptId))
                    errors.Add($"{label}: unknown conceptId '{v.conceptId}'");
                if (!Enum.TryParse<ViolationSeverity>(v.severity, out _))
                    errors.Add($"{label}: unknown severity '{v.severity}'");
                if (!Enum.TryParse<DifficultyLevel>(v.minimumDifficulty, out _))
                    errors.Add($"{label}: unknown minimumDifficulty '{v.minimumDifficulty}'");

                RequireText(errors, v.description, $"{label}: description");
                RequireText(errors, v.necArticle, $"{label}: necArticle");
                RequireText(errors, v.componentObjectName, $"{label}: componentObjectName");
            }

            return errors;
        }

        private static void RequireText(List<string> errors, string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value))
                errors.Add($"{field} is missing");
        }

        // Names become folder and asset names, so only allow plain identifiers.
        private static void RequireName(List<string> errors, string value, string field)
        {
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "^[A-Za-z0-9_-]+$"))
                errors.Add($"{field} must contain only letters, digits, '_' or '-' (got '{value}')");
        }

        private static void EnsureFolder(string parent, string name)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{name}"))
                AssetDatabase.CreateFolder(parent, name);
        }
    }
}
