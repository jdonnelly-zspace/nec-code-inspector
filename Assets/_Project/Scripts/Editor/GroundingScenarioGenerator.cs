using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class GroundingScenarioGenerator
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
                violationId = "GND-ELECTRODE-001",
                description = "Ground rod not meeting minimum 8-foot length requirement",
                necArticle = "250.52(A)(5)",
                necArticleText = "Rod-type grounding electrodes of stainless steel, copper, or zinc coated steel shall be at least 8 feet in length and 5/8 inch in diameter.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "GroundRod_Main",
                hintText = "Ground rods must be at least 8 feet long and 5/8 inch diameter. Check the rod length.",
                componentType = "Ground Rod",
                inspectionNote = "This ground rod is only 6 feet long, failing to meet the 8-foot minimum required by code."
            },
            new ViolationData
            {
                violationId = "GND-GEC-001",
                description = "Grounding electrode conductor not connected to service equipment",
                necArticle = "250.24(A)(1)",
                necArticleText = "A grounding electrode conductor shall be used to connect the equipment grounding conductors, the service-equipment enclosures, and, where the system is grounded, the grounded conductor to the grounding electrode(s).",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "GEC_ServicePanel",
                hintText = "The grounding electrode conductor must connect from the panel to the grounding electrode. Look for a disconnected or missing conductor.",
                componentType = "Conductor",
                inspectionNote = "Grounding electrode conductor is disconnected at the service panel. No path to earth exists."
            },
            new ViolationData
            {
                violationId = "GND-BOND-WATER-001",
                description = "Metal water piping not bonded to grounding system",
                necArticle = "250.104(A)",
                necArticleText = "The metal water piping system shall be bonded to the service equipment enclosure, the grounded conductor at the service, the grounding electrode conductor where of sufficient size, or to the one or more grounding electrodes used.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "WaterPipe_Bonding",
                hintText = "Metal water pipes must be bonded to the grounding system. Look for a bonding clamp and conductor.",
                componentType = "Bonding Connection",
                inspectionNote = "Metal water pipe enters the building without a bonding connection to the electrical grounding system."
            },
            new ViolationData
            {
                violationId = "GND-EGC-SIZE-001",
                description = "Equipment grounding conductor undersized for circuit",
                necArticle = "250.122",
                necArticleText = "Equipment grounding conductors of the wire type shall not be smaller than shown in Table 250.122.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "EGC_SubPanel",
                hintText = "Equipment grounding conductors must be sized per Table 250.122 based on the overcurrent device rating.",
                componentType = "Conductor",
                inspectionNote = "Equipment grounding conductor is 14 AWG on a 30A circuit. Table 250.122 requires minimum 10 AWG for 30A."
            },

            // === STANDARD (4 additional violations) ===
            new ViolationData
            {
                violationId = "GND-ELECTRODE-SYS-001",
                description = "Multiple grounding electrodes present but not bonded together",
                necArticle = "250.50",
                necArticleText = "All grounding electrodes as described in 250.52(A)(1) through (A)(7) that are present at each building or structure served shall be bonded together to form the grounding electrode system.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Electrode_BondingJumper",
                hintText = "All grounding electrodes at a building must be bonded together into one system.",
                componentType = "Bonding Connection",
                inspectionNote = "Ground rod and water pipe electrode are both present but not bonded together. They must form a single grounding electrode system."
            },
            new ViolationData
            {
                violationId = "GND-SYSTEM-001",
                description = "Electrical system grounding does not limit voltage from lightning or surges",
                necArticle = "250.4(A)(1)",
                necArticleText = "Electrical systems that are grounded shall be connected to earth in a manner that will limit the voltage imposed by lightning, line surges, or unintentional contact with higher-voltage lines and that will stabilize the voltage to earth during normal operation.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "ServiceGround_Connection",
                hintText = "The system ground must provide a low-impedance path to earth for lightning and surge protection.",
                componentType = "System Ground",
                inspectionNote = "Service grounding connection uses a corroded clamp with high resistance, failing to provide adequate surge protection."
            },
            new ViolationData
            {
                violationId = "GND-GEC-ALUM-001",
                description = "Bare aluminum grounding electrode conductor in contact with earth",
                necArticle = "250.64(A)",
                necArticleText = "Bare aluminum or copper-clad aluminum grounding electrode conductors shall not be used where in direct contact with masonry or the earth or where subject to corrosive conditions.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "GEC_AluminumRun",
                hintText = "Aluminum grounding electrode conductors cannot contact masonry or earth directly.",
                componentType = "Conductor",
                inspectionNote = "Bare aluminum grounding electrode conductor is run through earth to the ground rod. Aluminum corrodes in contact with soil."
            },
            new ViolationData
            {
                violationId = "GND-WATERPIPE-001",
                description = "Water pipe electrode not meeting 10-foot earth contact requirement",
                necArticle = "250.52(A)(1)",
                necArticleText = "A metal underground water pipe in direct contact with the earth for 10 feet or more, including any metal well casing that is bonded to the pipe and that is in direct contact with the earth.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "WaterPipe_Underground",
                hintText = "To qualify as a grounding electrode, a metal water pipe must have at least 10 feet of earth contact.",
                componentType = "Grounding Electrode",
                inspectionNote = "Metal water pipe transitions to plastic 4 feet after entering the building. Less than 10 feet of metal is in contact with earth, so it does not qualify as a grounding electrode on its own."
            },

            // === EXPERT (2 subtle violations) ===
            new ViolationData
            {
                violationId = "GND-INTERSYSTEM-001",
                description = "Missing intersystem bonding termination for communications grounding",
                necArticle = "250.94",
                necArticleText = "An intersystem bonding termination shall be provided external to enclosures at the service equipment or metering equipment enclosure and at the disconnecting means for any additional buildings or structures.",
                severity = ViolationSeverity.Minor,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "ServicePanel_IntersystemBond",
                hintText = "An intersystem bonding termination is required for communications (cable, phone, antenna) grounding.",
                componentType = "Bonding Termination",
                inspectionNote = "No intersystem bonding termination is provided at the service equipment for communications system grounding connections."
            },
            new ViolationData
            {
                violationId = "GND-SUPPLEMENT-001",
                description = "Single ground rod used as sole electrode without supplemental electrode",
                necArticle = "250.53(A)(2)",
                necArticleText = "A single rod, pipe, or plate electrode shall be supplemented by an additional electrode. The supplemental electrode shall be bonded to one of the following: the rod, pipe, or plate electrode, the grounding electrode conductor, the grounded service-entrance conductor, or the service equipment enclosure.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "GroundRod_Supplemental",
                hintText = "A single ground rod must be supplemented by an additional electrode unless it can achieve 25 ohms or less.",
                componentType = "Ground Rod",
                inspectionNote = "Only one ground rod is installed with no supplemental electrode. Unless resistance is verified at 25 ohms or less, a second rod is required."
            }
        };

        [MenuItem("NEC Inspector/Generate Grounding Scenario")]
        public static void Generate()
        {
            const string VIOLATION_DIR = "Assets/_Project/ScriptableObjects/Violations/Grounding";
            const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations/Grounding"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations"))
                    AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Violations");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects/Violations", "Grounding");
            }
            if (!AssetDatabase.IsValidFolder(SCENARIO_DIR))
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Scenarios");

            AssetDatabase.StartAssetEditing();
            try
            {
                var violationAssets = new ViolationDefinitionSO[_violations.Length];
                for (int i = 0; i < _violations.Length; i++)
                {
                    var data = _violations[i];
                    string assetPath = $"{VIOLATION_DIR}/VD_GND_{data.violationId}.asset";

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

                string scenarioPath = $"{SCENARIO_DIR}/ScenarioDefinition_Grounding.asset";
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(scenarioPath);
                if (scenario == null)
                {
                    scenario = ScriptableObject.CreateInstance<ScenarioDefinitionSO>();
                    AssetDatabase.CreateAsset(scenario, scenarioPath);
                }

                scenario.sceneName = "GroundingInspection";
                scenario.displayName = "Grounding & Bonding Inspection";
                scenario.description = "Inspect a residential service entrance and grounding electrode system for code violations. Verify proper grounding electrode installation, bonding connections, conductor sizing, and system grounding integrity.";
                scenario.availableDifficulties = new DifficultyLevel[]
                {
                    DifficultyLevel.Beginner,
                    DifficultyLevel.Standard,
                    DifficultyLevel.Expert
                };
                scenario.violations = violationAssets;
                scenario.necChapters = new string[] { "2" };
                scenario.expertTimeLimit = 1200;
                scenario.environmentDescription = "A residential utility room and exterior service entrance showing the grounding electrode system, service panel, water pipe bonding, and ground rods.";

                EditorUtility.SetDirty(scenario);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Grounding scenario generated: {_violations.Length} violations + 1 scenario definition.");
        }
    }
}
