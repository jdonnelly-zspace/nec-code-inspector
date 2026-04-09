using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AppManagement;
using UnityEngine;
using zSpace.Core;
using zSpace.Core.Extensions;
using zSpace.Core.Input;
using zSpace.Core.Sdk;

[RequireComponent(typeof(LineRenderer))]
public class ObjectPointer : ZPointerVisualization
{
    public bool forceDefaultRotation = true;
    [Tooltip("Default rotation of the object when no surface is hit.")]
    public Vector3 defaultRotation = Vector3.zero;
    [Tooltip("List of all colliders (including child colliders) to check for overlap.")]
    public List<Collider> pointerColliders = new();
    [Tooltip("If true, the pointer will try to resolve collisions automatically by rotating the object.")]
    public bool autoResolveCollisions = true;
    [DrawIf(nameof(autoResolveCollisions), true)]
    [Tooltip("Rotation step in degrees to try when resolving collisions. Note: Step size may be increased at runtime to avoid performance issues.")]
    [Range(5f, 90f)]
    public float rotationStepDegrees = 15f;
    [Tooltip("Offsets to apply when resolving hits on colliders.")]
    public List<ContactOffsets> contactOffsets = new List<ContactOffsets>();
    public List<Collider> ignoredColliders = new List<Collider>();
    public List<Collider> ignoredForAutoResolveColliders = new List<Collider>();
    public int ToolID = -1;


    public bool showStylusBeam = true;
    public Material lineMaterial;
    public float stylusLineStartWidth = 0.02f;
    public float stylusLineEndWidth = 0.01f;
    public Color stylusLineStartColor = new Color32(25, 121, 255, 200);
    public Color stylusLineEndColor = new Color32(25, 121, 255, 0);

    public Collider HitTargetCollider { get => hitTargetCollider; }
    [SerializeField] private Collider hitTargetCollider;
    private Dictionary<ZPointer, ZPointerVisualization> originalVisualizations = new Dictionary<ZPointer, ZPointerVisualization>();

    private ZTarget stylusTarget;
    [SerializeField] private float vibrationIntensity = 0.1f;
    [SerializeField] private VibrationTypeEnum vibrationType = VibrationTypeEnum.FastPulse;
    private Dictionary<GameObject, int> layerMap = new();
    [SerializeField] private List<Collider> potentialCollidingObjects = new();

    private Quaternion[] testAngles = new Quaternion[0];
    private Vector3 lastValidPosition;
    private Dictionary<Collider, ContactOffsets> contactOffsetMap = new Dictionary<Collider, ContactOffsets>();
    private Dictionary<Collider, ObjectPointable> objectPointableMap = new Dictionary<Collider, ObjectPointable>();
    private LayerMask pointerObjectMask = 0;
    public Bounds pointerBounds;
    public bool IsTemporarilyDisconnected => isTemporarilyDisconnected;
    private bool isTemporarilyDisconnected = false;
    private Renderer[] renderersToTurnOn;
    private GameObject hitUIObject;
    private LineRenderer lr;
    private Quaternion lastDesiredRotation = Quaternion.identity;
    ZCameraRig zCameraRig;

    #region Enumerators
    public enum VibrationTypeEnum
    {
        Constant,
        FastPulse,
        MediumPulse,
        SlowPulse
    }
    #endregion



    protected void Start()
    {
        lastValidPosition = transform.position;
        zCameraRig = Camera.main.GetComponentInParent<ZCameraRig>();
        GetLineRenderer();
        GetStylusTarget();
        AttachCollisionDetector();
        pointerBounds = GetTotalBounds();

        if (autoResolveCollisions)
        {
            testAngles = GenerateSpiralEulerAngles(rotationStepDegrees, 180f);
            Debug.Log($"Test angles generated: {testAngles.Length} on ObjectPointer ({name})");
        }

        foreach (var collider in contactOffsets.SelectMany(co => co.colliders).Distinct())
        {
            if (collider != null)
            {
                potentialCollidingObjects.Add(collider);
            }
        }
        foreach (var contactOffset in contactOffsets)
        {
            foreach (var collider in contactOffset.colliders)
            {
                contactOffsetMap[collider] = contactOffset;
            }
        }
        pointerObjectMask = ~LayerMask.GetMask("PointerObject");
    }
    void OnDisable()
    {
        // detach from all pointers if this object is disabled
        DetachAllPointers();
    }

