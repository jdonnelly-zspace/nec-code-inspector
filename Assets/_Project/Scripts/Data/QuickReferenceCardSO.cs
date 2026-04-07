using UnityEngine;

namespace NECInspector.Data
{
    public enum CardCategory
    {
        General,
        Grounding,
        BranchCircuits,
        Overcurrent,
        WireSizing,
        GFCIProtection,
        AFCIProtection,
        LoadCalculation,
        PanelDesign,
        SpecialLocations
    }

    [CreateAssetMenu(fileName = "QuickReferenceCard", menuName = "NEC Inspector/Quick Reference Card")]
    public class QuickReferenceCardSO : ScriptableObject
    {
        [Header("Identity")]
        public string cardId;
        public string title;
        public CardCategory category;

        [Header("Content")]
        [TextArea(3, 8)]
        public string summary;
        [TextArea(2, 4)]
        public string keyRule;
        public string[] necReferences;      // e.g., "210.8(A)", "240.4(D)"
        public string[] keywords;

        [Header("Visual")]
        public Sprite icon;
        public Color accentColor = Color.white;

        [Header("Difficulty")]
        [Tooltip("Minimum difficulty to show this card")]
        public Core.DifficultyLevel minimumDifficulty = Core.DifficultyLevel.Beginner;

        public string DisplayTitle => $"{title} ({category})";
    }
}
