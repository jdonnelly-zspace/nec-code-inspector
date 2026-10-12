# Product requirements

Status: draft. The owner accepted the occupation-driven direction, the proposed skills, the three waves and the utility line work decision on 2026-10-12 (`docs/product/DECISIONS.md`); the rest is still a draft for review. Supersedes `docs/PROJECT_BRIEF.md`, which was written around the NEC alone and names audiences and credentials.
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
 chooses        safe (steps, hazards,   (13 today,          (data, tested)
                standards)              10 more proposed)
                         \                    /
                    Code profile (region): NEC, CEC, BS 7671, and later others
```

- A **procedure** has an ordered set of steps, hazards, controls and the standards that govern it. Procedures are documented in `docs/occupations/` and are the unit the
  app is checked against.
- A **skill** is a unit of ability the app can teach and measure (`ConceptIds`). Procedures use one or more skills; skills are shared across occupations.
- **Content** teaches a skill: lessons, inspection scenarios, procedure trainers, sandbox designs, quick reference cards.
- Progress is recorded per skill and tier (existing). Progress per procedure is derived from the skills it uses (planned).

## 4. Users

### 4.1 Core occupations

The six occupations in the market view. "Fit today" is the share of the occupation's documented procedures that the app teaches at least in part, from the researchers'
coverage ratings (a judgement, not a test).

| O*NET code | Occupation | Workers (BLS or O*NET) | Procedures documented | Fit today | Verdict |
|---|---|---|---|---|---|
| 47-2111 | Electricians | 821,000 (2025) | 7 | 57% | Core. Needs safe work, testing, troubleshooting and plan skills. |
| 47-3013 | Helpers, electricians | about 85,000 | 8 | 38% | Core, mostly safety and basic skills the app lacks. |
| 47-4011 | Construction and building inspectors | about 106,000 | 7 | 71% | Best fit: the inspection scenarios are its work. Needs stages and code administration. |
| 47-2231 | Solar photovoltaic installers | about 22,000 | 7 | 57% | Needs a PV and storage skill (new content in NEC Articles 690, 705, 706). |
| 49-2098 | Security and fire alarm systems installers | about 85,000 | 7 | 43% | Needs low-voltage and fire alarm content and a second code (the fire alarm standard). |
| 49-9051 | Electrical power-line installers and repairers | 131,900 (2025) | 8 | 12% | **Wave 3, blocked.** Governed mainly by the NESC and OSHA line-work rules, not the NEC. Decided: build it (a NESC profile and utility scenes), after the licence decision. |

### 4.2 Adjacent occupations

Eighteen more occupations in four clusters (`docs/occupations/ADJACENT.md`). The research found nine where beginner electrical literacy fits, five that need new skills, and six that are not
NEC occupations. General maintenance workers are the strongest fit among them.

### 4.3 Learner levels

Beginner, Standard and Expert are generic levels described by what the student can already do. They do not name a school pathway or credential.

### 4.4 Buyers

CTE programs, apprenticeship and trade-school programs, community colleges, and inspector continuing-education programs (the market view's three tiers: high-school CTE, apprentice and trade school, continuing education and inspector programs). Teachers need reporting; see FR-15.

## 5. Functional requirements

Status: **Built** (in the repository and tested), **Partial** (data or code exists, not wired or not run in Unity), **Planned**, **Blocked** (needs a decision or something outside the project).

| ID | Requirement | Status | Where |
|---|---|---|---|
| FR-1 | The student chooses an occupation; the app shows the procedures of that occupation and which are taught. | Partial | `occupations.json`, `OccupationLibrary`, `OccupationPickerPanel`; the panel is not in a scene yet. |
| FR-2 | Each procedure is available as an SOP: purpose, steps, hazards, controls, standards. | Partial | 44 procedures load from `occupations.json` (purpose, steps, hazards, controls; standards stay in the research documents because students never see them); draft, no expert review. |
| FR-3 | Skills with tiers (Foundation, Practitioner, Authority); code skills and procedure skills. | Partial | 20 built (13 code, 7 procedure skills of wave 1; `ConceptIds`); 3 skills of waves 2 and 3 not built. |
| FR-4 | Inspection scenarios: point at a part of a 3D building, flag a violation, cite the rule, get scored feedback. | Partial | 6 scenarios, 139 NEC violations, 14 CEC, 9 BS 7671 as data; no 3D scenes built. |
| FR-5 | Sessions draw a different set of violations each time, spread across skills. | Built | `ViolationDraw`, `docs/POOL_AND_DRAW.md`. |
| FR-6 | Procedure trainers: guided practice of a procedure's steps (lockout and test-before-touch, troubleshooting, a staged inspection), including a virtual meter and tester. | Partial | Step-ordering practice built for 28 of 44 procedures (`ProcedurePractice`, `ProcedurePracticePanel`); the virtual meter and scene-based trainers are planned. |
| FR-7 | Panel design sandbox: design, wire and check a residential panel. | Partial | 1 design, US only (needs tables); scene not built. |
| FR-8 | Skill lessons before practice, in the student's region's words. | Partial | 20 lessons and a panel; not wired into a scene. |
| FR-9 | Tutorial that teaches the stylus and a first inspection. | Partial | 8 steps as data with a panel (`tutorial.json`, `TutorialPanel`); needs a scene and `Report` calls from the inspection scene. |
| FR-10 | Reference lookup: search rule entries and quick reference cards. | Partial | NEC 161 entries, CEC 12, BS 7671 29; 13 NEC cards; UI built, not run in Unity. |
| FR-11 | Region choice sets the code, units and wording; the code is never named. | Partial | Built and tested; picker not wired into a scene. |
| FR-12 | Three generic difficulty levels. | Built | Text and tests. |
| FR-13 | Progress per skill and tier, saved with the student; derived progress per procedure. | Partial | Per skill built; each procedure practice is saved with its score (`ProcedureProgress`); a derived per-procedure view is planned. |
| FR-14 | A content pipeline: scaffold, tests, source verification, expert worksheets. | Built | `tools/`, `docs/AUTHORING_VIOLATIONS.md`, expert review packet. |
| FR-15 | Teacher reporting: class progress by skill and procedure. | Planned | An offline alignment report exists (`tools/alignment-report`); nothing in the app. |
| FR-16 | Code profiles for each region and for occupations that need another code (fire alarm standard). | Partial | NEC, CEC (draft), BS 7671 (draft); a fire alarm profile planned; the NESC blocked. |
| FR-17 | Edition handling: the app knows which edition's numbering its content follows. | Blocked | Open decision 6 (TODO). |
| FR-18 | Occupation fit is visible to the owner: a generated traceability matrix and gap list. | Built | `docs/occupations/TRACEABILITY.md`, `tools/occupations/build_docs.py`. |

## 6. Non-functional requirements

- **Performance:** 90 fps minimum on zSpace Inspire 2 (stereo), under 200 draw calls, particle systems under 5,000.
- **Input and display:** stylus primary, mouse only as an editor fallback; all UI canvases world-space; ZCamera, never a normal camera; TextMeshPro for all text.
- **Platform:** Windows 11, Unity 6.3 LTS, zCore 6.3, URP.
- **Content accuracy:** every citation, number and procedure step is checked by a second pass against public sources, then an expert, before it is called reviewed. Statuses are visible in the worksheets.
- **Content licensing:** installation codes and standards are copyrighted; content is paraphrase plus reference numbers. The licence decision is the owner's (open).
- **Testing:** logic tests run in CI, a Unity compile check runs locally, and the zSpace hardware is tested weekly.
- **Gaps in this draft:** accessibility needs, student data privacy and retention, localization (open decision), and teacher account and classroom management are not yet specified.

## 7. Scope by occupation, in waves (accepted 2026-10-12)

The market view plans the six core occupations for a first-half-2027 launch. The research does not support that for all six:

| Wave | Occupations | What has to exist first |
|---|---|---|
| 1 | Electricians, electrician helpers, inspectors | New skills: electrically safe work, testing and measurement, troubleshooting, plans and documentation, jobsite safety, code administration; the 3D scenes; a staged inspection; the remaining phase 1 content. |
| 2 | Solar PV installers, security and fire alarm installers | A PV and storage skill, and a low-voltage and fire alarm skill with a second code profile and its licence decision; both depend on the NEC edition decision for the PV articles. |
| 3 | Power-line installers; adjacent clusters | Utility line work, decided 2026-10-12: a NESC code profile and new utility scenes, after the NESC licence decision and the earlier waves; beginner-literacy packages for maintenance, HVAC and construction roles. |

## 8. Market, as stated and as checked

The market view claims: a primary US education TAM of $26 million (6,400 programs at $250 to $375 per device, 15 devices a program), an adjacent TAM of $34.6 million, and a combined $60.6 million
across 24 occupations. The occupation research checked what it could against O*NET and BLS:

- Worker and opening totals do not reconcile: the 18 adjacent occupations alone sum to about 6.4 million workers, so "about 6.7 million" for all 24 looks understated or uses another basis; the 700,000 openings figure is plausible.
- "All Bright Outlook" is false for five of the 18 (sheet metal workers, helpers of plumbers, energy engineers, energy auditors, telecom installers).
- The document says 24 occupations but lists 6 core plus 18 adjacent with one repeated (49-2098 appears in both groups); codes and titles for 49-9021, 49-2095 and one 49-9071 entry are wrong or mislabelled.
- Three adjacent occupations that look electrical (powerhouse and substation repairers, transportation equipment installers, energy engineers) are not NEC occupations, and elevator work is governed by a different standard.
- The pricing, penetration and revenue figures are the owner's business assumptions; none was checked. The document itself notes that the NFPA licence cost is not modelled.

Details: `docs/occupations/ADJACENT.md`. The market view should be corrected before it is used outside the team.

## 9. Success measures

Product measures (to be set by the owner): share of procedures of a supported occupation that are taught; skill attainment (the existing rule: three attempts at 80 percent mastery per tier); completion of the tutorial and a first scenario unaided;
teacher-reported use. Business measures are those in the market view and are not set here.

## 10. Risks and open decisions

1. **NEC edition.** Content follows 2023 numbering; the project targets 2026; checks keep finding 2020 numbering in older content. Blocks load calculation and PV content. (TODO, open decision 6)
2. **Licensing.** The NEC, CEC and BS 7671 licence decisions are open, the fire alarm standard and the NESC would add more, and the market view says the NFPA cost is not modelled.
3. **SOP accuracy.** The 44 procedures were drafted from public pages; the key standards (the electrical safe work standard, the fire alarm standard, the NESC) are paywalled and were not opened. An occupational expert must review each occupation.
4. **Occupations the NEC does not cover.** Power-line work, and parts of fire alarm, elevator and utility work, need other codes. The owner decided to build utility line work (wave 3); until it exists, promising it in marketing would mislead buyers.
5. **Scenes.** Nothing teaches in 3D until scenes exist; content outpaces art.
6. **Scope growth.** Ten proposed skills is a large addition; sequence by wave and by how many procedures each unlocks (`docs/occupations/TRACEABILITY.md`).

## 11. Where the detail lives

- Occupations, procedures and SOPs: `docs/occupations/` (profiles, `TRACEABILITY.md`, `ADJACENT.md`, `proposed-skills.json`, and the research data).
- Content rules: `docs/CONTENT_POLICY.md`, `docs/CONTENT_PLAN.md`, `docs/TEACHING_CONTENT.md`, `docs/POOL_AND_DRAW.md`, `docs/SCENE_DESIGN.md`, `docs/SKILL_SCORING.md`.
- Work remaining: `TODO.md`.
