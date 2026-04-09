using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using JobProfiles;
using UnityEngine;

namespace StateMachine
{
    public class StateMachine : MonoBehaviour
    {
        public TaskEndDefinition taskEndDefinition;
        public static ObjectPointer CurrentTool;
        [System.Serializable]
        public class StateID
        {
            public readonly int ID;
            static int lastID = 0;
            public readonly string Name;
            public StateID()
            {
                Interlocked.Increment(ref lastID);
                ID = lastID;
                Name = "StepID-" + ID;
            }
            public StateID(int id)
            {
                ID = lastID = id;
                Name = "StepID-" + id;
            }
            public StateID(int id, string name)
            {
                ID = lastID = id;
                Name = name;
            }
            public StateID(string name)
            {
                Interlocked.Increment(ref lastID); // Not necessary for single-threading but it looks cool
                ID = lastID;
                Name = name;
            }
            public bool Equals(StateID state)
            {
                return state.ID == ID;
            }
            public override string ToString()
            {
                return Name;
            }
        }
        public class StateIDs { };
        [System.Serializable]
        public class Transition
        {
            public readonly int ID;
            public readonly string Name;
            public Transition(int id)
            {
                ID = id;
                Name = "TransitionID-" + id;
            }
            public Transition(int id, string name)
            {
                ID = id;
                Name = name;
            }
            public bool Equals(Transition trans)
            {
                return trans.ID == ID;
            }
            public override string ToString()
            {
                return Name;
            }
        };
        public static Transition NextStep = new(-1, "Next Step");
        public static Transition ToScaffoldingTransition = new(-2, "To Remediation");
        public static Transition ConfirmationYes = new(-3, "");
        public static Transition ConfirmationNo = new(-4, "");
        public class Transitions { };
        [System.Serializable]
        public class Step
        {
            public StateMachine stateMachine;
            public bool isLastStep = false;
            /// <summary> Delay before the step executes </summary>
            public float PreDelay = 0f;
            /// <summary> Delay after the step executes </summary>
            public float PostDelay = 0f;
            protected StateID stateID;
            public StateID StateID => stateID;
            protected Transition nextTransition = StateMachine.NextStep;
            public Dictionary<Transition, StateID> transitionMap = new Dictionary<Transition, StateID>();
            public Action SetupAction, CleanupAction;
            public Action ScaffoldingAction;
            protected Coroutine scaffoldingTimeout;
            public float scaffoldingTimeoutSeconds = -1f;
            public virtual IEnumerator OnExit(Transition transition, StateID toStateId) { yield return null; }
            public IEnumerator OnEnter(Transition transition, StateID fromStateId)
            {
                if (PreDelay > 0f) yield return new WaitForSeconds(PreDelay);
                SetupAction?.Invoke();
                if (scaffoldingTimeoutSeconds >= 0f && ScaffoldingAction != null)
                {
                    scaffoldingTimeout = stateMachine.StartCoroutine(ScaffoldingTimeoutCoroutine());
                }
                yield return OnStepEntered(transition, fromStateId);
                CleanupAction?.Invoke();
                if (PostDelay > 0f) yield return new WaitForSeconds(PostDelay);
                StartTransition();
            }
            protected virtual IEnumerator OnStepEntered(Transition transition, StateID fromStateId) { yield return null; }
            public StateID GetStateFromTransition(Transition transition) =>
                transitionMap.ContainsKey(transition) ? transitionMap[transition] : null;

            public Step(StateID id, StateMachine stateMachine, StateID nextStepID)
            {
                (stateID, this.stateMachine) = (id, stateMachine);
                transitionMap = new() { { NextStep, nextStepID } };
                stateMachine.States.Add(this);
            }
            private void StartTransition() => stateMachine.DoTransition(nextTransition);
            protected virtual IEnumerator ScaffoldingTimeoutCoroutine()
            {
                yield return new WaitForSeconds(scaffoldingTimeoutSeconds);
                if (stateMachine.IsPracticeMode)
                {
                    Debug.LogWarning($"Scaffolding timeout reached for step {stateID.Name}. Running Scaffolding.");
                    ScaffoldingAction.Invoke();
                }
            }
            protected void DisplayScaffolding()
            {
                if (scaffoldingTimeout != null)
                {
                    stateMachine.StopCoroutine(scaffoldingTimeout);
                    scaffoldingTimeout = null;
                }
                ScaffoldingAction?.Invoke();
            }
        }
        public List<Step> States = new List<Step>();
        protected StateID currentStateID;
        public StateID CurrentStateID { get { return currentStateID; } }
        protected Step currentState;
        public Step CurrentState { get { return currentState; } }
        public bool IsPracticeMode = true;
        public virtual void DoTransition(Transition transition)
        {
            StateID newStateID = currentState.GetStateFromTransition(transition);
            if (newStateID == null && !currentState.isLastStep)
            {
                Debug.LogError($"No next state defined for state ({currentState.StateID.Name}) and transition ({transition.Name})");
                return;
            }
            if (!(currentState is TaskMachineGenerics.ShowTaskEndStep || currentState is TaskMachineGenerics.GoToSceneStep))
                StartCoroutine(DoStateChange(transition, newStateID));
        }
        public virtual IEnumerator DoStateChange(Transition transition, StateID nextStepID)
        {
            yield return currentState.OnExit(transition, nextStepID);
            StateID oldStateID = currentStateID;
            currentStateID = nextStepID;
            currentState = null;
            foreach (var state in States)
            {
                if (state.StateID.ID != currentStateID.ID)
                    continue;
                currentState = state;
                break;
            }
            yield return new WaitForEndOfFrame();
            if (currentState == null)
            {
                Debug.LogWarning($"No state defined for given StateId ({currentStateID.Name}) on StateMachine ({name})");
                yield break;
            }

            Debug.Log($"Starting step {currentState.StateID.Name}");

            yield return currentState.OnEnter(transition, oldStateID);
        }
    }
    public enum ScaffoldingType
    {
        None,
        Timed,
        Incremental,
    }
}