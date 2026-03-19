# NEC Code Inspector - Development TODO

## Current Phase: Step 1 - Repository & Project Setup

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
- [ ] Kitchen/bathroom 3D scene
- [ ] GFCI/AFCI violation types
- [ ] Receptacle spacing violations
- [ ] Wire gauge violations

### Step 5: Panel Design Sandbox (Week 5-6)
- [ ] PanelBuilder with BreakerSlot snap system
- [ ] Breaker placement (drag from tray, snap to slots)
- [ ] Wire routing (Grabbable endpoints + LineRenderer)
- [ ] Load calculation tool (Art. 220)
- [ ] ComplianceChecker rule engine (10 core rules)
- [ ] Sandbox scoring

### Step 6: NEC Reference & Assessment (Week 5-6)
- [ ] Quick Reference Panel (searchable world-space UI)
- [ ] Violation context display
- [ ] Quick-reference cards
- [ ] Assessment scoring
- [ ] Progress tracking (JSON persistence)
- [ ] Certificate generation

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
