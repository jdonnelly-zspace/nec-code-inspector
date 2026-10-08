using System;
using System.Collections.Generic;
using System.Globalization;

namespace NECInspector.Codes
{
    /// <summary>
    /// The licensing status of an installation code's material in the app (profile.json, <c>license</c>).
    /// Installation codes (NEC, CEC, BS 7671, ...) are copyrighted. The app stores reference numbers and its own
    /// paraphrases (docs/CONTENT_POLICY.md), but whether that is enough to ship is a decision for the owner and
    /// counsel, not for the code. This record says what has been decided, by whom to ask, and where the evidence is.
    /// </summary>
    [Serializable]
    public class CodeLicense
    {
        public const string Unreviewed = "unreviewed";      // nobody has checked yet; the default for any new code
        public const string OwnWords = "own-words";         // checked and decided: own-words content and reference numbers need no licence
        public const string Licensed = "licensed";          // a licence or written permission is held

        public static readonly string[] Statuses = { Unreviewed, OwnWords, Licensed };

        public string status;       // unreviewed | own-words | licensed
        public string holder;       // who holds the copyright, e.g. "National Fire Protection Association (NFPA)"
        public string note;         // what is used and what has to be decided
        public string evidence;     // for licensed: where the licence or permission is kept (agreement id, document name)
        public string checkedOn;    // yyyy-MM-dd of the decision; empty while unreviewed

        /// <summary>True once a decision to ship has been recorded (own-words or licensed).</summary>
        public bool IsCleared => status == OwnWords || status == Licensed;
    }

    public static class CodeLicenseValidator
    {
        /// <summary>One message per problem; empty if the licence record is complete for its status.</summary>
        public static List<string> Validate(CodeLicense license)
        {
            var errors = new List<string>();
            if (license == null || string.IsNullOrEmpty(license.status))
            {
                errors.Add($"license.status is missing (use one of {string.Join(", ", CodeLicense.Statuses)}; new codes start as '{CodeLicense.Unreviewed}')");
                return errors;
            }

            if (Array.IndexOf(CodeLicense.Statuses, license.status) < 0)
            {
                errors.Add($"license.status must be one of {string.Join(", ", CodeLicense.Statuses)} (got '{license.status}')");
                return errors;
            }

            if (string.IsNullOrWhiteSpace(license.holder)) errors.Add("license.holder is missing (who holds the copyright?)");
            if (string.IsNullOrWhiteSpace(license.note)) errors.Add("license.note is missing (what is used, and what is left to decide?)");

            if (license.IsCleared)
            {
                if (string.IsNullOrWhiteSpace(license.checkedOn)) errors.Add($"license.checkedOn is missing (a '{license.status}' status needs the date of the decision)");
                if (license.status == CodeLicense.Licensed && string.IsNullOrWhiteSpace(license.evidence))
                    errors.Add("license.evidence is missing (a 'licensed' status needs the agreement or permission it rests on)");
            }

            if (!string.IsNullOrEmpty(license.checkedOn)
                && !DateTime.TryParseExact(license.checkedOn, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
                errors.Add($"license.checkedOn must be a date like 2026-10-08 (got '{license.checkedOn}')");

            return errors;
        }
    }
}
