using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// All the panel designs the sandbox can offer. Generated from Content/Sandbox/panel-designs.json (NEC Inspector > Generate Panel Sandbox Data).
    /// </summary>
    [CreateAssetMenu(fileName = "PanelDesignCatalog", menuName = "NEC Inspector/Panel Design Catalog")]
    public class PanelDesignCatalogSO : ScriptableObject
    {
        public List<PanelDesignDefinitionSO> designs = new List<PanelDesignDefinitionSO>();

        /// <summary>True if the code has at least one design (otherwise the sandbox is not available for it).</summary>
        public bool HasDesignFor(string profileId) => designs.Any(d => d != null && d.profileId == profileId);

        /// <summary>The design to play for a code and level, preferring designs not played lately (see <see cref="PanelDesignChooser"/>); null if the code has none.</summary>
        public PanelDesignDefinitionSO Choose(string profileId, DifficultyLevel level, IReadOnlyList<string> recentPanelTypes, int seed)
        {
            var candidates = designs.Where(d => d != null)
                .Select(d => new PanelDesignChooser.Candidate { id = d.panelType, profileId = d.profileId, levels = d.availableDifficulties }).ToList();
            string id = PanelDesignChooser.Choose(candidates, profileId, level, recentPanelTypes, seed);
            return id == null ? null : designs.FirstOrDefault(d => d != null && d.panelType == id);
        }
    }
}
