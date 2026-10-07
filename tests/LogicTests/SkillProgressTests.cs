using NECInspector.Core;
using NECInspector.Data;
using NECInspector.Skills;

namespace NECInspector.LogicTests
{
    public static class SkillProgressTests
    {
        public static void Run(TestContext t)
        {
            TiersAndNames(t);
            RecordingAndMastery(t);
            Attainment(t);
        }

        private static void TiersAndNames(TestContext t)
        {
            t.Begin("skill tiers");

            t.Equal(SkillTier.Foundation, SkillTiers.FromDifficulty(DifficultyLevel.Beginner), "Beginner -> Foundation");
            t.Equal(SkillTier.Practitioner, SkillTiers.FromDifficulty(DifficultyLevel.Standard), "Standard -> Practitioner");
            t.Equal(SkillTier.Authority, SkillTiers.FromDifficulty(DifficultyLevel.Expert), "Expert -> Authority");

            t.IsTrue(SkillTiers.TryParse("Authority", out var tier) && tier == SkillTier.Authority, "parses Authority");
            t.IsTrue(!SkillTiers.TryParse("authority", out _), "names are case-sensitive");
            t.IsTrue(!SkillTiers.TryParse("2", out _), "numbers are rejected");
            t.IsTrue(!SkillTiers.TryParse(null, out _), "null is rejected");

            t.Equal("Arc fault protection", SkillNames.Display(ConceptIds.ArcFaultProtection), "readable skill name");
            t.Equal("", SkillNames.Display(null), "null skill name");
        }

        private static void RecordingAndMastery(TestContext t)
        {
            t.Begin("skill recording");

            t.Near(0, SkillOutcomes.ForInspection(false, false), "missed");
            t.Near(0, SkillOutcomes.ForInspection(false, true), "citation without finding counts as missed");
            t.Near(0.5, SkillOutcomes.ForInspection(true, false), "found, wrong citation");
            t.Near(1, SkillOutcomes.ForInspection(true, true), "found, correct citation");

            var progress = new SkillProgress();
            progress.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Foundation, 1f, SkillEvidenceSources.Inspection), "t1");
            var stat = progress.Get(ConceptIds.ShockProtection, SkillTier.Foundation);
            t.IsTrue(stat != null, "a stat is created");
            t.Equal(1, stat.attempts, "first attempt counted");
            t.Near(1, stat.mastery, "first observation sets mastery");

            progress.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Foundation, 0f, SkillEvidenceSources.Inspection), "t2");
            t.Equal(2, stat.attempts, "second attempt counted");
            t.Near(0.7, stat.mastery, "running average: 1 + 0.3 * (0 - 1)");
            t.Equal("t2", stat.updatedAt, "timestamp updated");

            progress.Record(new SkillEvidence(ConceptIds.ShockProtection, SkillTier.Practitioner, 0.5f, SkillEvidenceSources.Sandbox));
            t.Equal(2, progress.stats.Count, "tiers of one skill are tracked separately");

            progress.Record(new SkillEvidence(ConceptIds.ArcFaultProtection, SkillTier.Foundation, 5f, SkillEvidenceSources.Inspection));
            t.Near(1, progress.Get(ConceptIds.ArcFaultProtection, SkillTier.Foundation).mastery, "outcome above 1 is clamped");
            progress.Record(new SkillEvidence(ConceptIds.ArcFaultProtection, SkillTier.Authority, -3f, SkillEvidenceSources.Inspection));
            t.Near(0, progress.Get(ConceptIds.ArcFaultProtection, SkillTier.Authority).mastery, "outcome below 0 is clamped");

            int before = progress.stats.Count;
            progress.Record((SkillEvidence)null);
            progress.Record(new SkillEvidence("", SkillTier.Foundation, 1f, "x"));
            progress.Record((System.Collections.Generic.IEnumerable<SkillEvidence>)null);
            t.Equal(before, progress.stats.Count, "null and empty evidence are ignored");
        }

        private static void Attainment(TestContext t)
        {
            t.Begin("skill attainment");

            var progress = new SkillProgress();
            string skill = ConceptIds.OvercurrentProtection;

            for (int i = 0; i < SkillPolicy.MinAttempts - 1; i++)
                progress.Record(new SkillEvidence(skill, SkillTier.Practitioner, 1f, "x"));
            t.IsTrue(!progress.HasAttained(skill, SkillTier.Practitioner), "perfect score but too few attempts is not attained");

            progress.Record(new SkillEvidence(skill, SkillTier.Practitioner, 1f, "x"));
            t.IsTrue(progress.HasAttained(skill, SkillTier.Practitioner), "enough attempts at high mastery is attained");
            t.IsTrue(progress.HasAttained(skill, SkillTier.Foundation), "a higher tier covers a lower requirement");
            t.IsTrue(!progress.HasAttained(skill, SkillTier.Authority), "a lower tier does not cover a higher requirement");
            t.Equal(SkillTier.Practitioner, progress.HighestAttainedTier(skill).Value, "highest attained tier");
            t.IsTrue(progress.HighestAttainedTier(ConceptIds.ConductorSizing) == null, "no tier for an untouched skill");
            t.Equal(1, progress.AttainedSkillCount(), "one skill attained");

            // Mastery decays when performance drops: three misses after a perfect run
            for (int i = 0; i < 3; i++)
                progress.Record(new SkillEvidence(skill, SkillTier.Practitioner, 0f, "x"));
            t.IsTrue(!progress.HasAttained(skill, SkillTier.Practitioner), "recent failures lower mastery below the threshold");

            var weak = new SkillProgress();
            for (int i = 0; i < 5; i++)
                weak.Record(new SkillEvidence(skill, SkillTier.Foundation, 0.5f, "x"));
            t.IsTrue(!weak.HasAttained(skill, SkillTier.Foundation), "many attempts at low mastery is not attained");
        }
    }
}
