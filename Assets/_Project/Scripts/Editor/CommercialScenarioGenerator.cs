using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class CommercialScenarioGenerator
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
            // === BEGINNER (4 violations) ===
            new ViolationData
            {
                violationId = "COM-MOTOR-OCPD-001",
                description = "Motor branch circuit overcurrent protection device exceeds Table 430.52 maximum",
                necArticle = "430.52",
                necArticleText = "The motor branch-circuit short-circuit and ground-fault protective device shall comply with the percentages listed in Table 430.52 and shall not exceed the maximum value resulting from application of Table 430.52.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Motor1_BranchBreaker",
                hintText = "Check the breaker size against the motor's full-load current. Table 430.52 sets the maximum OCPD size for different motor types.",
                componentType = "Motor OCPD",
                inspectionNote = "The 50A breaker on this 10HP motor exceeds the maximum allowed by Table 430.52 for an inverse-time breaker (typically 250% of FLC)."
            },
            new ViolationData
            {
                violationId = "COM-MOTOR-WIRE-001",
                description = "Motor branch circuit conductors undersized — not 125% of motor full-load current",
                necArticle = "430.22",
                necArticleText = "Conductors supplying a single motor used in a continuous duty application shall have an ampacity of not less than 125 percent of the motor full-load current rating as determined by 430.6(A)(1).",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Motor2_BranchConductor",
                hintText = "Motor branch circuit conductors must be sized at 125% of the motor's full-load current from Table 430.248/250.",
                componentType = "Motor Conductor",
                inspectionNote = "10 AWG conductors supply a 7.5HP motor with 22A FLC. Required ampacity is 27.5A (125% of 22A), but 10 AWG is only rated 30A at 60C — marginal with temperature derating applied."
            },
            new ViolationData
            {
                violationId = "COM-DISC-SIGHT-001",
                description = "Motor disconnect is not within sight of the motor controller",
                necArticle = "430.110",
                necArticleText = "The disconnecting means for motor circuits rated 600 volts nominal or less shall have an ampere rating not less than 115 percent of the full-load current rating of the motor.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Motor1_Disconnect_Remote",
                hintText = "The disconnect must be within sight from the motor and its controller. 'Within sight' means visible and not more than 50 feet away.",
                componentType = "Motor Disconnect",
                inspectionNote = "The motor disconnect is located around a corner, out of sight from the motor. NEC requires the disconnect to be within sight of the motor it controls."
            },
            new ViolationData
            {
                violationId = "COM-MOTOR-OL-001",
                description = "Motor overload protection exceeds 115% of nameplate full-load amperes",
                necArticle = "430.32",
                necArticleText = "Each continuous-duty motor rated more than 1 hp shall be protected against overload by a separate overload device that is responsive to motor current. This device shall be selected to trip or shall be rated at not more than 115 percent of the motor nameplate full-load current rating for motors marked with a service factor not less than 1.15.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Motor2_OverloadRelay",
                hintText = "Check the overload relay setting against the motor nameplate current. For motors with SF >= 1.15, the maximum is 115% of nameplate FLA.",
                componentType = "Motor Overload",
                inspectionNote = "The overload relay is set to 30A for a motor with 22A nameplate FLA (136% of FLA). Maximum should be 25.3A (115% of 22A) for a motor with service factor 1.15."
            },

            // === STANDARD (4 additional violations) ===
            new ViolationData
            {
                violationId = "COM-LOAD-CALC-001",
                description = "Calculated service load does not account for all connected loads",
                necArticle = "220.40",
                necArticleText = "The calculated load of a feeder or service shall not be less than the sum of the loads on the branch circuits supplied, as determined by Part II of this article, after any applicable demand factors permitted by Parts III or IV or V have been applied.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "ServicePanel_LoadSchedule",
                hintText = "Verify the load schedule accounts for all connected loads: lighting, receptacles, motors, HVAC, and special equipment.",
                componentType = "Service Load",
                inspectionNote = "The load schedule omits the rooftop HVAC unit (15 kVA) from the total service calculation, resulting in an undersized service."
            },
            new ViolationData
            {
                violationId = "COM-FEEDER-SIZE-001",
                description = "Feeder conductor undersized for the calculated load it serves",
                necArticle = "215.2",
                necArticleText = "Feeder conductors shall have an ampacity not less than required to supply the load as calculated in Parts III, IV, and V of Article 220.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Feeder_SubPanel_Conductor",
                hintText = "Calculate the total load on this feeder and compare it to the conductor ampacity. Don't forget to add 125% for continuous loads.",
                componentType = "Feeder Conductor",
                inspectionNote = "The feeder to the mechanical room subpanel uses 4 AWG copper (85A at 75C) but serves a calculated load of 95A including continuous lighting."
            },
            new ViolationData
            {
                violationId = "COM-RECPT-LOAD-001",
                description = "Receptacle outlets not calculated at minimum 180 VA per outlet",
                necArticle = "220.44",
                necArticleText = "Receptacle loads calculated in accordance with 220.14 shall be permitted to be made subject to the demand factors given in Table 220.44.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Office_ReceptacleLoadCalc",
                hintText = "In commercial occupancies, each general-use receptacle outlet must be calculated at a minimum of 180 VA per strap.",
                componentType = "Receptacle Load",
                inspectionNote = "The load calculation uses 90 VA per receptacle instead of the required minimum 180 VA per outlet per NEC 220.14."
            },
            new ViolationData
            {
                violationId = "COM-MOTOR-GND-001",
                description = "Equipment grounding conductor undersized for the motor branch circuit OCPD",
                necArticle = "250.122",
                necArticleText = "Equipment grounding conductors shall not be smaller than shown in Table 250.122 based on the rating or setting of the overcurrent device protecting the circuit conductors.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Motor1_EGC",
                hintText = "The equipment grounding conductor must be sized based on the rating of the overcurrent device, per Table 250.122.",
                componentType = "Equipment Grounding Conductor",
                inspectionNote = "A 14 AWG EGC is used on a motor circuit protected by a 40A breaker. Table 250.122 requires a minimum 10 AWG copper EGC for a 40A OCPD."
            },

            // === EXPERT (4 subtle violations) ===
            new ViolationData
            {
                violationId = "COM-MULTI-MOTOR-001",
                description = "Multiple motors on a single branch circuit without proper overcurrent protection sizing",
                necArticle = "430.24",
                necArticleText = "Conductors supplying several motors, or a motor(s) and other load(s), shall have an ampacity not less than 125 percent of the full-load current rating of the highest rated motor plus the sum of the full-load current ratings of all the other motors in the group.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "MechRoom_MultiMotorFeeder",
                hintText = "When multiple motors share a feeder, size it at 125% of the largest motor FLC plus 100% of all other motor FLCs.",
                componentType = "Multi-Motor Feeder",
                inspectionNote = "The mechanical room feeder supplies three motors but is sized at only 100% of the largest motor plus the sum of the others, missing the required 125% factor on the largest motor."
            },
            new ViolationData
            {
                violationId = "COM-MOTOR-CTRL-001",
                description = "Motor controller disconnect not rated for locked-rotor current",
                necArticle = "430.102(B)",
                necArticleText = "A disconnecting means shall be located in sight from the motor location and the driven machinery location. The disconnecting means shall disconnect the motor and the controller from all ungrounded supply conductors.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "Motor2_Controller_Disc",
                hintText = "The disconnect for a motor controller must be rated to handle the motor's locked-rotor current, not just full-load current.",
                componentType = "Motor Controller",
                inspectionNote = "The disconnect switch at the VFD is rated for 30A but the motor has a locked-rotor current of 145A. The disconnect must be horsepower-rated or capable of interrupting locked-rotor current."
            },
            new ViolationData
            {
                violationId = "COM-MOTOR-MARK-001",
                description = "Motor nameplate data missing or illegible",
                necArticle = "430.7",
                necArticleText = "A motor shall be marked with the manufacturer's name, rated volts, rated frequency, number of phases, rated full-load current, rated temperature rise or insulation system class and rated ambient temperature, time rating, rated horsepower, locked-rotor code letter.",
                severity = ViolationSeverity.Minor,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "Motor3_Nameplate",
                hintText = "Every motor must have a legible nameplate with all required markings. Can you read the nameplate on this motor?",
                componentType = "Motor Nameplate",
                inspectionNote = "The nameplate on this exhaust fan motor is painted over and illegible. Without readable nameplate data, proper conductor sizing and overcurrent protection cannot be verified."
            },
            new ViolationData
            {
                violationId = "COM-FEEDER-TAP-001",
                description = "Feeder tap conductor does not meet the 10-foot tap rule requirements",
                necArticle = "240.21(B)(1)",
                necArticleText = "Conductors shall be permitted to be tapped, without overcurrent protection at the tap, to a feeder where: the length of the tap conductors does not exceed 3.0 m (10 ft), the ampacity of the tap conductors is not less than the combined calculated loads on the circuits supplied by the tap conductors, and the tap conductors are enclosed in a raceway.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "Feeder_TapConductor",
                hintText = "Feeder taps without overcurrent protection at the tap point must meet strict length, ampacity, and raceway requirements.",
                componentType = "Feeder Tap",
                inspectionNote = "A 12-foot feeder tap is run without overcurrent protection at the tap point and without being enclosed in a raceway. The 10-foot tap rule requires the tap to be no longer than 10 feet and enclosed in a raceway."
            }
        };

        [MenuItem("NEC Inspector/Generate Commercial Scenario")]
        public static void Generate()
        {
            const string VIOLATION_DIR = "Assets/_Project/ScriptableObjects/Violations/Commercial";
            const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

            // Ensure directories exist
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations/Commercial"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations"))
                    AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Violations");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects/Violations", "Commercial");
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
                    string assetPath = $"{VIOLATION_DIR}/VD_COM_{data.violationId}.asset";

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
                string scenarioPath = $"{SCENARIO_DIR}/ScenarioDefinition_Commercial.asset";
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(scenarioPath);
                if (scenario == null)
                {
                    scenario = ScriptableObject.CreateInstance<ScenarioDefinitionSO>();
                    AssetDatabase.CreateAsset(scenario, scenarioPath);
                }

                scenario.id = "scenario-commercial";
                scenario.sceneName = "CommercialInspection";
                scenario.displayName = "Commercial Installation Inspection";
                scenario.description = "Inspect a small commercial building's electrical installation including motor circuits, feeders, load calculations, and equipment disconnects. Verify proper conductor sizing, overcurrent protection, and NEC compliance for commercial occupancies.";
                scenario.availableDifficulties = new DifficultyLevel[]
                {
                    DifficultyLevel.Beginner,
                    DifficultyLevel.Standard,
                    DifficultyLevel.Expert
                };
                scenario.violations = violationAssets;
                scenario.necChapters = new string[] { "2", "4" };
                scenario.expertTimeLimit = 1500;
                scenario.environmentDescription = "A small commercial space with multiple motor installations (HVAC compressors, exhaust fans), a mechanical room with subpanel and feeders, a battery/UPS closet, and an office area with general receptacle and lighting circuits.";

                EditorUtility.SetDirty(scenario);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Commercial scenario generated: {_violations.Length} violations + 1 scenario definition.");
        }
    }
}
