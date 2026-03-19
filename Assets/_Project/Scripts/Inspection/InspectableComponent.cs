using System;
using UnityEngine;
using NECInspector.Data;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Attach to any inspectable electrical component in a scenario scene.
    /// Provides component metadata and tracks inspection state during gameplay.
    /// </summary>
    public class InspectableComponent : MonoBehaviour
    {
        [Header("Component Info")]
        public string componentName = "Electrical Component";
        public string componentType = "General";
        [TextArea(2, 4)]
        public string description = "";

        [Header("Inspection State")]
        [SerializeField] private bool _hasBeenInspected = false;
        [SerializeField] private bool _hasBeenFlagged = false;

        [Header("Visual")]
        [SerializeField] private Color _highlightColor = new Color(0f, 0.83f, 1f, 1f); // Cyan
        [SerializeField] private Color _flaggedColor = new Color(1f, 0.7f, 0.2f, 1f);  // Amber
        [SerializeField] private Color _compliantColor = new Color(0.2f, 0.9f, 0.3f, 1f); // Green

        private Renderer[] _renderers;
        private Material[] _originalMaterials;
        private Outline _outline;
        private bool _isHighlighted = false;

        public bool HasBeenInspected => _hasBeenInspected;
        public bool HasBeenFlagged => _hasBeenFlagged;
        public string FlaggedViolationId { get; private set; }
        public string FlaggedNECArticle { get; private set; }

        public event Action<InspectableComponent> OnInspected;
        public event Action<InspectableComponent> OnFlagged;
        public event Action<InspectableComponent> OnMarkedCompliant;

        private void Awake()
        {
            _renderers = GetComponentsInChildren<Renderer>();
            _outline = GetComponent<Outline>();
            if (_outline == null)
            {
                _outline = gameObject.AddComponent<Outline>();
                _outline.OutlineMode = Outline.Mode.OutlineAll;
                _outline.OutlineWidth = 4f;
                _outline.enabled = false;
            }
        }

        /// <summary>
        /// Called when the stylus hovers over this component
        /// </summary>
        public void SetHighlighted(bool highlighted)
        {
            _isHighlighted = highlighted;
            if (_outline != null)
            {
                _outline.enabled = highlighted;
                _outline.OutlineColor = _hasBeenFlagged ? _flaggedColor :
                                        _hasBeenInspected ? _compliantColor :
                                        _highlightColor;
            }
        }

        /// <summary>
        /// Called when the student inspects this component (clicks on it)
        /// </summary>
        public void Inspect()
        {
            _hasBeenInspected = true;
            OnInspected?.Invoke(this);
            Debug.Log($"[Inspect] {componentName} inspected");
        }

        /// <summary>
        /// Called when the student flags a violation on this component
        /// </summary>
        public void FlagViolation(string violationId, string necArticle)
        {
            _hasBeenFlagged = true;
            FlaggedViolationId = violationId;
            FlaggedNECArticle = necArticle;
            OnFlagged?.Invoke(this);

            if (_outline != null)
                _outline.OutlineColor = _flaggedColor;

            Debug.Log($"[Inspect] {componentName} flagged: {violationId} ({necArticle})");
        }

        /// <summary>
        /// Called when the student marks this component as compliant
        /// </summary>
        public void MarkCompliant()
        {
            _hasBeenInspected = true;
            _hasBeenFlagged = false;
            FlaggedViolationId = null;
            FlaggedNECArticle = null;
            OnMarkedCompliant?.Invoke(this);

            if (_outline != null)
                _outline.OutlineColor = _compliantColor;

            Debug.Log($"[Inspect] {componentName} marked compliant");
        }

        /// <summary>
        /// Reset for replay
        /// </summary>
        public void ResetInspection()
        {
            _hasBeenInspected = false;
            _hasBeenFlagged = false;
            FlaggedViolationId = null;
            FlaggedNECArticle = null;
            SetHighlighted(false);
        }

        /// <summary>
        /// Show a hint pulse for Beginner mode
        /// </summary>
        public void ShowHintPulse()
        {
            if (_outline != null)
            {
                _outline.enabled = true;
                _outline.OutlineColor = _flaggedColor;
                // Pulse will be driven by animation or coroutine in the step
            }
        }
    }
}
