using System;
using System.Collections.Generic;

namespace NECInspector.Codes
{
    /// <summary>
    /// The kinds of panel design check the app can run. A code profile chooses its rule set in tables.json
    /// (<c>complianceRules</c>): which kinds, in what order, how many of each, with its own id, name, citation and
    /// parameters. Adding a rule that uses an existing kind is data only; a new kind needs C# in ComplianceRules
    /// (docs/COMPLIANCE_RULES.md).
    /// </summary>
    public static class ComplianceRuleKinds
    {
        public const string BreakerConductorMatch = "breaker-conductor-match";   // a breaker's rating is within its wire's ampacity (maxRatio)
        public const string RequiredCircuits = "required-circuits";              // every required circuit is present
        public const string ProtectionRequired = "protection-required";          // circuits that need GFCI or AFCI have it (protection)
        public const string LoadBalance = "load-balance";                        // load is balanced between the two bus sides (maxImbalance)
        public const string MainBreakerSizing = "main-breaker-sizing";           // the main covers the load (margin)
        public const string DoubleTap = "double-tap";                            // no breaker takes more slots than it has poles
        public const string ConductorAmpacity = "conductor-ampacity";            // each wire carries its breaker's rating (maxRatio)
        public const string PanelSpaces = "panel-spaces";                        // the breakers fit in the panel
        public const string WireConnections = "wire-connections";                // every breaker has a wire

        public static readonly string[] All =
        {
            BreakerConductorMatch, RequiredCircuits, ProtectionRequired, LoadBalance, MainBreakerSizing,
            DoubleTap, ConductorAmpacity, PanelSpaces, WireConnections
        };

        public static readonly string[] Protections = { "gfci", "afci" };

        // The built-in rule ids, in the order the original ten ran
        private static readonly Dictionary<string, (string kind, string protection)> BuiltIn = new Dictionary<string, (string, string)>
        {
            { "RULE-01", (BreakerConductorMatch, "") }, { "RULE-02", (RequiredCircuits, "") },
            { "RULE-03", (ProtectionRequired, "gfci") }, { "RULE-04", (ProtectionRequired, "afci") },
            { "RULE-05", (LoadBalance, "") }, { "RULE-06", (MainBreakerSizing, "") }, { "RULE-07", (DoubleTap, "") },
            { "RULE-08", (ConductorAmpacity, "") }, { "RULE-09", (PanelSpaces, "") }, { "RULE-10", (WireConnections, "") }
        };

        public static bool IsKnown(string kind) => Array.IndexOf(All, kind) >= 0;

        /// <summary>The kind of a built-in rule id (RULE-01 to RULE-10), or empty for a custom id.</summary>
        public static string Infer(string ruleId) => ruleId != null && BuiltIn.TryGetValue(ruleId, out var b) ? b.kind : "";

        /// <summary>The protection a built-in protection rule checks, or empty.</summary>
        public static string InferProtection(string ruleId) => ruleId != null && BuiltIn.TryGetValue(ruleId, out var b) ? b.protection : "";

        /// <summary>The kind a rule runs: its own, or the one implied by a built-in id (files written before kinds existed).</summary>
        public static string Resolve(ComplianceRuleConfig config)
        {
            if (config == null) return "";
            return string.IsNullOrEmpty(config.kind) ? Infer(config.ruleId) : config.kind;
        }

        /// <summary>The protection a protection rule checks: its own, or the one implied by a built-in id.</summary>
        public static string ResolveProtection(ComplianceRuleConfig config)
        {
            if (config == null) return "";
            return string.IsNullOrEmpty(config.protection) ? InferProtection(config.ruleId) : config.protection;
        }
    }
}
