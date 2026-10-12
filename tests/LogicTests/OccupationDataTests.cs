using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The occupation research data (docs/occupations/data): every procedure is complete, names only skills that exist or are proposed,
    /// and rates the app's coverage consistently. The data is documentation outside the app; the checks keep it from drifting away from the skills.
    /// </summary>
    public static class OccupationDataTests
    {
        private static readonly string[] CoverageWords = { "none", "partial", "good" };

        public static void Run(TestContext t)
        {
            string root = TestContext.RepoRoot();
            string folder = Path.Combine(root, "docs/occupations");
            var proposed = LoadProposed(folder);
            ProposedSkills(t, proposed);
            Procedures(t, folder, proposed);
            AdjacentFile(t, folder);
        }

        private static Dictionary<string, JsonElement> LoadProposed(string folder)
        {
            var doc = JsonDocument.Parse(File.ReadAllText(Path.Combine(folder, "proposed-skills.json")));
            return doc.RootElement.GetProperty("skills").EnumerateArray().ToDictionary(e => e.GetProperty("id").GetString(), e => e.Clone());
        }

        private static string Text(JsonElement e, string name) => e.TryGetProperty(name, out var v) && v.ValueKind == JsonValueKind.String ? v.GetString() : "";

        private static void ProposedSkills(TestContext t, Dictionary<string, JsonElement> proposed)
        {
            t.Begin("proposed skills");

            t.IsTrue(proposed.Count > 0, "there are proposed skills");
            foreach (var kv in proposed)
            {
                var e = kv.Value;
                t.IsTrue(!ConceptIds.IsKnown(kv.Key), $"{kv.Key}: not already one of the app's skills (a built skill is removed from the proposals)");
                t.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(kv.Key, "^[a-z]+(-[a-z]+)*$"), $"{kv.Key}: the id is lower-case words joined by '-'");
                foreach (string field in new[] { "summary", "reason", "standards", "firstContent" })
                    t.IsTrue(Text(e, field).Length > 20, $"{kv.Key}: {field} is written");
                t.IsTrue(Text(e, "title").Length >= 8, $"{kv.Key}: a title is written");
                t.IsTrue(Text(e, "codeProfile").Length >= 4, $"{kv.Key}: says which code profile it needs, or none");
                string status = Text(e, "status");
                t.IsTrue(status == "proposed" || status == "approved" || status == "blocked", $"{kv.Key}: status is proposed, approved or blocked");
                if (status == "blocked") t.IsTrue(Text(e, "blockedBy").Length > 20, $"{kv.Key}: a blocked skill says what blocks it");
                t.IsTrue(e.TryGetProperty("wave", out var wave) && wave.GetInt32() >= 1 && wave.GetInt32() <= 3, $"{kv.Key}: sits in release wave 1, 2 or 3");
                if (status == "approved" || status == "blocked") t.IsTrue(Text(e, "decision").StartsWith("approved"), $"{kv.Key}: records the owner's decision");
            }
        }

        private static void Procedures(TestContext t, string folder, Dictionary<string, JsonElement> proposed)
        {
            t.Begin("occupation procedures");

            var files = Directory.GetFiles(Path.Combine(folder, "data"), "*.json").Where(f => char.IsDigit(Path.GetFileName(f)[0])).ToArray();
            t.Equal(6, files.Length, "six core occupations have data files");

            var usedProposed = new HashSet<string>();
            foreach (string file in files)
            {
                var d = JsonDocument.Parse(File.ReadAllText(file)).RootElement;
                string soc = Text(d, "soc");
                t.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(soc, @"^\d\d-\d{4}$"), $"{Path.GetFileName(file)}: a valid O*NET-SOC code");
                t.IsTrue(Path.GetFileName(file).StartsWith(soc), $"{Path.GetFileName(file)}: named for its code");
                t.IsTrue(Text(d, "onetUrl").StartsWith("https://www.onetonline.org/"), $"{soc}: points at O*NET OnLine");
                t.IsTrue(Text(d, "summary").Length > 60 && Text(d, "typicalPath").Length > 60, $"{soc}: a summary and a path are written");

                int tasks = d.GetProperty("tasks").GetArrayLength();
                t.IsTrue(tasks >= 10 && tasks <= 15, $"{soc}: 10 to 15 tasks ({tasks})");
                foreach (var task in d.GetProperty("tasks").EnumerateArray())
                    foreach (var s in task.GetProperty("appSkills").EnumerateArray())
                        t.IsTrue(ConceptIds.IsKnown(s.GetString()), $"{soc}: task skill '{s.GetString()}' is one of the app's skills");

                var procedures = d.GetProperty("procedures").EnumerateArray().ToList();
                t.IsTrue(procedures.Count >= 5 && procedures.Count <= 8, $"{soc}: 5 to 8 procedures ({procedures.Count})");
                var names = new HashSet<string>();
                foreach (var p in procedures)
                {
                    string name = Text(p, "name");
                    string label = $"{soc} '{(name.Length > 48 ? name.Substring(0, 48) + "..." : name)}'";
                    t.IsTrue(names.Add(name), $"{label}: the name is unique within the occupation");
                    foreach (string field in new[] { "purpose", "whenUsed", "role" })
                        t.IsTrue(Text(p, field).Length > 15, $"{label}: {field} is written");
                    t.IsTrue(p.GetProperty("steps").GetArrayLength() >= 5, $"{label}: at least 5 steps");
                    t.IsTrue(p.GetProperty("hazards").GetArrayLength() >= 1 && p.GetProperty("controls").GetArrayLength() >= 1, $"{label}: hazards and controls are listed");
                    t.IsTrue(p.GetProperty("standards").GetArrayLength() >= 1, $"{label}: names the standards that govern it");
                    t.IsTrue(p.GetProperty("evidence").GetArrayLength() >= 1, $"{label}: cites evidence");
                    if (p.GetProperty("evidence").GetArrayLength() < 2)
                        t.Warn($"{label}: only one source (the research asked for two)");
                    foreach (var e in p.GetProperty("evidence").EnumerateArray())
                        t.IsTrue(Text(e, "source").StartsWith("http"), $"{label}: every source is a web address");

                    string coverage = Text(p, "appCoverage");
                    string word = coverage.Split(':', ' ')[0].ToLowerInvariant();
                    t.IsTrue(CoverageWords.Contains(word), $"{label}: coverage starts with none, partial or good");

                    var appSkills = p.GetProperty("appSkills").EnumerateArray().Select(x => x.GetString()).ToList();
                    foreach (string s in appSkills)
                        t.IsTrue(ConceptIds.IsKnown(s), $"{label}: skill '{s}' is one of the app's skills");

                    var planned = p.GetProperty("plannedSkills").EnumerateArray().Select(x => x.GetString()).ToList();
                    foreach (string s in planned)
                    {
                        t.IsTrue(proposed.ContainsKey(s), $"{label}: planned skill '{s}' is in proposed-skills.json");
                        usedProposed.Add(s);
                    }

                    if (word == "good") t.IsTrue(appSkills.Count > 0 && planned.Count == 0, $"{label}: a 'good' rating names skills and needs no new skill");
                    if (word == "none") t.IsTrue(planned.Count > 0, $"{label}: a 'none' rating names the skill still to build");
                    t.IsTrue(!Text(p, "name").Contains("TODO") && !coverage.Contains("TODO"), $"{label}: no TODO left");
                }

                t.IsTrue(d.GetProperty("gapsForTheApp").GetArrayLength() >= 1, $"{soc}: the gaps for the app are listed");
                t.IsTrue(d.GetProperty("doubts").GetArrayLength() >= 1, $"{soc}: the doubts are listed");
            }

            foreach (string id in proposed.Keys)
                t.IsTrue(usedProposed.Contains(id), $"proposed skill '{id}' is needed by at least one procedure");
        }

        private static void AdjacentFile(TestContext t, string folder)
        {
            t.Begin("adjacent occupations");

            var d = JsonDocument.Parse(File.ReadAllText(Path.Combine(folder, "data", "adjacent-occupations.json"))).RootElement;
            var occupations = d.GetProperty("occupations").EnumerateArray().ToList();
            t.Equal(18, occupations.Count, "18 adjacent occupations");
            foreach (var o in occupations)
            {
                string soc = Text(o, "soc");
                t.IsTrue(System.Text.RegularExpressions.Regex.IsMatch(soc, @"^\d\d-\d{4}(\.\d\d)?$"), $"{soc}: a valid O*NET-SOC code");
                t.IsTrue(Text(o, "verdict").Length > 5, $"{soc}: has a verdict");
                foreach (var s in o.GetProperty("appSkillsThatFit").EnumerateArray())
                    t.IsTrue(ConceptIds.IsKnown(s.GetString()), $"{soc}: fitting skill '{s.GetString()}' is one of the app's skills");
            }
            t.IsTrue(d.GetProperty("headlineChecks").GetArrayLength() >= 3, "the market document's claims were checked");
        }
    }
}
