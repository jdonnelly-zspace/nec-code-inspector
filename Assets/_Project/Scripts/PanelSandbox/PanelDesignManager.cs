using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NECInspector.Core;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// Central manager for the Panel Design Sandbox mode.
    /// Tracks placed breakers, available breakers, compliance state, and scoring.
    /// </summary>
    public class PanelDesignManager : MonoBehaviour
    {
        public static PanelDesignManager Instance { get; private set; }

        [Header("Definition")]
        [SerializeField] private PanelDesignDefinitionSO _definition;

        [Header("Panel")]
        [SerializeField] private BreakerSlot[] _slots;

        private List<PlacedBreaker> _placedBreakers = new();
        private ComplianceChecker _complianceChecker = new();
        private List<ComplianceResult> _lastComplianceResults;
        private float _startTime;

        public PanelDesignDefinitionSO Definition => _definition;
        public BreakerSlot[] Slots => _slots;
        public IReadOnlyList<PlacedBreaker> PlacedBreakers => _placedBreakers;
        public IReadOnlyList<ComplianceResult> LastComplianceResults => _lastComplianceResults;
        public float ElapsedTime => Time.time - _startTime;

        public event Action<PlacedBreaker, BreakerSlot> OnBreakerPlaced;
        public event Action<PlacedBreaker, BreakerSlot> OnBreakerRemoved;
        public event Action<List<ComplianceResult>> OnComplianceCheckComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void Initialize()
        {
            _startTime = Time.time;
            _placedBreakers.Clear();
            _lastComplianceResults = null;

            // Subscribe to slot events
            foreach (var slot in _slots)
            {
                slot.OnBreakerPlaced += HandleBreakerPlaced;
                slot.OnBreakerRemoved += HandleBreakerRemoved;
            }

            Debug.Log($"[PanelDesignManager] Initialized: {_definition.displayName}, {_slots.Length} slots");
        }

        /// <summary>
        /// Get a specific slot by index and bus side.
        /// </summary>
        public BreakerSlot GetSlot(int index, BusSide side)
        {
            return _slots.FirstOrDefault(s => s.SlotIndex == index && s.BusSide == side);
        }

        /// <summary>
        /// Get all occupied slots on a given bus side.
        /// </summary>
        public List<BreakerSlot> GetOccupiedSlots(BusSide side)
        {
            return _slots.Where(s => s.BusSide == side && s.IsOccupied).ToList();
        }

        /// <summary>
        /// Run all 10 compliance checks and fire event.
        /// </summary>
        public List<ComplianceResult> RunComplianceCheck()
        {
            _lastComplianceResults = _complianceChecker.RunAllChecks(_definition, _slots, _placedBreakers);
            OnComplianceCheckComplete?.Invoke(_lastComplianceResults);
            return _lastComplianceResults;
        }

        /// <summary>
        /// Calculate the sandbox score based on compliance results.
        /// </summary>
        public SandboxScore CalculateScore()
        {
            if (_lastComplianceResults == null)
                RunComplianceCheck();

            int errors = _lastComplianceResults.Count(r => !r.passed);
            int total = _lastComplianceResults.Count;

            // Check required circuits
            bool allRequired = _definition.requiredCircuits
                .Where(r => r.isRequired)
                .All(req => _placedBreakers.Any(b => b.AssignedCircuitName == req.circuitName));

            // Load calculation accuracy
            float actualLoad = CalculateTotalPlacedLoad();
            float targetLoad = _definition.targetLoadVA;
            float loadAccuracy = targetLoad > 0f
                ? 1f - Math.Abs(actualLoad - targetLoad) / targetLoad
                : 1f;
            loadAccuracy = Math.Max(0f, loadAccuracy);

            return new SandboxScore
            {
                complianceErrors = errors,
                totalChecks = total,
                loadCalcAccuracy = loadAccuracy,
                allRequiredCircuitsPresent = allRequired
            };
        }

        /// <summary>
        /// Get the total placed load in VA.
        /// </summary>
        public float CalculateTotalPlacedLoad()
        {
            float total = 0f;
            foreach (var breaker in _placedBreakers)
                total += breaker.GetLoadVA();
            return total;
        }

        /// <summary>
        /// Get the count of placed breakers vs required circuits.
        /// </summary>
        public (int placed, int required) GetCircuitCounts()
        {
            int required = _definition.requiredCircuits.Count(r => r.isRequired);
            int placed = _placedBreakers.Count(b => !string.IsNullOrEmpty(b.AssignedCircuitName));
            return (placed, required);
        }

        private void HandleBreakerPlaced(BreakerSlot slot, PlacedBreaker breaker)
        {
            if (!_placedBreakers.Contains(breaker))
                _placedBreakers.Add(breaker);
            OnBreakerPlaced?.Invoke(breaker, slot);
        }

        private void HandleBreakerRemoved(BreakerSlot slot)
        {
            // Find and remove the breaker that was in this slot
            var breaker = _placedBreakers.FirstOrDefault(b => b.CurrentSlot == null);
            if (breaker != null)
            {
                _placedBreakers.Remove(breaker);
                OnBreakerRemoved?.Invoke(breaker, slot);
            }
        }

        private void OnDestroy()
        {
            if (_slots != null)
            {
                foreach (var slot in _slots)
                {
                    slot.OnBreakerPlaced -= HandleBreakerPlaced;
                    slot.OnBreakerRemoved -= HandleBreakerRemoved;
                }
            }

            if (Instance == this) Instance = null;
        }
    }
}
