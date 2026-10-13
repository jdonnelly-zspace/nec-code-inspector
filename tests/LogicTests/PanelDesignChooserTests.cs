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
    /// Which panel design the sandbox gives a student: written for the student's code, offered at the student's level, and not one they just played.
    /// Uses the real designs in Content/Sandbox/panel-designs.json.
    /// </summary>
    public static class PanelDesignChooserTests
    {
        private static readonly DifficultyLevel[] Levels = { DifficultyLevel.Apprentice, DifficultyLevel.Journeyman, DifficultyLevel.Master };

        public static void Run(TestContext t)
        {
            var opt = new JsonSerializerOptions { IncludeFields = true };
            var file = JsonSerializer.Deserialize<SandboxFile>(File.ReadAllText(Path.Combine(TestContext.RepoRoot(), "Assets/_Project/Content/Sandbox/panel-designs.json")), opt);
            var designs = file.designs.Select(d => new PanelDesignChooser.Candidate
            {
                id = d.panelType,
                profileId = d.profileId,
                levels = d.availableDifficulties.Select(x => (DifficultyLevel)Enum.Parse(typeof(DifficultyLevel), x)).ToArray()
            }).ToList();
            var byId = designs.ToDictionary(d => d.id);

            t.Begin("sandbox design: every level has designs to play");
            foreach (var level in Levels)
                t.IsTrue(designs.Count(d => d.profileId == "nec-2023" && d.levels.Contains(level)) >= 2, $"nec-2023 has at least two designs at {level}");
            t.IsTrue(designs.All(d => !string.IsNullOrEmpty(d.id)), "every design has a panel type to identify it");
            t.Equal(designs.Count, designs.Select(d => d.id).Distinct().Count(), "panel types are unique");

            t.Begin("sandbox design: the design suits the code and the level");
            foreach (var level in Levels)
                for (int seed = 0; seed < 20; seed++)
                {
                    string id = PanelDesignChooser.Choose(designs, "nec-2023", level, null, seed);
                    t.IsTrue(id != null && byId[id].profileId == "nec-2023" && byId[id].levels.Contains(level), $"{level}, seed {seed}: a nec-2023 design offered at the level");
                }
            t.IsTrue(PanelDesignChooser.Choose(designs, "cec", DifficultyLevel.Journeyman, null, 1) == null, "a code with no design gets none, so the sandbox is not offered");
            t.IsTrue(PanelDesignChooser.Choose(designs, "", DifficultyLevel.Journeyman, null, 1) == null, "no code, no design");
            t.IsTrue(PanelDesignChooser.Choose(null, "nec-2023", DifficultyLevel.Journeyman, null, 1) == null, "no designs, no design");
            t.Equal(PanelDesignChooser.Choose(designs, "nec-2023", DifficultyLevel.Master, null, 7), PanelDesignChooser.Choose(designs, "nec-2023", DifficultyLevel.Master, null, 7), "the same seed gives the same design");

            t.Begin("sandbox design: repeat visits cycle through every design before repeating");
            foreach (var level in Levels)
            {
                var pool = designs.Where(d => d.profileId == "nec-2023" && d.levels.Contains(level)).Select(d => d.id).ToList();
                var played = new List<string>();
                for (int i = 0; i < pool.Count; i++)
                    played.Add(PanelDesignChooser.Choose(designs, "nec-2023", level, played, 100 + i));
                t.Equal(pool.Count, played.Distinct().Count(), $"{level}: {pool.Count} plays show {pool.Count} different designs");
                string next = PanelDesignChooser.Choose(designs, "nec-2023", level, played, 5);
                t.Equal(played[0], next, $"{level}: after all are played, the one played longest ago comes back");
            }

            t.Begin("sandbox design: a level with no design falls back to the code's other designs");
            var onlyLow = new List<PanelDesignChooser.Candidate>
            {
                new PanelDesignChooser.Candidate { id = "A", profileId = "x", levels = new[] { DifficultyLevel.Apprentice } }
            };
            t.Equal("A", PanelDesignChooser.Choose(onlyLow, "x", DifficultyLevel.Master, null, 1), "a Master student still gets the code's only design");
        }
    }
}
