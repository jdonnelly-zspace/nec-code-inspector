using UnityEngine;
using UnityEditor;
using NECInspector.Core;
using NECInspector.Data;

namespace NECInspector.Editor
{
    public static class QuickReferenceCardGenerator
    {
        private struct CardData
        {
            public string cardId;
            public string title;
            public CardCategory category;
            public string summary;
            public string keyRule;
            public string[] necReferences;
            public string[] keywords;
            public DifficultyLevel minimumDifficulty;
        }

        private static readonly CardData[] _cards = new CardData[]
        {
            new CardData
            {
                cardId = "QRC-GFCI-001",
                title = "GFCI Protection Requirements",
                category = CardCategory.GFCIProtection,
                summary = "Ground-Fault Circuit Interrupter (GFCI) protection is required for receptacles in bathrooms, kitchens (countertop), garages, outdoors, laundry areas, and near bathtubs/showers. The 2026 NEC also requires GFCI for dishwasher circuits.",
                keyRule = "All 125V-250V receptacles in wet/damp locations must have GFCI protection.",
                necReferences = new[] { "210.8(A)", "210.8(A)(1)", "210.8(A)(2)", "210.8(A)(5)", "210.8(D)" },
                keywords = new[] { "GFCI", "ground fault", "bathroom", "kitchen", "garage", "outdoor", "wet" },
                minimumDifficulty = DifficultyLevel.Beginner
            },
            new CardData
            {
                cardId = "QRC-AFCI-001",
                title = "AFCI Protection Requirements",
                category = CardCategory.AFCIProtection,
                summary = "Arc-Fault Circuit Interrupter (AFCI) protection is required for 120V, 15A and 20A branch circuits in virtually all dwelling unit living spaces: bedrooms, living rooms, kitchens, dining rooms, hallways, closets, laundry areas, and similar rooms.",
                keyRule = "AFCI protection is required in all habitable rooms of dwelling units.",
                necReferences = new[] { "210.12(A)", "210.12(B)" },
                keywords = new[] { "AFCI", "arc fault", "bedroom", "living room", "fire prevention" },
                minimumDifficulty = DifficultyLevel.Beginner
            },
            new CardData
            {
                cardId = "QRC-WIRE-001",
                title = "Wire Gauge & Breaker Sizing",
                category = CardCategory.WireSizing,
                summary = "Wire gauge must match the breaker amperage: 14 AWG max 15A, 12 AWG max 20A, 10 AWG max 30A, 8 AWG max 40A, 6 AWG max 55A. Never put a smaller wire on a larger breaker.",
                keyRule = "14 AWG = 15A, 12 AWG = 20A, 10 AWG = 30A, 8 AWG = 40A, 6 AWG = 55A",
                necReferences = new[] { "240.4(D)", "310.14", "310.16" },
                keywords = new[] { "wire gauge", "AWG", "ampacity", "breaker", "conductor", "overcurrent" },
                minimumDifficulty = DifficultyLevel.Beginner
            },
            new CardData
            {
                cardId = "QRC-BRANCH-001",
                title = "Required Branch Circuits",
                category = CardCategory.BranchCircuits,
                summary = "Dwelling units require dedicated 20A circuits for: bathroom receptacles, laundry receptacles, and at least two small-appliance circuits for kitchen countertops. These circuits cannot serve other rooms.",
                keyRule = "Bathroom, laundry, and kitchen small-appliance circuits must be dedicated 20A.",
                necReferences = new[] { "210.11(C)(1)", "210.11(C)(2)", "210.11(C)(3)" },
                keywords = new[] { "branch circuit", "dedicated", "bathroom", "laundry", "kitchen", "small appliance" },
                minimumDifficulty = DifficultyLevel.Beginner
            },
            new CardData
            {
                cardId = "QRC-SPACING-001",
                title = "Receptacle Spacing Rules",
                category = CardCategory.BranchCircuits,
                summary = "Wall receptacles: no point more than 6 feet from an outlet (max 12 feet between outlets). Countertop receptacles: no point more than 24 inches from an outlet. Islands and peninsulas 24\"x12\" or larger need at least one outlet.",
                keyRule = "Walls: 6ft rule (12ft max spacing). Countertops: 24-inch rule.",
                necReferences = new[] { "210.52(A)", "210.52(C)", "210.52(C)(5)" },
                keywords = new[] { "receptacle", "spacing", "6 feet", "24 inches", "countertop", "wall" },
                minimumDifficulty = DifficultyLevel.Beginner
            },
            new CardData
            {
                cardId = "QRC-LOAD-001",
                title = "Residential Load Calculation",
                category = CardCategory.LoadCalculation,
                summary = "Standard method: General lighting at 3 VA/sq ft + small-appliance circuits at 1,500 VA each + laundry at 1,500 VA. Apply Table 220.42 demand factors: first 3,000 VA at 100%, remainder at 35%. Add dryer (5,000 VA) and range (8,000 VA demand) at 100%.",
                keyRule = "3 VA/sq ft lighting + 1,500 VA per SA circuit + demand factors from Table 220.42",
                necReferences = new[] { "220.12", "220.42", "220.52", "220.54", "220.55" },
                keywords = new[] { "load calculation", "demand factor", "VA", "service size", "dwelling" },
                minimumDifficulty = DifficultyLevel.Standard
            },
            new CardData
            {
                cardId = "QRC-GROUND-001",
                title = "Grounding Electrode System",
                category = CardCategory.Grounding,
                summary = "All grounding electrodes present at a building must be bonded together. Common electrodes: metal underground water pipe (10ft+ contact), ground rods (8ft, 5/8\" diameter), concrete-encased electrode (Ufer ground). The grounding electrode conductor connects the system to earth.",
                keyRule = "All present electrodes must be bonded together to form the grounding electrode system.",
                necReferences = new[] { "250.50", "250.52(A)(1)", "250.52(A)(5)", "250.24(A)(1)" },
                keywords = new[] { "grounding", "electrode", "bonding", "ground rod", "water pipe", "earth" },
                minimumDifficulty = DifficultyLevel.Standard
            },
            new CardData
            {
                cardId = "QRC-PANEL-001",
                title = "Panel Design Basics",
                category = CardCategory.PanelDesign,
                summary = "Panelboards must not exceed their bus rating. All circuits must be identified in a directory. Working space: 30\" wide, 36\" deep, 6.5' high minimum. Balance loads between bus sides. Main breaker must handle the calculated service load.",
                keyRule = "Panel directory required. Do not exceed bus rating. Maintain working clearance.",
                necReferences = new[] { "408.4", "408.36", "110.26(A)", "230.79" },
                keywords = new[] { "panel", "directory", "bus rating", "working space", "clearance", "breaker" },
                minimumDifficulty = DifficultyLevel.Standard
            },
            new CardData
            {
                cardId = "QRC-NM-001",
                title = "NM Cable Installation",
                category = CardCategory.General,
                summary = "NM (Romex) cable must be secured within 12 inches of every box and at intervals not exceeding 4.5 feet. When more than 2 NM cables pass through the same hole in framing, ampacity must be derated per Table 310.15(C)(1).",
                keyRule = "Secure within 12\" of boxes, every 4.5 ft. Derate when bundling 3+ cables.",
                necReferences = new[] { "334.30", "334.80", "310.14" },
                keywords = new[] { "NM cable", "Romex", "securing", "staple", "bundling", "derate" },
                minimumDifficulty = DifficultyLevel.Standard
            },
            new CardData
            {
                cardId = "QRC-2026-001",
                title = "Key 2026 NEC Changes",
                category = CardCategory.General,
                summary = "Major 2026 changes: GFCI now required for dishwashers (210.8(D)). GFCI voltage range expanded to 125V-250V (210.8(A)). AFCI list updated. Enhanced circuit directory requirements (408.4). Updated panel labeling requirements.",
                keyRule = "Dishwasher GFCI, expanded voltage range for GFCI, enhanced circuit directories.",
                necReferences = new[] { "210.8(A)", "210.8(D)", "210.12(A)", "408.4" },
                keywords = new[] { "2026", "new", "changes", "update", "dishwasher", "GFCI", "directory" },
                minimumDifficulty = DifficultyLevel.Expert
            }
        };

