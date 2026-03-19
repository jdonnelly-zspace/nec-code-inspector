using System.Collections;
using UnityEngine;
using NECInspector.Core;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Shows the results: violations found vs missed, false positives.
    /// </summary>
    public class ReviewStep : InspectionStateMachine.Step
    {
        private readonly InspectionManager _inspectionManager;
        private readonly InspectionReviewPanel _reviewPanel;

        public ReviewStep(InspectionStateMachine.StateID id, InspectionStateMachine stateMachine,
            InspectionStateMachine.StateID nextStepID, InspectionManager inspectionManager,
            InspectionReviewPanel reviewPanel)
            : base(id, stateMachine, nextStepID)
        {
            _inspectionManager = inspectionManager;
            _reviewPanel = reviewPanel;
        }

        protected override IEnumerator OnStepEntered(InspectionStateMachine.Transition transition,
            InspectionStateMachine.StateID fromStateId)
        {
            var score = _inspectionManager.CalculateScore();
            var missed = _inspectionManager.GetMissedViolations();

            Debug.Log($"[ReviewStep] Found: {score.violationsFound}/{score.totalViolations}, " +
                      $"False positives: {score.falsePositives}, " +
                      $"Citation accuracy: {score.CitationAccuracy:P0}");

            if (_reviewPanel != null)
            {
                _reviewPanel.ShowReviewSummary(score, missed);

                bool continuePressed = false;
                _reviewPanel.OnContinuePressed += () => continuePressed = true;
                yield return new WaitUntil(() => continuePressed);
            }
            else
            {
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
