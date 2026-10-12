namespace NECInspector.Data
{
    /// <summary>
    /// Code-neutral concept IDs. A violation is tagged with a concept so it can be
    /// matched to any installation code (NEC, CEC, BS 7671, HD 60364 national codes)
    /// instead of only to an NEC article number.
    /// </summary>
    public static class ConceptIds
    {
        public const string ShockProtection = "shock-protection";                 // GFCI / RCD protection
        public const string ArcFaultProtection = "arc-fault-protection";          // AFCI / AFDD protection
        public const string OvercurrentProtection = "overcurrent-protection";    // OCPD rating vs conductor or load
        public const string ConductorSizing = "conductor-sizing";                 // ampacity, derating, feeder and motor conductors
        public const string EarthingBonding = "earthing-bonding";                 // grounding, earthing and bonding
        public const string BranchCircuitRequirements = "branch-circuit-requirements"; // required circuits, outlet spacing
        public const string LoadCalculation = "load-calculation";
        public const string DisconnectingMeans = "disconnecting-means";
        public const string WorkingSpaceAccess = "working-space-access";          // clearances, mounting heights, accessibility
        public const string EquipmentInstallation = "equipment-installation";     // workmanship, terminations, panel capacity
        public const string IdentificationMarking = "identification-marking";     // directories, nameplates, labels
        public const string WiringMethods = "wiring-methods";                     // cable and raceway installation, support, burial depth, box fill, overhead clearances
        public const string SpecialLocations = "special-locations";               // pools and spas, wet-location devices, storage batteries

        // Procedure skills (wave 1 of the occupation-driven product, docs/product/DECISIONS.md): what a tradesperson does and how, as opposed to what an installation must look like.
        // They are taught by lessons and procedure practice, not by inspection violations or reference entries.
        public const string ElectricallySafeWork = "electrically-safe-work";         // make a circuit safe to work on and prove it: lockout and tagging, test before touch, PPE
        public const string TestingMeasurement = "testing-and-measurement";         // choose and use testers and meters, read what they show
        public const string Troubleshooting = "troubleshooting";                     // find the cause of a dead or tripping circuit step by step
        public const string PlansDocumentation = "plans-and-documentation";         // read drawings and schedules, lay out work, record findings
        public const string JobsiteSafety = "jobsite-safety";                       // ladders and falls, tools and cords, material handling, enclosed spaces, housekeeping
        public const string CodeAdministration = "code-administration";             // editions and amendments, permit and inspection stages, approve or reject
        public const string UtilityInterface = "utility-interface";                 // where the utility's equipment ends and the customer's begins

        /// <summary>The skills taught by code content: inspection violations, reference entries, the sandbox.</summary>
        public static readonly string[] CodeSkills =
        {
            ShockProtection, ArcFaultProtection, OvercurrentProtection, ConductorSizing, EarthingBonding, BranchCircuitRequirements, LoadCalculation,
            DisconnectingMeans, WorkingSpaceAccess, EquipmentInstallation, IdentificationMarking, WiringMethods, SpecialLocations
        };

        /// <summary>The skills taught by procedure practice and lessons.</summary>
        public static readonly string[] ProcedureSkills =
        {
            ElectricallySafeWork, TestingMeasurement, Troubleshooting, PlansDocumentation, JobsiteSafety, CodeAdministration, UtilityInterface
        };

        public static readonly string[] All =
        {
            ShockProtection,
            ArcFaultProtection,
            OvercurrentProtection,
            ConductorSizing,
            EarthingBonding,
            BranchCircuitRequirements,
            LoadCalculation,
            DisconnectingMeans,
            WorkingSpaceAccess,
            EquipmentInstallation,
            IdentificationMarking,
            WiringMethods,
            SpecialLocations,
            ElectricallySafeWork,
            TestingMeasurement,
            Troubleshooting,
            PlansDocumentation,
            JobsiteSafety,
            CodeAdministration,
            UtilityInterface
        };

        public static bool IsKnown(string conceptId)
        {
            return System.Array.IndexOf(All, conceptId) >= 0;
        }

        /// <summary>True for a skill taught by procedures rather than by code content.</summary>
        public static bool IsProcedureSkill(string conceptId)
        {
            return System.Array.IndexOf(ProcedureSkills, conceptId) >= 0;
        }
    }
}
