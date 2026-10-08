using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NECInspector.Core;
using NECInspector.PanelSandbox;
using NECInspector.Skills;

namespace NECInspector.Data
{
    // Shapes of the content JSON files under Assets/_Project/Content. Plain data with no engine
    // dependency, so the same types are used by the editor importers and by the automated content tests.
    // Enum values are stored by name and checked by ContentFileValidator.

    [Serializable]
    public class DifficultySettingsFile
    {
        public DifficultySettingsEntry[] settings;
    }

    [Serializable]
    public class DifficultySettingsEntry
    {
        public string level;                    // DifficultyLevel
        public string displayName;
        public bool showHighlightHints;
        public bool showScaffolding;
        public float scaffoldingTimeoutSeconds;
        public float hintCooldownSeconds;
        public string citationMode;             // CitationMode
        public bool enableTimeLimit;
        public int timeLimitSeconds;
        public bool penalizeFalsePositives;
        public float falsePositivePenalty;
        public bool showSimplifiedTerminology;
        public bool highlightNewInEdition;
        public bool includeSubtleViolations;
    }

    [Serializable]
    public class CertificateFile
    {
        public CertificateEntry[] templates;
    }

    [Serializable]
    public class CertificateEntry
    {
        public string assetName;                // CertTemplate_{assetName}.asset
        public string certificateId;
        public string certificateTitle;
        public string descriptionTemplate;      // {StudentName}, {Date}, {Score}, {Difficulty}
        public string type;                     // CertificateType
        public float minimumAccuracy;
        public string[] requiredSkills;
        public bool requiresAllSkills;          // every skill in ConceptIds (instead of listing them)
        public string requiredTier;             // SkillTier name; empty means Practitioner
        public string[] requiredScenarios;
        public bool requiresSandbox;
        public float[] accentColor;             // r, g, b, a in 0..1
    }

    [Serializable]
    public class QuickReferenceFile
    {
        public QuickReferenceEntry[] cards;
    }

    [Serializable]
    public class QuickReferenceEntry
    {
        public string cardId;
        public string profileId;                // the installation code the card is written for
        public string title;
        public string category;                 // CardCategory
        public string summary;
        public string keyRule;
        public string[] codeReferences;
        public string[] keywords;
        public string minimumDifficulty;        // DifficultyLevel
    }

    [Serializable]
    public class SandboxFile
    {
        public SandboxDesignEntry[] designs;
    }

    [Serializable]
    public class SandboxDesignEntry
    {
        public string assetName;                // PanelDesign_{assetName}.asset
        public string profileId;                // the installation code its references and conductors come from
        public string panelType;
        public string displayName;
        public string description;
        public int totalAmps;
        public int totalSlots;
        public float dwellingArea;              // in the code's area unit
        public float targetLoadVA;              // the expected calculated load, for scoring
        public float loadCalcTolerancePercent;
        public int expertTimeLimit;
        public string[] availableDifficulties;
        public RequiredCircuit[] requiredCircuits;
    }

    public static class ContentFileValidator
    {
        private static readonly string[] CertificatePlaceholders = { "StudentName", "Date", "Score", "Difficulty" };

        public static List<string> Validate(DifficultySettingsFile file)
        {
            var errors = new List<string>();
            if (file?.settings == null || file.settings.Length == 0)
            {
                errors.Add("settings is empty");
                return errors;
            }

            var seen = new HashSet<string>();
            foreach (var s in file.settings)
            {
                string label = string.IsNullOrEmpty(s?.level) ? "(setting without level)" : s.level;
                if (s == null) { errors.Add("a setting is empty"); continue; }
                if (!IsEnumName<DifficultyLevel>(s.level)) errors.Add($"{label}: unknown level");
                else if (!seen.Add(s.level)) errors.Add($"duplicate level '{s.level}'");
                RequireText(errors, s.displayName, $"{label}: displayName");
                if (!IsEnumName<CitationMode>(s.citationMode)) errors.Add($"{label}: unknown citationMode '{s.citationMode}'");
                if (s.timeLimitSeconds < 0) errors.Add($"{label}: timeLimitSeconds is negative");
                if (s.enableTimeLimit && s.timeLimitSeconds <= 0) errors.Add($"{label}: the time limit is on but timeLimitSeconds is not above zero");
                if (s.falsePositivePenalty < 0f || s.falsePositivePenalty > 1f) errors.Add($"{label}: falsePositivePenalty must be between 0 and 1");
            }

            foreach (string level in Enum.GetNames(typeof(DifficultyLevel)))
                if (!seen.Contains(level)) errors.Add($"no settings for difficulty '{level}'");

            return errors;
        }

