using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel for choosing the occupation the student follows. An occupation is a pathway of procedures and skills, not a
    /// credential. Occupations not yet released are listed as coming soon and cannot be chosen. The choice is saved with the progress;
    /// it changes what the app suggests, never what a skill counts for. Wire the list, prefab and texts in the menu scene (TODO).
    /// </summary>
    public class OccupationPickerPanel : MonoBehaviour
    {
        [Header("List")]
        [SerializeField] private Transform _listContent;
        [SerializeField] private GameObject _listItemPrefab;      // a Button with a TextMeshProUGUI child

        [Header("Detail")]
        [SerializeField] private TextMeshProUGUI _currentText;
        [SerializeField] private TextMeshProUGUI _detailText;
        [SerializeField] private UnityEngine.UI.Button _chooseButton;
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        /// <summary>Raised after the student chooses an occupation.</summary>
        public event Action<string> OnOccupationChosen;

        private string _focusedId;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Hide);
            _chooseButton?.onClick.AddListener(ChooseFocused);
        }

        public void Show()
        {
            gameObject.SetActive(true);
            string current = GameManager.Instance?.Progress?.GetOccupation();
            _focusedId = string.IsNullOrEmpty(current) ? OccupationLibrary.All.FirstOrDefault(o => o.available)?.id : current;
            Refresh();
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Refresh()
        {
            var all = OccupationLibrary.All;
            var current = OccupationLibrary.Get(GameManager.Instance?.Progress?.GetOccupation());
            SetText(_currentText, current == null ? "No occupation chosen" : "Following: " + current.title);

            if (_listContent != null && _listItemPrefab != null)
            {
                for (int i = _listContent.childCount - 1; i >= 0; i--) Destroy(_listContent.GetChild(i).gameObject);
                foreach (var o in all)
                {
                    var item = Instantiate(_listItemPrefab, _listContent);
                    var label = item.GetComponentInChildren<TextMeshProUGUI>();
                    if (label != null) label.text = o.available ? o.title : o.title + " (coming soon)";
                    var button = item.GetComponent<UnityEngine.UI.Button>();
                    string id = o.id;
                    button?.onClick.AddListener(() => { _focusedId = id; Refresh(); });
                }
            }

            var focused = OccupationLibrary.Get(_focusedId);
            SetText(_detailText, focused == null ? "" : Describe(focused));
            if (_chooseButton != null) _chooseButton.interactable = focused != null && focused.available;
        }

        private void ChooseFocused()
        {
            var focused = OccupationLibrary.Get(_focusedId);
            if (focused == null || !focused.available) return;
            GameManager.Instance?.Progress?.SetOccupation(focused.id);
            OnOccupationChosen?.Invoke(focused.id);
            Refresh();
        }

        private static string Describe(OccupationData o)
        {
            var sb = new StringBuilder();
            sb.AppendLine(o.title);
            sb.AppendLine(o.summary);
            sb.AppendLine();
            sb.AppendLine($"{o.procedures.Length} procedures you will learn:");
            foreach (var p in o.procedures) sb.AppendLine("• " + p.name);
            if (!o.available) sb.AppendLine().Append("Coming in a later release.");
            return sb.ToString();
        }

        private static void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
