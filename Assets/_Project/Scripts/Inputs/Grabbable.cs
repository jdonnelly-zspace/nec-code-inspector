using System;
using UnityEngine;
using UnityEngine.EventSystems;
using zSpace.Core.EventSystems;
using zSpace.Core.Input;

namespace InputHandlers
{
    public enum RotationalAxis { X, Y, Z }
    public enum GrabMode
    {
        PositionAndRotation,
        PositionOnly,
        RotationOnly
    }
    public enum RotationMode
    {
        Free,
        SingleAxis,
        // TODO: Need to apply two axis rotation. When Implementing this remember to go to the Editor Script to show the field for secondaryAxis
        // TwoAxis
    }
    public enum StylusRotationMode
    {
        MatchStylus,
        DragRotation
    }
    public enum ContstraintType
    {
        None,
        Spherical,
        Rectangular
    }
    [Serializable]
    public struct Vector3Bool
    {
        public bool x, y, z;
        public Vector3Bool(bool x, bool y, bool z)
        {
            this.x = x; this.y = y; this.z = z;
        }
    }
    [Serializable]
    public struct FloatRange
    {
        [Range(float.NegativeInfinity, 0f)]
        public float min;
        [Range(0f, float.PositiveInfinity)]
        public float max;
        public FloatRange(float min, float max)
        {
            this.min = min; this.max = max;
        }
    }
    public class Grabbable : ZPointerInteractable, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        //#region Public Properties

        public event Action<PointerEventData> OnPointerEnterEvent;
        public event Action<PointerEventData> OnBeginDragEvent;
        public event Action<PointerEventData> OnDragEvent;
        public event Action<PointerEventData> OnEndDragEvent;
        public event Action<PointerEventData> OnPointerExitEvent;
        public event Action OnOrientationReset;
        public bool IsHovered => _isHovered;
        public bool IsPsudoHovered => _isPsudoHovered;
        public bool IsDragging => _isDragging;

        //#region  General Settings
        public GrabMode stylusGrabMode = GrabMode.PositionAndRotation;
        public GrabMode mouseGrabMode = GrabMode.PositionOnly;
        public bool dragOnPlane = false;

        public Transform planeQuadTransform;
        public bool allowsDragging => stylusGrabMode != GrabMode.RotationOnly || mouseGrabMode != GrabMode.RotationOnly;
        public bool allowsRotation => stylusGrabMode != GrabMode.PositionOnly || mouseGrabMode != GrabMode.PositionOnly;
        //#endregion

        //#region Drag Settings
        public Vector3Bool allowDrag = new Vector3Bool(true, true, true);
        public ContstraintType dragConstraintType = ContstraintType.None;
        public float maxDragDistance = 1f;
        public Vector3Bool constrainDrag = new Vector3Bool(false, false, false);
        public FloatRange xDragRange = new FloatRange(-1f, 1f);
        public FloatRange yDragRange = new FloatRange(-1f, 1f);
        public FloatRange zDragRange = new FloatRange(-1f, 1f);
        //#endregion


        //#region Rotation Settings
        public StylusRotationMode stylusRotationMode = StylusRotationMode.MatchStylus;
        public RotationMode rotationMode = RotationMode.Free;
        public RotationalAxis primaryAxis = RotationalAxis.X;
        public Vector3 PrimaryWorldAxis => ValidateMagnitueTheshold(primaryAxis == RotationalAxis.Z ? transform.forward : primaryAxis == RotationalAxis.Y ? transform.up : transform.right);
        public RotationalAxis secondaryAxis = RotationalAxis.Z;
        public Vector3 SecondaryWorldAxis => ValidateMagnitueTheshold(secondaryAxis == RotationalAxis.Z ? transform.forward : secondaryAxis == RotationalAxis.Y ? transform.up : transform.right);
        [Range(0, 180)]
        public float maxRotationAngle = 180f;
        public Vector3Bool constrainRotation = new Vector3Bool(false, false, false);
        public FloatRange xAxisRange = new FloatRange(-45f, 45f);
        public FloatRange yAxisRange = new FloatRange(-45f, 45f);
        public FloatRange zAxisRange = new FloatRange(-45f, 45f);
        //#endregion

        //#endregion

