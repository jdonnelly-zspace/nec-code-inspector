using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using NECInspector.Codes;
using NECInspector.Data;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The skill lessons (StreamingAssets/Lessons/lessons.json): complete, in our own words, free of code and credential names,
    /// and written so they read correctly in every region's vocabulary. See docs/TEACHING_CONTENT.md.
    /// </summary>
    public static class LessonTests
    {
        public static void Run(TestContext t)
        {
            var file = Load(TestContext.RepoRoot());
            Completeness(t, file);
            ContentRules(t, file);
            RegionWording(t, file);
            ValidatorCatchesProblems(t);
        }

        private static LessonFileData Load(string root)
        {
            string path = Path.Combine(root, "Assets/_Project/StreamingAssets/Lessons/lessons.json");
            return JsonSerializer.Deserialize<LessonFileData>(File.ReadAllText(path), new JsonSerializerOptions { IncludeFields = true });
        }

        private static void Completeness(TestContext t, LessonFileData file)
        {
            t.Begin("skill lessons are complete");

            var errors = LessonValidator.Validate(file);
            t.Equal(0, errors.Count, "the lessons file is valid: " + string.Join("; ", errors));
            t.Equal(ConceptIds.All.Length, file.lessons.Length, "there is one lesson per skill");
            foreach (string skill in ConceptIds.All)
                t.IsTrue(file.lessons.Count(l => l.skillId == skill) == 1, $"exactly one lesson for {skill}");
        }

        private static void ContentRules(TestContext t, LessonFileData file)
        {
            t.Begin("skill lessons follow the content policy");

            var codeNames = new Regex(@"\b(NEC|CEC|NFPA|CSA|IET|BSI|BS ?7671|C22\.1|IEC|HD 60364)\b");
            var pathway = new Regex(@"\b(CTE|apprentice|apprenticeship|journeyman|licensed|licence|license|certification|certificate|credential|exam|trade school|NCCER|Red Seal)\b", RegexOptions.IgnoreCase);

            foreach (var lesson in file.lessons)
            {
                foreach (string text in LessonValidator.AllText(lesson))
                {
                    string where = $"{lesson.skillId}: '{(text.Length > 40 ? text.Substring(0, 40) + "..." : text)}'";
                    t.IsTrue(!codeNames.IsMatch(text), $"{where} names a code or publisher");
                    t.IsTrue(!pathway.IsMatch(text), $"{where} names a credential or training pathway");
                    t.IsTrue(!Regex.IsMatch(text, @"\bshall\b", RegexOptions.IgnoreCase), $"{where} uses statutory wording");
                    t.IsTrue(!text.Contains("TODO"), $"{where} has a TODO");
                    // Numbers belong to the rules (which differ by region and unit system), not to the lesson
                    t.IsTrue(!text.Any(char.IsDigit), $"{where} contains a number or a unit");
                }
                t.IsTrue(lesson.summary.Length <= 260, $"{lesson.skillId}: the summary stays short");
            }
        }

        private static void RegionWording(TestContext t, LessonFileData file)
        {
            t.Begin("skill lessons read correctly in each region's words");

            foreach (var profile in ProfileFiles.LoadAll(TestContext.RepoRoot()))
            {
                var terms = profile.terminology;
                foreach (var lesson in file.lessons)
                {
                    foreach (string key in LessonValidator.TermKeys(lesson))
                    {
                        bool has = terms.terms != null && terms.terms.Any(x => x.key == key && !string.IsNullOrEmpty(x.text));
                        if (profile.Id == CodeProfileIds.Nec)
                            t.IsTrue(has, $"{lesson.skillId} uses the term '{key}', which the NEC terminology defines");
                        else if (!has)
                            t.Warn($"{lesson.skillId} uses the term '{key}', which {profile.Id} does not define (the key itself would show)");
                    }

                    foreach (string text in LessonValidator.AllText(lesson))
                    {
                        string shown = terms.Format(text);
                        t.IsTrue(!shown.Contains("{") && !shown.Contains("}"), $"{profile.Id} {lesson.skillId}: every token is filled");
                    }
                }
            }

            var nec = ProfileFiles.Load(TestContext.RepoRoot(), CodeProfileIds.Nec).terminology;
            var uk = ProfileFiles.Load(TestContext.RepoRoot(), "bs7671").terminology;
            var shock = file.lessons.Single(l => l.skillId == ConceptIds.ShockProtection);
            t.IsTrue(nec.Format(shock.summary).StartsWith("GFCI protection"), "the shock lesson says GFCI in the US");
            t.IsTrue(uk.Format(shock.summary).StartsWith("RCD protection"), "and RCD in the UK, with a capital at the start of the sentence");
            var earthing = file.lessons.Single(l => l.skillId == ConceptIds.EarthingBonding);
            t.IsTrue(nec.Format(earthing.summary).StartsWith("Grounding"), "the earthing lesson says grounding in the US");
            t.IsTrue(uk.Format(earthing.summary).StartsWith("Earthing"), "and earthing in the UK");
        }

        private static void ValidatorCatchesProblems(TestContext t)
        {
            t.Begin("lesson validator");

            Assert(t, new LessonFileData { lessons = new SkillLessonData[0] }, "lessons is empty", "an empty file is rejected");

            var good = Load(TestContext.RepoRoot());
            var missing = new LessonFileData { lessons = good.lessons.Skip(1).ToArray() };
            Assert(t, missing, "no lesson for the skill", "a missing skill is reported");

            var dup = new LessonFileData { lessons = good.lessons.Concat(new[] { good.lessons[0] }).ToArray() };
            Assert(t, dup, "more than one lesson", "a skill with two lessons is reported");

            var bad = JsonSerializer.Deserialize<LessonFileData>(JsonSerializer.Serialize(good, new JsonSerializerOptions { IncludeFields = true }),
                new JsonSerializerOptions { IncludeFields = true });
            bad.lessons[0].skillId = "not-a-skill";
            bad.lessons[1].whatToLookFor = new[] { "only one" };
            bad.lessons[2].tiers.practitioner = "";
            bad.lessons[3].reviewStatus = "maybe";
            bad.lessons[4].title = "";
            var errors = LessonValidator.Validate(bad);
            t.IsTrue(errors.Any(e => e.Contains("unknown skillId")), "an unknown skill is reported");
            t.IsTrue(errors.Any(e => e.Contains("whatToLookFor needs")), "too few things to look for is reported");
            t.IsTrue(errors.Any(e => e.Contains("tiers.practitioner")), "a missing tier text is reported");
            t.IsTrue(errors.Any(e => e.Contains("reviewStatus")), "a bad review status is reported");
            t.IsTrue(errors.Any(e => e.Contains("title is missing")), "a missing title is reported");
        }

        private static void Assert(TestContext t, LessonFileData file, string expected, string message)
        {
            t.IsTrue(LessonValidator.Validate(file).Any(e => e.Contains(expected)), message);
        }
    }
}
