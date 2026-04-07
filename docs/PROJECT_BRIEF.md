# NEC Code Inspector — Project Brief

## Executive Summary

NEC Code Inspector is an AR/VR educational application for the zSpace Inspire 2 platform that trains students on the National Electrical Code (NEC/NFPA 70, 2026 Edition). Students learn to identify code violations through hands-on 3D inspection scenarios, design electrical panels, and demonstrate NEC proficiency through assessments with certificate tracking.

**Platform:** zSpace Inspire 2 (Windows 11, Unity 6.3 LTS)
**Target Users:** CTE high school students, trade school apprentices, licensed electricians
**Differentiator:** Only AR/VR NEC training tool with 3D stereoscopic inspection, three difficulty tiers, and direct NEC article citation practice

---

## Product Overview

### Three Core Modes

**1. Inspection Scenarios**
Students inspect 3D electrical installations to identify code violations. They flag problems, cite the specific NEC article, and receive scored feedback. Three difficulty levels control hint availability, citation method (dropdown vs. free-text), time pressure, and violation subtlety.

*Scenarios built:*
- Scenario 1: Residential Service Panel (8 articles, MVP)
- Scenario 2: Branch Circuit Wiring — GFCI, AFCI, spacing, wire gauge (12 violations)
- Scenario 3: Grounding & Bonding — electrodes, bonding, conductor sizing (10 violations)
- Scenario 4: Commercial Installation (planned)
- Scenario 5: Outdoor/Wet Location (planned)

**2. Panel Design Sandbox**
Students design a complete residential electrical panel: select breakers, assign circuits, route wires, and verify NEC compliance. A 10-rule compliance checker validates the design against code, and load calculations follow Art. 220 standard method.

**3. NEC Reference & Assessment**
Searchable NEC article database (66 articles), quick reference cards (10 topics), progress dashboard with score history, chapter mastery tracking, and certificate generation.

### Difficulty Tiers

| Feature | Beginner (CTE) | Standard (Apprentice) | Expert (Licensed) |
|---------|:-:|:-:|:-:|
| NEC Citation | Dropdown | Searchable | Free text |
| Hints | Yes + scaffolding | No | No |
| Time Limit | No | No | Yes (20 min) |
| Subtle Violations | No | No | Yes |
| False Positive Penalty | No | No | Yes |

---

## Technical Architecture

- **Engine:** Unity 6.3 LTS + zCore 6.3
- **Target:** Windows 11 on zSpace Inspire 2
- **Stereo:** All UI in World Space (required for zSpace 3D)
- **Input:** 6DOF stylus (primary) + mouse fallback
- **Data:** NEC articles in JSON (StreamingAssets), progress in JSON (persistentDataPath)
- **Scripts:** 73 C# scripts across 10 modules
- **Performance:** 90fps minimum for stereo rendering

### Module Map

| Module | Scripts | Purpose |
|--------|:-------:|---------|
| Core | 8 | GameManager, Difficulty, Progress, Audio, Settings, Boot, Transitions |
| Data | 5 | ScenarioDefinition, ViolationDefinition, PanelDesignDefinition, QuickReferenceCard, CertificateTemplate |
| Inspection | 8 | Manager, Runner, 5 Step classes, Pointer Controller |
| PanelSandbox | 11 | Manager, Runner, HUD, BreakerSlot, PlacedBreaker, WireConnection, LoadCalculator, ComplianceChecker, data classes |
| UI | 7 | InspectionHUD, ViolationFlaggingPanel, ReviewPanel, NECReferencePanel, QuickReferenceCardPanel, ProgressDashboard, MainMenu |
| Inputs | 7 | Clickable, Grabbable, DropTarget, SnappingDraggablePlane, DraggablePlane, HoverTooltip (from career-explorer) |
| Tools | 4 | VirtualTool, Flashlight, Multimeter, ToolBelt |
| NEC | 2 | NECDatabase, NECArticle |
| Utils | 8 | StateMachine, GlowEffect, HighlightObject, RendererHighlight, PopupCanvas, ObjectPointer, FaceMainCamera, ObjectHoverText |
| Editor | 5 | Generator scripts for scenarios, sandbox, quick reference cards |

