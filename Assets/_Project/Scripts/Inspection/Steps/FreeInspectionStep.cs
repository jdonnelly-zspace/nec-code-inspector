using System.Collections;
using UnityEngine;
using NECInspector.Core;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Main inspection gameplay loop. The student freely examines the installation,
    /// hovers over components, inspects them, and flags violations.
    /// Exits when the student presses "Finish Inspection" or the time limit expires.
    /// </summary>
    public class FreeInspectionStep : InspectionStateMachine.Step
    {
        private readonly InspectionManager _inspectionManager;
        private readonly InspectionHUD _hud;
        private readonly ViolationFlaggingPanel _flaggingPanel;
        private readonly DifficultyLevel _difficulty;

        private bool _inspectionComplete = false;
        private float _hintCooldownTimer = 0f;
        private Coroutine _timeLimitCoroutine;

        public FreeInspectionStep(InspectionStateMachine.StateID id, InspectionStateMachine stateMachine,
            InspectionStateMachine.StateID nextStepID, InspectionManager inspectionManager,
            InspectionHUD hud, ViolationFlaggingPanel flaggingPanel, DifficultyLevel difficulty)
            : base(id, stateMachine, nextStepID)
        {
            _inspectionManager = inspectionManager;
            _hud = hud;
            _flaggingPanel = flaggingPanel;
            _difficulty = difficulty;
        }

        protected override IEnumerator OnStepEntered(InspectionStateMachine.Transition transition,
            InspectionStateMachine.StateID fromStateId)
        {
            _inspectionComplete = false;
            _inspectionManager.StartInspection();

            // Show HUD with inspection controls
            if (_hud != null)
            {
                _hud.ShowInspectionHUD(
                    _inspectionManager.TotalActiveViolations,
                    _inspectionManager.FlaggedCount
                );
                _hud.OnFinishPressed += OnFinishInspection;
                _hud.OnHintPressed += OnHintRequested;
            }

            // Subscribe to flagging events
            if (_flaggingPanel != null)
            {
                _flaggingPanel.OnViolationSubmitted += OnViolationSubmitted;
                _flaggingPanel.OnMarkedCompliant += OnMarkedCompliant;
            }

            // Start time limit for Expert mode
            var settings = GameManager.Instance?.Difficulty.CurrentSettings;
            if (settings != null && settings.enableTimeLimit && settings.timeLimitSeconds > 0)
            {
                _timeLimitCoroutine = StateMachine.StartCoroutine(TimeLimitCoroutine(settings.timeLimitSeconds));
            }

            // Beginner scaffolding: show hints after timeout
            if (_difficulty == DifficultyLevel.Beginner)
            {
                ScaffoldingTimeoutSeconds = settings?.scaffoldingTimeoutSeconds ?? 60f;
                ScaffoldingAction = () => _inspectionManager.ShowHints();
            }

            // Main loop - wait for inspection to complete
            yield return new WaitUntil(() => _inspectionComplete);

            _inspectionManager.StopInspection();

            // Cleanup
            if (_hud != null)
            {
                _hud.OnFinishPressed -= OnFinishInspection;
                _hud.OnHintPressed -= OnHintRequested;
            }
            if (_flaggingPanel != null)
            {
                _flaggingPanel.OnViolationSubmitted -= OnViolationSubmitted;
                _flaggingPanel.OnMarkedCompliant -= OnMarkedCompliant;
                _flaggingPanel.Hide();
            }

            if (_timeLimitCoroutine != null)
            {
                StateMachine.StopCoroutine(_timeLimitCoroutine);
                _timeLimitCoroutine = null;
            }
        }

        private void OnFinishInspection()
        {
            _inspectionComplete = true;
        }

        private void OnHintRequested()
        {
            if (_difficulty == DifficultyLevel.Beginner)
            {
                _inspectionManager.ShowHints();
            }
        }

        private void OnViolationSubmitted(InspectableComponent component, string description, string necArticle)
        {
            _inspectionManager.FlagViolation(component, description, necArticle);
            _hud?.UpdateFlaggedCount(_inspectionManager.FlaggedCount);
        }

        private void OnMarkedCompliant(InspectableComponent component)
        {
            _inspectionManager.MarkCompliant(component);
        }

        private IEnumerator TimeLimitCoroutine(int seconds)
        {
            float remaining = seconds;

            while (remaining > 0f && !_inspectionComplete)
            {
                remaining -= Time.deltaTime;
                _hud?.UpdateTimer(remaining);
                yield return null;
            }

            if (!_inspectionComplete)
            {
                Debug.Log("[FreeInspection] Time limit reached!");
                _hud?.ShowTimeLimitWarning();
                yield return new WaitForSeconds(2f);
                _inspectionComplete = true;
            }
        }

        public override IEnumerator OnExit(InspectionStateMachine.Transition transition,
            InspectionStateMachine.StateID toStateId)
        {
            CancelScaffolding();
            _hud?.HideInspectionHUD();
            yield return null;
        }
    }
}
