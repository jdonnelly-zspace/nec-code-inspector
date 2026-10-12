# Product decisions

A log of the owner's decisions that shape the product. Newest first. Each entry says what was decided, what it affects and what it leaves open.

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
