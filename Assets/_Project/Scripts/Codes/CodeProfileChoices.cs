using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// One row in the code profile picker: what a student sees about an installation code
    /// before choosing it. Plain data, so the wording is tested without Unity.
    /// </summary>
    public class CodeProfileChoice
    {
        public string id;
        public string displayName;
        public string edition;
        public string regionName;
        public string reviewStatus;
        public bool isActive;
        public bool hasOwnTables;
        public int availableScenarios;
        public int totalScenarios;
        public bool artAvailable = true;   // false when no art exists for the code's art set (see ArtSets)

        public string Label => $"{displayName} {edition}" + (isActive ? "  (current)" : "");

        public string StatusText
        {
            get
            {
                switch (reviewStatus)
                {
                    case CodeProfileManifest.StatusReviewed: return "Reviewed by a credential expert";
                    case CodeProfileManifest.StatusDraft: return "Draft: not yet reviewed by a credential expert, and the content is partial";
                    case CodeProfileManifest.StatusAppDefined: return "Written for this app; review by a credential expert is pending";
                    default: return "Review status unknown";
                }
            }
        }

        public string ScenarioText
        {
            get
            {
                if (!artAvailable) return "Scenarios not available for this code yet: no scene art for its region";
                if (totalScenarios <= 0) return "";
                if (availableScenarios >= totalScenarios) return $"All {totalScenarios} scenarios available";
                if (availableScenarios <= 0) return "No scenarios available yet";
                return $"{availableScenarios} of {totalScenarios} scenarios available";
            }
        }

        public string SandboxText => hasOwnTables ? "" : "Panel sandbox: not available for this code yet";

        /// <summary>Several short lines for the detail area of the picker.</summary>
        public string Details
        {
            get
            {
                var lines = new List<string>();
                if (!string.IsNullOrEmpty(regionName)) lines.Add(regionName);
                lines.Add(StatusText);
                if (ScenarioText.Length > 0) lines.Add(ScenarioText);
                if (SandboxText.Length > 0) lines.Add(SandboxText);
                return string.Join("\n", lines);
            }
        }
    }

    public static class CodeProfileChoices
    {
        /// <summary>
        /// Choices for every loaded profile: the default code first, then the rest by name.
        /// availableScenarios tells how many scenarios apply to a profile; it is not used (zero) for a
        /// profile whose art set has no art.
        /// </summary>
        public static List<CodeProfileChoice> Build(IEnumerable<ICodeProfile> profiles, string activeId,
            Func<string, int> availableScenarios, int totalScenarios)
        {
            var choices = new List<CodeProfileChoice>();
            if (profiles == null) return choices;

            foreach (var profile in profiles)
            {
                // Scenes are only offered under codes whose art exists (docs/SCENE_DESIGN.md)
                bool art = ArtSets.IsAvailable(profile.ArtSet);

                choices.Add(new CodeProfileChoice
                {
                    id = profile.ProfileId,
                    displayName = profile.DisplayName,
                    edition = profile.Edition,
                    regionName = RegionName(profile.Region),
                    reviewStatus = profile.ReviewStatus,
                    isActive = profile.ProfileId == activeId,
                    hasOwnTables = profile.HasOwnTables,
                    availableScenarios = art && availableScenarios != null ? availableScenarios(profile.ProfileId) : 0,
                    totalScenarios = totalScenarios,
                    artAvailable = art
                });
            }

            return choices
                .OrderBy(c => c.id == CodeProfileIds.Nec ? 0 : 1)
                .ThenBy(c => c.displayName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>A readable region name for a region code; unknown codes are shown as given.</summary>
        public static string RegionName(string code)
        {
            switch ((code ?? "").ToUpperInvariant())
            {
                case "": return "";
                case "US": return "United States";
                case "CA": return "Canada";
                case "UK":
                case "GB": return "United Kingdom";
                case "EU": return "European Union";
                case "FR": return "France";
                case "DE": return "Germany";
                default: return code;
            }
        }
    }
}
