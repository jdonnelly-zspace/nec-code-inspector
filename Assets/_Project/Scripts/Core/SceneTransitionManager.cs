using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NECInspector.Core
{
    /// <summary>
    /// Manages scene transitions with fade effects and loading progress.
    /// Uses a world-space Canvas with a full-screen quad for the fade overlay.
    /// </summary>
    public class SceneTransitionManager : MonoBehaviour
    {
        public static SceneTransitionManager Instance { get; private set; }

        [Header("Fade Settings")]
        [SerializeField] private CanvasGroup _fadeOverlay;
        [SerializeField] private float _fadeDuration = 0.5f;

        [Header("Loading")]
        [SerializeField] private GameObject _loadingPanel;
        [SerializeField] private TMPro.TextMeshProUGUI _loadingText;

        private bool _isTransitioning;

        public bool IsTransitioning => _isTransitioning;
        public event Action<string> OnSceneLoadStarted;
        public event Action<string> OnSceneLoadComplete;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_fadeOverlay != null)
            {
                _fadeOverlay.alpha = 0f;
                _fadeOverlay.gameObject.SetActive(false);
            }
            if (_loadingPanel != null)
                _loadingPanel.SetActive(false);
        }

        /// <summary>
        /// Transition to a scene with fade out, async load, fade in.
        /// </summary>
        public void TransitionToScene(string sceneName)
        {
            if (_isTransitioning) return;
            StartCoroutine(TransitionCoroutine(sceneName));
        }

        /// <summary>
        /// Fade out, execute an action, then fade back in.
        /// Useful for resetting a scene without loading.
        /// </summary>
        public void FadeAction(Action action)
        {
            if (_isTransitioning) return;
            StartCoroutine(FadeActionCoroutine(action));
        }

        private IEnumerator TransitionCoroutine(string sceneName)
        {
            _isTransitioning = true;
            OnSceneLoadStarted?.Invoke(sceneName);

            // Fade out
            yield return StartCoroutine(Fade(0f, 1f));

            // Show loading panel
            if (_loadingPanel != null)
                _loadingPanel.SetActive(true);
            SetLoadingText($"Loading {sceneName}...");

            // Load scene async
            var operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            while (operation.progress < 0.9f)
            {
                SetLoadingText($"Loading {sceneName}... {operation.progress * 100f:F0}%");
                yield return null;
            }

            SetLoadingText("Ready");
            operation.allowSceneActivation = true;

            // Wait for scene activation
            while (!operation.isDone)
                yield return null;

            // Hide loading, fade in
            if (_loadingPanel != null)
                _loadingPanel.SetActive(false);

            yield return StartCoroutine(Fade(1f, 0f));

            _isTransitioning = false;
            OnSceneLoadComplete?.Invoke(sceneName);
        }

        private IEnumerator FadeActionCoroutine(Action action)
        {
            _isTransitioning = true;

            yield return StartCoroutine(Fade(0f, 1f));
            action?.Invoke();
            yield return StartCoroutine(Fade(1f, 0f));

            _isTransitioning = false;
        }

        private IEnumerator Fade(float from, float to)
        {
            if (_fadeOverlay == null) yield break;

            _fadeOverlay.gameObject.SetActive(true);
            float elapsed = 0f;

            while (elapsed < _fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;
                _fadeOverlay.alpha = Mathf.Lerp(from, to, elapsed / _fadeDuration);
                yield return null;
            }

            _fadeOverlay.alpha = to;

            if (to <= 0f)
                _fadeOverlay.gameObject.SetActive(false);
        }

        private void SetLoadingText(string text)
        {
            if (_loadingText != null)
                _loadingText.text = text;
        }
    }
}
