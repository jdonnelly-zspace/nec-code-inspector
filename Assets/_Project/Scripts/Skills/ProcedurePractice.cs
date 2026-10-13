using System;
using System.Collections.Generic;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Skills
{
    /// <summary>
    /// Step-ordering practice for an occupation's procedure: the student puts the steps of a standard procedure in the right order.
    /// Pure logic, so it is tested without the engine. Difficulty sets how many steps are shown (a run of consecutive steps from a seeded start):
    /// Apprentice 4, Journeyman 6, Master all. The score is the share of neighbouring pairs the student ordered correctly, so a nearly right
    /// answer earns most of the credit. Evidence goes to every skill the procedure uses, at the tier of the difficulty.
    /// </summary>
    public static class ProcedurePractice
    {
        public const int ApprenticeSteps = 4;
        public const int JourneymanSteps = 6;

        public static int StepCount(DifficultyLevel difficulty, int available)
        {
            int n = difficulty == DifficultyLevel.Apprentice ? ApprenticeSteps : difficulty == DifficultyLevel.Journeyman ? JourneymanSteps : available;
            return Math.Max(0, Math.Min(n, available));
        }

        /// <summary>The steps to put in order, as indices into the procedure's steps, in the order to show them (never already correct).</summary>
        public static int[] Deal(ProcedureData procedure, DifficultyLevel difficulty, int seed)
        {
            int available = procedure?.steps?.Length ?? 0;
            int count = StepCount(difficulty, available);
            if (count < 2) return new int[0];

            var rng = new Random(seed);
            int start = rng.Next(0, available - count + 1);
            var shown = new int[count];
            for (int i = 0; i < count; i++) shown[i] = start + i;

            for (int attempt = 0; attempt < 20; attempt++)
            {
                for (int i = count - 1; i > 0; i--)
                {
                    int j = rng.Next(0, i + 1);
                    (shown[i], shown[j]) = (shown[j], shown[i]);
                }
                if (!IsInOrder(shown)) return shown;
            }

            // Fall back to a rotation, which is never in order for two or more steps
            var rotated = new int[count];
            for (int i = 0; i < count; i++) rotated[i] = start + (i + 1) % count;
            return rotated;
        }

        /// <summary>Share of neighbouring pairs in the right order, 0 to 1. The answer is step indices as the student arranged them.</summary>
        public static float Score(int[] answer)
        {
            if (answer == null || answer.Length < 2) return 0f;
            int right = 0;
            for (int i = 0; i < answer.Length - 1; i++)
                if (answer[i] < answer[i + 1]) right++;
            return right / (float)(answer.Length - 1);
        }

        public static bool IsInOrder(int[] answer) => answer != null && answer.Length > 1 && Math.Abs(Score(answer) - 1f) < 0.0001f;

        /// <summary>One piece of evidence for each skill the procedure uses.</summary>
        public static List<SkillEvidence> Evidence(ProcedureData procedure, DifficultyLevel difficulty, float score)
        {
            var list = new List<SkillEvidence>();
            if (procedure?.skills == null) return list;
            var tier = SkillTiers.FromDifficulty(difficulty);
            foreach (string skill in procedure.skills)
                list.Add(new SkillEvidence(skill, tier, score, SkillEvidenceSources.Procedure));
            return list;
        }
    }
}
