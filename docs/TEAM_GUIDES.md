# NEC Code Inspector — Team Reference Guide

This document maps each team to the documentation and artifacts most relevant to their role.

---

## Product

**Key docs:** `PROJECT_BRIEF.md`, `TODO.md`, `NEC_COVERAGE.md`

- **What it is:** AR/VR NEC code training app for zSpace Inspire 2 with inspection scenarios, panel design sandbox, and assessment/certification
- **Roadmap:** TODO.md tracks phases 1-7 (scripts complete) through alpha/beta/release
- **Content scope:** 66 NEC articles, 34 violations, 10 quick reference cards, 3 difficulty tiers
- **AI opportunity:** World Labs Marble for 3D environments (~$250 budget), Claude API for NEC Q&A chatbot. See `WORLD_LABS_INTEGRATION.md`
- **Decision needed:** Prioritize Unity scene assembly (weeks 7-8) vs. AI environment generation pipeline

---

## Design

**Key docs:** `PROJECT_BRIEF.md`, `WORLD_LABS_INTEGRATION.md`, `.claude/CLAUDE.md`

- **Constraints:** ALL UI must be World Space canvases (no Screen Space). TextMeshPro only. 90fps stereo target.
- **Input:** 6DOF stylus is primary. All interactions must work with pointer + 3 buttons.
- **UI panels scripted:** InspectionHUD, ViolationFlaggingPanel, ReviewPanel, NECReferencePanel, QuickReferenceCardPanel, ProgressDashboard, MainMenuPanel, PanelDesignHUD — all need visual design and prefab layout
- **3D assets needed:** Electrical panels, breakers, receptacles, conductors, junction boxes, ground rods, wire runs, tools (flashlight, multimeter), 5 room environments
- **AI environments:** World Labs can generate photorealistic rooms from reference photos. Design team reviews/approves generated environments for brand consistency.
- **Color coding:** Cyan = hover highlight, Amber = flagged violation, Green = marked compliant

---

## Engineering

**Key docs:** `.claude/CLAUDE.md`, `TODO.md`, source code in `Assets/_Project/Scripts/`

- **Architecture:** GameManager is only persistent singleton. Scenarios use InspectionStateMachine. Content defined in ScriptableObjects. NEC data in StreamingAssets JSON.
- **73 scripts** across: Core (8), Data (5), Inspection (8), PanelSandbox (11), UI (7), Inputs (7), Tools (4), NEC (2), Utils (8), Editor (5)
- **Immediate work:** Run 5 editor generator scripts, create Unity scenes, wire prefabs to serialized fields, build 3D scene hierarchies matching componentObjectName values in ViolationDefinitionSO assets
- **Performance:** 90fps min, <200 draw calls, 5K particle cap, object pooling for spawned objects
- **zSpace:** ZCamera replaces Unity Camera. Never reference zSpace types outside Scripts/Inputs/. Input abstraction handles ZStylus + ZMouse automatically.
- **AI integration:** Marble API → GLB mesh + SPZ splat → UnityGaussianSplatting plugin → overlay InspectableComponent GameObjects

---

## QA

**Key docs:** `PROJECT_BRIEF.md`, `NEC_COVERAGE.md`, `TODO.md`

- **Test matrix:** 3 difficulty levels x 5 scenarios x inspection flow (5 steps each) + panel sandbox (5 steps) + reference/dashboard
- **NEC accuracy:** 66 articles must match NEC 2026 text. Violation NEC references must map to correct articles. Citation matching: exact + partial ("210" matches "210.8(A)(1)")
- **Scoring validation:** Letter grades (A=90%+, B=80%+, C=70%+, D=60%+, F=<60%). Combined accuracy = (detection + citation) / 2. Sandbox: compliance rate + load accuracy.
- **Difficulty filtering:** Beginner sees only Beginner violations (no subtle). Standard adds Standard. Expert adds Expert + subtle. Verify violation counts per difficulty.
- **Panel sandbox rules:** 10 compliance rules in ComplianceChecker.cs. Verify each rule catches its target condition.
- **LoadCalculator:** 3 VA/sqft, 1500 VA per SA circuit, Table 220.42 demand factors. Verify against manual NEC calculation for 2000 sqft dwelling.
- **Hardware testing:** Weekly on zSpace Inspire 2 — stylus interactions, stereo rendering, 90fps target, World Space UI readability
- **Progress persistence:** nec_progress.json in Application.persistentDataPath. Verify save/load across sessions. Certificate earning conditions.

