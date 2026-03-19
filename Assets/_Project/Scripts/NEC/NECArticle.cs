using System;

namespace NECInspector.NEC
{
    [Serializable]
    public class NECArticle
    {
        public string article;
        public string subsection;
        public string title;
        public string text;
        public int chapter;
        public string[] keywords;
        public string[] relatedArticles;
        public bool isNewIn2026;

        /// <summary>
        /// Full article reference string (e.g., "250.24(A)(1)")
        /// </summary>
        public string FullReference => string.IsNullOrEmpty(subsection)
            ? article
            : $"{article}{subsection}";

        /// <summary>
        /// Display string for UI (e.g., "Art. 250.24(A)(1) - Grounding Electrode Conductor Connection")
        /// </summary>
        public string DisplayString => $"Art. {FullReference} - {title}";
    }

    [Serializable]
    public class NECArticleCollection
    {
        public NECArticle[] articles;
    }
}
