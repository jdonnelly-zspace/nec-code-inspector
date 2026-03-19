using UnityEngine;

namespace NECInspector.Tools
{
    /// <summary>
    /// Base class for virtual inspection tools that attach to the stylus.
    /// Each tool has a model, activation behavior, and optional measurement capability.
    /// </summary>
    public abstract class VirtualTool : MonoBehaviour
    {
        [Header("Tool Info")]
        public string toolName = "Tool";
        public string description = "";
        public Sprite icon;

        [Header("Visuals")]
        [SerializeField] protected GameObject _toolModel;
        [SerializeField] protected Transform _tipPoint;

        protected bool _isActive = false;
        public bool IsActive => _isActive;

        public virtual void Activate()
        {
            _isActive = true;
            _toolModel?.SetActive(true);
            Debug.Log($"[Tool] {toolName} activated");
        }

        public virtual void Deactivate()
        {
            _isActive = false;
            _toolModel?.SetActive(false);
            Debug.Log($"[Tool] {toolName} deactivated");
        }

        /// <summary>
        /// Called each frame while the tool is active and pointing at something
        /// </summary>
        public virtual void OnPointAt(RaycastHit hit) { }

        /// <summary>
        /// Called when the primary button is pressed while the tool is active
        /// </summary>
        public virtual void OnUse(RaycastHit hit) { }

        /// <summary>
        /// Get the world position of the tool tip
        /// </summary>
        public Vector3 TipPosition => _tipPoint != null ? _tipPoint.position : transform.position;
    }
}
