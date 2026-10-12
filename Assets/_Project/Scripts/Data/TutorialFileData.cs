using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NECInspector.Data
{
    /// <summary>
    /// Shape of StreamingAssets/Tutorial/tutorial.json: the guided walk-through that teaches the stylus and a first inspection.
    /// Each step says what to show and which action of the student completes it; the scene reports actions to the tutorial panel.
    /// Plain data with no engine dependency. Text never names a code or a credential; words that differ by region use {term:key} tokens.
    /// </summary>
    [Serializable]
    public class TutorialFileData
    {
        public TutorialStepData[] steps;
    }

    [Serializable]
    public class TutorialStepData
    {
        public string id;
        public string title;
        public string instruction;      // what the student does, in a sentence or two
        public string action;           // one of TutorialActions: what completes the step
        public string hint;             // shown if the student waits
    }

    public static class TutorialActions
    {
        public const string Continue = "continue";              // the student taps Next
        public const string Point = "point";                    // aim the stylus at a part
        public const string Inspect = "inspect";                // front button on a part
        public const string Flag = "flag";                      // flag a part as a violation
        public const string Cite = "cite";                      // choose the rule that applies
        public const string MarkCompliant = "mark-compliant";   // middle button on a part that is fine
        public const string OpenReference = "open-reference";   // open the rule reference
        public const string Finish = "finish";                  // finish the inspection and see the result

        public static readonly string[] All = { Continue, Point, Inspect, Flag, Cite, MarkCompliant, OpenReference, Finish };
    }

    public static class TutorialValidator
    {
        private static readonly Regex Banned = new Regex(@"\b(NEC|CEC|NFPA|CSA|IET|BSI|BS ?7671|C22\.1|NESC|OSHA)\b");
        private static readonly Regex Digit = new Regex(@"\d");

        public static IEnumerable<string> AllText(TutorialStepData s)
        {
            if (s == null) yield break;
            foreach (string t in new[] { s.title, s.instruction, s.hint }) if (t != null) yield return t;
        }

        /// <summary>One message per problem; empty if the walk-through is sound.</summary>
        public static List<string> Validate(TutorialFileData file)
        {
            var errors = new List<string>();
            if (file?.steps == null || file.steps.Length < 3)
            {
                errors.Add("the tutorial needs at least 3 steps");
                return errors;
            }

            var ids = new HashSet<string>();
            foreach (var s in file.steps)
            {
                if (s == null) { errors.Add("steps contains an empty entry"); continue; }
                string label = string.IsNullOrWhiteSpace(s.id) ? "(step without an id)" : s.id;
                if (string.IsNullOrWhiteSpace(s.id)) errors.Add($"{label}: id is missing");
                else if (!ids.Add(s.id)) errors.Add($"{label}: duplicate id");
                if (string.IsNullOrWhiteSpace(s.title)) errors.Add($"{label}: title is missing");
                if (string.IsNullOrWhiteSpace(s.instruction)) errors.Add($"{label}: instruction is missing");
                if (string.IsNullOrWhiteSpace(s.hint)) errors.Add($"{label}: hint is missing");
                if (!TutorialActions.All.Contains(s.action)) errors.Add($"{label}: unknown action '{s.action}'");
                foreach (string t in AllText(s))
                {
                    if (Banned.IsMatch(t)) errors.Add($"{label}: names a code or regulator");
                    if (Digit.IsMatch(t)) errors.Add($"{label}: contains a number (figures belong to the rules)");
                }
            }

            if (file.steps[file.steps.Length - 1]?.action != TutorialActions.Finish) errors.Add("the last step must be 'finish'");
            return errors;
        }
    }
}
