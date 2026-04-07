using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.PanelSandbox;

namespace NECInspector.Editor
{
    public static class PanelDesignSandboxGenerator
    {
        [MenuItem("NEC Inspector/Generate Panel Sandbox Data")]
        public static void Generate()
        {
            const string ASSET_DIR = "Assets/_Project/ScriptableObjects/Scenarios";

            if (!AssetDatabase.IsValidFolder(ASSET_DIR))
            {
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "Scenarios");
            }

            string assetPath = $"{ASSET_DIR}/PanelDesign_Residential200A.asset";
            var definition = AssetDatabase.LoadAssetAtPath<PanelDesignDefinitionSO>(assetPath);
            if (definition == null)
            {
                definition = ScriptableObject.CreateInstance<PanelDesignDefinitionSO>();
                AssetDatabase.CreateAsset(definition, assetPath);
            }

            definition.panelType = "Residential 200A";
            definition.displayName = "Residential Panel Design - 200A Service";
            definition.description = "Design a complete residential electrical panel for a 2,000 sq ft single-family dwelling. Select and place the correct breakers for all required circuits, route wiring with proper gauge conductors, and verify NEC compliance.";
            definition.totalAmps = 200;
            definition.totalSlots = 40;
            definition.dwellingSquareFootage = 2000f;
            definition.loadCalcTolerancePercent = 10f;
            definition.expertTimeLimit = 1800;
            definition.necChapters = new[] { "2", "3" };
            definition.availableDifficulties = new[]
            {
                DifficultyLevel.Beginner,
                DifficultyLevel.Standard,
                DifficultyLevel.Expert
            };

            // Calculate target load for scoring reference
            // 2000 sqft * 3 VA = 6000 lighting + 3000 SA + 1500 laundry = 10500 base
            // Demand: 3000 + (7500 * 0.35) = 5625 + dryer 5000 + range 8000 = 18625 VA
            definition.targetLoadVA = 18625f;

            definition.requiredCircuits = new RequiredCircuit[]
            {
                new RequiredCircuit
                {
                    circuitName = "Kitchen Small Appliance 1",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = true,
                    necReference = "210.11(C)(3)",
                    isRequired = true,
                    description = "First of two required 20A small-appliance branch circuits for kitchen countertop receptacles."
                },
                new RequiredCircuit
                {
                    circuitName = "Kitchen Small Appliance 2",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = true,
                    necReference = "210.11(C)(3)",
                    isRequired = true,
                    description = "Second of two required 20A small-appliance branch circuits for kitchen countertop receptacles."
                },
                new RequiredCircuit
                {
                    circuitName = "Bathroom Receptacles",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = false,
                    necReference = "210.11(C)(1)",
                    isRequired = true,
                    description = "Dedicated 20A branch circuit for bathroom receptacle outlets."
                },
                new RequiredCircuit
                {
                    circuitName = "Laundry",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = true,
                    necReference = "210.11(C)(2)",
                    isRequired = true,
                    description = "Dedicated 20A branch circuit for laundry room receptacle."
                },
                new RequiredCircuit
                {
                    circuitName = "Dishwasher",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = false,
                    necReference = "210.8(D)",
                    isRequired = true,
                    description = "Dedicated circuit for dishwasher with GFCI protection (2026 NEC)."
                },
                new RequiredCircuit
                {
                    circuitName = "Garbage Disposal",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = false,
                    necReference = "210.8(A)(5)",
                    isRequired = true,
                    description = "Dedicated circuit for garbage disposal under kitchen sink."
                },
                new RequiredCircuit
                {
                    circuitName = "Electric Range",
                    ampsRequired = 50,
                    wireGauge = "6 AWG",
                    poleCount = 2,
                    requiresGFCI = false,
                    requiresAFCI = false,
                    necReference = "220.55",
                    isRequired = true,
                    description = "240V circuit for electric range/oven. 50A with 6 AWG conductors."
                },
                new RequiredCircuit
                {
                    circuitName = "Clothes Dryer",
                    ampsRequired = 30,
                    wireGauge = "10 AWG",
                    poleCount = 2,
                    requiresGFCI = false,
                    requiresAFCI = false,
                    necReference = "220.54",
                    isRequired = true,
                    description = "240V circuit for electric clothes dryer. 30A with 10 AWG conductors."
                },
                new RequiredCircuit
                {
                    circuitName = "Air Conditioning",
                    ampsRequired = 30,
                    wireGauge = "10 AWG",
                    poleCount = 2,
                    requiresGFCI = false,
                    requiresAFCI = false,
                    necReference = "440.4",
                    isRequired = true,
                    description = "240V circuit for central air conditioning condensing unit."
                },
                new RequiredCircuit
                {
                    circuitName = "General Lighting",
                    ampsRequired = 15,
                    wireGauge = "14 AWG",
                    poleCount = 1,
                    requiresGFCI = false,
                    requiresAFCI = true,
                    necReference = "220.12",
                    isRequired = true,
                    description = "General lighting circuit for living areas. AFCI required for dwelling unit."
                },
                new RequiredCircuit
                {
                    circuitName = "General Receptacles",
                    ampsRequired = 15,
                    wireGauge = "14 AWG",
                    poleCount = 1,
                    requiresGFCI = false,
                    requiresAFCI = true,
                    necReference = "210.52(A)",
                    isRequired = true,
                    description = "General receptacle circuit for bedrooms and living areas. AFCI required."
                },
                new RequiredCircuit
                {
                    circuitName = "Garage Receptacles",
                    ampsRequired = 20,
                    wireGauge = "12 AWG",
                    poleCount = 1,
                    requiresGFCI = true,
                    requiresAFCI = false,
                    necReference = "210.8(A)(2)",
                    isRequired = true,
                    description = "Garage receptacle circuit with GFCI protection."
                }
            };

            EditorUtility.SetDirty(definition);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Panel sandbox data generated: {definition.requiredCircuits.Length} required circuits for {definition.panelType}.");
        }
    }
}
