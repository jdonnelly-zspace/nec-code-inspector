using System;
using System.Collections.Generic;

namespace NECInspector.Skills
{
    /// <summary>
    /// Thresholds for deciding that a skill is attained. Change them here, not in callers.
    /// </summary>
    public static class SkillPolicy
    {
        public const int MinAttempts = 3;
        public const float MasteryThreshold = 0.8f;

        /// <summary>Weight of the newest observation in the running mastery average.</summary>
        public const float NewEvidenceWeight = 0.3f;
    }

    /// <summary>
    /// Mastery of one skill at one tier. Mastery is a running average of outcomes, so recent
    /// performance counts more than old attempts.
    /// </summary>
    [Serializable]
    public class SkillStat
    {
        public string skillId;
        public int tier;            // SkillTier as an int (JsonUtility stores enums as numbers)
        public int attempts;
        public float mastery;
        public string updatedAt;

        public SkillTier Tier => (SkillTier)tier;

        public bool IsAttained =>
            attempts >= SkillPolicy.MinAttempts && mastery >= SkillPolicy.MasteryThreshold;
    }

    /// <summary>
    /// What the learner can do, independent of any credential. Credentials are views over
    /// this data (see CredentialReadiness); progress is never stored per credential.
    /// </summary>
    [Serializable]
    public class SkillProgress
    {
        public List<SkillStat> stats = new List<SkillStat>();

        public void Record(SkillEvidence evidence, string timestamp = null)
        {
            if (evidence == null || string.IsNullOrEmpty(evidence.skillId))
                return;

            float outcome = Math.Max(0f, Math.Min(1f, evidence.outcome));
            var stat = Get(evidence.skillId, evidence.tier);

            if (stat == null)
            {
                stat = new SkillStat { skillId = evidence.skillId, tier = (int)evidence.tier, attempts = 0, mastery = outcome };
                stats.Add(stat);
            }
            else
            {
                stat.mastery += SkillPolicy.NewEvidenceWeight * (outcome - stat.mastery);
            }

            stat.attempts++;
            stat.updatedAt = timestamp;
        }

        public void Record(IEnumerable<SkillEvidence> evidence, string timestamp = null)
        {
            if (evidence == null) return;
            foreach (var item in evidence)
                Record(item, timestamp);
        }

        public SkillStat Get(string skillId, SkillTier tier)
        {
            foreach (var stat in stats)
            {
                if (stat.skillId == skillId && stat.tier == (int)tier)
                    return stat;
            }

            return null;
        }

        /// <summary>
        /// True if the skill is attained at the tier or any higher tier
        /// (attaining Authority-level work also covers Practitioner-level requirements).
        /// </summary>
        public bool HasAttained(string skillId, SkillTier tier)
        {
            foreach (var stat in stats)
            {
                if (stat.skillId == skillId && stat.tier >= (int)tier && stat.IsAttained)
                    return true;
            }

            return false;
        }

        /// <summary>Highest tier at which the skill is attained, or null if none.</summary>
        public SkillTier? HighestAttainedTier(string skillId)
        {
            SkillTier? best = null;
            foreach (var stat in stats)
            {
                if (stat.skillId != skillId || !stat.IsAttained) continue;
                if (best == null || stat.tier > (int)best.Value)
                    best = stat.Tier;
            }

            return best;
        }

        /// <summary>Number of distinct skills attained at any tier.</summary>
        public int AttainedSkillCount()
        {
            var attained = new HashSet<string>();
            foreach (var stat in stats)
            {
                if (stat.IsAttained)
                    attained.Add(stat.skillId);
            }

            return attained.Count;
        }
    }
}
