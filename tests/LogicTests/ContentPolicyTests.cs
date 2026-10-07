using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Credentials;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Enforces docs/CONTENT_POLICY.md: content covers only skills that a credential requires,
    /// and code wording is paraphrased, not copied (no statutory "shall" anywhere in the content).
    /// </summary>
    public static class ContentPolicyTests
    {
        public static void Run(TestContext t)
        {
            ContentStaysInCredentialScope(t);
            CodeTextIsParaphrased(t);
        }

        // Skills (concept IDs) required by at least one credential file
        public static HashSet<string> RequiredSkills(string root)
        {
            var options = new JsonSerializerOptions { IncludeFields = true };
            var skills = new HashSet<string>();

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/StreamingAssets/Credentials"), "*.json"))
            {
                var profile = JsonSerializer.Deserialize<CredentialProfile>(File.ReadAllText(file), options);
                if (profile?.requirements == null) continue;
                foreach (var r in profile.requirements)
                    skills.Add(r.skillId);
            }

            return skills;
        }

        private static void ContentStaysInCredentialScope(TestContext t)
        {
            t.Begin("content scope");

            string root = TestContext.RepoRoot();
            var required = RequiredSkills(root);
            var options = new JsonSerializerOptions { IncludeFields = true };

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
            {
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                if (data?.violations == null) continue;

                foreach (var v in data.violations)
                    t.IsTrue(required.Contains(v.conceptId),
                        $"{Path.GetFileName(file)}: {v.violationId} tests '{v.conceptId}', which no credential requires");
            }

            foreach (var rule in ElectricalTables.CreateNecDefaults().complianceRules)
                t.IsTrue(required.Contains(rule.conceptId),
                    $"sandbox rule {rule.ruleId} gives evidence for '{rule.conceptId}', which no credential requires");
        }

        // Statutory wording ("shall") is a sign that text was copied from a code book.
        // Applies to every piece of student-facing text in the content files.
        private static void CodeTextIsParaphrased(TestContext t)
        {
            t.Begin("content policy: paraphrased text");

            string root = TestContext.RepoRoot();
            var options = new JsonSerializerOptions { IncludeFields = true };

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
            {
                string name = Path.GetFileName(file);
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                if (data?.violations == null) continue;

                t.IsTrue(!LooksStatutory(data.description), $"{name}: scenario description uses statutory wording");
                t.IsTrue(!LooksStatutory(data.environmentDescription), $"{name}: environment description uses statutory wording");

                foreach (var v in data.violations)
                {
                    t.IsTrue(!LooksStatutory(v.description), $"{v.violationId}: description uses statutory wording");
                    t.IsTrue(!LooksStatutory(v.hintText), $"{v.violationId}: hint uses statutory wording");
                    t.IsTrue(!LooksStatutory(v.inspectionNote), $"{v.violationId}: inspection note uses statutory wording");

                    foreach (var c in v.citations ?? new ViolationCitation[0])
                        t.IsTrue(!LooksStatutory(c.text), $"{v.violationId}: citation text for '{c.reference}' uses statutory wording");
                }
            }

            var articles = JsonSerializer.Deserialize<ArticleFile>(
                File.ReadAllText(Path.Combine(root, "Assets/_Project/StreamingAssets/NECDatabase/nec_articles.json")));
            foreach (var a in articles.articles)
                t.IsTrue(!LooksStatutory(a.text), $"article '{a.article}{a.subsection}' text uses statutory wording");
        }

        private static bool LooksStatutory(string text)
        {
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\bshall\b", RegexOptions.IgnoreCase);
        }

        private class ArticleFile
        {
            public ArticleText[] articles { get; set; }
        }

        private class ArticleText
        {
            public string article { get; set; }
            public string subsection { get; set; }
            public string text { get; set; }
        }
    }
}
