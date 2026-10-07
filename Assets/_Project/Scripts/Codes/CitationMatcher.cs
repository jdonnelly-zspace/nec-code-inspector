namespace NECInspector.Codes
{
    /// <summary>
    /// Default citation matching for codes that use hierarchical numeric references
    /// such as "250.24(A)(1)": exact match, or the student cites a parent of the expected rule.
    /// Profiles with different numbering can implement their own matching.
    /// </summary>
    public static class CitationMatcher
    {
        public static bool Default(string cited, string expected)
        {
            if (string.IsNullOrEmpty(cited) || string.IsNullOrEmpty(expected))
                return false;

            string normalizedCited = Normalize(cited);
            string normalizedExpected = Normalize(expected);

            // Exact match
            if (normalizedCited == normalizedExpected) return true;

            // Partial match (student cites parent article, expected is subsection)
            if (normalizedExpected.StartsWith(normalizedCited)) return true;

            return false;
        }

        private static string Normalize(string reference)
        {
            return reference.Replace(" ", "").Replace("Art.", "").Replace("art.", "").Trim();
        }
    }
}
