using System.Text.RegularExpressions;

namespace NECInspector.Codes
{
    /// <summary>
    /// Art sets are groups of device and panel art that suit a region (see docs/SCENE_DESIGN.md).
    /// A code profile names the set its scenes use in profile.json (<c>artSet</c>). NEC and CEC share
    /// the North American set; a code from another group needs its own set before its scenes can be offered.
    /// </summary>
    public static class ArtSets
    {
        public const string NorthAmerica = "north-america";

        private static readonly string[] Available = { NorthAmerica };

        /// <summary>True if art exists for the set. Add a set here when its art is built.</summary>
        public static bool IsAvailable(string artSet)
        {
            return System.Array.IndexOf(Available, artSet) >= 0;
        }

        /// <summary>Set names are lower-case words joined by '-', like "north-america".</summary>
        public static bool IsValidName(string artSet)
        {
            return !string.IsNullOrEmpty(artSet) && Regex.IsMatch(artSet, "^[a-z0-9]+(-[a-z0-9]+)*$");
        }
    }
}
