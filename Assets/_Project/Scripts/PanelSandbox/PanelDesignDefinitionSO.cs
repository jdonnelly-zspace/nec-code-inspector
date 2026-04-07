using UnityEngine;
using NECInspector.Core;

namespace NECInspector.PanelSandbox
{
    [CreateAssetMenu(fileName = "PanelDesignDefinition", menuName = "NEC Inspector/Panel Design Definition")]
    public class PanelDesignDefinitionSO : ScriptableObject
    {
        [HideInInspector] public string id;

        [Header("Panel")]
        public string panelType = "Residential 200A";
        public string displayName;
        [TextArea(2, 4)]
        public string description;
        public int totalAmps = 200;
        public int totalSlots = 40;

        [Header("Required Circuits")]
        public RequiredCircuit[] requiredCircuits;

        [Header("Load Calculation")]
        [Tooltip("Expected total load in VA for scoring comparison")]
        public float targetLoadVA;
        [Tooltip("Acceptable % error for load calculation (e.g., 10 = within 10%)")]
        public float loadCalcTolerancePercent = 10f;
        [Tooltip("Dwelling square footage for general lighting calculation")]
        public float dwellingSquareFootage = 2000f;

        [Header("Difficulty")]
        public DifficultyLevel[] availableDifficulties = {
            DifficultyLevel.Beginner,
            DifficultyLevel.Standard,
            DifficultyLevel.Expert
        };

        [Header("Time")]
        [Tooltip("Time limit in seconds for Expert mode. 0 = no limit.")]
        public int expertTimeLimit = 1800; // 30 minutes

        [Header("NEC Reference")]
        public string[] necChapters;

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = System.Guid.NewGuid().ToString();
            }
        }
    }
}
