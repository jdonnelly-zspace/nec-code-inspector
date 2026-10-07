using UnityEngine;
using NECInspector.Core;

namespace NECInspector.Data
{
    [CreateAssetMenu(fileName = "ScenarioDefinition", menuName = "NEC Inspector/Scenario Definition")]
    public class ScenarioDefinitionSO : ScriptableObject
    {
        [HideInInspector] public string id;

        [Header("Scene")]
        public string sceneName;
        public string displayName;
        [TextArea(2, 4)]
        public string description;

        [Header("Difficulty")]
        public DifficultyLevel[] availableDifficulties = {
            DifficultyLevel.Beginner,
            DifficultyLevel.Standard,
            DifficultyLevel.Expert
        };

        [Header("Content")]
        public ViolationDefinitionSO[] violations;
        public string[] necChapters;

        [Header("Time")]
        [Tooltip("Time limit in seconds for Expert mode. 0 = no limit.")]
        public int expertTimeLimit = 1200; // 20 minutes

        [Header("Environment")]
        public string environmentDescription;

        /// <summary>
        /// True if at least one violation in this scenario applies to the code profile.
        /// </summary>
        public bool AppliesTo(string profileId)
        {
            if (violations == null) return false;

            foreach (var violation in violations)
            {
                if (violation != null && violation.AppliesTo(profileId))
                    return true;
            }

            return false;
        }

        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
            {
                id = System.Guid.NewGuid().ToString();
            }
        }
    }
}
