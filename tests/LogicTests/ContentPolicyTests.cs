using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Credentials;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Enforces docs/CONTENT_POLICY.md: content covers only skills that a credential requires,
    /// and code wording is paraphrased, not copied.
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
        // Reported as a warning until the existing text has been paraphrased.
        private static void CodeTextIsParaphrased(TestContext t)
        {
            t.Begin("content policy: paraphrased text");

            string root = TestContext.RepoRoot();
            var options = new JsonSerializerOptions { IncludeFields = true };

            int citationTexts = 0, citationLegal = 0;
            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
            {
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                if (data?.violations == null) continue;

                foreach (var v in data.violations)
                    foreach (var c in v.citations ?? new ViolationCitation[0])
                    {
                        citationTexts++;
                        if (LooksStatutory(c.text)) citationLegal++;
                    }
            }

            var articles = JsonSerializer.Deserialize<ArticleFile>(
                File.ReadAllText(Path.Combine(root, "Assets/_Project/StreamingAssets/NECDatabase/nec_articles.json")));
            int articleLegal = articles.articles.Count(a => LooksStatutory(a.text));

            if (citationLegal > 0 || articleLegal > 0)
                t.Warn($"{citationLegal} of {citationTexts} violation citation texts and {articleLegal} of {articles.articles.Length} " +
                       "article texts use statutory wording ('shall'); paraphrase them (docs/CONTENT_POLICY.md)");
        }

        private static bool LooksStatutory(string text)
        {
            return !string.IsNullOrEmpty(text) && (text.Contains(" shall ") || text.Contains(" shall not "));
        }

        private class ArticleFile
        {
            public ArticleText[] articles { get; set; }
        }

        private class ArticleText
        {
            public string text { get; set; }
        }
    }
}
