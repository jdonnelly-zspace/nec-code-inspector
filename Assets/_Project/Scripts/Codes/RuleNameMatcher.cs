using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NECInspector.Codes
{
    /// <summary>
    /// Works out which rule a student means when they name it in their own words, for the Master level where no list or number is shown.
    /// The typed words are compared with each entry's title and keywords. Pure logic, so it is tested without the engine.
    ///
    /// <see cref="Names"/> asks whether the words plausibly name one particular rule (the one the violation expects): at least two meaningful words match
    /// and that rule scores close to the best of all. <see cref="ConceptOf"/> asks which skill the words are about, even if they are too general to
    /// name one rule ("gfci" is about shock protection), so a near miss still earns the same-skill credit.
    /// </summary>
    public static class RuleNameMatcher
    {
        public const float NameThreshold = 0.55f;       // an entry must score at least this to count as named
        public const float TieMargin = 0.1f;            // and be within this of the best entry
        public const float ConceptThreshold = 0.3f;     // the best entry must score at least this for a skill to be read from the words
        public const int MinMatchedWords = 2;           // to name a rule, not just a topic

        private static readonly HashSet<string> Stop = new HashSet<string>
        {
            "a", "an", "the", "of", "in", "on", "at", "for", "to", "and", "or", "with", "is", "are", "be", "by", "it", "its", "this", "that", "no", "not",
            "must", "need", "needs", "needed", "should", "rule", "rules", "requirement", "requirements", "required", "code", "article", "section", "about", "when"
        };

        public struct RuleMatch
        {
            public CodeArticle article;
            public float score;
            public int matchedWords;
            public int titleWords;
        }

        /// <summary>Every entry the words touch, best first.</summary>
        public static List<RuleMatch> Rank(IEnumerable<CodeArticle> articles, string text)
        {
            var typed = Tokens(text);
            var result = new List<RuleMatch>();
            if (articles == null || typed.Count == 0) return result;

            foreach (var article in articles)
            {
                if (article == null) continue;
                var title = Tokens(article.title);
                var keywords = new HashSet<string>(Tokens(string.Join(" ", article.keywords ?? new string[0])));
                keywords.ExceptWith(title);
                if (title.Count == 0 && keywords.Count == 0) continue;

                int titleHits = title.Count(typed.Contains);
                int keywordHits = keywords.Count(typed.Contains);
                var all = new HashSet<string>(title);
                all.UnionWith(keywords);
                int typedHits = typed.Count(all.Contains);
                if (typedHits == 0) continue;

                // Long titles are not held against the student: naming the first three words of a title is a full match
                float titleRecall = title.Count == 0 ? 0f : Math.Min(1f, titleHits / (float)Math.Min(title.Count, 3));
                float precision = typedHits / (float)typed.Count;
                float keywordBonus = Math.Min(1f, keywordHits / 3f);
                float fullTitle = title.Count == 0 ? 0f : titleHits / (float)title.Count;   // naming the whole title scores a little more than its first words
                float score = 0.4f * titleRecall + 0.1f * fullTitle + 0.3f * precision + 0.2f * keywordBonus;
                result.Add(new RuleMatch { article = article, score = score, matchedWords = typedHits, titleWords = title.Count });
            }

            return result.OrderByDescending(m => m.score).ThenBy(m => m.article.reference, StringComparer.Ordinal).ToList();
        }

        /// <summary>True if the words plausibly name the expected rule: enough words match it, and it scores near the top.</summary>
        public static bool Names(IEnumerable<CodeArticle> articles, string text, string expectedReference)
        {
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrEmpty(expectedReference)) return false;

            var ranked = Rank(articles, text);
            if (ranked.Count == 0) return false;

            var expected = ranked.FirstOrDefault(m => string.Equals(m.article.reference, expectedReference, StringComparison.OrdinalIgnoreCase));
            if (expected.article == null) return false;
            return expected.matchedWords >= Math.Min(MinMatchedWords, Math.Max(1, expected.titleWords)) && expected.score >= NameThreshold && expected.score >= ranked[0].score - TieMargin;
        }

        /// <summary>The skill the words are about, or empty if the best matches belong to different skills or nothing matches well.</summary>
        public static string ConceptOf(IEnumerable<CodeArticle> articles, string text)
        {
            var ranked = Rank(articles, text);
            if (ranked.Count == 0 || ranked[0].score < ConceptThreshold) return "";

            // One entry clearly ahead of the rest: its skill
            if (ranked[0].score >= 0.5f && (ranked.Count == 1 || ranked[0].score - ranked[1].score >= TieMargin)) return ranked[0].article.conceptId ?? "";

            // Otherwise the skill that holds most of the weight among the best matches, if it holds at least half
            var best = ranked.Where(m => m.score >= ranked[0].score - TieMargin).ToList();
            var byConcept = best.GroupBy(m => m.article.conceptId ?? "").Select(g => new { concept = g.Key, weight = g.Sum(m => m.score * m.score) }).OrderByDescending(g => g.weight).ToList();
            float total = byConcept.Sum(g => g.weight);
            return byConcept[0].weight * 2f >= total ? byConcept[0].concept : "";
        }

        /// <summary>All entries of a profile, for matching against its rules.</summary>
        public static List<CodeArticle> ArticlesOf(ICodeProfile profile)
        {
            var list = new List<CodeArticle>();
            if (profile == null) return list;
            foreach (string reference in profile.GetAllReferences())
            {
                var article = profile.GetArticle(reference);
                if (article != null && string.Equals(article.reference, reference, StringComparison.Ordinal)) list.Add(article);
            }
            return list;
        }

        /// <summary>True if the text is words rather than a reference number, so it is worth matching by name.</summary>
        public static bool LooksLikeWords(string text)
        {
            return Tokens(text).Count > 0;
        }

        private static HashSet<string> Tokens(string text)
        {
            var set = new HashSet<string>();
            if (string.IsNullOrWhiteSpace(text)) return set;

            foreach (Match m in Regex.Matches(text.ToLowerInvariant(), @"[a-z][a-z]+"))
            {
                string word = m.Value;
                if (Stop.Contains(word)) continue;
                if (word.EndsWith("ies", StringComparison.Ordinal) && word.Length > 4) word = word.Substring(0, word.Length - 3) + "y";
                else if (word.EndsWith("s", StringComparison.Ordinal) && !word.EndsWith("ss", StringComparison.Ordinal) && word.Length > 3) word = word.Substring(0, word.Length - 1);
                set.Add(word);
            }

            // Everyday words for the same thing
            if (set.Contains("ground") && set.Contains("fault")) set.Add("gfci");
            if (set.Contains("arc") && set.Contains("fault")) set.Add("afci");
            if (set.Contains("counter")) set.Add("countertop");
            return set;
        }
    }
}
