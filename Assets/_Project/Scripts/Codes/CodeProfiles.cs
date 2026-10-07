using UnityEngine;

namespace NECInspector.Codes
{
    /// <summary>
    /// Holds the code profile currently in use. A profile registers itself when it loads
    /// (for example NECDatabase in its Awake).
    /// </summary>
    public static class CodeProfiles
    {
        private static ICodeProfile _active;

        /// <summary>The active profile, or null if none is loaded (or it was destroyed).</summary>
        public static ICodeProfile Active
        {
            get
            {
                // A destroyed MonoBehaviour is not a C# null when held through an interface.
                if (_active is Object unityObject && unityObject == null)
                    _active = null;
                return _active;
            }
        }

        public static void SetActive(ICodeProfile profile)
        {
            _active = profile;
        }

        public static void Clear(ICodeProfile profile)
        {
            if (ReferenceEquals(_active, profile))
                _active = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetOnLoad()
        {
            _active = null;
        }
    }
}
