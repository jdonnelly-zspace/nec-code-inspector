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
        public int apprentice;
        public int journeyman;
        public int master;

        public int For(DifficultyLevel difficulty)
        {
            switch (difficulty)
            {
                case DifficultyLevel.Apprentice: return apprentice;
                case DifficultyLevel.Master: return master;
                default: return journeyman;
            }
        }

        public bool IsSet => apprentice > 0 || journeyman > 0 || master > 0;
    }
}
