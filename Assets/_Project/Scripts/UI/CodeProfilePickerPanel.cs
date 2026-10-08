using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using NECInspector.Codes;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel for choosing the installation code to study under (NEC, CEC, ...).
    /// Lists every loaded code profile with its review status and how much of the scenario content
    /// it covers, and applies the choice at once. The choice is saved with the student's progress.
    /// </summary>
    public class CodeProfilePickerPanel : MonoBehaviour
    {
        [Header("List")]
        [SerializeField] private Transform _listContent;
        [SerializeField] private GameObject _listItemPrefab;

        [Header("Detail")]
        [SerializeField] private TextMeshProUGUI _currentCodeText;
        [SerializeField] private TextMeshProUGUI _detailText;

        [Header("Scenario counts")]
        [Tooltip("Used to show how many scenarios each code covers")]
        [SerializeField] private ScenarioCatalogSO _scenarioCatalog;

        [Header("Controls")]
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        /// <summary>Raised after the student picks a different code.</summary>
        public event Action OnCodeChanged;

        private string _focusedId;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Hide);
        }

        private void OnEnable()
        {
            CodeProfiles.ActiveChanged += Refresh;
        }

        private void OnDisable()
        {
            CodeProfiles.ActiveChanged -= Refresh;
        }

        public void Show()
        {
            gameObject.SetActive(true);
            _focusedId = CodeProfiles.ActiveId;
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Refresh()
        {
            if (!gameObject.activeInHierarchy) return;

            var choices = BuildChoices();

            SetText(_currentCodeText, CurrentCodeLine());
            RebuildList(choices);

            var focused = choices.FirstOrDefault(c => c.id == _focusedId);
            SetText(_detailText, choices.Count == 0
                ? "No installation codes were found."
                : focused?.Details ?? "");
        }

        private List<CodeProfileChoice> BuildChoices()
        {
            var scenarios = _scenarioCatalog != null && _scenarioCatalog.scenarios != null
                ? _scenarioCatalog.scenarios.Where(s => s != null).ToList()
                : new List<ScenarioDefinitionSO>();

            return CodeProfileChoices.Build(
                CodeProfileLibrary.All,
                CodeProfiles.ActiveId,
                id => scenarios.Count(s => s.AppliesTo(id)),
                scenarios.Count);
        }

        private void RebuildList(List<CodeProfileChoice> choices)
        {
            if (_listContent == null) return;

            foreach (Transform child in _listContent)
                Destroy(child.gameObject);

            if (_listItemPrefab == null) return;

            foreach (var choice in choices)
            {
                var item = Instantiate(_listItemPrefab, _listContent);

                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = choice.Label;

                var button = item.GetComponent<UnityEngine.UI.Button>();
                string id = choice.id;
                button?.onClick.AddListener(() => Choose(id));
            }
        }

        private void Choose(string profileId)
        {
            AudioManager.Instance?.PlayButtonClick();
            _focusedId = profileId;

            if (profileId != CodeProfiles.ActiveId)
            {
                var progress = GameManager.Instance?.Progress;
                if (progress != null)
                {
                    progress.SetActiveCodeProfile(profileId);   // saves the choice and activates the code
                }
                else if (!CodeProfileLibrary.Activate(profileId))   // no game manager (a scene played on its own)
                {
                    return;
                }

                OnCodeChanged?.Invoke();
            }

            Refresh();
        }

        private static string CurrentCodeLine()
        {
            var profile = CodeProfiles.Active;
            return profile != null
                ? $"Studying under: {profile.DisplayName} {profile.Edition}"
                : "Studying under: no code loaded";
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
