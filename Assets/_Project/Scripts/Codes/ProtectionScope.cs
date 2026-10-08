using System;
using System.Collections.Generic;
using System.Linq;

namespace NECInspector.Codes
{
    /// <summary>
    /// Where a code requires ground-fault (GFCI) or arc-fault (AFCI) protection in a dwelling, as data
    /// (<c>scope.json</c> in a code's folder). Codes test this differently: the NEC mostly by room, the CEC
    /// mostly by distance from a fixture and by exemptions. One shape covers both, so a scene can be checked
    /// automatically under any code that ships a scope file.
    /// </summary>
    [Serializable]
    public class ProtectionScope
    {
        public string status;                   // draft | reviewed
        public string note;
        public ScopeRule[] rules;
        public ScopeExemption[] exemptions;
        public string[] notModeled;             // protection the code may require that has no verified rule here
    }

    /// <summary>One place protection is required. Every condition that is set must hold; empty or zero means "not tested".</summary>
    [Serializable]
    public class ScopeRule
    {
        public string id;
        public string protection;               // gfci | afci
        public string reference;                // the code's reference for the rule
        public string[] rooms;                  // the receptacle is in one of these room types
        public string[] excludeRooms;
        public string[] nearFixtureTypes;       // the receptacle is within nearFixtureMaxM of one of these fixtures
        public float nearFixtureMaxM;
        public float heightAboveGradeMaxM;      // outdoor receptacles within this height of finished grade
        public bool floorAtOrBelowGrade;        // the room's floor is at or below grade
        public string appliance;                // the receptacle serves this appliance
        public float minVolts;
        public float maxVolts;
        public float minAmps;
        public float maxAmps;
    }

    /// <summary>A receptacle that a rule does not cover. Every condition that is set must hold.</summary>
    [Serializable]
    public class ScopeExemption
    {
        public string id;
        public string protection;               // gfci | afci
        public string reference;
        public string[] appliances;             // the receptacle serves one of these appliances
        public string[] rooms;
        public bool onCounter;                  // a counter work surface receptacle
        public bool behindAppliance;            // behind a fixed appliance where portable appliances cannot reach it
        public string[] nearFixtureTypes;
        public float nearFixtureMaxM;
    }

    [Serializable]
    public class ScopeFixtureDistance
    {
        public string type;                     // sink | bathtub | shower | laundry-tub
        public float distanceM;
    }

    /// <summary>A receptacle in a scene, with what the rules test.</summary>
    [Serializable]
    public class ScopeReceptacle
    {
        public string id;
        public string room;                     // a room type from ScopeVocabulary.Rooms
        public bool floorAtOrBelowGrade;
        public float volts = 125f;     // rated voltage
        public float amps = 15f;
        public string appliance;
        public bool onCounter;
        public bool behindAppliance;
        public float heightAboveGradeM;
        public ScopeFixtureDistance[] fixtures;
        public string[] protectedBy;            // gfci | afci | dual (dual-function counts as both)
    }

    /// <summary>The offending receptacle a violation's scene shows (see docs/SCENE_DESIGN.md).</summary>
    [Serializable]
    public class ScopeSceneFact
    {
        public string protection;               // gfci | afci: the protection the receptacle lacks
        public ScopeReceptacle receptacle;
    }

    public static class ScopeVocabulary
    {
        public const string Gfci = "gfci";
        public const string Afci = "afci";
        public const string Dual = "dual";

        public static readonly string[] Protections = { Gfci, Afci };

        public static readonly string[] Rooms =
        {
            "bathroom", "kitchen", "garage", "accessory-building", "outdoors", "crawl-space", "basement", "laundry",
            "boathouse", "indoor-damp-wet", "family-room", "dining-room", "living-room", "parlor", "library", "den",
            "bedroom", "sunroom", "recreation-room", "closet", "hallway", "other"
        };

        public static readonly string[] Fixtures = { "sink", "bathtub", "shower", "laundry-tub" };
        public static readonly string[] Appliances = { "dishwasher", "refrigerator", "freezer", "other" };
    }

