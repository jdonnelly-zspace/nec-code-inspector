using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NECInspector.NEC;
using NECInspector.Codes;

namespace NECInspector.Inspection
{
    /// <summary>
    /// World-space panel for browsing and searching NEC articles.
    /// Used during NEC review and as a quick-reference tool during inspection.
    /// </summary>
    public class NECReferencePanel : MonoBehaviour
    {
        [Header("Search")]
        [SerializeField] private TMP_InputField _searchInput;
        [SerializeField] private Transform _searchResultsContent;
        [SerializeField] private GameObject _searchResultPrefab;

        [Header("Article Display")]
        [SerializeField] private GameObject _articleGroup;
        [SerializeField] private TMP_Text _articleRefText;
        [SerializeField] private TMP_Text _articleTitleText;
        [SerializeField] private TMP_Text _articleBodyText;
        [SerializeField] private TMP_Text _chapterText;
        [SerializeField] private GameObject _newIn2026Badge;

        [Header("Related Articles")]
        [SerializeField] private Transform _relatedContent;
        [SerializeField] private GameObject _relatedItemPrefab;

        [Header("Controls")]
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        private void Awake()
        {
            _searchInput?.onValueChanged.AddListener(OnSearchChanged);
            _closeButton?.onClick.AddListener(Hide);
            _articleGroup?.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void ShowArticle(CodeArticle article)
        {
            if (article == null) return;

            _articleGroup?.SetActive(true);
            gameObject.SetActive(true);

            if (_articleRefText != null) _articleRefText.text = article.referenceLabel;
            if (_articleTitleText != null) _articleTitleText.text = article.title;
            if (_articleBodyText != null) _articleBodyText.text = article.text;
            if (_chapterText != null) _chapterText.text = CodeProfiles.Terminology.SectionName(article.chapter);
            _newIn2026Badge?.SetActive(article.isNewInEdition);

            // Populate related articles
            PopulateRelated(article.relatedReferences);
        }

        private void OnSearchChanged(string query)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2 || CodeProfiles.Active == null)
            {
                ClearSearchResults();
                return;
            }

            var results = CodeProfiles.Active.Search(query, 10);
            ClearSearchResults();

            foreach (var article in results)
            {
                if (_searchResultPrefab != null && _searchResultsContent != null)
                {
                    var item = Instantiate(_searchResultPrefab, _searchResultsContent);
                    var text = item.GetComponentInChildren<TMP_Text>();
                    if (text != null) text.text = article.DisplayString;

                    var button = item.GetComponent<UnityEngine.UI.Button>();
                    var captured = article;
                    button?.onClick.AddListener(() => ShowArticle(captured));
                }
            }
        }

        private void PopulateRelated(string[] relatedRefs)
        {
            if (_relatedContent == null) return;

            foreach (Transform child in _relatedContent)
                Destroy(child.gameObject);

            if (relatedRefs == null || CodeProfiles.Active == null) return;

            foreach (var refStr in relatedRefs)
            {
                var related = CodeProfiles.Active.GetArticle(refStr);
                if (related == null) continue;

                if (_relatedItemPrefab != null)
                {
                    var item = Instantiate(_relatedItemPrefab, _relatedContent);
                    var text = item.GetComponentInChildren<TMP_Text>();
                    if (text != null) text.text = related.DisplayString;

                    var button = item.GetComponent<UnityEngine.UI.Button>();
                    var captured = related;
                    button?.onClick.AddListener(() => ShowArticle(captured));
                }
            }
        }

        private void ClearSearchResults()
        {
            if (_searchResultsContent == null) return;
            foreach (Transform child in _searchResultsContent)
                Destroy(child.gameObject);
        }
    }
}
