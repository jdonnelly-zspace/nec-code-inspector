# NEC Article Coverage Map

## Inspection Scenarios

### Scenario 1: Residential Service Panel (MVP)
| NEC Article | Topic | Violation Types |
|-------------|-------|-----------------|
| Art. 110.12 | Mechanical execution of work | Poor workmanship, unsecured conductors |
| Art. 110.14 | Electrical connections | Improper terminations, mixed metals |
| Art. 110.26 | Working space | Insufficient clearance, blocked access |
| Art. 230.70 | Service disconnect | Location, marking, accessibility |
| Art. 240.4 | Protection of conductors | Oversized breaker for conductor |
| Art. 240.24 | Overcurrent device location | Improper location, accessibility |
| Art. 408.4 | Panel directory | Missing/inaccurate circuit directory |
| Art. 408.36 | Overcurrent protection | Bus rating exceeded |

### Scenario 2: Branch Circuit Wiring (MVP) - 12 Violations
| NEC Article | Topic | Violation ID | Difficulty | Severity |
|-------------|-------|-------------|:----------:|:--------:|
| Art. 210.8(A)(1) | GFCI - Bathrooms | BC-GFCI-BATH-001 | Beginner | Critical |
| Art. 210.8(A)(5) | GFCI - Kitchens | BC-GFCI-KITCHEN-001 | Beginner | Critical |
| Art. 210.8(A)(2) | GFCI - Garages | BC-GFCI-GARAGE-001 | Beginner | Major |
| Art. 210.52(A) | Receptacle spacing - General | BC-SPACING-WALL-001 | Beginner | Major |
| Art. 240.4(D) | Small conductors (14 AWG on 20A) | BC-WIRE-14AWG-001 | Beginner | Critical |
| Art. 210.12(A) | AFCI - Bedrooms | BC-AFCI-BEDROOM-001 | Standard | Critical |
| Art. 210.12(A) | AFCI - Living rooms | BC-AFCI-LIVING-001 | Standard | Critical |
| Art. 210.52(C) | Receptacle spacing - Countertops | BC-SPACING-COUNTER-001 | Standard | Major |
| Art. 210.11(C)(1) | Dedicated bathroom circuit | BC-DEDICATED-BATH-001 | Standard | Major |
| Art. 210.11(C)(3) | Two small-appliance circuits | BC-DEDICATED-KITCHEN-001 | Standard | Major |
| Art. 334.80 | NM cable bundling derate | BC-WIRE-KITCHEN-001 | Expert | Major |
| Art. 210.8(D) | GFCI - Dishwashers (2026) | BC-GFCI-DISHWASHER-001 | Expert | Major |

**Difficulty distribution:** Beginner sees 5, Standard sees 10, Expert sees all 12.
**Generator script:** `Assets/_Project/Scripts/Editor/BranchCircuitScenarioGenerator.cs`

### Scenario 3: Grounding & Bonding (Alpha)
| NEC Article | Topic |
|-------------|-------|
| Art. 250.4 | General requirements |
| Art. 250.24 | Grounding electrode conductor |
| Art. 250.50 | Grounding electrode system |
| Art. 250.52 | Grounding electrodes |
| Art. 250.64 | Grounding electrode conductor installation |
| Art. 250.104 | Bonding of piping systems |
| Art. 250.122 | Equipment grounding conductor size |

### Scenario 4: Commercial Installation (Alpha)
| NEC Article | Topic |
|-------------|-------|
| Art. 220.40-44 | Optional/demand calculations |
| Art. 230 | Services |
| Art. 240 | Overcurrent protection |
| Art. 430 | Motors |

### Scenario 5: Outdoor/Wet Location (Alpha)
| NEC Article | Topic |
|-------------|-------|
| Art. 406.9 | Receptacles in damp/wet locations |
| Art. 410.10 | Luminaires in specific locations |
| Art. 680 | Swimming pools, spas, hot tubs |

## Panel Design Sandbox - 10 Compliance Rules

**ComplianceChecker:** `Assets/_Project/Scripts/PanelSandbox/ComplianceChecker.cs`
**Generator:** `Assets/_Project/Scripts/Editor/PanelDesignSandboxGenerator.cs` (menu: NEC Inspector > Generate Panel Sandbox Data)

| Rule ID | Rule Name | NEC Reference | Description |
|---------|-----------|:-------------:|-------------|
| RULE-01 | Breaker/Conductor Match | Art. 240.4 | Breaker amps must not exceed wire ampacity |
| RULE-02 | Required Branch Circuits | Art. 210.11 | All required circuits present and properly sized |
| RULE-03 | GFCI Protection | Art. 210.8 | GFCI breakers where required (kitchen, bath, garage, laundry, dishwasher) |
| RULE-04 | AFCI Protection | Art. 210.12 | AFCI breakers where required (bedrooms, living areas, kitchen) |
| RULE-05 | Load Balance | General Practice | Bus sides within 20% load imbalance |
| RULE-06 | Main Breaker Sizing | Art. 230.79 | Main breaker ≥ calculated load in amps |
| RULE-07 | No Double-Tapped Breakers | Art. 110.14 | One circuit per breaker terminal |
| RULE-08 | Conductor Ampacity | Art. 310.14 | Wire gauge matches breaker rating per Table 310.16 |
| RULE-09 | Panel Spaces | Art. 408.36 | Total breaker poles ≤ panel slot count |
| RULE-10 | Wire Connections | General Practice | All placed breakers must have wire connections |

### Load Calculation (Art. 220)
**LoadCalculator:** `Assets/_Project/Scripts/PanelSandbox/LoadCalculator.cs`

| Calculation | NEC Reference | Value |
|-------------|:-------------:|-------|
| General lighting | Art. 220.12 | 3 VA/sq ft |
| Small appliance circuits | Art. 220.52 | 1,500 VA each (min 2 required) |
| Laundry circuit | Art. 220.52 | 1,500 VA |
| Demand factor | Table 220.42 | First 3,000 VA at 100%, 3,001-120,000 at 35% |
| Dryer load | Art. 220.54 | 5,000 VA minimum |
| Range demand | Table 220.55 | 8,000 VA (single, ≤12 kW) |

### Residential 200A Panel — Required Circuits (12)
| Circuit | Amps | Wire | Poles | GFCI | AFCI | NEC Ref |
|---------|:----:|:----:|:-----:|:----:|:----:|:-------:|
| Kitchen Small Appliance 1 | 20A | 12 AWG | 1 | Yes | Yes | 210.11(C)(3) |
| Kitchen Small Appliance 2 | 20A | 12 AWG | 1 | Yes | Yes | 210.11(C)(3) |
| Bathroom Receptacles | 20A | 12 AWG | 1 | Yes | No | 210.11(C)(1) |
| Laundry | 20A | 12 AWG | 1 | Yes | Yes | 210.11(C)(2) |
| Dishwasher | 20A | 12 AWG | 1 | Yes | No | 210.8(D) |
| Garbage Disposal | 20A | 12 AWG | 1 | Yes | No | 210.8(A)(5) |
| Electric Range | 50A | 6 AWG | 2 | No | No | 220.55 |
| Clothes Dryer | 30A | 10 AWG | 2 | No | No | 220.54 |
| Air Conditioning | 30A | 10 AWG | 2 | No | No | 440.4 |
| General Lighting | 15A | 14 AWG | 1 | No | Yes | 220.12 |
| General Receptacles | 15A | 14 AWG | 1 | No | Yes | 210.52(A) |
| Garage Receptacles | 20A | 12 AWG | 1 | Yes | No | 210.8(A)(2) |
