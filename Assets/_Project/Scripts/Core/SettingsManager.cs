using System;
using System.IO;
using UnityEngine;

namespace NECInspector.Core
{
    /// <summary>
    /// Persists user settings (audio, difficulty, display) to JSON.
    /// Loaded by GameManager at startup.
    /// </summary>
    public class SettingsManager
    {
        private const string SETTINGS_FILE = "nec_settings.json";

        public UserSettings Settings { get; private set; } = new UserSettings();

        private string SavePath => Path.Combine(Application.persistentDataPath, SETTINGS_FILE);

        public void Load()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    string json = File.ReadAllText(SavePath);
                    Settings = JsonUtility.FromJson<UserSettings>(json) ?? new UserSettings();
                    Debug.Log($"[SettingsManager] Loaded settings from {SavePath}");
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"[SettingsManager] Failed to load settings: {e.Message}");
                    Settings = new UserSettings();
                }
            }
            else
            {
                Settings = new UserSettings();
            }
        }

        public void Save()
        {
            try
            {
                string json = JsonUtility.ToJson(Settings, true);
                File.WriteAllText(SavePath, json);
                Debug.Log($"[SettingsManager] Saved settings to {SavePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[SettingsManager] Failed to save settings: {e.Message}");
            }
        }

        /// <summary>
        /// Apply loaded settings to the game systems.
        /// Call after GameManager, AudioManager, and DifficultyManager are initialized.
        /// </summary>
        public void ApplySettings()
        {
            // Audio
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.MasterVolume = Settings.masterVolume;
                AudioManager.Instance.SFXVolume = Settings.sfxVolume;
                AudioManager.Instance.AmbientVolume = Settings.ambientVolume;
            }

            // Difficulty
            if (GameManager.Instance?.Difficulty != null)
            {
                GameManager.Instance.Difficulty.SetDifficulty(Settings.difficulty);
            }
        }
    }

    [Serializable]
    public class UserSettings
    {
        // Audio
        public float masterVolume = 1f;
        public float sfxVolume = 1f;
        public float ambientVolume = 0.3f;

        // Difficulty
        public DifficultyLevel difficulty = DifficultyLevel.Standard;

        // Display
        public bool showHints = true;
        public bool show2026Badges = true;

        // Student
        public string studentName = "";
    }
}
