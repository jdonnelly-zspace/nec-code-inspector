using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Which skills the scenario content can teach under each installation code, and at what tier.
    /// The app has no credentials, so this is the measure of how complete a code's content is.
    /// </summary>
    public static class SkillCoverageTests
    {
        public static void Run(TestContext t)
        {
            t.Begin("skill coverage");

            string root = TestContext.RepoRoot();
            var options = new JsonSerializerOptions { IncludeFields = true };
            var scenarios = Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json")
                .Select(f => JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(f), options))
                .Where(s => s?.violations != null)
                .ToList();

            foreach (var profile in ProfileFiles.LoadAll(root))
            {
                var tiersBySkill = EvidenceTiers(scenarios, profile.Id);
                var without = new List<string>();
                var foundationOnly = new List<string>();

                foreach (string skill in ConceptIds.All)
                {
                    if (!tiersBySkill.TryGetValue(skill, out var tiers)) without.Add(skill);
                    else if (tiers.All(x => x < (int)SkillTier.Practitioner)) foundationOnly.Add(skill);
                }

                if (profile.Id == CodeProfileIds.Nec)
                {
                    // The default code must be able to teach every skill up to Practitioner level
                    foreach (string skill in without.Concat(foundationOnly))
                        t.IsTrue(false, $"the {profile.Id} content has no Practitioner-level violation for {skill}");
                }
                else
                {
                    if (without.Count > 0)
                        t.Warn($"{profile.Id}: no violations yet for {string.Join(", ", without)}");
                    if (foundationOnly.Count > 0)
                        t.Warn($"{profile.Id}: only Foundation-level violations for {string.Join(", ", foundationOnly)}");
                }
            }
        }

        // For each skill, the tiers of evidence the violations that apply to one code can produce
        private static Dictionary<string, HashSet<int>> EvidenceTiers(List<ScenarioFileData> scenarios, string codeProfileId)
        {
            var result = new Dictionary<string, HashSet<int>>();

            foreach (var scenario in scenarios)
            {
                foreach (var v in scenario.violations)
                {
                    if (ViolationCitations.Find(v.citations, codeProfileId) == null) continue;
                    if (!Enum.TryParse<DifficultyLevel>(v.minimumDifficulty, out var difficulty)) continue;

                    if (!result.TryGetValue(v.conceptId, out var tiers))
                        result[v.conceptId] = tiers = new HashSet<int>();
                    tiers.Add((int)SkillTiers.FromDifficulty(difficulty));
                }
            }

            return result;
        }
    }
}
