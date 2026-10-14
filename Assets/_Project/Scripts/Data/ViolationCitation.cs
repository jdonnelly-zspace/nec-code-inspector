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

        // Optional: this code's limit on the quantity the scene measures (see SceneFacts)
        public SceneLimit limit;
    }

    public static class ViolationCitations
    {
        /// <summary>The override if one is given, otherwise the violation's own text.</summary>
        public static string Choose(string overrideText, string fallback)
        {
            return string.IsNullOrEmpty(overrideText) ? fallback : overrideText;
        }

        /// <summary>The first citation for any of the ids, tried in order (a code, then its companions), or null.</summary>
        public static ViolationCitation FindAny(ViolationCitation[] citations, System.Collections.Generic.IReadOnlyList<string> profileIds)
        {
            if (profileIds == null) return null;
            foreach (string id in profileIds)
            {
                var found = Find(citations, id);
                if (found != null) return found;
            }

            return null;
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
