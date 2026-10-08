using System;
using NECInspector.Codes;

namespace NECInspector.Skills
{
    /// <summary>
    /// How much credit a citation earns for a violation the student found. The rule (docs/SKILL_SCORING.md):
    /// the expected citation, or one that matches it, earns full credit; a different entry that belongs to the
    /// same skill as the violation earns three quarters (the student knew where the rule lives, but picked a
    /// neighbouring one); anything else the student cited earns half, as before.
    /// </summary>
    public static class CitationCredit
    {
        public static float Outcome(ICodeProfile profile, string violationConceptId, string cited, string expected)
        {
            bool matches = profile != null
                ? profile.CitationMatches(cited, expected)
                : CitationMatcher.Default(cited, expected);
            if (matches)
                return SkillOutcomes.FoundCorrectCitation;

            if (profile != null && !string.IsNullOrEmpty(violationConceptId)
                && string.Equals(ConceptOf(profile, cited), violationConceptId, StringComparison.Ordinal))
                return SkillOutcomes.FoundSameSkillCitation;

            return SkillOutcomes.FoundWrongCitation;
        }

        /// <summary>
        /// The skill a cited reference belongs to: its own entry, or the closest entry that contains it
        /// (a student may cite a sub-item the data only has at article level). Empty if it cannot be resolved.
        /// </summary>
        public static string ConceptOf(ICodeProfile profile, string cited)
        {
            if (profile == null || string.IsNullOrWhiteSpace(cited)) return "";

            var article = profile.GetArticle(cited);
            if (article == null)
            {
                string best = null;
                foreach (string reference in profile.GetAllReferences())
                    if (CitationMatcher.IsParentOf(reference, cited) && (best == null || reference.Length > best.Length))
                        best = reference;
                if (best != null) article = profile.GetArticle(best);
            }

            return article?.conceptId ?? "";
        }
    }
}