    public static class ProtectionScopeValidator
    {
        /// <summary>One message per problem; empty if the scope data is usable.</summary>
        public static List<string> Validate(ProtectionScope scope)
        {
            var errors = new List<string>();
            if (scope == null)
            {
                errors.Add("scope is empty");
                return errors;
            }

            var ids = new HashSet<string>();
            foreach (var r in scope.rules ?? new ScopeRule[0])
            {
                string label = string.IsNullOrEmpty(r?.id) ? "(rule without id)" : r.id;
                if (r == null || string.IsNullOrEmpty(r.id)) { errors.Add("a rule has no id"); continue; }
                if (!ids.Add(r.id)) errors.Add($"duplicate id '{r.id}'");
                if (Array.IndexOf(ScopeVocabulary.Protections, r.protection) < 0) errors.Add($"{label}: unknown protection '{r.protection}'");
                if (string.IsNullOrWhiteSpace(r.reference)) errors.Add($"{label}: reference is missing");
                CheckNames(errors, label, "rooms", r.rooms, ScopeVocabulary.Rooms);
                CheckNames(errors, label, "excludeRooms", r.excludeRooms, ScopeVocabulary.Rooms);
                CheckNames(errors, label, "nearFixtureTypes", r.nearFixtureTypes, ScopeVocabulary.Fixtures);
                if (!string.IsNullOrEmpty(r.appliance) && Array.IndexOf(ScopeVocabulary.Appliances, r.appliance) < 0)
                    errors.Add($"{label}: unknown appliance '{r.appliance}'");
                if (r.nearFixtureTypes != null && r.nearFixtureTypes.Length > 0 && r.nearFixtureMaxM <= 0)
                    errors.Add($"{label}: nearFixtureTypes needs a nearFixtureMaxM above zero");
                if (r.maxVolts > 0 && r.minVolts > r.maxVolts) errors.Add($"{label}: minVolts is above maxVolts");
                if (r.maxAmps > 0 && r.minAmps > r.maxAmps) errors.Add($"{label}: minAmps is above maxAmps");
            }

            foreach (var e in scope.exemptions ?? new ScopeExemption[0])
            {
                string label = string.IsNullOrEmpty(e?.id) ? "(exemption without id)" : e.id;
                if (e == null || string.IsNullOrEmpty(e.id)) { errors.Add("an exemption has no id"); continue; }
                if (!ids.Add(e.id)) errors.Add($"duplicate id '{e.id}'");
                if (Array.IndexOf(ScopeVocabulary.Protections, e.protection) < 0) errors.Add($"{label}: unknown protection '{e.protection}'");
                CheckNames(errors, label, "appliances", e.appliances, ScopeVocabulary.Appliances);
                CheckNames(errors, label, "rooms", e.rooms, ScopeVocabulary.Rooms);
                CheckNames(errors, label, "nearFixtureTypes", e.nearFixtureTypes, ScopeVocabulary.Fixtures);
                if (e.nearFixtureTypes != null && e.nearFixtureTypes.Length > 0 && e.nearFixtureMaxM <= 0)
                    errors.Add($"{label}: nearFixtureTypes needs a nearFixtureMaxM above zero");
            }

            return errors;
        }

        public static List<string> ValidateReceptacle(string label, ScopeReceptacle r)
        {
            var errors = new List<string>();
            if (r == null) { errors.Add($"{label}: receptacle is missing"); return errors; }
            if (Array.IndexOf(ScopeVocabulary.Rooms, r.room) < 0) errors.Add($"{label}: unknown room '{r.room}'");
            foreach (var f in r.fixtures ?? new ScopeFixtureDistance[0])
                if (f == null || Array.IndexOf(ScopeVocabulary.Fixtures, f.type) < 0) errors.Add($"{label}: unknown fixture '{f?.type}'");
            foreach (string p in r.protectedBy ?? new string[0])
                if (p != ScopeVocabulary.Gfci && p != ScopeVocabulary.Afci && p != ScopeVocabulary.Dual) errors.Add($"{label}: unknown protection '{p}'");
            if (!string.IsNullOrEmpty(r.appliance) && Array.IndexOf(ScopeVocabulary.Appliances, r.appliance) < 0)
                errors.Add($"{label}: unknown appliance '{r.appliance}'");
            return errors;
        }

        private static void CheckNames(List<string> errors, string label, string field, string[] values, string[] allowed)
        {
            foreach (string v in values ?? new string[0])
                if (Array.IndexOf(allowed, v) < 0) errors.Add($"{label}: unknown {field} value '{v}'");
        }
    }

    /// <summary>The result for one receptacle and one kind of protection.</summary>
    public class ScopeFinding
    {
        public string receptacleId;
        public string protection;
        public bool required;
        public string ruleId;           // the rule that requires it
        public string reference;
        public bool exempt;             // a rule would require it but an exemption removes it
        public string exemptionId;
        public bool provided;           // the receptacle has the protection
        public bool Missing => required && !provided;
    }

