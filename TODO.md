# NEC Code Inspector - Development TODO

## Current Phase: Step 1 - Repository & Project Setup

### In Progress
- [x] Create GitHub repository
- [x] README.md
- [x] .claude/CLAUDE.md
- [x] .gitignore
- [x] .gitattributes
- [x] docs/NEC_COVERAGE.md
- [x] docs/ASSET_SOURCES.md
- [ ] Unity project folder structure
- [ ] Copy reusable scripts from career-explorer
- [ ] Initial commit & push to GitHub

### Step 2: Core Systems (Week 1-2)
- [ ] GameManager singleton
- [ ] NECDatabase singleton + JSON schema
- [ ] DifficultyManager (Beginner/Standard/Expert)
- [ ] ProgressManager (JSON persistence)
- [ ] ScenarioCatalog + ScenarioDefinition ScriptableObjects
- [ ] ViolationDefinition ScriptableObjects
- [ ] 50 core NEC articles in JSON database
- [ ] Boot scene with SDK detection
- [ ] Main menu scene (placeholder)

### Step 3: Inspection Scenario #1 - Residential Panel (Week 3-4)
- [ ] InspectionScenarioRunner (StateMachine)
- [ ] IntroStep, FreeInspectionStep, ReviewStep, NECReviewStep, ScoreStep
- [ ] Residential panel 3D scene with embedded violations
- [ ] Component hover/highlight/inspect interaction
- [ ] Violation flagging UI (PopupCanvas)
- [ ] NEC citation input (dropdown/search/free-text per difficulty)
- [ ] Virtual flashlight tool
- [ ] Scoring system
- [ ] Difficulty differentiation (hints, scaffolding, time limits)

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
