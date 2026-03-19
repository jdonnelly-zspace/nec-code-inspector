using UnityEngine;
using NECInspector.Core;
using NECInspector.StateMachine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Drives the step-by-step flow of an inspection scenario.
    /// Attach to the scenario scene root alongside InspectionManager.
    /// </summary>
    [RequireComponent(typeof(InspectionManager))]
    public class InspectionScenarioRunner : InspectionStateMachine
    {
        [Header("UI References")]
        [SerializeField] private InspectionHUD _hud;
        [SerializeField] private ViolationFlaggingPanel _flaggingPanel;
        [SerializeField] private InspectionReviewPanel _reviewPanel;
        [SerializeField] private NECReferencePanel _necPanel;

        private InspectionManager _inspectionManager;

        // State IDs
        private StateID _introID;
        private StateID _freeInspectionID;
        private StateID _reviewID;
        private StateID _necReviewID;
        private StateID _scoreID;

        private void Start()
        {
            _inspectionManager = GetComponent<InspectionManager>();

            var difficulty = GameManager.Instance != null
                ? GameManager.Instance.Difficulty.CurrentLevel
                : DifficultyLevel.Standard;

            _inspectionManager.Initialize(difficulty);

            BuildSteps(difficulty);

            // Start the state machine
            StartMachine(_introID);
        }

        private void BuildSteps(DifficultyLevel difficulty)
        {
            _introID = new StateID("Intro");
            _freeInspectionID = new StateID("FreeInspection");
            _reviewID = new StateID("Review");
            _necReviewID = new StateID("NECReview");
            _scoreID = new StateID("Score");

            // Intro -> FreeInspection -> Review -> NECReview -> Score
            var introStep = new IntroStep(_introID, this, _freeInspectionID, _inspectionManager, _hud);
            var inspectionStep = new FreeInspectionStep(_freeInspectionID, this, _reviewID, _inspectionManager, _hud, _flaggingPanel, difficulty);
            var reviewStep = new ReviewStep(_reviewID, this, _necReviewID, _inspectionManager, _reviewPanel);
            var necReviewStep = new NECReviewStep(_necReviewID, this, _scoreID, _inspectionManager, _reviewPanel, _necPanel);
            var scoreStep = new ScoreStep(_scoreID, this, null, _inspectionManager, _reviewPanel);
            scoreStep.IsLastStep = true;
        }

        protected override void OnMachineComplete()
        {
            Debug.Log("[ScenarioRunner] Inspection scenario complete!");

            // Save progress
            if (GameManager.Instance != null)
            {
                var score = _inspectionManager.CalculateScore();
                GameManager.Instance.Progress.RecordInspectionScore(
                    _inspectionManager.ScenarioDefinition.id,
                    GameManager.Instance.Difficulty.CurrentLevel,
                    score
                );
            }
        }
    }
}
