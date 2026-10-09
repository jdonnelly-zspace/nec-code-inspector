using System;
using NECInspector.Core;

namespace NECInspector.Data
{
    /// <summary>
    /// How many violations one inspection session draws from a scenario's pool, per difficulty
    /// (scenario JSON, <c>sessionSize</c>). A value of 0 means "all of them", which is how scenarios
    /// behaved before pools existed. See docs/POOL_AND_DRAW.md.
    /// </summary>
    [Serializable]
    public class SessionSize
    {
        public int beginner;
        public int standard;
        public int expert;

        public int For(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Beginner: return beginner;
                case DifficultyLevel.Expert: return expert;
                default: return standard;
            }
        }

        public bool IsSet => beginner > 0 || standard > 0 || expert > 0;
    }
}
