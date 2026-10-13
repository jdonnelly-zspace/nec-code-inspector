namespace NECInspector.Core
{
    /// <summary>How a student enters the code reference when flagging a violation.</summary>
    public enum CitationMode
    {
        Dropdown,           // Apprentice: pick from list
        SearchableDropdown, // Journeyman: type to search
        FreeText            // Master: type the exact reference
    }
}
