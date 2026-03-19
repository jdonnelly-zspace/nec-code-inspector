using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Tracks all violations and student actions during an inspection scenario.
    /// Lives on the scenario scene root and is accessed by the InspectionScenarioRunner.
    /// </summary>
    public class InspectionManager : MonoBehaviour
    {
        public static InspectionManager Instance { get; private set; }

        [Header("Scenario")]
        [SerializeField] private ScenarioDefinitionSO _scenarioDefinition;

        private List<ViolationDefinitionSO> _activeViolations = new();
        private List<FlaggedViolation> _flaggedViolations = new();
        private List<string> _markedCompliant = new();
        private Dictionary<string, InspectableComponent> _componentMap = new();
        private float _startTime;
        private bool _isActive = false;

        public ScenarioDefinitionSO ScenarioDefinition => _scenarioDefinition;
        public bool IsActive => _isActive;
        public int FlaggedCount => _flaggedViolations.Count;
        public int TotalActiveViolations => _activeViolations.Count;
        public float ElapsedTime => _isActive ? Time.time - _startTime : 0f;

        public event Action<FlaggedViolation> OnViolationFlagged;
        public event Action<string> OnComponentMarkedCompliant;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
                return;
            }
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        /// <summary>
        /// Initialize the inspection with the current difficulty level.
        /// Filters violations based on difficulty.
        /// </summary>
        public void Initialize(DifficultyLevel difficulty)
        {
            if (_scenarioDefinition == null)
            {
                Debug.LogError("[InspectionManager] No scenario definition assigned!");
                return;
            }

            _activeViolations.Clear();
            _flaggedViolations.Clear();
            _markedCompliant.Clear();

            // Filter violations by difficulty
            foreach (var violation in _scenarioDefinition.violations)
            {
                if (violation == null) continue;
                if ((int)violation.minimumDifficulty <= (int)difficulty)
                {
                    // Skip subtle violations unless Expert
                    if (violation.isSubtle && difficulty != DifficultyLevel.Expert)
                        continue;

                    _activeViolations.Add(violation);
                }
            }

            // Build component map from scene
            _componentMap.Clear();
            var components = FindObjectsByType<InspectableComponent>(FindObjectsSortMode.None);
            foreach (var comp in components)
            {
                _componentMap[comp.gameObject.name] = comp;
            }

            Debug.Log($"[InspectionManager] Initialized: {_activeViolations.Count} violations, {_componentMap.Count} components, difficulty={difficulty}");
        }

        public void StartInspection()
        {
            _startTime = Time.time;
            _isActive = true;
            Debug.Log("[InspectionManager] Inspection started");
        }

        public void StopInspection()
        {
            _isActive = false;
            Debug.Log($"[InspectionManager] Inspection stopped. Time: {ElapsedTime:F1}s");
        }

        /// <summary>
        /// Student flags a violation on a component
        /// </summary>
        public void FlagViolation(InspectableComponent component, string violationDescription, string necArticle)
        {
            var flagged = new FlaggedViolation
            {
                componentName = component.gameObject.name,
                componentDisplayName = component.componentName,
                description = violationDescription,
                citedNECArticle = necArticle,
                timeStamp = ElapsedTime
            };

            _flaggedViolations.Add(flagged);
            component.FlagViolation(flagged.description, necArticle);
            OnViolationFlagged?.Invoke(flagged);

            Debug.Log($"[InspectionManager] Violation flagged on {component.componentName}: {necArticle}");
        }

        /// <summary>
        /// Student marks a component as compliant
        /// </summary>
        public void MarkCompliant(InspectableComponent component)
        {
            _markedCompliant.Add(component.gameObject.name);
            component.MarkCompliant();
            OnComponentMarkedCompliant?.Invoke(component.gameObject.name);
        }

        /// <summary>
        /// Generate the final inspection score
        /// </summary>
        public InspectionScore CalculateScore()
        {
            int correctFlags = 0;
            int correctCitations = 0;
            int falsePositives = 0;

            foreach (var flagged in _flaggedViolations)
            {
                var matchingViolation = _activeViolations.FirstOrDefault(v =>
                    v.componentObjectName == flagged.componentName);

                if (matchingViolation != null)
                {
                    correctFlags++;

                    // Check if NEC citation is correct
                    if (IsNECCitationCorrect(flagged.citedNECArticle, matchingViolation.necArticle))
                        correctCitations++;
                }
                else
                {
                    falsePositives++;
                }
            }

            return new InspectionScore
            {
                violationsFound = correctFlags,
                totalViolations = _activeViolations.Count,
                falsePositives = falsePositives,
                correctCitations = correctCitations,
                totalCitations = correctFlags, // Only count correct flags for citation scoring
                timeElapsed = ElapsedTime
            };
        }

        /// <summary>
        /// Get violations that were not found by the student
        /// </summary>
        public List<ViolationDefinitionSO> GetMissedViolations()
        {
            var flaggedComponentNames = _flaggedViolations.Select(f => f.componentName).ToHashSet();
            return _activeViolations.Where(v => !flaggedComponentNames.Contains(v.componentObjectName)).ToList();
        }

        /// <summary>
        /// Get the InspectableComponent for a violation's target object
        /// </summary>
        public InspectableComponent GetComponentForViolation(ViolationDefinitionSO violation)
        {
            return _componentMap.TryGetValue(violation.componentObjectName, out var comp) ? comp : null;
        }

        /// <summary>
        /// Show hint highlights for Beginner mode (pulses on violation areas)
        /// </summary>
        public void ShowHints()
        {
            foreach (var violation in _activeViolations)
            {
                if (_componentMap.TryGetValue(violation.componentObjectName, out var comp))
                {
                    if (!comp.HasBeenFlagged)
                        comp.ShowHintPulse();
                }
            }
        }

        private bool IsNECCitationCorrect(string cited, string actual)
        {
            if (string.IsNullOrEmpty(cited) || string.IsNullOrEmpty(actual))
                return false;

            string normalizedCited = cited.Replace(" ", "").Replace("Art.", "").Replace("art.", "").Trim();
            string normalizedActual = actual.Replace(" ", "").Replace("Art.", "").Replace("art.", "").Trim();

            // Exact match
            if (normalizedCited == normalizedActual) return true;

            // Partial match (student cites parent article, actual is subsection)
            if (normalizedActual.StartsWith(normalizedCited)) return true;

            return false;
        }
    }

    [Serializable]
    public class FlaggedViolation
    {
        public string componentName;
        public string componentDisplayName;
        public string description;
        public string citedNECArticle;
        public float timeStamp;
    }
}
