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
        public bool isNewInEdition;     // new or changed in this edition of the code

        /// <summary>
        /// Full article reference string (e.g., "250.24(A)(1)")
        /// </summary>
        public string FullReference => string.IsNullOrEmpty(subsection)
            ? article
            : $"{article}{subsection}";
    }

    [Serializable]
    public class NECArticleCollection
    {
        public NECArticle[] articles;
    }
}
