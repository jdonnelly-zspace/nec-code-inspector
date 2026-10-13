# Product decisions

A log of the owner's decisions that shape the product. Newest first. Each entry says what was decided, what it affects and what it leaves open.

## 2026-10-13: delivery decisions for the deck (analytics, art, content, licences)

**Context.** Reviewing the stakeholder deck outline, the owner set how the product will be delivered.

| # | Decision | Effect |
|---|---|---|
| D-14 | **Analytics through RudderStack and Mixpanel**, specified by the owner in the format of zSpace's In-app Analytics Requirements (Confluence, PROD space). | Replaces "reuse the other apps' service" in D-9. The interface and the event list in the TODO wait on the owner's spec. |
| D-15 | **Art:** 3DFrame by Vection Technologies for the 3D scenes and art; GameCoders Studio, zSpace's art studio, where higher fidelity is needed. | The scene and art work in the TODO is delivered this way. |
| D-16 | **Content is verified from public information, with Claude checking it against the sources;** no expert sign-off is planned. | The deck must not claim expert review. The expert-review items in the TODO and the `reviewStatus` fields remain a record of what has and has not been reviewed. |
| D-17 | **Owner's position: no licences are needed for the installation codes and standards,** because the app teaches skills and competencies in its own words and does not reproduce them. | The deck no longer lists licences as a decision. This is the owner's position, not counsel's: `docs/CONTENT_POLICY.md` still records the licence status as unreviewed, and the market view itself called the NFPA licence cost an open risk, so a short confirmation from counsel is recommended before release. |

## 2026-10-13: phases, not waves, and utility line work before solar and alarm

**Context.** The owner judged the old third wave (power-line installers) more important than the second (solar PV and alarm installers), and asked to call them phases.

| # | Decision | Effect |
|---|---|---|
| D-13 | **Release phases replace waves, and the order of the last two swaps:** phase 1 electricians, helpers and inspectors; **phase 2 power-line installers and the adjacent clusters (utility line work)**; **phase 3 solar PV installers and security and fire alarm installers.** | PRD section 7, the deck outline, the TODO, `proposed-skills.json` (`phase`), `occupations.json` (`phase`), `OccupationData.phase` and the build scripts follow. Earlier entries below keep the old words: old wave 2 is now phase 3, old wave 3 is now phase 2. |

**Consequences to plan for.** The utility scenes (poles, vaults, bucket trucks; the largest new art in the plan) now gate the next phase, not the last, and the NESC licence question (see D-17) comes with them. Verifying the 2026 numbering for the PV content matters later than before. D-4's gate ("the earlier waves are done") now means phase 1. A release phase is not a content phase: `docs/CONTENT_PLAN.md` keeps its own phases 0 to 5.

## 2026-10-13: one naming system for levels

**Context.** The deck outline named occupations in the trade's words (electrician, helper, inspector) but the levels generically (Beginner, Standard, Expert). The owner asked for one vocabulary: the industry ladder, Apprentice, Journeyman, Master.

| # | Decision | Effect |
|---|---|---|
| D-12 | **Levels are named Apprentice, Journeyman and Master**, in the deck and in the app. This replaces the earlier decision for three generic levels (Beginner, Standard, Expert), which was made so the levels could connect to any credential. | The descriptions of each level stay as they are (guided, working knowledge, full fluency). Work: rename the labels first; rename the identifiers in code and content only if worth it. |

**Left open.** The labels are licence classes in many places, so the wording must say they describe skill and award nothing. How helpers, inspectors, solar, alarm and power-line occupations, and regions with other ladders, use the same three names. Both are written up in `docs/product/STAKEHOLDER_DECK_OUTLINE.md` section 7 with recommendations.

## 2026-10-12: teacher reporting, analytics and progress outside the app

**Context.** Four items were open in the PRD (teacher accounts, student data privacy, accessibility, localization). The owner decided all four.

| # | Decision | Effect |
|---|---|---|
| D-8 | **No teacher accounts in the first release.** Teachers get a per-student progress export (CSV, by skill and tier), collected by USB or shared folder. | FR-15. The export is to be built; `tools/alignment-report` already reads progress files. |
| D-9 | **Analytics is required** (the other zSpace Unity apps have it), and **progress must be trackable outside the app, and clear to the student**. A generic account or id (no personal data) ties events and exports to a student; a plain-language screen says what is collected, where it goes, who sees it, and how to export or delete it. This replaces the recommendation to collect nothing off the device. | FR-19 and FR-20, PRD section 6. Events carry ids, skill, tier, score and time, never free text, a name or an email. The analytics service is reused from the other apps; the app talks to it through an interface. |
| D-10 | **Accessibility floors are accepted:** never colour alone (wire colours always carry a label); a minimum text size and 4.5:1 contrast for UI text; student text at about grade 8; every timer can be turned off as an accommodation; a stereo depth setting; seated, stylus-only use with no long holds; read-aloud later. | PRD section 6. Needs tests on the device (stereo comfort, low vision); a text-size and contrast check can be automated for the UI. |
| D-11 | **English first; the other priority-one languages come next.** | Student text stays in data so it can be extracted; translate only reviewed content. The language list and order are the owner's to set (French for Canada is the likely first). |

