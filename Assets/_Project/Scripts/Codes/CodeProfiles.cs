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

        /// <summary>
        /// ID of the active profile. Falls back to the NEC when none is loaded, matching the
        /// fallback used for Tables, so content filtering and scoring still work in a bare scene.
        /// </summary>
        public static string ActiveId
        {
            get
            {
                var profile = Active;
                return profile != null ? profile.ProfileId : CodeProfileIds.Nec;
            }
        }

        private static ElectricalTables _fallbackTables;

        /// <summary>
        /// Tables for the active profile. Falls back to NEC defaults when no profile is loaded
        /// (for example when a scene is played directly without the boot scene).
        /// </summary>
        public static ElectricalTables Tables
        {
            get
            {
                var profile = Active;
                if (profile != null && profile.Tables != null)
                    return profile.Tables;

                _fallbackTables ??= ElectricalTables.CreateNecDefaults();
                return _fallbackTables;
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
