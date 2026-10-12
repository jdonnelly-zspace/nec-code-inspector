using System.Linq;
using System.Text;
using UnityEngine;
using TMPro;
using NECInspector.Codes;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.UI
{
    /// <summary>
    /// World-space panel that teaches one skill before the student practises it: what the skill is, what to look for, why it
    /// matters, what each tier asks of them and the usual mistakes. The text comes from <see cref="LessonLibrary"/> and is shown
    /// in the words of the student's region. It names no code and no credential.
    /// </summary>
    public class SkillLessonPanel : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _summaryText;
        [SerializeField] private TextMeshProUGUI _lookForText;
        [SerializeField] private TextMeshProUGUI _whyText;
        [SerializeField] private TextMeshProUGUI _tiersText;
        [SerializeField] private TextMeshProUGUI _mistakesText;
        [SerializeField] private UnityEngine.UI.Button _closeButton;

        private void Awake()
        {
            _closeButton?.onClick.AddListener(Hide);
        }

        /// <summary>Show the lesson for a skill. Does nothing if the skill has no lesson.</summary>
        public void Show(string skillId)
        {
            var lesson = LessonLibrary.Get(skillId);
            if (lesson == null) return;

            gameObject.SetActive(true);

            SetText(_titleText, Format(lesson.title));
            SetText(_summaryText, Format(lesson.summary));
            SetText(_lookForText, "What to look for\n" + Bullets(lesson.whatToLookFor));
            SetText(_whyText, "Why it matters\n" + Format(lesson.whyItMatters));
            SetText(_tiersText, new StringBuilder()
                .AppendLine("What you will be able to do")
                .AppendLine($"{SkillTiers.Names[(int)SkillTier.Foundation]}: {Format(lesson.tiers.foundation)}")
                .AppendLine($"{SkillTiers.Names[(int)SkillTier.Practitioner]}: {Format(lesson.tiers.practitioner)}")
                .Append($"{SkillTiers.Names[(int)SkillTier.Authority]}: {Format(lesson.tiers.authority)}").ToString());
            SetText(_mistakesText, "Common mistakes\n" + Bullets(lesson.commonMistakes));
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private static string Format(string text) => CodeProfiles.Terminology.Format(text);

        private static string Bullets(string[] items)
        {
            return items == null ? "" : string.Join("\n", items.Select(i => "• " + Format(i)));
        }

        private static void SetText(TextMeshProUGUI tmp, string text)
        {
            if (tmp != null) tmp.text = text ?? "";
        }
    }
}
