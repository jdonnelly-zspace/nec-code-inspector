using System;

namespace NECInspector.Codes
{
    /// <summary>
    /// Code-neutral reference entry (an NEC article, a BS 7671 regulation, a CEC rule, ...).
    /// Profiles convert their own data format into this type so UI and scoring code never
    /// depend on a specific code's numbering or fields.
    /// </summary>
    [Serializable]
    public class CodeArticle
    {
        public string reference;            // e.g., "250.24(A)(1)"
        public string referenceLabel;       // e.g., "Art. 250.24(A)(1)" (profile-specific prefix)
        public string title;
        public string text;
        public int chapter;                 // section, chapter or part number in the source code
        public string[] keywords;
        public string[] relatedReferences;
        public bool isNewInEdition;         // new or changed in the profile's current edition

        /// <summary>
        /// Display string for UI (e.g., "Art. 250.24(A)(1) - Grounding Electrode Conductor Connection")
        /// </summary>
        public string DisplayString => $"{referenceLabel} - {title}";
    }
}
