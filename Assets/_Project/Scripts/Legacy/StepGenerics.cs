using System;
using System.Collections;
using System.Collections.Generic;
using AppManagement;
using InputHandlers;
using Unity.Mathematics;
using PrimeTween;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.Rendering.Universal;
using UnityEngine.Localization.Tables;
using UnityEngine.UI;
using Utils;
using zSpace.Core;
using zSpace.Core.Input;
using static StateMachine.StateMachine;
using Locations;
using Sequence = PrimeTween.Sequence;
using System.Linq;
using static TaskMachineGenerics.AnimationEventListener;
using JobProfiles;
using zSpace.Core.Samples;

namespace TaskMachineGenerics
{
    public interface IResettableStep
    {
        void ResetStep();
    }


    /// <summary>
    /// Creates a popup dialog with a label and a button.
    /// </summary>
    /// <returns></returns>
    public class PopupStep : Step
    {
        // TODO: Add a real popup instead of the temp one once we have UI
        private readonly LocalizedString labelText;
        private readonly bool hidePopupOnExit;
        private readonly bool waitForContinue;
        private readonly bool alwaysShow;

        public PopupStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, LocalizedString labelText, bool hidePopupOnExit = false, bool waitForContinue = false, bool alwaysShow = false)
            : base(id, stateMachine, nextStepID)
        {
            (this.labelText, this.hidePopupOnExit, this.waitForContinue, this.alwaysShow) = (labelText, hidePopupOnExit, waitForContinue, alwaysShow);
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {

            if (!stateMachine.IsPracticeMode && !alwaysShow)
            {
                // Skip popup entirely if not in practice mode
                yield break;
            }
            if (CurrentTool)
            {
                foreach (ZPointer pointer in ZPointer.GetInstances())
                    CurrentTool.DetachFromPointer(pointer);
                CurrentTool.gameObject.SetActive(false);
            }

            if (TaskTextDisplay.Instance == null)
            {
                Debug.LogError("TaskTextDisplay instance not found in the scene.");
                yield break;
            }
            bool done = !waitForContinue;

            Action onNextPressed = waitForContinue ? () =>
            {
                TaskTextDisplay.Instance.SetCallbacks(onContinue: null, onPrevious: null);
                done = true;
            }
            : null;

            TaskTextDisplay.Instance.SetTextPopup(labelText, onNextPressed, waitForContinue);

            //disable colliders on a tool while the popup is present
            var zMouse = GameObject.FindObjectOfType<ZMouseCursor>();
            if (zMouse != null)
            {
                foreach (BoxCollider collider in zMouse.GetComponentsInChildren<BoxCollider>())
                    collider.enabled = false;
            }

            if (waitForContinue)
            {
                yield return new WaitUntil(() => done);
            }

            if (CurrentTool)
            {
                foreach (ZPointer pointer in ZPointer.GetInstances())
                    CurrentTool.AttachToPointer(pointer);
                CurrentTool.gameObject.SetActive(true);
            }
        }

        public override IEnumerator OnExit(Transition transition, StateID toStepId)
        {
            bool nextStepIsPopup = stateMachine.States.Any(s => s.StateID == toStepId && s is PopupStep);
            if (hidePopupOnExit && TaskTextDisplay.Instance != null && !nextStepIsPopup)
            {
                if (TaskTextDisplay.Instance != null)
                {
                    TaskTextDisplay.Instance.HidePopup();
                }
            }

            //re-enable colliders on a tool
            var zMouse = GameObject.FindObjectOfType<ZMouseCursor>();
            if (zMouse != null)
            {
                foreach (BoxCollider collider in zMouse.GetComponentsInChildren<BoxCollider>())
                    collider.enabled = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Display or remove a tooltip.
    /// </summary>
    public class ToggleTooltipStep : Step
    {
        private readonly Vector3 position;
        private readonly float scale;
        private readonly TableReference table;
        private readonly string key;
        private readonly bool showTooltip;

        public ToggleTooltipStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Vector3 position, float scale, TableReference table, string key, bool showTooltip)
            : base(id, stateMachine, nextStepID)
        {
            (this.position, this.scale, this.table, this.key, this.showTooltip) = (position, scale, table, key, showTooltip);
        }
        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (showTooltip)
                ObjectHoverTextManager.Instance.ShowText(new LocalizedString(table, key), position, key, scale);
            else
                ObjectHoverTextManager.Instance.ClearText(key);

            yield return null;
        }
    }

    /// <summary>
    /// Starts a given animator.
    /// </summary>
    /// <returns></returns>
    public class AnimationStep : Step
    {
        protected readonly Animator animator;
        protected readonly string triggerName;
        protected readonly float overrideTime;
        protected readonly string extraTrigger;

        public AnimationStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Animator animator, string triggerName = "", float overrideTime = 1f, string extraTrigger = "") // Defaulting to 1 for now
            : base(id, stateMachine, nextStepID) => (this.animator, this.triggerName, this.overrideTime, this.extraTrigger) = (animator, triggerName, overrideTime, extraTrigger);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            animator.enabled = true;
            animator.speed = 1f;
            if (!string.IsNullOrEmpty(triggerName))
            {
                Debug.Log($"Animation triggered: {triggerName}");
                animator.SetTrigger(triggerName);
            }
            if (!string.IsNullOrEmpty(extraTrigger))
            {
                Debug.Log($"Extra animation triggered: {extraTrigger}");
                animator.SetTrigger(extraTrigger);
            }
            yield return new WaitForEndOfFrame();
            // This assumes the animator only has a single state
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
            Debug.Log($"ClipInfo length: {clipInfo[0].clip.length}");
            var (length, speed, speedMultiplier) = (stateInfo.length, stateInfo.speed, stateInfo.speedMultiplier);
            Debug.Log("Aniamtor info length: " + length);
            if (overrideTime > 0f)
                yield return new WaitForSeconds(overrideTime);
            else
                yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        }
    }

    public class PausingAnimationStep : AnimationStep
    {
        protected readonly AnimationEventListener eventListener;

        public PausingAnimationStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Animator animator, string triggerName,
            AnimationEventListener eventListener) : base(id, stateMachine, nextStepID, animator, triggerName)
        => this.eventListener = eventListener;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            animator.enabled = true;
            animator.SetTrigger(triggerName);
            bool isPaused = false;

            eventListener.OnAnimationEventReceived.AddListener((AnimationEventType eventType) =>
            {
                if (eventType == AnimationEventType.Pause)
                {
                    isPaused = true;
                    animator.speed = 0;
                }
            });
            yield return new WaitUntil(() => isPaused);
        }
    }

    public class AwaitIEnumeratorStep : Step
    {
        private readonly IEnumerator coroutine;

        public AwaitIEnumeratorStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, IEnumerator coroutine)
            : base(id, stateMachine, nextStepID) => this.coroutine = coroutine;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return coroutine;
        }
    }

    public class AwaitTweenStep : Step
    {
        private Tween tween;
        private readonly Func<Tween> tweenConstructor;

        public AwaitTweenStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Tween tween)
            : base(id, stateMachine, nextStepID) => this.tween = tween;

        public AwaitTweenStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Func<Tween> tweenConstructor)
        : base(id, stateMachine, nextStepID) => this.tweenConstructor = tweenConstructor;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (tweenConstructor != null) tween = tweenConstructor.Invoke();
            if (!tween.isAlive)
            {
                Debug.Log($"Tween is not alive. Step name: '{this.stateID.Name}'.");
                yield break;
            }
            if (!tween.isPaused || tween.isAlive)
            {
                Debug.Log("Tween is already running or paused. This step will yield until the tween completes.");
            }
            tween.isPaused = false;
            yield return tween.ToYieldInstruction();
        }
    }

    public class AwaitSequenceStep : Step
    {
        private Sequence sequence;
        private readonly Func<Sequence> sequenceConstructor;

        public AwaitSequenceStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Sequence sequence)
            : base(id, stateMachine, nextStepID) => this.sequence = sequence;

        public AwaitSequenceStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Func<Sequence> sequenceConstructor)
            : base(id, stateMachine, nextStepID) => this.sequenceConstructor = sequenceConstructor;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (sequenceConstructor != null) sequence = sequenceConstructor.Invoke();
            if (sequence.isAlive)
            {
                if (!sequence.isPaused)
                {
                    Debug.LogWarning("Sequence is already running. This step will yield until the sequence completes.");
                }
                sequence.isPaused = false;

            }
            yield return sequence.ToYieldInstruction();
        }
    }

    public class SetMaterialStep : Step
    {
        private readonly Renderer renderer;
        private readonly Material material;

        public SetMaterialStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Renderer renderer, Material material)
            : base(id, stateMachine, nextStepID) => (this.renderer, this.material) = (renderer, material);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (renderer == null || material == null)
            {
                Debug.LogError("Renderer or Material is null.");
                yield break;
            }
            renderer.material = material;
            yield return null;
        }
    }

    /// PopupResetStep displays a popup dialog with a label, continue, and previous buttons.
    ///
    /// Inputs:
    /// - labelText: The main message to display (localized).
    /// - continueButtonText: The text for the continue/confirm button (localized).
    /// - previousButtonText: The text for the back/cancel button (localized).
    /// - nextStepID: The state to transition to when continue is pressed.
    /// - previousStepID: The state to transition to when back is pressed.
    /// - displayPosition: Where to display the popup (e.g., bottom, middle).
    /// - resetCallback: Optional action to run when back is pressed.
    /// </summary>
    public class PopupConfirmationStep : Step
    {
        private readonly LocalizedString labelText;
        private readonly LocalizedString continueButtonText;
        private readonly LocalizedString previousButtonText;
        private readonly Action onConfirmation;
        private readonly Action onRejection;
        private readonly bool hidePopupOnExit;
        private readonly bool showWhenNotInPracticeMode;

        public PopupConfirmationStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, LocalizedString labelText, StateID rejectionStepID, Action confirmationCallback = null, Action rejectionCallback = null, bool hidePopupOnExit = false, LocalizedString continueButtonText = null, LocalizedString previousButtonText = null, bool showWhenNotInPracticeMode = true)
            : base(id, stateMachine, nextStepID)
        {

            void onConfirmation()
            {
                transitionMap = new() { { NextStep, nextStepID } };
                confirmationCallback?.Invoke();
                if (hidePopupOnExit)
                {
                    TaskTextDisplay.Instance.HidePopup();
                }
            }

            void onRejection()
            {
                transitionMap = new() { { NextStep, rejectionStepID } };
                rejectionCallback?.Invoke();
                if (hidePopupOnExit)
                {
                    TaskTextDisplay.Instance.HidePopup();
                }
            }


            (this.labelText, this.hidePopupOnExit, this.continueButtonText, this.previousButtonText, this.onConfirmation, this.onRejection, this.showWhenNotInPracticeMode) =
            (
                labelText,
                hidePopupOnExit,
                continueButtonText,
                previousButtonText,
                onConfirmation,
                onRejection,
                showWhenNotInPracticeMode
            );
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {

            if (TaskTextDisplay.Instance == null)
            {
                Debug.LogError("TaskTextDisplay instance not found in the scene.");
                yield break;
            }

            bool done = false;

            if (!stateMachine.IsPracticeMode && !showWhenNotInPracticeMode)
            {
                // Skip popup entirely if not in practice mode
                onRejection();
                // done = true;
                yield break;
            }


            TaskTextDisplay.Instance.SetConfirmationPopup(labelText, () => { onConfirmation(); done = true; }, () => { onRejection(); done = true; }, continueButtonText, previousButtonText);

            if (CurrentTool)
            {
                foreach (ZPointer pointer in ZPointer.GetInstances())
                    CurrentTool.DetachFromPointer(pointer);
                CurrentTool.gameObject.SetActive(false);
            }
            yield return new WaitUntil(() => done);
        }

        public override IEnumerator OnExit(Transition transition, StateID toStateId)
        {
            if (TaskTextDisplay.Instance != null)
            {
                TaskTextDisplay.Instance.ClearText();
            }

            //re-enable colliders on a tool
            var zMouse = GameObject.FindObjectOfType<ZMouseCursor>();
            if (zMouse != null)
            {
                foreach (BoxCollider collider in zMouse.GetComponentsInChildren<BoxCollider>())
                    collider.enabled = true;
            }

            if (CurrentTool)
            {
                foreach (ZPointer pointer in ZPointer.GetInstances())
                    CurrentTool.AttachToPointer(pointer);
                CurrentTool.gameObject.SetActive(true);
            }
            yield return null;
        }
    }

    /// <summary>
    /// Moves the camera to the provided ZFrame.
    /// </summary>
    public class MoveCameraStep : Step
    {
        public enum TransitionType
        {
            Fade,
            Instant,
            Slide
        }
        private readonly ZFrame frame;
        private readonly TransitionType transitionType;
        private readonly float transitionDuration;
        private readonly bool fadeOutText;
        public Action midFadeAction;

        public MoveCameraStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, ZFrame frame, TransitionType transitionType = TransitionType.Fade, float transitionDuration = 1f, Action midFadeAction = null, bool fadeOutText = true)
            : base(id, stateMachine, nextStepID) => (this.frame, this.transitionType, this.transitionDuration, this.midFadeAction, this.fadeOutText) = (frame, transitionType, transitionDuration, midFadeAction, fadeOutText);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            var cameraRig = GameObject.FindWithTag("MainCamera").transform.parent.GetComponent<ZCameraRig>();
            if (fadeOutText)
            {
                TaskTextDisplay.Instance?.FadeOutText(transitionDuration / 2);
            }
            if (cameraRig == null)
            {
                Debug.LogError("ZCameraRig not found in the scene.");
                yield break;
            }
            var screenSpaceCanvas = GameObject.Find("ScreenSpaceCanvas");
            if (screenSpaceCanvas == null)
            {
                Debug.LogError("ScreenSpaceCanvas not found in the scene.");
                yield break;
            }
            screenSpaceCanvas.GetComponent<Canvas>().planeDistance = Camera.main.nearClipPlane + 0.5f;
            var fadeImage = screenSpaceCanvas.transform.Find("FadePanel").GetComponent<Image>();
            if (fadeImage == null)
            {
                Debug.LogError("Fade image not found in the scene.");
                yield break;
            }

            switch (transitionType)
            {
                case TransitionType.Fade:
                    yield return MoveCameraFade(cameraRig, frame, transitionDuration, midFadeAction);
                    break;
                case TransitionType.Instant:
                    MoveCameraInstant(cameraRig, frame);
                    break;
                case TransitionType.Slide:
                    yield return MoveCameraSlide(frame, cameraRig, transitionDuration);
                    break;
            }
            yield return null;
        }

        private static void MoveCameraInstant(ZCameraRig cameraRig, ZFrame frame)
        {
            cameraRig.Frame = frame;
        }

        public static IEnumerator MoveCameraFade(ZCameraRig cameraRig, ZFrame frame, float duration, Action midFadeAction = null)
        {
            var fadeImage = GameObject.Find("ScreenSpaceCanvas/FadePanel").GetComponent<Image>();
            fadeImage.gameObject.SetActive(true);
            var timeElapsed = -Time.deltaTime;
            var startColor = fadeImage.color;
            var endColor = new Color(startColor.r, startColor.g, startColor.b, 1f);
            while (timeElapsed < duration / 2)
            {
                timeElapsed += Time.deltaTime;
                fadeImage.color = Color.Lerp(startColor, endColor, timeElapsed / duration);
                yield return new WaitForEndOfFrame();
            }
            fadeImage.color = endColor;
            cameraRig.Frame = frame;
            Debug.Log("Invoking mid fade action");
            midFadeAction?.Invoke();
            timeElapsed = -Time.deltaTime;
            while (timeElapsed < duration / 2)
            {
                timeElapsed += Time.deltaTime;
                fadeImage.color = Color.Lerp(endColor, startColor, timeElapsed / duration);
                yield return new WaitForEndOfFrame();
            }
            fadeImage.color = startColor;

            yield return new WaitForSeconds(duration);
        }

        private static IEnumerator MoveCameraSlide(ZFrame frame, ZCameraRig cameraRig, float duration)
        {
            ZFrame initialFrame = cameraRig.Frame;
            Vector3 initialPosition = initialFrame.transform.position;
            Vector3 initialRotation = initialFrame.transform.rotation.eulerAngles;
            float initialScale = initialFrame.ViewerScale;
            Vector3 targetPosition = frame.transform.position;
            Vector3 targetRotation = frame.transform.rotation.eulerAngles;
            float targetScale = frame.ViewerScale;
            float elapsedTime = -Time.deltaTime;
            while (elapsedTime < duration)
            {
                float t = elapsedTime / duration;
                cameraRig.Frame.transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                cameraRig.Frame.transform.rotation = Quaternion.Lerp(initialFrame.transform.rotation, frame.transform.rotation, t);
                cameraRig.Frame.ViewerScale = Mathf.Lerp(initialScale, targetScale, t);
                elapsedTime += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
            cameraRig.Frame = frame;

            initialFrame.transform.position = initialPosition;
            initialFrame.transform.rotation = Quaternion.Euler(initialRotation);
            initialFrame.ViewerScale = initialScale;
        }
    }

    /// <summary>
    /// Adds a GlowEffect to the given item.
    /// </summary>
    /// <see cref="GlowEffect"/>
    public class AddGlowStep : Step
    {
        private readonly GameObject item;
        private readonly bool indefinite;
        private readonly Color color;
        private readonly bool pulse;
        private readonly float durationSeconds;

        public AddGlowStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, GameObject item, bool indefinite = true, Color? color = null, float durationSeconds = 5, bool pulse = false)
            : base(id, stateMachine, nextStepID) => (this.item, this.indefinite, this.color, this.durationSeconds, this.pulse) =
                (item, indefinite, color ?? Color.cyan, durationSeconds, pulse);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (!stateMachine.IsPracticeMode)
            {
                // Skip popup entirely if not in practice mode
                yield break;
            }

            // var glowEffect = item.AddComponent<GlowEffect>();
            // glowEffect.color = color;
            // glowEffect.glowSpeed = glowSpeed;
            // glowEffect.minGlow = minGlow;
            // glowEffect.StartGlow();
            yield return AnimationUtils.AddGlowEffect(item, color, indefinite, durationSeconds, pulse);
            // var glowEffect = item.AddComponent<GlowEffect>();
            // glowEffect.color = color;
            // glowEffect.glowSpeed = glowSpeed;
            // glowEffect.minGlow = minGlow;

            // if (!indefinite)
            //     yield return glowEffect.WaitStopGlow(durationSeconds);
        }
    }

    /// <summary>
    /// Removes a GlowEffect from the given item.
    /// </summary>
    public class RemoveGlowStep : Step
    {
        private readonly GameObject item;

        public RemoveGlowStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, GameObject item)
            : base(id, stateMachine, nextStepID) => this.item = item;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            var glowEffect = item.GetComponent<GlowEffect>();
            if (glowEffect == null)
            {
                Debug.LogError("Item does not have a GlowEffect component.");
                yield break;
            }
            GameObject.Destroy(glowEffect);
        }
    }

    public class AssignObjectPointerStep : Step
    {
        private readonly ObjectPointer item;
        private readonly IList<ZPointer> pointers;

        public AssignObjectPointerStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, ObjectPointer item, IList<ZPointer> pointers) : base(id, stateMachine, nextStepID)
            => (this.item, this.pointers) = (item, pointers);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            CurrentTool = item;
            foreach (ZPointer pointer in pointers)
            {
                item.AttachToPointer(pointer);
            }
            item.gameObject.SetActive(true);
            yield return null;
        }
    }

    /// <summary>
    /// Unassigns a given tool from the ZMouseCursor.
    /// </summary>
    public class UnassignToolStep : Step
    {
        private readonly Transform tool;

        public UnassignToolStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform tool)
            : base(id, stateMachine, nextStepID) => this.tool = tool;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            CurrentTool = null;
            tool.parent = null;
            tool.gameObject.SetActive(false);
            // Re-enable all colliders on the tool
            foreach (var collider in tool.GetComponentsInChildren<Collider>())
            {
                collider.enabled = true;
            }

            yield return null;
        }
    }

    /// <summary>
    /// Add a tool to the TaskToolInventory
    /// </summary>
    public class ToolboxAddToolStep : Step, IResettableStep
    {
        private readonly TaskToolInventory inventory;
        private readonly ObjectPointer tool;
        private readonly LocalizedString label;
        private readonly Sprite sprite;

        public ToolboxAddToolStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, TaskToolInventory inventory, ObjectPointer tool, LocalizedString label, Sprite sprite)
            : base(id, stateMachine, nextStepID)
        {
            this.inventory = inventory;
            this.tool = tool;
            this.label = label;
            this.sprite = sprite;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            inventory.AddTool(tool, label, sprite);
            yield return null;
        }

        public void ResetStep()
        {
            inventory.RemoveTool(tool);
        }
    }

    /// <summary>
    /// Wait for a tool to be selected from the TaskToolInventory
    /// </summary>
    public class ToolboxSelectToolStep : Step
    {
        private readonly TaskToolInventory inventory;
        private readonly ObjectPointer tool;
        private readonly bool closeInventoryOnSelection;

        public ToolboxSelectToolStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, TaskToolInventory inventory, ObjectPointer tool, bool closeInventoryOnSelection = false)
            : base(id, stateMachine, nextStepID)
        {
            this.inventory = inventory;
            this.tool = tool;
            this.closeInventoryOnSelection = closeInventoryOnSelection;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            inventory.Toggle(true);
            inventory.Highlight(tool);
            while (CurrentTool != tool)
            {
                yield return null;
            }
            inventory.Highlight();
            if (closeInventoryOnSelection)
                inventory.Toggle(false);
        }
    }

    /// <summary>
    /// Remove tool (that's in the TaskToolInventory) from the pointer
    /// </summary>
    public class ToolboxUnassignToolStep : Step
    {
        private readonly TaskToolInventory inventory;
        private readonly bool closeInventoryOnRemoval;

        public ToolboxUnassignToolStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, TaskToolInventory inventory, bool closeInventoryOnRemoval = false)
            : base(id, stateMachine, nextStepID)
        {
            this.inventory = inventory;
            this.closeInventoryOnRemoval = closeInventoryOnRemoval;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            inventory.DetatchToolFromPointer();
            if (closeInventoryOnRemoval)
                inventory.Toggle(false);
            yield return null;
        }
    }

    /// <summary>
    /// Toggle visibility of the TaskToolInventory
    /// </summary>
    public class ToolboxToggleStep : Step
    {
        private readonly TaskToolInventory inventory;
        private readonly bool visibility;

        public ToolboxToggleStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, TaskToolInventory inventory, bool visibility)
            : base(id, stateMachine, nextStepID)
        {
            this.inventory = inventory;
            this.visibility = visibility;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            inventory.Toggle(visibility);
            yield return null;
        }
    }

    /// <summary>
    /// Wait for a certain amount of time before moving to the next step
    /// </summary>
    public class AwaitTimeStep : Step
    {
        private readonly float time; //in seconds
        public AwaitTimeStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, float time)
            : base(id, stateMachine, nextStepID) => (this.time) = (time);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return new WaitForSeconds(time);
        }
    }

    /// <summary>
    /// Awaits a given object to be clicked
    /// </summary>
    public class AwaitClickStep : Step
    {
        private readonly Clickable item;
        private bool glow;
        private Color glowColor;
        private bool pulse;

        public AwaitClickStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Clickable item, bool glow = true, Color? glowColor = null, bool pulse = true)
            : base(id, stateMachine, nextStepID) => (this.item, this.glow, this.glowColor, this.pulse) = (item, glow, glowColor ?? Color.cyan, pulse);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (glow)
            {
                yield return AnimationUtils.AddGlowEffect(item.gameObject, glowColor, true, 5, pulse);
            }

            var clicked = false;

            System.Action<PointerEventData> handler = null;
            handler = (pointerEventData) =>
            {
                clicked = true;
                if (glow)
                    item.gameObject.GetComponent<GlowEffect>().StopGlow();
                item.OnPointerClickEvent -= handler;
            };
            item.OnPointerClickEvent += handler;

            yield return new WaitUntil(() => clicked);
        }
    }

    /// <summary>
    /// Awaits a given object to achieve a target position.
    /// </summary>
    public class AwaitPositionStep : Step
    {
        private readonly Transform item;
        private readonly GameObject targetObject;
        // private readonly Vector3 targetPosition;
        private readonly float maxPositionDelta;
        private readonly bool requireClick;

        public AwaitPositionStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform item, GameObject targetObject, float maxPositionDelta, bool requireClick = false)
            : base(id, stateMachine, nextStepID) => (this.targetObject, this.maxPositionDelta, this.item, this.requireClick) = (targetObject, maxPositionDelta, item, requireClick);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            // If the item is not active, make it active
            if (!item.gameObject.activeInHierarchy)
            {
                item.gameObject.SetActive(true);
            }

            var glowEffect = item.gameObject.AddComponent<GlowEffect>();
            var targetPosition = targetObject.transform.position;

            if (requireClick)
            {
                var clickable = targetObject.GetComponent<Clickable>();
                if (clickable != null)
                {
                    bool clicked = false;
                    void OnClicked(PointerEventData e)
                    {
                        clicked = true;
                    }
                    clickable.OnPointerClickEvent += OnClicked;
                    yield return new WaitUntil(() => clicked);
                    clickable.OnPointerClickEvent -= OnClicked;

                    item.position = targetPosition; // Move the item to the target position after clicking
                    yield break;
                }
                else
                {
                    Debug.LogWarning("AwaitPositionStep requires a Clickable component on the item if requireClick is true.");
                }
            }

            yield return new WaitUntil(() => Vector3.Distance(item.position, targetPosition) <= maxPositionDelta);
        }
    }

    /// <summary>
    /// Awaits a given object to achieve a target rotation.
    /// Will attempt to use a SingleAxisRotation component if one exists on the target.
    /// </summary>
    public class AwaitRotationStep : Step, IResettableStep
    {
        private readonly Transform item;
        private readonly Vector3 targetRotation;
        private readonly float maxRotationDelta;
        private readonly AudioClip rotateStopSound;
        private AudioSource rotateStartAudioSource;
        private AudioSource rotateStopAudioSource;
        private Quaternion initialRotation;
        private bool pulse;

        public AwaitRotationStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform item, Vector3 targetRotation, float maxRotationDelta, AudioClip rotateStopSound = null, bool pulse = true)
            : base(id, stateMachine, nextStepID) => (this.item, this.targetRotation, this.maxRotationDelta, this.initialRotation, this.rotateStopSound, this.pulse) = (item, targetRotation, maxRotationDelta, item.rotation, rotateStopSound, pulse);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            GlowEffect glowEffect = null;
            SingleAxisRotation sAR = null;

            // Create audio sources to play the rotate sounds
            if (rotateStopSound != null)
            {
                rotateStopAudioSource = item.gameObject.AddComponent<AudioSource>();
                rotateStopAudioSource.clip = rotateStopSound;
                rotateStopAudioSource.playOnAwake = false;
                rotateStopAudioSource.loop = false;
            }


            // Apply a glow if one exists and we're in practice mode
            if (stateMachine.IsPracticeMode)
            {
                glowEffect = item.gameObject.AddComponent<GlowEffect>();
                glowEffect.enabled = false;
                glowEffect.pulse = pulse;
                glowEffect.enabled = true;
            }

            sAR = item.gameObject.GetComponent<SingleAxisRotation>();
            if (sAR != null)
            {
                sAR.enabled = true;
            }
            yield return new WaitUntil(() => Quaternion.Angle(item.localRotation, Quaternion.Euler(targetRotation)) <= maxRotationDelta);

            if (rotateStopAudioSource != null)
            {
                rotateStopAudioSource.Play();
            }

            if (stateMachine.IsPracticeMode && glowEffect != null) glowEffect.enabled = false;
            if (sAR != null) sAR.enabled = false;
        }

        public void ResetStep()
        {
            item.rotation = initialRotation;
        }
    }


    /// <summary>
    /// Toggles the active state of a given GameObject.
    /// </summary>
    public class ToggleGameObjectStep : Step, IResettableStep
    {
        private readonly GameObject item;
        private readonly bool active;

        public ToggleGameObjectStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, GameObject item, bool active)
            : base(id, stateMachine, nextStepID) => (this.item, this.active) = (item, active);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            item.SetActive(active);
            yield return null;
        }

        public void ResetStep()
        {
            // Reset the GameObject to its initial state (active or inactive)
            item.SetActive(!active);
        }
    }

    /// <summary>
    /// Go to a given scene when this step is entered.
    /// </summary>
    public class GoToSceneStep : Step
    {
        private readonly LocationDefinition location;

        public GoToSceneStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, LocationDefinition location)
            : base(id, stateMachine, nextStepID) => this.location = location;

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            SceneLoader.Instance.LoadScene(location.sceneName);
            yield return null;
        }
    }

    /// <summary>
    /// Show a task end popup.
    /// </summary>
    public class ShowTaskEndStep : Step
    {
        public ShowTaskEndStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID)
            : base(id, stateMachine, nextStepID)
        {
            this.isLastStep = true;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (stateMachine.taskEndDefinition == null)
            {
                Debug.LogError($"TaskEndDefinition instance not found on {stateMachine.name}.");
                yield break;
            }
            JobProfileManager.Instance.OpenTaskEnd(stateMachine.taskEndDefinition);
            TaskTextDisplay.Instance.HidePopup();
            yield return null;
        }
    }

    /// <summary>
    /// Executes a given action when the step is entered.
    /// </summary>
    public class GenericStep : Step, IResettableStep
    {
        private readonly Action action;
        private readonly Action resetAction;

        public GenericStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Action action, Action resetAction = null)
            : base(id, stateMachine, nextStepID) => (this.action, this.resetAction) = (action, resetAction);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            action?.Invoke();
            yield return null;
        }

        public void ResetStep()
        {
            resetAction?.Invoke();
        }
    }

    /// <summary>
    /// Waits for the user to click a specific Clickable object.
    /// </summary>
    public class AwaitClickableStep : Step
    {
        private readonly Clickable clickable;
        private readonly AudioSource pluginAudioSource;

        public AwaitClickableStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Clickable clickable, AudioSource pluginAudioSource = null)
            : base(id, stateMachine, nextStepID)
        {
            this.clickable = clickable;
            this.pluginAudioSource = pluginAudioSource;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            bool clicked = false;
            void OnClicked(PointerEventData e)
            {
                clicked = true;
            }
            clickable.OnPointerClickEvent += OnClicked;
            yield return new WaitUntil(() => clicked);

            if (pluginAudioSource != null)
            {
                pluginAudioSource.Play();
            }
            clickable.OnPointerClickEvent -= OnClicked;
        }
    }

    /// <summary>
    /// Allows the user to grab and drag an object to a specified position before continuing.
    /// </summary>
    public class DragToPositionStep : Step, IResettableStep
    {
        private readonly Transform item;
        private readonly Transform target;
        private readonly float maxPositionDelta;
        private readonly bool showTargetGhost;
        private readonly bool enableGlow;
        private readonly Vector3 distanceMask;
        private readonly Vector3 offsetRotation;
        private bool snapToStart;
        private readonly Vector3 pickupOffsetWorld;

        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 _appliedOffset;



        public DragToPositionStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform item, Transform target, float maxPositionDelta, bool showTargetGhost = true, bool enableGlow = true, Vector3 distanceMask = default, bool snapToStart = false, Vector3 pickupOffsetWorld = default)
            : base(id, stateMachine, nextStepID)
        {
            this.item = item;
            this.target = target;
            this.maxPositionDelta = maxPositionDelta;
            this.showTargetGhost = showTargetGhost;
            this.enableGlow = enableGlow;
            this.initialPosition = item.position;
            this.distanceMask = distanceMask == default ? Vector3.one : distanceMask; // Default to no masking
            this.snapToStart = snapToStart;
            this.initialRotation = item.rotation;
            this.pickupOffsetWorld = pickupOffsetWorld == default ? Vector3.zero : pickupOffsetWorld;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            var targetPosition = target.position;
            _appliedOffset = Vector3.zero;

            GlowEffect glowEffect = null;
            GameObject ghost = null;

            var ge = item.GetComponent<GlowEffect>();
            if (enableGlow && ge != null)
            {
                glowEffect = ge;
            }
            else if (enableGlow && ge == null)
            {
                glowEffect = item.gameObject.AddComponent<GlowEffect>();
            }

            if (glowEffect != null)
            {
                glowEffect.enabled = false;
                glowEffect.color = new Color(0, 1, 1);

                if (stateMachine.IsPracticeMode)
                {
                    glowEffect.pulse = true;
                }
                else
                {
                    glowEffect.onlyShowWhenHovered = true;
                }

                glowEffect.enabled = true;
            }

            // Show transparent clone at target position as indicator
            if (stateMachine.IsPracticeMode && showTargetGhost)
            {
                ghost = GameObject.Instantiate(item.gameObject, targetPosition, item.rotation);
                ghost.transform.localScale = item.localScale;
                // Remove all MonoBehaviours (scripts) from the ghost
                foreach (var comp in ghost.GetComponents<MonoBehaviour>())
                    GameObject.Destroy(comp);
                foreach (var col in ghost.GetComponents<Collider>())
                    GameObject.Destroy(col);
                // Make all renderers transparent
                foreach (var renderer in ghost.GetComponentsInChildren<Renderer>())
                {
                    foreach (var mat in renderer.materials)
                    {
                        mat.shader = Shader.Find("Transparent/Diffuse");
                        var color = mat.color;
                        color.a = 0.3f;
                        mat.color = color;
                    }
                }
            }

            var draggable = item.GetComponent<zSpace.Core.Samples.Draggable>();
            if (draggable == null)
                draggable = item.gameObject.AddComponent<zSpace.Core.Samples.Draggable>();
            draggable.enabled = true;

            DragPointerDownListener downListener = item.GetComponent<DragPointerDownListener>();
            if (downListener == null)
            {
                downListener = item.gameObject.AddComponent<DragPointerDownListener>();
            }

            downListener.OnDragStart += () =>
            {
                if (_appliedOffset == Vector3.zero && pickupOffsetWorld != Vector3.zero)
                {
                    item.localPosition += pickupOffsetWorld;
                    _appliedOffset = pickupOffsetWorld;
                }
            };

            DragPointerUpListener pointerUpListener = item.GetComponent<DragPointerUpListener>();
            if (snapToStart && pointerUpListener == null)
            {
                pointerUpListener = item.gameObject.AddComponent<DragPointerUpListener>();
            }

            pointerUpListener.OnDragEnd += () =>
            {
                var posNoOffset = item.position - _appliedOffset;

                bool closeEnough = (MaskedDistance(posNoOffset, targetPosition, distanceMask) <= maxPositionDelta);

                if (closeEnough)
                {
                    item.position = targetPosition;
                    item.rotation = target.rotation;
                }
                else if (snapToStart)
                {
                    item.position = initialPosition;
                    item.rotation = initialRotation;
                    _appliedOffset = Vector3.zero; // Reset offset if snapping to start
                }
                else
                {
                    item.localPosition = posNoOffset;
                    _appliedOffset = Vector3.zero;
                }
                // if (!closeEnough)
                // {
                //     item.position = initialPosition;
                //     item.rotation = initialRotation;
                // }

            };

            // yield return new WaitUntil(() => MaskedDistance(item.position, targetPosition, distanceMask) <= maxPositionDelta);
            yield return new WaitUntil(() =>
                MaskedDistance(item.position - _appliedOffset, targetPosition, distanceMask) <= maxPositionDelta);

            // Successful: ensure we’re settled exactly at target (no offset)
            if (_appliedOffset != Vector3.zero)
            {
                item.position = targetPosition;
                _appliedOffset = Vector3.zero;
            }

            if (glowEffect != null)
                glowEffect.enabled = false;
            // Explicitly release pointer capture from all ZPointers before destroying Draggable
            foreach (var pointer in GameObject.FindObjectsOfType<zSpace.Core.Input.ZPointer>())
            {
                pointer.CapturePointer(null);
            }
            GameObject.Destroy(draggable);
            if (ghost != null)
                GameObject.Destroy(ghost);
        }

        private static float MaskedDistance(Vector3 a, Vector3 b, Vector3 mask)
        {
            Vector3 diff = Vector3.Scale(a - b, mask);
            return diff.magnitude;
        }

        public void ResetStep()
        {
            _appliedOffset = Vector3.zero;
            item.position = initialPosition;
            item.rotation = initialRotation;

            // remove the glow effect if it exists
            var glowEffect = item.GetComponent<GlowEffect>();
            if (glowEffect != null)
            {
                GameObject.Destroy(glowEffect);
            }
        }
    }

    public class DragWireToPositionStep : Step
    {
        private readonly Transform item;
        private readonly Transform target;
        private readonly float maxPositionDelta;
        private readonly Vector3 offsetAngle;
        private readonly Vector3 positionOffset;
        private readonly AudioSource pluginAudioSource;
        private readonly GameObject glowObject;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private GlowEffect glowEffect;
        private DraggableWire draggable;
        private bool soundPlayed = false;
        private Vector3 endPosition;

        public DragWireToPositionStep(
            StateID id,
            StateMachine.StateMachine stateMachine,
            StateID nextStepID,
            Transform item,
            Transform target,
            float maxPositionDelta,
            Vector3 offsetAngle = default,
            Vector3 positionOffset = default,
            Vector3 endPosition = default,
            AudioSource pluginAudioSource = null,
            GameObject glowObject = null)
            : base(id, stateMachine, nextStepID)
        {
            this.item = item;
            this.target = target;
            this.maxPositionDelta = maxPositionDelta;
            this.offsetAngle = offsetAngle == default ? Vector3.zero : offsetAngle;
            this.positionOffset = positionOffset == default ? Vector3.zero : positionOffset;
            this.endPosition = endPosition == default ? Vector3.zero : endPosition;
            this.pluginAudioSource = pluginAudioSource;
            this.glowObject = glowObject;
            this.initialPosition = item.localPosition;
            this.initialRotation = item.localRotation;
            this.draggable = item.GetComponent<DraggableWire>();

        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            var targetPosition = target.position;
            // bool isMouseOver = IsMouseOverTarget();
            bool closeEnough = Vector3.Distance(item.position, targetPosition) <= maxPositionDelta;
            bool snapped = false;

            glowEffect = glowObject?.GetComponent<GlowEffect>();
            if (glowEffect == null)
            {
                glowEffect = item.gameObject.AddComponent<GlowEffect>();
                glowEffect.enabled = false;
                glowEffect.pulse = true;
                glowEffect.enabled = true;
            }

            DragPointerUpListener pointerUpListener = item.GetComponent<DragPointerUpListener>();
            if (pointerUpListener == null)
            {
                pointerUpListener = item.gameObject.AddComponent<DragPointerUpListener>();
            }

            DragPointerDownListener pointerDownListener = item.GetComponent<DragPointerDownListener>();
            if (pointerDownListener == null)
            {
                pointerDownListener = item.gameObject.AddComponent<DragPointerDownListener>();
            }

            pointerDownListener.OnDragStart += () =>
            {
                // Disable glow
                glowEffect.enabled = false;

                // rotate the object if there is an offset
                if (offsetAngle != Vector3.zero)
                {
                    item.localRotation = Quaternion.Euler(offsetAngle);
                }
            };

            pointerUpListener.OnDragEnd += () =>
            {

                // bool closeEnough = Vector3.Distance(item.position, target.position) <= maxPositionDelta;

                if (!snapped)
                {
                    glowEffect.enabled = true; // Re-enable glow if not snapped
                    item.localPosition = initialPosition;
                    item.localRotation = initialRotation;
                }

                // if (Vector3.Distance(item.position, target.position) <= maxPositionDelta)
                // {
                //     SnapPlug();
                // }
                // else
                // {
                //     glowEffect.enabled = true; // Re-enable glow if not snapped
                //     item.localPosition = initialPosition;
                //     item.localRotation = initialRotation;
                // }

            };

            // Wait until the wire end is close enough to the target
            yield return new WaitUntil(() => Vector3.Distance(item.position, targetPosition) <= maxPositionDelta);
            snapped = true;
            SnapPlug();
        }

        // private bool IsMouseOverTarget()
        // {
        //     if (!draggable.IsDragging) return false;
        //     // Only apply if the current pointer is a ZMouse
        //     var mousePointer = GameObject.FindObjectOfType<zSpace.Core.Input.ZMouse>();
        //     if (mousePointer == null)
        //     {
        //         Debug.LogWarning("No ZMouse found in the scene.");
        //         return false;
        //     }

        //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //     if (Physics.Raycast(ray, out RaycastHit hit))
        //     {
        //         return hit.transform == target;
        //     }

        //     return false;
        // }

        private void SnapPlug()
        {
            foreach (var pointer in GameObject.FindObjectsOfType<zSpace.Core.Input.ZPointer>())
            {
                pointer.CapturePointer(null);
            }

            if (draggable != null)
            {
                if (endPosition != Vector3.zero)
                {
                    draggable.LockAt(endPosition, positionOffset, offsetAngle);
                }
                else
                {
                    draggable.LockAt(target, positionOffset, offsetAngle);
                }
            }

            if (endPosition == Vector3.zero)
            {
                item.position = target.position + positionOffset;
            }

            item.localRotation = Quaternion.Euler(offsetAngle);

            var collider = item.GetComponent<Collider>();
            if (collider != null)
                collider.enabled = false;

            if (pluginAudioSource != null && !soundPlayed)
            {
                pluginAudioSource.Play();
                soundPlayed = true;
            }
        }
        // public void ResetStep()
        // {
        //     item.position = initialPosition;
        // }
    }

    /// <summary>
    /// Animates an object from its current position to a target position along a parabolic arc.
    /// </summary>
    public class AnimateArcStep : Step, IResettableStep
    {
        private readonly Transform item;
        private readonly Vector3 targetPosition;
        private readonly float duration;
        private readonly float arcHeight;
        private readonly AnimationCurve arcCurve;
        private Vector3 initialPosition;

        public AnimateArcStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform item, Vector3 targetPosition, float duration = 1.0f, float arcHeight = 0.2f, bool enableGlow = true, AnimationCurve arcCurve = null)
            : base(id, stateMachine, nextStepID)
        {
            this.item = item;
            this.targetPosition = targetPosition;
            this.duration = duration;
            this.arcHeight = arcHeight;
            this.arcCurve = arcCurve ?? new AnimationCurve(
                new Keyframe(0, 0),
                new Keyframe(0.5f, 1),
                new Keyframe(1, 0)
            );
            this.initialPosition = item.position;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            Vector3 start = item.position;
            Vector3 end = targetPosition;
            float elapsed = 0f;
            while (elapsed < duration)
            {
                float t = Mathf.Clamp01(elapsed / duration);
                // Parabolic arc: interpolate position, then add arc offset
                Vector3 pos = Vector3.Lerp(start, end, t);
                float arcT = arcCurve.Evaluate(t);
                // Arc is perpendicular to the straight line
                Vector3 up = Vector3.up;
                if (Mathf.Abs(Vector3.Dot((end - start).normalized, up)) > 0.95f)
                    up = Vector3.Cross((end - start).normalized, Vector3.right); // avoid colinear
                pos += up.normalized * arcHeight * arcT;
                item.position = pos;
                elapsed += Time.deltaTime;
                yield return null;
            }
            item.position = end;
        }

        public void ResetStep()
        {
            item.position = initialPosition;
        }
    }

    public class ResetStatesStep : Step
    {
        private readonly System.Action onResetCallback;

        public ResetStatesStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, System.Action onResetCallback = null)
            : base(id, stateMachine, nextStepID)
        {
            this.onResetCallback = onResetCallback;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            // Reset all steps in the state machine to their initial state
            foreach (var step in stateMachine.States)
            {
                var resettable = step as IResettableStep;
                if (resettable != null)
                {
                    resettable.ResetStep();
                }
            }

            yield return null;
            onResetCallback?.Invoke();
        }
    }

    /// <summary>
    /// Waits for a specified number of seconds, then performs an optional action before continuing to the next step.
    /// </summary>
    public class WaitStep : Step
    {
        private readonly float seconds;
        private readonly Action postWaitAction;

        public WaitStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, float seconds, Action postWaitAction = null)
            : base(id, stateMachine, nextStepID)
        {
            this.seconds = seconds;
            this.postWaitAction = postWaitAction;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return new WaitForSeconds(seconds);
            postWaitAction?.Invoke();
        }
    }

    public class FadeInObjectStep : Step
    {
        private readonly Transform item;
        private readonly IEnumerable<Transform> items;
        private readonly Vector3? position;
        private readonly Vector3? rotation;
        private readonly float durationMs;

        public FadeInObjectStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Transform item, float durationMs, Vector3? position, Vector3? rotation) : base(id, stateMachine, nextStepID)
        {
            this.item = item;
            this.position = position;
        }
        public FadeInObjectStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, IEnumerable<Transform> items, float durationMs, Vector3? position, Vector3? rotation) : base(id, stateMachine, nextStepID)
        {
            this.items = items;
            this.position = position;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (item != null)
                yield return AnimationUtils.FadeInObject(item, durationMs, position, rotation);

        }
    }



    public class FadeDecalStep : Step
    {
        private readonly DecalProjector decal;
        private readonly float targetAlpha;
        private readonly float fadeDuration;
        public FadeDecalStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, DecalProjector decal, float targetAlpha = 1f, float fadeDuration = 1f)
            : base(id, stateMachine, nextStepID) => (this.decal, this.targetAlpha, this.fadeDuration) = (decal, targetAlpha, fadeDuration);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            if (!decal.enabled)
            {
                // Assume it's not set to the inital opacity
                decal.fadeFactor = 0f;
                decal.enabled = true;
            }

            yield return Tween.Custom(decal.fadeFactor, targetAlpha, duration: fadeDuration, onValueChange: newVal => decal.fadeFactor = newVal)
                .ToYieldInstruction();
        }
    }

    public class FadeOutObjectStep : Step
    {
        private readonly Transform item;
        private readonly float durationMs;

        public FadeOutObjectStep(StateID id, StateMachine.StateMachine stateID, StateID nextStepID, Transform item, float durationMs) : base(id, stateID, nextStepID)
        {
            this.item = item;
            this.durationMs = durationMs;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return AnimationUtils.FadeOutObject(item, durationMs);
        }
    }

    /// <summary>
    /// Step for awaiting a draggable to be snapped to a drop target.
    /// </summary>
    public class DragToDropTargetStep : Step
    {
        private readonly SnappingDraggablePlane item;
        private readonly List<DropTarget> targets;
        private readonly float maxDistance;
        private readonly Action onSnapFailedAction;

        public DragToDropTargetStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, SnappingDraggablePlane item, List<DropTarget> targets, float maxDistance = 0.1f, Action onSnapFailedAction = null)
            : base(id, stateMachine, nextStepID) => (this.item, this.targets, this.maxDistance, this.onSnapFailedAction) = (item, targets, maxDistance, onSnapFailedAction);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            foreach (DropTarget target in targets)
                target.enabled = true;
            item.shouldSnapToDropTarget = true;
            item.dropTargets = targets;
            item.maxSnappingDistance = maxDistance;
            item.shouldReturnToStartIfNotSnapped = true;
            if (onSnapFailedAction is not null) item.OnSnapFailed += onSnapFailedAction;
            yield return new WaitUntil(() => item.snapped);
            item.enabled = false;
            foreach (DropTarget target in targets)
                target.enabled = false;
        }
    }

    /// <summary>
    /// Play audio from a source
    /// </summary>
    public class PlayAudioStep : Step
    {
        private readonly AudioSource audio;
        public PlayAudioStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, AudioSource audio)
            : base(id, stateMachine, nextStepID) => (this.audio) = (audio);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            audio.Play();
            yield return null;
        }
    }

    public class WaitForToolSelectedStep : Step
    {
        private readonly ObjectPointer tool;
        private readonly Action onToolSelected;

        public WaitForToolSelectedStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, ObjectPointer tool, Action onToolSelected = null)
            : base(id, stateMachine, nextStepID) => (this.tool, this.onToolSelected) = (tool, onToolSelected);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return new WaitUntil(() => CurrentTool == tool);
            onToolSelected?.Invoke();
        }
    }

    public class ShowVideoStep : Step
    {
        private string videoResource;
        private List<AudioSource> haltedAudio;

        public ShowVideoStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, string videoResource)
            : base(id, stateMachine, nextStepID)
        {
            this.videoResource = videoResource;
        }

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStateId)
        {
            if (VideoPlayerManager.Instance == null)
            {
                Debug.LogError("VideoPlayerManager.Instance is null. Cannot play video.");
                yield break;
            }

            if (string.IsNullOrEmpty(videoResource))
            {
                Debug.LogError("Video resource is null or empty. Cannot play video.");
                yield break;
            }

            haltedAudio = GameObject.FindObjectsOfType<AudioSource>().Where(a => a.isPlaying).ToList();
            foreach (AudioSource audio in haltedAudio)
                audio.Pause();

            VideoPlayerManager.Instance.ShowVideo(videoResource);
            while (VideoPlayerController.Instance != null && VideoPlayerController.Instance.gameObject.activeInHierarchy)
            {
                yield return null;
            }

            foreach (AudioSource audio in haltedAudio)
                audio.UnPause();
        }
    }

    public class CoroutineStep : Step
    {
        private readonly Func<IEnumerator> coroutine;

        public CoroutineStep(StateID id, StateMachine.StateMachine stateMachine, StateID nextStepID, Func<IEnumerator> coroutine)
            : base(id, stateMachine, nextStepID) => (this.coroutine) = (coroutine);

        protected override IEnumerator OnStepEntered(Transition transition, StateID fromStepId)
        {
            yield return coroutine.Invoke();
        }
    }
}
