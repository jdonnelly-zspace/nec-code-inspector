using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NECInspector.Codes;
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

        [Header("Time")]
        [Tooltip("Time limit in seconds for Expert mode. 0 = no limit.")]
        public int expertTimeLimit = 1200; // 20 minutes

        [Header("Environment")]
        public string environmentDescription;

        /// <summary>
        /// The code's sections (chapters, parts, ...) this scenario touches, worked out from the
        /// violations' citations in the given profile, in ascending order.
        /// </summary>
        public string[] GetSections(ICodeProfile profile)
        {
            var sections = new SortedSet<int>();
            if (profile == null || violations == null) return new string[0];

            foreach (var violation in violations)
            {
                var citation = violation?.GetCitation(profile.ProfileId);
                if (citation == null) continue;

                var article = profile.GetArticle(citation.reference);
                if (article != null) sections.Add(article.chapter);
            }

            return sections.Select(s => s.ToString()).ToArray();
        }

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