        public static List<string> Validate(CertificateFile file)
        {
            var errors = new List<string>();
            if (file?.templates == null || file.templates.Length == 0)
            {
                errors.Add("templates is empty");
                return errors;
            }

            var ids = new HashSet<string>();
            var names = new HashSet<string>();
            foreach (var c in file.templates)
            {
                string label = string.IsNullOrEmpty(c?.certificateId) ? "(certificate without id)" : c.certificateId;
                if (c == null) { errors.Add("a template is empty"); continue; }
                RequireName(errors, c.assetName, $"{label}: assetName");
                if (!string.IsNullOrEmpty(c.assetName) && !names.Add(c.assetName)) errors.Add($"duplicate assetName '{c.assetName}'");
                RequireText(errors, c.certificateId, $"{label}: certificateId");
                if (!string.IsNullOrEmpty(c.certificateId) && !ids.Add(c.certificateId)) errors.Add($"duplicate certificateId '{c.certificateId}'");
                RequireText(errors, c.certificateTitle, $"{label}: certificateTitle");
                RequireText(errors, c.descriptionTemplate, $"{label}: descriptionTemplate");
                if (!IsEnumName<CertificateType>(c.type)) errors.Add($"{label}: unknown type '{c.type}'");
                if (c.minimumAccuracy < 0f || c.minimumAccuracy > 1f) errors.Add($"{label}: minimumAccuracy must be between 0 and 1");
                if (!string.IsNullOrEmpty(c.requiredTier) && !SkillTiers.TryParse(c.requiredTier, out _)) errors.Add($"{label}: unknown requiredTier '{c.requiredTier}'");
                if (c.requiresAllSkills && c.requiredSkills != null && c.requiredSkills.Length > 0) errors.Add($"{label}: requiresAllSkills and requiredSkills cannot both be set");
                foreach (string skill in c.requiredSkills ?? new string[0])
                    if (!ConceptIds.IsKnown(skill)) errors.Add($"{label}: unknown skill '{skill}'");
                if (c.accentColor == null || c.accentColor.Length != 4) errors.Add($"{label}: accentColor needs four numbers (r, g, b, a)");
                else foreach (float v in c.accentColor) if (v < 0f || v > 1f) { errors.Add($"{label}: accentColor values must be between 0 and 1"); break; }

                if (!string.IsNullOrEmpty(c.descriptionTemplate))
                    foreach (Match m in Regex.Matches(c.descriptionTemplate, @"\{([^}]*)\}"))
                        if (Array.IndexOf(CertificatePlaceholders, m.Groups[1].Value) < 0)
                            errors.Add($"{label}: unknown placeholder {{{m.Groups[1].Value}}} (use {string.Join(", ", CertificatePlaceholders)})");
            }

            return errors;
        }

        public static List<string> Validate(QuickReferenceFile file)
        {
            var errors = new List<string>();
            if (file?.cards == null || file.cards.Length == 0)
            {
                errors.Add("cards is empty");
                return errors;
            }

            var ids = new HashSet<string>();
            foreach (var c in file.cards)
            {
                string label = string.IsNullOrEmpty(c?.cardId) ? "(card without id)" : c.cardId;
                if (c == null) { errors.Add("a card is empty"); continue; }
                RequireName(errors, c.cardId, $"{label}: cardId");
                if (!string.IsNullOrEmpty(c.cardId) && !ids.Add(c.cardId)) errors.Add($"duplicate cardId '{c.cardId}'");
                RequireName(errors, c.profileId, $"{label}: profileId");
                RequireText(errors, c.title, $"{label}: title");
                RequireText(errors, c.summary, $"{label}: summary");
                RequireText(errors, c.keyRule, $"{label}: keyRule");
                if (!IsEnumName<CardCategory>(c.category)) errors.Add($"{label}: unknown category '{c.category}'");
                if (!IsEnumName<DifficultyLevel>(c.minimumDifficulty)) errors.Add($"{label}: unknown minimumDifficulty '{c.minimumDifficulty}'");
                if (c.codeReferences == null || c.codeReferences.Length == 0) errors.Add($"{label}: codeReferences is empty");
                if (c.keywords == null || c.keywords.Length == 0) errors.Add($"{label}: keywords is empty");
            }

            return errors;
        }

