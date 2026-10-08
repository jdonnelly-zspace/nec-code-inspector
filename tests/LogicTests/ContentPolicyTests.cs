using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Enforces docs/CONTENT_POLICY.md: content covers only skills from the app's skill list,
    /// and code wording is paraphrased, not copied (no statutory "shall" anywhere in the content).
    /// </summary>
    public static class ContentPolicyTests
    {
        public static void Run(TestContext t)
        {
            ContentStaysInSkillList(t);
            CodeTextIsParaphrased(t);
        }

        // Content may only teach skills from the app's own skill list (ConceptIds). The app contains no
        // credentials; adding a skill needs a documented reason in docs/credential-alignment.
        private static void ContentStaysInSkillList(TestContext t)
        {
            t.Begin("content scope");

            string root = TestContext.RepoRoot();
            var options = new JsonSerializerOptions { IncludeFields = true };

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
            {
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                if (data?.violations == null) continue;

                foreach (var v in data.violations)
                    t.IsTrue(ConceptIds.IsKnown(v.conceptId),
                        $"{Path.GetFileName(file)}: {v.violationId} tests '{v.conceptId}', which is not in the skill list");
            }

            foreach (var rule in ElectricalTables.CreateNecDefaults().complianceRules)
                t.IsTrue(ConceptIds.IsKnown(rule.conceptId),
                    $"sandbox rule {rule.ruleId} gives evidence for '{rule.conceptId}', which is not in the skill list");

            // The app must not ship credential files or credential code
            t.IsTrue(!Directory.Exists(Path.Combine(root, "Assets/_Project/StreamingAssets/Credentials")),
                "no credential files ship in StreamingAssets");
            t.IsTrue(!Directory.Exists(Path.Combine(root, "Assets/_Project/Scripts/Credentials")),
                "no credential code ships in the app");
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
                    {
                        t.IsTrue(!LooksStatutory(c.text), $"{v.violationId}: citation text for '{c.reference}' uses statutory wording");
                        t.IsTrue(!LooksStatutory(c.description), $"{v.violationId}: {c.profileId} description override uses statutory wording");
                        t.IsTrue(!LooksStatutory(c.hintText), $"{v.violationId}: {c.profileId} hint override uses statutory wording");
                        t.IsTrue(!LooksStatutory(c.inspectionNote), $"{v.violationId}: {c.profileId} note override uses statutory wording");
                    }
                }
            }

            foreach (var profile in ProfileFiles.LoadAll(root))
                foreach (var a in profile.articles ?? new CodeArticleData[0])
                    t.IsTrue(!LooksStatutory(a.text), $"{profile.Id} '{a.reference}' text uses statutory wording");
        }

        private static bool LooksStatutory(string text)
        {
            return !string.IsNullOrEmpty(text) && Regex.IsMatch(text, @"\bshall\b", RegexOptions.IgnoreCase);
        }
    }
}
