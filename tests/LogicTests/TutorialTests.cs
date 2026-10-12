using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>The guided tutorial (StreamingAssets/Tutorial/tutorial.json): sound, free of code names and numbers, and readable in each region's words.</summary>
    public static class TutorialTests
    {
        private static readonly Regex Token = new Regex(@"\{(?:term|Term):([A-Za-z0-9-]+)\}");

        public static void Run(TestContext t)
        {
            var opt = new JsonSerializerOptions { IncludeFields = true };
            string path = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/StreamingAssets/Tutorial/tutorial.json");
            var file = JsonSerializer.Deserialize<TutorialFileData>(File.ReadAllText(path), opt);

            t.Begin("tutorial file is sound");
            var errors = TutorialValidator.Validate(file);
            t.Equal(0, errors.Count, "the tutorial is valid: " + string.Join("; ", errors));
            foreach (string action in new[] { TutorialActions.Point, TutorialActions.Inspect, TutorialActions.Flag, TutorialActions.Cite, TutorialActions.MarkCompliant, TutorialActions.OpenReference, TutorialActions.Finish })
                t.IsTrue(file.steps.Any(s => s.action == action), $"the tutorial teaches '{action}'");

            t.Begin("tutorial reads correctly in each region's words");
            foreach (var profile in ProfileFiles.LoadAll(TestContext.RepoRoot()))
            {
                foreach (var step in file.steps)
                    foreach (string text in TutorialValidator.AllText(step))
                    {
                        foreach (Match m in Token.Matches(text))
                        {
                            bool has = profile.terminology.terms != null && profile.terminology.terms.Any(x => x.key == m.Groups[1].Value && !string.IsNullOrEmpty(x.text));
                            if (!has) t.Warn($"tutorial step {step.id} uses the term '{m.Groups[1].Value}', which {profile.Id} does not define");
                        }
                        string shown = profile.terminology.Format(text);
                        t.IsTrue(!shown.Contains("{") && !shown.Contains("}") || profile.Id != "nec", $"{profile.Id} {step.id}: every token is filled");
                    }
            }

            t.Begin("tutorial validator catches problems");
            t.IsTrue(TutorialValidator.Validate(null).Count > 0, "null is rejected");
            var bad = JsonSerializer.Deserialize<TutorialFileData>(JsonSerializer.Serialize(file, opt), opt);
            bad.steps[1].action = "teleport";
            t.IsTrue(TutorialValidator.Validate(bad).Any(e => e.Contains("unknown action")), "unknown action is rejected");
            bad = JsonSerializer.Deserialize<TutorialFileData>(JsonSerializer.Serialize(file, opt), opt);
            bad.steps[1].instruction = "Read the NEC first";
            t.IsTrue(TutorialValidator.Validate(bad).Any(e => e.Contains("names a code")), "a named code is rejected");
            bad = JsonSerializer.Deserialize<TutorialFileData>(JsonSerializer.Serialize(file, opt), opt);
            bad.steps[bad.steps.Length - 1].action = "continue";
            t.IsTrue(TutorialValidator.Validate(bad).Any(e => e.Contains("last step")), "a tutorial that does not end with finish is rejected");
        }
    }
}
