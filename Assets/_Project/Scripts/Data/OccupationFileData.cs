using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NECInspector.Data
{
    /// <summary>
    /// Shape of StreamingAssets/Occupations/occupations.json: the occupations a student can follow, each with its procedures
    /// (the standard way the work is done and made safe) and the skills each procedure uses. Generated from the research data by
    /// tools/occupations/build_app_data.py. Plain data with no engine dependency, so the same types serve the loader and the tests.
    /// An occupation is a pathway, not a credential, and student-facing text never names a code, a regulator or a standard.
    /// </summary>
    [Serializable]
    public class OccupationFileData
    {
        public OccupationData[] occupations;
    }

    [Serializable]
    public class OccupationData
    {
        public string id;
        public string socCode;          // classification code, for traceability only; not shown to students
        public string title;
        public string summary;
        public int phase;               // release phase 1 to 3 (docs/product/DECISIONS.md, D-13)
        public bool available;          // false: shown as coming soon
        public string reviewStatus;     // draft | reviewed (by an occupational expert)
        public ProcedureData[] procedures;
    }

    [Serializable]
    public class ProcedureData
    {
        public string id;
        public string name;
        public string purpose;
        public string whenUsed;
        public string role;
        public string[] steps;          // in order
        public string[] hazards;
        public string[] controls;
        public string[] skills;         // ConceptIds this procedure uses
        public string coverage;         // none | partial | good: how well the app trains it today
        public bool practice;           // true: the ordering practice is offered
        public string reviewStatus;
    }

    public static class OccupationValidator
    {
        public const string StatusDraft = "draft";
        public const string StatusReviewed = "reviewed";

        // Names a student must never see (docs/CONTENT_POLICY.md)
        private static readonly Regex Banned = new Regex(@"\b(NEC|CEC|NFPA|CSA|IET|BSI|BS ?7671|C22\.1|NESC|IEEE|OSHA|UL \d+|ICC|IAEI|NCCER|IBEW|NECA)\b");

        public static IEnumerable<string> AllText(OccupationData o)
        {
            if (o == null) yield break;
            if (o.title != null) yield return o.title;
            if (o.summary != null) yield return o.summary;
            if (o.procedures == null) yield break;
            foreach (var p in o.procedures)
                foreach (string s in AllText(p)) yield return s;
        }

        public static IEnumerable<string> AllText(ProcedureData p)
        {
            if (p == null) yield break;
            foreach (string s in new[] { p.name, p.purpose, p.whenUsed, p.role }) if (s != null) yield return s;
            foreach (var list in new[] { p.steps, p.hazards, p.controls })
                if (list != null) foreach (string s in list) if (s != null) yield return s;
        }

        /// <summary>One message per problem; empty if the file is sound.</summary>
        public static List<string> Validate(OccupationFileData file)
        {
            var errors = new List<string>();
            if (file?.occupations == null || file.occupations.Length == 0)
            {
                errors.Add("occupations is empty");
                return errors;
            }

            var ids = new HashSet<string>();
            var procedureIds = new HashSet<string>();
            foreach (var o in file.occupations)
            {
                if (o == null) { errors.Add("occupations contains an empty entry"); continue; }
                string label = string.IsNullOrEmpty(o.id) ? "(occupation without an id)" : o.id;
                if (string.IsNullOrWhiteSpace(o.id)) errors.Add($"{label}: id is missing");
                else if (!ids.Add(o.id)) errors.Add($"{label}: duplicate id");
                if (string.IsNullOrWhiteSpace(o.title)) errors.Add($"{label}: title is missing");
                if (string.IsNullOrWhiteSpace(o.summary)) errors.Add($"{label}: summary is missing");
                if (o.phase < 1 || o.phase > 3) errors.Add($"{label}: phase must be 1, 2 or 3");
                if (o.reviewStatus != StatusDraft && o.reviewStatus != StatusReviewed) errors.Add($"{label}: reviewStatus must be draft or reviewed");
                if (o.procedures == null || o.procedures.Length == 0) { errors.Add($"{label}: no procedures"); continue; }

                foreach (var p in o.procedures)
                {
                    if (p == null) { errors.Add($"{label}: empty procedure"); continue; }
                    string pl = $"{label}/{p.id}";
                    if (string.IsNullOrWhiteSpace(p.id)) errors.Add($"{label}: procedure without an id");
                    else if (!procedureIds.Add(p.id)) errors.Add($"{pl}: duplicate procedure id");
                    if (string.IsNullOrWhiteSpace(p.name)) errors.Add($"{pl}: name is missing");
                    if (string.IsNullOrWhiteSpace(p.purpose)) errors.Add($"{pl}: purpose is missing");
                    if (p.steps == null || p.steps.Length < 3) errors.Add($"{pl}: needs at least 3 steps");
                    else if (p.steps.Any(string.IsNullOrWhiteSpace)) errors.Add($"{pl}: has an empty step");
                    if (p.hazards == null || p.hazards.Length == 0) errors.Add($"{pl}: no hazards");
                    if (p.controls == null || p.controls.Length == 0) errors.Add($"{pl}: no controls");
                    if (p.coverage != "none" && p.coverage != "partial" && p.coverage != "good") errors.Add($"{pl}: coverage must be none, partial or good");
                    if (p.skills != null)
                        foreach (string s in p.skills) if (!ConceptIds.IsKnown(s)) errors.Add($"{pl}: unknown skill '{s}'");
                    if (p.practice && (p.skills == null || p.skills.Length == 0)) errors.Add($"{pl}: practice is on but the procedure uses no skill");
                    if (p.reviewStatus != StatusDraft && p.reviewStatus != StatusReviewed) errors.Add($"{pl}: reviewStatus must be draft or reviewed");
                }

                foreach (string text in AllText(o))
                    if (Banned.IsMatch(text)) errors.Add($"{label}: student-facing text names a code, regulator or standard: \"{Shorten(text)}\"");
            }

            return errors;
        }

        private static string Shorten(string s) => s.Length <= 70 ? s : s.Substring(0, 70) + "...";
    }
}
