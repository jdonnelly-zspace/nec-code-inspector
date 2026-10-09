using NECInspector.Core;

namespace NECInspector.Data
{
    /// <summary>
    /// Which violations a session can show. A scenario's violations are a pool: the ones that apply to the student's
    /// code and are visible at the difficulty they play. A violation is visible at its minimum difficulty and above,
    /// except that subtle ones only show at Expert. The same rule sizes the pools in the content targets.
    /// </summary>
    public static class ViolationPools
    {
        public static bool IsActive(DifficultyLevel minimumDifficulty, bool isSubtle, DifficultyLevel played)
        {
            if ((int)minimumDifficulty > (int)played) return false;
            if (isSubtle && played != DifficultyLevel.Expert) return false;
            return true;
        }
    }
}
