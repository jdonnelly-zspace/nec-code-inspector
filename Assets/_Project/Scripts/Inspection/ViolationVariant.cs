using UnityEngine;

namespace NECInspector.Inspection
{
    /// <summary>
    /// Lets one scene object show its violating or its compliant state. Put it on (or above) the object whose name a
    /// violation's <c>componentObjectName</c> points at. InspectionManager calls <see cref="Apply"/> for every violation
    /// in the scenario at the start of a session: present for the violations drawn this session, absent for the rest,
    /// so a part that is not part of this session looks compliant and flagging it counts as a false positive, not a find.
    /// See docs/POOL_AND_DRAW.md.
    /// </summary>
    public class ViolationVariant : MonoBehaviour
    {
        [Tooltip("Shown when the violation is part of this session (a missing clamp, a handle at the wrong height)")]
        [SerializeField] private GameObject[] _whenViolating;

        [Tooltip("Shown when it is not (the clamp is there, the handle is at the right height)")]
        [SerializeField] private GameObject[] _whenCompliant;

        public void Apply(bool violationPresent)
        {
            SetAll(_whenViolating, violationPresent);
            SetAll(_whenCompliant, !violationPresent);
        }

        private static void SetAll(GameObject[] objects, bool active)
        {
            if (objects == null) return;
            foreach (var go in objects)
                if (go != null) go.SetActive(active);
        }
    }
}
