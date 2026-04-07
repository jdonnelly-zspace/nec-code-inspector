using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using NECInspector.Data;
using NECInspector.Core;
using NECInspector.NEC;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel displaying quick reference cards.
    /// Cards are filtered by difficulty and can be searched by keyword or category.
    /// </summary>
    public class QuickReferenceCardPanel : MonoBehaviour
    {
        [Header("Card Data")]
        [SerializeField] private QuickReferenceCardSO[] _allCards;

        [Header("Card List")]
        [SerializeField] private Transform _cardListContent;
        [SerializeField] private GameObject _cardListItemPrefab;

        [Header("Category Filter")]
        [SerializeField] private TMP_Dropdown _categoryDropdown;

        [Header("Search")]
        [SerializeField] private TMP_InputField _searchInput;

        [Header("Card Display")]
        [SerializeField] private GameObject _cardDetailGroup;
        [SerializeField] private TextMeshProUGUI _cardTitle;
        [SerializeField] private TextMeshProUGUI _cardCategory;
        [SerializeField] private TextMeshProUGUI _cardSummary;
        [SerializeField] private TextMeshProUGUI _cardKeyRule;
        [SerializeField] private TextMeshProUGUI _cardNECRefs;

        [Header("NEC Link")]
        [SerializeField] private NECReferencePanel _necPanel;

        [Header("Controls")]
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        private List<QuickReferenceCardSO> _filteredCards = new();
        private CardCategory? _selectedCategory;

        private void Awake()
        {
            _searchInput?.onValueChanged.AddListener(OnSearchChanged);
            _categoryDropdown?.onValueChanged.AddListener(OnCategoryChanged);
            _closeButton?.onClick.AddListener(Hide);
            _cardDetailGroup?.SetActive(false);

            PopulateCategoryDropdown();
        }

        public void Show()
        {
            gameObject.SetActive(true);
            RefreshCardList();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show a specific card by ID, e.g., from a violation review context.
        /// </summary>
        public void ShowCardForContext(string necReference)
        {
            Show();
            var card = _allCards.FirstOrDefault(c =>
                c.necReferences != null && c.necReferences.Any(r => r == necReference));

            if (card != null)
                DisplayCard(card);
        }

        private void RefreshCardList()
        {
            ClearCardList();

            var difficulty = GameManager.Instance?.Difficulty?.CurrentLevel ?? DifficultyLevel.Beginner;
            string search = _searchInput != null ? _searchInput.text?.Trim().ToLower() : "";

            _filteredCards.Clear();
            foreach (var card in _allCards)
            {
                if (card == null) continue;
                if ((int)card.minimumDifficulty > (int)difficulty) continue;
                if (_selectedCategory.HasValue && card.category != _selectedCategory.Value) continue;

                if (!string.IsNullOrEmpty(search))
                {
                    bool matches = card.title.ToLower().Contains(search)
                        || card.summary.ToLower().Contains(search)
                        || (card.keywords != null && card.keywords.Any(k => k.ToLower().Contains(search)));
                    if (!matches) continue;
                }

                _filteredCards.Add(card);
            }

            // Sort by category then title
            _filteredCards.Sort((a, b) =>
            {
                int cat = a.category.CompareTo(b.category);
                return cat != 0 ? cat : string.Compare(a.title, b.title, StringComparison.Ordinal);
            });

            foreach (var card in _filteredCards)
            {
                if (_cardListItemPrefab == null || _cardListContent == null) continue;

                var item = Instantiate(_cardListItemPrefab, _cardListContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = card.DisplayTitle;

                var button = item.GetComponent<UnityEngine.UI.Button>();
                var captured = card;
                button?.onClick.AddListener(() => DisplayCard(captured));
            }
        }

        private void DisplayCard(QuickReferenceCardSO card)
        {
            _cardDetailGroup?.SetActive(true);

            SetText(_cardTitle, card.title);
            SetText(_cardCategory, card.category.ToString());
            SetText(_cardSummary, card.summary);
            SetText(_cardKeyRule, card.keyRule);

            // Build NEC references with links
            if (_cardNECRefs != null && card.necReferences != null)
            {
                var refs = new List<string>();
                foreach (var r in card.necReferences)
                    refs.Add($"Art. {r}");
                _cardNECRefs.text = string.Join("  |  ", refs);
            }
        }

        /// <summary>
        /// Called by NEC reference link buttons on a displayed card.
        /// Opens the NECReferencePanel to the specified article.
        /// </summary>
        public void OpenNECArticle(string reference)
        {
            if (_necPanel == null || NECDatabase.Instance == null) return;
            var article = NECDatabase.Instance.GetArticle(reference);
            if (article != null)
                _necPanel.ShowArticle(article);
        }

        private void OnSearchChanged(string query) => RefreshCardList();

        private void OnCategoryChanged(int index)
        {
            if (index == 0)
                _selectedCategory = null; // "All" option
            else
                _selectedCategory = (CardCategory)(index - 1);
            RefreshCardList();
        }

        private void PopulateCategoryDropdown()
        {
            if (_categoryDropdown == null) return;
            _categoryDropdown.ClearOptions();

            var options = new List<string> { "All Categories" };
            foreach (CardCategory cat in Enum.GetValues(typeof(CardCategory)))
                options.Add(cat.ToString());
            _categoryDropdown.AddOptions(options);
        }

        private void ClearCardList()
        {
            if (_cardListContent == null) return;
            foreach (Transform child in _cardListContent)
                Destroy(child.gameObject);
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
