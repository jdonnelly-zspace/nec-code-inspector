using System;

namespace NECInspector.Data
{
    /// <summary>
    /// How one installation code cites a violation. A violation carries one citation per code
    /// profile it applies to; a violation with no citation for a profile does not apply to it.
    /// </summary>
    [Serializable]
    public class ViolationCitation
    {
        public string profileId;    // e.g., "nec" (see CodeProfileIds)
        public string reference;    // e.g., "250.24(A)(1)"
        public string text;         // explanation shown in review, in our own words

        // Optional wording for this code when the shared violation text names code-specific numbers
        // (feet vs metres, a different limit). Empty means use the violation's own text.
        public string description;
        public string hintText;
        public string inspectionNote;
    }

    public static class ViolationCitations
    {
        /// <summary>The override if one is given, otherwise the violation's own text.</summary>
        public static string Choose(string overrideText, string fallback)
        {
            return string.IsNullOrEmpty(overrideText) ? fallback : overrideText;
        }

        /// <summary>The citation for a profile, or null if the violation does not apply to it.</summary>
        public static ViolationCitation Find(ViolationCitation[] citations, string profileId)
        {
            if (citations == null || string.IsNullOrEmpty(profileId))
                return null;

            foreach (var citation in citations)
            {
                if (citation != null && citation.profileId == profileId)
                    return citation;
            }

            return null;
        }
    }
}
