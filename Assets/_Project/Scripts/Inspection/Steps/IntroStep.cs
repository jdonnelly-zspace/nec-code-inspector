using System.Collections;
using UnityEngine;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Displays the scenario introduction: description, objectives, and a continue prompt.
    /// </summary>
    public class IntroStep : InspectionStateMachine.Step
    {
        private readonly InspectionManager _inspectionManager;
        private readonly InspectionHUD _hud;
        private System.Action _continueHandler;

        public IntroStep(InspectionStateMachine.StateID id, InspectionStateMachine stateMachine,
            InspectionStateMachine.StateID nextStepID, InspectionManager inspectionManager, InspectionHUD hud)
            : base(id, stateMachine, nextStepID)
        {
            _inspectionManager = inspectionManager;
            _hud = hud;
        }

        protected override IEnumerator OnStepEntered(InspectionStateMachine.Transition transition,
            InspectionStateMachine.StateID fromStateId)
        {
            var scenario = _inspectionManager.ScenarioDefinition;

            if (_hud != null)
            {
                _hud.ShowIntroPanel(
                    scenario.displayName,
                    scenario.description,
                    $"Violations to find: {_inspectionManager.TotalActiveViolations}",
                    scenario.necChapters
                );
            }

            Debug.Log($"[IntroStep] Scenario: {scenario.displayName}");
            Debug.Log($"[IntroStep] Active violations: {_inspectionManager.TotalActiveViolations}");

            // Wait for user to press continue
            bool continuePressed = false;
            if (_hud != null)
            {
                _continueHandler = () => continuePressed = true;
                _hud.OnContinuePressed += _continueHandler;
                yield return new WaitUntil(() => continuePressed);
                _hud.OnContinuePressed -= _continueHandler;
                _continueHandler = null;
                _hud.HideIntroPanel();
            }
            else
            {
                // Auto-advance after 3 seconds if no HUD
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
