using System;
using System.Text.RegularExpressions;

namespace NECInspector.Codes
{
    [Serializable]
    public class TermEntry
    {
        public string key;      // neutral key, e.g. "grounding"
        public string text;     // what the active code calls it, e.g. "earthing"
    }

    /// <summary>
    /// Names and labels that differ between installation codes: what the code is called, how a
    /// reference is written ("Art. 250.24", "Reg. 411.3"), what its top-level divisions are called
    /// (chapter, part, section), and everyday vocabulary (grounding or earthing). UI text asks the
    /// active profile instead of hard-coding NEC wording.
    ///
    /// Content rule: entries are generic labels and vocabulary written for this app. Do not put
    /// wording copied from a code book here (see docs/CONTENT_POLICY.md).
    /// </summary>
    [Serializable]
    public class CodeTerminology
    {
        /// <summary>Reference text used for rules that come from general practice, not a code rule.</summary>
        public const string GeneralPractice = "General Practice";

        public string codeName = "NEC";                     // short name used in headings
        public string referencePrefix = "Art. ";            // goes before a reference
        public string referenceNoun = "Article";            // what one numbered reference is called
        public string sectionLabel = "Chapter";             // what the top-level divisions are called
        public string generalPracticeLabel = "General practice";
        public TermEntry[] terms;

        /// <summary>A reference as shown to students, e.g. "Art. 250.24(A)(1)".</summary>
        public string ReferenceLabel(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference)) return "";
            if (string.Equals(reference, GeneralPractice, StringComparison.OrdinalIgnoreCase))
                return generalPracticeLabel;

            return referencePrefix + reference;
        }

        public string CitationsHeading => $"{codeName} Citations";
        public string SectionsHeading => $"{codeName} {sectionLabel}s";
        public string SelectReferencePrompt => $"Select {codeName} {referenceNoun}...";
        public string SectionName(int section) => $"{sectionLabel} {section}";

        /// <summary>The active code's word for a neutral term key, or the fallback (the key if none) when it has none.</summary>
        public string Term(string key, string fallback = null)
        {
            if (terms != null)
            {
                foreach (var entry in terms)
                {
                    if (entry != null && entry.key == key && !string.IsNullOrEmpty(entry.text))
                        return entry.text;
                }
            }

            return fallback ?? key;
        }

        /// <summary>
        /// Replaces {code} with the code name, {term:key} with that code's word for the term, and
        /// {Term:key} with the same word starting with a capital letter (for the start of a name or sentence).
        /// </summary>
        public string Format(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;

            string result = text.Replace("{code}", codeName);
            return Regex.Replace(result, @"\{(term|Term):([A-Za-z0-9-]+)\}", match =>
            {
                string word = Term(match.Groups[2].Value);
                return match.Groups[1].Value == "Term" && word.Length > 0
                    ? char.ToUpperInvariant(word[0]) + word.Substring(1)
                    : word;
            });
        }

        /// <summary>
        /// NEC labels. Used when the profile has no terminology file.
        /// Keep in sync with StreamingAssets/Codes/nec/terminology.json.
        /// </summary>
        public static CodeTerminology CreateNecDefaults()
        {
            return new CodeTerminology
            {
                terms = new[]
                {
                    Entry("grounding", "grounding"),
                    Entry("ground-rod", "ground rod"),
                    Entry("receptacle", "receptacle"),
                    Entry("breaker", "breaker"),
                    Entry("panel", "panel"),
                    Entry("shock-protection-device", "GFCI"),
                    Entry("arc-fault-device", "AFCI"),
                    Entry("service-disconnect", "service disconnect")
                }
            };
        }

        private static TermEntry Entry(string key, string text)
        {
            return new TermEntry { key = key, text = text };
        }
    }
}
