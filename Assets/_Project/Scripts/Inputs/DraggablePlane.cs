using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using zSpace.Core.Samples;
using zSpace.Core.Input;
using zSpace.Core.EventSystems;
using zSpace.Core;

public class DraggablePlane : ZPointerInteractable, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    ////////////////////////////////////////////////////////////////////////
    // Inspector Fields
    ////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// A transform from which to base planar movement on. Forward (Z) is
    /// normal to the plane, and the object will drag along its X and Y
    /// coordinates.
    /// </summary>
    [Tooltip(
        "A transform from which to base planar movement on. Forward (Z) " +
        "is normal to the plane, and the object will drag along its X " +
        "and Y coordinates.")]
    public Transform PlaneQuadTransform;

    [Header("Zoom Settings")]
    [Tooltip("How far the plane can be zoomed in (in meters, relative to start position)")]
    public float maxZoomInDistance = 1.0f;

    [Header("Interaction Settings")]
    [Tooltip("If true, moving the cursor will zoom the plane instead of moving it normally.")]
    public bool cursorMovesZoom = false;

    [Header("Movement Bounds")]
    public float minX = 0f, maxX = 0f;
    public float minY = 0f, maxY = 0f;

    private float _scrollAccumulator = 0.0f;

    ////////////////////////////////////////////////////////////////////////
    // Private Methods
    ////////////////////////////////////////////////////////////////////////


    private void OnDrawGizmosSelected()
    {
        if (PlaneQuadTransform == null) return;

        Vector3 p1 = PlaneQuadTransform.TransformPoint(new Vector3(minX, minY, 0));
        Vector3 p2 = PlaneQuadTransform.TransformPoint(new Vector3(maxX, minY, 0));
        Vector3 p3 = PlaneQuadTransform.TransformPoint(new Vector3(maxX, maxY, 0));
        Vector3 p4 = PlaneQuadTransform.TransformPoint(new Vector3(minX, maxY, 0));

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(p1, p2);
        Gizmos.DrawLine(p2, p3);
        Gizmos.DrawLine(p3, p4);
        Gizmos.DrawLine(p4, p1);
    }

    private void Awake()
    {
        // Start a coroutine to apply to children after a short delay
        // This gives zSpace time to initialize properly
        StartCoroutine(DelayedApplyToChildren());

        _startPosition = transform.position;
        _zoomInitialized = true;

        _scrollAccumulator = maxZoomInDistance;
    }

    private IEnumerator DelayedApplyToChildren()
    {
        // Wait for a few frames to ensure zSpace has time to initialize
        yield return new WaitForSeconds(0.1f);

        // Try to apply to children, but don't fail if zSpace isn't initialized
        try
        {
            ApplyToChildren();
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Failed to apply DraggablePlane to children: " + e.Message);
        }
    }

    private void ApplyToChildren()
    {
        var children = GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child == transform)
            {
                continue;
            }

            if (child.GetComponent<DraggablePlane>() != null)
            {
                continue;
            }

            var childDraggable = child.gameObject.AddComponent<ChildDraggablePlane>();
            // childDraggable.PlaneQuadTransform = PlaneQuadTransform;
            childDraggable.Initialize(this);
        }
    }
    ////////////////////////////////////////////////////////////////////////
    // Public Methods
    ////////////////////////////////////////////////////////////////////////

    public override ZPointer.DragPolicy GetDragPolicy(ZPointer pointer)
    {
        return ZPointer.DragPolicy.LockToCustomPlane;
    }

    public override Plane GetDragPlane(ZPointer pointer)
    {
        if (PlaneQuadTransform != null)
        {
            return new Plane(
                PlaneQuadTransform.forward,
                PlaneQuadTransform.position);
        }

        return base.GetDragPlane(pointer);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        ZPointerEventData pointerEventData = eventData as ZPointerEventData;
        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Pose pose = pointerEventData.Pointer.EndPointWorldPose;

        // Cache the initial grab state.
        this._initialGrabOffset =
            Quaternion.Inverse(this.transform.rotation) *
            (this.transform.position - pose.position);

        // If the grabbable object has a rigidbody component,
        // mark it as kinematic during the grab.
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            this._isKinematic = rigidbody.isKinematic;
            rigidbody.isKinematic = true;
        }

        // Capture pointer events.
        pointerEventData.Pointer.CapturePointer(this.gameObject);
    }

    public void OnDrag(PointerEventData eventData)
    {

        // if (cursorMovesZoom)
        // {
        //     // Zoom logic: move plane along its normal based on vertical mouse movement (screen space)
        //     Camera cam = Camera.main;
        //     if (cam == null) return;
        //     // Use eventData.delta.y: positive when moving up, negative when moving down
        //     float scrollDelta = eventData.delta.y;
        //     // Reversed sign: moving down brings plane closer to camera
        //     float zoomStep = 0.01f * scrollDelta; // Adjust sensitivity as needed
        //     Vector3 camToPlane = (transform.position - cam.transform.position).normalized;
        //     float currentDistance = Vector3.Distance(transform.position, cam.transform.position);
        //     float startDistance = Vector3.Distance(_startPosition, cam.transform.position);
        //     float newDistanceFromStart = currentDistance + zoomStep;
        //     float minDistance = startDistance - maxZoomInDistance;
        //     float maxDistance = startDistance;
        //     float clampedDistance = Mathf.Clamp(newDistanceFromStart, minDistance, maxDistance);
        //     Vector3 newPosition = cam.transform.position + camToPlane * clampedDistance;
        //     transform.position = newPosition;
        // }
        // else
        // {
        //     // Normal plane movement
        //     this.transform.position =
        //         pose.position +
        //         (this.transform.rotation * this._initialGrabOffset);
        // }

        ZPointerEventData pointerEventData = eventData as ZPointerEventData;
        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        Pose pose = pointerEventData.Pointer.EndPointWorldPose;

        // Step 1: Compute desired position in world space
        Vector3 targetWorldPos = pose.position + (transform.rotation * _initialGrabOffset);

        // Step 2: Convert to local space of the movement plane
        Vector3 local = PlaneQuadTransform.InverseTransformPoint(targetWorldPos);

        // Step 3: Clamp in XY
        if (minX != 0 && maxX != 0)
        {
            // If minX and maxX are set, clamp the x coordinate
            local.x = Mathf.Clamp(local.x, minX, maxX);
        }
        if (minY != 0 && maxY != 0)
        {
            // If minY and maxY are set, clamp the y coordinate
            local.y = Mathf.Clamp(local.y, minY, maxY);
        }

        // Clamp the z to the plane's z position
        local.z = PlaneQuadTransform.localPosition.z; // Keep the z position constant

        // Step 4: Convert back to world space and apply
        Vector3 clampedWorldPos = PlaneQuadTransform.TransformPoint(local);
        transform.position = clampedWorldPos;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        ZPointerEventData pointerEventData = eventData as ZPointerEventData;
        if (pointerEventData == null ||
            pointerEventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        // Release the pointer.
        pointerEventData.Pointer.CapturePointer(null);

        // If the grabbable object has a rigidbody component,
        // restore its original isKinematic state.
        var rigidbody = this.GetComponent<Rigidbody>();
        if (rigidbody != null)
        {
            rigidbody.isKinematic = this._isKinematic;
        }
    }

    void OnDrawGizmos()
    {
        ZFrame zFrame = FindObjectOfType<ZFrame>();
        if (zFrame == null) return;

        Vector3 frameCenter = zFrame.WorldPosition;
        Vector3 frameNormal = zFrame.WorldRotation * Vector3.forward;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(frameCenter, frameCenter + frameNormal * 0.5f); // Adjust length as needed
    }

    // public void OnScroll(PointerEventData eventData)
    // {
    //     if (!_zoomInitialized) return;

    //     float scrollDelta = eventData.scrollDelta.y;
    //     if (Mathf.Approximately(scrollDelta, 0f)) return;

    //     Camera cam = Camera.main;
    //     if (cam == null) return;

    //     ZFrame zFrame = FindObjectOfType<ZFrame>();
    //     if (zFrame == null) return;

    //     Vector3 frameNormal = zFrame.WorldRotation * Vector3.forward;

    //     float zoomStep = 0.5f * -scrollDelta; // scroll up = zoom in (positive), scroll down = zoom out (negative)
    //     float proposedZoom = _scrollAccumulator + zoomStep;

    //     // Clamp the new zoom level
    //     float clampedZoom = Mathf.Clamp(proposedZoom, 0f, maxZoomInDistance);
    //     float actualZoomChange = clampedZoom - _scrollAccumulator;

    //     // No change? Skip applying
    //     if (Mathf.Approximately(actualZoomChange, 0f)) return;

    //     // Apply zoom
    //     transform.position += frameNormal * actualZoomChange;
    //     _scrollAccumulator = clampedZoom;
    // }

    public void ToggleZoomMode()
    {
        cursorMovesZoom = !cursorMovesZoom;
    }

    ////////////////////////////////////////////////////////////////////////
    // Private Members
    ////////////////////////////////////////////////////////////////////////

    protected Vector3 _initialGrabOffset = Vector3.zero;
    //private Quaternion _initialGrabRotation = Quaternion.identity;
    private bool _isKinematic = false;
    private Vector3 _startPosition;
    private bool _zoomInitialized = false;
}

/// <summary>
/// A component that delegates drag events to its parent's DraggablePlane component.
/// This allows child objects to be dragged, moving the entire parent.
/// </summary>
public class ChildDraggablePlane : ZPointerInteractable, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private DraggablePlane _parentDraggable;

    public void Initialize(DraggablePlane parent)
    {
        _parentDraggable = parent;
    }

    public override ZPointer.DragPolicy GetDragPolicy(ZPointer pointer)
    {
        return ZPointer.DragPolicy.LockToCustomPlane;
    }

    public override Plane GetDragPlane(ZPointer pointer)
    {
        if (_parentDraggable != null)
        {
            return _parentDraggable.GetDragPlane(pointer);
        }

        return base.GetDragPlane(pointer);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_parentDraggable != null)
        {
            _parentDraggable.OnBeginDrag(eventData);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_parentDraggable != null)
        {
            _parentDraggable.OnDrag(eventData);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_parentDraggable != null)
        {
            _parentDraggable.OnEndDrag(eventData);
        }
    }
}
