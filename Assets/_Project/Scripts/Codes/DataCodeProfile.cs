using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// An installation code built entirely from data (profile.json, articles.json, tables.json,
    /// terminology.json). Adding a code means adding a folder of files, not writing a class.
    /// </summary>
    public class DataCodeProfile : ICodeProfile
    {
        private readonly CodeProfileManifest _manifest;
        private readonly List<CodeArticle> _articles;
        private readonly Dictionary<string, CodeArticle> _byReference;
        private readonly ElectricalTables _tables;

        public DataCodeProfile(CodeProfileManifest manifest, CodeArticleData[] articles,
            ElectricalTables tables, CodeTerminology terminology)
        {
            _manifest = manifest;
            Terminology = terminology;

            _tables = tables;
            _tables?.EnsureComplete();

            _articles = articles.Select(ToCodeArticle).ToList();
            _byReference = new Dictionary<string, CodeArticle>(System.StringComparer.OrdinalIgnoreCase);
            foreach (var article in _articles)
            {
                if (!_byReference.ContainsKey(article.reference))
                    _byReference[article.reference] = article;
            }
        }

        public string ProfileId => _manifest.id;
        public string DisplayName => _manifest.displayName;
        public string Edition => _manifest.edition;
        public string Region => _manifest.region;
        public string ArtSet => _manifest.artSet;
        public string ReviewStatus => _manifest.reviewStatus;
        public bool IsLoaded => true;
        public int ArticleCount => _articles.Count;
        public CodeTerminology Terminology { get; }

        /// <summary>Where this code requires GFCI and AFCI protection in a dwelling, or null if the folder has no scope.json.</summary>
        public ProtectionScope Scope { get; set; }

        /// <summary>False when the profile has no tables.json; sandbox calculations then use NEC values and must not count as evidence.</summary>
        public bool HasOwnTables => _tables != null;

        public ElectricalTables Tables => _tables ?? FallbackTables;
        private static ElectricalTables FallbackTables => _fallback ??= ElectricalTables.CreateNecDefaults();
        private static ElectricalTables _fallback;

        private CodeArticle ToCodeArticle(CodeArticleData source)
        {
            return new CodeArticle
            {
                reference = source.reference,
                referenceLabel = Terminology.ReferenceLabel(source.reference),
                title = source.title,
                text = source.text,
                chapter = source.section,
                keywords = source.keywords,
                relatedReferences = source.related,
                isNewInEdition = source.isNewInEdition,
                conceptId = source.conceptId
            };
        }

        /// <summary>Exact reference, or the entry a shorter (parent) reference leads to.</summary>
        public CodeArticle GetArticle(string reference)
        {
            if (string.IsNullOrWhiteSpace(reference))
                return null;

            if (_byReference.TryGetValue(reference, out var article))
                return article;

            foreach (var candidate in _articles)
            {
                if (CitationMatcher.IsParentOf(reference, candidate.reference))
                    return candidate;
            }

            return null;
        }

        public List<CodeArticle> GetSection(int section)
        {
            return _articles.Where(a => a.chapter == section).ToList();
        }

        public List<CodeArticle> GetNewInEditionArticles()
        {
            return _articles.Where(a => a.isNewInEdition).ToList();
        }

        /// <summary>Ranked full-text search across references, titles, keywords and text.</summary>
        public List<CodeArticle> Search(string query, int maxResults = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<CodeArticle>();

            string lowerQuery = query.ToLowerInvariant();
            var results = new List<(CodeArticle article, int score)>();

            foreach (var article in _articles)
            {
                int score = 0;

                if (article.reference.ToLowerInvariant().Contains(lowerQuery))
                    score += 100;

                if (article.title != null && article.title.ToLowerInvariant().Contains(lowerQuery))
                    score += 50;

                if (article.keywords != null)
                {
                    foreach (var keyword in article.keywords)
                    {
                        if (keyword.ToLowerInvariant().Contains(lowerQuery))
                        {
                            score += 30;
                            break;
                        }
                    }
                }

                if (article.text != null && article.text.ToLowerInvariant().Contains(lowerQuery))
                    score += 10;

                if (score > 0)
                    results.Add((article, score));
            }

            return results
                .OrderByDescending(r => r.score)
                .Take(maxResults)
                .Select(r => r.article)
                .ToList();
        }

        public List<string> GetAllReferences()
        {
            return _articles.Select(a => a.reference).OrderBy(r => r).ToList();
        }

        public List<string> GetAllDisplayStrings()
        {
            return _articles.Select(a => a.DisplayString).OrderBy(s => s).ToList();
        }

        public bool CitationMatches(string cited, string expected)
        {
            return CitationMatcher.Default(cited, expected);
        }
    }
}
