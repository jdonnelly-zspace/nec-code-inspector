using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space main menu UI. Provides mode selection (inspection scenarios,
    /// panel sandbox, reference, progress) and difficulty selection.
    /// All canvases use World Space render mode for zSpace stereo.
    /// </summary>
    public class MainMenuPanel : MonoBehaviour
    {
        [Header("Title")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _subtitleText;

        [Header("Mode Selection")]
        [SerializeField] private GameObject _modeSelectionPanel;

        [Header("Scenario Selection")]
        [SerializeField] private GameObject _scenarioSelectionPanel;
        [SerializeField] private Transform _scenarioListContent;
        [SerializeField] private GameObject _scenarioListItemPrefab;
        [SerializeField] private ScenarioCatalogSO _scenarioCatalog;

        [Header("Scenario Detail")]
        [SerializeField] private GameObject _scenarioDetailPanel;
        [SerializeField] private TextMeshProUGUI _scenarioTitle;
        [SerializeField] private TextMeshProUGUI _scenarioDescription;
        [SerializeField] private TextMeshProUGUI _scenarioDifficulties;
        [SerializeField] private TextMeshProUGUI _scenarioBestScore;

        [Header("Difficulty Selection")]
        [SerializeField] private GameObject _difficultyPanel;
        [SerializeField] private TextMeshProUGUI _difficultyDescription;

        [Header("Settings")]
        [SerializeField] private GameObject _settingsPanel;
        [SerializeField] private UnityEngine.UI.Slider _masterVolumeSlider;
        [SerializeField] private UnityEngine.UI.Slider _sfxVolumeSlider;
        [SerializeField] private UnityEngine.UI.Slider _ambientVolumeSlider;
        [SerializeField] private TMP_InputField _studentNameInput;

        private ScenarioDefinitionSO _selectedScenario;

        private void Start()
        {
            ShowModeSelection();

            if (_masterVolumeSlider != null)
                _masterVolumeSlider.onValueChanged.AddListener(v => { if (AudioManager.Instance != null) AudioManager.Instance.MasterVolume = v; });
            if (_sfxVolumeSlider != null)
                _sfxVolumeSlider.onValueChanged.AddListener(v => { if (AudioManager.Instance != null) AudioManager.Instance.SFXVolume = v; });
            if (_ambientVolumeSlider != null)
                _ambientVolumeSlider.onValueChanged.AddListener(v => { if (AudioManager.Instance != null) AudioManager.Instance.AmbientVolume = v; });
        }

        #region Panel Navigation

        public void ShowModeSelection()
        {
            HideAllPanels();
            SetActive(_modeSelectionPanel, true);
        }

        public void ShowScenarioSelection()
        {
            HideAllPanels();
            SetActive(_scenarioSelectionPanel, true);
            PopulateScenarioList();
        }

        public void ShowSandboxMode()
        {
            AudioManager.Instance?.PlayButtonClick();
            var transition = SceneTransitionManager.Instance;
            if (transition != null)
                transition.TransitionToScene("PanelSandbox");
            else
                GameManager.Instance?.LoadScene("PanelSandbox");
        }

        public void ShowReferenceMode()
        {
            HideAllPanels();
            // Reference panel is shown in-scene; the MainMenu scene would have
            // a NECReferencePanel + QuickReferenceCardPanel that can be activated
        }

        public void ShowProgressDashboard()
        {
            HideAllPanels();
            // Progress dashboard is shown in-scene via ProgressDashboardPanel
        }

        public void ShowSettings()
        {
            HideAllPanels();
            SetActive(_settingsPanel, true);
            LoadSettingsUI();
        }

        #endregion

        #region Scenario Selection

        private void PopulateScenarioList()
        {
            ClearContent(_scenarioListContent);
            if (_scenarioCatalog == null) return;

            foreach (var scenario in _scenarioCatalog.scenarios)
            {
                if (scenario == null) continue;

                var item = Instantiate(_scenarioListItemPrefab, _scenarioListContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = scenario.displayName;

                var button = item.GetComponent<UnityEngine.UI.Button>();
                var captured = scenario;
                button?.onClick.AddListener(() => SelectScenario(captured));
            }
        }

        private void SelectScenario(ScenarioDefinitionSO scenario)
        {
            _selectedScenario = scenario;
            AudioManager.Instance?.PlayButtonClick();

            SetActive(_scenarioDetailPanel, true);
            SetText(_scenarioTitle, scenario.displayName);
            SetText(_scenarioDescription, scenario.description);

            // Show available difficulties
            var diffs = new List<string>();
            foreach (var d in scenario.availableDifficulties)
                diffs.Add(d.ToString());
            SetText(_scenarioDifficulties, $"Difficulties: {string.Join(", ", diffs)}");

            // Show best score if available
            var best = GameManager.Instance?.Progress?.GetBestScenarioAttempt(scenario.id);
            if (best != null)
            {
                float acc = best.totalViolations > 0 ? (float)best.violationsFound / best.totalViolations : 0f;
                SetText(_scenarioBestScore, $"Best: {acc:P0} ({best.difficulty})");
            }
            else
            {
                SetText(_scenarioBestScore, "Not attempted");
            }
        }

        public void LaunchSelectedScenario()
        {
            if (_selectedScenario == null) return;

            AudioManager.Instance?.PlayButtonClick();

            var transition = SceneTransitionManager.Instance;
            if (transition != null)
                transition.TransitionToScene(_selectedScenario.sceneName);
            else
                GameManager.Instance?.LoadScene(_selectedScenario.sceneName);
        }

        #endregion

        #region Difficulty

        public void ShowDifficultySelection()
        {
            SetActive(_difficultyPanel, true);
            UpdateDifficultyDisplay();
        }

        public void SetDifficultyBeginner() => SetDifficulty(DifficultyLevel.Beginner);
        public void SetDifficultyStandard() => SetDifficulty(DifficultyLevel.Standard);
        public void SetDifficultyExpert() => SetDifficulty(DifficultyLevel.Expert);

        private void SetDifficulty(DifficultyLevel level)
        {
            GameManager.Instance?.Difficulty?.SetDifficulty(level);
            AudioManager.Instance?.PlayButtonClick();
            UpdateDifficultyDisplay();
        }

        private void UpdateDifficultyDisplay()
        {
            var level = GameManager.Instance?.Difficulty?.CurrentLevel ?? DifficultyLevel.Standard;
            string desc = level switch
            {
                DifficultyLevel.Beginner => "CTE Students: Guided inspection with dropdown NEC citations, highlight hints, and scaffolding. Fewer violations to find.",
                DifficultyLevel.Standard => "Apprentices: Searchable NEC citations, no hints. All standard violations active.",
                DifficultyLevel.Expert => "Licensed Electricians: Free-text NEC citations, time limits, subtle violations, false positive penalties.",
                _ => ""
            };
            SetText(_difficultyDescription, $"Current: {level}\n\n{desc}");
        }

        #endregion

        #region Settings

        private void LoadSettingsUI()
        {
            if (_masterVolumeSlider != null && AudioManager.Instance != null)
                _masterVolumeSlider.value = AudioManager.Instance.MasterVolume;
            if (_sfxVolumeSlider != null && AudioManager.Instance != null)
                _sfxVolumeSlider.value = AudioManager.Instance.SFXVolume;
            if (_ambientVolumeSlider != null && AudioManager.Instance != null)
                _ambientVolumeSlider.value = AudioManager.Instance.AmbientVolume;
            if (_studentNameInput != null)
                _studentNameInput.text = GameManager.Instance?.Progress?.Data?.studentName ?? "";
        }

        public void SaveStudentName()
        {
            if (_studentNameInput == null || GameManager.Instance?.Progress == null) return;
            GameManager.Instance.Progress.Data.studentName = _studentNameInput.text;
            GameManager.Instance.Progress.Save();
        }

        #endregion

        #region Helpers

        private void HideAllPanels()
        {
            SetActive(_modeSelectionPanel, false);
            SetActive(_scenarioSelectionPanel, false);
            SetActive(_scenarioDetailPanel, false);
            SetActive(_difficultyPanel, false);
            SetActive(_settingsPanel, false);
        }

        private void SetActive(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }

        private void ClearContent(Transform content)
        {
            if (content == null) return;
            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        #endregion
    }
}
