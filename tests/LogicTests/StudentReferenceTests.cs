using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Students learn skills and violations, not the installation code's article numbers. The reference numbers stay in the data
    /// (for scoring and review) but are hidden in the UI (CodeTerminology.showReferenceNumbers), and the text students read should not
    /// quote them either. Text that still does is counted as a warning until it is rewritten.
    /// </summary>
    public static class StudentReferenceTests
    {
        private static readonly Regex Number = new Regex(@"\bArticle \d+|\bArt\. ?\d+|\bTable \d{2,3}\.\d+|\b\d{2,3}\.\d{1,3}(\([A-Za-z0-9]+\))+|\b\d{3}\.\d{1,3}\b(?!\s*(V|A|ft|in|VA|mm))|\bSection \d");
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true };

        public static void Run(TestContext t)
        {
            t.Begin("student-facing text and rule numbers");
            string root = TestContext.RepoRoot();

            foreach (var profile in ProfileFiles.LoadAll(root).Where(p => p.Id.StartsWith("nec")))
            {
                t.IsTrue(!profile.terminology.showReferenceNumbers, $"{profile.Id}: rule numbers are hidden from students");
                int n = profile.articles.Count(a => Number.IsMatch(a.title ?? "") || Number.IsMatch(a.text ?? ""));
                if (n > 0) t.Warn($"{profile.Id}: {n} of {profile.articles.Length} reference entries quote rule numbers in the text students read");
            }

            int violations = 0, total = 0;
            foreach (string path in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
                foreach (var v in JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(path), Options).violations)
                {
                    total++;
                    var texts = new[] { v.description, v.hintText, v.inspectionNote }
                        .Concat((v.citations ?? new ViolationCitation[0]).SelectMany(c => new[] { c.text, c.description, c.hintText, c.inspectionNote }));
                    if (texts.Any(x => !string.IsNullOrEmpty(x) && Number.IsMatch(x))) violations++;
                }
            if (violations > 0) t.Warn($"{violations} of {total} violations quote rule numbers in the text students read");

            foreach (string path in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/QuickReference"), "*.json"))
            {
                int n = JsonSerializer.Deserialize<QuickReferenceFile>(File.ReadAllText(path), Options).cards
                    .Count(c => Number.IsMatch(c.summary ?? "") || Number.IsMatch(c.keyRule ?? ""));
                if (n > 0) t.Warn($"{Path.GetFileName(path)}: {n} cards quote rule numbers in the text students read");
            }
        }
    }
}
