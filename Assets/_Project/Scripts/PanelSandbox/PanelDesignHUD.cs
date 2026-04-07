using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// World-space UI for the Panel Design Sandbox.
    /// All canvases use World Space render mode (required for zSpace stereo).
    /// </summary>
    public class PanelDesignHUD : MonoBehaviour
    {
        [Header("Briefing Panel")]
        [SerializeField] private GameObject _briefingPanel;
        [SerializeField] private TextMeshProUGUI _briefingTitle;
        [SerializeField] private TextMeshProUGUI _briefingDescription;
        [SerializeField] private TextMeshProUGUI _briefingRequirements;

        [Header("Design Panel")]
        [SerializeField] private GameObject _designPanel;
        [SerializeField] private TextMeshProUGUI _breakerCountText;
        [SerializeField] private TextMeshProUGUI _loadDisplayText;
        [SerializeField] private TextMeshProUGUI _instructionText;

        [Header("Wiring Panel")]
        [SerializeField] private GameObject _wiringPanel;
        [SerializeField] private TextMeshProUGUI _wiringInstructionText;

        [Header("Compliance Panel")]
        [SerializeField] private GameObject _compliancePanel;
        [SerializeField] private TextMeshProUGUI _complianceTitle;
        [SerializeField] private TextMeshProUGUI _complianceResultsText;

        [Header("Score Panel")]
        [SerializeField] private GameObject _scorePanel;
        [SerializeField] private TextMeshProUGUI _gradeText;
        [SerializeField] private TextMeshProUGUI _complianceRateText;
        [SerializeField] private TextMeshProUGUI _loadAccuracyText;
        [SerializeField] private TextMeshProUGUI _timeText;

        public event Action OnContinuePressed;
        public event Action OnFinishPressed;
        public event Action OnRunCheckPressed;
        public event Action OnRetryPressed;
        public event Action OnMenuPressed;

        private void Awake()
        {
            HideAllPanels();
        }

        public void HideAllPanels()
        {
            SetActive(_briefingPanel, false);
            SetActive(_designPanel, false);
            SetActive(_wiringPanel, false);
            SetActive(_compliancePanel, false);
            SetActive(_scorePanel, false);
        }

        #region Briefing

        public void ShowBriefingPanel(PanelDesignDefinitionSO definition)
        {
            HideAllPanels();
            SetActive(_briefingPanel, true);

            SetText(_briefingTitle, definition.displayName);
            SetText(_briefingDescription, definition.description);

            // Build requirements list
            var lines = new System.Text.StringBuilder();
            lines.AppendLine($"Panel: {definition.panelType} ({definition.totalAmps}A, {definition.totalSlots} spaces)");
            lines.AppendLine($"Square Footage: {definition.dwellingSquareFootage:N0} sq ft");
            lines.AppendLine();
            lines.AppendLine("Required Circuits:");

            foreach (var circuit in definition.requiredCircuits)
            {
                if (!circuit.isRequired) continue;
                string protection = "";
                if (circuit.requiresGFCI && circuit.requiresAFCI) protection = " [GFCI+AFCI]";
                else if (circuit.requiresGFCI) protection = " [GFCI]";
                else if (circuit.requiresAFCI) protection = " [AFCI]";

                string poles = circuit.poleCount == 2 ? " (240V)" : "";
                lines.AppendLine($"  - {circuit.circuitName}: {circuit.ampsRequired}A, {circuit.wireGauge}{poles}{protection}");
            }

            SetText(_briefingRequirements, lines.ToString());
        }

        #endregion

        #region Design

        public void ShowDesignPanel()
        {
            HideAllPanels();
            SetActive(_designPanel, true);
            SetText(_instructionText, "Drag breakers from the tray to the panel slots. Assign each breaker to a required circuit.");
        }

        public void UpdateBreakerCount(int placed, int required)
        {
            SetText(_breakerCountText, $"Circuits: {placed} / {required}");
        }

        public void UpdateLoadDisplay(float currentVA, float targetVA)
        {
            SetText(_loadDisplayText, $"Load: {currentVA:N0} VA / {targetVA:N0} VA");
        }

        #endregion

        #region Wiring

        public void ShowWiringPanel()
        {
            HideAllPanels();
            SetActive(_wiringPanel, true);
            SetText(_wiringInstructionText, "Connect wires between each breaker and its load point. Select the correct wire gauge for each circuit.");
        }

        #endregion

        #region Compliance

        public void ShowCompliancePanel(List<ComplianceResult> results)
        {
            HideAllPanels();
            SetActive(_compliancePanel, true);

            int passed = 0, failed = 0;
            var lines = new System.Text.StringBuilder();

            foreach (var result in results)
            {
                string icon = result.passed ? "[PASS]" : "[FAIL]";
                lines.AppendLine($"{icon} {result.ruleName} (Art. {result.necReference})");
                lines.AppendLine($"    {result.message}");
                lines.AppendLine();

                if (result.passed) passed++;
                else failed++;
            }

            SetText(_complianceTitle, $"Compliance Check: {passed} passed, {failed} failed");
            SetText(_complianceResultsText, lines.ToString());
        }

        #endregion

        #region Score

        public void ShowScorePanel(Core.SandboxScore score, float elapsedTime)
        {
            HideAllPanels();
            SetActive(_scorePanel, true);

            // Calculate letter grade from compliance rate and load accuracy
            float combined = (score.ComplianceRate + score.loadCalcAccuracy) / 2f;
            string grade;
            if (combined >= 0.9f) grade = "A";
            else if (combined >= 0.8f) grade = "B";
            else if (combined >= 0.7f) grade = "C";
            else if (combined >= 0.6f) grade = "D";
            else grade = "F";

            SetText(_gradeText, grade);
            SetText(_complianceRateText, $"Compliance: {score.ComplianceRate:P0} ({score.totalChecks - score.complianceErrors}/{score.totalChecks} rules passed)");
            SetText(_loadAccuracyText, $"Load Accuracy: {score.loadCalcAccuracy:P0}");

            int minutes = (int)(elapsedTime / 60f);
            int seconds = (int)(elapsedTime % 60f);
            SetText(_timeText, $"Time: {minutes}:{seconds:D2}");
        }

        #endregion

        #region Button Handlers (called by UI buttons)

        public void OnContinueButton() => OnContinuePressed?.Invoke();
        public void OnFinishButton() => OnFinishPressed?.Invoke();
        public void OnRunCheckButton() => OnRunCheckPressed?.Invoke();
        public void OnRetryButton() => OnRetryPressed?.Invoke();
        public void OnMenuButton() => OnMenuPressed?.Invoke();

        #endregion

        #region Helpers

        private void SetActive(GameObject go, bool active)
        {
            if (go != null) go.SetActive(active);
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text;
        }

        #endregion
    }
}