        [MenuItem("NEC Inspector/Generate Quick Reference Cards")]
        public static void Generate()
        {
            const string CARD_DIR = "Assets/_Project/ScriptableObjects/QuickReferenceCards";

            if (!AssetDatabase.IsValidFolder(CARD_DIR))
            {
                AssetDatabase.CreateFolder("Assets/_Project/ScriptableObjects", "QuickReferenceCards");
            }

            AssetDatabase.StartAssetEditing();
            try
            {
                foreach (var data in _cards)
                {
                    string assetPath = $"{CARD_DIR}/{data.cardId}.asset";
                    var card = AssetDatabase.LoadAssetAtPath<QuickReferenceCardSO>(assetPath);
                    if (card == null)
                    {
                        card = ScriptableObject.CreateInstance<QuickReferenceCardSO>();
                        AssetDatabase.CreateAsset(card, assetPath);
                    }

                    card.cardId = data.cardId;
                    card.title = data.title;
                    card.category = data.category;
                    card.summary = data.summary;
                    card.keyRule = data.keyRule;
                    card.necReferences = data.necReferences;
                    card.keywords = data.keywords;
                    card.minimumDifficulty = data.minimumDifficulty;

                    EditorUtility.SetDirty(card);
                }
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[NEC Inspector] Generated {_cards.Length} quick reference cards.");
        }
    }
}
