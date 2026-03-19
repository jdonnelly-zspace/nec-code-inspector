using UnityEngine;
using UnityEngine.SceneManagement;

namespace NECInspector.Core
{
    /// <summary>
    /// Central singleton managing scene flow, difficulty state, and system initialization.
    /// Persists across all scenes via DontDestroyOnLoad.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Difficulty Settings")]
        [SerializeField] private DifficultySettingsSO[] _difficultySettings;

        public DifficultyManager Difficulty { get; private set; }
        public ProgressManager Progress { get; private set; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Difficulty = new DifficultyManager();
            Difficulty.Initialize(_difficultySettings);

            Progress = new ProgressManager();
            Progress.Load();

            Debug.Log("[GameManager] Initialized");
        }

        public void LoadScene(string sceneName)
        {
            Debug.Log($"[GameManager] Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        public void LoadSceneAsync(string sceneName)
        {
            Debug.Log($"[GameManager] Loading scene async: {sceneName}");
            SceneManager.LoadSceneAsync(sceneName);
        }

        public void ReturnToMainMenu()
        {
            LoadScene("MainMenu");
        }

        private void OnApplicationQuit()
        {
            Progress?.Save();
        }
    }
}