    public static class ProtectionScopeChecker
    {
        /// <summary>The required-protection findings for a receptacle: one per protection kind.</summary>
        public static List<ScopeFinding> Check(ProtectionScope scope, ScopeReceptacle receptacle)
        {
            var findings = new List<ScopeFinding>();
            foreach (string protection in ScopeVocabulary.Protections)
                findings.Add(Check(scope, receptacle, protection));
            return findings;
        }

        public static ScopeFinding Check(ProtectionScope scope, ScopeReceptacle receptacle, string protection)
        {
            var finding = new ScopeFinding
            {
                receptacleId = receptacle?.id,
                protection = protection,
                provided = IsProvided(receptacle, protection)
            };
            if (scope == null || receptacle == null) return finding;

            var rule = (scope.rules ?? new ScopeRule[0]).FirstOrDefault(r => r != null && r.protection == protection && Applies(r, receptacle));
            if (rule == null) return finding;

            var exemption = (scope.exemptions ?? new ScopeExemption[0]).FirstOrDefault(e => e != null && e.protection == protection && Exempts(e, receptacle));
            finding.ruleId = rule.id;
            finding.reference = rule.reference;
            if (exemption != null)
            {
                finding.exempt = true;
                finding.exemptionId = exemption.id;
                return finding;
            }

            finding.required = true;
            return finding;
        }

        /// <summary>Receptacles in a scene that need protection they do not have.</summary>
        public static List<ScopeFinding> MissingProtection(ProtectionScope scope, IEnumerable<ScopeReceptacle> receptacles)
        {
            return (receptacles ?? new ScopeReceptacle[0])
                .SelectMany(r => Check(scope, r))
                .Where(f => f.Missing)
                .ToList();
        }

        /// <summary>A dual-function device counts as both kinds of protection.</summary>
        public static bool IsProvided(ScopeReceptacle r, string protection)
        {
            return (r?.protectedBy ?? new string[0]).Any(p => p == protection || p == ScopeVocabulary.Dual);
        }

        private static bool Applies(ScopeRule rule, ScopeReceptacle r)
        {
            if (rule.rooms != null && rule.rooms.Length > 0 && Array.IndexOf(rule.rooms, r.room) < 0) return false;
            if (rule.excludeRooms != null && Array.IndexOf(rule.excludeRooms, r.room) >= 0) return false;
            if (rule.floorAtOrBelowGrade && !r.floorAtOrBelowGrade) return false;
            if (!string.IsNullOrEmpty(rule.appliance) && rule.appliance != r.appliance) return false;
            if (rule.heightAboveGradeMaxM > 0 && r.heightAboveGradeM > rule.heightAboveGradeMaxM) return false;
            if (rule.maxVolts > 0 && (r.volts < rule.minVolts || r.volts > rule.maxVolts)) return false;
            if (rule.maxVolts <= 0 && rule.minVolts > 0 && r.volts < rule.minVolts) return false;
            if (rule.maxAmps > 0 && (r.amps < rule.minAmps || r.amps > rule.maxAmps)) return false;
            if (rule.maxAmps <= 0 && rule.minAmps > 0 && r.amps < rule.minAmps) return false;
            if (rule.nearFixtureTypes != null && rule.nearFixtureTypes.Length > 0 && !Near(r, rule.nearFixtureTypes, rule.nearFixtureMaxM)) return false;
            return true;
        }

        private static bool Exempts(ScopeExemption e, ScopeReceptacle r)
        {
            if (e.appliances != null && e.appliances.Length > 0 && Array.IndexOf(e.appliances, r.appliance) < 0) return false;
            if (e.rooms != null && e.rooms.Length > 0 && Array.IndexOf(e.rooms, r.room) < 0) return false;
            if (e.onCounter && !r.onCounter) return false;
            if (e.behindAppliance && !r.behindAppliance) return false;
            if (e.nearFixtureTypes != null && e.nearFixtureTypes.Length > 0 && !Near(r, e.nearFixtureTypes, e.nearFixtureMaxM)) return false;
            return true;
        }

        // A distance exactly at the limit counts as within it
        private static bool Near(ScopeReceptacle r, string[] types, float maxM)
        {
            return (r.fixtures ?? new ScopeFixtureDistance[0]).Any(f => f != null && Array.IndexOf(types, f.type) >= 0 && f.distanceM <= maxM);
        }
    }
}
