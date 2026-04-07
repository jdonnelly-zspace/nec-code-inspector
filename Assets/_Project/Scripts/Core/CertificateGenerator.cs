using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NECInspector.Data;

namespace NECInspector.Core
{
    [Serializable]
    public class EarnedCertificate
    {
        public string certificateId;
        public string certificateTitle;
        public string studentName;
        public string earnedDate;
        public float accuracy;
        public string difficulty;
    }

    /// <summary>
    /// Evaluates student progress against certificate requirements
    /// and generates earned certificates.
    /// </summary>
    public class CertificateGenerator
    {
        private readonly CertificateTemplateSO[] _templates;
        private readonly ProgressManager _progress;

        public CertificateGenerator(CertificateTemplateSO[] templates, ProgressManager progress)
        {
            _templates = templates;
            _progress = progress;
        }

        /// <summary>
        /// Check all templates and return certificates the student has earned
        /// but not yet received.
        /// </summary>
        public List<EarnedCertificate> EvaluateNewCertificates(string studentName)
        {
            var earned = new List<EarnedCertificate>();

            foreach (var template in _templates)
            {
                if (template == null) continue;

                // Skip if already earned
                if (_progress.Data.earnedCertificates.Any(c => c.certificateId == template.certificateId))
                    continue;

                if (MeetsRequirements(template, out float accuracy))
                {
                    var cert = new EarnedCertificate
                    {
                        certificateId = template.certificateId,
                        certificateTitle = template.certificateTitle,
                        studentName = studentName,
                        earnedDate = DateTime.UtcNow.ToString("o"),
                        accuracy = accuracy,
                        difficulty = GameManager.Instance?.Difficulty?.CurrentLevel.ToString() ?? "Standard"
                    };

                    earned.Add(cert);
                    _progress.Data.earnedCertificates.Add(cert);
                }
            }

            if (earned.Count > 0)
                _progress.Save();

            return earned;
        }

        /// <summary>
        /// Get all previously earned certificates.
        /// </summary>
        public List<EarnedCertificate> GetEarnedCertificates()
        {
            return _progress.Data.earnedCertificates;
        }

        /// <summary>
        /// Format a certificate's description by replacing template tokens.
        /// </summary>
        public string FormatDescription(CertificateTemplateSO template, EarnedCertificate cert)
        {
            return template.descriptionTemplate
                .Replace("{StudentName}", cert.studentName)
                .Replace("{Date}", DateTime.Parse(cert.earnedDate).ToString("MMMM d, yyyy"))
                .Replace("{Score}", $"{cert.accuracy:P0}")
                .Replace("{Difficulty}", cert.difficulty);
        }

        private bool MeetsRequirements(CertificateTemplateSO template, out float accuracy)
        {
            accuracy = 0f;
            var scenarios = _progress.Data.completedScenarios;
            var sandboxes = _progress.Data.completedSandboxes;

            // Check required scenarios
            if (template.requiredScenarios != null && template.requiredScenarios.Length > 0)
            {
                foreach (var reqId in template.requiredScenarios)
                {
                    var best = scenarios
                        .Where(s => s.scenarioId == reqId)
                        .OrderByDescending(s => GetScenarioAccuracy(s))
                        .FirstOrDefault();

                    if (best == null) return false;
                    if (GetScenarioAccuracy(best) < template.minimumAccuracy) return false;
                }
            }

            // Check sandbox requirement
            if (template.requiresSandbox && sandboxes.Count == 0)
                return false;

            // Check chapter mastery
            if (template.requiredChapters != null && template.requiredChapters.Length > 0)
            {
                foreach (var chapter in template.requiredChapters)
                {
                    if (!_progress.Data.masteredChapters.Contains(chapter))
                        return false;
                }
            }

            // Calculate overall accuracy from best attempts
            var accuracies = new List<float>();
            foreach (var s in scenarios)
                accuracies.Add(GetScenarioAccuracy(s));
            foreach (var sb in sandboxes)
                accuracies.Add(sb.totalChecks > 0 ? 1f - (float)sb.complianceErrors / sb.totalChecks : 0f);

            accuracy = accuracies.Count > 0 ? accuracies.Average() : 0f;
            return accuracy >= template.minimumAccuracy;
        }

        private float GetScenarioAccuracy(ScenarioProgress s)
        {
            float detection = s.totalViolations > 0 ? (float)s.violationsFound / s.totalViolations : 0f;
            float citation = s.totalCitations > 0 ? (float)s.correctCitations / s.totalCitations : 0f;
            return (detection + citation) / 2f;
        }
    }
}
