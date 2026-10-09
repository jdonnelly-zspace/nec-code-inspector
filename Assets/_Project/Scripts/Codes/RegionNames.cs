namespace NECInspector.Codes
{
    /// <summary>Readable names for the region codes in profile.json (see <see cref="RegionChoices"/>).</summary>
    public static class RegionNames
    {
        /// <summary>A readable region name for a region code; unknown codes are shown as given.</summary>
        public static string Of(string code)
        {
            switch ((code ?? "").ToUpperInvariant())
            {
                case "": return "";
                case "US": return "United States";
                case "CA": return "Canada";
                case "UK":
                case "GB": return "United Kingdom";
                case "EU": return "European Union";
                case "FR": return "France";
                case "DE": return "Germany";
                default: return code;
            }
        }
    }
}
