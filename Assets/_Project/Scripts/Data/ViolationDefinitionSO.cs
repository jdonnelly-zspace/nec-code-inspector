using UnityEngine;
using NECInspector.Core;

namespace NECInspector.Data
{
    [CreateAssetMenu(fileName = "ViolationDefinition", menuName = "NEC Inspector/Violation Definition")]
    public class ViolationDefinitionSO : ScriptableObject
    {
        [Header("Identity")]
        public string violationId;
        [TextArea(2, 4)]
        public string description;

        [Header("Concept")]
        [Tooltip("Code-neutral concept this violation tests. Use a value from ConceptIds.")]
        public string conceptId;

        [Header("Citations")]
        [Tooltip("One citation per code profile this violation applies to. No citation for a profile = does not apply to it.")]
        public ViolationCitation[] citations;

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

        /// <summary>The citation for a code profile, or null if this violation does not apply to it.</summary>
        public ViolationCitation GetCitation(string profileId)
        {
            return ViolationCitations.Find(citations, profileId);
        }

        public bool AppliesTo(string profileId)
        {
            return GetCitation(profileId) != null;
        }
    }
}
