using System.Collections;
using UnityEngine;
using NECInspector.Core;
using NECInspector.StateMachine;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// Orchestrates the Panel Design Sandbox 5-step flow.
    /// Extends InspectionStateMachine for consistent step sequencing.
    /// </summary>
    [RequireComponent(typeof(PanelDesignManager))]
    public class PanelDesignRunner : InspectionStateMachine
    {
        [Header("UI References")]
        [SerializeField] private PanelDesignHUD _hud;

        private PanelDesignManager _manager;

        // State IDs
        private readonly StateID _briefingID = new("Briefing");
        private readonly StateID _placementID = new("Placement");
        private readonly StateID _wiringID = new("WireRouting");
        private readonly StateID _complianceID = new("ComplianceCheck");
        private readonly StateID _scoringID = new("Scoring");

        private void Start()
        {
            _manager = GetComponent<PanelDesignManager>();
            _manager.Initialize();
            BuildSteps();
            StartMachine(_briefingID);
        }

        private void BuildSteps()
        {
            // Step 1: Briefing - show requirements
            var briefing = new BriefingStep(_briefingID, this, _placementID, _manager, _hud);

            // Step 2: Placement - drag breakers to slots
            var placement = new PlacementStep(_placementID, this, _wiringID, _manager, _hud);

            // Step 3: Wire routing - connect wires
            var wiring = new WireRoutingStep(_wiringID, this, _complianceID, _manager, _hud);

            // Step 4: Compliance check - run rules
            var compliance = new ComplianceCheckStep(_complianceID, this, _scoringID, _manager, _hud);

            // Step 5: Scoring - display grade
            var scoring = new ScoringStep(_scoringID, this, null, _manager, _hud);
            scoring.IsLastStep = true;
        }

        protected override void OnMachineComplete()
        {
            var score = _manager.CalculateScore();
            GameManager.Instance.Progress.RecordSandboxScore(
                _manager.Definition.panelType,
                score
            );
            Debug.Log($"[PanelDesignRunner] Complete. Compliance: {score.ComplianceRate:P0}, Load accuracy: {score.loadCalcAccuracy:P0}");
        }
    }

    #region Step Implementations

    /// <summary>
    /// Step 1: Display panel requirements, required circuits, and NEC chapter references.
    /// </summary>
    public class BriefingStep : InspectionStateMachine.Step
    {
        private readonly PanelDesignManager _manager;
        private readonly PanelDesignHUD _hud;
        private bool _continued;

        public BriefingStep(StateID id, InspectionStateMachine sm, StateID nextID,
            PanelDesignManager manager, PanelDesignHUD hud)
            : base(id, sm, nextID)
        {
            _manager = manager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            _continued = false;
            _hud.ShowBriefingPanel(_manager.Definition);
            _hud.OnContinuePressed += HandleContinue;

            while (!_continued)
                yield return null;

            _hud.OnContinuePressed -= HandleContinue;
            _hud.HideAllPanels();
        }

        private void HandleContinue() => _continued = true;
    }

    /// <summary>
    /// Step 2: Student drags breakers from tray to panel slots.
    /// Waits until student presses "Finish Placement" or all required circuits are placed.
    /// </summary>
    public class PlacementStep : InspectionStateMachine.Step
    {
        private readonly PanelDesignManager _manager;
        private readonly PanelDesignHUD _hud;
        private bool _finished;

        public PlacementStep(StateID id, InspectionStateMachine sm, StateID nextID,
            PanelDesignManager manager, PanelDesignHUD hud)
            : base(id, sm, nextID)
        {
            _manager = manager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            _finished = false;
            _hud.ShowDesignPanel();
            _hud.OnFinishPressed += HandleFinish;
            _manager.OnBreakerPlaced += HandleBreakerUpdate;

            while (!_finished)
            {
                var counts = _manager.GetCircuitCounts();
                _hud.UpdateBreakerCount(counts.placed, counts.required);
                yield return null;
            }

            _manager.OnBreakerPlaced -= HandleBreakerUpdate;
            _hud.OnFinishPressed -= HandleFinish;
        }

        private void HandleFinish() => _finished = true;
        private void HandleBreakerUpdate(PlacedBreaker b, BreakerSlot s) { }
    }

    /// <summary>
    /// Step 3: Student connects wires between breakers and load points.
    /// </summary>
    public class WireRoutingStep : InspectionStateMachine.Step
    {
        private readonly PanelDesignManager _manager;
        private readonly PanelDesignHUD _hud;
        private bool _finished;

        public WireRoutingStep(StateID id, InspectionStateMachine sm, StateID nextID,
            PanelDesignManager manager, PanelDesignHUD hud)
            : base(id, sm, nextID)
        {
            _manager = manager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            _finished = false;
            _hud.ShowWiringPanel();
            _hud.OnFinishPressed += HandleFinish;

            while (!_finished)
                yield return null;

            _hud.OnFinishPressed -= HandleFinish;
        }

        private void HandleFinish() => _finished = true;
    }

    /// <summary>
    /// Step 4: Run compliance checker and display results per rule.
    /// </summary>
    public class ComplianceCheckStep : InspectionStateMachine.Step
    {
        private readonly PanelDesignManager _manager;
        private readonly PanelDesignHUD _hud;
        private bool _continued;

        public ComplianceCheckStep(StateID id, InspectionStateMachine sm, StateID nextID,
            PanelDesignManager manager, PanelDesignHUD hud)
            : base(id, sm, nextID)
        {
            _manager = manager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            _continued = false;
            var results = _manager.RunComplianceCheck();
            _hud.ShowCompliancePanel(results);
            _hud.OnContinuePressed += HandleContinue;

            while (!_continued)
                yield return null;

            _hud.OnContinuePressed -= HandleContinue;
        }

        private void HandleContinue() => _continued = true;
    }

    /// <summary>
    /// Step 5: Display final score, compliance rate, and load accuracy.
    /// Offer retry or return to menu.
    /// </summary>
    public class ScoringStep : InspectionStateMachine.Step
    {
        private readonly PanelDesignManager _manager;
        private readonly PanelDesignHUD _hud;
        private bool _continued;

        public ScoringStep(StateID id, InspectionStateMachine sm, StateID nextID,
            PanelDesignManager manager, PanelDesignHUD hud)
            : base(id, sm, nextID)
        {
            _manager = manager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            _continued = false;
            var score = _manager.CalculateScore();
            _hud.ShowScorePanel(score, _manager.ElapsedTime);
            _hud.OnContinuePressed += HandleContinue;

            while (!_continued)
                yield return null;

            _hud.OnContinuePressed -= HandleContinue;
        }

        private void HandleContinue() => _continued = true;
    }

    #endregion
}
