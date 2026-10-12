using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel for practising the order of a procedure's steps. The steps are dealt shuffled as buttons; the student taps
    /// them in the order they would do the work (a stylus tap each; no dragging needed), then checks the answer. The result shows the
    /// right order, the hazards and how each is controlled, and records evidence for every skill the procedure uses.
    /// Wire the buttons, prefab and texts in the scene (TODO).
    /// </summary>
    public class ProcedurePracticePanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _purposeText;
        [SerializeField] private Transform _stepListContent;
        [SerializeField] private GameObject _stepButtonPrefab;      // a Button with a TextMeshProUGUI child
        [SerializeField] private TextMeshProUGUI _yourOrderText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private UnityEngine.UI.Button _checkButton;
        [SerializeField] private UnityEngine.UI.Button _undoButton;
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        /// <summary>Raised when the student checks an answer: procedure id and score 0 to 1.</summary>
        public event Action<string, float> OnPracticed;

        private ProcedureData _procedure;
        private DifficultyLevel _difficulty;
        private int[] _dealt;
        private readonly List<int> _chosen = new List<int>();
        private bool _checked;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Hide);
            _checkButton?.onClick.AddListener(Check);
            _undoButton?.onClick.AddListener(Undo);
        }

        public void Show(string procedureId)
        {
            _procedure = OccupationLibrary.GetProcedure(procedureId);
            if (_procedure == null || !_procedure.practice) return;

            gameObject.SetActive(true);
            _difficulty = GameManager.Instance != null && GameManager.Instance.Difficulty != null
                ? GameManager.Instance.Difficulty.CurrentLevel : DifficultyLevel.Standard;
            _dealt = ProcedurePractice.Deal(_procedure, _difficulty, Environment.TickCount);
            _chosen.Clear();
            _checked = false;

            SetText(_titleText, _procedure.name);
            SetText(_purposeText, _procedure.purpose + "\n\nTap the steps in the order you would do them.");
            SetText(_resultText, "");
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Pick(int step)
        {
            if (_checked || _chosen.Contains(step)) return;
            _chosen.Add(step);
            Refresh();
        }

        private void Undo()
        {
            if (_checked || _chosen.Count == 0) return;
            _chosen.RemoveAt(_chosen.Count - 1);
            Refresh();
        }

        private void Check()
        {
            if (_checked || _dealt == null || _chosen.Count != _dealt.Length) return;
            _checked = true;

            float score = ProcedurePractice.Score(_chosen.ToArray());
            GameManager.Instance?.Progress?.RecordProcedurePractice(_procedure.id, _difficulty, score,
                ProcedurePractice.Evidence(_procedure, _difficulty, score));
            OnPracticed?.Invoke(_procedure.id, score);

            var sb = new StringBuilder();
            sb.AppendLine(ProcedurePractice.IsInOrder(_chosen.ToArray()) ? "That is the right order." : $"{Mathf.RoundToInt(score * 100)} percent of the steps are in the right order.");
            sb.AppendLine().AppendLine("Right order:");
            int n = 1;
            foreach (int i in _dealt.OrderBy(x => x)) sb.AppendLine($"{n++}. {_procedure.steps[i]}");
            sb.AppendLine().AppendLine("Hazards: " + string.Join("; ", _procedure.hazards));
            sb.Append("Controls: " + string.Join("; ", _procedure.controls));
            SetText(_resultText, sb.ToString());
            Refresh();
        }

        private void Refresh()
        {
            if (_procedure == null || _dealt == null) return;

            if (_stepListContent != null && _stepButtonPrefab != null)
            {
                for (int i = _stepListContent.childCount - 1; i >= 0; i--) Destroy(_stepListContent.GetChild(i).gameObject);
                foreach (int step in _dealt)
                {
                    var item = Instantiate(_stepButtonPrefab, _stepListContent);
                    var label = item.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) label.text = _procedure.steps[step];
                    var button = item.GetComponent<UnityEngine.UI.Button>();
                    if (button != null)
                    {
                        button.interactable = !_checked && !_chosen.Contains(step);
                        int captured = step;
                        button.onClick.AddListener(() => Pick(captured));
                    }
                }
            }

            SetText(_yourOrderText, _chosen.Count == 0 ? "" : "Your order:\n" + string.Join("\n", _chosen.Select((s, i) => $"{i + 1}. {_procedure.steps[s]}")));
            if (_checkButton != null) _checkButton.interactable = !_checked && _chosen.Count == _dealt.Length;
            if (_undoButton != null) _undoButton.interactable = !_checked && _chosen.Count > 0;
        }

        private static void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
