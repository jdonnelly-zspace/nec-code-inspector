using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NECInspector.Credentials;
using NECInspector.Skills;

namespace NECInspector.Core
{
    /// <summary>
    /// Persists student progress across sessions as JSON in Application.persistentDataPath.
    /// Progress is recorded per skill (Data.skills), never per credential; a credential is a
    /// view over the skills (see GetReadiness).
    /// </summary>
    public class ProgressManager
    {
        private const string SAVE_FILE = "nec_progress.json";

        public ProgressData Data { get; private set; } = new ProgressData();

        private string SavePath => Path.Combine(Application.persistentDataPath, SAVE_FILE);

        public void Load()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    Data = JsonUtility.FromJson<ProgressData>(json) ?? new ProgressData();
                    Debug.Log($"[ProgressManager] Loaded progress from {SavePath}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[ProgressManager] Failed to load progress: {e.Message}");
                    Data = new ProgressData();
                }
            }
            else
            {
                Data = new ProgressData();
                Debug.Log("[ProgressManager] No existing progress file, starting fresh");
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[ProgressManager] Saved progress to {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[ProgressManager] Failed to save progress: {e.Message}");
            }
        }

        public void RecordInspectionScore(string scenarioId, DifficultyLevel difficulty, InspectionScore score,
            IEnumerable<SkillEvidence> evidence = null)
        {
            var entry = new ScenarioProgress
            {
                scenarioId = scenarioId,
                difficulty = difficulty.ToString(),
                violationsFound = score.violationsFound,
                totalViolations = score.totalViolations,
                falsePositives = score.falsePositives,
                correctCitations = score.correctCitations,
                totalCitations = score.totalCitations,
                timeElapsed = score.timeElapsed,
                completedAt = DateTime.UtcNow.ToString("o")
            };

            Data.completedScenarios.Add(entry);
            Data.skills.Record(evidence, DateTime.UtcNow.ToString("o"));
            Save();
        }

        /// <summary>
        /// The credential the student works toward (empty = the default). Choosing one also selects
        /// the installation code it is based on. Skill progress itself is not tied to it.
        /// </summary>
        public CredentialProfile GetActiveCredential()
        {
            return CredentialLibrary.Get(Data.activeCredentialId) ?? CredentialLibrary.Default;
        }

        public void SetActiveCredential(CredentialProfile credential)
        {
            if (credential == null) return;

            Data.activeCredentialId = credential.id;
            Save();
            ApplyActiveCredential();
        }

        /// <summary>Activate the code profile of the active credential (called at startup and after a change).</summary>
        public void ApplyActiveCredential()
        {
            var credential = GetActiveCredential();
            if (credential != null)
                Codes.CodeProfileLibrary.Activate(credential.codeProfileId);
        }

        /// <summary>
        /// How close the student's skills are to a credential's requirements.
        /// </summary>
        public ReadinessReport GetReadiness(CredentialProfile credential)
        {
            return CredentialReadiness.Evaluate(credential, Data.skills);
        }

        /// <summary>
        /// Get the best attempt for a given scenario (highest accuracy).
        /// </summary>
        public ScenarioProgress GetBestScenarioAttempt(string scenarioId)
        {
            ScenarioProgress best = null;
            float bestAccuracy = -1f;

            foreach (var entry in Data.completedScenarios)
            {
                if (entry.scenarioId != scenarioId) continue;
                float acc = entry.totalViolations > 0
                    ? (float)entry.violationsFound / entry.totalViolations
                    : 0f;
                if (acc > bestAccuracy)
                {
                    bestAccuracy = acc;
                    best = entry;
                }
            }
            return best;
        }

        public void RecordSandboxScore(string panelType, SandboxScore score,
            IEnumerable<SkillEvidence> evidence = null)
        {
            var entry = new SandboxProgress
            {
                panelType = panelType,
                complianceErrors = score.complianceErrors,
                totalChecks = score.totalChecks,
                loadCalcAccuracy = score.loadCalcAccuracy,
                allRequiredCircuits = score.allRequiredCircuitsPresent,
                completedAt = DateTime.UtcNow.ToString("o")
            };

            Data.completedSandboxes.Add(entry);
            Data.skills.Record(evidence, DateTime.UtcNow.ToString("o"));
            Save();
        }
    }

    [Serializable]
    public class ProgressData
    {
        public string studentName = "";
        public List<ScenarioProgress> completedScenarios = new List<ScenarioProgress>();
        public List<SandboxProgress> completedSandboxes = new List<SandboxProgress>();
        public SkillProgress skills = new SkillProgress();
        public string activeCredentialId = "";
        public List<EarnedCertificate> earnedCertificates = new List<EarnedCertificate>();
        public float totalTimeSpent = 0f;
    }

    [Serializable]
    public class ScenarioProgress
    {
        public string scenarioId;
        public string difficulty;
        public int violationsFound;
        public int totalViolations;
        public int falsePositives;
        public int correctCitations;
        public int totalCitations;
        public float timeElapsed;
        public string completedAt;
    }

    [Serializable]
    public class SandboxProgress
    {
        public string panelType;
        public int complianceErrors;
        public int totalChecks;
        public float loadCalcAccuracy;
        public bool allRequiredCircuits;
        public string completedAt;
    }

    [Serializable]
    public class InspectionScore
    {
        public int violationsFound;
        public int totalViolations;
        public int falsePositives;
        public int correctCitations;
        public int totalCitations;
        public float timeElapsed;

        public float Accuracy => totalViolations > 0 ? (float)violationsFound / totalViolations : 0f;
        public float CitationAccuracy => totalCitations > 0 ? (float)correctCitations / totalCitations : 0f;

        public string LetterGrade
        {
            get
            {
                float combined = (Accuracy + CitationAccuracy) / 2f;
                if (combined >= 0.9f) return "A";
                if (combined >= 0.8f) return "B";
                if (combined >= 0.7f) return "C";
                if (combined >= 0.6f) return "D";
                return "F";
            }
        }
    }

    [Serializable]
    public class SandboxScore
    {
        public int complianceErrors;
        public int totalChecks;
        public float loadCalcAccuracy;
        public bool allRequiredCircuitsPresent;

        public float ComplianceRate => totalChecks > 0 ? 1f - ((float)complianceErrors / totalChecks) : 0f;
    }
}
