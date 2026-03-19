using UnityEngine;
using NECInspector.Core;

namespace NECInspector.Data
{
    public enum ViolationSeverity
    {
        Minor,
        Major,
        Critical
    }

    [CreateAssetMenu(fileName = "ViolationDefinition", menuName = "NEC Inspector/Violation Definition")]
    public class ViolationDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string violationId;
        [TextArea(2, 4)]
        public string description;

        [Header("NEC Reference")]
        public string necArticle;        // e.g., "250.24(A)(1)"
        [TextArea(3, 6)]
        public string necArticleText;    // Full text for display

        [Header("Classification")]
        public ViolationSeverity severity = ViolationSeverity.Major;
        [Tooltip("Minimum difficulty level at which this violation appears")]
        public DifficultyLevel minimumDifficulty = DifficultyLevel.Beginner;
        [Tooltip("Mark true for subtle violations only visible at Expert level")]
        public bool isSubtle = false;

        [Header("Scene Binding")]
        [Tooltip("Name of the GameObject in the scene that has this violation")]
        public string componentObjectName;
        public Vector3 highlightOffset = Vector3.zero;

        [Header("Hints")]
        [TextArea(1, 3)]
        public string hintText;          // For Beginner mode scaffolding

        [Header("Display")]
        public string componentType;     // e.g., "Breaker", "Conductor", "Receptacle"
        [TextArea(1, 3)]
        public string inspectionNote;    // What the student should observe
    }
}
