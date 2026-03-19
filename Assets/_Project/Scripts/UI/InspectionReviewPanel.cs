using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.NEC;

namespace NECInspector.Inspection
{
    /// <summary>
    /// World-space panel for the review, NEC review, and score steps.
    /// Reused across multiple steps with different display modes.
    /// </summary>
    public class InspectionReviewPanel : MonoBehaviour
    {
        [Header("Summary View")]
        [SerializeField] private GameObject _summaryGroup;
        [SerializeField] private TMP_Text _summaryTitleText;
        [SerializeField] private TMP_Text _foundCountText;
        [SerializeField] private TMP_Text _missedCountText;
        [SerializeField] private TMP_Text _falsePositiveText;
        [SerializeField] private TMP_Text _citationAccuracyText;
        [SerializeField] private TMP_Text _timeText;

        [Header("Missed Violation View")]
        [SerializeField] private GameObject _missedGroup;
        [SerializeField] private TMP_Text _missedIndexText;
        [SerializeField] private TMP_Text _missedComponentText;
        [SerializeField] private TMP_Text _missedDescriptionText;
        [SerializeField] private TMP_Text _missedNECArticleText;
        [SerializeField] private TMP_Text _missedNECTextContent;

        [Header("Score View")]
        [SerializeField] private GameObject _scoreGroup;
        [SerializeField] private TMP_Text _gradeText;
        [SerializeField] private TMP_Text _accuracyText;
        [SerializeField] private TMP_Text _citationText;
        [SerializeField] private TMP_Text _totalTimeText;

        [Header("Perfect Score")]
        [SerializeField] private GameObject _perfectGroup;
        [SerializeField] private TMP_Text _perfectText;

        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button _continueButton;
        [SerializeField] private UnityEngine.UI.Button _retryButton;
        [SerializeField] private UnityEngine.UI.Button _menuButton;

        public event Action OnContinuePressed;
        public event Action OnRetryPressed;
        public event Action OnMenuPressed;

        private void Awake()
        {
            _continueButton?.onClick.AddListener(() => OnContinuePressed?.Invoke());
            _retryButton?.onClick.AddListener(() => OnRetryPressed?.Invoke());
            _menuButton?.onClick.AddListener(() => OnMenuPressed?.Invoke());

            HideAll();
        }

        private void HideAll()
        {
            _summaryGroup?.SetActive(false);
            _missedGroup?.SetActive(false);
            _scoreGroup?.SetActive(false);
            _perfectGroup?.SetActive(false);
            _retryButton?.gameObject.SetActive(false);
            _menuButton?.gameObject.SetActive(false);
        }

        public void ShowReviewSummary(InspectionScore score, List<ViolationDefinitionSO> missed)
        {
            HideAll();
            _summaryGroup?.SetActive(true);
            _continueButton?.gameObject.SetActive(true);

            if (_summaryTitleText != null) _summaryTitleText.text = "Inspection Review";
            if (_foundCountText != null) _foundCountText.text = $"Violations Found: {score.violationsFound} / {score.totalViolations}";
            if (_missedCountText != null) _missedCountText.text = $"Missed: {missed.Count}";
            if (_falsePositiveText != null) _falsePositiveText.text = $"False Positives: {score.falsePositives}";
            if (_citationAccuracyText != null) _citationAccuracyText.text = $"Citation Accuracy: {score.CitationAccuracy:P0}";
            if (_timeText != null) _timeText.text = $"Time: {FormatTime(score.timeElapsed)}";

            gameObject.SetActive(true);

            // Clear previous listeners to avoid stacking
            OnContinuePressed = null;
        }

        public void ShowMissedViolation(int violationIndex, int totalMissed,
            ViolationDefinitionSO violation, NECArticle article)
        {
            HideAll();
            _missedGroup?.SetActive(true);
            _continueButton?.gameObject.SetActive(true);

            if (_missedIndexText != null) _missedIndexText.text = $"Missed Violation {violationIndex} of {totalMissed}";
            if (_missedComponentText != null) _missedComponentText.text = violation.componentObjectName;
            if (_missedDescriptionText != null) _missedDescriptionText.text = violation.description;
            if (_missedNECArticleText != null) _missedNECArticleText.text = $"Art. {violation.necArticle}";
            if (_missedNECTextContent != null) _missedNECTextContent.text = article?.text ?? violation.necArticleText;

            gameObject.SetActive(true);
            OnContinuePressed = null;
        }

        public void ShowPerfectScore()
        {
            HideAll();
            _perfectGroup?.SetActive(true);
            _continueButton?.gameObject.SetActive(true);

            if (_perfectText != null)
                _perfectText.text = "All violations found! Excellent work.";

            gameObject.SetActive(true);
            OnContinuePressed = null;
        }

        public void ShowFinalScore(InspectionScore score)
        {
            HideAll();
            _scoreGroup?.SetActive(true);
            _retryButton?.gameObject.SetActive(true);
            _menuButton?.gameObject.SetActive(true);
            _continueButton?.gameObject.SetActive(false);

            if (_gradeText != null)
            {
                _gradeText.text = score.LetterGrade;
                _gradeText.color = score.LetterGrade switch
                {
                    "A" => new Color(0.2f, 0.9f, 0.3f),
                    "B" => new Color(0.5f, 0.9f, 0.2f),
                    "C" => new Color(1f, 0.85f, 0.2f),
                    "D" => new Color(1f, 0.5f, 0.2f),
                    _ => new Color(1f, 0.3f, 0.3f)
                };
            }

            if (_accuracyText != null) _accuracyText.text = $"Detection: {score.Accuracy:P0}";
            if (_citationText != null) _citationText.text = $"NEC Citations: {score.CitationAccuracy:P0}";
            if (_totalTimeText != null) _totalTimeText.text = $"Time: {FormatTime(score.timeElapsed)}";

            gameObject.SetActive(true);
        }

        private string FormatTime(float seconds)
        {
            int min = Mathf.FloorToInt(seconds / 60f);
            int sec = Mathf.FloorToInt(seconds % 60f);
            return $"{min}:{sec:00}";
        }
    }
}
