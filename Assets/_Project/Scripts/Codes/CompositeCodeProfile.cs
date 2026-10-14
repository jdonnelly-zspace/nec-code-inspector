using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// A student's code together with the companion standards that supplement it. Identity, tables, terminology and art come from the
    /// main code; looking up, searching and listing entries also reach the companions, so a violation can cite a fire alarm or
    /// line work entry and the student can find it.
    /// </summary>
    public class CompositeCodeProfile : ICodeProfile
    {
        private readonly ICodeProfile _main;
        private readonly List<ICodeProfile> _companions;

        public CompositeCodeProfile(ICodeProfile main, IEnumerable<ICodeProfile> companions)
        {
            _main = main;
            _companions = (companions ?? Enumerable.Empty<ICodeProfile>()).Where(c => c != null && c.ProfileId != main.ProfileId).ToList();
        }

        public ICodeProfile Main => _main;
        public IReadOnlyList<ICodeProfile> Companions => _companions;

        private IEnumerable<ICodeProfile> All => new[] { _main }.Concat(_companions);

        public string ProfileId => _main.ProfileId;
        public string DisplayName => _main.DisplayName;
        public string Edition => _main.Edition;
        public string Region => _main.Region;
        public string Units => _main.Units;
        public string ArtSet => _main.ArtSet;
        public string ReviewStatus => _main.ReviewStatus;
        public bool IsLoaded => _main.IsLoaded;
        public int ArticleCount => All.Sum(p => p.ArticleCount);
        public ElectricalTables Tables => _main.Tables;
        public bool HasOwnTables => _main.HasOwnTables;
        public CodeTerminology Terminology => _main.Terminology;

        /// <summary>The main code's entry first; then the companions' (exact or parent reference).</summary>
        public CodeArticle GetArticle(string reference)
        {
            foreach (var profile in All)
            {
                var article = profile.GetArticle(reference);
                if (article != null) return article;
            }

            return null;
        }

        /// <summary>Results from each code in turn (the best of each first), so a companion's entries are not buried.</summary>
        public List<CodeArticle> Search(string query, int maxResults = 20)
        {
            var lists = All.Select(p => p.Search(query, maxResults)).ToList();
            var merged = new List<CodeArticle>();
            for (int rank = 0; merged.Count < maxResults; rank++)
            {
                bool any = false;
                foreach (var list in lists)
                {
                    if (rank >= list.Count) continue;
                    any = true;
                    merged.Add(list[rank]);
                    if (merged.Count >= maxResults) break;
                }

                if (!any) break;
            }

            return merged;
        }

        public List<string> GetAllReferences() => All.SelectMany(p => p.GetAllReferences()).Distinct().OrderBy(r => r).ToList();
        public List<string> GetAllDisplayStrings() => All.SelectMany(p => p.GetAllDisplayStrings()).Distinct().OrderBy(s => s).ToList();

        /// <summary>True if the citation matches under the code that holds the expected reference.</summary>
        public bool CitationMatches(string cited, string expected)
        {
            foreach (var profile in All)
            {
                if (profile.GetArticle(expected) != null && profile.CitationMatches(cited, expected))
                    return true;
            }

            return _main.CitationMatches(cited, expected);
        }
    }
}
