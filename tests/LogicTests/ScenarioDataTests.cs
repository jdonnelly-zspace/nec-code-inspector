using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
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
            SceneFactRules(t);
            ContentFilesAreValid(t);
            SharedNumericViolationsHaveSceneFacts(t);
            TermTokensInContent(t);
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
            noCitation.violations[0].citations = new ViolationCitation[0];
            t.IsTrue(HasError(noCitation, "citations is empty"), "violation without any citation is rejected");

            var noReference = MakeValid();
            noReference.violations[0].citations[0].reference = "";
            t.IsTrue(HasError(noReference, "citation reference"), "citation without a reference is rejected");

            var badProfile = MakeValid();
            badProfile.violations[0].citations[0].profileId = "bad profile!";
            t.IsTrue(HasError(badProfile, "citation profileId"), "citation with an unsafe profile id is rejected");

            var twoForOneProfile = MakeValid();
            twoForOneProfile.violations[0].citations = new[]
            {
                new ViolationCitation { profileId = CodeProfileIds.Nec, reference = "210.8" },
                new ViolationCitation { profileId = CodeProfileIds.Nec, reference = "210.12" }
            };
            t.IsTrue(HasError(twoForOneProfile, "more than one citation"), "two citations for one profile are rejected");

            var twoProfiles = MakeValid();
            twoProfiles.violations[0].citations = new[]
            {
                new ViolationCitation { profileId = CodeProfileIds.Nec, reference = "210.8" },
                new ViolationCitation { profileId = "cec", reference = "26-700" }
            };
            t.Equal(0, ScenarioFileValidator.Validate(twoProfiles).Count, "citations for different profiles are accepted");
        }

        // Violations that mix codes need scene values that break every code (docs/SCENE_DESIGN.md)
        private static ScenarioFileData WithFacts(float sceneValue, float necLimit, float cecLimitMetres, float compliantValue = 5)
        {
            var data = MakeValid();
            var v = data.violations[0];
            v.sceneFact = new SceneFact { quantity = "q", value = sceneValue, unit = "ft" };
            v.compliantFact = new SceneFact { quantity = "q", value = compliantValue, unit = "ft" };
            v.citations = new[]
            {
                new ViolationCitation { profileId = CodeProfileIds.Nec, reference = "210.8", text = "t",
                    limit = new SceneLimit { quantity = "q", kind = "max", value = necLimit, unit = "ft" } },
                new ViolationCitation { profileId = "cec", reference = "26-700", text = "t",
                    limit = new SceneLimit { quantity = "q", kind = "max", value = cecLimitMetres, unit = "m" } }
            };
            return data;
        }

        private static void SceneFactRules(TestContext t)
        {
            t.Begin("scene value rules");

            t.Equal(0, ScenarioFileValidator.Validate(WithFacts(7, 6, 1.8f)).Count, "a value past every limit by the margin is accepted");

            // 6.3 ft is 5% past 6 ft and 6.3 ft = 1.92 m is 6.7% past 1.8 m
            t.IsTrue(HasError(WithFacts(6.3f, 6, 1.8f), "only"), "a value less than the margin past a limit is rejected");

            // 6.9 ft is 15% past 6 ft but 2.10 m is 16.8% past 1.8 m: both fine; 6.5 ft is 8% past the NEC limit only
            t.IsTrue(HasError(WithFacts(6.5f, 6, 3.0f), "'cec'"), "breaking one code but not the other is rejected, naming the code");

            // A compliant value that breaks one of the codes
            t.IsTrue(HasError(WithFacts(7, 6, 1.8f, 6.5f), "compliant value"), "a compliant value that breaks a code is rejected");

            var noLimit = WithFacts(7, 6, 1.8f);
            noLimit.violations[0].citations[1].limit = null;
            t.IsTrue(HasError(noLimit, "has no limit"), "a citation without a limit is rejected when the scene has a value");

            var limitOnly = WithFacts(7, 6, 1.8f);
            limitOnly.violations[0].sceneFact = null;
            t.IsTrue(HasError(limitOnly, "no sceneFact"), "a limit without a scene value is rejected");

            var orphanCompliant = MakeValid();
            orphanCompliant.violations[0].compliantFact = new SceneFact { quantity = "q", value = 1, unit = "ft" };
            t.IsTrue(HasError(orphanCompliant, "compliantFact needs"), "a compliant value without a scene value is rejected");

            var wrongQuantity = WithFacts(7, 6, 1.8f);
            wrongQuantity.violations[0].citations[0].limit.quantity = "other";
            t.IsTrue(HasError(wrongQuantity, "scene measures"), "a limit on another quantity is rejected");

            var badUnit = WithFacts(7, 6, 1.8f);
            badUnit.violations[0].citations[0].limit.unit = "cubits";
            t.IsTrue(HasError(badUnit, "unknown unit"), "an unknown unit is rejected");

            var badKind = WithFacts(7, 6, 1.8f);
            badKind.violations[0].citations[0].limit.kind = "about";
            t.IsTrue(HasError(badKind, "unknown unit or kind"), "an unknown limit kind is rejected");

            // A minimum limit works the other way: the scene value must be below it by the margin
            var minimum = WithFacts(6, 8, 3);
            minimum.violations[0].compliantFact.value = 10;
            foreach (var c in minimum.violations[0].citations) c.limit.kind = "min";
            minimum.violations[0].citations[1].limit.unit = "m";
            t.Equal(0, ScenarioFileValidator.Validate(minimum).Count, "a value under every minimum by the margin is accepted");
            minimum.violations[0].sceneFact.value = 7.5f;
            t.IsTrue(HasError(minimum, "only"), "a value only slightly under a minimum is rejected");

            t.IsTrue(SceneFacts.TryBreach(new SceneFact { quantity = "q", value = 12, unit = "in" },
                new SceneLimit { quantity = "q", kind = "max", value = 1, unit = "ft" }, out double zero) && Math.Abs(zero) < 1e-9,
                "units are converted before comparing");
        }

        // {code} and {term:key} tokens in scenario content are filled from the active code's terminology.
        // A key that no profile defines would show up as the raw key, so every key must exist in the NEC terms
        // (the reference set); other codes that lack a key get a warning because they would fall back to it.
        private static void TermTokensInContent(TestContext t)
        {
            t.Begin("term tokens in scenario content");

            string dir = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Scenarios");
            var options = new JsonSerializerOptions { IncludeFields = true };
            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot());
            var necKeys = new HashSet<string>(
                profiles.First(p => p.folder == CodeProfileIds.Nec).terminology.terms.Select(e => e.key));

            var used = new SortedSet<string>();
            foreach (string file in Directory.GetFiles(dir, "*.json"))
            {
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                var texts = new List<string> { data.displayName, data.description, data.environmentDescription };
                foreach (var v in data.violations)
                {
                    texts.AddRange(new[] { v.description, v.hintText, v.inspectionNote });
                    foreach (var c in v.citations)
                        texts.AddRange(new[] { c.text, c.description, c.hintText, c.inspectionNote });
                }

                foreach (string text in texts.Where(x => !string.IsNullOrEmpty(x)))
                    foreach (System.Text.RegularExpressions.Match m in System.Text.RegularExpressions.Regex.Matches(text, @"\{term:([^}]*)\}"))
                        used.Add(m.Groups[1].Value);
            }

            foreach (string key in used)
            {
                t.IsTrue(necKeys.Contains(key), $"term token '{{term:{key}}}' is a term the NEC profile defines");
                foreach (var p in profiles.Where(p => p.terminology != null && !p.terminology.terms.Any(e => e.key == key)))
                    t.Warn($"term token '{{term:{key}}}' has no entry in Codes/{p.folder}/terminology.json, so it would read as '{key}'");
            }

            t.IsTrue(used.Count > 0, "scenario content uses at least one term token (so this check has something to check)");
        }

        private static void SharedNumericViolationsHaveSceneFacts(TestContext t)
        {
            t.Begin("scene values on shared violations");

            string dir = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Scenarios");
            var options = new JsonSerializerOptions { IncludeFields = true };
            var all = Directory.GetFiles(dir, "*.json")
                .SelectMany(f => JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(f), options).violations)
                .ToDictionary(v => v.violationId);

            // Violations whose rule is a single number that differs between codes
            string[] numeric = { "BC-SPACING-WALL-001", "BC-SPACING-COUNTER-001", "GND-ELECTRODE-001", "RP-CLEAR-FRONT-001" };
            foreach (string id in numeric)
            {
                t.IsTrue(all.ContainsKey(id), $"{id} exists");
                if (!all.ContainsKey(id)) continue;

                var v = all[id];
                t.IsTrue(v.sceneFact != null && v.sceneFact.IsSet, $"{id} has a scene value");
                t.IsTrue(v.compliantFact != null && v.compliantFact.IsSet, $"{id} has a compliant value");
                t.IsTrue(v.citations.Length >= 2 && v.citations.All(c => c.limit != null && c.limit.IsSet),
                    $"{id} has a limit for every code it lists");
            }
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
            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot());

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

                    foreach (var citation in v.citations ?? new ViolationCitation[0])
                    {
                        var profile = profiles.FirstOrDefault(p => p.Id == citation.profileId);
                        t.IsTrue(profile != null, $"{name}: {v.violationId} cites unknown code profile '{citation.profileId}'");
                        if (profile == null) continue;

                        t.IsTrue(profile.HasReference(citation.reference),
                            $"{name}: {v.violationId} cites '{citation.reference}' which is not in Codes/{profile.Id}/articles.json");
                    }
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
                        citations = new[] { new ViolationCitation { profileId = CodeProfileIds.Nec, reference = "210.8", text = "t" } },
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
