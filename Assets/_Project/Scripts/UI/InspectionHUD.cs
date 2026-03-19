using System;
using UnityEngine;
using TMPro;

namespace NECInspector.Inspection
{
    /// <summary>
    /// World-space HUD for inspection scenarios.
    /// Shows scenario info, violation count, timer, and action buttons.
    /// Attach to a World Space Canvas parented to ZCameraRig.
    /// </summary>
    public class InspectionHUD : MonoBehaviour
    {
        [Header("Intro Panel")]
        [SerializeField] private GameObject _introPanel;
        [SerializeField] private TMP_Text _introTitleText;
        [SerializeField] private TMP_Text _introDescriptionText;
        [SerializeField] private TMP_Text _introObjectivesText;
        [SerializeField] private TMP_Text _introChaptersText;
        [SerializeField] private UnityEngine.UI.Button _introContinueButton;

        [Header("Inspection Panel")]
        [SerializeField] private GameObject _inspectionPanel;
        [SerializeField] private TMP_Text _violationCountText;
        [SerializeField] private TMP_Text _timerText;
        [SerializeField] private GameObject _timerGroup;
        [SerializeField] private UnityEngine.UI.Button _finishButton;
        [SerializeField] private UnityEngine.UI.Button _hintButton;

        [Header("Status")]
        [SerializeField] private TMP_Text _stepLabel;

        public event Action OnContinuePressed;
        public event Action OnFinishPressed;
        public event Action OnHintPressed;

        private void Awake()
        {
            _introPanel?.SetActive(false);
            _inspectionPanel?.SetActive(false);

            _introContinueButton?.onClick.AddListener(() => OnContinuePressed?.Invoke());
            _finishButton?.onClick.AddListener(() => OnFinishPressed?.Invoke());
            _hintButton?.onClick.AddListener(() => OnHintPressed?.Invoke());
        }

        public void ShowIntroPanel(string title, string description, string objectives, string[] chapters)
        {
            _introPanel?.SetActive(true);
            _inspectionPanel?.SetActive(false);

            if (_introTitleText != null) _introTitleText.text = title;
            if (_introDescriptionText != null) _introDescriptionText.text = description;
            if (_introObjectivesText != null) _introObjectivesText.text = objectives;
            if (_introChaptersText != null && chapters != null)
                _introChaptersText.text = "NEC Chapters: " + string.Join(", ", chapters);
        }

        public void HideIntroPanel()
        {
            _introPanel?.SetActive(false);
        }

        public void ShowInspectionHUD(int totalViolations, int flaggedCount)
        {
            _inspectionPanel?.SetActive(true);
            UpdateFlaggedCount(flaggedCount);
            _timerGroup?.SetActive(false); // Hidden until time limit kicks in
        }

        public void HideInspectionHUD()
        {
            _inspectionPanel?.SetActive(false);
        }

        public void UpdateFlaggedCount(int count)
        {
            if (_violationCountText != null)
                _violationCountText.text = $"Flagged: {count}";
        }

        public void UpdateTimer(float remainingSeconds)
        {
            _timerGroup?.SetActive(true);
            if (_timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
                int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
                _timerText.text = $"{minutes:00}:{seconds:00}";

                // Red text when under 2 minutes
                _timerText.color = remainingSeconds < 120f
                    ? new Color(1f, 0.3f, 0.3f)
                    : Color.white;
            }
        }

        public void ShowTimeLimitWarning()
        {
            if (_timerText != null)
                _timerText.text = "TIME'S UP!";
        }

        public void SetStepLabel(string label)
        {
            if (_stepLabel != null)
                _stepLabel.text = label;
        }

        public void ShowHintButton(bool show)
        {
            _hintButton?.gameObject.SetActive(show);
        }
    }
}
