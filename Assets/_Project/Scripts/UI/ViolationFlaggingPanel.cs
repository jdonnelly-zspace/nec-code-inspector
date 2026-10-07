using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NECInspector.Core;
using NECInspector.NEC;
using NECInspector.Codes;

namespace NECInspector.Inspection
{
    /// <summary>
    /// World-space popup panel for flagging violations on inspected components.
    /// Shows component info and provides NEC citation input appropriate to difficulty level.
    /// </summary>
    public class ViolationFlaggingPanel : MonoBehaviour
    {
        [Header("Component Info")]
        [SerializeField] private TMP_Text _componentNameText;
        [SerializeField] private TMP_Text _componentTypeText;
        [SerializeField] private TMP_Text _componentDescriptionText;

        [Header("Actions")]
        [SerializeField] private UnityEngine.UI.Button _flagViolationButton;
        [SerializeField] private UnityEngine.UI.Button _markCompliantButton;
        [SerializeField] private UnityEngine.UI.Button _cancelButton;

        [Header("Violation Entry")]
        [SerializeField] private GameObject _violationEntryGroup;
        [SerializeField] private TMP_InputField _descriptionInput;
        [SerializeField] private TMP_Dropdown _necDropdown;           // Beginner mode
        [SerializeField] private TMP_InputField _necSearchInput;      // Standard/Expert mode
        [SerializeField] private GameObject _searchResultsPanel;
        [SerializeField] private Transform _searchResultsContent;
        [SerializeField] private UnityEngine.UI.Button _submitButton;
        [SerializeField] private UnityEngine.UI.Button _cancelEntryButton;

        [Header("Search Result Prefab")]
        [SerializeField] private GameObject _searchResultItemPrefab;

        public event Action<InspectableComponent, string, string> OnViolationSubmitted;
        public event Action<InspectableComponent> OnMarkedCompliant;

        private InspectableComponent _currentComponent;
        private CitationMode _citationMode;
        private string _selectedReference;

        private void Awake()
        {
            _flagViolationButton?.onClick.AddListener(ShowViolationEntry);
            _markCompliantButton?.onClick.AddListener(OnMarkCompliant);
            _cancelButton?.onClick.AddListener(Hide);
            _submitButton?.onClick.AddListener(SubmitViolation);
            _cancelEntryButton?.onClick.AddListener(HideViolationEntry);

            _necSearchInput?.onValueChanged.AddListener(OnSearchTextChanged);

            gameObject.SetActive(false);
        }

        /// <summary>
        /// Show the panel for a specific component
        /// </summary>
        public void Show(InspectableComponent component, CitationMode citationMode)
        {
            _currentComponent = component;
            _citationMode = citationMode;
            _selectedReference = null;

            if (_componentNameText != null) _componentNameText.text = component.componentName;
            if (_componentTypeText != null) _componentTypeText.text = component.componentType;
            if (_componentDescriptionText != null) _componentDescriptionText.text = component.description;

            _violationEntryGroup?.SetActive(false);
            gameObject.SetActive(true);

            // Position near the component in world space
            transform.position = component.transform.position + Vector3.up * 0.3f;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
            _currentComponent = null;
        }

        private void ShowViolationEntry()
        {
            _violationEntryGroup?.SetActive(true);
            _descriptionInput?.SetTextWithoutNotify("");
            _selectedReference = null;

            // Show appropriate citation input based on difficulty
            bool isDropdown = _citationMode == CitationMode.Dropdown;
            _necDropdown?.gameObject.SetActive(isDropdown);
            _necSearchInput?.gameObject.SetActive(!isDropdown);
            _searchResultsPanel?.SetActive(false);

            if (isDropdown)
                PopulateDropdown();
            else
                _necSearchInput?.SetTextWithoutNotify("");
        }

        private void HideViolationEntry()
        {
            _violationEntryGroup?.SetActive(false);
        }

        private void PopulateDropdown()
        {
            if (_necDropdown == null || CodeProfiles.Active == null) return;

            _necDropdown.ClearOptions();
            var options = new List<TMP_Dropdown.OptionData> { new(CodeProfiles.Terminology.SelectReferencePrompt) };

            foreach (var display in CodeProfiles.Active.GetAllDisplayStrings())
            {
                options.Add(new TMP_Dropdown.OptionData(display));
            }

            _necDropdown.AddOptions(options);
            _necDropdown.onValueChanged.AddListener(OnDropdownChanged);
        }

        private void OnDropdownChanged(int index)
        {
            if (index <= 0)
            {
                _selectedReference = null;
                return;
            }

            var refs = CodeProfiles.Active.GetAllReferences();
            if (index - 1 < refs.Count)
                _selectedReference = refs[index - 1];
        }

        private void OnSearchTextChanged(string text)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length < 2)
            {
                _searchResultsPanel?.SetActive(false);
                return;
            }

            if (CodeProfiles.Active == null) return;

            var results = CodeProfiles.Active.Search(text, 8);
            _searchResultsPanel?.SetActive(results.Count > 0);

            // Clear existing results
            if (_searchResultsContent != null)
            {
                foreach (Transform child in _searchResultsContent)
                    Destroy(child.gameObject);
            }

            // Populate search results
            foreach (var article in results)
            {
                if (_searchResultItemPrefab != null && _searchResultsContent != null)
                {
                    var item = Instantiate(_searchResultItemPrefab, _searchResultsContent);
                    var text_comp = item.GetComponentInChildren<TMP_Text>();
                    if (text_comp != null) text_comp.text = article.DisplayString;

                    var button = item.GetComponent<UnityEngine.UI.Button>();
                    string articleRef = article.reference;
                    button?.onClick.AddListener(() =>
                    {
                        _selectedReference = articleRef;
                        _necSearchInput?.SetTextWithoutNotify(article.DisplayString);
                        _searchResultsPanel?.SetActive(false);
                    });
                }
            }
        }

        private void SubmitViolation()
        {
            if (_currentComponent == null) return;

            string description = _descriptionInput != null ? _descriptionInput.text : "";

            // For FreeText mode (Expert), use whatever they typed
            if (_citationMode == CitationMode.FreeText && string.IsNullOrEmpty(_selectedReference))
            {
                _selectedReference = _necSearchInput != null ? _necSearchInput.text : "";
            }

            if (string.IsNullOrEmpty(_selectedReference))
            {
                Debug.LogWarning("[FlaggingPanel] No NEC article selected");
                return;
            }

            OnViolationSubmitted?.Invoke(_currentComponent, description, _selectedReference);
            Hide();
        }

        private void OnMarkCompliant()
        {
            if (_currentComponent == null) return;
            OnMarkedCompliant?.Invoke(_currentComponent);
            Hide();
        }
    }
}
