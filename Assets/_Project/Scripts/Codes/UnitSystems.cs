using System;

namespace NECInspector.Codes
{
    /// <summary>
    /// The measurement systems a code can use (profile.json, <c>units</c>). The student sees the unit system,
    /// never the name of the installation code behind it.
    /// </summary>
    public static class UnitSystems
    {
        public const string Imperial = "imperial";
        public const string Metric = "metric";

        public static readonly string[] All = { Imperial, Metric };

        public static bool IsValid(string units) => Array.IndexOf(All, units) >= 0;

        /// <summary>A short line for students, such as "Metric: metres, millimetres, mm2".</summary>
        public static string Describe(string units)
        {
            switch (units)
            {
                case Imperial: return "Imperial: feet, inches, AWG";
                case Metric: return "Metric: metres, millimetres, mm²";
                default: return "";
            }
        }
    }
}
