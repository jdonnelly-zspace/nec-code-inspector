using UnityEngine;

namespace NECInspector.Core
{
    /// <summary>
    /// Manages the current difficulty level and provides difficulty-dependent settings.
    /// Accessed via GameManager.Instance.Difficulty.
    /// </summary>
    public class DifficultyManager
    {
        public DifficultyLevel CurrentLevel { get; private set; } = DifficultyLevel.Standard;
        public DifficultySettingsSO CurrentSettings { get; private set; }

        private DifficultySettingsSO[] _allSettings;

        public void Initialize(DifficultySettingsSO[] settings)
        {
            _allSettings = settings;
            ApplySettings();
        }

        public void SetDifficulty(DifficultyLevel level)
        {
            CurrentLevel = level;
            ApplySettings();
            Debug.Log($"[DifficultyManager] Difficulty set to {level}");
        }

        private void ApplySettings()
        {
            if (_allSettings == null) return;

            foreach (var settings in _allSettings)
            {
                if (settings != null && settings.level == CurrentLevel)
                {
                    CurrentSettings = settings;
                    return;
                }
            }

            Debug.LogWarning($"[DifficultyManager] No settings found for {CurrentLevel}");
        }
    }
}
