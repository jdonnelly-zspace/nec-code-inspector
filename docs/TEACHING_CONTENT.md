# Teaching content

Phase 2 of `docs/CONTENT_PLAN.md`: content that teaches a skill before the student is tested on it. This file says what exists, how it is
built, and what is still to do.

## 1. Skill lessons (built)

One short lesson per skill, 20 in all (13 code skills and 7 procedure skills), in `Assets/_Project/StreamingAssets/Lessons/lessons.json`.

| Part | What it is |
|---|---|
| `title`, `summary` | The skill in a sentence or two |
| `whatToLookFor` | 3 to 6 things a student checks in a scene |
| `whyItMatters` | The danger the skill guards against |
| `tiers` | What the student can do at Foundation, Practitioner and Authority (the same three tiers progress uses) |
| `commonMistakes` | 2 to 4 |
| `reviewStatus` | `draft` until a credential expert has read it, then `reviewed` |

### Rules for lesson text

- **Own words, no copied wording, no "shall".** The tests check.
- **No code, publisher or credential names, and no pathway words** (exam, apprentice, licence). The tests check.
- **No numbers or units.** Figures belong to the rules, which differ by region and by unit system; a lesson that said "within six feet"
  would be wrong in a metric region. The tests fail on any digit. Say what to look for, not how many.
- **Region words use tokens.** `{term:key}` and `{Term:key}` (capital for the start of a sentence) are filled from the student's code
  terminology, so the same lesson says GFCI, breaker and panel in the US and RCD, MCB and consumer unit in the UK. Every key a lesson uses must exist in the NEC
  terminology (tests fail otherwise) and should exist in the others (tests warn).
- **One lesson per skill, every skill covered.** Adding a skill to `ConceptIds` fails the tests until it has a lesson.

### How it reaches the student

- `LessonLibrary` loads the file at runtime from StreamingAssets and validates it; a broken file is logged and gives no lessons, so it never stops the app.
- `SkillLessonPanel` is a world-space panel that shows one lesson through `CodeProfiles.Terminology.Format`. It names no code and no credential.
- **Not wired yet.** Nothing calls `SkillLessonPanel.Show` yet and no scene has the panel. It has never run in the Unity Editor. The plan is to show the lesson
  before a student's first scenario that tests a skill, and from the skills screen, and to offer a "refresh" from the review screen when a skill is missed.

### Review

The 13 lessons are drafts written for the app and checked once by a separate pass against public sources, for wrong or region-specific statements. That
pass found 14 problems (2 must-fix, 7 should-fix, 5 minor) and all were fixed:

- A "common mistake" that was backwards (measuring outlet spacing along the wall is the right way, so the lesson no longer says otherwise).
- US and Canadian rules stated as general fact: arc-fault protection is required in some UK buildings and only recommended in others, almost all UK sockets
  need RCD protection, the UK has no prescribed outlet counts or spacing like the North American ones, a UK consumer unit can be a place to isolate supply, a UK
  supply can come with the supplier's earth terminal instead of an electrode, and a UK consumer unit can sit under the stairs. The lessons now say "where you are" or "where the rules require it".
- Safety wording: shock protection does not help if someone touches both live wires; the lesson now says so.
- Overstated or inconsistent lines: "most" fires, and three tier lines that mislabelled what the tier asks.

Not checked by the reviewer, so worth a look by the expert: the CEC, the outlet-type arc-fault distance limits, pool and spa rules by region, and the
terminal-temperature rule. The UK points above were inferred from sources that were silent on them. A credential expert should read all 13 lessons; the UK ones need a UK-qualified reader.

## 2. Still to build in phase 2

| Item | Now | Target | Notes |
|---|---|---|---|
| Tutorial | 8 steps as data (`StreamingAssets/Tutorial/tutorial.json`, `TutorialPanel`) | one guided walk-through | Teaches the stylus (point, front button to inspect, flag, cite, mark compliant with the middle button, open the reference with the back button), then finish. Built: the step format, validator, tests and panel. Still needs a small scene for the inspection part, and the inspection scene must call `TutorialPanel.Report` for each action; the button wording follows `docs/ALPHA_BUILD_PLAN.md` and should be checked on the device. |
| Quick reference cards | 17 (NEC only; every one of the 13 skills has a card) | one per skill, per region that has cards | The eight added on 2026-10-12 (working space, disconnecting means, marking, damp and wet locations, overcurrent, terminations, boxes and cable, panel enclosures) restate only entries already in `articles.json`, which went through the two-step check; they have not had a separate source check or an expert read. The CEC and BS 7671 have none. |
| Sandbox designs | 6 (2026-10-12: 1,200 to 4,200 sq ft, 150 A and 200 A services) | 6 (two per difficulty) | Each needs its required circuits, expected load and wire sizes checked by the load calculator tests. Only the NEC has the tables the sandbox needs. |
| Wiring the lessons in | not wired | shown before a first scenario, from the skills screen and from the review screen | Unity Editor work: add the panel to the menu scene and call it. |

## 3. Exit test (from the plan)

A new student can finish the tutorial and a first scenario unaided. That needs the tutorial, the lesson panel in a scene, and at least one built scenario,
so it cannot be met until the Unity scenes exist.
