using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Checks nec_articles.json, the data behind the NEC profile's reference and citation features.
    /// </summary>
    public static class ArticleDatabaseTests
    {
        // Mirrors the fields of NECArticle in Scripts/NEC/NECArticle.cs
        private class Article
        {
            public string article { get; set; }
            public string subsection { get; set; }
            public string title { get; set; }
            public string text { get; set; }
            public int chapter { get; set; }
            public string[] relatedArticles { get; set; }
        }

        private class Collection
        {
            public Article[] articles { get; set; }
        }

        public static void Run(TestContext t)
        {
            t.Begin("article database");

            var db = Load();
            t.IsTrue(db.articles != null && db.articles.Length > 0, "database has articles");
            if (db.articles == null) return;

            var refs = db.articles.Select(a => Reference(a)).ToList();

            t.Equal(refs.Count, refs.Distinct().Count(), "article references are unique");

            foreach (var a in db.articles)
            {
                string r = Reference(a);
                t.IsTrue(!string.IsNullOrWhiteSpace(a.article), $"{r}: has an article number");
                t.IsTrue(!string.IsNullOrWhiteSpace(a.title), $"{r}: has a title");
                t.IsTrue(!string.IsNullOrWhiteSpace(a.text), $"{r}: has text");
            }

            // Related references the UI cannot resolve are silently skipped, so report them
            foreach (var a in db.articles)
            {
                if (a.relatedArticles == null) continue;
                foreach (string related in a.relatedArticles)
                    if (!Resolves(refs, related))
                        t.Warn($"{Reference(a)} lists related article '{related}' which is not in the database");
            }
        }

        public static List<string> LoadReferences()
        {
            var db = Load();
            return db.articles == null ? new List<string>() : db.articles.Select(a => Reference(a)).ToList();
        }

        /// <summary>Same lookup rule as NECDatabase.GetArticle: exact reference, or any reference starting with it.</summary>
        public static bool Resolves(List<string> refs, string reference)
        {
            if (string.IsNullOrEmpty(reference)) return false;
            return refs.Contains(reference) || refs.Any(r => r.StartsWith(reference));
        }

        private static string Reference(Article a)
        {
            return string.IsNullOrEmpty(a.subsection) ? a.article : a.article + a.subsection;
        }

        private static Collection Load()
        {
            string path = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/StreamingAssets/NECDatabase/nec_articles.json");
            return JsonSerializer.Deserialize<Collection>(File.ReadAllText(path));
        }
    }
}
