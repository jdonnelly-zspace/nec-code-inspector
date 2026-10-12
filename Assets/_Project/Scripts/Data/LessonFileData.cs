using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NECInspector.Data
{
    /// <summary>
    /// Shape of StreamingAssets/Lessons/lessons.json: one short lesson per skill, shown before a student practises it
    /// (docs/TEACHING_CONTENT.md). Plain data with no engine dependency, so the same types serve the runtime loader and the tests.
    /// Lesson text is written in our own words and never names a code, a publisher or a credential; words that differ by region
    /// use {term:key} and {Term:key} tokens, filled from the active code's terminology.
    /// </summary>
    [Serializable]
    public class LessonFileData
    {
        public SkillLessonData[] lessons;
    }

    [Serializable]
    public class SkillLessonData
    {
        public string skillId;              // one of ConceptIds
        public string title;                // short name of the skill
        public string summary;              // one or two sentences: what the skill is
        public string[] whatToLookFor;      // 3 to 6 things a student checks in a scene
        public string whyItMatters;         // one or two sentences
        public LessonTierText tiers;        // what the student can do at each tier
        public string[] commonMistakes;     // 2 to 4
        public string reviewStatus;         // draft | reviewed (by a credential expert)
    }

    [Serializable]
    public class LessonTierText
    {
        public string foundation;
        public string practitioner;
        public string authority;
    }

    public static class LessonValidator
    {
        public const string StatusDraft = "draft";
        public const string StatusReviewed = "reviewed";

        private static readonly Regex Token = new Regex(@"\{(?:term|Term):([A-Za-z0-9-]+)\}");

        /// <summary>The term keys a lesson's text uses, for checking against a code's terminology.</summary>
        public static IEnumerable<string> TermKeys(SkillLessonData lesson)
        {
            return AllText(lesson).SelectMany(t => Token.Matches(t).Cast<Match>().Select(m => m.Groups[1].Value)).Distinct();
        }

        /// <summary>Every piece of student-facing text in a lesson.</summary>
        public static IEnumerable<string> AllText(SkillLessonData lesson)
        {
            if (lesson == null) yield break;
            if (lesson.title != null) yield return lesson.title;
            if (lesson.summary != null) yield return lesson.summary;
            if (lesson.whyItMatters != null) yield return lesson.whyItMatters;
            if (lesson.whatToLookFor != null) foreach (string s in lesson.whatToLookFor) if (s != null) yield return s;
            if (lesson.commonMistakes != null) foreach (string s in lesson.commonMistakes) if (s != null) yield return s;
            if (lesson.tiers != null)
            {
                if (lesson.tiers.foundation != null) yield return lesson.tiers.foundation;
                if (lesson.tiers.practitioner != null) yield return lesson.tiers.practitioner;
                if (lesson.tiers.authority != null) yield return lesson.tiers.authority;
            }
        }

        /// <summary>One message per problem; empty if the file is complete: one lesson for every skill, each with all its parts.</summary>
        public static List<string> Validate(LessonFileData file)
        {
            var errors = new List<string>();
            if (file?.lessons == null || file.lessons.Length == 0)
            {
                errors.Add("lessons is empty");
                return errors;
            }

            var seen = new HashSet<string>();
            foreach (var lesson in file.lessons)
            {
                string label = string.IsNullOrEmpty(lesson?.skillId) ? "(lesson without a skillId)" : lesson.skillId;
                if (lesson == null) { errors.Add("lessons contains an empty entry"); continue; }

                if (!ConceptIds.IsKnown(lesson.skillId)) errors.Add($"{label}: unknown skillId");
                else if (!seen.Add(lesson.skillId)) errors.Add($"{label}: more than one lesson for this skill");

                Require(errors, lesson.title, $"{label}: title");
                Require(errors, lesson.summary, $"{label}: summary");
                Require(errors, lesson.whyItMatters, $"{label}: whyItMatters");

                if (lesson.whatToLookFor == null || lesson.whatToLookFor.Length < 3 || lesson.whatToLookFor.Length > 6)
                    errors.Add($"{label}: whatToLookFor needs 3 to 6 items");
                else if (lesson.whatToLookFor.Any(string.IsNullOrWhiteSpace))
                    errors.Add($"{label}: whatToLookFor has an empty item");

                if (lesson.commonMistakes == null || lesson.commonMistakes.Length < 2 || lesson.commonMistakes.Length > 4)
                    errors.Add($"{label}: commonMistakes needs 2 to 4 items");
                else if (lesson.commonMistakes.Any(string.IsNullOrWhiteSpace))
                    errors.Add($"{label}: commonMistakes has an empty item");

                if (lesson.tiers == null)
                {
                    errors.Add($"{label}: tiers is missing");
                }
                else
                {
                    Require(errors, lesson.tiers.foundation, $"{label}: tiers.foundation");
                    Require(errors, lesson.tiers.practitioner, $"{label}: tiers.practitioner");
                    Require(errors, lesson.tiers.authority, $"{label}: tiers.authority");
                }

                if (lesson.reviewStatus != StatusDraft && lesson.reviewStatus != StatusReviewed)
                    errors.Add($"{label}: reviewStatus must be '{StatusDraft}' or '{StatusReviewed}'");
            }

            foreach (string skill in ConceptIds.All)
                if (!seen.Contains(skill)) errors.Add($"no lesson for the skill '{skill}'");

            return errors;
        }

        private static void Require(List<string> errors, string value, string field)
        {
            if (string.IsNullOrWhiteSpace(value)) errors.Add($"{field} is missing");
        }
    }
}
