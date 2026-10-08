using System;
using System.Collections.Generic;

namespace NECInspector.Data
{
    /// <summary>
    /// A measured value in a scene, for example the distance from the farthest point on a wall to the
    /// nearest receptacle. Stored in whatever unit the author used; compared in metres.
    /// </summary>
    [Serializable]
    public class SceneFact
    {
        public string quantity;   // what is measured, e.g. "max-distance-to-receptacle"
        public float value;
        public string unit;       // ft | in | m | mm
        public string note;       // how it is measured, for the scene builder

        public bool IsSet => !string.IsNullOrEmpty(quantity);
    }

    /// <summary>One code's limit on a measured quantity.</summary>
    [Serializable]
    public class SceneLimit
    {
        public string quantity;   // must match the scene fact's quantity
        public string kind;       // max | min
        public float value;
        public string unit;       // ft | in | m | mm

        public bool IsSet => !string.IsNullOrEmpty(quantity);
    }

    /// <summary>
    /// Rules for scene values that are shared between codes (see docs/SCENE_DESIGN.md): a violation's
    /// scene value must break every listed code's limit by a margin, and the scene's compliant parts
    /// must pass every listed code.
    /// </summary>
    public static class SceneFacts
    {
        /// <summary>How far past a limit a violating value must be, as a fraction of the limit.</summary>
        public const float RequiredMargin = 0.10f;

        public const string Max = "max";
        public const string Min = "min";

        public static bool TryToMetres(float value, string unit, out double metres)
        {
            switch (unit)
            {
                case "ft": metres = value * 0.3048; return true;
                case "in": metres = value * 0.0254; return true;
                case "m": metres = value; return true;
                case "mm": metres = value * 0.001; return true;
                default: metres = 0; return false;
            }
        }

        /// <summary>
        /// How far a value is past a limit, as a fraction of the limit: positive means it breaks the
        /// limit, zero or negative means it complies. False if a unit or the limit kind is unknown.
        /// </summary>
        public static bool TryBreach(SceneFact fact, SceneLimit limit, out double breach)
        {
            breach = 0;
            if (fact == null || limit == null) return false;
            if (!TryToMetres(fact.value, fact.unit, out double value)) return false;
            if (!TryToMetres(limit.value, limit.unit, out double bound) || bound <= 0) return false;

            switch (limit.kind)
            {
                case Max: breach = (value - bound) / bound; return true;
                case Min: breach = (bound - value) / bound; return true;
                default: return false;
            }
        }

        /// <summary>Problems with a violation's scene values against its citations' limits.</summary>
        public static List<string> Validate(string label, SceneFact fact, SceneFact compliant, ViolationCitation[] citations)
        {
            var errors = new List<string>();
            bool hasFact = fact != null && fact.IsSet;
            bool hasCompliant = compliant != null && compliant.IsSet;
            var list = citations ?? new ViolationCitation[0];

            if (!hasFact)
            {
                if (hasCompliant)
                    errors.Add($"{label}: compliantFact needs a sceneFact");
                foreach (var c in list)
                    if (c != null && c.limit != null && c.limit.IsSet)
                        errors.Add($"{label}: the '{c.profileId}' citation has a limit but the violation has no sceneFact");
                return errors;
            }

            if (!TryToMetres(fact.value, fact.unit, out _))
                errors.Add($"{label}: sceneFact has an unknown unit '{fact.unit}'");
            if (hasCompliant)
            {
                if (compliant.quantity != fact.quantity)
                    errors.Add($"{label}: compliantFact measures '{compliant.quantity}' but sceneFact measures '{fact.quantity}'");
                if (!TryToMetres(compliant.value, compliant.unit, out _))
                    errors.Add($"{label}: compliantFact has an unknown unit '{compliant.unit}'");
            }

            foreach (var c in list)
            {
                if (c == null) continue;
                string who = $"{label}: the '{c.profileId}' citation";

                if (c.limit == null || !c.limit.IsSet)
                {
                    errors.Add($"{who} has no limit (needed because the violation has a sceneFact)");
                    continue;
                }
                if (c.limit.quantity != fact.quantity)
                {
                    errors.Add($"{who} limits '{c.limit.quantity}' but the scene measures '{fact.quantity}'");
                    continue;
                }
                if (!TryBreach(fact, c.limit, out double breach))
                {
                    errors.Add($"{who} has an unknown unit or kind (limit {c.limit.value} {c.limit.unit}, kind '{c.limit.kind}')");
                    continue;
                }
                if (breach < RequiredMargin)
                    errors.Add($"{who}: the scene value {fact.value} {fact.unit} is only {breach:P0} past the limit of {c.limit.value} {c.limit.unit}; it must be at least {RequiredMargin:P0}");

                if (hasCompliant && TryBreach(compliant, c.limit, out double compliantBreach) && compliantBreach > 0)
                    errors.Add($"{who}: the compliant value {compliant.value} {compliant.unit} breaks the limit of {c.limit.value} {c.limit.unit}");
            }

            return errors;
        }
    }
}
