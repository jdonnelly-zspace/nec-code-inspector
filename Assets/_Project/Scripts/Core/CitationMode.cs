namespace NECInspector.Core
{
    /// <summary>How a student enters the code reference when flagging a violation.</summary>
    public enum CitationMode
    {
        Dropdown,           // Beginner: pick from list
        SearchableDropdown, // Standard: type to search
        FreeText            // Expert: type the exact reference
    }
}
