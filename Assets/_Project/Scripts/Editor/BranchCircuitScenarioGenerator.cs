using UnityEngine;
using UnityEditor;
using System.IO;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class BranchCircuitScenarioGenerator
    {
        private struct ViolationData
        {
            public string violationId;
            public string description;
            public string necArticle;
            public string necArticleText;
            public ViolationSeverity severity;
            public DifficultyLevel minimumDifficulty;
            public bool isSubtle;
            public string componentObjectName;
            public string hintText;
            public string componentType;
            public string inspectionNote;
        }

        private static readonly ViolationData[] _violations = new ViolationData[]
        {
            // === BEGINNER (5 violations) ===
            new ViolationData
            {
                violationId = "BC-GFCI-BATH-001",
                description = "Standard receptacle at bathroom sink without GFCI protection",
                necArticle = "210.8(A)(1)",
                necArticleText = "All 125-volt through 250-volt receptacles in bathrooms of dwelling units shall have GFCI protection.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "BathroomReceptacle_Sink",
                hintText = "Check if bathroom outlets have GFCI protection. Look for the TEST/RESET buttons on the receptacle face.",
                componentType = "Receptacle",
                inspectionNote = "This bathroom receptacle lacks required GFCI protection. Water proximity creates an electrocution hazard."
            },
            new ViolationData
            {
                violationId = "BC-GFCI-KITCHEN-001",
                description = "Kitchen countertop receptacle lacks GFCI protection",
                necArticle = "210.8(A)(5)",
                necArticleText = "All 125-volt through 250-volt receptacles that serve countertop surfaces in kitchens of dwelling units shall have GFCI protection.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "KitchenReceptacle_Counter1",
                hintText = "Kitchen countertop outlets need GFCI protection. Check for TEST/RESET buttons.",
                componentType = "Receptacle",
                inspectionNote = "This kitchen countertop receptacle is missing required GFCI protection near water sources."
            },
            new ViolationData
            {
                violationId = "BC-GFCI-GARAGE-001",
                description = "Garage receptacle without GFCI protection",
                necArticle = "210.8(A)(2)",
                necArticleText = "All 125-volt through 250-volt receptacles in garages and accessory buildings with floors at or below grade level shall have GFCI protection.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "GarageReceptacle_Workbench",
                hintText = "Garages at or below grade level require GFCI-protected outlets.",
                componentType = "Receptacle",
                inspectionNote = "Garage receptacle at grade level is missing required GFCI protection."
            },
            new ViolationData
            {
                violationId = "BC-SPACING-WALL-001",
                description = "Wall receptacles spaced 14 feet apart, exceeding 12-foot maximum",
                necArticle = "210.52(A)",
                necArticleText = "Receptacle outlets shall be installed so that no point measured horizontally along the floor line of any wall space is more than 6 feet from a receptacle outlet.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "LivingRoom_WallGap",
                hintText = "No point along a wall should be more than 6 feet from a receptacle. That means outlets must be no more than 12 feet apart.",
                componentType = "Wall Space",
                inspectionNote = "This wall section has a 14-foot gap between receptacles, exceeding the 12-foot maximum spacing rule."
            },
            new ViolationData
            {
                violationId = "BC-WIRE-14AWG-001",
                description = "14 AWG conductor connected to a 20-ampere breaker",
                necArticle = "240.4(D)",
                necArticleText = "The overcurrent protection shall not exceed 15 amperes for 14 AWG, 20 amperes for 12 AWG, and 30 amperes for 10 AWG copper.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "KitchenCircuit_Wire",
                hintText = "Check the wire gauge against the breaker size. 14 AWG wire can only be on a 15-amp breaker.",
                componentType = "Conductor",
                inspectionNote = "14 AWG wire is connected to a 20A breaker. This wire is rated for only 15A and could overheat under load."
            },

            // === STANDARD (5 additional violations) ===
            new ViolationData
            {
                violationId = "BC-AFCI-BEDROOM-001",
                description = "Bedroom branch circuit protected by standard breaker instead of AFCI",
                necArticle = "210.12(A)",
                necArticleText = "All 120-volt, single-phase, 15- and 20-ampere branch circuits supplying outlets and devices installed in dwelling unit kitchens, family rooms, dining rooms, living rooms, parlors, libraries, dens, bedrooms, sunrooms, recreation rooms, closets, hallways, laundry areas, and similar rooms or areas shall be protected by any of the means described in 210.12(A)(1) through (A)(6).",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "BedroomCircuit_Breaker",
                hintText = "Bedroom circuits require arc-fault circuit interrupter (AFCI) protection at the panel.",
                componentType = "Breaker",
                inspectionNote = "This bedroom circuit uses a standard breaker. AFCI protection is required to prevent electrical fires from arc faults."
            },
            new ViolationData
            {
                violationId = "BC-AFCI-LIVING-001",
                description = "Living room branch circuit missing AFCI protection at panel",
                necArticle = "210.12(A)",
                necArticleText = "All 120-volt, single-phase, 15- and 20-ampere branch circuits supplying outlets and devices installed in dwelling unit kitchens, family rooms, dining rooms, living rooms, parlors, libraries, dens, bedrooms, sunrooms, recreation rooms, closets, hallways, laundry areas, and similar rooms or areas shall be protected by any of the means described in 210.12(A)(1) through (A)(6).",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "LivingRoomCircuit_Breaker",
                hintText = "Living room circuits also require AFCI protection. Check the breaker type at the panel.",
                componentType = "Breaker",
                inspectionNote = "Living room circuit lacks AFCI protection. The 2026 NEC requires AFCI for virtually all dwelling unit living spaces."
            },
            new ViolationData
            {
                violationId = "BC-SPACING-COUNTER-001",
                description = "Kitchen countertop gap exceeds 24-inch receptacle spacing rule",
                necArticle = "210.52(C)",
                necArticleText = "Receptacle outlets for countertop spaces shall be installed so that no point along the wall line is more than 24 inches measured horizontally from a receptacle outlet in that space.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "KitchenCountertop_Gap",
                hintText = "Countertop receptacles must be within 24 inches of any point along the counter wall line.",
                componentType = "Countertop Space",
                inspectionNote = "This countertop section has a 36-inch gap without a receptacle, exceeding the 24-inch maximum."
            },
            new ViolationData
            {
                violationId = "BC-DEDICATED-BATH-001",
                description = "Bathroom receptacle circuit shared with hallway, not a dedicated branch circuit",
                necArticle = "210.11(C)(1)",
                necArticleText = "At least one 120-volt, 20-ampere branch circuit shall be provided to supply bathroom receptacle outlet(s).",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "BathroomCircuit_SharedBreaker",
                hintText = "Bathroom receptacles require a dedicated 20A branch circuit. Check if the circuit also serves other rooms.",
                componentType = "Breaker",
                inspectionNote = "This breaker serves both bathroom and hallway outlets. Bathroom receptacles require a dedicated 20A branch circuit."
            },
            new ViolationData
            {
                violationId = "BC-DEDICATED-KITCHEN-001",
                description = "Only one small-appliance branch circuit serving kitchen instead of required two",
                necArticle = "210.11(C)(3)",
                necArticleText = "Two or more 20-ampere small-appliance branch circuits shall be provided for kitchen receptacle outlets.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "KitchenPanel_SmallAppliance",
                hintText = "Kitchens require at least two 20A small-appliance branch circuits. Count the circuits at the panel.",
                componentType = "Panel Circuit",
                inspectionNote = "Only one small-appliance circuit is provided for the kitchen. The NEC requires a minimum of two 20A circuits."
            },

            // === EXPERT (2 subtle violations) ===
            new ViolationData
            {
                violationId = "BC-WIRE-KITCHEN-001",
                description = "Four NM cables bundled through single stud hole without ampacity derate",
                necArticle = "334.80",
                necArticleText = "Where more than two NM cables containing two or more current-carrying conductors are installed through the same opening in wood framing without maintaining spacing, the ampacity of each conductor shall be adjusted in accordance with Table 310.15(C)(1).",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "KitchenWall_CableBundleHole",
                hintText = "When multiple NM cables pass through the same hole, ampacity derating may be required.",
                componentType = "Cable Bundle",
                inspectionNote = "Four NM cables are bundled through a single stud hole. With this many cables, ampacity must be derated per Table 310.15(C)(1), potentially making the conductors undersized for their breakers."
            },
            new ViolationData
            {
                violationId = "BC-GFCI-DISHWASHER-001",
                description = "Dishwasher branch circuit lacks GFCI protection per 2026 NEC requirement",
                necArticle = "210.8(D)",
                necArticleText = "GFCI protection shall be provided for dishwashers in dwelling unit locations. The branch circuit supplying the dishwasher shall be protected by a listed GFCI device.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "KitchenDishwasher_Circuit",
                hintText = "The 2026 NEC added GFCI requirements for dishwashers. Check the breaker type.",
                componentType = "Appliance Circuit",
                inspectionNote = "Dishwasher circuit lacks GFCI protection. This is a new requirement in the 2026 NEC edition (Art. 210.8(D))."
            }
        };

        [MenuItem("NEC Inspector/Generate Branch Circuit Scenario")]
        public static void Generate()
        {
            const string VIOLATION_DIR = "Assets/_Project/ScriptableObjects/Violations/BranchCircuits";
            const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

            // Ensure directories exist
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations/BranchCircuits"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations"))
                    AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Violations");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects/Violations", "BranchCircuits");
            }
            if (!AssetDatabase.IsValidFolder(SCENARIO_DIR))
            {
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Scenarios");
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                // Create violation assets
                var violationAssets = new ViolationDefinitionSO[_violations.Length];
                for (int i = 0; i < _violations.Length; i++)
                {
                    var data = _violations[i];
                    string assetPath = $"{VIOLATION_DIR}/VD_BC_{data.violationId}.asset";

                    // Load existing or create new
                    var asset = AssetDatabase.LoadAssetAtPath<ViolationDefinitionSO>(assetPath);
                    if (asset == null)
                    {
                        asset = ScriptableObject.CreateInstance<ViolationDefinitionSO>();
                        AssetDatabase.CreateAsset(asset, assetPath);
                    }

                    asset.violationId = data.violationId;
                    asset.description = data.description;
                    asset.necArticle = data.necArticle;
                    asset.necArticleText = data.necArticleText;
                    asset.severity = data.severity;
                    asset.minimumDifficulty = data.minimumDifficulty;
                    asset.isSubtle = data.isSubtle;
                    asset.componentObjectName = data.componentObjectName;
                    asset.highlightOffset = Vector3.zero;
                    asset.hintText = data.hintText;
                    asset.componentType = data.componentType;
                    asset.inspectionNote = data.inspectionNote;

                    EditorUtility.SetDirty(asset);
                    violationAssets[i] = asset;
                }

                // Create scenario definition
                string scenarioPath = $"{SCENARIO_DIR}/ScenarioDefinition_BranchCircuits.asset";
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(scenarioPath);
                if (scenario == null)
                {
                    scenario = ScriptableObject.CreateInstance<ScenarioDefinitionSO>();
                    AssetDatabase.CreateAsset(scenario, scenarioPath);
                }

                scenario.sceneName = "BranchCircuitInspection";
                scenario.displayName = "Branch Circuit Wiring Inspection";
                scenario.description = "Inspect a residential kitchen, bathroom, and living area electrical installation for branch circuit code violations including GFCI/AFCI protection, receptacle spacing, wire sizing, and dedicated circuit requirements.";
                scenario.availableDifficulties = new DifficultyLevel[]
                {
                    DifficultyLevel.Beginner,
                    DifficultyLevel.Standard,
                    DifficultyLevel.Expert
                };
                scenario.violations = violationAssets;
                scenario.necChapters = new string[] { "2", "3" };
                scenario.expertTimeLimit = 1200;
                scenario.environmentDescription = "A residential dwelling with exposed kitchen, bathroom, garage, bedroom, and living room electrical installations showing branch circuit wiring, receptacles, breakers, and conductors.";

                EditorUtility.SetDirty(scenario);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Branch Circuit scenario generated: {_violations.Length} violations + 1 scenario definition.");
            Debug.Log("[NEC Inspector] Remember to add the scenario to your ScenarioCatalog asset.");
        }
    }
}
