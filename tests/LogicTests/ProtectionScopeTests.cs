using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The GFCI and AFCI scope data and checker: where each code requires protection in a dwelling, and whether a
    /// scene's offending receptacle really breaks every code its violation cites.
    /// </summary>
    public static class ProtectionScopeTests
    {
        public static void Run(TestContext t)
        {
            CheckerBehaviour(t);
            Exemptions(t);
            ValidatorCatchesBadData(t);
            RealFilesAreValid(t);
            ViolationScenesBreakEveryCitedCode(t);
        }

        private static ScopeReceptacle Receptacle(string room, float volts = 125f, float amps = 15f, params string[] protectedBy)
        {
            return new ScopeReceptacle { id = "r", room = room, volts = volts, amps = amps, protectedBy = protectedBy };
        }

        private static ScopeFixtureDistance[] Near(string type, float m) => new[] { new ScopeFixtureDistance { type = type, distanceM = m } };

        private static ProtectionScope Load(string code)
        {
            return ProfileFiles.Load(TestContext.RepoRoot(), code).scope;
        }

        private static void CheckerBehaviour(TestContext t)
        {
            t.Begin("protection scope checker");

            var nec = Load(CodeProfileIds.Nec);
            var cec = Load("cec");

            // Rooms (NEC) against distance (CEC)
            var bath = Receptacle("bathroom");
            t.IsTrue(ProtectionScopeChecker.Check(nec, bath, "gfci").required, "NEC: every bathroom receptacle needs GFCI");
            t.IsTrue(!ProtectionScopeChecker.Check(cec, bath, "gfci").required, "CEC: a bathroom receptacle far from the fixtures does not by distance");
            bath.fixtures = Near("sink", 0.6f);
            t.IsTrue(ProtectionScopeChecker.Check(cec, bath, "gfci").required, "CEC: within 1.5 m of a sink it does");

            var edge = Receptacle("other"); edge.fixtures = Near("sink", 1.5f);
            t.IsTrue(ProtectionScopeChecker.Check(cec, edge, "gfci").required, "a distance exactly at the limit is within it");
            edge.fixtures = Near("sink", 1.51f);
            t.IsTrue(!ProtectionScopeChecker.Check(cec, edge, "gfci").required, "a hair beyond the limit is not");

            // Kitchen far from the sink, laundry without a tub, dishwasher: NEC only
            var kitchen = Receptacle("kitchen"); kitchen.onCounter = true; kitchen.fixtures = Near("sink", 2.4f);
            t.IsTrue(ProtectionScopeChecker.Check(nec, kitchen, "gfci").required, "NEC: kitchen receptacles need GFCI");
            t.IsTrue(!ProtectionScopeChecker.Check(cec, kitchen, "gfci").required, "CEC: a kitchen receptacle 2.4 m from the sink is not covered");
            var laundry = Receptacle("laundry");
            t.IsTrue(ProtectionScopeChecker.Check(nec, laundry, "gfci").required && !ProtectionScopeChecker.Check(cec, laundry, "gfci").required, "laundry without a tub: NEC only");
            var dishwasher = Receptacle("other"); dishwasher.appliance = "dishwasher";
            t.Equal("210.8(D)", ProtectionScopeChecker.Check(nec, dishwasher, "gfci").reference.Substring(0, 8), "NEC: the dishwasher rule applies");

            // The sink rule excludes kitchens in the NEC (kitchens have their own item) and needs 6 ft
            var sinkRoom = Receptacle("other"); sinkRoom.fixtures = Near("sink", 1.8f);
            t.Equal("gfci-sinks", ProtectionScopeChecker.Check(nec, sinkRoom, "gfci").ruleId, "NEC: within 6 ft of a sink outside the kitchen");
            sinkRoom.fixtures = Near("sink", 1.9f);
            t.IsTrue(!ProtectionScopeChecker.Check(nec, sinkRoom, "gfci").required, "NEC: beyond 6 ft is not");

            // Outdoors: NEC always, CEC only within 2.5 m of grade
            var deck = Receptacle("outdoors"); deck.heightAboveGradeM = 4f;
            t.IsTrue(ProtectionScopeChecker.Check(nec, deck, "gfci").required, "NEC: outdoors always");
            t.IsTrue(!ProtectionScopeChecker.Check(cec, deck, "gfci").required, "CEC: a receptacle 4 m above grade is not covered");
            deck.heightAboveGradeM = 1f;
            t.IsTrue(ProtectionScopeChecker.Check(cec, deck, "gfci").required, "CEC: 1 m above grade is");

            // Crawl spaces and accessory buildings only below grade
            var crawl = Receptacle("crawl-space");
            t.IsTrue(!ProtectionScopeChecker.Check(nec, crawl, "gfci").required, "a crawl space above grade is not covered");
            crawl.floorAtOrBelowGrade = true;
            t.IsTrue(ProtectionScopeChecker.Check(nec, crawl, "gfci").required, "a crawl space at or below grade is");

            // Voltage and ampere ranges
            var big = Receptacle("bedroom", 125f, 30f);
            t.IsTrue(!ProtectionScopeChecker.Check(nec, big, "afci").required, "AFCI covers 15 and 20 A circuits, not 30 A");
            t.IsTrue(ProtectionScopeChecker.Check(nec, Receptacle("bedroom", 125f, 20f), "afci").required, "a 20 A bedroom circuit needs AFCI");
            t.IsTrue(!ProtectionScopeChecker.Check(nec, Receptacle("bathroom", 125f, 15f), "afci").required, "NEC AFCI does not list bathrooms");
            t.IsTrue(ProtectionScopeChecker.Check(cec, Receptacle("bathroom", 125f, 15f), "afci").required, "CEC AFCI is not room based, so a bathroom is in scope");
            t.IsTrue(ProtectionScopeChecker.Check(nec, Receptacle("bathroom", 250f, 50f), "gfci").required, "a 250 V receptacle in a bathroom still needs GFCI");

            // Protection provided
            var bed = Receptacle("bedroom");
            t.IsTrue(ProtectionScopeChecker.Check(nec, bed, "afci").Missing, "an unprotected bedroom circuit is missing AFCI");
            bed.protectedBy = new[] { "afci" };
            t.IsTrue(!ProtectionScopeChecker.Check(nec, bed, "afci").Missing, "an AFCI breaker satisfies it");
            bed.protectedBy = new[] { "gfci" };
            t.IsTrue(ProtectionScopeChecker.Check(nec, bed, "afci").Missing, "a GFCI does not satisfy AFCI");
            bed.protectedBy = new[] { "dual" };
            t.IsTrue(!ProtectionScopeChecker.Check(nec, bed, "afci").Missing && ProtectionScopeChecker.IsProvided(bed, "gfci"), "a dual-function device counts as both");

            var scene = new[] { Receptacle("bedroom"), Receptacle("closet", 125f, 15f, "afci"), Receptacle("kitchen") };
            t.Equal(3, ProtectionScopeChecker.MissingProtection(nec, scene).Count, "a scene lists each missing protection: bedroom AFCI, kitchen GFCI and kitchen AFCI");
        }

        private static void Exemptions(TestContext t)
        {
            t.Begin("protection scope exemptions");
            var cec = Load("cec");

            var fridge = Receptacle("kitchen"); fridge.appliance = "refrigerator";
            t.IsTrue(ProtectionScopeChecker.Check(cec, fridge, "afci").exempt && !ProtectionScopeChecker.Check(cec, fridge, "afci").required, "CEC: a refrigerator receptacle is exempt from AFCI");
            var counter = Receptacle("kitchen"); counter.onCounter = true;
            t.IsTrue(ProtectionScopeChecker.Check(cec, counter, "afci").exempt, "CEC: a kitchen counter receptacle is exempt from AFCI");
            var basin = Receptacle("bathroom"); basin.fixtures = Near("sink", 1.0f);
            t.IsTrue(ProtectionScopeChecker.Check(cec, basin, "afci").exempt, "CEC: a bathroom receptacle within 1 m of the basin is exempt");
            basin.fixtures = Near("sink", 1.2f);
            t.IsTrue(ProtectionScopeChecker.Check(cec, basin, "afci").required, "CEC: 1.2 m from the basin is not exempt");
            var washer = Receptacle("laundry"); washer.behindAppliance = true; washer.fixtures = Near("sink", 0.5f);
            t.IsTrue(ProtectionScopeChecker.Check(cec, washer, "gfci").exempt, "CEC: a receptacle behind a fixed appliance is exempt from GFCI");
            t.IsTrue(!ProtectionScopeChecker.Check(cec, washer, "gfci").required, "an exempt receptacle is not required");
        }

        private static void ValidatorCatchesBadData(TestContext t)
        {
            t.Begin("protection scope validator");

            t.IsTrue(ProtectionScopeValidator.Validate(null).Count > 0, "an empty scope is rejected");
            t.Equal(0, ProtectionScopeValidator.Validate(new ProtectionScope { rules = new ScopeRule[0] }).Count, "a scope with no rules is valid");

            Func<ScopeRule, bool> bad = r => ProtectionScopeValidator.Validate(new ProtectionScope { rules = new[] { r } }).Count > 0;
            var ok = new ScopeRule { id = "a", protection = "gfci", reference = "1", rooms = new[] { "bathroom" } };
            t.IsTrue(!bad(ok), "a valid rule is accepted");
            t.IsTrue(bad(new ScopeRule { id = "a", protection = "rcd", reference = "1" }), "an unknown protection is rejected");
            t.IsTrue(bad(new ScopeRule { id = "a", protection = "gfci", reference = "" }), "a missing reference is rejected");
            t.IsTrue(bad(new ScopeRule { id = "a", protection = "gfci", reference = "1", rooms = new[] { "attic" } }), "an unknown room is rejected");
            t.IsTrue(bad(new ScopeRule { id = "a", protection = "gfci", reference = "1", nearFixtureTypes = new[] { "sink" } }), "a fixture type without a distance is rejected");
            t.IsTrue(bad(new ScopeRule { id = "", protection = "gfci", reference = "1" }), "a rule without an id is rejected");
            t.IsTrue(bad(new ScopeRule { id = "a", protection = "gfci", reference = "1", minAmps = 30, maxAmps = 20 }), "an inverted ampere range is rejected");

            var dup = new ProtectionScope { rules = new[] { ok, new ScopeRule { id = "a", protection = "afci", reference = "2" } } };
            t.IsTrue(ProtectionScopeValidator.Validate(dup).Any(e => e.Contains("duplicate")), "a duplicate id is rejected");

            var badExemption = new ProtectionScope { exemptions = new[] { new ScopeExemption { id = "e", protection = "gfci", rooms = new[] { "moon" } } } };
            t.IsTrue(ProtectionScopeValidator.Validate(badExemption).Count > 0, "an exemption with an unknown room is rejected");

            t.IsTrue(ProtectionScopeValidator.ValidateReceptacle("r", new ScopeReceptacle { room = "attic" }).Count > 0, "a receptacle in an unknown room is rejected");
        }

        private static void RealFilesAreValid(TestContext t)
        {
            t.Begin("protection scope files");

            foreach (var p in ProfileFiles.LoadAll(TestContext.RepoRoot()).Where(p => p.scope != null))
            {
                foreach (string error in ProtectionScopeValidator.Validate(p.scope))
                    t.IsTrue(false, $"Codes/{p.folder}/scope.json: {error}");
                t.IsTrue(p.scope.rules != null && p.scope.rules.Length > 0, $"Codes/{p.folder}: the scope has rules");
                t.IsTrue(p.scope.status == "draft" || p.scope.status == "reviewed", $"Codes/{p.folder}: the scope has a status");
            }

            t.IsTrue(Load(CodeProfileIds.Nec) != null, "the NEC ships a scope");
            t.IsTrue(Load("cec").notModeled != null && Load("cec").notModeled.Length > 0, "the CEC scope says what it does not model");
        }

        private static void ViolationScenesBreakEveryCitedCode(TestContext t)
        {
            t.Begin("violation scenes against protection scope");

            string dir = Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Scenarios");
            var options = new JsonSerializerOptions { IncludeFields = true };
            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot()).Where(p => p.scope != null).ToList();
            var violations = Directory.GetFiles(dir, "*.json")
                .SelectMany(f => JsonSerializer.Deserialize<ScenarioFileData>(File.ReadAllText(f), options).violations)
                .Where(v => v.sceneScope != null && v.sceneScope.receptacle != null)
                .ToList();

            t.IsTrue(violations.Count >= 6, "the GFCI and AFCI violations describe the receptacle their scene shows");

            foreach (var v in violations)
            {
                var fact = v.sceneScope;
                foreach (string error in ProtectionScopeValidator.ValidateReceptacle(v.violationId, fact.receptacle))
                    t.IsTrue(false, error);

                string expectedSkill = fact.protection == "gfci" ? ConceptIds.ShockProtection : ConceptIds.ArcFaultProtection;
                t.Equal(expectedSkill, v.conceptId, $"{v.violationId}: the protection matches the violation's skill");
                t.IsTrue(!ScopeProvided(fact), $"{v.violationId}: the offending receptacle lacks the protection");

                foreach (var p in profiles)
                {
                    bool cited = ViolationCitations.Find(v.citations, p.folder) != null;
                    bool required = ProtectionScopeChecker.Check(p.scope, fact.receptacle, fact.protection).Missing;

                    // The scene must break every code the violation cites (docs/SCENE_DESIGN.md)
                    if (cited)
                        t.IsTrue(required, $"{v.violationId}: the scene is a violation under {p.folder}, which the violation cites");
                    // A code that would also require it could carry a citation; a code that does not is why it stays hidden
                    else if (required)
                        t.Warn($"{v.violationId}: the {p.folder} scope also requires this protection; it could carry a {p.folder} citation");
                }
            }
        }

        private static bool ScopeProvided(ScopeSceneFact fact)
        {
            return ProtectionScopeChecker.IsProvided(fact.receptacle, fact.protection);
        }
    }
}
