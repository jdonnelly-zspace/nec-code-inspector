using System;
using System.Text.RegularExpressions;

namespace NECInspector.Codes
{
    /// <summary>
    /// Default citation matching for codes that use hierarchical references such as
    /// "250.24(A)(1)" or "26-712(d)(iii)": the student cites the exact reference, or a parent of it
    /// ("250.24" for "250.24(A)(1)"). A parent must end where a new level starts, so "250.2" is not
    /// a parent of "250.24". Labels ("Art.", "Rule"), spaces and letter case are ignored.
    /// </summary>
    public static class CitationMatcher
    {
        public static bool Default(string cited, string expected)
        {
            if (string.IsNullOrEmpty(cited) || string.IsNullOrEmpty(expected))
                return false;

            string c = Normalize(cited);
            string e = Normalize(expected);
            if (c.Length == 0 || e.Length == 0)
                return false;

            return c == e || IsParentOfNormalized(c, e);
        }

        /// <summary>True if the first reference is a parent level of the second, at a level boundary.</summary>
        public static bool IsParentOf(string parent, string child)
        {
            if (string.IsNullOrEmpty(parent) || string.IsNullOrEmpty(child))
                return false;

            string p = Normalize(parent);
            string c = Normalize(child);
            return p.Length > 0 && IsParentOfNormalized(p, c);
        }

        private static bool IsParentOfNormalized(string parent, string child)
        {
            if (child.Length <= parent.Length || !child.StartsWith(parent, StringComparison.Ordinal))
                return false;

            // The next character must open a new level: "(" subsection, "." or "-" numbering
            char next = child[parent.Length];
            return next == '(' || next == '.' || next == '-';
        }

        private static string Normalize(string reference)
        {
            string text = reference.Replace(" ", "").Trim().ToLowerInvariant();
            // Drop a leading label such as "art." or "rule"
            return Regex.Replace(text, @"^[a-z.]+(?=\d)", "");
        }
    }
}
