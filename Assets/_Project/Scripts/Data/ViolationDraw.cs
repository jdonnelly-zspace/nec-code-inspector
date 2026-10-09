using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Data
{
    /// <summary>
    /// Draws the violations for one inspection session from a pool, so replays differ and a student cannot
    /// memorise a scenario. Plain logic with no engine types; the draw is the same for a given seed.
    ///
    /// Rules: never more than <c>count</c>; skills are spread as evenly as the pool allows (one from each skill in
    /// turn); within a skill, violations the student has not seen recently come first; the result keeps the pool's
    /// original order, so the scene reads the same way each time.
    /// </summary>
    public static class ViolationDraw
    {
        public static List<T> Choose<T>(IReadOnlyList<T> pool, int count, int seed,
            Func<T, string> skillOf, Func<T, string> idOf, ISet<string> recentIds = null)
        {
            var all = pool == null ? new List<T>() : pool.ToList();
            if (count <= 0 || count >= all.Count) return all;   // 0 means "all"

            // Deterministic shuffle (Fisher-Yates), then recently seen violations last
            var rng = new Random(seed);
            var order = Enumerable.Range(0, all.Count).ToList();
            for (int i = order.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                int swap = order[i]; order[i] = order[j]; order[j] = swap;
            }

            var ranked = order.Where(i => !IsRecent(all[i], idOf, recentIds))
                .Concat(order.Where(i => IsRecent(all[i], idOf, recentIds)))
                .ToList();

            // One queue per skill, in the order each skill first appears in the ranking
            var skills = new List<string>();
            var queues = new Dictionary<string, Queue<int>>();
            foreach (int i in ranked)
            {
                string skill = skillOf(all[i]) ?? "";
                if (!queues.TryGetValue(skill, out var q))
                {
                    queues[skill] = q = new Queue<int>();
                    skills.Add(skill);
                }
                q.Enqueue(i);
            }

            var picked = new HashSet<int>();
            while (picked.Count < count)
            {
                bool tookAny = false;
                foreach (string skill in skills)
                {
                    if (queues[skill].Count == 0) continue;
                    picked.Add(queues[skill].Dequeue());
                    tookAny = true;
                    if (picked.Count == count) break;
                }
                if (!tookAny) break;
            }

            return all.Where((x, i) => picked.Contains(i)).ToList();
        }

        private static bool IsRecent<T>(T item, Func<T, string> idOf, ISet<string> recentIds)
        {
            return recentIds != null && recentIds.Contains(idOf(item));
        }
    }
}
