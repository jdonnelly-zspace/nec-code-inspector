using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class SnappingDraggablePlane : DraggablePlane, IPointerEnterHandler, IPointerExitHandler
{
    public enum SnappingAxis
    {
        All,
        X,
        Y,
        Z
    }
    public List<DropTarget> dropTargets;
    public float maxSnappingDistance = 0.5f;
    public bool snapped { get; private set; } = false;
    public bool shouldSnapToDropTarget = true;
    public bool shouldReturnToStartIfNotSnapped = true;
    public float returnTweenDuration = 0.3f;
    public SnappingAxis snappingAxis = SnappingAxis.All;

    [SerializeField] private Vector3 originalPosOffset = Vector3.zero;
    [SerializeField] private Vector3 originalRotOffset = Vector3.zero;

    private new Collider collider;
    private Vector3? _initialPosition = null;
    private Quaternion _initialRotation;
    public System.Action OnSnapFailed;
    private Outline outline;

    // Audio
    private AudioSource audioSource;
    [SerializeField] private AudioClip[] snapClips;

    protected void Start()
    {
        collider = GetComponent<Collider>();
        audioSource = GetComponent<AudioSource>();
        outline = GetComponent<Outline>();
        outline.enabled = true;
        if (shouldSnapToDropTarget)
        {
            Assert.IsNotNull(dropTargets, "Drop target must be set if shouldSnapToDropTarget is true.");
            Assert.IsTrue(dropTargets.Count > 0, "Drop targets array must contain at least one target.");
        }
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        base.OnBeginDrag(eventData);
        if (_initialPosition == null) _initialPosition = transform.position;
        _initialGrabOffset -= originalPosOffset;
        _initialRotation = Quaternion.Euler(transform.rotation.eulerAngles + originalRotOffset);
        snapped = false;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("OnEndDrag");
        base.OnEndDrag(eventData);
        if (shouldSnapToDropTarget)
        {
            TrySnapToDropTarget();
        }
        if (shouldSnapToDropTarget && !snapped && shouldReturnToStartIfNotSnapped)
        {
            Debug.Log("Snap failed");
            OnSnapFailed?.Invoke();
            // Prevent any clicks from being registered while animating back
            collider.enabled = false;

            // Return to start position if not snapped
            var seq = Sequence.Create();
            TweenSettings<Vector3> tweenSettings = new(_initialPosition.Value, returnTweenDuration);
            TweenSettings<Quaternion> rotationTweenSettings = new(_initialRotation, returnTweenDuration);

            seq.Group(Tween.Position(transform, tweenSettings));
            seq.Group(Tween.Rotation(transform, rotationTweenSettings));

            // Tween back to original position
            seq.OnComplete(() =>
            {
                collider.enabled = true; // Re-enable collider after tween completes
            });
        }
    }

    private void TrySnapToDropTarget()
    {
        foreach (var target in dropTargets)
        {
            if (!target.enabled) continue;
            var distance = Vector3.Distance(transform.position, target.transform.position);
            if (distance > maxSnappingDistance)
            {
                Debug.Log($"Distance {distance} is greater than maxSnappingDistance {maxSnappingDistance} for target {target.name}. Skipping snap.");
                continue;
            }
            Debug.Log($"Snapping {name} to {target.name} at distance {distance}");
            snapped = true;
            target.enabled = false;
            outline.enabled = false;
            // TODO: Maybe tween to position instead?
            transform.position = target.transform.position;
            transform.rotation = target.transform.rotation;
            // RaiseEndDragEvent();
            // Play an audio clip when snapping
            if (snapClips.Length > 0 && audioSource != null)
            {
                int randomIndex = Random.Range(0, snapClips.Length);
                var pitch = UnityEngine.Random.Range(0.9f, 1.1f);
                audioSource.pitch = pitch;
                audioSource.PlayOneShot(snapClips[randomIndex]);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (outline != null)
            outline.Pulse = false;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (outline != null)
            outline.Pulse = true;
    }

    // private float DistanceOnSnappingAxis(DropTarget target)
    // {
    //     Vector3 position = transform.position;
    //     Vector3 targetPosition = target.transform.position;

    //     switch(snappingAxis)
    //     {
    //         case SnappingAxis.X:
    //             return Mathf.Abs(position.x - targetPosition.x);
    //         case SnappingAxis.Y:
    //             return Mathf.Abs(position.y - targetPosition.y);
    //         case SnappingAxis.Z:
    //             return Mathf.Abs(position.z - targetPosition.z);
    //         case SnappingAxis.All:
    //             return Vector3.Distance(position, targetPosition);
    //         default:
    //             return Vector3.Distance(position, targetPosition);
    //     }
    // }
}
