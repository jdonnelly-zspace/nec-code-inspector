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
            SpecialLocations
        };

        public static bool IsKnown(string conceptId)
        {
            return System.Array.IndexOf(All, conceptId) >= 0;
        }
    }
}
