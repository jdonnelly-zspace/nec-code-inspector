using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// How big each skill's violation pool is, per tier and code, against the targets in content-targets.json
    /// (from docs/CONTENT_PLAN.md). Gaps are warnings until a code's target is marked "enforce", then they fail.
    /// </summary>
    public static class ContentTargetsTests
    {
        private class TargetFile { public TargetEntry[] targets { get; set; } }

        private class TargetEntry
        {
            public string code { get; set; }
            public int beginner { get; set; }
            public int standard { get; set; }
            public int expert { get; set; }
            public bool enforce { get; set; }
        }

        private static readonly DifficultyLevel[] Levels = { DifficultyLevel.Beginner, DifficultyLevel.Standard, DifficultyLevel.Expert };
        private static readonly string[] TierNames = { "Foundation", "Practitioner", "Authority" };

        public static void Run(TestContext t)
        {
            CountingRule(t);
            AgainstTargets(t);
        }

        /// <summary>Pool size for a skill at a played difficulty under one code.</summary>
        public static int PoolSize(IEnumerable<ViolationFileData> violations, string profileId, string skill, DifficultyLevel played)
        {
            return violations.Count(v =>
                v.conceptId == skill
                && ViolationCitations.Find(v.citations, profileId) != null
                && Enum.TryParse<DifficultyLevel>(v.minimumDifficulty, out var min)
                && ViolationPools.IsActive(min, v.isSubtle, played));
        }

        /// <summary>
        /// New violations a skill needs to reach the goals. A new Beginner violation also joins the Standard and Expert
        /// pools, and a new Standard one joins the Expert pool, so the goals are met from the lowest tier up.
        /// </summary>
        public static int NewViolationsNeeded(int[] pools, int[] goals)
        {
            int addBeginner = Math.Max(0, goals[0] - pools[0]);
            int addStandard = Math.Max(0, goals[1] - (pools[1] + addBeginner));
            int addExpert = Math.Max(0, goals[2] - (pools[2] + addBeginner + addStandard));
            return addBeginner + addStandard + addExpert;
        }

        private static ViolationFileData V(string skill, string min, bool subtle = false, string profile = "nec")
        {
            return new ViolationFileData
            {
                violationId = Guid.NewGuid().ToString(), conceptId = skill, minimumDifficulty = min, isSubtle = subtle,
                citations = new[] { new NECInspector.Data.ViolationCitation { profileId = profile, reference = "1" } }
            };
        }

        private static void CountingRule(TestContext t)
        {
            t.Begin("pool sizes");

            var violations = new[]
            {
                V(ConceptIds.ShockProtection, "Beginner"), V(ConceptIds.ShockProtection, "Standard"), V(ConceptIds.ShockProtection, "Expert"),
                V(ConceptIds.ShockProtection, "Beginner", subtle: true), V(ConceptIds.ShockProtection, "Beginner", profile: "cec"),
                V(ConceptIds.EarthingBonding, "Beginner")
            };

            t.Equal(1, PoolSize(violations, "nec", ConceptIds.ShockProtection, DifficultyLevel.Beginner), "Beginner pool: only the plain Beginner violation");
            t.Equal(2, PoolSize(violations, "nec", ConceptIds.ShockProtection, DifficultyLevel.Standard), "Standard pool adds the Standard one, not the subtle one");
            t.Equal(4, PoolSize(violations, "nec", ConceptIds.ShockProtection, DifficultyLevel.Expert), "Expert pool adds the Expert and the subtle ones");
            t.Equal(1, PoolSize(violations, "cec", ConceptIds.ShockProtection, DifficultyLevel.Beginner), "another code counts only its own citations");
            t.Equal(0, PoolSize(violations, "bs7671", ConceptIds.ShockProtection, DifficultyLevel.Expert), "a code with no citations has an empty pool");

            t.Equal(0, NewViolationsNeeded(new[] { 4, 8, 12 }, new[] { 4, 8, 12 }), "a skill at its goals needs nothing");
            t.Equal(12, NewViolationsNeeded(new[] { 0, 0, 0 }, new[] { 4, 8, 12 }), "an empty skill needs one pool of the top size, not three");
            t.Equal(10, NewViolationsNeeded(new[] { 0, 2, 2 }, new[] { 4, 8, 12 }), "arc-fault today: 4 Beginner, 2 Standard, 4 Expert");
            t.Equal(1, NewViolationsNeeded(new[] { 4, 9, 11 }, new[] { 4, 8, 12 }), "earthing today: one more Expert violation");
        }

        private static void AgainstTargets(TestContext t)
        {
            t.Begin("violation pools against the content targets");

            string root = TestContext.RepoRoot();
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNameCaseInsensitive = true };
            var file = JsonSerializer.Deserialize<TargetFile>(File.ReadAllText(Path.Combine(root, "tests/LogicTests/content-targets.json")), options);
            t.IsTrue(file?.targets != null && file.targets.Length > 0, "the targets file lists at least one code");

            var violations = Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json")
                .Select(f => JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(f), options))
                .Where(s => s?.violations != null)
                .SelectMany(s => s.violations)
                .ToList();

            foreach (var target in file.targets)
            {
                int[] goal = { target.beginner, target.standard, target.expert };
                t.IsTrue(goal[0] <= goal[1] && goal[1] <= goal[2], $"{target.code}: pools grow with the tier (a higher tier sees at least what a lower one does)");

                var gaps = new List<string>();
                int needed = 0;
                foreach (string skill in ConceptIds.All)
                {
                    var have = Levels.Select(l => PoolSize(violations, target.code, skill, l)).ToArray();
                    var parts = new List<string>();
                    for (int i = 0; i < Levels.Length; i++)
                        if (have[i] < goal[i]) parts.Add($"{TierNames[i]} {have[i]}/{goal[i]}");
                    if (parts.Count == 0) continue;

                    needed += NewViolationsNeeded(have, goal);
                    gaps.Add($"{skill} ({string.Join(", ", parts)})");
                }

                if (gaps.Count == 0) continue;

                string summary = $"{target.code}: {gaps.Count} of {ConceptIds.All.Length} skills are below the pool targets {goal[0]}/{goal[1]}/{goal[2]} ({needed} new violations needed): {string.Join("; ", gaps)}";
                if (target.enforce) t.IsTrue(false, summary);
                else t.Warn(summary);
            }
        }
    }
}