**Left open.** Which analytics service and package the other apps use; what the generic account is (a class code, a student code, or a zSpace account); who hosts the place progress is viewed outside the app; the notice wording, consent route for minors and retention period (counsel); the list and order of the other priority-one languages.

## 2026-10-12: NEC editions are profiles, and rule numbers stay out of the student UI

**Context.** The TODO asked which NEC edition's numbering the content should follow (open decision 6). The owner's answer: the edition is a backend matter, the app teaches
skills, competencies, procedures and violations, and credentials (and the code's article numbers) only inform them.

| # | Decision | Effect |
|---|---|---|
| D-5 | **Both NEC editions are profiles:** `nec-2023` (the default; the verified content) and `nec-2026` (draft). Every NEC violation and quick reference card carries a citation for each. | `StreamingAssets/Codes/nec-2023`, `nec-2026`; `CodeProfileIds.Nec2023`, `Nec2026` (`Nec` stays an alias for 2023). The held load-calculation batch no longer waits on an edition choice: it is written for both. |
| D-6 | **`nec-2026` is a draft copied from 2023.** The only change applied is the renumbering known from secondary sources (Article 220 becomes 120). Everything else is unverified. | It is `draft`, so the US region keeps `nec-2023` until an expert reviews 2026. Sandbox designs exist for 2023 only (the 2026 dwelling load values are unverified). |
| D-7 | **Rule numbers are hidden from students.** `CodeTerminology.showReferenceNumbers` is false for every code; entries show their title, never "Art. 210.8". Numbers stay in the data for scoring, alignment and review. | The review, reference, card, flagging and sandbox panels show titles. The 78 texts that quoted rule or table numbers were rewritten without them (145 edits across both profiles, the violations and the cards); the test `StudentReferenceTests` warns if a new one appears. |

**Left open.** The 2026 numbers and values need a credential expert and the 2026 text; the Article 200 renumbering reported by one source is unconfirmed; chapter names ("Chapter 2") are hidden the same way (`ShownSectionName`), in the reference panel and the inspection intro.

## 2026-10-12: occupation-driven product, proposed skills, waves and utility line work

**Context.** The market view and the occupation research (`docs/product/PRD.md`, `docs/occupations/`) showed that the NEC was the starting point, not the definition of the
product, and that of the 44 documented procedures of the six core occupations the app teaches 2 well, 18 in part and 24 not at all.

| # | Decision | Effect |
|---|---|---|
| D-1 | **The app is occupation driven.** Occupations, their procedures (SOPs) and the skills behind them define the content; the installation code is a reference. | The PRD is the requirements source; `docs/PROJECT_BRIEF.md` is superseded. |
| D-2 | **All 10 proposed skills are approved:** electrically safe work, testing and measurement, troubleshooting, plans and documentation, jobsite safety, code administration, utility interface (wave 1); solar PV and storage, low-voltage and fire alarm (wave 2); utility line work (wave 3). | Their reasons in `docs/occupations/proposed-skills.json` now count as the documented reasons the content policy requires. None is in the app until it is added to `ConceptIds` with a lesson, content and tests. |
| D-3 | **The three waves are accepted** (PRD section 7). The first-half-2027 launch is not planned for all six core occupations: wave 1 is electricians, helpers and inspectors; wave 2 is solar PV and fire alarm installers; wave 3 is power-line installers and the adjacent clusters. | Marketing must not promise the occupations of waves 2 and 3 for the first launch. |
| D-4 | **Utility line work will be built** (wave 3): a NESC code profile and new utility scenes, rather than leaving power-line installers out of the core set. | It stays **blocked** until the NESC licence decision is made, the utility scenes are planned, and the earlier waves are done. The answer was given as "yes" to the question "build it, or leave it out?", and was read as "build it"; reverse it here if that was not meant. |

**Still open after these decisions**
- Which NEC edition's numbering the content follows (TODO, open decision 6): blocks the load calculation content and the PV articles.
- The licence decisions for the NEC, CEC, BS 7671 and now the fire alarm standard and the NESC.
- The PRD's open items: accessibility, student data privacy, localization and teacher accounts (PRD section 6).
- Correcting the market view before outside use (`docs/occupations/ADJACENT.md`).
- An occupational expert's review of the six profiles and 44 SOPs.

## Earlier decisions (recorded elsewhere)

- The app contains skills, never credentials (`docs/CONTENT_POLICY.md`). An occupation is a pathway, not a credential.
- The student chooses a region, which selects the code and units; the code is never named (`docs/CONTENT_POLICY.md`, section 3).
- Three generic difficulty levels (Beginner, Standard, Expert) described by what the student can do.
- Progress is per skill and tier; a skill is attained after three attempts at 80 percent mastery (`docs/SKILL_SCORING.md`).
- Scene rules for violations shared between codes (`docs/SCENE_DESIGN.md`).
