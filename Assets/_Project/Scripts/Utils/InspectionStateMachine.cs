using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace NECInspector.StateMachine
{
    /// <summary>
    /// Clean state machine for NEC Inspector scenarios.
    /// Follows the same pattern as career-explorer's StateMachine but without external dependencies.
    /// </summary>
    public class InspectionStateMachine : MonoBehaviour
    {
        [Serializable]
        public class StateID
        {
            public readonly int ID;
            private static int _lastID = 0;
            public readonly string Name;

            public StateID() { Interlocked.Increment(ref _lastID); ID = _lastID; Name = "Step-" + ID; }
            public StateID(string name) { Interlocked.Increment(ref _lastID); ID = _lastID; Name = name; }
            public StateID(int id, string name) { ID = _lastID = id; Name = name; }

            public bool Equals(StateID other) => other != null && other.ID == ID;
            public override string ToString() => Name;
        }

        [Serializable]
        public class Transition
        {
            public readonly int ID;
            public readonly string Name;

            public Transition(int id, string name) { ID = id; Name = name; }
            public bool Equals(Transition other) => other != null && other.ID == ID;
            public override string ToString() => Name;
        }

        public static readonly Transition NextStep = new(-1, "Next Step");
        public static readonly Transition SkipStep = new(-2, "Skip Step");

        [Serializable]
        public class Step
        {
            public InspectionStateMachine StateMachine;
            public bool IsLastStep = false;
            public float PreDelay = 0f;
            public float PostDelay = 0f;

            protected StateID _stateID;
            public StateID StateID => _stateID;

            protected Transition _nextTransition = NextStep;
            public Dictionary<Transition, StateID> TransitionMap = new();

            public Action SetupAction;
            public Action CleanupAction;
            public Action ScaffoldingAction;

            protected Coroutine _scaffoldingTimeout;
            public float ScaffoldingTimeoutSeconds = -1f;

            public Step(StateID id, InspectionStateMachine stateMachine, StateID nextStepID)
            {
                _stateID = id;
                StateMachine = stateMachine;
                TransitionMap = new() { { NextStep, nextStepID } };
                stateMachine.Steps.Add(this);
            }

            public IEnumerator OnEnter(Transition transition, StateID fromStateId)
            {
                if (PreDelay > 0f) yield return new WaitForSeconds(PreDelay);
                SetupAction?.Invoke();

                if (ScaffoldingTimeoutSeconds >= 0f && ScaffoldingAction != null)
                    _scaffoldingTimeout = StateMachine.StartCoroutine(ScaffoldingTimeoutCoroutine());

                yield return OnStepEntered(transition, fromStateId);
                CleanupAction?.Invoke();
                if (PostDelay > 0f) yield return new WaitForSeconds(PostDelay);

                DoTransition();
            }

            public virtual IEnumerator OnExit(Transition transition, StateID toStateId) { yield return null; }
            protected virtual IEnumerator OnStepEntered(Transition transition, StateID fromStateId) { yield return null; }

            public StateID GetStateFromTransition(Transition transition) =>
                TransitionMap.ContainsKey(transition) ? TransitionMap[transition] : null;

            private void DoTransition() => StateMachine.DoTransition(_nextTransition);

            protected void SetNextTransition(Transition t) => _nextTransition = t;

            protected virtual IEnumerator ScaffoldingTimeoutCoroutine()
            {
                yield return new WaitForSeconds(ScaffoldingTimeoutSeconds);
                Debug.Log($"[Scaffolding] Timeout for step {_stateID.Name}");
                ScaffoldingAction?.Invoke();
            }

            protected void CancelScaffolding()
            {
                if (_scaffoldingTimeout != null)
                {
                    StateMachine.StopCoroutine(_scaffoldingTimeout);
                    _scaffoldingTimeout = null;
                }
            }
        }

        public List<Step> Steps = new();
        protected StateID _currentStateID;
        protected Step _currentStep;

        public StateID CurrentStateID => _currentStateID;
        public Step CurrentStep => _currentStep;

        public event Action<StateID> OnStepChanged;

        public void StartMachine(StateID initialStateID)
        {
            _currentStateID = initialStateID;
            _currentStep = FindStep(initialStateID);

            if (_currentStep == null)
            {
                Debug.LogError($"[StateMachine] No step found for initial state: {initialStateID.Name}");
                return;
            }

            Debug.Log($"[StateMachine] Starting: {_currentStep.StateID.Name}");
            StartCoroutine(_currentStep.OnEnter(NextStep, null));
        }

        public virtual void DoTransition(Transition transition)
        {
            StateID nextID = _currentStep.GetStateFromTransition(transition);
            if (nextID == null)
            {
                if (_currentStep.IsLastStep)
                {
                    Debug.Log("[StateMachine] Reached last step. Machine complete.");
                    OnMachineComplete();
                    return;
                }
                Debug.LogError($"[StateMachine] No state for transition ({transition.Name}) from ({_currentStep.StateID.Name})");
                return;
            }

            StartCoroutine(DoStateChange(transition, nextID));
        }

        protected virtual IEnumerator DoStateChange(Transition transition, StateID nextStepID)
        {
            yield return _currentStep.OnExit(transition, nextStepID);

            StateID oldStateID = _currentStateID;
            _currentStateID = nextStepID;
            _currentStep = FindStep(nextStepID);

            yield return new WaitForEndOfFrame();

            if (_currentStep == null)
            {
                Debug.LogWarning($"[StateMachine] No step for StateID: {nextStepID.Name}");
                yield break;
            }

            Debug.Log($"[StateMachine] Step: {_currentStep.StateID.Name}");
            OnStepChanged?.Invoke(_currentStateID);

            yield return _currentStep.OnEnter(transition, oldStateID);
        }

        protected Step FindStep(StateID id)
        {
            foreach (var step in Steps)
                if (step.StateID.ID == id.ID) return step;
            return null;
        }

        protected virtual void OnMachineComplete()
        {
            Debug.Log("[StateMachine] Complete");
        }
    }
}
