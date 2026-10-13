# Product requirements

Status: draft, brought up to date on 2026-10-12 (last audit against the repository, the wireframe and the mockup). The owner has decided the occupation-driven direction, the skills, the three phases, utility line work, both NEC editions as profiles, hidden rule numbers, teacher reporting, analytics, accessibility and localization (`docs/product/DECISIONS.md`, D-1 to D-11); the rest is still a draft for review. The wireframe and mockup (`docs/design/`) match this document. Supersedes `docs/PROJECT_BRIEF.md`, which was written around the NEC alone and names audiences and credentials.
Sources: the market view the owner supplied ("NEC Code Inspector, zSpace Inspire 2, Full Market View", September 2026), the occupation research in
`docs/occupations/`, and the project's own rules (`.claude/CLAUDE.md`, `docs/CONTENT_POLICY.md`). Market figures are the owner's and have not been verified
(see section 8).

## 1. What the product is

A training application for the zSpace Inspire 2 stylus display that teaches the **procedures of electrical occupations**. A student chooses the occupation they
are working toward, and the app teaches what that occupation actually does and how it does it safely: the standard procedures (SOPs), the skills behind them and the
rules that govern them. The installation code (the NEC, and the codes of other regions) is a **reference** the procedures cite, not the definition of the product.
This is a change from the first design, which was an NEC code-violation trainer; the NEC work already built becomes one part of it.

## 2. Principles

1. **Occupation driven.** Content exists because an occupation does it. Every skill, scenario and lesson traces to a procedure of at least one occupation, and every procedure of a
   supported occupation is either taught or listed as a gap.
2. **Skills, not credentials.** The app teaches and measures skills. It contains no credential names, exam blueprints or readiness scores. Alignment to credentials is worked
   out outside the app (`docs/credential-alignment/`). An occupation is a pathway the student chooses, not a credential. (This keeps the existing rule; the market view's
   "exam prep" positioning is a sales choice that must not leak into the product.)
3. **Procedures are faithful and sourced.** Each SOP is written in our own words from public sources, with the standards that govern it, and checked before it ships.
4. **The code is a reference.** The region the student picks selects the installation code and units behind the scenes; the code is never named to the student.
5. **Honest about coverage.** The app records, per occupation and procedure, what it teaches and what it does not (`docs/occupations/TRACEABILITY.md`), and does not claim an occupation it cannot teach.
6. **Stylus first, in 3D.** Every interaction works with the stylus and three buttons; UI is world-space (zSpace stereo rendering requires it).
7. **Nothing ships unverified.** New content goes through draft, separate verification against public sources, and a credential expert, in that order.

## 3. The model

```
Occupation  ->  Procedure (SOP)  ->  Skill  ->  Content
 pathway        what the trade does    what the student    lessons, scenarios, trainers,
 the student    and how it is made     can do              sandbox designs, reference entries
 chooses        safe (steps, hazards,   (20 today,          (data, tested)
                standards)              3 more planned)
                         \                    /
                    Code profile (region): NEC, CEC, BS 7671, and later others
```

- A **procedure** has an ordered set of steps, hazards, controls and the standards that govern it. Procedures are documented in `docs/occupations/` and are the unit the
  app is checked against.
- A **skill** is a unit of ability the app can teach and measure (`ConceptIds`): 13 code skills and 7 procedure skills today, with 3 more planned for phases 2 and 3. Procedures use one or more skills; skills are shared across occupations.
- **Content** teaches a skill: lessons, inspection scenarios, procedure trainers, sandbox designs, quick reference cards.
- Progress is recorded per skill and tier, and each procedure practice is recorded with its score; a derived per-procedure view is planned.
- **Editions are backend.** Each edition of a code is its own profile (`nec-2023`, `nec-2026`); the student never sees an edition, a rule number or a chapter, only the skill, the violation and the rule's title.

## 4. Users

### 4.1 Core occupations

The six occupations in the market view. "Fit today" is the share of the occupation's documented procedures that the app teaches at least in part, from the researchers'
coverage ratings (a judgement, not a test). After phase 1 "in part" mostly means a lesson and step-by-step practice of the procedure; hands-on trainers and the 3D scenes are still to build.