    void OnDestroy()
    {
        DetachAllPointers();
    }

    public void AttachToPointer(ZPointer pointer)
    {
        if (pointer == null || pointer.Visualization == this)
            return;

        // Store the original visualization if it exists
        if (pointer.Visualization != null)
        {
            ZPointerVisualization originalVisualization = pointer.Visualization;
            // if the pointer's visualization is an instance of an ObjectPointer,
            // detach it from the pointer and then get the other original visualization
            if (originalVisualization is ObjectPointer objectPointer)
            {
                objectPointer.DetachFromPointer(pointer);
                originalVisualization = pointer.Visualization;
            }


            // store the original visualization of the object
            if (originalVisualization != null)
            {
                if (originalVisualizations.ContainsKey(pointer))
                {
                    originalVisualizations[pointer] = originalVisualization;
                }
                else
                {
                    originalVisualizations.Add(pointer, originalVisualization);
                }
                originalVisualization.gameObject.SetActive(false);
            }
        }

        pointer.Visualization = this;
        SetPointerObjectLayer();

        // add the PointerObject layer to the pointer's ignore mask
        pointer.IgnoreMask |= LayerMask.GetMask("PointerObject");
    }

    public void DetachFromPointer(ZPointer pointer)
    {
        if (pointer == null || pointer.Visualization != this)
            return;

        // Restore the original visualization if it exists
        if (originalVisualizations.TryGetValue(pointer, out ZPointerVisualization originalVisualization))
        {
            pointer.Visualization = originalVisualization;
            originalVisualization.gameObject.SetActive(true);
            originalVisualizations.Remove(pointer);
        }
        else
        {
            // If no original visualization is stored, set the pointer's visualization to null
            pointer.Visualization = null;
        }
        ResetLayers();
    }

    public void DetachAllPointers()
    {
        List<ZPointer> pointers = originalVisualizations.Keys.ToList();
        foreach (ZPointer pointer in pointers)
        {
            DetachFromPointer(pointer);
        }
    }

    public override void Process(ZPointer pointer, Vector3 worldScale)
    {
        if (ZPointerMonitor.Instance == null)
        {
            AppLogger.LogFatalError("ZPointerMonitor instance is null. Cannot process pointer.");
            return;
        }

        if (pointer != ZPointerMonitor.Instance.ActivePointer)
        {
            return;
        }
        base.Process(pointer, worldScale);

        // rotate the object to be opposite to any surface it is pointing at
        ReorientToPointer(pointer);

        if (showStylusBeam && pointer is ZStylus)
        {
            var zFrame = zCameraRig.Frame;
            float viewerScale = zFrame != null ? zFrame.ViewerScale : 15f;

            float baseUserScale = 15f;
            float scaleMultiplier = viewerScale / baseUserScale;

            lr.enabled = !isTemporarilyDisconnected;
            lr.startWidth = stylusLineStartWidth * scaleMultiplier;
            lr.endWidth = stylusLineEndWidth * scaleMultiplier;
            lr.startColor = stylusLineStartColor;
            lr.endColor = stylusLineEndColor;

            lr.SetPosition(0, pointer.PointerRay.origin);

            Vector3 endPosition = pointer.EndPointWorldPose.position;
            lr.SetPosition(1, endPosition);
        }
        else
        {
            lr.enabled = false;
        }
    }



