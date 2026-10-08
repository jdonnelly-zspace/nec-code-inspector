using System;

namespace NECInspector.Skills
{
    /// <summary>
    /// One observation of a learner using a skill. Produced by inspection scenarios and the
    /// panel sandbox; recorded into SkillProgress. The skill ID is a concept ID (see ConceptIds).
    /// </summary>
    [Serializable]
    public class SkillEvidence
    {
        public string skillId;
        public SkillTier tier;
        public float outcome;       // 0 = failed, 1 = fully correct; partial credit in between
        public string source;       // see SkillEvidenceSources

        public SkillEvidence()
        {
        }

        public SkillEvidence(string skillId, SkillTier tier, float outcome, string source)
        {
            this.skillId = skillId;
            this.tier = tier;
            this.outcome = outcome;
            this.source = source;
        }
    }

    public static class SkillEvidenceSources
    {
        public const string Inspection = "inspection";
        public const string Sandbox = "sandbox";
    }

    /// <summary>
    /// How evidence from an inspection violation turns into an outcome.
    /// </summary>
    public static class SkillOutcomes
    {
        public const float Missed = 0f;
        public const float FoundWrongCitation = 0.5f;
        public const float FoundSameSkillCitation = 0.75f;   // cited a different entry of the same skill (see CitationCredit)
        public const float FoundCorrectCitation = 1f;

        public static float ForInspection(bool found, bool citationCorrect)
        {
            if (!found) return Missed;
            return citationCorrect ? FoundCorrectCitation : FoundWrongCitation;
        }
    }
}