| O*NET code | Occupation | Workers (BLS or O*NET) | Procedures documented | Fit today | Verdict |
|---|---|---|---|---|---|
| 47-2111 | Electricians | 821,000 (2025) | 7 | 100% (was 57%) | Core, phase 1. Every procedure has a lesson and step practice; the 3D scenes and hands-on trainers (a virtual meter) remain. |
| 47-3013 | Helpers, electricians | 65,000 (2025) | 8 | 100% (was 38%) | Core, phase 1. The safety and basic skills it lacked are now lessons with practice. |
| 47-4011 | Construction and building inspectors | 153,900 (2025) | 7 | 100% (was 71%) | Best fit: the inspection scenarios are its work. Stages and code administration are now lessons; a staged inspection scene remains. |
| 47-2231 | Solar photovoltaic installers | 31,100 (2025) | 7 | 71% (was 57%) | Phase 3. Needs a PV and storage skill (new content in NEC Articles 690, 705, 706), which depends on the 2026 numbering being verified. |
| 49-2098 | Security and fire alarm systems installers | 86,300 (2025) | 7 | 43% | Phase 3. Needs low-voltage and fire alarm content and a second code (the fire alarm standard). |
| 49-9051 | Electrical power-line installers and repairers | 131,900 (2025) | 8 | 38% (was 12%) | **Phase 2, blocked.** Governed mainly by the NESC and OSHA line-work rules, not the NEC. Decided: build it (a NESC profile and utility scenes), after the licence decision. |

### 4.2 Adjacent occupations

Eighteen more occupations in four clusters (`docs/occupations/ADJACENT.md`). The research found nine where beginner electrical literacy fits, five that need new skills, and six that are not
NEC occupations. General maintenance workers are the strongest fit among them.

### 4.3 Learner levels

Learner levels follow the trade's own ladder, **Apprentice, Journeyman and Master** (owner's decision, 2026-10-13, D-12; renamed from Beginner, Standard and Expert in the app, the content, the tests, the wireframe and the mockup). They are described by what the student can already do and name stages of skill, not licence classes: the app awards no credential and none is implied.

### 4.4 Buyers

