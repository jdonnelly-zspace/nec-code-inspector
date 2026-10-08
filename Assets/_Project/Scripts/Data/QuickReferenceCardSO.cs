using UnityEngine;
using UnityEngine.Serialization;

namespace NECInspector.Data
{
    [CreateAssetMenu(fileName = "QuickReferenceCard", menuName = "NEC Inspector/Quick Reference Card")]
    public class QuickReferenceCardSO : ScriptableObject
    {
        [Header("Identity")]
        public string cardId;
        [Tooltip("The installation code this card is written for (for example nec); empty shows it under every code")]
        public string profileId;
        public string title;
        public CardCategory category;

        [Header("Content")]
        [TextArea(3, 8)]
        public string summary;
        [TextArea(2, 4)]
        public string keyRule;
        [FormerlySerializedAs("necReferences")]
        public string[] codeReferences;     // e.g., "210.8(A)", "240.4(D)"
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
