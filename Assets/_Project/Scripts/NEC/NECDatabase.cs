using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NECInspector.NEC
{
    /// <summary>
    /// Singleton that loads NEC article data from StreamingAssets/NECDatabase/nec_articles.json
    /// and provides search/lookup functionality.
    /// </summary>
    public class NECDatabase : MonoBehaviour
    {
        public static NECDatabase Instance { get; private set; }

        private Dictionary<string, NECArticle> _articlesByReference = new();
        private List<NECArticle> _allArticles = new();
        private bool _isLoaded = false;

        public bool IsLoaded => _isLoaded;
        public int ArticleCount => _allArticles.Count;

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

                _allArticles = new List<NECArticle>(collection.articles);
                _articlesByReference.Clear();

                foreach (var article in _allArticles)
                {
                    string key = article.FullReference;
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
        /// Get an article by exact reference (e.g., "250.24(A)(1)" or "250.24")
        /// </summary>
        public NECArticle GetArticle(string reference)
        {
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
        public List<NECArticle> GetChapter(int chapter)
        {
            return _allArticles.Where(a => a.chapter == chapter).ToList();
        }

        /// <summary>
        /// Get articles new or changed in NEC 2026
        /// </summary>
        public List<NECArticle> GetNew2026Articles()
        {
            return _allArticles.Where(a => a.isNewIn2026).ToList();
        }

        /// <summary>
        /// Full-text search across article numbers, titles, keywords, and text
        /// </summary>
        public List<NECArticle> Search(string query, int maxResults = 20)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<NECArticle>();

            string lowerQuery = query.ToLowerInvariant();
            var results = new List<(NECArticle article, int score)>();

            foreach (var article in _allArticles)
            {
                int score = 0;

                // Exact article number match (highest priority)
                if (article.FullReference.ToLowerInvariant().Contains(lowerQuery))
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
            return _allArticles.Select(a => a.FullReference).OrderBy(r => r).ToList();
        }

        /// <summary>
        /// Get all display strings (for searchable dropdown)
        /// </summary>
        public List<string> GetAllDisplayStrings()
        {
            return _allArticles.Select(a => a.DisplayString).OrderBy(s => s).ToList();
        }
    }
}
