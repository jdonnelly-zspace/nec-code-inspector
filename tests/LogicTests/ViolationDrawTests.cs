using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Pools and draws: which violations a session can show, and how a session picks from them.
    /// See docs/POOL_AND_DRAW.md.
    /// </summary>
    public static class ViolationDrawTests
    {
        private class V
        {
            public string id, skill;
            public override string ToString() => id;
        }

        public static void Run(TestContext t)
        {
            PoolRule(t);
            DrawBasics(t);
            DrawSpreadsSkills(t);
            DrawPrefersUnseen(t);
            SessionSizeField(t);
        }

        private static List<V> Pool(params string[] skills)
        {
            return skills.Select((s, i) => new V { id = $"V{i + 1:00}", skill = s }).ToList();
        }

        private static List<V> Draw(List<V> pool, int count, int seed, ISet<string> recent = null)
        {
            return ViolationDraw.Choose(pool, count, seed, v => v.skill, v => v.id, recent);
        }

        private static void PoolRule(TestContext t)
        {
            t.Begin("which violations are in a pool");

            t.IsTrue(ViolationPools.IsActive(DifficultyLevel.Beginner, false, DifficultyLevel.Beginner), "a Beginner violation shows at Beginner");
            t.IsTrue(ViolationPools.IsActive(DifficultyLevel.Beginner, false, DifficultyLevel.Expert), "and at every higher level");
            t.IsTrue(!ViolationPools.IsActive(DifficultyLevel.Standard, false, DifficultyLevel.Beginner), "a Standard violation is hidden at Beginner");
            t.IsTrue(ViolationPools.IsActive(DifficultyLevel.Standard, false, DifficultyLevel.Standard), "and shows at Standard");
            t.IsTrue(!ViolationPools.IsActive(DifficultyLevel.Beginner, true, DifficultyLevel.Standard), "a subtle violation is hidden below Expert, whatever its minimum");
            t.IsTrue(ViolationPools.IsActive(DifficultyLevel.Beginner, true, DifficultyLevel.Expert), "and shows at Expert");
            t.IsTrue(!ViolationPools.IsActive(DifficultyLevel.Expert, true, DifficultyLevel.Standard), "an Expert subtle violation is hidden at Standard");
        }

        private static void DrawBasics(TestContext t)
        {
            t.Begin("a draw from a pool");

            var pool = Pool("a", "a", "a", "b", "b", "c", "c", "c", "d", "d");

            t.Equal(10, Draw(pool, 0, 1).Count, "a size of 0 means all violations");
            t.Equal(10, Draw(pool, 10, 1).Count, "a size equal to the pool gives all of it");
            t.Equal(10, Draw(pool, 25, 1).Count, "a size above the pool gives all of it, not an error");
            t.Equal(0, Draw(new List<V>(), 5, 1).Count, "an empty pool gives nothing");
            t.Equal(0, ViolationDraw.Choose<V>(null, 5, 1, v => v.skill, v => v.id).Count, "no pool gives nothing");

            var drawn = Draw(pool, 5, 7);
            t.Equal(5, drawn.Count, "the draw has the size asked for");
            t.Equal(5, drawn.Select(v => v.id).Distinct().Count(), "with no violation twice");
            t.IsTrue(drawn.All(v => pool.Contains(v)), "and only violations from the pool");
            t.IsTrue(drawn.Select(v => pool.IndexOf(v)).SequenceEqual(drawn.Select(v => pool.IndexOf(v)).OrderBy(i => i)), "kept in the pool's order");

            t.IsTrue(Draw(pool, 5, 42).Select(v => v.id).SequenceEqual(Draw(pool, 5, 42).Select(v => v.id)), "the same seed gives the same draw");

            var sets = Enumerable.Range(1, 30).Select(seed => string.Join(",", Draw(pool, 5, seed).Select(v => v.id))).Distinct().Count();
            t.IsTrue(sets >= 8, $"different seeds give different draws ({sets} different sets in 30 seeds)");

            // Every violation can turn up
            var seen = new HashSet<string>();
            for (int seed = 1; seed <= 60; seed++) foreach (var v in Draw(pool, 5, seed)) seen.Add(v.id);
            t.Equal(10, seen.Count, "over many sessions every violation in the pool is shown");
        }

        private static void DrawSpreadsSkills(TestContext t)
        {
            t.Begin("a draw spreads across skills");

            var pool = Pool("a", "a", "a", "a", "a", "a", "b", "c", "d", "e");   // one skill dominates the pool

            for (int seed = 1; seed <= 20; seed++)
            {
                var skills = Draw(pool, 5, seed).Select(v => v.skill).Distinct().Count();
                t.IsTrue(skills == 5, $"seed {seed}: five violations cover all five skills (covered {skills})");
            }

            var four = Draw(pool, 7, 3);
            t.Equal(5, four.Select(v => v.skill).Distinct().Count(), "a bigger draw still covers every skill");
            t.Equal(3, four.Count(v => v.skill == "a"), "and fills the rest from the biggest skill");

            var one = Pool("a", "a", "a", "a");
            t.Equal(2, Draw(one, 2, 1).Count, "a pool with one skill still draws");
        }

        private static void DrawPrefersUnseen(TestContext t)
        {
            t.Begin("a draw prefers violations the student has not seen");

            var pool = Pool("a", "a", "a", "a", "b", "b", "b", "b");
            var recent = new HashSet<string> { "V01", "V02", "V05", "V06" };

            for (int seed = 1; seed <= 20; seed++)
            {
                var drawn = Draw(pool, 4, seed, recent);
                t.IsTrue(drawn.All(v => !recent.Contains(v.id)), $"seed {seed}: four fresh violations are available, so none of the seen ones are drawn");
            }

            var all = new HashSet<string>(pool.Select(v => v.id));
            t.Equal(4, Draw(pool, 4, 1, all).Count, "if every violation was seen recently the draw still fills");

            var most = new HashSet<string>(pool.Select(v => v.id).Where(id => id != "V03"));
            t.IsTrue(Draw(pool, 2, 5, most).Any(v => v.id == "V03"), "the one unseen violation is always included");
        }

        private static void SessionSizeField(TestContext t)
        {
            t.Begin("session size");

            var size = new SessionSize { beginner = 4, standard = 6, expert = 8 };
            t.Equal(4, size.For(DifficultyLevel.Beginner), "Beginner size");
            t.Equal(6, size.For(DifficultyLevel.Standard), "Standard size");
            t.Equal(8, size.For(DifficultyLevel.Expert), "Expert size");
            t.IsTrue(size.IsSet && !new SessionSize().IsSet, "an empty size is not set (all violations)");
            t.Equal(0, new SessionSize().For(DifficultyLevel.Standard), "an empty size draws everything");

            var scenario = new ScenarioFileData
            {
                id = "x", sceneName = "x", displayName = "x", assetPrefix = "X", violationFolder = "X", scenarioAssetName = "X",
                availableDifficulties = new[] { "Beginner" },
                violations = new[] { new ViolationFileData
                {
                    violationId = "X-1", conceptId = ConceptIds.ShockProtection, description = "d", severity = "Major", minimumDifficulty = "Beginner",
                    componentObjectName = "c", citations = new[] { new NECInspector.Data.ViolationCitation { profileId = "nec", reference = "1.1" } }
                } }
            };
            t.Equal(0, ScenarioFileValidator.Validate(scenario).Count, "a scenario with no session size is valid");
            scenario.sessionSize = new SessionSize { standard = 3 };
            t.Equal(0, ScenarioFileValidator.Validate(scenario).Count, "a scenario with a session size is valid");
            scenario.sessionSize = new SessionSize { expert = -1 };
            t.IsTrue(ScenarioFileValidator.Validate(scenario).Any(e => e.Contains("sessionSize")), "a negative session size is rejected");
        }
    }
}
