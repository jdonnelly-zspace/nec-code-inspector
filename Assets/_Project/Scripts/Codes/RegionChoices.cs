using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// One row in the region picker. The student chooses where they work or study; the app picks the
    /// installation code for that region behind the scenes and never shows its name. Plain data, so the
    /// wording is tested without Unity.
    /// </summary>
    public class RegionChoice
    {
        public string regionCode;           // normalised, e.g. "US", "CA", "GB"
        public string regionName;           // e.g. "United States"
        public string profileId;            // the code applied for this region; internal, never shown
        public string units;                // imperial or metric
        public bool isActive;
        public bool hasOwnTables;
        public int availableScenarios;
        public int totalScenarios;
        public bool artAvailable = true;

        public string Label => regionName + (isActive ? "  (current)" : "");

        public string ScenarioText
        {
            get
            {
                if (!artAvailable) return "Scenarios are not available for this region yet";
                if (totalScenarios <= 0) return "";
                if (availableScenarios >= totalScenarios) return $"All {totalScenarios} scenarios available";
                if (availableScenarios <= 0) return "No scenarios available yet";
                return $"{availableScenarios} of {totalScenarios} scenarios available";
            }
        }

        public string SandboxText => hasOwnTables ? "" : "Panel sandbox: not available for this region yet";

        /// <summary>Several short lines for the detail area of the picker.</summary>
        public string Details
        {
            get
            {
                var lines = new List<string> { UnitSystems.Describe(units) };
                if (ScenarioText.Length > 0) lines.Add(ScenarioText);
                if (SandboxText.Length > 0) lines.Add(SandboxText);
                return string.Join("\n", lines.Where(l => l.Length > 0));
            }
        }
    }

    public static class RegionChoices
    {
        /// <summary>Region codes are upper case, and "UK" is written "GB".</summary>
        public static string Normalize(string region)
        {
            string code = (region ?? "").Trim().ToUpperInvariant();
            return code == "UK" ? "GB" : code;
        }

        /// <summary>
        /// The code to apply for a region: reviewed codes first, then app-defined, then draft, then by id.
        /// Returns null if no loaded code belongs to the region.
        /// </summary>
        public static ICodeProfile ProfileForRegion(IEnumerable<ICodeProfile> profiles, string region)
        {
            string wanted = Normalize(region);
            if (wanted.Length == 0 || profiles == null) return null;

            return profiles
                .Where(p => p != null && Normalize(p.Region) == wanted)
                .OrderBy(p => StatusRank(p.ReviewStatus))
                .ThenBy(p => p.ProfileId, StringComparer.Ordinal)
                .FirstOrDefault();
        }

        private static int StatusRank(string status)
        {
            switch (status)
            {
                case CodeProfileManifest.StatusReviewed: return 0;
                case CodeProfileManifest.StatusAppDefined: return 1;
                default: return 2;
            }
        }

        /// <summary>
        /// One choice per region that has a loaded code, the default code's region first, then by name.
        /// availableScenarios tells how many scenarios apply under a profile; it is zero for a profile whose
        /// art set has no art.
        /// </summary>
        public static List<RegionChoice> Build(IEnumerable<ICodeProfile> profiles, string activeProfileId,
            Func<string, int> availableScenarios, int totalScenarios, string defaultProfileId = CodeProfileIds.Nec)
        {
            var choices = new List<RegionChoice>();
            if (profiles == null) return choices;

            var all = profiles.Where(p => p != null).ToList();
            string defaultRegion = Normalize(all.FirstOrDefault(p => p.ProfileId == defaultProfileId)?.Region);

            foreach (string region in all.Select(p => Normalize(p.Region)).Where(r => r.Length > 0).Distinct())
            {
                var profile = ProfileForRegion(all, region);
                bool art = ArtSets.IsAvailable(profile.ArtSet);

                choices.Add(new RegionChoice
                {
                    regionCode = region,
                    regionName = RegionNames.Of(region),
                    profileId = profile.ProfileId,
                    units = profile.Units,
                    isActive = profile.ProfileId == activeProfileId,
                    hasOwnTables = profile.HasOwnTables,
                    availableScenarios = art && availableScenarios != null ? availableScenarios(profile.ProfileId) : 0,
                    totalScenarios = totalScenarios,
                    artAvailable = art
                });
            }

            return choices
                .OrderBy(c => c.regionCode == defaultRegion ? 0 : 1)
                .ThenBy(c => c.regionName, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
    }
}