        //#region Private Properties
        [SerializeField] protected bool _isHovered = false;
        [SerializeField] protected bool _isDragging = false;
        [SerializeField] protected bool _isPsudoHovered = false;
        [SerializeField] protected bool _isKinematic = false;
        // [SerializeField] protected Vector3 _initialPosition = Vector3.zero;
        [SerializeField] protected Vector3 _initialLocalPosition = Vector3.zero;
        // [SerializeField] protected Quaternion _initialRotation = Quaternion.identity;
        [SerializeField] protected Quaternion _initialLocalRotation = Quaternion.identity;
        [SerializeField] protected Vector3 _initialGrabPosition = Vector3.zero;
        [SerializeField] protected Vector3 _initialGrabOffset = Vector3.zero;
        [SerializeField] protected Vector2 _initialMousePosition = Vector2.zero;
        [SerializeField] protected Quaternion _initialGrabRotation = Quaternion.identity;
        [SerializeField] protected Quaternion _initialGrabLocalRotation = Quaternion.identity;
        [SerializeField] protected Vector3 _initialGrabPrimaryAxis = Vector3.right;
        //#endregion

        [SerializeField] bool _reset = false;
        [SerializeField] bool _resetDefaults = false;
        [SerializeField] private Grabbable[] _childGrabbables;
        private Grabbable hoveredChild => _childGrabbables != null ? Array.Find(_childGrabbables, g => g.IsHovered) : null;
        [SerializeField] private Grabbable[] _parentGrabbables;
        private Grabbable[] hoveredParents => _parentGrabbables != null ? Array.FindAll(_parentGrabbables, g => g.IsHovered || g.IsPsudoHovered) : null;

        //#region Public Methods

        void Start()
        {
            _childGrabbables = GetComponentsInChildren<Grabbable>();
            _childGrabbables = Array.FindAll(_childGrabbables, g => g != this);
            _parentGrabbables = GetComponentsInParent<Grabbable>();
            _parentGrabbables = Array.FindAll(_parentGrabbables, g => g != this);
            ResetInitialPositionAndRotation();
        }
        public void Update()
        {
            if (_reset)
            {
                ResetOrientation();
                _reset = false;
            }
            if (_resetDefaults)
            {
                ResetInitialPositionAndRotation();
                _resetDefaults = false;
            }
        }

        public void ResetInitialPositionAndRotation()
        {
            _initialLocalPosition = transform.localPosition;
            _initialLocalRotation = transform.localRotation;
        }

        public override ZPointer.DragPolicy GetDragPolicy(ZPointer pointer)
        {
            if (dragOnPlane)
            {
                return ZPointer.DragPolicy.LockToCustomPlane;
            }

            if (pointer is ZMouse)
            {
                return ZPointer.DragPolicy.LockToScreenAlignedPlane;
            }

            if (pointer is ZStylus)
            {
                return ZPointer.DragPolicy.LockHitPosition;
            }

            return base.GetDragPolicy(pointer);
        }

        public override Plane GetDragPlane(ZPointer pointer)
        {
            if (dragOnPlane && planeQuadTransform != null)
            {
                return new Plane(
                    planeQuadTransform.forward,
                    planeQuadTransform.position);
            }

            return base.GetDragPlane(pointer);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _isPsudoHovered = true;
            if (hoveredChild != null)
            {
                OnPointerExit(eventData, false);
                return;
            }

            if (hoveredParents != null)
            {
                foreach (var parent in hoveredParents)
                    parent.OnPointerExit(eventData, false);
            }

            _isHovered = true;
            OnPointerEnterEvent?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnPointerExit(eventData, true);
        }
        public void OnPointerExit(PointerEventData eventData, bool setPsudoHoveredState = true)
        {
            var _hoveredParents = hoveredParents;

            if (setPsudoHoveredState)
                _isPsudoHovered = false;

            _isHovered = false;
            OnPointerExitEvent?.Invoke(eventData);

            // Note: If any parent is hovered, call its OnPointerEnter to restore its hover state.
            // this needs to be done after setting _isHovered to false so the parent knows its child is no longer hovered.
            if (_hoveredParents != null)
            {
                foreach (var parent in _hoveredParents)
                    parent.OnPointerEnter(eventData);
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null || (pointerEventData.button != PointerEventData.InputButton.Left))
                return;


            // Capture pointer events.
            pointerEventData.Pointer.CapturePointer(gameObject);

            Pose pose = pointerEventData.Pointer.EndPointWorldPose;

            // Cache the initial grab position.
            _initialGrabPosition = pose.position;

            // Cache the initial grab state.
            _initialGrabOffset =
                Quaternion.Inverse(transform.rotation) *
                (transform.position - pose.position);

            _initialGrabRotation =
                Quaternion.Inverse(pose.rotation) *
                transform.rotation;

            var _grabMode = stylusGrabMode;
            if (pointerEventData.Pointer is ZMouse)
            {
                _grabMode = mouseGrabMode;
            }

            if (_grabMode == GrabMode.RotationOnly || stylusRotationMode == StylusRotationMode.DragRotation)
                _initialGrabRotation = transform.rotation;

            _initialGrabLocalRotation = transform.localRotation; // ToSigned(transform.localEulerAngles) - _initialLocalRotation;
            _initialGrabPrimaryAxis = PrimaryWorldAxis;
            _initialMousePosition = Input.mousePosition;


            // If the grabbable object has a rigidbody component,
            // mark it as kinematic during the grab.
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                _isKinematic = rigidbody.isKinematic;
                rigidbody.isKinematic = true;
            }


