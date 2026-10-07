using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using NECInspector.Codes;

namespace NECInspector.NEC
{
    /// <summary>
    /// NEC code profile. Loads NEC article data from StreamingAssets/NECDatabase/nec_articles.json,
    /// exposes it through ICodeProfile, and registers itself as the active profile.
    /// </summary>
    public class NECDatabase : MonoBehaviour, ICodeProfile
    {
        public static NECDatabase Instance { get; private set; }

        private Dictionary<string, CodeArticle> _articlesByReference = new();
        private List<CodeArticle> _allArticles = new();
        private bool _isLoaded = false;

        public string ProfileId => CodeProfileIds.Nec;
        public string DisplayName => "NEC (NFPA 70)";
        public string Edition => "2026";
        public bool IsLoaded => _isLoaded;
        public int ArticleCount => _allArticles.Count;
        public ElectricalTables Tables => _tables ??= ElectricalTables.CreateNecDefaults();

        private ElectricalTables _tables;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadArticles();
            LoadTables();
            CodeProfiles.SetActive(this);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
            CodeProfiles.Clear(this);
        }

        private void LoadArticles()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "NECDatabase", "nec_articles.json");

            if (!File.Exists(path))
            {
                Debug.LogError($"[NECDatabase] Article database not found at {path}");
                return;
            }

            try
            {
                string json = File.ReadAllText(path);
                var collection = JsonUtility.FromJson<NECArticleCollection>(json);

                if (collection?.articles == null)
                {
                    Debug.LogError("[NECDatabase] Failed to parse article database");
                    return;
                }

                _allArticles = collection.articles.Select(ToCodeArticle).ToList();
                _articlesByReference.Clear();

                foreach (var article in _allArticles)
                {
                    string key = article.reference;
                    if (!_articlesByReference.ContainsKey(key))
                    {
                        _articlesByReference[key] = article;
                    }
                    else
                    {
                        Debug.LogWarning($"[NECDatabase] Duplicate article reference: {key}");
                    }
                }

                _isLoaded = true;
                Debug.Log($"[NECDatabase] Loaded {_allArticles.Count} NEC articles");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NECDatabase] Error loading articles: {e.Message}");
            }
        }

        /// <summary>
        /// Load voltages, ampacities, load-calc constants and compliance rule settings from
        /// StreamingAssets/NECDatabase/electrical_tables.json. Missing file or arrays fall back to NEC defaults.
        /// </summary>
        private void LoadTables()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "NECDatabase", "electrical_tables.json");

            if (!File.Exists(path))
            {
                Debug.LogWarning($"[NECDatabase] Electrical tables not found at {path}; using built-in NEC defaults");
                return;
            }

            try
            {
                var tables = JsonUtility.FromJson<ElectricalTables>(File.ReadAllText(path));
                if (tables == null)
                {
                    Debug.LogError("[NECDatabase] Failed to parse electrical tables; using built-in NEC defaults");
                    return;
                }

                tables.EnsureComplete();
                _tables = tables;
                Debug.Log($"[NECDatabase] Loaded electrical tables ({tables.conductorSizes.Length} conductor sizes, {tables.complianceRules.Length} rule settings)");
            }
            catch (Exception e)
            {
                Debug.LogError($"[NECDatabase] Error loading electrical tables: {e.Message}; using built-in NEC defaults");
            }
        }

        /// <summary>
        /// Convert the NEC JSON record into the code-neutral article type.
        /// </summary>
        private static CodeArticle ToCodeArticle(NECArticle source)
        {
            return new CodeArticle
            {
                reference = source.FullReference,
                referenceLabel = $"Art. {source.FullReference}",
                title = source.title,
                text = source.text,
                chapter = source.chapter,
                keywords = source.keywords,
                relatedReferences = source.relatedArticles,
                isNewInEdition = source.isNewIn2026
            };
        }

        /// <summary>
        /// Get an article by exact reference (e.g., "250.24(A)(1)" or "250.24")
        /// </summary>
        public CodeArticle GetArticle(string reference)
        {
            if (string.IsNullOrEmpty(reference))
                return null;

            if (_articlesByReference.TryGetValue(reference, out var article))
                return article;

            // Try partial match (article number without subsection)
            foreach (var kvp in _articlesByReference)
            {
                if (kvp.Key.StartsWith(reference))
                    return kvp.Value;
            }

            return null;
        }

        /// <summary>
        /// Get all articles in a chapter
        /// </summary>
        public List<CodeArticle> GetChapter(int chapter)
        {
            return _allArticles.Where(a => a.chapter == chapter).ToList();
        }

        /// <summary>
        /// Get articles new or changed in the current edition (NEC 2026)
        /// </summary>
        public List<CodeArticle> GetNewInEditionArticles()
        {
            return _allArticles.Where(a => a.isNewInEdition).ToList();
        }

        /// <summary>
        /// Full-text search across article numbers, titles, keywords, and text
        /// </summary>
        public List<CodeArticle> Search(string query, int maxResults = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<CodeArticle>();

            string lowerQuery = query.ToLowerInvariant();
            var results = new List<(CodeArticle article, int score)>();

            foreach (var article in _allArticles)
            {
                int score = 0;

                // Exact article number match (highest priority)
                if (article.reference.ToLowerInvariant().Contains(lowerQuery))
                    score += 100;

                // Title match
                if (article.title != null && article.title.ToLowerInvariant().Contains(lowerQuery))
                    score += 50;

                // Keyword match
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

                // Text match (lowest priority)
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

        /// <summary>
        /// Get all article references as a list (for dropdown population)
        /// </summary>
        public List<string> GetAllReferences()
        {
            return _allArticles.Select(a => a.reference).OrderBy(r => r).ToList();
        }

        /// <summary>
        /// Get all display strings (for searchable dropdown)
        /// </summary>
        public List<string> GetAllDisplayStrings()
        {
            return _allArticles.Select(a => a.DisplayString).OrderBy(s => s).ToList();
        }

        /// <summary>
        /// NEC references are hierarchical ("250.24(A)(1)"), so the default matcher applies.
        /// </summary>
        public bool CitationMatches(string cited, string expected)
        {
            return CitationMatcher.Default(cited, expected);
        }
    }
}
