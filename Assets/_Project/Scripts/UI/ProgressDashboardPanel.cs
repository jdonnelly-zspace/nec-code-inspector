using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;
using NECInspector.Core;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space dashboard showing student progress across all scenarios and sandbox.
    /// Displays score history, chapter mastery, and earned certificates.
    /// </summary>
    public class ProgressDashboardPanel : MonoBehaviour
    {
        [Header("Student Info")]
        [SerializeField] private TextMeshProUGUI _studentNameText;
        [SerializeField] private TextMeshProUGUI _overallStatsText;

        [Header("Scenario Scores")]
        [SerializeField] private Transform _scenarioScoresContent;
        [SerializeField] private GameObject _scoreEntryPrefab;

        [Header("Sandbox Scores")]
        [SerializeField] private Transform _sandboxScoresContent;

        [Header("Chapter Mastery")]
        [SerializeField] private Transform _masteryContent;
        [SerializeField] private GameObject _masteryEntryPrefab;

        [Header("Certificates")]
        [SerializeField] private Transform _certificatesContent;
        [SerializeField] private GameObject _certificateEntryPrefab;

        [Header("Controls")]
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Hide);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Refresh()
        {
            var progress = GameManager.Instance?.Progress;
            if (progress == null) return;

            var data = progress.Data;

            // Student info
            SetText(_studentNameText, string.IsNullOrEmpty(data.studentName) ? "Student" : data.studentName);

            // Overall stats
            int totalAttempts = data.completedScenarios.Count + data.completedSandboxes.Count;
            float avgAccuracy = CalculateOverallAccuracy(data);
            SetText(_overallStatsText,
                $"Total Attempts: {totalAttempts}\n" +
                $"Overall Accuracy: {avgAccuracy:P0}\n" +
                $"Chapters Mastered: {data.masteredChapters.Count}\n" +
                $"Certificates Earned: {data.earnedCertificates.Count}");

            // Scenario scores (best per scenario)
            PopulateScenarioScores(data.completedScenarios);

            // Sandbox scores
            PopulateSandboxScores(data.completedSandboxes);

            // Chapter mastery
            PopulateMastery(data.masteredChapters);

            // Certificates
            PopulateCertificates(data.earnedCertificates);
        }

        private void PopulateScenarioScores(List<ScenarioProgress> scenarios)
        {
            ClearContent(_scenarioScoresContent);
            if (_scoreEntryPrefab == null || _scenarioScoresContent == null) return;

            // Group by scenario, show best attempt
            var grouped = scenarios.GroupBy(s => s.scenarioId);
            foreach (var group in grouped)
            {
                var best = group.OrderByDescending(s =>
                    s.totalViolations > 0 ? (float)s.violationsFound / s.totalViolations : 0f)
                    .First();

                float accuracy = best.totalViolations > 0
                    ? (float)best.violationsFound / best.totalViolations
                    : 0f;
                float citation = best.totalCitations > 0
                    ? (float)best.correctCitations / best.totalCitations
                    : 0f;

                string grade = GetLetterGrade((accuracy + citation) / 2f);
                int minutes = (int)(best.timeElapsed / 60f);
                int seconds = (int)(best.timeElapsed % 60f);

                var item = Instantiate(_scoreEntryPrefab, _scenarioScoresContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = $"{best.scenarioId} [{best.difficulty}]\n" +
                                $"  Grade: {grade}  |  Detection: {accuracy:P0}  |  Citations: {citation:P0}  |  Time: {minutes}:{seconds:D2}\n" +
                                $"  Attempts: {group.Count()}";
                }
            }
        }

        private void PopulateSandboxScores(List<SandboxProgress> sandboxes)
        {
            ClearContent(_sandboxScoresContent);
            if (_scoreEntryPrefab == null || _sandboxScoresContent == null) return;

            var grouped = sandboxes.GroupBy(s => s.panelType);
            foreach (var group in grouped)
            {
                var best = group.OrderByDescending(s =>
                    s.totalChecks > 0 ? 1f - (float)s.complianceErrors / s.totalChecks : 0f)
                    .First();

                float compliance = best.totalChecks > 0
                    ? 1f - (float)best.complianceErrors / best.totalChecks
                    : 0f;

                var item = Instantiate(_scoreEntryPrefab, _sandboxScoresContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    text.text = $"{best.panelType}\n" +
                                $"  Compliance: {compliance:P0}  |  Load Accuracy: {best.loadCalcAccuracy:P0}  |  Required Circuits: {(best.allRequiredCircuits ? "Yes" : "No")}\n" +
                                $"  Attempts: {group.Count()}";
                }
            }
        }

        private void PopulateMastery(List<string> chapters)
        {
            ClearContent(_masteryContent);
            if (_masteryEntryPrefab == null || _masteryContent == null) return;

            if (chapters.Count == 0)
            {
                var item = Instantiate(_masteryEntryPrefab, _masteryContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = "No chapters mastered yet. Complete scenarios with 80%+ accuracy.";
                return;
            }

            foreach (var chapter in chapters)
            {
                var item = Instantiate(_masteryEntryPrefab, _masteryContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = $"Chapter {chapter} - Mastered";
            }
        }

        private void PopulateCertificates(List<EarnedCertificate> certificates)
        {
            ClearContent(_certificatesContent);
            if (_certificateEntryPrefab == null || _certificatesContent == null) return;

            if (certificates.Count == 0)
            {
                var item = Instantiate(_certificateEntryPrefab, _certificatesContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = "No certificates earned yet. Keep practicing!";
                return;
            }

            foreach (var cert in certificates)
            {
                var item = Instantiate(_certificateEntryPrefab, _certificatesContent);
                var text = item.GetComponentInChildren<TMP_Text>();
                if (text != null)
                {
                    string date = "";
                    if (System.DateTime.TryParse(cert.earnedDate, out var dt))
                        date = dt.ToString("MMM d, yyyy");
                    text.text = $"{cert.certificateTitle}\n  Earned: {date}  |  Accuracy: {cert.accuracy:P0}  |  {cert.difficulty}";
                }
            }
        }

        private float CalculateOverallAccuracy(ProgressData data)
        {
            var accuracies = new List<float>();

            foreach (var s in data.completedScenarios)
            {
                float det = s.totalViolations > 0 ? (float)s.violationsFound / s.totalViolations : 0f;
                float cit = s.totalCitations > 0 ? (float)s.correctCitations / s.totalCitations : 0f;
                accuracies.Add((det + cit) / 2f);
            }

            foreach (var sb in data.completedSandboxes)
            {
                float comp = sb.totalChecks > 0 ? 1f - (float)sb.complianceErrors / sb.totalChecks : 0f;
                accuracies.Add(comp);
            }

            return accuracies.Count > 0 ? accuracies.Average() : 0f;
        }

        private string GetLetterGrade(float combined)
        {
            if (combined >= 0.9f) return "A";
            if (combined >= 0.8f) return "B";
            if (combined >= 0.7f) return "C";
            if (combined >= 0.6f) return "D";
            return "F";
        }

        private void ClearContent(Transform content)
        {
            if (content == null) return;
            foreach (Transform child in content)
                Destroy(child.gameObject);
        }

        private void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
