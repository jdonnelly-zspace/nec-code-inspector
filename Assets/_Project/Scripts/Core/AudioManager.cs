using System;
using System.Collections;
using UnityEngine;

namespace NECInspector.Core
{
    /// <summary>
    /// Centralized audio manager for SFX, ambient sounds, and UI feedback.
    /// Persists across scenes. Uses two AudioSources: one for SFX, one for ambient.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _ambientSource;

        [Header("Volume")]
        [Range(0f, 1f)]
        [SerializeField] private float _masterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _sfxVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _ambientVolume = 0.3f;

        [Header("UI Sounds")]
        [SerializeField] private AudioClip _buttonClick;
        [SerializeField] private AudioClip _buttonHover;
        [SerializeField] private AudioClip _success;
        [SerializeField] private AudioClip _error;
        [SerializeField] private AudioClip _flagViolation;
        [SerializeField] private AudioClip _compliancePass;
        [SerializeField] private AudioClip _complianceFail;

        [Header("Tool Sounds")]
        [SerializeField] private AudioClip _flashlightToggle;
        [SerializeField] private AudioClip _multimeterProbe;
        [SerializeField] private AudioClip _breakerSnap;
        [SerializeField] private AudioClip _breakerRemove;
        [SerializeField] private AudioClip _wireConnect;

        [Header("Ambient")]
        [SerializeField] private AudioClip _ambientElectrical;
        [SerializeField] private AudioClip _ambientWorkshop;

        public float MasterVolume
        {
            get => _masterVolume;
            set { _masterVolume = Mathf.Clamp01(value); UpdateVolumes(); }
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set { _sfxVolume = Mathf.Clamp01(value); UpdateVolumes(); }
        }

        public float AmbientVolume
        {
            get => _ambientVolume;
            set { _ambientVolume = Mathf.Clamp01(value); UpdateVolumes(); }
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (_sfxSource == null)
            {
                _sfxSource = gameObject.AddComponent<AudioSource>();
                _sfxSource.playOnAwake = false;
            }

            if (_ambientSource == null)
            {
                _ambientSource = gameObject.AddComponent<AudioSource>();
                _ambientSource.playOnAwake = false;
                _ambientSource.loop = true;
            }

            UpdateVolumes();
        }

        #region SFX Playback

        public void PlaySFX(AudioClip clip, float pitchVariation = 0f)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.pitch = 1f + UnityEngine.Random.Range(-pitchVariation, pitchVariation);
            _sfxSource.PlayOneShot(clip, _sfxVolume * _masterVolume);
        }

        // Convenience methods for common sounds
        public void PlayButtonClick() => PlaySFX(_buttonClick);
        public void PlayButtonHover() => PlaySFX(_buttonHover);
        public void PlaySuccess() => PlaySFX(_success);
        public void PlayError() => PlaySFX(_error);
        public void PlayFlagViolation() => PlaySFX(_flagViolation);
        public void PlayCompliancePass() => PlaySFX(_compliancePass);
        public void PlayComplianceFail() => PlaySFX(_complianceFail);
        public void PlayFlashlightToggle() => PlaySFX(_flashlightToggle);
        public void PlayMultimeterProbe() => PlaySFX(_multimeterProbe);
        public void PlayBreakerSnap() => PlaySFX(_breakerSnap, 0.1f);
        public void PlayBreakerRemove() => PlaySFX(_breakerRemove, 0.1f);
        public void PlayWireConnect() => PlaySFX(_wireConnect);

        #endregion

        #region Ambient

        public void PlayAmbient(AudioClip clip, float fadeInDuration = 1f)
        {
            if (_ambientSource == null) return;

            if (_ambientSource.isPlaying)
                StartCoroutine(CrossfadeAmbient(clip, fadeInDuration));
            else
            {
                _ambientSource.clip = clip;
                _ambientSource.volume = 0f;
                _ambientSource.Play();
                StartCoroutine(FadeAmbient(0f, _ambientVolume * _masterVolume, fadeInDuration));
            }
        }

        public void PlayAmbientElectrical() => PlayAmbient(_ambientElectrical);
        public void PlayAmbientWorkshop() => PlayAmbient(_ambientWorkshop);

        public void StopAmbient(float fadeOutDuration = 1f)
        {
            if (_ambientSource == null || !_ambientSource.isPlaying) return;
            StartCoroutine(FadeAmbientOut(fadeOutDuration));
        }

        private IEnumerator CrossfadeAmbient(AudioClip newClip, float duration)
        {
            yield return StartCoroutine(FadeAmbientOut(duration * 0.5f));
            _ambientSource.clip = newClip;
            _ambientSource.Play();
            yield return StartCoroutine(FadeAmbient(0f, _ambientVolume * _masterVolume, duration * 0.5f));
        }

        private IEnumerator FadeAmbient(float from, float to, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                _ambientSource.volume = Mathf.Lerp(from, to, elapsed / duration);
                yield return null;
            }
            _ambientSource.volume = to;
        }

        private IEnumerator FadeAmbientOut(float duration)
        {
            yield return StartCoroutine(FadeAmbient(_ambientSource.volume, 0f, duration));
            _ambientSource.Stop();
        }

        #endregion

        private void UpdateVolumes()
        {
            if (_sfxSource != null)
                _sfxSource.volume = _sfxVolume * _masterVolume;
            if (_ambientSource != null && _ambientSource.isPlaying)
                _ambientSource.volume = _ambientVolume * _masterVolume;
        }
    }
}