---

## Support

**Key docs:** `PROJECT_BRIEF.md`

- **User types:** CTE students (Beginner), trade school apprentices (Standard), licensed electricians (Expert)
- **Common workflows:** Select difficulty → Pick scenario → Inspect → Flag violations with NEC citation → Review missed → See score → View certificates
- **Data locations:** Progress saves to `%APPDATA%/../LocalLow/[CompanyName]/NECCodeInspector/nec_progress.json`. Settings in `nec_settings.json` same directory.
- **Known limitations:** Requires zSpace Inspire 2 hardware. Mouse fallback works in editor only. 3D scenes require Unity-assembled environments (not yet built).
- **Troubleshooting:** If NEC database fails to load, check StreamingAssets/NECDatabase/nec_articles.json exists. If progress resets, check persistentDataPath file permissions.

---

## Customer Success

**Key docs:** `PROJECT_BRIEF.md`, `NEC_COVERAGE.md`

- **Onboarding path:** Start at Beginner difficulty → Complete Scenario 1 (Service Panel) → Progress to Scenario 2 (Branch Circuits) → Try Panel Sandbox → Advance to Standard/Expert
- **Success metrics:** Scenario completion rate, accuracy improvement over attempts, certificate earning rate, time-to-mastery per chapter
- **Progress tracking:** ProgressDashboardPanel shows scores, attempts, mastered chapters, earned certificates. JSON-based — can be exported for LMS integration.
- **Content depth:** 66 NEC articles covering the most-tested topics on journeyman/master electrician licensing exams
- **Differentiation:** Only product combining AR/VR inspection training + NEC citation practice + adaptive difficulty on zSpace

---

## Sales

**Key docs:** `PROJECT_BRIEF.md`

- **Elevator pitch:** "NEC Code Inspector turns NEC code training from textbook memorization into hands-on 3D inspection practice. Students inspect photorealistic electrical installations on zSpace, identify violations, and cite the exact NEC article — the same skill tested on licensing exams."
- **Target buyers:** CTE program directors, trade school instructors, electrical apprenticeship coordinators, corporate training managers
- **Competitive advantage:** Only AR/VR NEC training tool. Runs on existing zSpace Inspire 2 hardware. Three difficulty tiers serve CTE through licensed electrician. NEC 2026 edition (latest code cycle).
- **Content scope:** 5 inspection scenarios, panel design sandbox, 66+ NEC articles, 10 quick reference cards, certificate generation
- **Timeline:** Scripts complete. Unity assembly in progress. Alpha targeting weeks 9-10. Beta weeks 11-16.

---

## GTM / Marketing

**Key docs:** `PROJECT_BRIEF.md`, `WORLD_LABS_INTEGRATION.md`

- **Positioning:** "The first AR/VR training tool for the National Electrical Code"
- **Key messages:**
  1. Learn by doing — inspect 3D electrical installations, not read textbooks
  2. Practice the exact skill tested on licensing exams: identifying violations and citing NEC articles
  3. Three difficulty levels serve classrooms from CTE students to licensed electricians
  4. Built on NEC 2026 (latest edition) — always current
  5. AI-generated photorealistic environments (World Labs) for authentic training scenarios
- **Demo talking points:** Show a Beginner student finding a missing GFCI in a bathroom → flag it → select NEC 210.8(A)(1) from dropdown → see scored feedback. Then show Expert mode with free-text citation and time pressure.
- **Content marketing angles:**
  - "Why NEC 2026 changes matter for training programs"
  - "From textbook to 3D: how AR/VR improves code retention"
  - "The electrician shortage and how CTE programs are using technology to train faster"
  - "AI-generated training environments: how World Labs powers realistic inspection scenarios"
- **Launch timeline:** Alpha demo available weeks 9-10. Beta with full content weeks 11-16. GA weeks 17-20.