CTE programs, apprenticeship and trade-school programs, community colleges, and inspector continuing-education programs (the market view's three tiers: high-school CTE, apprentice and trade school, continuing education and inspector programs). Teachers need reporting; see FR-15.

## 5. Functional requirements

Status: **Built** (in the repository and tested), **Partial** (data or code exists, not wired or not run in Unity), **Planned**, **Blocked** (needs a decision or something outside the project).

| ID | Requirement | Status | Where |
|---|---|---|---|
| FR-1 | The student chooses an occupation; the app shows the procedures of that occupation and which are taught. | Partial | `occupations.json`, `OccupationLibrary`, `OccupationPickerPanel`; the panel is not in a scene yet. |
| FR-2 | Each procedure is available as an SOP: purpose, steps, hazards, controls, standards. | Partial | 44 procedures load from `occupations.json` (purpose, steps, hazards, controls; standards stay in the research documents because students never see them); draft, no expert review. |
| FR-3 | Skills with tiers (Foundation, Practitioner, Authority); code skills and procedure skills. | Partial | 20 built (13 code, 7 procedure skills of phase 1; `ConceptIds`); 3 skills of phases 2 and 3 not built. |
| FR-4 | Inspection scenarios: point at a part of a 3D building, flag a violation, name the rule, get scored feedback. | Partial | 6 scenarios, 139 NEC violations (under both NEC editions), 14 CEC, 9 BS 7671 as data; no 3D scenes built. |
| FR-5 | Sessions draw a different set of violations each time, spread across skills. | Built | `ViolationDraw`, `docs/POOL_AND_DRAW.md`. |
| FR-6 | Procedure trainers: guided practice of a procedure's steps (lockout and test-before-touch, troubleshooting, a staged inspection), including a virtual meter and tester. | Partial | Step-ordering practice built for 28 of 44 procedures (`ProcedurePractice`, `ProcedurePracticePanel`); the virtual meter and scene-based trainers are planned. |
| FR-7 | Panel design sandbox: design, wire and check a residential panel. | Partial | 6 designs, US only (needs tables); each is tested to be completable (required circuits fit the panel, the main breaker covers them, the load can be balanced); scene not built. |
| FR-8 | Skill lessons before practice, in the student's region's words. | Partial | 20 lessons and a panel; not wired into a scene. |
| FR-9 | Tutorial that teaches the stylus and a first inspection. | Partial | 8 steps as data with a panel (`tutorial.json`, `TutorialPanel`); needs a scene and `Report` calls from the inspection scene; the stylus button mapping needs checking on the device. |
| FR-10 | Rule lookup: search rule entries by name and open quick reference cards. | Partial | NEC 161 entries (in each edition's profile), CEC 12, BS 7671 29; 17 NEC cards, one for every skill; entries show a title, never a number, chapter or edition; UI built, not run in Unity. |
| FR-11 | Region choice sets the code, units and wording; the code and its edition are never named or shown. | Partial | Built and tested; the picker is not wired into a scene. |
| FR-12 | Three generic difficulty levels. | Built | Text and tests. |
| FR-13 | Progress per skill and tier, saved with the student; derived progress per procedure. | Partial | Per skill built; each procedure practice is saved with its score (`ProcedureProgress`); a derived per-procedure view is planned. |
| FR-14 | A content pipeline: scaffold, tests, source verification, expert worksheets. | Built | `tools/`, `docs/AUTHORING_VIOLATIONS.md`, expert review packet. |
| FR-15 | Teacher reporting: class progress by skill and procedure, without teacher accounts in the first release. The app exports a per-student progress file (CSV, by skill and tier) a teacher collects by USB or shared folder; `tools/alignment-report` can read it. | Planned | Decided 2026-10-12 (D-8). The export is not built. |
| FR-16 | Code profiles for each region and edition, and for occupations that need another code (fire alarm standard). | Partial | `nec-2023` (default), `nec-2026` (draft), CEC (draft), BS 7671 (draft); a fire alarm profile planned; the NESC blocked. |
| FR-17 | Edition handling: each NEC edition is its own profile; the student never sees an edition or a rule number. | Built | `nec-2023` and `nec-2026` (draft); decided 2026-10-12 (D-5 to D-7). |
| FR-18 | Occupation fit is visible to the owner: a generated traceability matrix and gap list. | Built | `docs/occupations/TRACEABILITY.md`, `tools/occupations/build_docs.py`. |
| FR-19 | Analytics: usage and learning events are sent to the same analytics service the other zSpace Unity apps use, keyed by a generic id, with no personal data in any event. | Planned | Decided 2026-10-12 (D-9). Nothing is built; the service is to be confirmed. |
| FR-20 | Progress can be followed outside the app, and the student can see plainly what is tracked: a generic account or id (no name or email needed), a screen saying what is collected, where it goes, who sees it and how to export or delete it. | Planned | Decided 2026-10-12 (D-9); wording needs counsel. |
| FR-21 | Rule numbers, chapters and editions are never shown to students; the UI shows an entry's title. Numbers stay in the data for scoring, alignment and review. | Built | `CodeTerminology.showReferenceNumbers` (off); student text rewritten without numbers; `StudentReferenceTests`. At the Master level the student names the rule in words, matched to its title and keywords (`RuleNameMatcher`). |
| FR-22 | Accessibility floors: never colour alone, minimum text size and contrast, grade-8 reading level, timers that can be turned off, a stereo depth setting, seated stylus-only use. | Planned | Decided 2026-10-12 (D-10). Settings and checks not built; the mockup shows the controls. |
| FR-23 | Localization: English first, other priority-one languages next, translating only reviewed content. | Planned | Decided 2026-10-12 (D-11). Student text is already data; no language switch is built. |

## 6. Non-functional requirements

- **Performance:** 90 fps minimum on zSpace Inspire 2 (stereo), under 200 draw calls, particle systems under 5,000.
- **Input and display:** stylus primary, mouse only as an editor fallback; all UI canvases world-space; ZCamera, never a normal camera; TextMeshPro for all text.
- **Platform:** Windows 11, Unity 6.3 LTS, zCore 6.3, URP.
- **Content accuracy:** every citation, number and procedure step is checked by a second pass against public sources, then an expert, before it is called reviewed. Statuses are visible in the worksheets.
- **Content licensing:** installation codes and standards are copyrighted; content is paraphrase plus reference numbers. The licence decision is the owner's (open).
- **Testing:** logic tests run in CI, a Unity compile check runs locally, and the zSpace hardware is tested weekly.
- **Analytics and identity (decided 2026-10-12, D-9):** the app sends analytics like the other zSpace Unity apps, behind an interface so no code depends on the vendor. A generic id (a class or student code, no personal data) ties events and the progress export to a student. Events carry ids, skill, tier, score and time, never free text, a name or an email. A plain-language screen shows what is collected, where it goes, who sees it, and how to export or delete it. Minors are students here: the notice wording, consent (a school consent route) and retention need counsel before release.
- **Teacher reporting (decided 2026-10-12, D-8):** no teacher accounts in the first release; a per-student progress export (FR-15).
- **Accessibility (decided 2026-10-12, D-10):** never rely on colour alone (wire colours always carry a label); a minimum text size and 4.5:1 contrast for all UI text; student text at about grade 8; every timer can be turned off as an accommodation; a stereo depth setting; seated, stylus-only use with no long holds; read-aloud later. Stereo comfort and low-vision use need testing on the device.
- **Localization (decided 2026-10-12, D-11):** English first. The other priority-one languages follow, in an order and list the owner sets; student text is already data, so it can be extracted. French (Canada) is the likely first, once the CEC reaches parity (phase 3). Translate only reviewed content.

## 7. Scope by occupation, in phases (accepted 2026-10-12; order changed 2026-10-13)

A note on the word: a **release phase** (this section) is not a **content phase** (`docs/CONTENT_PLAN.md`, phases 0 to 5). Where both could be meant, the content plan's are called content phases.

The market view plans the six core occupations for a first-half-2027 launch. The research does not support that for all six:

| Phase | Occupations | What has to exist first |
|---|---|---|
| 1 | Electricians, electrician helpers, inspectors | The seven new skills are built as lessons and step practice (electrically safe work, testing and measurement, troubleshooting, plans and documentation, jobsite safety, code administration, utility interface). Still needed: the 3D scenes, a staged inspection, hands-on trainers, the remaining phase 1 content and expert review. |
| 2 | Power-line installers and repairers; adjacent clusters | Utility line work, decided 2026-10-12 and moved ahead of solar and alarm on 2026-10-13 (D-13): a NESC code profile and new utility scenes, after the NESC licence decision and phase 1; then beginner-literacy packages for maintenance, HVAC and construction roles. |
| 3 | Solar PV installers, security and fire alarm installers | A PV and storage skill, and a low-voltage and fire alarm skill with a second code profile and its licence decision; the PV content needs the 2026 numbering verified (the edition decision itself is settled: both editions are profiles). |

## 8. Market, as stated and as checked

The market view claims: a primary US education TAM of $26 million (6,400 programs at $250 to $375 per device, 15 devices a program), an adjacent TAM of $34.6 million, and a combined $60.6 million
across 24 occupations. The occupation research checked what it could against O*NET and BLS:

- Worker and opening totals do not reconcile (checked on the live O*NET and BLS pages, 2026-10-13): counting each code once, the 23 distinct occupations have about 7.3 million workers, not 6.7 million, and about 616,000 annual openings, not 700,000; about 78 percent of the adjacent workers are general maintenance workers, laborers, construction managers and supervisors.
- "All Bright Outlook" is false for five of the 18 (sheet metal workers, helpers of plumbers, energy engineers, energy auditors, telecom installers), confirmed on each O*NET page.
- The document says 24 occupations but lists 6 core plus 18 adjacent with one repeated (49-2098 appears in both groups); codes and titles for 49-9021, 49-2095 and one 49-9071 entry are wrong or mislabelled.
- Three adjacent occupations that look electrical (powerhouse and substation repairers, transportation equipment installers, energy engineers) are not NEC occupations, and elevator work is governed by a different standard.
- The pricing, penetration and revenue figures are the owner's business assumptions; none was checked. The document itself notes that the NFPA licence cost is not modelled.

Details: `docs/product/MARKET_FACTS.md` (every figure with its page, date and status) and `docs/occupations/ADJACENT.md`. The market view should be corrected before it is used outside the team. The market sizes, program and device counts and prices are still the owner's unverified assumptions.

## 9. Success measures

Product measures (to be set by the owner): share of procedures of a supported occupation that are taught; skill attainment (the existing rule: three attempts at 80 percent mastery per tier); completion of the tutorial and a first scenario unaided;
teacher-reported use. Analytics (D-9) is how these are measured: events keyed by a generic id, with no personal data. Business measures are those in the market view and are not set here.

## 10. Risks and open decisions

1. **NEC edition.** Settled 2026-10-12: both editions are profiles (`nec-2023`, `nec-2026` draft). Still open: an expert verifies the 2026 numbers and values, and older content may still carry 2020 numbering. (D-5 to D-7)
1a. **Student data.** Students include minors and analytics is required, so the privacy notice, consent route and retention rules need counsel before any release that sends data. (D-9)
2. **Licensing.** The NEC, CEC and BS 7671 licence decisions are open, the fire alarm standard and the NESC would add more, and the market view says the NFPA cost is not modelled.
3. **SOP accuracy.** The 44 procedures were drafted from public pages; the key standards (the electrical safe work standard, the fire alarm standard, the NESC) are paywalled and were not opened. An occupational expert must review each occupation.
4. **Occupations the NEC does not cover.** Power-line work, and parts of fire alarm, elevator and utility work, need other codes. The owner decided to build utility line work (phase 2); until it exists, promising it in marketing would mislead buyers.
5. **Scenes.** Nothing teaches in 3D until scenes exist; content outpaces art.
6. **Scope growth.** Ten skills were approved: seven are built as lessons and step practice, three remain for phases 2 and 3. Sequence the rest by phase and by how many procedures each unlocks (`docs/occupations/TRACEABILITY.md`).

## 11. Where the detail lives

- Occupations, procedures and SOPs: `docs/occupations/` (profiles, `TRACEABILITY.md`, `ADJACENT.md`, `proposed-skills.json`, and the research data).
- Content rules: `docs/CONTENT_POLICY.md`, `docs/CONTENT_PLAN.md`, `docs/TEACHING_CONTENT.md`, `docs/POOL_AND_DRAW.md`, `docs/SCENE_DESIGN.md`, `docs/SKILL_SCORING.md`.
- Screens: the wireframe and the click-through mockup (`docs/design/wireframe.html`, `docs/design/mockup.html`, also published as private artifacts); they are updated with this document.
- Work remaining: `TODO.md`.
