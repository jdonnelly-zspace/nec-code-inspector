namespace NECInspector.Codes
{
    /// <summary>
    /// IDs of the installation code profiles. Content files refer to profiles by these IDs
    /// (for example a violation's citation for the "nec" profile).
    /// </summary>
    public static class CodeProfileIds
    {
        public const string Nec2023 = "nec-2023";
        public const string Nec2026 = "nec-2026";

        /// <summary>The default NEC profile: the edition the verified content follows.</summary>
        public const string Nec = Nec2023;
    }
}
