using UnityEngine;
using NECInspector.Core;
using NECInspector.Tools;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Bridges stylus/mouse pointer input to the inspection system.
    /// Handles raycasting, component highlighting, inspection popups, and tool usage.
    /// Attach to the ZCameraRig or a persistent scene object.
    /// </summary>
    public class InspectionPointerController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _rayDistance = 10f;
        [SerializeField] private LayerMask _inspectableLayer;

        [Header("References")]
        [SerializeField] private Camera _mainCamera;
        [SerializeField] private ViolationFlaggingPanel _flaggingPanel;
        [SerializeField] private ToolBelt _toolBelt;

        [Header("Visual Feedback")]
        [SerializeField] private LineRenderer _pointerLine;
        [SerializeField] private GameObject _hitIndicator;

        private InspectableComponent _currentHover;
        private bool _isInspecting = false;

        private void Update()
        {
            if (_isInspecting) return; // Don't raycast while flagging panel is open

            DoRaycast();
            HandleInput();
        }

        private void DoRaycast()
        {
            // Use mouse position for Mac fallback; stylus ray for zSpace
            // ZPointerEventData handles this automatically through Clickable,
            // but for direct raycasting we use the camera
            Ray ray = GetPointerRay();

            if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _inspectableLayer))
            {
                var component = hit.collider.GetComponent<InspectableComponent>();

                if (component != _currentHover)
                {
                    // Exit previous hover
                    _currentHover?.SetHighlighted(false);

                    // Enter new hover
                    _currentHover = component;
                    _currentHover?.SetHighlighted(true);
                }

                // Update pointer line
                UpdatePointerVisual(ray.origin, hit.point, true);

                // Update active tool
                _toolBelt?.ActiveTool?.OnPointAt(hit);
            }
            else
            {
                if (_currentHover != null)
                {
                    _currentHover.SetHighlighted(false);
                    _currentHover = null;
                }

                UpdatePointerVisual(ray.origin, ray.origin + ray.direction * _rayDistance, false);
            }
        }

        private void HandleInput()
        {
            // Primary button / left click - inspect component
            if (Input.GetMouseButtonDown(0) && _currentHover != null)
            {
                InspectComponent(_currentHover);
            }

            // Secondary button / right click - use active tool
            if (Input.GetMouseButtonDown(1) && _currentHover != null)
            {
                Ray ray = GetPointerRay();
                if (Physics.Raycast(ray, out RaycastHit hit, _rayDistance, _inspectableLayer))
                {
                    _toolBelt?.ActiveTool?.OnUse(hit);
                }
            }
        }

        private void InspectComponent(InspectableComponent component)
        {
            component.Inspect();

            if (_flaggingPanel != null)
            {
                _isInspecting = true;

                var citationMode = GameManager.Instance?.Difficulty.CurrentSettings?.citationMode
                    ?? NECCitationMode.SearchableDropdown;

                _flaggingPanel.Show(component, citationMode);

                // Listen for panel close to resume raycasting
                _flaggingPanel.OnViolationSubmitted += OnPanelClosed;
                _flaggingPanel.OnMarkedCompliant += OnPanelClosedCompliant;
            }
        }

        private void OnPanelClosed(InspectableComponent comp, string desc, string nec)
        {
            _isInspecting = false;
            _flaggingPanel.OnViolationSubmitted -= OnPanelClosed;
            _flaggingPanel.OnMarkedCompliant -= OnPanelClosedCompliant;
        }

        private void OnPanelClosedCompliant(InspectableComponent comp)
        {
            _isInspecting = false;
            _flaggingPanel.OnViolationSubmitted -= OnPanelClosed;
            _flaggingPanel.OnMarkedCompliant -= OnPanelClosedCompliant;
        }

        private Ray GetPointerRay()
        {
            // This will be replaced with ZStylus ray when on zSpace hardware.
            // For Mac development, use mouse position through camera.
            if (_mainCamera == null) _mainCamera = Camera.main;
            return _mainCamera != null
                ? _mainCamera.ScreenPointToRay(Input.mousePosition)
                : new Ray(transform.position, transform.forward);
        }

        private void UpdatePointerVisual(Vector3 start, Vector3 end, bool hasHit)
        {
            if (_pointerLine != null)
            {
                _pointerLine.SetPosition(0, start);
                _pointerLine.SetPosition(1, end);
                _pointerLine.startColor = hasHit ? Color.cyan : Color.white;
                _pointerLine.endColor = hasHit ? Color.cyan : new Color(1, 1, 1, 0.3f);
            }

            if (_hitIndicator != null)
            {
                _hitIndicator.SetActive(hasHit);
                if (hasHit) _hitIndicator.transform.position = end;
            }
        }
    }
}
