using UnityEngine;
using NECInspector.Codes;

namespace NECInspector.NEC
{
    /// <summary>
    /// Scene bootstrap for code profiles. Loads every profile folder under StreamingAssets/Codes/
    /// (see CodeProfileLibrary) and activates the default one. The code content itself is data;
    /// this component only starts the loading (the name is kept so existing scenes keep working).
    /// </summary>
    public class NECDatabase : MonoBehaviour
    {
        public static NECDatabase Instance { get; private set; }

        [Tooltip("Profile to activate at startup. The student's saved choice replaces it at boot.")]
        [SerializeField] private string _defaultProfileId = CodeProfileLibrary.DefaultProfileId;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (!CodeProfileLibrary.Activate(_defaultProfileId))
                Debug.LogError($"[NECDatabase] Code profile '{_defaultProfileId}' could not be loaded; reference features will be limited");
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }
    }
}