---

## Timeline

| Phase | Duration | Status | Deliverables |
|-------|:--------:|:------:|-------------|
| Step 1: Setup | Week 1 | Done | Repo, project structure, reusable scripts |
| Step 2: Core | Week 1 | Done | GameManager, NEC database, difficulty, progress |
| Step 3: Inspection | Week 2 | Done | 5-step inspection flow, tools, HUD |
| Step 4: Branch Circuits | Week 3 | Done (scripts) | 12 violations, generator, 10 new NEC articles |
| Step 5: Panel Sandbox | Week 4 | Done (scripts) | Breaker system, load calc, compliance checker |
| Step 6: Reference | Week 5 | Done (scripts) | Quick ref cards, certificates, dashboard |
| Step 7: Integration | Week 6 | Done (scripts) | Boot, transitions, audio, main menu, settings |
| **Unity Assembly** | **Weeks 7-8** | **Next** | **Scenes, prefabs, 3D assets, wiring** |
| **Alpha** | **Weeks 9-10** | Planned | Scenarios 3-5, 200+ articles, tutorial |
| **Beta** | **Weeks 11-16** | Planned | Full content, analytics, exam prep |
| **Release** | **Weeks 17-20** | Planned | Polish, accessibility, licensing alignment |

### Current State
All script architecture is complete. The project needs:
1. Unity Editor scene assembly (placing 3D objects, wiring UI prefabs)
2. 3D environment assets (electrical panels, rooms, components)
3. Audio assets (SFX, ambient)
4. zSpace hardware testing

---

## AI Integration Opportunity

### World Labs Marble (3D Environment Generation)
Generate photorealistic electrical environments from text/photos instead of manual 3D modeling. Estimated cost: **~$250 total** for all environments through beta. See `docs/WORLD_LABS_INTEGRATION.md` for full analysis.

### Additional AI Opportunities
- **NEC Q&A chatbot** via Claude API for contextual code questions
- **Adaptive difficulty** based on student performance patterns
- **Procedural violation placement** for unique sessions
- **Photo-to-scenario** for teacher-created content

---

## Value Proposition

### For Students
- Learn NEC through hands-on 3D inspection, not textbook reading
- Practice citing specific code articles — the skill tested on licensing exams
- Three difficulty levels grow with the student from CTE through journeyman
- Immediate scored feedback with NEC article review for missed violations

### For Educators
- Aligns with NEC 2026 edition (latest code cycle)
- Progress tracking and certificate generation for grading
- Difficulty tiers serve mixed classrooms (CTE through licensed)
- Scenario-based assessment mirrors real inspection workflows

### For CTE Programs / Trade Schools
- Differentiated offering: only AR/VR NEC training on the market
- Runs on existing zSpace Inspire 2 hardware (no new purchases)
- Covers electrical program curriculum requirements
- Certificate tracking for program accreditation documentation

---

## Content Coverage

### NEC Articles: 66 implemented (targeting 200+ for beta)
- Chapter 1: General Requirements (Art. 110)
- Chapter 2: Wiring, Branch Circuits, Services, Grounding (Art. 210, 220, 230, 240, 250)
- Chapter 3: Wiring Methods (Art. 310, 334)
- Chapter 4: Equipment (Art. 406, 408, 410, 422, 430)
- Chapter 6: Special Equipment (Art. 680)

### Violations: 34 defined across 3 scenarios
- 12 Branch Circuit violations (GFCI, AFCI, spacing, wire gauge, dedicated circuits)
- 10 Grounding & Bonding violations (electrodes, GEC, bonding, EGC sizing)
- 12 Panel Sandbox compliance rules (breaker match, load balance, panel spaces)

### Quick Reference Cards: 10 topics
GFCI, AFCI, Wire Sizing, Branch Circuits, Receptacle Spacing, Load Calculation, Grounding, Panel Design, NM Cable, 2026 Changes
