using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NECInspector.Data;
using NECInspector.NEC;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// For each missed violation, shows the NEC article reference and highlights
    /// the component in the scene. Educational review step.
    /// </summary>
    public class NECReviewStep : InspectionStateMachine.Step
    {
        private readonly InspectionManager _inspectionManager;
        private readonly InspectionReviewPanel _reviewPanel;
        private readonly NECReferencePanel _necPanel;

        public NECReviewStep(InspectionStateMachine.StateID id, InspectionStateMachine stateMachine,
            InspectionStateMachine.StateID nextStepID, InspectionManager inspectionManager,
            InspectionReviewPanel reviewPanel, NECReferencePanel necPanel)
            : base(id, stateMachine, nextStepID)
        {
            _inspectionManager = inspectionManager;
            _reviewPanel = reviewPanel;
            _necPanel = necPanel;
        }

        protected override IEnumerator OnStepEntered(InspectionStateMachine.Transition transition,
            InspectionStateMachine.StateID fromStateId)
        {
            var missed = _inspectionManager.GetMissedViolations();

            if (missed.Count == 0)
            {
                Debug.Log("[NECReview] No missed violations - perfect score!");
                if (_reviewPanel != null)
                {
                    _reviewPanel.ShowPerfectScore();
                    bool done = false;
                    _reviewPanel.OnContinuePressed += () => done = true;
                    yield return new WaitUntil(() => done);
                }
                else
                {
                    yield return new WaitForSeconds(2f);
                }
                yield break;
            }

            Debug.Log($"[NECReview] Reviewing {missed.Count} missed violations");

            // Walk through each missed violation
            for (int i = 0; i < missed.Count; i++)
            {
                var violation = missed[i];
                var component = _inspectionManager.GetComponentForViolation(violation);

                // Highlight the component in the scene
                if (component != null)
                    component.ShowHintPulse();

                // Show NEC article in the reference panel
                NECArticle article = null;
                if (NECDatabase.Instance != null)
                    article = NECDatabase.Instance.GetArticle(violation.necArticle);

                if (_reviewPanel != null)
                {
                    _reviewPanel.ShowMissedViolation(
                        violationIndex: i + 1,
                        totalMissed: missed.Count,
                        violation: violation,
                        article: article
                    );
                }

                if (_necPanel != null && article != null)
                    _necPanel.ShowArticle(article);

                // Wait for user to press next
                bool nextPressed = false;
                if (_reviewPanel != null)
                {
                    _reviewPanel.OnContinuePressed += () => nextPressed = true;
                    yield return new WaitUntil(() => nextPressed);
                }
                else
                {
                    yield return new WaitForSeconds(3f);
                }

                // Clear highlight
                if (component != null)
                    component.SetHighlighted(false);
            }

            _necPanel?.Hide();
        }
    }
}