            _isDragging = true;

            // Notify listeners that the drag has begun
            OnBeginDragEvent?.Invoke(pointerEventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_isDragging) return;

            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null || (pointerEventData.button != PointerEventData.InputButton.Left))
                return;

            Pose pose = pointerEventData.Pointer.EndPointWorldPose;
            var _grabMode = stylusGrabMode;

            if (pointerEventData.Pointer is ZMouse)
            {
                _grabMode = mouseGrabMode;
            }

            if (_grabMode == GrabMode.PositionAndRotation)
            {
                if (pointerEventData.Pointer is ZStylus)
                {
                    if (stylusRotationMode == StylusRotationMode.MatchStylus)
                        UpdateRotation_StylusDefault(pose);
                    else if (stylusRotationMode == StylusRotationMode.DragRotation)
                        UpdateRotation_StylusDrag(pose);

                    UpdatePosition_Default(pose);
                }
                else if (pointerEventData.Pointer is ZMouse)
                {
                    UpdateRotation_MouseDefault();
                    UpdatePosition_Default(pose);
                }
            }
            else if (_grabMode == GrabMode.PositionOnly)
            {
                if (pointerEventData.Pointer is ZStylus)
                {
                    UpdatePosition_Default(pose);
                }
                else if (pointerEventData.Pointer is ZMouse)
                {
                    UpdatePosition_Default(pose);
                }
            }
            else if (_grabMode == GrabMode.RotationOnly)
            {
                if (pointerEventData.Pointer is ZStylus)
                {
                    UpdateRotation_StylusDrag(pose);
                }
                else if (pointerEventData.Pointer is ZMouse)
                {
                    UpdateRotation_MouseDefault();
                }
            }

            // Notify listeners that the drag is in progress.
            OnDragEvent?.Invoke(eventData);

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ZPointerEventData pointerEventData = eventData as ZPointerEventData;
            if (pointerEventData == null || (pointerEventData.button != PointerEventData.InputButton.Left))
                return;

            // Release the pointer.
            pointerEventData.Pointer.CapturePointer(null);

            // If the grabbable object has a rigidbody component,
            // restore its original isKinematic state.
            var rigidbody = GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = _isKinematic;
            }

            _isDragging = false;

            // Notify listeners that the drag has ended.
            OnEndDragEvent?.Invoke(eventData);

        }

        public void ResetOrientation()
        {
            transform.localPosition = _initialLocalPosition;
            transform.localRotation = _initialLocalRotation;
            OnOrientationReset?.Invoke();
        }

        //#endregion

        //#region Private Members


        // Rotational Update Functions
        private void ApplyRotationWithConstraints(Vector3 p0, Vector3 p1)
        {
            if (rotationMode == RotationMode.Free)
            {
                // Vectors from the rotation center to the two hits.
                Vector3 v0 = p0 - transform.position;
                Vector3 v1 = p1 - transform.position;

                float m0 = v0.magnitude;
                float m1 = v1.magnitude;
                if (m0 < 1e-5f || m1 < 1e-5f) return;

                v0 /= m0; v1 /= m1;

                // Minimal rotation that takes v0 to v1 (in world space).
                Quaternion delta = Quaternion.FromToRotation(v0, v1);

                ApplyRotationWithConstraints(delta);
            }
            else if (rotationMode == RotationMode.SingleAxis)
            {
                Vector3 axis = _initialGrabPrimaryAxis;

                // Vectors from pivot to the two points
                var pivot = transform.position;
                Vector3 a = p0 - pivot;
                Vector3 b = p1 - pivot;

                // Project onto plane perpendicular to axis (so we only measure rotation around that axis)
                Vector3 aProj = Vector3.ProjectOnPlane(a, axis);
                Vector3 bProj = Vector3.ProjectOnPlane(b, axis);

                if (aProj.sqrMagnitude < 1e-5f || bProj.sqrMagnitude < 1e-5f)
                    return;

                var dA = Vector3.SignedAngle(aProj, bProj, axis);
                Quaternion delta = Quaternion.AngleAxis(dA, axis);
                ApplyRotationWithConstraints(delta);
            }
        }
        private void ApplyRotationWithConstraints(Quaternion rotationDelta)
        {
            // Note: we are calculating the world space rotation from the initial local rotation
            // in case the parent has rotated
            var initialWorldRotation = LocalOrientationToWorld(transform, _initialLocalRotation); ;
            if (rotationMode == RotationMode.Free)
            {
                var newRotation = rotationDelta * _initialGrabRotation;
                newRotation = Quaternion.RotateTowards(initialWorldRotation, newRotation, maxRotationAngle);
                transform.rotation = newRotation;
            }
            else if (rotationMode == RotationMode.SingleAxis)
            {
                // get the change in rotation from the initial rotation
                // Note: this user world space rotation delta, not local space
                var qDelta = Quaternion.Normalize(rotationDelta * _initialGrabRotation * Quaternion.Inverse(initialWorldRotation));
                var v = new Vector3(qDelta.x, qDelta.y, qDelta.z);
                var w = qDelta.w;

                // get the change in angle around the axis
                float angleRotated = 2f * Mathf.Atan2(Vector3.Dot(v, _initialGrabPrimaryAxis), w) * Mathf.Rad2Deg;
                angleRotated = Mathf.DeltaAngle(0f, angleRotated);

                // clamp the maximum rotation angle from the initial rotation
                if (primaryAxis == RotationalAxis.X && constrainRotation.x)
                    angleRotated = Mathf.Clamp(angleRotated, xAxisRange.min, xAxisRange.max);
                else if (primaryAxis == RotationalAxis.Y && constrainRotation.y)
                    angleRotated = Mathf.Clamp(angleRotated, yAxisRange.min, yAxisRange.max);
                else if (primaryAxis == RotationalAxis.Z && constrainRotation.z)
                    angleRotated = Mathf.Clamp(angleRotated, zAxisRange.min, zAxisRange.max);

                // get the local rotation axis
                Vector3 axis = Vector3.right;
                if (primaryAxis == RotationalAxis.Y) axis = Vector3.up;
                else if (primaryAxis == RotationalAxis.Z) axis = Vector3.forward;

                // apply the rotation around the axis to the initial local rotation
                var newRotation = Quaternion.AngleAxis(angleRotated, axis) * _initialLocalRotation;
                transform.localRotation = newRotation;
            }

            // TODO: Need to apply two axis rotation. The code below was generated by AI and is untested
            // else if (rotationMode == RotationMode.TwoAxis)
            // {
            //     Vector3 primary = transform.right;
            //     Vector3 secondary = transform.up;
            //     if (primaryAxis == RotationalAxis.Y) primary = transform.up;
            //     else if (primaryAxis == RotationalAxis.Z) primary = transform.forward;

            //     if (secondaryAxis == RotationalAxis.X) secondary = transform.right;
            //     else if (secondaryAxis == RotationalAxis.Y) secondary = transform.up;
            //     else if (secondaryAxis == RotationalAxis.Z) secondary = transform.forward;

            //     // Project the rotationDelta onto the two chosen axes
            //     rotationDelta.ToAngleAxis(out float angle, out Vector3 rotAxis);
            //     float primaryAngle = Vector3.Dot(rotAxis, primary) * angle;
            //     float secondaryAngle = Vector3.Dot(rotAxis, secondary) * angle;

            //     Quaternion primaryRotation = Quaternion.AngleAxis(primaryAngle, primary);
            //     Quaternion secondaryRotation = Quaternion.AngleAxis(secondaryAngle, secondary);

            //     var newRotation = secondaryRotation * primaryRotation * _initialGrabRotation;
            //     newRotation = Quaternion.RotateTowards(_initialRotation, newRotation, maxRotationAngle);
            //     transform.rotation = newRotation;
            // }

        }
        private void UpdateRotation_StylusDefault(Pose pose)
        {
            ApplyRotationWithConstraints(pose.rotation);
        }
        private void UpdateRotation_StylusDrag(Pose pose)
        {
            // Get the starting and ending positions
            Vector3 p0 = _initialGrabPosition;
            Vector3 p1 = pose.position;

            // Debug.DrawRay(_initialGrabPosition, tiltedNormal.normalized * 0.25f, Color.green, 0f, false);
            Debug.DrawLine(transform.position, p0, Color.cyan, 0f, false);
            Debug.DrawLine(transform.position, p1, Color.cyan, 0f, false);

            Debug.DrawLine(transform.position, transform.position + transform.right, Color.red, 0f, false);
            Debug.DrawLine(transform.position, transform.position + transform.up, Color.green, 0f, false);
            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, 0f, false);

            ApplyRotationWithConstraints(p0, p1);
        }
        private void UpdateRotation_MouseDefault()
        {
            if (Camera.main == null) return;
            var cam = Camera.main;

            Plane dragPlane;
            if (dragOnPlane && planeQuadTransform != null)
            {
                dragPlane = new Plane(planeQuadTransform.forward, _initialGrabPosition);
            }
            else
            {
                // Choose the axis to rotate the screen normal around.
                Vector3 axis = cam.transform.right;

                // Build the tilted plane (rotate camera.forward by angleDeg around 'axis').
                Vector3 tiltedNormal = Quaternion.AngleAxis(45f, axis) * cam.transform.forward;
                dragPlane = new Plane(tiltedNormal, _initialGrabPosition);
            }

            // Rays from initial and current screen positions.
            var mouseDelta = (Vector2)Input.mousePosition - _initialMousePosition;
            Ray r0 = cam.ScreenPointToRay(_initialMousePosition);
            Ray r1 = cam.ScreenPointToRay(_initialMousePosition + mouseDelta);

            // Intersections with the plane.
            if (!dragPlane.Raycast(r0, out float t0)) return; // parallel edge case
            if (!dragPlane.Raycast(r1, out float t1)) return;

            // Get the starting and ending positions
            Vector3 p0 = r0.GetPoint(t0);
            Vector3 p1 = r1.GetPoint(t1);

            // Debug.DrawRay(_initialGrabPosition, tiltedNormal.normalized * 0.25f, Color.green, 0f, false);
            Debug.DrawLine(transform.position, p0, Color.cyan, 0f, false);
            Debug.DrawLine(transform.position, p1, Color.cyan, 0f, false);

            Debug.DrawLine(transform.position, transform.position + transform.right, Color.red, 0f, false);
            Debug.DrawLine(transform.position, transform.position + transform.up, Color.green, 0f, false);
            Debug.DrawLine(transform.position, transform.position + transform.forward, Color.blue, 0f, false);

            ApplyRotationWithConstraints(p0, p1);
        }

        // Positional Update Functions
        private void ApplyTranslationWithConstraints(Vector3 newPosition)
        {
            // Note: we are calculating the world space position from the initial local position
            // in case the parent has moved
            var initialWorldPosition = transform.parent != null ? transform.parent.TransformPoint(_initialLocalPosition) : _initialLocalPosition;

            // get the delta from initial grab position
            var positionDelta = newPosition - initialWorldPosition;

            // get the change in x, y, and z in the transform's local space
            Vector3 ax = transform.right;   // local +X in world space
            Vector3 ay = transform.up;      // local +Y in world space
            Vector3 az = transform.forward; // local +Z in world space
            var dx = Vector3.Dot(positionDelta, ax);
            var dy = Vector3.Dot(positionDelta, ay);
            var dz = Vector3.Dot(positionDelta, az);

            // constrain allowed local movements
            if (!allowDrag.x) dx = 0; //localPositionDelta.x = 0;
            if (!allowDrag.y) dy = 0; //localPositionDelta.y = 0;
            if (!allowDrag.z) dz = 0; //localPositionDelta.z = 0;
            Vector3 constrainedDelta = ax * dx + ay * dy + az * dz;

            // apply distance constraints
            if (dragConstraintType == ContstraintType.Rectangular)
            {
                // Clamp per axis relative to the origin
                if (constrainDrag.x)
                    dx = Mathf.Clamp(dx, xDragRange.min, xDragRange.max);
                if (constrainDrag.y)
                    dy = Mathf.Clamp(dy, yDragRange.min, yDragRange.max);
                if (constrainDrag.z)
                    dz = Mathf.Clamp(dz, zDragRange.min, zDragRange.max);

                // Final clamped position
                constrainedDelta = ax * dx + ay * dy + az * dz;
                newPosition = initialWorldPosition + constrainedDelta;
            }
            else if (dragConstraintType == ContstraintType.Spherical)
            {
                if (constrainedDelta.magnitude > maxDragDistance)
                {
                    var direction = constrainedDelta.normalized;
                    newPosition = initialWorldPosition + direction * maxDragDistance;
                }
            }
            else
            {
                // No constraint, but still apply axis constraints
                newPosition = initialWorldPosition + constrainedDelta;
            }

            transform.position = newPosition;
        }

        private void UpdatePosition_Default(Pose pose)
        {
            var newPosition = pose.position + (transform.rotation * _initialGrabOffset);
            ApplyTranslationWithConstraints(newPosition);
        }

        //#endregion

        //#region Private Helper Functions
        // Convert a local orientation (relative to parent) to world orientation
        public static Quaternion LocalOrientationToWorld(Transform t, Quaternion localOrientation)
        {
            var parentWorld = t.parent ? t.parent.rotation : Quaternion.identity;
            return parentWorld * localOrientation;
        }

        static Vector3 ToSigned(Vector3 e) => new Vector3(ToSigned(e.x), ToSigned(e.y), ToSigned(e.z));
        static float ToSigned(float a) => ValidateMagnitueTheshold(Mathf.Repeat(a + 180f, 360f) - 180f);
        static Vector3 ValidateMagnitueTheshold(Vector3 e) => new Vector3(ValidateMagnitueTheshold(e.x), ValidateMagnitueTheshold(e.y), ValidateMagnitueTheshold(e.z));
        static float ValidateMagnitueTheshold(float value, float threshold = 1e-3f)
        {
            return Mathf.Abs(value) < threshold ? 0f : value;
        }
        private void OnDrawGizmos()
        {
            // Draw magenta dot at the center
            Gizmos.color = Color.cyan * new Color(1f, 1f, 1f, 0.5f);
            Gizmos.DrawSphere(transform.position, 0.025f);

            // Draw red line in the positive X direction (local)
            Gizmos.color = Color.red * new Color(1f, 1f, 1f, 0.5f);
            Vector3 start = transform.position;
            Vector3 endX = transform.position + transform.right * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endX);

            // Draw blue line in the positive Z direction (local)
            Gizmos.color = Color.blue * new Color(1f, 1f, 1f, 0.5f);
            Vector3 endZ = start + transform.forward * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endZ);

            // Draw blue line in the positive Y direction (local)
            Gizmos.color = Color.green * new Color(1f, 1f, 1f, 0.5f);
            Vector3 endY = start + transform.up * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endY);
        }
        private void OnDrawGizmosSelected()
        {
            // Draw magenta dot at the center
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(transform.position, 0.025f);

            // Draw red line in the positive X direction (local)
            Gizmos.color = Color.red;
            Vector3 start = transform.position;
            Vector3 endX = transform.position + transform.right * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endX);

            // Draw blue line in the positive Z direction (local)
            Gizmos.color = Color.blue;
            Vector3 endZ = start + transform.forward * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endZ);

            // Draw blue line in the positive Y direction (local)
            Gizmos.color = Color.green;
            Vector3 endY = start + transform.up * 0.2f; // short line (0.2 units)
            Gizmos.DrawLine(start, endY);
        }

        //#endregion
    }
}