using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Core;

namespace NECInspector.Data
{
    /// <summary>
    /// Picks which panel design the sandbox gives a student: one written for the student's code, offered at the student's level, and preferably one they
    /// have not played lately, so repeat visits cycle through every design before any repeats. Pure logic with no engine types, so it is tested directly.
    /// </summary>
    public static class PanelDesignChooser
    {
        public struct Candidate
        {
            public string id;                   // unique per design (the panel type)
            public string profileId;            // the code the design was written for
            public DifficultyLevel[] levels;    // the levels it is offered at
        }

        /// <summary>
        /// The id of the design to play, or null if the code has no design at all (the sandbox is then not available for it).
        /// If no design is offered at the level, a design of the code at another level is used rather than none.
        /// recentIds lists the designs played lately, oldest first.
        /// </summary>
        public static string Choose(IReadOnlyList<Candidate> designs, string profileId, DifficultyLevel level, IReadOnlyList<string> recentIds, int seed)
        {
            if (designs == null || string.IsNullOrEmpty(profileId)) return null;

            var forCode = designs.Where(d => d.profileId == profileId && !string.IsNullOrEmpty(d.id)).ToList();
            if (forCode.Count == 0) return null;

            var pool = forCode.Where(d => d.levels != null && d.levels.Contains(level)).ToList();
            if (pool.Count == 0) pool = forCode;

            var recent = recentIds ?? new string[0];
            var unplayed = pool.Where(d => !recent.Contains(d.id)).ToList();
            if (unplayed.Count > 0)
                return unplayed[new Random(seed).Next(unplayed.Count)].id;

            // All played: the one played longest ago
            return pool.OrderBy(d => LastIndex(recent, d.id)).First().id;
        }

        private static int LastIndex(IReadOnlyList<string> list, string id)
        {
            for (int i = list.Count - 1; i >= 0; i--)
                if (list[i] == id) return i;
            return -1;
        }
    }
}
