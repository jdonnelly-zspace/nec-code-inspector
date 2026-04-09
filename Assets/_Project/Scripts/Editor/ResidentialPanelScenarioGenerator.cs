using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class ResidentialPanelScenarioGenerator
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
            // === BEGINNER (3 violations) ===
            new ViolationData
            {
                violationId = "RP-WORK-PANEL-001",
                description = "Unsecured conductors in panel with poor workmanship",
                necArticle = "110.12",
                necArticleText = "Electrical equipment shall be installed in a neat and workmanlike manner.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Panel_LooseConductors",
                hintText = "Look at how the wires are arranged inside the panel. Are they neat and properly secured?",
                componentType = "Panel Wiring",
                inspectionNote = "Conductors inside the panel are loose and crossing over each other. NEC requires neat and workmanlike installation."
            },
            new ViolationData
            {
                violationId = "RP-CONN-LUG-001",
                description = "Aluminum service entrance conductor terminated on copper-only rated lug",
                necArticle = "110.14",
                necArticleText = "Connection of conductors to terminal parts shall ensure a thoroughly good connection without damaging the conductors. Connectors and terminals shall be identified for the conductor material.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Panel_MainLug_Aluminum",
                hintText = "Check the main lugs. Is the conductor material compatible with the lug rating? Look for AL/CU markings.",
                componentType = "Termination",
                inspectionNote = "Aluminum conductor is terminated on a lug rated for copper only. This can cause overheating due to dissimilar metal expansion rates."
            },
            new ViolationData
            {
                violationId = "RP-CLEAR-FRONT-001",
                description = "Working space in front of panel is less than 36 inches deep",
                necArticle = "110.26(A)(1)",
                necArticleText = "The depth of the working space in the direction of live parts shall not be less than that specified in Table 110.26(A)(1). For 0-150V nominal, the minimum clear distance is 3 feet.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Beginner,
                isSubtle = false,
                componentObjectName = "Panel_WorkingSpace_Obstruction",
                hintText = "Measure the clear space in front of the panel. Is there at least 36 inches of unobstructed working space?",
                componentType = "Working Space",
                inspectionNote = "Storage boxes are stacked within 24 inches of the panel face. NEC requires a minimum 36-inch clear working space."
            },

            // === STANDARD (3 additional violations) ===
            new ViolationData
            {
                violationId = "RP-DISC-LOC-001",
                description = "Service disconnect located in an area not readily accessible",
                necArticle = "230.70(A)",
                necArticleText = "The service disconnecting means shall be installed at a readily accessible location either outside of a building or structure or inside nearest the point of entrance of the service conductors.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "ServiceDisconnect_Inaccessible",
                hintText = "The service disconnect must be readily accessible. Can you reach it without moving obstacles or using a ladder?",
                componentType = "Service Disconnect",
                inspectionNote = "Service disconnect is located behind stored equipment and requires moving obstacles to access. Must be readily accessible per NEC."
            },
            new ViolationData
            {
                violationId = "RP-WIRE-OVER-001",
                description = "12 AWG conductor protected by a 30-ampere overcurrent device",
                necArticle = "240.4",
                necArticleText = "Conductors, other than flexible cords, shall be protected against overcurrent in accordance with their ampacity.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Panel_Breaker30A_12AWG",
                hintText = "Verify that each conductor's ampacity matches or exceeds its breaker rating. 12 AWG is rated for 20A max.",
                componentType = "Overcurrent Protection",
                inspectionNote = "A 12 AWG conductor (rated 20A) is connected to a 30A breaker. The conductor could overheat before the breaker trips."
            },
            new ViolationData
            {
                violationId = "RP-PANEL-HEIGHT-001",
                description = "Breaker operating handle center is mounted above 6 feet 7 inches",
                necArticle = "240.24(A)",
                necArticleText = "Overcurrent devices shall be readily accessible and installed so that the center of the grip of the operating handle of the switch or circuit breaker, when in its highest position, is not more than 2.0 m (6 ft 7 in.) above the floor or working platform.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Standard,
                isSubtle = false,
                componentObjectName = "Panel_TopBreaker_High",
                hintText = "Check the height of the topmost breaker handle. The center of the grip must not exceed 6 feet 7 inches.",
                componentType = "Panel Mounting",
                inspectionNote = "The top breaker's operating handle center is at approximately 7 feet. This exceeds the NEC maximum of 6 ft 7 in."
            },

            // === EXPERT (2 subtle violations) ===
            new ViolationData
            {
                violationId = "RP-DIR-MISS-001",
                description = "Panel circuit directory is missing or has inaccurate circuit descriptions",
                necArticle = "408.4",
                necArticleText = "Every circuit and circuit modification shall be legibly identified as to its clear, evident, and specific purpose or use. The identification shall include an approved degree of detail sufficient to allow each circuit to be distinguished from all others.",
                severity = ViolationSeverity.Major,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "Panel_CircuitDirectory",
                hintText = "Examine the circuit directory inside the panel door. Is every circuit clearly and accurately labeled?",
                componentType = "Panel Directory",
                inspectionNote = "Several circuits are labeled generically as 'spare' or 'misc' while actively serving loads. The 2026 NEC requires specific, clear identification."
            },
            new ViolationData
            {
                violationId = "RP-BUS-EXCEED-001",
                description = "More circuit breakers installed than the panel's rated number of spaces",
                necArticle = "408.36",
                necArticleText = "Panelboards shall be provided with physical means to prevent the installation of more overcurrent devices than that number for which the panelboard was designed, rated, and listed.",
                severity = ViolationSeverity.Critical,
                minimumDifficulty = DifficultyLevel.Expert,
                isSubtle = true,
                componentObjectName = "Panel_OverfilledBus",
                hintText = "Count the breakers and compare with the panel's rated space count. Are tandem breakers used in non-approved locations?",
                componentType = "Panel Bus",
                inspectionNote = "Tandem breakers are installed in slots not designed for them, exceeding the panel's rated number of circuits. This overloads the bus bars."
            }
        };

        [MenuItem("NEC Inspector/Generate Residential Panel Scenario")]
        public static void Generate()
        {
            const string VIOLATION_DIR = "Assets/_Project/ScriptableObjects/Violations/ResidentialPanel";
            const string SCENARIO_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

            // Ensure directories exist
            if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations/ResidentialPanel"))
            {
                if (!AssetDatabase.IsValidFolder("Assets/_Project/ScriptableObjects/Violations"))
                    AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Violations");
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects/Violations", "ResidentialPanel");
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
                    string assetPath = $"{VIOLATION_DIR}/VD_RP_{data.violationId}.asset";

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
                string scenarioPath = $"{SCENARIO_DIR}/ScenarioDefinition_ResidentialPanel.asset";
                var scenario = AssetDatabase.LoadAssetAtPath<ScenarioDefinitionSO>(scenarioPath);
                if (scenario == null)
                {
                    scenario = ScriptableObject.CreateInstance<ScenarioDefinitionSO>();
                    AssetDatabase.CreateAsset(scenario, scenarioPath);
                }

                scenario.id = "scenario-residential-panel";
                scenario.sceneName = "ResidentialPanelInspection";
                scenario.displayName = "Residential Service Panel Inspection";
                scenario.description = "Inspect a 200A residential service entrance panel for code violations including workmanship, terminations, working space clearances, overcurrent protection, and panel configuration.";
                scenario.availableDifficulties = new DifficultyLevel[]
                {
                    DifficultyLevel.Beginner,
                    DifficultyLevel.Standard,
                    DifficultyLevel.Expert
                };
                scenario.violations = violationAssets;
                scenario.necChapters = new string[] { "1", "2", "4" };
                scenario.expertTimeLimit = 1200;
                scenario.environmentDescription = "A residential garage with a 200A service entrance panel, exposed conductors, breakers, main disconnect, and surrounding working space.";

                EditorUtility.SetDirty(scenario);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Residential Panel scenario generated: {_violations.Length} violations + 1 scenario definition.");
        }
    }
}
