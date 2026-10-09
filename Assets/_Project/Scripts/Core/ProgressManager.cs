using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using NECInspector.Skills;

namespace NECInspector.Core
{
    /// <summary>
    /// Persists student progress across sessions as JSON in Application.persistentDataPath.
    /// Progress is recorded per skill (Data.skills). The app does not contain credentials; how the
    /// skills line up with a credential is worked out outside the app.
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
        /// The installation code the student studies under (empty = the default). Skill progress
        /// is not tied to it: the same skills count whichever code the student practiced with.
        /// </summary>
        public string GetActiveCodeProfileId()
        {
            return string.IsNullOrEmpty(Data.activeCodeProfileId) ? Codes.CodeProfileLibrary.DefaultProfileId : Data.activeCodeProfileId;
        }

        private const int MAX_RECENT_VIOLATIONS = 60;

        /// <summary>Ids of the violations the student saw lately, oldest first. A session prefers violations not in this list.</summary>
        public IReadOnlyList<string> GetRecentViolationIds() => Data.recentViolationIds;

        /// <summary>Remember the violations a session showed, keeping the most recent ones.</summary>
        public void RememberViolations(IEnumerable<string> violationIds)
        {
            if (violationIds == null) return;

            foreach (string id in violationIds)
            {
                if (string.IsNullOrEmpty(id)) continue;
                Data.recentViolationIds.Remove(id);
                Data.recentViolationIds.Add(id);
            }

            while (Data.recentViolationIds.Count > MAX_RECENT_VIOLATIONS)
                Data.recentViolationIds.RemoveAt(0);

            Save();
        }

        /// <summary>The region the student chose (empty until they choose one).</summary>
        public string GetRegion() => Data.region ?? "";

        /// <summary>
        /// Save the region the student chose and apply the installation code for it. The code is never shown
        /// to the student. Returns false (and changes nothing) if no loaded code belongs to the region.
        /// </summary>
        public bool SetRegion(string region)
        {
            var profile = Codes.RegionChoices.ProfileForRegion(Codes.CodeProfileLibrary.All, region);
            if (profile == null || !Codes.CodeProfileLibrary.Activate(profile.ProfileId)) return false;

            Data.region = Codes.RegionChoices.Normalize(region);
            Data.activeCodeProfileId = profile.ProfileId;
            Save();
            return true;
        }

        public void SetActiveCodeProfile(string profileId)
        {
            if (!Codes.CodeProfileLibrary.Activate(profileId)) return;

            Data.activeCodeProfileId = profileId;
            Save();
        }

        /// <summary>Activate the student's code profile (called at startup); falls back to the default.</summary>
        public void ApplyActiveCodeProfile()
        {
            // The student's region decides the code; a saved code id from before regions existed still works
            var byRegion = Codes.RegionChoices.ProfileForRegion(Codes.CodeProfileLibrary.All, Data.region);
            if (byRegion != null && Codes.CodeProfileLibrary.Activate(byRegion.ProfileId)) return;

            string id = GetActiveCodeProfileId();
            if (!Codes.CodeProfileLibrary.Activate(id) && id != Codes.CodeProfileLibrary.DefaultProfileId)
                Codes.CodeProfileLibrary.Activate(Codes.CodeProfileLibrary.DefaultProfileId);
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
        public List<string> recentViolationIds = new List<string>();   // violations shown in recent sessions, oldest first
        public string region = "";                  // the region the student chose (for example US); decides the installation code
        public string activeCodeProfileId = "";
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
