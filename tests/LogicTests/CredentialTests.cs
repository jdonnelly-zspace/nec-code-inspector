using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Core;
using NECInspector.Credentials;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    public static class CredentialTests
    {
        public static void Run(TestContext t)
        {
            ValidatorCatchesBadData(t);
            ReadinessMath(t);
            DeferredRequirements(t);
            CredentialFilesAreValid(t);
        }

        private static CredentialProfile MakeCredential()
        {
            return new CredentialProfile
            {
                id = "test-cred",
                displayName = "Test",
                codeProfileId = "nec",
                requirements = new[]
                {
                    new CredentialRequirement { skillId = ConceptIds.ShockProtection, tier = "Practitioner", weight = 1f },
                    new CredentialRequirement { skillId = ConceptIds.EarthingBonding, tier = "Foundation", weight = 3f }
                }
            };
        }

        private static bool HasError(CredentialProfile profile, string fragment)
        {
            return CredentialValidator.Validate(profile).Any(e => e.Contains(fragment));
        }

        private static void ValidatorCatchesBadData(TestContext t)
        {
            t.Begin("credential validator");

            t.IsTrue(CredentialValidator.Validate(null).Count > 0, "null file is rejected");
            t.Equal(0, CredentialValidator.Validate(MakeCredential()).Count, "a valid credential has no errors");

            var badId = MakeCredential(); badId.id = "../x";
            t.IsTrue(HasError(badId, "id must"), "unsafe id is rejected");

            var noCode = MakeCredential(); noCode.codeProfileId = "";
            t.IsTrue(HasError(noCode, "codeProfileId"), "missing code profile is rejected");

            var badSkill = MakeCredential(); badSkill.requirements[0].skillId = "welding";
            t.IsTrue(HasError(badSkill, "unknown skillId"), "unknown skill is rejected");

            var badTier = MakeCredential(); badTier.requirements[0].tier = "Master";
            t.IsTrue(HasError(badTier, "unknown tier"), "unknown tier is rejected");

            var numericTier = MakeCredential(); numericTier.requirements[0].tier = "1";
            t.IsTrue(HasError(numericTier, "unknown tier"), "numeric tier is rejected");

            var zeroWeight = MakeCredential(); zeroWeight.requirements[0].weight = 0f;
            t.IsTrue(HasError(zeroWeight, "weight"), "zero weight is rejected");

            var duplicate = MakeCredential(); duplicate.requirements[1].skillId = duplicate.requirements[0].skillId;
            t.IsTrue(HasError(duplicate, "more than once"), "duplicate skill is rejected");

            var empty = MakeCredential(); empty.requirements = new CredentialRequirement[0];
            t.IsTrue(HasError(empty, "requirements is empty"), "no requirements is rejected");

            var badLabels = MakeCredential(); badLabels.tierLabels = new[] { "A", "B" };
            t.IsTrue(HasError(badLabels, "tierLabels"), "wrong number of tier labels is rejected");

            var goodLabels = MakeCredential(); goodLabels.tierLabels = new[] { "Beginner", "Standard", "Expert" };
            t.Equal(0, CredentialValidator.Validate(goodLabels).Count, "three tier labels are accepted");
            t.Equal("Standard", goodLabels.GetTierLabel(SkillTier.Practitioner), "tier label comes from the credential");
            t.Equal("Authority", MakeCredential().GetTierLabel(SkillTier.Authority), "default tier label");
        }

        private static void ReadinessMath(TestContext t)
        {
            t.Begin("credential readiness");

            var credential = MakeCredential();

            var none = CredentialReadiness.Evaluate(credential, new SkillProgress());
            t.Near(0, none.percent, "no progress is 0%");
            t.Equal(0, none.attainedCount, "nothing attained");
            t.Equal(2, none.TotalCount, "two requirements");
            t.IsTrue(!none.IsComplete, "not complete");

            var progress = new SkillProgress();
            for (int i = 0; i < SkillPolicy.MinAttempts; i++)
                progress.Record(new SkillEvidence(ConceptIds.EarthingBonding, SkillTier.Foundation, 1f, "x"));

            var partial = CredentialReadiness.Evaluate(credential, progress);
            t.Equal(1, partial.attainedCount, "earthing attained");
            t.Near(0.75, partial.percent, "weighted: earthing weight 3 of 4");

            // One attempt at the right tier: partial credit toward shock protection
            progress.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Practitioner, 1f, "x"));
            var started = CredentialReadiness.Evaluate(credential, progress);
            double expectedFraction = 1.0 * (1.0 / SkillPolicy.MinAttempts);
            t.Near((3 + expectedFraction) / 4, started.percent, "one attempt earns a third of that requirement");
            t.IsTrue(!started.requirements[0].attained, "not yet attained");

            // Evidence at a lower tier than required does not count
            var wrongTier = new SkillProgress();
            for (int i = 0; i < 5; i++)
                wrongTier.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Foundation, 1f, "x"));
            t.Near(0, CredentialReadiness.Evaluate(credential, wrongTier).requirements[0].fraction, "Foundation evidence does not count toward Practitioner");

            // Evidence at a higher tier does
            var higher = new SkillProgress();
            for (int i = 0; i < SkillPolicy.MinAttempts; i++)
                higher.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Authority, 1f, "x"));
            t.IsTrue(CredentialReadiness.Evaluate(credential, higher).requirements[0].attained, "Authority evidence satisfies a Practitioner requirement");

            // The same progress is a view for any credential: nothing is stored per credential
            var other = new CredentialProfile
            {
                id = "other", displayName = "Other", codeProfileId = "cec",
                requirements = new[] { new CredentialRequirement { skillId = ConceptIds.EarthingBonding, tier = "Practitioner", weight = 1f } }
            };
            t.IsTrue(!CredentialReadiness.Evaluate(other, progress).requirements[0].attained, "Foundation-level earthing does not meet a Practitioner requirement elsewhere");
            t.IsTrue(CredentialReadiness.Evaluate(credential, progress).requirements[1].attained, "while the first credential sees the same skill as attained");

            var all = new SkillProgress();
            foreach (var r in credential.requirements)
                for (int i = 0; i < SkillPolicy.MinAttempts; i++)
                    all.Record(new SkillEvidence(r.skillId, SkillTier.Authority, 1f, "x"));
            var complete = CredentialReadiness.Evaluate(credential, all);
            t.IsTrue(complete.IsComplete, "all requirements attained");
            t.Near(1, complete.percent, "100% when complete");

            t.Near(0, CredentialReadiness.Evaluate(null, progress).percent, "null credential");
            t.Equal(2, CredentialReadiness.Evaluate(credential, null).TotalCount, "null progress still lists requirements");
        }

        private static void DeferredRequirements(TestContext t)
        {
            t.Begin("credential deferred requirements");

            var credential = new CredentialProfile
            {
                id = "partial", displayName = "Partial", codeProfileId = "cec",
                requirements = new[]
                {
                    new CredentialRequirement { skillId = ConceptIds.ShockProtection, tier = "Practitioner", weight = 3f },
                    new CredentialRequirement { skillId = ConceptIds.EarthingBonding, tier = "Practitioner", weight = 1f, deferred = true },
                    new CredentialRequirement { skillId = ConceptIds.LoadCalculation, tier = "Practitioner", weight = 4f, deferred = true }
                }
            };
            t.Equal(0, CredentialValidator.Validate(credential).Count, "deferred requirements are valid");

            var none = CredentialReadiness.Evaluate(credential, new SkillProgress());
            t.Equal(1, none.TotalCount, "only requirements the app can teach are listed");
            t.Equal(2, none.deferredCount, "deferred requirements are counted separately");
            t.Near(3.0 / 8.0, none.coverage, "coverage is the weight the app can teach");
            t.Near(0, none.percent, "no progress is 0%");

            var progress = new SkillProgress();
            for (int i = 0; i < SkillPolicy.MinAttempts; i++)
                progress.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Practitioner, 1f, "x"));
            var done = CredentialReadiness.Evaluate(credential, progress);
            t.Near(1, done.percent, "readiness counts only what the app can teach");
            t.IsTrue(done.IsComplete, "complete when every teachable requirement is attained");

            // Evidence for a deferred skill is still recorded, it just does not count toward the credential yet
            for (int i = 0; i < SkillPolicy.MinAttempts; i++)
                progress.Record(new SkillEvidence(ConceptIds.EarthingBonding, SkillTier.Practitioner, 1f, "x"));
            t.Equal(2, progress.AttainedSkillCount(), "the learner's skill progress still includes the deferred skill");
            t.Equal(1, CredentialReadiness.Evaluate(credential, progress).attainedCount, "but the credential does not count it");

            var allDeferred = new CredentialProfile
            {
                id = "none", displayName = "None", codeProfileId = "cec",
                requirements = new[] { new CredentialRequirement { skillId = ConceptIds.EarthingBonding, tier = "Practitioner", deferred = true } }
            };
            var empty = CredentialReadiness.Evaluate(allDeferred, new SkillProgress());
            t.Near(0, empty.coverage, "nothing teachable means no coverage");
            t.IsTrue(!empty.IsComplete, "a credential with nothing teachable is never complete");
        }

        private static void CredentialFilesAreValid(TestContext t)
        {
            t.Begin("credential files");

            string root = TestContext.RepoRoot();
            string dir = Path.Combine(root, "Assets/_Project/StreamingAssets/Credentials");
            string[] files = Directory.GetFiles(dir, "*.json").OrderBy(f => f, StringComparer.Ordinal).ToArray();
            t.IsTrue(files.Length > 0, "at least one credential file exists");

            var options = new JsonSerializerOptions { IncludeFields = true };
            var ids = new HashSet<string>();
            var codeProfiles = ProfileFiles.LoadAll(root);

            foreach (string file in files)
            {
                string name = Path.GetFileName(file);
                var profile = JsonSerializer.Deserialize<CredentialProfile>(File.ReadAllText(file), options);

                foreach (string error in CredentialValidator.Validate(profile))
                    t.IsTrue(false, $"{name}: {error}");
                if (profile == null || profile.requirements == null) continue;

                t.IsTrue(ids.Add(profile.id), $"{name}: credential id '{profile.id}' is unique");
                t.IsTrue(codeProfiles.Any(c => c.Id == profile.codeProfileId),
                    $"{name}: code profile '{profile.codeProfileId}' has no folder under StreamingAssets/Codes");

                // A credential that asks for a skill at a tier nothing can teach could never be completed.
                // Only violations that apply to the credential's own code count as evidence.
                var evidenceTiers = ScenarioEvidenceTiers(root, profile.codeProfileId);
                foreach (var r in profile.requirements)
                {
                    if (!SkillTiers.TryParse(r.tier, out var tier)) continue;
                    bool reachable = evidenceTiers.TryGetValue(r.skillId, out var tiers) && tiers.Any(x => x >= (int)tier);

                    if (r.deferred)
                    {
                        // Keep the file honest: once content exists, the requirement should stop being deferred
                        if (reachable) t.Warn($"{name}: {r.skillId} is marked deferred but content for '{profile.codeProfileId}' now covers it at {r.tier}");
                    }
                    else
                    {
                        t.IsTrue(reachable, $"{name}: no violation applying to '{profile.codeProfileId}' gives evidence for {r.skillId} at {r.tier} or above");
                    }
                }
            }

            t.IsTrue(ids.Contains("core-skills"), "the default 'core-skills' credential exists");
        }

        // For each skill, the tiers of evidence the scenario content can produce under one code profile
        private static Dictionary<string, HashSet<int>> ScenarioEvidenceTiers(string root, string codeProfileId)
        {
            var result = new Dictionary<string, HashSet<int>>();
            var options = new JsonSerializerOptions { IncludeFields = true };

            foreach (string file in Directory.GetFiles(Path.Combine(root, "Assets/_Project/Content/Scenarios"), "*.json"))
            {
                var data = JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(file), options);
                if (data?.violations == null) continue;

                foreach (var v in data.violations)
                {
                    if (!Enum.TryParse<DifficultyLevel>(v.minimumDifficulty, out var difficulty)) continue;
                    if (ViolationCitations.Find(v.citations, codeProfileId) == null) continue;
                    if (!result.TryGetValue(v.conceptId, out var tiers))
                        result[v.conceptId] = tiers = new HashSet<int>();
                    tiers.Add((int)SkillTiers.FromDifficulty(difficulty));
                }
            }

            return result;
        }
    }
}
