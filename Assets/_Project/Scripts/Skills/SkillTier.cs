using NECInspector.Core;

namespace NECInspector.Skills
{
    /// <summary>
    /// Depth of a skill, independent of any credential. Credentials name and weight these
    /// tiers in their own way; progress is recorded against the tier of the evidence.
    /// </summary>
    public enum SkillTier
    {
        Foundation = 0,     // recognize hazards, apply simple rules with a reference open
        Practitioner = 1,   // select, size and install to code; find violations unaided
        Authority = 2       // design, verify, inspect and certify; special cases and code changes
    }

    public static class SkillTiers
    {
        public static readonly string[] Names = { "Foundation", "Practitioner", "Authority" };

        /// <summary>The tier of evidence produced at a difficulty setting or by content of that difficulty.</summary>
        public static SkillTier FromDifficulty(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Beginner: return SkillTier.Foundation;
                case DifficultyLevel.Expert: return SkillTier.Authority;
                default: return SkillTier.Practitioner;
            }
        }

        /// <summary>Parses an exact tier name (as used in credential files); numbers are not accepted.</summary>
        public static bool TryParse(string name, out SkillTier tier)
        {
            int index = System.Array.IndexOf(Names, name);
            tier = index >= 0 ? (SkillTier)index : SkillTier.Foundation;
            return index >= 0;
        }
    }
}