    private void ReorientToPointer(ZPointer pointer)
    {
        // Update the pointer's position and rotation.
        var targetPos = pointer.EndPointWorldPose.position;
        var targetRot = pointer.EndPointWorldPose.rotation;

        // cast a ray to keep the object upright
        int prioityLayerMask = ~pointer.IgnoreMask & pointerObjectMask & (pointer.PriorityMask != 0 ? pointer.PriorityMask : ~pointer.PriorityMask);
        int defaultLayerMask = ~pointer.IgnoreMask & pointerObjectMask;
        float worldScaleFactor = this.GetWorldScale(pointer);
        RaycastHit hit;
        var hadHit = Physics.Raycast(pointer.PointerRay.origin, pointer.PointerRay.direction, out hit, pointer.MaxHitDistance * worldScaleFactor, prioityLayerMask) && !ignoredColliders.Contains(hit.collider);
        if (!hadHit && prioityLayerMask != defaultLayerMask)
        {
            hadHit = Physics.Raycast(pointer.PointerRay.origin, pointer.PointerRay.direction, out hit, pointer.MaxHitDistance * worldScaleFactor, defaultLayerMask) && !ignoredColliders.Contains(hit.collider);
        }



        if (hadHit)
        {
            Quaternion rotation = Quaternion.LookRotation(-hit.normal, Vector3.up);
            targetRot = rotation;

            targetPos = hit.point;
            hitTargetCollider = hit.collider;

            if (pointer.HitInfo.gameObject != null && pointer.HitInfo.gameObject != hit.collider.gameObject)
            {
                // check to see if we hit a UI object instead of the expected object
                var rectTransform = pointer.HitInfo.gameObject.GetComponentInParent<RectTransform>();
                if (rectTransform != null)
                {
                    hitUIObject = pointer.HitInfo.gameObject;
                    ShowOriginalVisualization();
                    return;
                }

            }

            if (contactOffsetMap.ContainsKey(hitTargetCollider))
            {
                var contactOffset = contactOffsetMap[hitTargetCollider];
                if (contactOffset != null)
                {
                    if (contactOffset.shouldOverrideAngle)
                    {
                        targetRot = Quaternion.Euler(contactOffset.overrideAngle);
                    }
                    else if (contactOffset.shouldOffsetAngle)
                    {
                        targetRot *= Quaternion.Euler(contactOffset.offsetAngle);
                    }

                    if (contactOffset.shouldOffsetPosition)
                    {
                        targetPos += contactOffset.offsetPosition;
                    }
                }
            }

            if (!objectPointableMap.ContainsKey(hitTargetCollider))
            {
                var op = hitTargetCollider.GetComponentInChildren<ObjectPointable>();
                if (op == null) op = hitTargetCollider.GetComponentInParent<ObjectPointable>();
                objectPointableMap[hitTargetCollider] = op;
            }

            var ObjectPointable = objectPointableMap[hitTargetCollider];
            if (ObjectPointable != null)
            {
                if (ObjectPointable.shouldOverrideAngle)
                {
                    targetRot = Quaternion.Euler(ObjectPointable.overrideAngle);
                }
                else if (ObjectPointable.shouldOffsetAngle)
                {
                    targetRot *= Quaternion.Euler(ObjectPointable.offsetAngle);
                }

                if (ObjectPointable.shouldOffsetPosition)
                {
                    targetPos += ObjectPointable.offsetPosition;
                }
            }
        }
        else if (pointer.HitInfo.gameObject)
        {
            var rectTransform = pointer.HitInfo.gameObject.GetComponentInParent<RectTransform>();
            if (rectTransform != null)
            {
                hitUIObject = pointer.HitInfo.gameObject;
                ShowOriginalVisualization();
                return;
            }
        }
        else
        {
            hitTargetCollider = null;
            if (forceDefaultRotation)
                targetRot = Quaternion.Euler(defaultRotation);
        }
        Vector3 movementVector = targetPos - lastValidPosition;

        if (movementVector.sqrMagnitude > 0.000001f)
        {
            lastValidPosition = transform.position;

            transform.position = targetPos;
            transform.rotation = targetRot;

            if (autoResolveCollisions && IsColliding())
            {
                TryResolveByRotation();
            }
        }
    }
    private float GetWorldScale(ZPointer pointer)
    {
        if (pointer.EventCamera != null)
        {
            return pointer.EventCamera.WorldScale.z;
        }
        else
        {
            return 1;
        }
    }


