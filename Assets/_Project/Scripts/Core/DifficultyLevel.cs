namespace NECInspector.Core
{
    public enum DifficultyLevel
    {
        // The three levels follow the trade's own ladder (docs/product/DECISIONS.md, D-12). They are stages of skill, described by what the student
        // can already do. They are stages of skill, not credentials: the app awards none and implies none.
        Apprentice, // building familiarity: guided, with hints
        Journeyman, // working knowledge: names the rule from a search, no hints
        Master      // full fluency: names the rule unaided, optional time limit, subtle violations
    }
}
