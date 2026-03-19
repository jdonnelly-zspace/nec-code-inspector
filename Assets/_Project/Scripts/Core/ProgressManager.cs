using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace NECInspector.Core
{
    /// <summary>
    /// Persists student progress across sessions as JSON in Application.persistentDataPath.
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

        public void RecordInspectionScore(string scenarioId, DifficultyLevel difficulty, InspectionScore score)
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
            Save();
        }

        public void RecordSandboxScore(string panelType, SandboxScore score)
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
            Save();
        }
    }

    [Serializable]
    public class ProgressData
    {
        public string studentName = "";
        public List<ScenarioProgress> completedScenarios = new List<ScenarioProgress>();
        public List<SandboxProgress> completedSandboxes = new List<SandboxProgress>();
        public List<string> masteredChapters = new List<string>();
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