    private void SetPointerObjectLayer()
    {
        layerMap[gameObject] = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("PointerObject");

        var children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
        {
            layerMap[child.gameObject] = child.gameObject.layer;
            child.gameObject.layer = LayerMask.NameToLayer("PointerObject");
        }
    }
    private void ResetLayers()
    {
        gameObject.layer = layerMap.GetValueOrDefault(gameObject, LayerMask.NameToLayer("Default"));
        var children = gameObject.GetComponentsInChildren<Transform>(true);
        foreach (Transform child in children)
            child.gameObject.layer = layerMap.GetValueOrDefault(child.gameObject, LayerMask.NameToLayer("Default"));
    }

    private void GetStylusTarget()
    {
        if (stylusTarget == null)
        {
            stylusTarget = ZProvider.StylusTarget;
            if (stylusTarget != null)
            {
                stylusTarget.IsVibrationEnabled = true;
            }
            else
            {
                AppLogger.LogWarning("Stylus target is not initialized. Vibration will not work.");
            }
        }
    }
    protected void Vibrate()
    {
        switch (vibrationType)
        {
            case VibrationTypeEnum.Constant:
                Vibrate(1.0f, 0.0f, 100, vibrationIntensity);
                break;

            case VibrationTypeEnum.FastPulse:
                Vibrate(0.1f, 0.1f, 100, vibrationIntensity);
                break;

            case VibrationTypeEnum.MediumPulse:
                Vibrate(0.3f, 0.3f, 100, vibrationIntensity);
                break;

            case VibrationTypeEnum.SlowPulse:
                Vibrate(0.6f, 0.6f, 100, vibrationIntensity);
                break;
            default:
                break;
        }
    }
    protected void Vibrate(float onPeriod, float offPeriod, int numTimes, float intensity)
    {
        GetStylusTarget();
        if (stylusTarget == null) return;
        stylusTarget.StartVibration(onPeriod, offPeriod, numTimes, intensity);
    }
    private void AttachCollisionDetector()
    {
        if (!autoResolveCollisions) return;
        // add a rigidbody to the ObjectPointer if it doesn't have one
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.isKinematic = true; // Make it kinematic to avoid physics interactions
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        if (pointerColliders.Count == 0)
        {
            pointerColliders = GetComponentsInChildren<Collider>().ToList();
            if (pointerColliders.Count == 0)
            {
                Debug.LogWarning($"No colliders found on ObjectPointer, {name} or its children. No colliders will be used for collision detection.");
            }
        }

        foreach (var col in pointerColliders)
        {
            col.isTrigger = true; // Ensure all colliders are not triggers
        }
    }