        public static List<string> Validate(SandboxFile file)
        {
            var errors = new List<string>();
            if (file?.designs == null || file.designs.Length == 0)
            {
                errors.Add("designs is empty");
                return errors;
            }

            var names = new HashSet<string>();
            foreach (var d in file.designs)
            {
                string label = string.IsNullOrEmpty(d?.assetName) ? "(design without assetName)" : d.assetName;
                if (d == null) { errors.Add("a design is empty"); continue; }
                RequireName(errors, d.assetName, $"{label}: assetName");
                if (!string.IsNullOrEmpty(d.assetName) && !names.Add(d.assetName)) errors.Add($"duplicate assetName '{d.assetName}'");
                RequireName(errors, d.profileId, $"{label}: profileId");
                RequireText(errors, d.panelType, $"{label}: panelType");
                RequireText(errors, d.displayName, $"{label}: displayName");
                RequireText(errors, d.description, $"{label}: description");
                if (d.totalAmps <= 0) errors.Add($"{label}: totalAmps must be above zero");
                if (d.totalSlots <= 0) errors.Add($"{label}: totalSlots must be above zero");
                if (d.dwellingArea <= 0f) errors.Add($"{label}: dwellingArea must be above zero");
                if (d.targetLoadVA <= 0f) errors.Add($"{label}: targetLoadVA must be above zero");
                if (d.loadCalcTolerancePercent < 0f) errors.Add($"{label}: loadCalcTolerancePercent is negative");
                if (d.availableDifficulties == null || d.availableDifficulties.Length == 0) errors.Add($"{label}: availableDifficulties is empty");
                else foreach (string level in d.availableDifficulties)
                    if (!IsEnumName<DifficultyLevel>(level)) errors.Add($"{label}: unknown difficulty '{level}'");

                if (d.requiredCircuits == null || d.requiredCircuits.Length == 0) { errors.Add($"{label}: requiredCircuits is empty"); continue; }

                var circuits = new HashSet<string>();
                foreach (var r in d.requiredCircuits)
                {
                    string circuit = string.IsNullOrEmpty(r?.circuitName) ? "(circuit without a name)" : r.circuitName;
                    if (r == null) { errors.Add($"{label}: a circuit is empty"); continue; }
                    RequireText(errors, r.circuitName, $"{label}: circuitName");
                    if (!string.IsNullOrEmpty(r.circuitName) && !circuits.Add(r.circuitName)) errors.Add($"{label}: duplicate circuit '{r.circuitName}'");
                    if (r.ampsRequired <= 0) errors.Add($"{label}: {circuit}: ampsRequired must be above zero");
                    if (r.poleCount != 1 && r.poleCount != 2) errors.Add($"{label}: {circuit}: poleCount must be 1 or 2");
                    RequireText(errors, r.wireGauge, $"{label}: {circuit}: wireGauge");
                    RequireText(errors, r.codeReference, $"{label}: {circuit}: codeReference");
                    RequireText(errors, r.description, $"{label}: {circuit}: description");
                }
            }

            return errors;
        }

        private static bool IsEnumName<T>(string value) where T : struct
        {
            return !string.IsNullOrEmpty(value) && Array.IndexOf(Enum.GetNames(typeof(T)), value) >= 0;
        }

        private static void RequireText(List<string> errors, string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) errors.Add($"{field} is missing");
        }

        // Names become asset names, so only allow plain identifiers.
        private static void RequireName(List<string> errors, string value, string field)
        {
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "^[A-Za-z0-9_-]+$"))
                errors.Add($"{field} must contain only letters, digits, '_' or '-' (got '{value}')");
        }
    }
}
