using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// Which companion profiles apply under which codes. A companion (fire alarm, line work) names the codes it supplements in
    /// profile.json; its violations then count under those codes without being cited again for each one.
    /// </summary>
    public static class ProfileSupplements
    {
        /// <summary>For each supplemented profile id, the ids of the companions that supplement it, in id order.</summary>
        public static Dictionary<string, string[]> Build(IEnumerable<ICodeProfile> profiles)
        {
            var map = new Dictionary<string, List<string>>(StringComparer.Ordinal);
            foreach (var profile in profiles ?? Enumerable.Empty<ICodeProfile>())
            {
                if (profile == null) continue;
                foreach (string supplemented in profile.Supplements ?? new string[0])
                {
                    if (string.IsNullOrEmpty(supplemented) || supplemented == profile.ProfileId) continue;
                    if (!map.TryGetValue(supplemented, out var list)) map[supplemented] = list = new List<string>();
                    if (!list.Contains(profile.ProfileId)) list.Add(profile.ProfileId);
                }
            }

            return map.ToDictionary(p => p.Key, p => p.Value.OrderBy(id => id, StringComparer.Ordinal).ToArray());
        }

        /// <summary>The profile itself first, then every companion that supplements it. Empty for a null or empty id.</summary>
        public static string[] CitationIds(IReadOnlyDictionary<string, string[]> map, string profileId)
        {
            if (string.IsNullOrEmpty(profileId)) return new string[0];
            if (map == null || !map.TryGetValue(profileId, out var companions)) return new[] { profileId };
            return new[] { profileId }.Concat(companions).ToArray();
        }
    }
}
