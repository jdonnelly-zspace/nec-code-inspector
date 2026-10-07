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
        public string text;         // code wording or paraphrase shown in review
    }

    public static class ViolationCitations
    {
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
