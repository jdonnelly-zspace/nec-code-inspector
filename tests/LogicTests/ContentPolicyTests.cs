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

            // The three difficulty levels are generic: they describe what the student can do, not a pathway or a body
            string[] pathwayWords = { "CTE", "apprentice", "journeyman", "licensed", "trade school", "NCCER", "Red Seal", "high school" };
            foreach (string rel in new[] { "Assets/_Project/Scripts/Core/DifficultyLevel.cs", "Assets/_Project/Scripts/UI/MainMenuPanel.cs" })
            {
                string text = File.ReadAllText(Path.Combine(root, rel));
                foreach (string word in pathwayWords)
                    t.IsTrue(!Regex.IsMatch(text, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase),
                        $"{rel} does not name a training pathway or credential body ('{word}')");
            }

            StudentTextNamesNoCode(t, root);
        }

        // The student picks a region; the code behind it is never named in student-facing text
        // (UI strings, scenario and card content, and every code's reference entries).
        private static void StudentTextNamesNoCode(TestContext t, string root)
        {
            t.Begin("content policy: no code names shown to students");

            var codeName = new Regex(@"\b(NEC|CEC|NFPA|CSA|IET|BSI|BS ?7671|C22\.1)\b");

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Scripts/UI"), "*.cs"))
            {
                int line = 0;
                foreach (string raw in File.ReadLines(file))
                {
                    line++;
                    string trimmed = raw.TrimStart();
                    if (trimmed.StartsWith("//") || trimmed.StartsWith("///") || raw.Contains("Debug.")) continue;
                    foreach (Match literal in Regex.Matches(raw, @"""(?:[^""\\]|\\.)*"""))
                        t.IsTrue(!codeName.IsMatch(literal.Value), $"{Path.GetFileName(file)}:{line} shows a code name to students: {literal.Value}");
                }
            }

            var files = new List<string>();
            foreach (string dir in new[] { "Scenarios", "Difficulty", "Certificates", "QuickReference", "Sandbox" })
                files.AddRange(Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content", dir), "*.json"));
            files.AddRange(Directory.GetFiles(Path.Combine(root, "Assets/_Project/StreamingAssets/Codes"), "articles.json", SearchOption.AllDirectories));
            foreach (string file in files)
            {
                string text = File.ReadAllText(file);
                var m = codeName.Match(text);
                t.IsTrue(!m.Success, $"{Path.GetFileName(file)} shows a code name to students ('{m.Value}')");
                // tools/new_violation.py leaves TODO markers; unfinished content must not ship
                t.IsTrue(!text.Contains("TODO"), $"{Path.GetFileName(file)} still has a TODO placeholder");
            }
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
