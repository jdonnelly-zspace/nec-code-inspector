using System.Collections.Generic;

namespace NECInspector.Codes
{
    /// <summary>
    /// An installation code the app can teach against (NEC, CEC, BS 7671, ...).
    /// UI and scoring talk to this interface instead of a specific code's database.
    /// </summary>
    public interface ICodeProfile
    {
        string ProfileId { get; }       // e.g., "nec"
        string DisplayName { get; }     // e.g., "NEC (NFPA 70)"
        string Edition { get; }         // e.g., "2026"
        bool IsLoaded { get; }
        int ArticleCount { get; }

        /// <summary>Voltages, ampacities, load-calc constants and compliance rule settings.</summary>
        ElectricalTables Tables { get; }

        /// <summary>
        /// False if the profile has no tables of its own (Tables then holds NEC values), so panel sandbox
        /// results are not valid evidence for this code.
        /// </summary>
        bool HasOwnTables { get; }

        /// <summary>Names and labels this code uses (reference format, section names, vocabulary).</summary>
        CodeTerminology Terminology { get; }

        /// <summary>Exact or partial (parent article) lookup by reference.</summary>
        CodeArticle GetArticle(string reference);

        /// <summary>Ranked full-text search across references, titles, keywords and text.</summary>
        List<CodeArticle> Search(string query, int maxResults = 20);

        List<string> GetAllReferences();
        List<string> GetAllDisplayStrings();

        /// <summary>
        /// True if a student's citation is acceptable for the violation's expected citation
        /// under this code's numbering rules.
        /// </summary>
        bool CitationMatches(string cited, string expected);
    }
}
