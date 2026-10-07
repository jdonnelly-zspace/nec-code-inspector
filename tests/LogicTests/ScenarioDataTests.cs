using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Validates the scenario JSON files with the same validator the Unity importer uses,
    /// plus cross-file checks the importer cannot do.
    /// </summary>
    public static class ScenarioDataTests
    {
        public static void Run(TestContext t)
        {
            ValidatorCatchesBadData(t);
            ContentFilesAreValid(t);
        }

        private static void ValidatorCatchesBadData(TestContext t)
        {
            t.Begin("scenario validator");

            t.IsTrue(ScenarioFileValidator.Validate(null).Count > 0, "null file is rejected");

            var good = MakeValid();
            t.Equal(0, ScenarioFileValidator.Validate(good).Count, "a valid file has no errors");

            var badConcept = MakeValid();
            badConcept.violations[0].conceptId = "not-a-concept";
            t.IsTrue(HasError(badConcept, "conceptId"), "unknown concept is rejected");

            var badSeverity = MakeValid();
            badSeverity.violations[0].severity = "Catastrophic";
            t.IsTrue(HasError(badSeverity, "severity"), "unknown severity is rejected");

            var numericSeverity = MakeValid();
            numericSeverity.violations[0].severity = "1";
            t.IsTrue(HasError(numericSeverity, "severity"), "numeric severity is rejected");

            var badDifficulty = MakeValid();
            badDifficulty.violations[0].minimumDifficulty = "Hard";
            t.IsTrue(HasError(badDifficulty, "minimumDifficulty"), "unknown difficulty is rejected");

            var duplicate = MakeValid();
            duplicate.violations = new[] { duplicate.violations[0], Clone(duplicate.violations[0]) };
            t.IsTrue(HasError(duplicate, "duplicate"), "duplicate violation id is rejected");

            var badName = MakeValid();
            badName.violationFolder = "../Escape";
            t.IsTrue(HasError(badName, "violationFolder"), "unsafe folder name is rejected");

            var noViolations = MakeValid();
            noViolations.violations = new ViolationFileData[0];
            t.IsTrue(HasError(noViolations, "violations is empty"), "empty violation list is rejected");

            var noCitation = MakeValid();
            noCitation.violations[0].necArticle = "";
            t.IsTrue(HasError(noCitation, "necArticle"), "missing citation is rejected");
        }

        private static void ContentFilesAreValid(TestContext t)
        {
            t.Begin("scenario content files");

            string dir = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Scenarios");
            string[] files = Directory.GetFiles(dir, "*.json").OrderBy(f => f, StringComparer.Ordinal).ToArray();
            t.IsTrue(files.Length > 0, "at least one scenario file exists");

            var options = new JsonSerializerOptions { IncludeFields = true };
            var scenarioIds = new HashSet<string>();
            var violationIds = new HashSet<string>();
            var assetNames = new HashSet<string>();
            var conceptsUsed = new HashSet<string>();
            var articleRefs = ArticleDatabaseTests.LoadReferences();

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);

                foreach (string error in ScenarioFileValidator.Validate(data))
                    t.IsTrue(false, $"{name}: {error}");
                if (data == null || data.violations == null) continue;

                t.IsTrue(scenarioIds.Add(data.id), $"{name}: scenario id '{data.id}' is unique across files");

                foreach (var v in data.violations)
                {
                    t.IsTrue(violationIds.Add(v.violationId), $"{name}: violationId '{v.violationId}' is unique across files");
                    t.IsTrue(assetNames.Add($"{data.violationFolder}/VD_{data.assetPrefix}_{v.violationId}"),
                        $"{name}: asset path for '{v.violationId}' is unique");
                    conceptsUsed.Add(v.conceptId);

                    t.IsTrue(ArticleDatabaseTests.Resolves(articleRefs, v.necArticle),
                        $"{name}: {v.violationId} cites '{v.necArticle}' which is not in nec_articles.json");
                }
            }

            // Every defined concept should be exercised by at least one violation
            foreach (string concept in ConceptIds.All)
                if (!conceptsUsed.Contains(concept))
                    t.Warn($"concept '{concept}' is not used by any scenario violation");
        }

        private static ScenarioFileData MakeValid()
        {
            return new ScenarioFileData
            {
                id = "scenario-test",
                sceneName = "TestScene",
                displayName = "Test",
                assetPrefix = "T",
                violationFolder = "Test",
                scenarioAssetName = "Test",
                availableDifficulties = new[] { "Beginner", "Standard", "Expert" },
                violations = new[]
                {
                    new ViolationFileData
                    {
                        violationId = "T-001",
                        conceptId = ConceptIds.ShockProtection,
                        description = "d",
                        necArticle = "210.8",
                        severity = "Major",
                        minimumDifficulty = "Beginner",
                        componentObjectName = "Obj"
                    }
                }
            };
        }

        private static ViolationFileData Clone(ViolationFileData v)
        {
            return JsonSerializer.Deserialize<ViolationFileData>(
                JsonSerializer.Serialize(v, new JsonSerializerOptions { IncludeFields = true }),
                new JsonSerializerOptions { IncludeFields = true });
        }

        private static bool HasError(ScenarioFileData data, string fragment)
        {
            return ScenarioFileValidator.Validate(data).Any(e => e.Contains(fragment));
        }
    }
}
