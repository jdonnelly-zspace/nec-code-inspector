namespace NECInspector.Skills
{
    public static class SkillNames
    {
        /// <summary>Readable name for a skill ID, e.g. "arc-fault-protection" becomes "Arc fault protection".</summary>
        public static string Display(string skillId)
        {
            if (string.IsNullOrEmpty(skillId)) return "";

            string text = skillId.Replace('-', ' ');
            return char.ToUpperInvariant(text[0]) + text.Substring(1);
        }
    }
}
