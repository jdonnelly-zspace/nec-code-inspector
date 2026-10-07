using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NECInspector.Core;

namespace NECInspector.Data
{
    /// <summary>
    /// Shape of a scenario JSON file in Assets/_Project/Content/Scenarios.
    /// Plain data with no engine dependency, so the same type is used by the editor importer
    /// and by the automated content tests.
    /// </summary>
    [Serializable]
    public class ScenarioFileData
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
        public ViolationFileData[] violations;
    }

    [Serializable]
    public class ViolationFileData
    {
        public string violationId;
        public string conceptId;
        public string description;
        public ViolationCitation[] citations;   // one per code profile the violation applies to
        public string severity;            // Minor | Major | Critical
        public string minimumDifficulty;   // Beginner | Standard | Expert
        public bool isSubtle;
        public string componentObjectName;
        public string hintText;
        public string componentType;
        public string inspectionNote;
    }

    public static class ScenarioFileValidator
    {
        /// <summary>
        /// Check a scenario file before any asset is touched. Returns one message per problem.
        /// </summary>
        public static List<string> Validate(ScenarioFileData data)
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
            {
                errors.Add("availableDifficulties is empty");
            }
            else
            {
                foreach (string d in data.availableDifficulties)
                    if (!IsEnumName<DifficultyLevel>(d))
                        errors.Add($"unknown difficulty '{d}' in availableDifficulties");
            }

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
                if (!IsEnumName<ViolationSeverity>(v.severity))
                    errors.Add($"{label}: unknown severity '{v.severity}'");
                if (!IsEnumName<DifficultyLevel>(v.minimumDifficulty))
                    errors.Add($"{label}: unknown minimumDifficulty '{v.minimumDifficulty}'");

                RequireText(errors, v.description, $"{label}: description");
                RequireText(errors, v.componentObjectName, $"{label}: componentObjectName");
                ValidateCitations(errors, v.citations, label);
            }

            return errors;
        }

        // A violation needs at least one citation, with one citation at most per code profile.
        private static void ValidateCitations(List<string> errors, ViolationCitation[] citations, string label)
        {
            if (citations == null || citations.Length == 0)
            {
                errors.Add($"{label}: citations is empty (a violation needs a citation for at least one code profile)");
                return;
            }

            var profiles = new HashSet<string>();
            foreach (var citation in citations)
            {
                if (citation == null)
                {
                    errors.Add($"{label}: citations contains an empty entry");
                    continue;
                }

                RequireName(errors, citation.profileId, $"{label}: citation profileId");
                RequireText(errors, citation.reference, $"{label}: citation reference");

                if (!string.IsNullOrEmpty(citation.profileId) && !profiles.Add(citation.profileId))
                    errors.Add($"{label}: more than one citation for profile '{citation.profileId}'");
            }
        }

        // Enum.TryParse also accepts numbers such as "7"; require a real member name.
        private static bool IsEnumName<T>(string value) where T : struct
        {
            return !string.IsNullOrEmpty(value) && Array.IndexOf(Enum.GetNames(typeof(T)), value) >= 0;
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
    }
}
