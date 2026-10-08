using System.Collections.Generic;
using System.Linq;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// What the code profile picker shows, and the active-profile change event that keeps screens in step.
    /// </summary>
    public static class CodeProfileChoicesTests
    {
        public static void Run(TestContext t)
        {
            Ordering(t);
            Wording(t);
            ArtAvailability(t);
            RegionNames(t);
            RealProfiles(t);
            ActiveChangedEvent(t);
        }

        private static DataCodeProfile Make(string id, string name, string region, string status, bool tables, string artSet = ArtSets.NorthAmerica)
        {
            var manifest = new CodeProfileManifest { id = id, displayName = name, edition = "2030", region = region, reviewStatus = status, artSet = artSet };
            var terminology = new CodeTerminology { codeName = id.ToUpperInvariant(), terms = new TermEntry[0] };
            var articles = new[] { new CodeArticleData { reference = "1-1", title = "T", text = "x", section = 1 } };
            return new DataCodeProfile(manifest, articles, tables ? ElectricalTables.CreateNecDefaults() : null, terminology);
        }

        private static void Ordering(TestContext t)
        {
            t.Begin("picker ordering");

            var profiles = new ICodeProfile[]
            {
                Make("zed", "Zed Code", "UK", "draft", false),
                Make("nec", "NEC (NFPA 70)", "US", "app-defined", true),
                Make("alpha", "Alpha Code", "CA", "reviewed", true)
            };

            var choices = CodeProfileChoices.Build(profiles, "alpha", id => 0, 0);
            t.IsTrue(choices.Select(c => c.id).SequenceEqual(new[] { "nec", "alpha", "zed" }), "the default code is first, the rest by name");
            t.IsTrue(choices.Single(c => c.id == "alpha").isActive, "the active code is flagged");
            t.Equal(1, choices.Count(c => c.isActive), "only one code is active");

            t.Equal(0, CodeProfileChoices.Build(null, "nec", null, 0).Count, "no profiles gives no choices");
        }

        private static void Wording(TestContext t)
        {
            t.Begin("picker wording");

            var nec = Make("nec", "NEC (NFPA 70)", "US", "app-defined", true);
            var cec = Make("cec", "CEC (CSA C22.1)", "CA", "draft", false);

            var choices = CodeProfileChoices.Build(new ICodeProfile[] { nec, cec }, "nec",
                id => id == "nec" ? 4 : 3, 4);
            var necChoice = choices.Single(c => c.id == "nec");
            var cecChoice = choices.Single(c => c.id == "cec");

            t.Equal("NEC (NFPA 70) 2030  (current)", necChoice.Label, "the active code is marked in its label");
            t.Equal("CEC (CSA C22.1) 2030", cecChoice.Label, "other codes are not marked");

            t.Equal("All 4 scenarios available", necChoice.ScenarioText, "all scenarios available");
            t.Equal("3 of 4 scenarios available", cecChoice.ScenarioText, "some scenarios available");
            t.Equal("", necChoice.SandboxText, "a code with tables says nothing about the sandbox");
            t.Equal("Panel sandbox: not available for this code yet", cecChoice.SandboxText, "a code without tables says so");

            t.IsTrue(cecChoice.StatusText.StartsWith("Draft"), "draft status text");
            t.IsTrue(necChoice.StatusText.Contains("pending"), "app-defined status text");
            t.IsTrue(new CodeProfileChoice { reviewStatus = "reviewed" }.StatusText.StartsWith("Reviewed"), "reviewed status text");
            t.Equal("Review status unknown", new CodeProfileChoice { reviewStatus = "weird" }.StatusText, "unknown status text");

            t.Equal("No scenarios available yet", new CodeProfileChoice { availableScenarios = 0, totalScenarios = 5 }.ScenarioText, "no scenarios available");
            t.Equal("", new CodeProfileChoice { availableScenarios = 0, totalScenarios = 0 }.ScenarioText, "no catalog, no scenario line");

            var lines = cecChoice.Details.Split('\n');
            t.Equal("Canada", lines[0], "details start with the region");
            t.Equal(4, lines.Length, "details: region, status, scenarios, sandbox");
            t.Equal(3, necChoice.Details.Split('\n').Length, "details leave out empty lines");
        }

        private static void ArtAvailability(TestContext t)
        {
            t.Begin("picker with an art set that has no art");

            var nec = Make("nec", "NEC (NFPA 70)", "US", "app-defined", true);
            var uk = Make("uk", "UK Code", "UK", "draft", true, "uk");

            var choices = CodeProfileChoices.Build(new ICodeProfile[] { nec, uk }, "nec", id => 4, 4);
            var necChoice = choices.Single(c => c.id == "nec");
            var ukChoice = choices.Single(c => c.id == "uk");

            t.IsTrue(necChoice.artAvailable && necChoice.availableScenarios == 4, "a code with art keeps its scenarios");
            t.IsTrue(!ukChoice.artAvailable, "a code whose art set has no art is flagged");
            t.Equal(0, ukChoice.availableScenarios, "no scenarios are counted for a code without art, even if violations apply");
            t.IsTrue(ukChoice.ScenarioText.StartsWith("Scenarios not available"), "the picker says why there are no scenarios");
            t.IsTrue(ukChoice.Details.Contains("no scene art"), "the details include the reason");
            t.IsTrue(!necChoice.ScenarioText.Contains("art"), "a code with art says nothing about art");

            var noArtSet = Make("x", "X", "US", "draft", true, null);
            t.Equal(0, CodeProfileChoices.Build(new ICodeProfile[] { noArtSet }, "x", id => 4, 4)[0].availableScenarios,
                "a profile with no art set offers no scenarios");

            var withoutCatalog = CodeProfileChoices.Build(new ICodeProfile[] { uk }, "uk", id => 4, 0)[0];
            t.IsTrue(withoutCatalog.ScenarioText.StartsWith("Scenarios not available"), "the art message shows even without a scenario catalog");
        }

        private static void RegionNames(TestContext t)
        {
            t.Begin("picker region names");

            t.Equal("United States", CodeProfileChoices.RegionName("US"), "US");
            t.Equal("Canada", CodeProfileChoices.RegionName("ca"), "CA ignores case");
            t.Equal("United Kingdom", CodeProfileChoices.RegionName("GB"), "GB");
            t.Equal("XX", CodeProfileChoices.RegionName("XX"), "unknown codes are shown as given");
            t.Equal("", CodeProfileChoices.RegionName(null), "no region");
        }

        private static void RealProfiles(TestContext t)
        {
            t.Begin("picker with the real profiles");

            var root = TestContext.RepoRoot();
            var profiles = ProfileFiles.LoadAll(root).Select(p => (ICodeProfile)p.Build()).ToList();
            var choices = CodeProfileChoices.Build(profiles, CodeProfileIds.Nec, id => 1, 1);

            t.Equal(profiles.Count, choices.Count, "every profile folder gets a choice");
            t.Equal(CodeProfileIds.Nec, choices[0].id, "NEC is listed first");
            t.IsTrue(choices.All(c => !string.IsNullOrEmpty(c.regionName)), "every real profile has a readable region");
            t.IsTrue(choices.All(c => c.StatusText != "Review status unknown"), "every real profile has a known review status");
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
