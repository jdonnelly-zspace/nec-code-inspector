using UnityEngine;
using UnityEngine.Serialization;

namespace NECInspector.Core
{
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

        [Header("Citation")]
        public CitationMode citationMode = CitationMode.SearchableDropdown;

        [Header("Time")]
        public bool enableTimeLimit = false;
        public int timeLimitSeconds = 0;

        [Header("Scoring")]
        public bool penalizeFalsePositives = false;
        public float falsePositivePenalty = 0.1f;

        [Header("Content")]
        public bool showSimplifiedTerminology = false;
        [FormerlySerializedAs("highlight2026Changes")]
        public bool highlightNewInEdition = false;
        public bool includeSubtleViolations = false;
    }
}
