using System.Collections.Generic;
using UnityEngine;

namespace NECInspector.Data
{
    [CreateAssetMenu(fileName = "ScenarioCatalog", menuName = "NEC Inspector/Scenario Catalog")]
    public class ScenarioCatalogSO : ScriptableObject
    {
        public List<ScenarioDefinitionSO> scenarios;

        public ScenarioDefinitionSO GetScenarioBySceneName(string sceneName)
        {
            return scenarios.Find(s => s.sceneName == sceneName);
        }

        public ScenarioDefinitionSO GetScenarioById(string id)
        {
            return scenarios.Find(s => s.id == id);
        }
    }
}