    private bool IsColliding()
    {
        if (!autoResolveCollisions || pointerColliders.Count == 0 || potentialCollidingObjects.Count == 0)
            return false;
        pointerBounds = GetTotalBounds();
        // filter our any collider where the bounds are too far from the pointer bounds
        List<Collider> filteredColliders = potentialCollidingObjects
            .Where(c => c != null && c.enabled && c.gameObject.activeInHierarchy && c.bounds.Intersects(pointerBounds))
            .ToList();

        if (filteredColliders.Count == 0) return false;

        Physics.SyncTransforms();
        foreach (var col in pointerColliders)
        {
            if (!col.enabled || !col.gameObject.activeInHierarchy) continue;

            foreach (var other in filteredColliders)
            {
                if (other == null || !other.enabled || !other.gameObject.activeInHierarchy || ignoredForAutoResolveColliders.Contains(other) || ignoredColliders.Contains(other)) continue;
                if (Physics.ComputePenetration(
                            col, col.transform.position, col.transform.rotation,
                            other, other.transform.position, other.transform.rotation,
                            out Vector3 direction, out float distance))
                {
                    if ((other == hitTargetCollider && distance > .1) || (other != hitTargetCollider && distance > 0.001f))
                    // if (distance > 0.1f)
                    {// ✅ Only count as "colliding" if it's a true penetration
                        return true;
                    }
                }
            }
        }

        return false;
    }
    private bool TryResolveByRotation()
    {
        if (!autoResolveCollisions) return false;

        if (testAngles.Length == 0)
        {
            testAngles = GenerateSpiralEulerAngles(rotationStepDegrees, 180f);
            Debug.Log($"Test angles generated: {testAngles.Length} on ObjectPointer ({name})");
        }

        Quaternion originalRot = transform.localRotation;

        // sort test angles by distance from original angle
        if (lastDesiredRotation != originalRot)
        {
            Array.Sort(testAngles, (a, b) =>
            {
                float aA = Quaternion.Angle(a, originalRot);
                float aB = Quaternion.Angle(b, originalRot);
                return aA.CompareTo(aB);
            });

            lastDesiredRotation = originalRot;
        }


        foreach (var delta in testAngles)
        {
            transform.localRotation = delta;

            if (!IsColliding())
                return true;
        }

        // Revert since to solution was found
        transform.localRotation = originalRot;
        return false;
    }
    private Quaternion[] GenerateSpiralEulerAngles(float step, float maxAngle)
    {
        List<float> steps = new List<float> { 0f };

        for (float a = step; a <= maxAngle; a += step)
        {
            steps.Add(a);
            steps.Add(-a);
        }

        if (!steps.Contains(maxAngle)) steps.Add(maxAngle);
        if (!steps.Contains(-maxAngle)) steps.Add(-maxAngle);

        List<Quaternion> anglesList = new List<Quaternion>();

        foreach (float x in steps)
        {
            foreach (float y in steps)
            {
                foreach (float z in steps)
                {
                    anglesList.Add(Quaternion.Euler(x, y, z));
                }
            }
        }

        var angle = anglesList.ToArray();

        // we need to cap the number of calculated angles to avoid performance issues
        if (angle.Length * pointerColliders.Count > 1000000)
        {
            Debug.LogWarning($"Too many angles generated ({angle.Length}) on ObjectPointer ({name}). Incresing step size to {step + 1}.");
            angle = GenerateSpiralEulerAngles(step + 1f, maxAngle);
        }

        return angle;
    }
    void OnTriggerEnter(Collider other)
    {
        if (!other.transform.IsChildOf(transform) && !ignoredColliders.Contains(other))
        {
            if (!potentialCollidingObjects.Contains(other))
            {
                potentialCollidingObjects.Add(other);
            }
        }
    }

