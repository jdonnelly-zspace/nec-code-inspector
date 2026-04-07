# NEC Code Inspector - Development TODO

## Current Phase: Step 6 - NEC Reference & Assessment (scripts complete, awaiting Unity Editor)

### Completed
- [x] Create GitHub repository (https://github.com/jdonnelly-zspace/nec-code-inspector)
- [x] README.md, CLAUDE.md, .gitignore, .gitattributes
- [x] docs/NEC_COVERAGE.md, docs/ASSET_SOURCES.md
- [x] Unity project folder structure
- [x] Copy reusable scripts from career-explorer (Inputs, Utils, Data patterns)
- [x] Initial commit & push to GitHub

### Step 2: Core Systems - COMPLETED
- [x] GameManager singleton (scene flow, difficulty, progress)
- [x] NECDatabase singleton + JSON loader with full-text search
- [x] DifficultyManager (Beginner/Standard/Expert)
- [x] DifficultySettingsSO (citation modes, hints, scaffolding, time limits)
- [x] ProgressManager (JSON persistence to Application.persistentDataPath)
- [x] InspectionScore + SandboxScore data models with grading
- [x] ScenarioCatalogSO + ScenarioDefinitionSO ScriptableObjects
- [x] ViolationDefinitionSO (NEC reference, severity, scene binding, hints)
- [x] NECArticle + NECArticleCollection data models
- [x] 50 core NEC articles in JSON database (Ch 1-4, 6)
- [ ] Boot scene with SDK detection (deferred to Step 3)
- [ ] Main menu scene (deferred to Step 7)

### Step 3: Inspection Scenario System - COMPLETED (scripts)
- [x] InspectionStateMachine (clean version without career-explorer dependencies)
- [x] InspectionScenarioRunner (builds step sequence, starts machine)
- [x] IntroStep (scenario intro with continue prompt)
- [x] FreeInspectionStep (main gameplay loop: inspect, flag, time limit, scaffolding)
- [x] ReviewStep (found/missed/false positives summary)
- [x] NECReviewStep (walk through missed violations with NEC articles)
- [x] ScoreStep (letter grade, retry/menu options, progress save)
- [x] InspectableComponent (hover highlight, flag, mark compliant, hint pulse)
- [x] InspectionManager (tracks violations, calculates scores, filters by difficulty)
- [x] InspectionPointerController (raycast, hover, inspect, tool use bridge)
- [x] ViolationFlaggingPanel (NEC citation: dropdown/search/free-text per difficulty)
- [x] InspectionHUD (intro panel, inspection panel, timer, flagged count)
- [x] InspectionReviewPanel (summary, missed violations, final score)
- [x] NECReferencePanel (search, article display, related articles, 2026 badge)
- [x] VirtualTool base class + Flashlight (spotlight) + Multimeter (V/A/Ω/continuity)
- [x] MeasurementPoint (simulated electrical readings on components)
- [x] ToolBelt (tool switching manager)
- [ ] Residential panel 3D scene with embedded violations (requires Unity editor)
- [ ] Scene prefab setup and wiring (requires Unity editor)

### Step 4: Inspection Scenario #2 - Branch Circuits (Week 3-4)
- [x] 10 new NEC articles added to database (210.8(A)(3), 210.8(A)(9), 210.8(D), 210.12(B), 210.52(C)(5), 210.52(D), 210.52(E)(1), 210.52(G), 240.4(B), 334.80)
- [x] 12 violation definitions (5 Beginner, 5 Standard, 2 Expert/subtle)
- [x] BranchCircuitScenarioGenerator editor script (menu: NEC Inspector > Generate Branch Circuit Scenario)
- [x] GFCI violations: bathroom, kitchen, garage, dishwasher (2026)
- [x] AFCI violations: bedroom, living room
- [x] Receptacle spacing: wall (6ft rule), countertop (24in rule)
- [x] Wire gauge: 14 AWG on 20A breaker, NM cable bundling derate
- [x] Dedicated circuits: bathroom, small appliance (2 required)
- [x] NEC_COVERAGE.md updated with full violation-to-article mapping
- [ ] Kitchen/bathroom/living area 3D scene (requires Unity Editor)
- [ ] Run generator to create ScriptableObject assets (requires Unity Editor)
- [ ] Add scenario to ScenarioCatalog (requires Unity Editor)
- [ ] Scene prefab setup and wiring (requires Unity Editor)

### Step 5: Panel Design Sandbox (Week 5-6) - COMPLETED (scripts)
- [x] PanelDesignDefinitionSO + RequiredCircuit + BreakerData data classes
- [x] BreakerSlot snap system (integrates with DropTarget)
- [x] PlacedBreaker (drag behavior via SnappingDraggablePlane)
- [x] WireConnection (LineRenderer routing with gauge validation)
- [x] LoadCalculator (Art. 220: lighting 3VA/sqft, SA 1500VA, demand factors Table 220.42)
- [x] ComplianceChecker (10 NEC rules: breaker/conductor match, GFCI, AFCI, load balance, etc.)
- [x] PanelDesignManager (state tracking, scoring, compliance integration)
- [x] PanelDesignRunner (5-step state machine: Briefing → Placement → Wiring → Compliance → Score)
- [x] PanelDesignHUD (world-space UI: briefing, design, wiring, compliance, score panels)
- [x] PanelDesignSandboxGenerator editor script (menu: NEC Inspector > Generate Panel Sandbox Data)
- [x] 5 new NEC articles for load calculations (Art. 220.42, 220.52, 220.54, 220.55, 220.83)
- [x] Residential 200A panel definition with 12 required circuits
- [x] NEC_COVERAGE.md updated with full compliance rule + circuit tables
- [ ] Unity scene setup with 3D panel, breaker tray, slot GameObjects (requires Unity Editor)
- [ ] Run generator to create PanelDesignDefinitionSO asset (requires Unity Editor)
- [ ] Prefab wiring and playtest (requires Unity Editor)

### Step 6: NEC Reference & Assessment (Week 5-6) - COMPLETED (scripts)
- [x] NECReferencePanel (searchable world-space UI) — completed in Step 3
- [x] QuickReferenceCardSO data model (10 categories, difficulty-filtered)
- [x] QuickReferenceCardPanel (searchable, filterable by category, links to NEC articles)
- [x] QuickReferenceCardGenerator editor script (10 cards: GFCI, AFCI, wire sizing, spacing, load calc, grounding, panel design, NM cable, 2026 changes)
- [x] CertificateTemplateSO (4 types: chapter, scenario mastery, sandbox, overall)
- [x] CertificateGenerator (evaluates progress, awards certificates, token-based description formatting)
- [x] Chapter mastery tracking added to ProgressManager (threshold-based, best-attempt tracking)
- [x] ProgressDashboardPanel (scenario scores, sandbox scores, mastery, certificates)
- [x] EarnedCertificate persistence in ProgressData
- [ ] Certificate UI panel with visual design (requires Unity Editor)
- [ ] Wire up Quick Reference Cards to inspection HUD (requires Unity Editor)
- [ ] Run card/certificate generators (requires Unity Editor)

### Step 7: Integration & Polish (Week 6)
- [ ] MainMenu scene with mode selection
- [ ] Scene transitions
- [ ] Audio (SFX, ambient)
- [ ] Performance testing
- [ ] Windows build from Mac

## Future Milestones

### Alpha (Weeks 7-10)
- Inspection scenarios #3-5
- Virtual multimeter and clamp meter
- Tutorial system
- 200+ NEC articles
- Wire-a-House mode begins

### Beta (Weeks 11-16)
- Wire-a-House complete
- Full NEC coverage for journeyman exam
- Exam prep mode
- All 5 virtual tools
- Analytics dashboard

### Full Release (Weeks 17-20)
- Master electrician exam prep
- NEC 2026 changes highlight mode
- All creative features
- State licensing alignment
- Accessibility pass
