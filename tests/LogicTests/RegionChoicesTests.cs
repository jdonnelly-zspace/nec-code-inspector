using System;
using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The student picks a region; the app applies the installation code for it without naming it.
    /// Also covers the active-profile change event that keeps screens in step.
    /// </summary>
    public static class RegionChoicesTests
    {
        public static void Run(TestContext t)
        {
            RegionPicksTheCode(t);
            Ordering(t);
            Wording(t);
            ArtAvailability(t);
            EmptyListMessages(t);
            RegionNamesAndCodes(t);
            UnitsField(t);
            RealProfiles(t);
            ActiveChangedEvent(t);
        }

        private static DataCodeProfile Make(string id, string name, string region, string status, bool tables,
            string artSet = ArtSets.NorthAmerica, string units = UnitSystems.Imperial)
        {
            var manifest = new CodeProfileManifest { id = id, displayName = name, edition = "2030", region = region, reviewStatus = status, artSet = artSet, units = units };
            var terminology = new CodeTerminology { codeName = id.ToUpperInvariant(), terms = new TermEntry[0] };
            var articles = new[] { new CodeArticleData { reference = "1-1", title = "T", text = "x", section = 1 } };
            return new DataCodeProfile(manifest, articles, tables ? ElectricalTables.CreateNecDefaults() : null, terminology);
        }

        private static void RegionPicksTheCode(TestContext t)
        {
            t.Begin("a region picks the installation code");

            var profiles = new ICodeProfile[]
            {
                Make("zed", "Zed", "GB", "draft", false),
                Make("nec", "NEC", "US", "app-defined", true),
                Make("old-draft", "Old", "US", "draft", true),
                Make("ca-code", "Ca", "CA", "reviewed", true)
            };

            t.Equal("nec", RegionChoices.ProfileForRegion(profiles, "US")?.ProfileId, "the region's code is found");
            t.Equal("nec", RegionChoices.ProfileForRegion(profiles, " us ")?.ProfileId, "case and spaces are ignored");
            t.Equal("zed", RegionChoices.ProfileForRegion(profiles, "UK")?.ProfileId, "UK is the same region as GB");
            t.Equal("ca-code", RegionChoices.ProfileForRegion(profiles, "CA")?.ProfileId, "another region");
            t.IsTrue(RegionChoices.ProfileForRegion(profiles, "FR") == null, "a region with no code gives none");
            t.IsTrue(RegionChoices.ProfileForRegion(profiles, "") == null && RegionChoices.ProfileForRegion(null, "US") == null, "no region or no profiles gives none");

            var two = new ICodeProfile[] { Make("b-draft", "B", "US", "draft", true), Make("a-app", "A", "US", "app-defined", true), Make("c-rev", "C", "US", "reviewed", true) };
            t.Equal("c-rev", RegionChoices.ProfileForRegion(two, "US")?.ProfileId, "a reviewed code is preferred, then app-defined, then draft");
        }

        private static void Ordering(TestContext t)
        {
            t.Begin("region picker ordering");

            var profiles = new ICodeProfile[]
            {
                Make("zed", "Zed Code", "GB", "draft", false),
                Make("nec", "NEC (NFPA 70)", "US", "app-defined", true),
                Make("alpha", "Alpha Code", "CA", "reviewed", true)
            };

            var choices = RegionChoices.Build(profiles, "alpha", id => 0, 0);
            t.IsTrue(choices.Select(c => c.regionCode).SequenceEqual(new[] { "US", "CA", "GB" }), "the default code's region is first, the rest by name");
            t.IsTrue(choices.Single(c => c.regionCode == "CA").isActive, "the active region is flagged");
            t.Equal(1, choices.Count(c => c.isActive), "only one region is active");
            t.Equal(0, RegionChoices.Build(null, "nec", null, 0).Count, "no profiles gives no choices");

            var twoInOneRegion = RegionChoices.Build(new ICodeProfile[] { Make("a", "A", "US", "draft", true), Make("b", "B", "US", "reviewed", true) }, "a", id => 0, 0);
            t.Equal(1, twoInOneRegion.Count, "two codes in one region give one choice");
            t.Equal("b", twoInOneRegion[0].profileId, "the preferred code is behind it");
        }

        private static void Wording(TestContext t)
        {
            t.Begin("region picker wording");

            var nec = Make("nec", "NEC (NFPA 70)", "US", "app-defined", true);
            var cec = Make("cec", "CEC (CSA C22.1)", "CA", "draft", false, units: UnitSystems.Metric);

            var choices = RegionChoices.Build(new ICodeProfile[] { nec, cec }, "nec", id => id == "nec" ? 4 : 3, 4);
            var us = choices.Single(c => c.regionCode == "US");
            var ca = choices.Single(c => c.regionCode == "CA");

            t.Equal("United States  (current)", us.Label, "the active region is marked in its label");
            t.Equal("Canada", ca.Label, "other regions are not marked");
            t.Equal("All 4 scenarios available", us.ScenarioText, "all scenarios available");
            t.Equal("3 of 4 scenarios available", ca.ScenarioText, "some scenarios available");
            t.Equal("", us.SandboxText, "a region whose code has tables says nothing about the sandbox");
            t.Equal("Panel sandbox: not available for this region yet", ca.SandboxText, "a region without tables says so");
            t.Equal("No scenarios available yet", new RegionChoice { availableScenarios = 0, totalScenarios = 5 }.ScenarioText, "no scenarios available");
            t.Equal("", new RegionChoice { availableScenarios = 0, totalScenarios = 0 }.ScenarioText, "no catalog, no scenario line");

            var lines = ca.Details.Split('\n');
            t.Equal(3, lines.Length, "details: units, scenarios, sandbox");
            t.IsTrue(lines[0].StartsWith("Metric"), "details start with the unit system");
            t.IsTrue(us.Details.StartsWith("Imperial"), "the other region is imperial");

            // The student never sees a code name, edition or review status
            foreach (var c in choices)
            {
                string shown = c.Label + "\n" + c.Details;
                foreach (string word in new[] { "NEC", "CEC", "NFPA", "CSA", "2030", "Draft", "draft", "reviewed", "credential" })
                    t.IsTrue(!shown.Contains(word), $"region {c.regionCode} does not show '{word}'");
            }
        }

        private static void ArtAvailability(TestContext t)
        {
            t.Begin("region picker with an art set that has no art");

            var nec = Make("nec", "NEC (NFPA 70)", "US", "app-defined", true);
            var uk = Make("uk", "UK Code", "GB", "draft", true, "uk", UnitSystems.Metric);

            var choices = RegionChoices.Build(new ICodeProfile[] { nec, uk }, "nec", id => 4, 4);
            var us = choices.Single(c => c.regionCode == "US");
            var gb = choices.Single(c => c.regionCode == "GB");

            t.IsTrue(us.artAvailable && us.availableScenarios == 4, "a region with art keeps its scenarios");
            t.IsTrue(!gb.artAvailable, "a region whose art set has no art is flagged");
            t.Equal(0, gb.availableScenarios, "no scenarios are counted without art, even if violations apply");
            t.IsTrue(gb.ScenarioText.StartsWith("Scenarios are not available"), "the picker says there are no scenarios");
            t.IsTrue(!us.ScenarioText.Contains("art"), "a region with art says nothing about art");

            var noArtSet = Make("x", "X", "US", "draft", true, null);
            t.Equal(0, RegionChoices.Build(new ICodeProfile[] { noArtSet }, "x", id => 4, 4)[0].availableScenarios, "a profile with no art set offers no scenarios");

            var withoutCatalog = RegionChoices.Build(new ICodeProfile[] { uk }, "uk", id => 4, 0)[0];
            t.IsTrue(withoutCatalog.ScenarioText.StartsWith("Scenarios are not available"), "the message shows even without a scenario catalog");
        }

        private static void EmptyListMessages(TestContext t)
        {
            t.Begin("empty scenario list message");

            var nec = Make("nec", "NEC (NFPA 70)", "US", "app-defined", true);
            var uk = Make("uk", "UK Code", "GB", "draft", true, "uk", UnitSystems.Metric);

            t.Equal("", ScenarioListMessage.For(nec, 3), "no message when scenarios are listed");
            t.IsTrue(ScenarioListMessage.For(nec, 0).Contains("No scenarios cover your region"), "a region with art but no matching scenarios says so");
            t.IsTrue(ScenarioListMessage.For(uk, 0).Contains("no scene art"), "a region without art says why");
            t.IsTrue(!ScenarioListMessage.For(uk, 0).Contains("UK Code"), "the message does not name the code");
            t.IsTrue(!ScenarioListMessage.For(nec, 0).Contains("NEC"), "nor does the other message");
            t.IsTrue(ScenarioListMessage.For(uk, 0).Contains("still study"), "the message says references still work");
            t.IsTrue(ScenarioListMessage.For(uk, 5).Contains("no scene art"), "no art is explained even if scenarios would apply");
            t.IsTrue(ScenarioListMessage.For(null, 0).Contains("No region"), "no region set up is explained");
        }

        private static void RegionNamesAndCodes(TestContext t)
        {
            t.Begin("region names and codes");

            t.Equal("United States", RegionNames.Of("US"), "US");
            t.Equal("Canada", RegionNames.Of("ca"), "CA ignores case");
            t.Equal("United Kingdom", RegionNames.Of("GB"), "GB");
            t.Equal("XX", RegionNames.Of("XX"), "unknown codes are shown as given");
            t.Equal("", RegionNames.Of(null), "no region");
            t.Equal("GB", RegionChoices.Normalize("uk"), "UK is normalised to GB");
            t.Equal("US", RegionChoices.Normalize(" us "), "case and spaces are normalised");
        }

        private static void UnitsField(TestContext t)
        {
            t.Begin("units field");

            t.IsTrue(UnitSystems.IsValid("imperial") && UnitSystems.IsValid("metric"), "the two systems are valid");
            t.IsTrue(!UnitSystems.IsValid("") && !UnitSystems.IsValid(null) && !UnitSystems.IsValid("Metric") && !UnitSystems.IsValid("si"), "anything else is not");
            t.IsTrue(UnitSystems.Describe("metric").StartsWith("Metric") && UnitSystems.Describe("imperial").StartsWith("Imperial"), "descriptions name the system");
            t.Equal("", UnitSystems.Describe("x"), "an unknown system has no description");

            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec);
            Func<string, bool> flagged = units =>
            {
                var m = new CodeProfileManifest
                {
                    id = nec.manifest.id, displayName = nec.manifest.displayName, edition = nec.manifest.edition, region = nec.manifest.region,
                    reviewStatus = nec.manifest.reviewStatus, artSet = nec.manifest.artSet, license = nec.manifest.license, units = units
                };
                return CodeProfileValidator.Validate(m, nec.articles, nec.tables, nec.terminology).Any(e => e.Contains("units"));
            };
            t.IsTrue(flagged(null) && flagged("furlongs"), "a profile without valid units is rejected");
            t.IsTrue(!flagged("imperial") && !flagged("metric"), "a profile with valid units is accepted");
        }

        private static void RealProfiles(TestContext t)
        {
            t.Begin("region picker with the real profiles");

            var root = TestContext.RepoRoot();
            var profiles = ProfileFiles.LoadAll(root).Select(p => (ICodeProfile)p.Build()).ToList();
            var choices = RegionChoices.Build(profiles, CodeProfileIds.Nec, id => 1, 1);

            t.Equal("US", choices[0].regionCode, "the default code's region is listed first");
            t.IsTrue(choices.All(c => !string.IsNullOrEmpty(c.regionName)), "every real region has a readable name");
            t.IsTrue(choices.All(c => UnitSystems.IsValid(c.units)), "every real region has a unit system");
            t.Equal(profiles.Select(p => RegionChoices.Normalize(p.Region)).Distinct().Count(), choices.Count, "every region with a code gets a choice");
            t.Equal("imperial", choices.Single(c => c.regionCode == "US").units, "the US is imperial");
            t.Equal("metric", choices.Single(c => c.regionCode == "GB").units, "the UK is metric");
            t.IsTrue(choices.All(c => !c.Label.Contains(c.profileId.ToUpperInvariant())), "no label contains a code id");
        }

        private static void ActiveChangedEvent(TestContext t)
        {
            t.Begin("active profile change event");

            CodeProfiles.SetActive(null);
            int raised = 0;
            System.Action handler = () => raised++;
            CodeProfiles.ActiveChanged += handler;

            var a = Make("a", "A", "US", "draft", true);
            var b = Make("b", "B", "CA", "draft", true);

            CodeProfiles.SetActive(a);
            t.Equal(1, raised, "raised when a profile becomes active");
            CodeProfiles.SetActive(a);
            t.Equal(1, raised, "not raised when the same profile is set again");
            CodeProfiles.SetActive(b);
            t.Equal(2, raised, "raised when the active profile changes");
            CodeProfiles.Clear(a);
            t.Equal(2, raised, "clearing a profile that is not active does nothing");
            CodeProfiles.Clear(b);
            t.Equal(3, raised, "raised when the active profile is cleared");
            t.IsTrue(CodeProfiles.Active == null, "no active profile after clearing");

            CodeProfiles.ActiveChanged -= handler;
            CodeProfiles.SetActive(a);
            t.Equal(3, raised, "a removed handler is not called");
            CodeProfiles.Clear(a);
        }
    }
}