    void ShowOriginalVisualization()
    {
        isTemporarilyDisconnected = true;
        foreach (var pointer in originalVisualizations.Keys)
        {
            if (originalVisualizations.TryGetValue(pointer, out ZPointerVisualization originalVisualization))
            {
                originalVisualization.gameObject.SetActive(true);
                pointer.Visualization = originalVisualization;
            }
        }

        renderersToTurnOn = GetComponentsInChildren<Renderer>(false).Where(r => r != lr).ToArray();
        foreach (var r in renderersToTurnOn)
        {
            r.enabled = false;
        }
        if (lr != null)
            lr.enabled = false;
    }
    void ShowObjectPointer()
    {
        isTemporarilyDisconnected = false;
        ReorientToPointer(ZPointerMonitor.Instance.ActivePointer);
        foreach (var pointer in originalVisualizations.Keys)
        {
            if (originalVisualizations.TryGetValue(pointer, out ZPointerVisualization originalVisualization))
            {
                originalVisualization.gameObject.SetActive(false);
                pointer.Visualization = this;
            }
        }
        foreach (var r in renderersToTurnOn)
        {
            r.enabled = true;
        }
    }
    void Update()
    {
        if (isTemporarilyDisconnected)
        {
            if (ZPointerMonitor.Instance != null && ZPointerMonitor.Instance.ActivePointer != null)
            {
                if (ZPointerMonitor.Instance.ActivePointer.HitInfo.gameObject == null)
                {
                    ShowObjectPointer();
                }
                else
                {
                    if (hitUIObject != null && hitUIObject != ZPointerMonitor.Instance.ActivePointer.HitInfo.gameObject)
                    {
                        var rectTransform = ZPointerMonitor.Instance.ActivePointer.HitInfo.gameObject.GetComponentInParent<RectTransform>();
                        if (rectTransform == null)
                        {
                            ShowObjectPointer();
                        }
                        else
                        {
                            hitUIObject = ZPointerMonitor.Instance.ActivePointer.HitInfo.gameObject;
                        }
                    }
                }
            }
        }
    }
    Bounds GetTotalBounds()
    {
        // get all renderers and colliders in this object and its children (except the line renderer)
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>().Where(r => r != lr).ToArray();
        Collider[] colliders = gameObject.GetComponentsInChildren<Collider>();

        if (renderers.Length == 0 && colliders.Length == 0)
            // If no renderers or colliders, return a zero-sized bounds at the object's position
            return new Bounds(gameObject.transform.position, Vector3.zero);

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }
        for (int i = 1; i < colliders.Length; i++)
        {
            bounds.Encapsulate(colliders[i].bounds);
        }

        return bounds;
    }
    private void OnDrawGizmos()
    {
        // Draw magenta dot at the center
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(transform.position, 0.025f);

        // Draw blue line in the positive Z direction (local)
        Gizmos.color = Color.blue;
        Vector3 start = transform.position;
        Vector3 endZ = start + transform.forward * 0.2f; // short line (0.2 units)
        Gizmos.DrawLine(start, endZ);

        // Draw blue line in the positive Y direction (local)
        Gizmos.color = Color.green;
        Vector3 endY = start + transform.up * 0.2f; // short line (0.2 units)
        Gizmos.DrawLine(start, endY);

        // Draw a magenta bounding box around the bounds of the ObjectPointer
        Gizmos.color = Color.magenta;
        if (pointerBounds.size != Vector3.zero)
        {
            Gizmos.DrawWireCube(pointerBounds.center, pointerBounds.size);
        }
        else
        {
            // If bounds are zero, draw a small cube at the object's position
            Gizmos.DrawWireCube(transform.position, Vector3.one * 0.1f);
        }
    }

    private void GetLineRenderer()
    {
        if (lr != null) return;
        lr = GetComponent<LineRenderer>();

        if (lr == null)
            lr = gameObject.AddComponent<LineRenderer>();

        lr.positionCount = 2;
        lr.generateLightingData = false;
        lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        if (lineMaterial == null) lineMaterial = new Material(Shader.Find("Sprites/Default"));
        lr.material = lineMaterial;
        lr.enabled = false;
    }

}

[System.Serializable]
public class ContactOffsets
{
    public List<Collider> colliders;
    public bool shouldOverrideAngle = false;
    [DrawIf(nameof(shouldOverrideAngle), true)]
    [Tooltip("Angle to override the default rotation when this collider is hit.")]
    public Vector3 overrideAngle = Vector3.zero;


    [DrawIf(nameof(shouldOverrideAngle), false)]
    public bool shouldOffsetAngle = true;
    [DrawIf(nameof(shouldOverrideAngle), false), DrawIf(nameof(shouldOffsetAngle), true)]
    [Tooltip("Angle to offset the default rotation when this collider is hit.")]
    public Vector3 offsetAngle = Vector3.zero;


    public bool shouldOffsetPosition = false;
    [DrawIf(nameof(shouldOffsetPosition), true)]
    [Tooltip("Position offset to apply when this collider is hit.")]
    public Vector3 offsetPosition = Vector3.zero;
}
