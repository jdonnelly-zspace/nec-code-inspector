# Wireframe and mockup

Two private artifacts show the app's screens. Their sources live here so they are versioned with the code; edit the file, then republish it to the same URL.

| File | Artifact | What it is |
|---|---|---|
| `wireframe.html` | [Code Inspector Wireframe](https://claude.ai/artifact/Mb7u276ZC6bgTWYhtkMAXB) | A schematic of the UI panels as they exist in code. Each screen names its script and says whether it is built, not wired into a scene, or planned. |
| `mockup.html` | [Code Inspector Mockup](https://claude.ai/artifact/8SWCa6eiGtK1kiWmzxFz6p) | A click-through prototype in the app's own look: 21 screens, a region switch, three difficulty levels. Scenes are illustrations and scores are example data. |

Both were brought up to date on 2026-10-12 with `docs/product/PRD.md` (decisions D-1 to D-11): occupations, procedure practice, skill lessons, the tutorial, a progress and data screen, accessibility settings, 20 skills, 17 reference cards, six sandbox designs, and rule titles instead of rule numbers, chapters and editions. The occupation, procedure and lesson text in the mockup is the real text from `StreamingAssets`.

The artifact files carry no page skeleton (no doctype, head or body): the Artifact tool adds it when publishing.

## Choices made without the owner (2026-10-12), to review

1. **Master difficulty.** With rule numbers hidden, the free-text citation becomes "name the rule in your own words". Built 2026-10-13: the words are matched to the rule's title and keywords (`docs/SKILL_SCORING.md`, "Naming the rule in words").
2. **The menu leads with "Learn a procedure"**, then scenarios, sandbox, skills and cards. Tutorial and "Progress and data" are small buttons under the headline.
3. **Occupation picker:** all six core occupations are open (the MVP covers all six, D-19). Students are not shown phase numbers.
4. **The "New in edition" badge was removed** from the rule lookup, because the student should not see an edition.
5. **Progress and data screen:** the ID format (`ZS-4F2K-07`), the destination ("your program's progress dashboard"), the three buttons (export, reset, delete) and all wording are placeholders. They wait on which analytics service the other zSpace apps use, what the generic account is, and counsel's review.
6. **Tutorial button mapping** (front inspects, middle marks compliant, back opens the reference) comes from `docs/ALPHA_BUILD_PLAN.md` and is unchecked on the device.
7. **Settings:** time limits can be switched off, which also removes the Master timer; text size and stereo depth are shown as controls, not built.
8. **Procedure practice** shows four steps (Apprentice) of a real procedure in a fixed shuffled order, scored by neighbouring pairs, as `ProcedurePractice` does.
