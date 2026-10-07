using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.Credentials
{
    /// <summary>
    /// A credential seen as a view over skills: which skills, at what tier, count toward it,
    /// which installation code it is based on, and what it calls the tiers.
    /// Credentials never hold learner progress; readiness is computed from SkillProgress.
    /// </summary>
    [Serializable]
    public class CredentialProfile
    {
        public string id;                   // e.g., "core-skills", "red-seal-309a"
        public string displayName;
        public string issuingBody;
        public string region;               // e.g., "US", "CA", "UK"
        public string codeProfileId;        // installation code the credential is based on (see CodeProfileIds)
        public string reviewStatus;         // "app-defined", "draft" or "reviewed" (by a credential expert)
        public string[] tierLabels;         // optional names for Foundation, Practitioner, Authority
        public CredentialRequirement[] requirements;

        public string GetTierLabel(SkillTier tier)
        {
            int index = (int)tier;
            if (tierLabels != null && index < tierLabels.Length && !string.IsNullOrEmpty(tierLabels[index]))
                return tierLabels[index];

            return SkillTiers.Names[index];
        }
    }

    [Serializable]
    public class CredentialRequirement
    {
        public string skillId;      // a concept ID (see ConceptIds)
        public string tier;         // Foundation | Practitioner | Authority
        public float weight = 1f;
    }

    public static class CredentialValidator
    {
        /// <summary>One message per problem; empty if the credential file is usable.</summary>
        public static List<string> Validate(CredentialProfile profile)
        {
            var errors = new List<string>();

            if (profile == null)
            {
                errors.Add("file is empty or not a credential");
                return errors;
            }

            RequireName(errors, profile.id, "id");
            RequireName(errors, profile.codeProfileId, "codeProfileId");
            if (string.IsNullOrWhiteSpace(profile.displayName))
                errors.Add("displayName is missing");

            if (profile.tierLabels != null && profile.tierLabels.Length != 0 && profile.tierLabels.Length != SkillTiers.Names.Length)
                errors.Add($"tierLabels must be empty or have {SkillTiers.Names.Length} entries (Foundation, Practitioner, Authority)");

            if (profile.requirements == null || profile.requirements.Length == 0)
            {
                errors.Add("requirements is empty");
                return errors;
            }

            var seen = new HashSet<string>();
            foreach (var r in profile.requirements)
            {
                if (r == null)
                {
                    errors.Add("requirements contains an empty entry");
                    continue;
                }

                string label = string.IsNullOrEmpty(r.skillId) ? "(requirement without skill)" : r.skillId;

                if (!ConceptIds.IsKnown(r.skillId))
                    errors.Add($"{label}: unknown skillId '{r.skillId}'");
                else if (!seen.Add(r.skillId))
                    errors.Add($"{label}: skill listed more than once");

                if (!SkillTiers.TryParse(r.tier, out _))
                    errors.Add($"{label}: unknown tier '{r.tier}' (use Foundation, Practitioner or Authority)");

                if (!(r.weight > 0f))
                    errors.Add($"{label}: weight must be greater than 0");
            }

            return errors;
        }

        private static void RequireName(List<string> errors, string value, string field)
        {
            if (string.IsNullOrEmpty(value) || !Regex.IsMatch(value, "^[A-Za-z0-9_-]+$"))
                errors.Add($"{field} must contain only letters, digits, '_' or '-' (got '{value}')");
        }
    }

    public class RequirementStatus
    {
        public CredentialRequirement requirement;
        public SkillTier tier;
        public bool attained;
        public float mastery;       // best mastery at the required tier or higher
        public int attempts;        // attempts at the required tier or higher
        public float fraction;      // 0..1 progress toward attaining this requirement
    }

    public class ReadinessReport
    {
        public CredentialProfile credential;
        public List<RequirementStatus> requirements = new List<RequirementStatus>();
        public float percent;       // weighted progress, 0..1
        public int attainedCount;

        public int TotalCount => requirements.Count;
        public bool IsComplete => TotalCount > 0 && attainedCount == TotalCount;
    }

    public static class CredentialReadiness
    {
        /// <summary>
        /// How close the learner's skills are to the credential's requirements.
        /// Partial progress counts: mastery toward the threshold, scaled by attempts toward the minimum.
        /// </summary>
        public static ReadinessReport Evaluate(CredentialProfile credential, SkillProgress progress)
        {
            var report = new ReadinessReport { credential = credential };
            if (credential == null || credential.requirements == null)
                return report;

            float totalWeight = 0f;
            float earnedWeight = 0f;

            foreach (var requirement in credential.requirements)
            {
                if (requirement == null || !SkillTiers.TryParse(requirement.tier, out var tier))
                    continue;

                var status = new RequirementStatus { requirement = requirement, tier = tier };

                if (progress != null)
                {
                    foreach (var stat in progress.stats)
                    {
                        if (stat.skillId != requirement.skillId || stat.tier < (int)tier) continue;

                        status.attempts += stat.attempts;
                        if (stat.attempts > 0 && stat.mastery > status.mastery)
                            status.mastery = stat.mastery;
                    }

                    status.attained = progress.HasAttained(requirement.skillId, tier);
                }

                if (status.attained)
                {
                    status.fraction = 1f;
                    report.attainedCount++;
                }
                else if (status.attempts > 0)
                {
                    float masteryPart = Math.Min(1f, status.mastery / SkillPolicy.MasteryThreshold);
                    float attemptsPart = Math.Min(1f, (float)status.attempts / SkillPolicy.MinAttempts);
                    status.fraction = masteryPart * attemptsPart;
                }

                totalWeight += requirement.weight;
                earnedWeight += requirement.weight * status.fraction;
                report.requirements.Add(status);
            }

            report.percent = totalWeight > 0f ? earnedWeight / totalWeight : 0f;
            return report;
        }
    }
}
