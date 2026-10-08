using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NECInspector.Data;

namespace NECInspector.Codes
{
    /// <summary>
    /// profile.json: identifies an installation code (NEC, CEC, BS 7671, ...) and how far its
    /// content can be trusted. One folder per code under StreamingAssets/Codes/.
    /// </summary>
    [Serializable]
    public class CodeProfileManifest
    {
        public const string StatusAppDefined = "app-defined";
        public const string StatusDraft = "draft";
        public const string StatusReviewed = "reviewed";

        public string id;                   // folder name and ID used in citations, e.g. "nec", "cec"
        public string displayName;          // e.g. "CEC (CSA C22.1)"
        public string edition;              // e.g. "2026"
        public string region;               // e.g. "US", "CA"
        public string artSet;               // device and panel art the scenes use, e.g. "north-america" (see ArtSets)
        public string reviewStatus;         // app-defined | draft | reviewed (by a credential expert)
        public string note;                 // what is covered and what is not
        public CodeLicense license;         // who holds the copyright and whether shipping the material has been decided (see CodeLicense)
    }

    /// <summary>One reference entry in articles.json.</summary>
    [Serializable]
    public class CodeArticleData
    {
        public string reference;            // full reference as cited, e.g. "250.24(A)(1)" or "26-712"
        public string title;
        public string text;                 // explanation in our own words (see docs/CONTENT_POLICY.md)
        public int section;                 // chapter, section or part number in the code
        public string[] keywords;
        public string[] related;            // references of related entries
        public bool isNewInEdition;
        public string conceptId;            // the skill (ConceptIds) the entry belongs to; scoring gives partial credit for citing another entry of the same skill
    }

    [Serializable]
    public class CodeArticleFile
    {
        public CodeArticleData[] articles;
    }

    public static class CodeProfileValidator
    {
        private static readonly string[] Statuses =
        {
            CodeProfileManifest.StatusAppDefined, CodeProfileManifest.StatusDraft, CodeProfileManifest.StatusReviewed
        };

        /// <summary>One message per problem; empty if the profile data is usable.</summary>
        public static List<string> Validate(CodeProfileManifest manifest, CodeArticleData[] articles,
            ElectricalTables tables, CodeTerminology terminology)
        {
            var errors = new List<string>();

            if (manifest == null)
            {
                errors.Add("profile.json is missing or empty");
                return errors;
            }

            if (string.IsNullOrEmpty(manifest.id) || !Regex.IsMatch(manifest.id, "^[A-Za-z0-9_-]+$"))
                errors.Add($"id must contain only letters, digits, '_' or '-' (got '{manifest.id}')");
            if (string.IsNullOrWhiteSpace(manifest.displayName)) errors.Add("displayName is missing");
            if (string.IsNullOrWhiteSpace(manifest.edition)) errors.Add("edition is missing");
            if (!ArtSets.IsValidName(manifest.artSet))
                errors.Add($"artSet must be lower-case words joined by '-', like 'north-america' (got '{manifest.artSet}')");
            if (Array.IndexOf(Statuses, manifest.reviewStatus) < 0)
                errors.Add($"reviewStatus must be one of {string.Join(", ", Statuses)} (got '{manifest.reviewStatus}')");
            errors.AddRange(CodeLicenseValidator.Validate(manifest.license));

            if (terminology == null)
                errors.Add("terminology.json is missing or empty");
            else if (string.IsNullOrWhiteSpace(terminology.codeName))
                errors.Add("terminology codeName is missing");

            if (tables != null)
            {
                if (tables.conductorSizes == null || tables.conductorSizes.Length == 0)
                    errors.Add("tables.json has no conductorSizes");
                if (!string.IsNullOrEmpty(tables.defaultConductor) && tables.GetMaxAmps(tables.defaultConductor) <= 0)
                    errors.Add($"tables defaultConductor '{tables.defaultConductor}' is not in conductorSizes");
            }

            if (articles == null || articles.Length == 0)
            {
                errors.Add("articles.json has no articles");
                return errors;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var a in articles)
            {
                if (a == null || string.IsNullOrWhiteSpace(a.reference))
                {
                    errors.Add("an article has no reference");
                    continue;
                }

                if (!seen.Add(a.reference)) errors.Add($"duplicate reference '{a.reference}'");
                if (string.IsNullOrWhiteSpace(a.title)) errors.Add($"{a.reference}: title is missing");
                if (string.IsNullOrWhiteSpace(a.text)) errors.Add($"{a.reference}: text is missing");
                if (a.section <= 0) errors.Add($"{a.reference}: section must be 1 or more");
                if (!string.IsNullOrEmpty(a.conceptId) && !ConceptIds.IsKnown(a.conceptId)) errors.Add($"{a.reference}: unknown conceptId '{a.conceptId}'");
            }

            return errors;
        }
    }
}
