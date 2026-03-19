using System.Collections;
using UnityEngine;
using NECInspector.Core;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Final scoring display with letter grade, accuracy, and option to retry or return to menu.
    /// </summary>
    public class ScoreStep : InspectionStateMachine.Step
    {
        private readonly InspectionManager _inspectionManager;
        private readonly InspectionReviewPanel _reviewPanel;

        public ScoreStep(InspectionStateMachine.StateID id, InspectionStateMachine stateMachine,
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

            Debug.Log($"[ScoreStep] Grade: {score.LetterGrade}, " +
                      $"Accuracy: {score.Accuracy:P0}, " +
                      $"Citation: {score.CitationAccuracy:P0}, " +
                      $"Time: {score.timeElapsed:F0}s");

            if (_reviewPanel != null)
            {
                _reviewPanel.ShowFinalScore(score);

                // Wait for user to choose retry or menu
                bool actionTaken = false;
                _reviewPanel.OnRetryPressed += () =>
                {
                    actionTaken = true;
                    // Reload the current scene
                    var sceneName = _inspectionManager.ScenarioDefinition.sceneName;
                    GameManager.Instance?.LoadScene(sceneName);
                };
                _reviewPanel.OnMenuPressed += () =>
                {
                    actionTaken = true;
                    GameManager.Instance?.ReturnToMainMenu();
                };

                yield return new WaitUntil(() => actionTaken);
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
