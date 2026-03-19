using UnityEngine;

namespace NECInspector.Core
{
    public enum NECCitationMode
    {
        Dropdown,           // Beginner: pick from list
        SearchableDropdown, // Standard: type to search
        FreeText            // Expert: type exact article
    }

    [CreateAssetMenu(fileName = "DifficultySettings", menuName = "NEC Inspector/Difficulty Settings")]
    public class DifficultySettingsSO : ScriptableObject
    {
        [Header("Identity")]
        public DifficultyLevel level;
        public string displayName;

        [Header("Hints & Scaffolding")]
        public bool showHighlightHints = false;
        public bool showScaffolding = false;
        public float scaffoldingTimeoutSeconds = -1f;
        public float hintCooldownSeconds = 30f;

        [Header("NEC Citation")]
        public NECCitationMode citationMode = NECCitationMode.SearchableDropdown;

        [Header("Time")]
        public bool enableTimeLimit = false;
        public int timeLimitSeconds = 0;

        [Header("Scoring")]
        public bool penalizeFalsePositives = false;
        public float falsePositivePenalty = 0.1f;

        [Header("Content")]
        public bool showSimplifiedTerminology = false;
        public bool highlight2026Changes = false;
        public bool includeSubtleViolations = false;
    }
}
