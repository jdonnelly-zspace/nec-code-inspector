using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using NECInspector.Codes;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.PanelSandbox;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The content that used to live in editor scripts (difficulty settings, certificates, quick reference cards,
    /// the panel sandbox design) is now JSON under Assets/_Project/Content. These tests check its shape, that every
    /// code reference and conductor size it names exists in the code it is written for, and that its numbers agree
    /// with the load calculator and with the code's GFCI and AFCI scope.
    /// </summary>
    public static class ContentFilesTests
    {
        private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true };

        public static void Run(TestContext t)
        {
            ValidatorsCatchBadData(t);
            DifficultyFile(t);
            CertificateFiles(t);
            QuickReferenceFiles(t);
            SandboxFiles(t);
            NoStatutoryWording(t);
        }

        private static string ContentDir => Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content");

        private static T Read<T>(string relative) where T : class
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(Path.Combine(ContentDir, relative)), Options);
        }

        private static void ValidatorsCatchBadData(TestContext t)
        {
            t.Begin("content validators");

            t.IsTrue(ContentFileValidator.Validate((DifficultySettingsFile)null).Count > 0, "an empty difficulty file is rejected");
            var diff = Read<DifficultySettingsFile>("Difficulty/difficulty-settings.json");
            t.Equal(0, ContentFileValidator.Validate(diff).Count, "the shipped difficulty file is valid");
            var clone = Clone(diff);
            clone.settings = clone.settings.Take(2).ToArray();
            t.IsTrue(ContentFileValidator.Validate(clone).Any(e => e.Contains("no settings")), "a missing difficulty level is rejected");
            clone = Clone(diff); clone.settings[0].citationMode = "Guess";
            t.IsTrue(ContentFileValidator.Validate(clone).Any(e => e.Contains("citationMode")), "an unknown citation mode is rejected");
            clone = Clone(diff); clone.settings[2].timeLimitSeconds = 0;
            t.IsTrue(ContentFileValidator.Validate(clone).Any(e => e.Contains("time limit")), "a time limit that is on with no seconds is rejected");

            var certs = Read<CertificateFile>("Certificates/certificates.json");
            var c = Clone(certs); c.templates[0].descriptionTemplate += " {Nickname}";
            t.IsTrue(ContentFileValidator.Validate(c).Any(e => e.Contains("placeholder")), "an unknown certificate placeholder is rejected");
            c = Clone(certs); c.templates[0].requiredSkills = new[] { "not-a-skill" };
            t.IsTrue(ContentFileValidator.Validate(c).Any(e => e.Contains("unknown skill")), "an unknown skill is rejected");
            c = Clone(certs); c.templates[3].requiredSkills = new[] { ConceptIds.ShockProtection };
            t.IsTrue(ContentFileValidator.Validate(c).Any(e => e.Contains("cannot both")), "requiresAllSkills with a list is rejected");
            c = Clone(certs); c.templates[0].accentColor = new[] { 2f, 0f, 0f, 1f };
            t.IsTrue(ContentFileValidator.Validate(c).Any(e => e.Contains("accentColor")), "a colour outside 0 to 1 is rejected");
            c = Clone(certs); c.templates[1].certificateId = c.templates[0].certificateId;
            t.IsTrue(ContentFileValidator.Validate(c).Any(e => e.Contains("duplicate")), "a duplicate certificate id is rejected");

            var cards = Read<QuickReferenceFile>("QuickReference/cards-nec.json");
            var q = Clone(cards); q.cards[0].category = "Plumbing";
            t.IsTrue(ContentFileValidator.Validate(q).Any(e => e.Contains("category")), "an unknown card category is rejected");
            q = Clone(cards); q.cards[0].codeReferences = new string[0];
            t.IsTrue(ContentFileValidator.Validate(q).Any(e => e.Contains("codeReferences")), "a card without references is rejected");

            var sandbox = Read<SandboxFile>("Sandbox/panel-designs.json");
            var s = Clone(sandbox); s.designs[0].requiredCircuits[0].poleCount = 3;
            t.IsTrue(ContentFileValidator.Validate(s).Any(e => e.Contains("poleCount")), "a three-pole circuit is rejected");
            s = Clone(sandbox); s.designs[0].requiredCircuits[1].circuitName = s.designs[0].requiredCircuits[0].circuitName;
            t.IsTrue(ContentFileValidator.Validate(s).Any(e => e.Contains("duplicate circuit")), "a duplicate circuit name is rejected");
        }

        private static void DifficultyFile(TestContext t)
        {
            t.Begin("difficulty settings file");

            var file = Read<DifficultySettingsFile>("Difficulty/difficulty-settings.json");
            foreach (string error in ContentFileValidator.Validate(file)) t.IsTrue(false, $"difficulty-settings.json: {error}");

            var expert = file.settings.Single(x => x.level == "Expert");
            var beginner = file.settings.Single(x => x.level == "Beginner");
            t.IsTrue(expert.enableTimeLimit && expert.penalizeFalsePositives && expert.citationMode == "FreeText", "Expert has a time limit, penalties and free-text citations");
            t.IsTrue(beginner.showScaffolding && beginner.showHighlightHints && !beginner.enableTimeLimit, "Beginner has hints and scaffolding and no time limit");
        }

        private static void CertificateFiles(TestContext t)
        {
            t.Begin("certificate templates file");

            var file = Read<CertificateFile>("Certificates/certificates.json");
            foreach (string error in ContentFileValidator.Validate(file)) t.IsTrue(false, $"certificates.json: {error}");

            var scenarioIds = Directory.GetFiles(Path.Combine(ContentDir, "Scenarios"), "*.json")
                .Select(f => JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(f), Options).id).ToHashSet();
            foreach (var cert in file.templates)
                foreach (string id in cert.requiredScenarios ?? new string[0])
                    t.IsTrue(scenarioIds.Contains(id), $"{cert.certificateId}: required scenario '{id}' exists");

            var overall = file.templates.Single(x => x.type == "OverallProficiency");
            t.IsTrue(overall.requiresAllSkills, "the overall certificate requires every skill");
            t.IsTrue(!file.templates.Any(x => x.certificateTitle.Contains("NEC") || x.descriptionTemplate.Contains("NEC")),
                "certificate text does not name one installation code");
            t.Equal(1, file.templates.Count(x => x.type == "SandboxProficiency" && x.requiresSandbox), "one certificate requires the sandbox");
        }

        private static void QuickReferenceFiles(TestContext t)
        {
            t.Begin("quick reference card files");

            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot());
            var ids = new HashSet<string>();

            foreach (string path in Directory.GetFiles(Path.Combine(ContentDir, "QuickReference"), "*.json"))
            {
                string name = Path.GetFileName(path);
                var file = JsonSerializer.Deserialize<QuickReferenceFile>(File.ReadAllText(path), Options);
                foreach (string error in ContentFileValidator.Validate(file)) t.IsTrue(false, $"{name}: {error}");

                foreach (var card in file.cards)
                {
                    t.IsTrue(ids.Add(card.cardId), $"{name}: card id '{card.cardId}' is unique across files");
                    var profile = profiles.FirstOrDefault(p => p.Id == card.profileId);
                    t.IsTrue(profile != null, $"{name}: {card.cardId} is written for a code that exists ('{card.profileId}')");
                    if (profile == null) continue;

                    foreach (string reference in card.codeReferences)
                        t.IsTrue(profile.HasReference(reference), $"{name}: {card.cardId} cites '{reference}' which is not in Codes/{profile.Id}/articles.json");
                }
            }

            t.IsTrue(ids.Count >= 8, "there are quick reference cards");
        }

        private static void SandboxFiles(TestContext t)
        {
            t.Begin("panel sandbox design file");

            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot());
            var file = Read<SandboxFile>("Sandbox/panel-designs.json");
            foreach (string error in ContentFileValidator.Validate(file)) t.IsTrue(false, $"panel-designs.json: {error}");

            foreach (var design in file.designs)
            {
                var profile = profiles.FirstOrDefault(p => p.Id == design.profileId);
                t.IsTrue(profile != null, $"{design.assetName}: written for a code that exists");
                if (profile == null) continue;

                var tables = profile.tables ?? ElectricalTables.CreateNecDefaults();
                tables.EnsureComplete();
                var scope = profile.scope;

                int poles = design.requiredCircuits.Where(c => c.isRequired).Sum(c => c.poleCount);
                t.IsTrue(poles <= design.totalSlots, $"{design.assetName}: the required circuits ({poles} poles) fit the panel ({design.totalSlots} spaces)");

                foreach (var c in design.requiredCircuits)
                {
                    string who = $"{design.assetName} / {c.circuitName}";
                    t.IsTrue(profile.HasReference(c.codeReference), $"{who}: cites '{c.codeReference}' which is not in Codes/{profile.Id}/articles.json");

                    int ampacity = tables.GetMaxAmps(c.wireGauge);
                    t.IsTrue(ampacity > 0, $"{who}: conductor '{c.wireGauge}' is in the code's table");
                    t.IsTrue(ampacity >= c.ampsRequired, $"{who}: {c.wireGauge} (rated {ampacity} A) carries a {c.ampsRequired} A breaker");

                    // The protection flags must agree with the code's scope for the room the circuit serves
                    if (scope != null && !string.IsNullOrEmpty(c.room) && c.poleCount == 1)
                    {
                        var receptacle = new ScopeReceptacle { id = c.circuitName, room = c.room, amps = c.ampsRequired, appliance = c.appliance ?? "" };
                        t.Equal(ProtectionScopeChecker.Check(scope, receptacle, "gfci").required, c.requiresGFCI, $"{who}: GFCI flag matches the {profile.Id} scope for a {c.room}");
                        t.Equal(ProtectionScopeChecker.Check(scope, receptacle, "afci").required, c.requiresAFCI, $"{who}: AFCI flag matches the {profile.Id} scope for a {c.room}");
                    }
                }

                // The expected load is the dwelling service calculation for the design's area (NEC defaults here)
                if (profile.Id == CodeProfileIds.Nec)
                {
                    float expected = LoadCalculator.CalculateTotalServiceLoad(design.dwellingArea);
                    t.IsTrue(Math.Abs(expected - design.targetLoadVA) < 1f, $"{design.assetName}: targetLoadVA {design.targetLoadVA} equals the calculated service load {expected}");
                    t.IsTrue(LoadCalculator.GetMinimumServiceAmps(design.targetLoadVA) <= design.totalAmps, $"{design.assetName}: the service is big enough for the target load");
                }
            }
        }

        // Statutory wording is a sign that text was copied from a code book (docs/CONTENT_POLICY.md)
        private static void NoStatutoryWording(TestContext t)
        {
            t.Begin("content wording");

            var texts = new List<(string where, string text)>();
            foreach (var c in Read<CertificateFile>("Certificates/certificates.json").templates)
                texts.Add(($"certificate {c.certificateId}", c.certificateTitle + " " + c.descriptionTemplate));
            foreach (string path in Directory.GetFiles(Path.Combine(ContentDir, "QuickReference"), "*.json"))
                foreach (var c in JsonSerializer.Deserialize<QuickReferenceFile>(File.ReadAllText(path), Options).cards)
                    texts.Add(($"card {c.cardId}", c.title + " " + c.summary + " " + c.keyRule));
            foreach (var d in Read<SandboxFile>("Sandbox/panel-designs.json").designs)
            {
                texts.Add(($"design {d.assetName}", d.displayName + " " + d.description));
                foreach (var c in d.requiredCircuits) texts.Add(($"circuit {c.circuitName}", c.description));
            }

            foreach (var (where, text) in texts)
            {
                t.IsTrue(!Regex.IsMatch(text, @"\bshall\b", RegexOptions.IgnoreCase), $"{where}: no statutory wording");
                t.IsTrue(!Regex.IsMatch(text, @"\b2026 NEC (requires|added|requirement)", RegexOptions.IgnoreCase), $"{where}: makes no unverified claim about what is new in 2026");
            }
        }

        private static T Clone<T>(T value)
        {
            return JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value, Options), Options);
        }
    }
}
