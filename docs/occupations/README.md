# Occupations, procedures and SOPs

The app is driven by occupations (`docs/product/PRD.md`). This folder documents, for each target occupation, what it does, the procedures it follows and the standard
operating procedures (SOPs) that govern that work, and checks the app against them. Nothing here ships in the app; the app teaches skills (`docs/CONTENT_POLICY.md`).

## Contents

| File | What it is |
|---|---|
| `47-2111-electricians.md` and five more | One profile per core occupation: tasks, procedures with steps, hazards, controls and standards, coverage by the app, gaps. **Generated.** |
| `TRACEABILITY.md` | Every procedure against the app's skills and the skills still to build, with coverage totals. **Generated.** |
| `ADJACENT.md` | The 18 adjacent occupations: fit, knowledge needed, new skills needed, and checks on the market document. **Generated.** |
| `proposed-skills.json` | Skills the occupations need that the app lacks, each with its documented reason (the rule in `docs/CONTENT_POLICY.md`), standards, code profile needed and first content. |
| `data/*.json` | The research data the documents are generated from. This is the source to edit. |

## How it is made

1. **Research.** For each core occupation an agent read O*NET OnLine (tasks, tools, knowledge), the BLS, OSHA and trade sources, and wrote its tasks and 7 or 8 procedures with steps, hazards,
   controls, the standards that govern them and the pages that support them. Another agent checked the 18 adjacent occupations and the market document's claims.
2. **Mapping.** Each procedure lists the app's skills it uses (from the 13 in `ConceptIds`), and the skills it would need that the app lacks (`plannedSkills`, from `proposed-skills.json`),
   plus an honest coverage rating: **good** (the app's skills train the steps), **partial**, or **none**.
3. **Generation.** `python tools/occupations/build_docs.py` rebuilds the profiles, the matrix and the adjacent list from the data.
4. **Tests.** `tests/LogicTests/OccupationDataTests.cs` checks the data: every procedure is complete, every named skill exists or is proposed, a "good" rating names skills, a "none" rating names a skill still to build or says why not.

## What "SOP" means here

A procedure is the standard, repeatable way a task is done and made safe. An SOP is a procedure an employer or standard writes down. The steps in the profiles are the common elements of those
written procedures, summarised in our own words from public sources (OSHA rules are public domain; NFPA, UL and NESC documents are copyrighted and described, never copied). A real employer's SOP will add its own
equipment, forms and approvals.

## Reliability

- **Not reviewed by an occupational expert.** The research used public pages only; the paywalled standards that matter most (the electrical safe work standard, the fire alarm standard, the NESC, the NEC itself)
  were mostly not opened, and each profile lists its own doubts. An expert must review each occupation before an SOP is called reviewed.
- Coverage ratings are the researchers' judgement against the app's skills, not a test of the app.
- Employment figures are from O*NET and BLS as read at research time; recheck before quoting.

## Attribution

Occupation data: O*NET OnLine by the U.S. Department of Labor, Employment and Training Administration (USDOL/ETA), used under the CC BY 4.0 license (https://www.onetonline.org/).

## Updating

Change the data file (or `proposed-skills.json`), run the generator, run the logic tests, and commit all of it. Add an occupation by adding a data file in the same shape.
