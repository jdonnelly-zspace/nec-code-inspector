using System;
using UnityEngine;
using TMPro;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel that walks the student through the stylus and a first inspection. It shows one step at a time from
    /// <see cref="TutorialLibrary"/>. The inspection scene reports what the student does with <see cref="Report"/>; a step whose action
    /// matches is completed and the next shows. Steps with the action "continue" wait for the Next button. Wire the texts and buttons in the scene (TODO).
    /// </summary>
    public class TutorialPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _stepCountText;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _instructionText;
        [SerializeField] private TextMeshProUGUI _hintText;
        [SerializeField] private UnityEngine.UI.Button _nextButton;
        [SerializeField] private UnityEngine.UI.Button _skipButton;
        [SerializeField] private float _hintDelaySeconds = 12f;

        /// <summary>Raised when the last step is completed or the student skips.</summary>
        public event Action OnFinished;

        private TutorialStepData[] _steps;
        private int _index;
        private float _stepStartedAt;

        private void Awake()
        {
            _nextButton?.onClick.AddListener(() => Report(TutorialActions.Continue));
            _skipButton?.onClick.AddListener(Finish);
        }

        public void Show()
        {
            _steps = TutorialLibrary.Steps;
            if (_steps.Length == 0) { OnFinished?.Invoke(); return; }
            _index = 0;
            gameObject.SetActive(true);
            ShowStep();
        }

        /// <summary>Tell the tutorial what the student just did (a value of <see cref="TutorialActions"/>).</summary>
        public void Report(string action)
        {
            if (_steps == null || _index >= _steps.Length || _steps[_index].action != action) return;
            _index++;
            if (_index >= _steps.Length) Finish();
            else ShowStep();
        }

        private void Update()
        {
            if (_steps == null || _index >= _steps.Length || _hintText == null) return;
            if (Time.unscaledTime - _stepStartedAt > _hintDelaySeconds) _hintText.text = Format(_steps[_index].hint);
        }

        private void Finish()
        {
            gameObject.SetActive(false);
            OnFinished?.Invoke();
        }

        private void ShowStep()
        {
            var step = _steps[_index];
            _stepStartedAt = Time.unscaledTime;
            SetText(_stepCountText, $"{_index + 1} of {_steps.Length}");
            SetText(_titleText, Format(step.title));
            SetText(_instructionText, Format(step.instruction));
            SetText(_hintText, "");
            if (_nextButton != null) _nextButton.gameObject.SetActive(step.action == TutorialActions.Continue);
        }

        private static string Format(string text) => CodeProfiles.Terminology.Format(text);

        private static void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
