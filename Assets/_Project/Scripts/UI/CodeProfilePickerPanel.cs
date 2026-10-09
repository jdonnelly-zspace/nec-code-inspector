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
    /// World-space panel for choosing the student's region. The region decides the installation code
    /// and the unit system; the code itself is never named or offered. Lists each region that has a
    /// code loaded, with its units and how much scenario content it covers, and applies the choice at
    /// once. The choice is saved with the student's progress. (The class keeps its old name so scenes
    /// that already reference it keep working.)
    /// </summary>
    public class CodeProfilePickerPanel : MonoBehaviour
    {
        [Header("List")]
        [SerializeField] private Transform _listContent;
        [SerializeField] private GameObject _listItemPrefab;

        [Header("Detail")]
        [SerializeField] private TextMeshProUGUI _currentCodeText;   // shows the current region
        [SerializeField] private TextMeshProUGUI _detailText;

        [Header("Scenario counts")]
        [Tooltip("Used to show how many scenarios each region covers")]
        [SerializeField] private ScenarioCatalogSO _scenarioCatalog;

        [Header("Controls")]
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        /// <summary>Raised after the student picks a different region.</summary>
        public event Action OnCodeChanged;

        private string _focusedRegion;

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
            _focusedRegion = RegionChoices.Normalize(CodeProfiles.Active?.Region);
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

            SetText(_currentCodeText, CurrentRegionLine());
            RebuildList(choices);

            var focused = choices.FirstOrDefault(c => c.regionCode == _focusedRegion);
            SetText(_detailText, choices.Count == 0
                ? "No regions are set up."
                : focused?.Details ?? "");
        }

        private List<RegionChoice> BuildChoices()
        {
            var scenarios = _scenarioCatalog != null && _scenarioCatalog.scenarios != null
                ? _scenarioCatalog.scenarios.Where(s => s != null).ToList()
                : new List<ScenarioDefinitionSO>();

            return RegionChoices.Build(
                CodeProfileLibrary.All,
                CodeProfiles.ActiveId,
                id => scenarios.Count(s => s.AppliesTo(id)),
                scenarios.Count);
        }

        private void RebuildList(List<RegionChoice> choices)
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
                string region = choice.regionCode;
                button?.onClick.AddListener(() => Choose(region));
            }
        }

        private void Choose(string region)
        {
            AudioManager.Instance?.PlayButtonClick();
            _focusedRegion = region;

            var profile = RegionChoices.ProfileForRegion(CodeProfileLibrary.All, region);
            if (profile != null && profile.ProfileId != CodeProfiles.ActiveId)
            {
                var progress = GameManager.Instance?.Progress;
                if (progress != null)
                {
                    if (!progress.SetRegion(region)) return;   // saves the choice and activates the code for the region
                }
                else if (!CodeProfileLibrary.Activate(profile.ProfileId))   // no game manager (a scene played on its own)
                {
                    return;
                }

                OnCodeChanged?.Invoke();
            }

            Refresh();
        }

        private static string CurrentRegionLine()
        {
            var profile = CodeProfiles.Active;
            return profile != null
                ? $"Your region: {RegionNames.Of(profile.Region)}"
                : "Your region: not set";
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
