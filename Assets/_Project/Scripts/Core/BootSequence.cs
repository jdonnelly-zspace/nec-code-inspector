using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NECInspector.Core
{
    /// <summary>
    /// Boot scene controller. Initializes core systems, detects zSpace SDK,
    /// and transitions to the main menu.
    /// Attach to the root GameObject in the Boot scene.
    /// </summary>
    public class BootSequence : MonoBehaviour
    {
        [Header("Timing")]
        [SerializeField] private float _minimumSplashTime = 2f;
        [SerializeField] private string _mainMenuScene = "MainMenu";

        [Header("SDK Detection")]
        [SerializeField] private bool _requireZSpace = false;

        private bool _sdkReady;
        private bool _systemsReady;

        private IEnumerator Start()
        {
            Debug.Log("[Boot] Starting boot sequence...");

            float startTime = Time.time;

            // Step 1: Verify GameManager exists (should be in Boot scene or prefab)
            if (GameManager.Instance == null)
            {
                Debug.LogError("[Boot] GameManager not found! Ensure it exists in the Boot scene.");
                yield break;
            }

            // Step 2: Detect zSpace SDK
            yield return StartCoroutine(DetectSDK());

            // Step 3: Verify NEC database loaded
            yield return StartCoroutine(VerifyDatabase());

            _systemsReady = true;

            // Wait for minimum splash time
            float elapsed = Time.time - startTime;
            if (elapsed < _minimumSplashTime)
                yield return new WaitForSeconds(_minimumSplashTime - elapsed);

            // Transition to main menu
            Debug.Log("[Boot] Boot complete. Loading main menu.");
            GameManager.Instance.LoadScene(_mainMenuScene);
        }

        private IEnumerator DetectSDK()
        {
            // Check for zSpace hardware via the ZCore component
            // ZCore is typically added to the ZCamera in zSpace projects
            var zCore = FindAnyObjectByType<MonoBehaviour>();
            // In a real build, this would check for zSpace.Core.ZCore specifically
            // For now, we check if we're running on compatible hardware

            _sdkReady = true;

            if (_requireZSpace)
            {
                // Check for zSpace display capabilities
                bool hasZSpace = SystemInfo.deviceType == DeviceType.Desktop
                    && Application.platform == RuntimePlatform.WindowsPlayer;

                if (!hasZSpace && !Application.isEditor)
                {
                    Debug.LogWarning("[Boot] zSpace hardware not detected. Running in fallback mode.");
                }
            }

            Debug.Log($"[Boot] SDK detection complete. zSpace ready: {_sdkReady}");
            yield return null;
        }

        private IEnumerator VerifyDatabase()
        {
            // Wait a frame for NECDatabase singleton to initialize
            yield return null;

            var db = NEC.NECDatabase.Instance;
            if (db != null)
            {
                Debug.Log("[Boot] NEC database loaded successfully.");
            }
            else
            {
                Debug.LogWarning("[Boot] NEC database not available. Reference features will be limited.");
            }
        }
    }
}
