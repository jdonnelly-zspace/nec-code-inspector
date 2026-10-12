using System.IO;
using System.Linq;
using System.Text.Json;
using NECInspector.Core;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The occupations file the app loads (StreamingAssets/Occupations/occupations.json) and the step-ordering practice built on it.
    /// </summary>
    public static class OccupationFileTests
    {
        public static void Run(TestContext t)
        {
            var file = Load(TestContext.RepoRoot());
            FileIsSound(t, file);
            ValidatorCatchesProblems(t, file);
            PracticeDeal(t, file);
            PracticeScore(t, file);
        }

        private static OccupationFileData Load(string root)
        {
            string path = Path.Combine(root, "Assets/_Project/StreamingAssets/Occupations/occupations.json");
            return JsonSerializer.Deserialize<OccupationFileData>(File.ReadAllText(path), new JsonSerializerOptions { IncludeFields = true });
        }

        private static void FileIsSound(TestContext t, OccupationFileData file)
        {
            t.Begin("occupations file is sound");
            var errors = OccupationValidator.Validate(file);
            t.Equal(0, errors.Count, "the occupations file is valid: " + string.Join("; ", errors.Take(5)));
            t.Equal(6, file.occupations.Length, "six core occupations");
            t.Equal(44, file.occupations.Sum(o => o.procedures.Length), "44 procedures");
            t.IsTrue(file.occupations.Where(o => o.available).All(o => o.wave == 1), "only wave 1 occupations are available");
            t.IsTrue(file.occupations.Where(o => o.available).All(o => o.procedures.Any(p => p.practice)), "an available occupation offers some practice");
            foreach (var p in file.occupations.SelectMany(o => o.procedures))
                if (p.practice)
                    t.IsTrue(p.coverage != "none", $"{p.id}: practice means the app covers it at least in part");
        }

        private static void ValidatorCatchesProblems(TestContext t, OccupationFileData good)
        {
            t.Begin("occupation validator catches problems");
            t.IsTrue(OccupationValidator.Validate(null).Count > 0, "null is rejected");

            var bad = Clone(good);
            bad.occupations[0].procedures[0].skills = new[] { "not-a-skill" };
            t.IsTrue(OccupationValidator.Validate(bad).Any(e => e.Contains("unknown skill")), "unknown skill is rejected");

            bad = Clone(good);
            bad.occupations[0].procedures[0].steps = new[] { "a", "b" };
            t.IsTrue(OccupationValidator.Validate(bad).Any(e => e.Contains("3 steps")), "too few steps is rejected");

            bad = Clone(good);
            bad.occupations[0].procedures[0].steps[0] = "Check the rule in the NEC first";
            t.IsTrue(OccupationValidator.Validate(bad).Any(e => e.Contains("names a code")), "a named code is rejected");

            bad = Clone(good);
            bad.occupations[1].procedures[0].id = bad.occupations[0].procedures[0].id;
            t.IsTrue(OccupationValidator.Validate(bad).Any(e => e.Contains("duplicate procedure id")), "duplicate procedure id is rejected");
        }

        private static void PracticeDeal(TestContext t, OccupationFileData file)
        {
            t.Begin("procedure practice deals steps");
            var procs = file.occupations.SelectMany(o => o.procedures).Where(p => p.practice).ToList();
            t.IsTrue(procs.Count > 0, "there are procedures to practise");
            foreach (var p in procs)
            {
                foreach (var d in new[] { DifficultyLevel.Beginner, DifficultyLevel.Standard, DifficultyLevel.Expert })
                {
                    int[] a = ProcedurePractice.Deal(p, d, 7);
                    int[] b = ProcedurePractice.Deal(p, d, 7);
                    t.Equal(ProcedurePractice.StepCount(d, p.steps.Length), a.Length, $"{p.id} {d}: number of steps dealt");
                    t.IsTrue(a.SequenceEqual(b), $"{p.id} {d}: the same seed deals the same order");
                    t.IsTrue(!ProcedurePractice.IsInOrder(a), $"{p.id} {d}: never dealt already in order");
                    t.IsTrue(a.Distinct().Count() == a.Length && a.All(i => i >= 0 && i < p.steps.Length), $"{p.id} {d}: valid distinct step indices");
                }
            }
        }

        private static void PracticeScore(TestContext t, OccupationFileData file)
        {
            t.Begin("procedure practice scoring");
            t.Near(1.0, ProcedurePractice.Score(new[] { 2, 3, 4, 5 }), "right order scores 1");
            t.Near(0.0, ProcedurePractice.Score(new[] { 5, 4, 3, 2 }), "reverse order scores 0");
            t.Near(2.0 / 3.0, ProcedurePractice.Score(new[] { 2, 4, 3, 5 }), "one swap keeps most credit");
            t.Near(0.0, ProcedurePractice.Score(null), "no answer scores 0");

            var p = file.occupations.SelectMany(o => o.procedures).First(x => x.practice);
            var ev = ProcedurePractice.Evidence(p, DifficultyLevel.Expert, 0.75f);
            t.Equal(p.skills.Length, ev.Count, "evidence for each skill the procedure uses");
            t.IsTrue(ev.All(e => e.tier == SkillTier.Authority && e.source == SkillEvidenceSources.Procedure && e.outcome == 0.75f), "evidence carries tier, source and outcome");
        }

        private static OccupationFileData Clone(OccupationFileData f)
        {
            var o = new JsonSerializerOptions { IncludeFields = true };
            return JsonSerializer.Deserialize<OccupationFileData>(JsonSerializer.Serialize(f, o), o);
        }
    }
}
