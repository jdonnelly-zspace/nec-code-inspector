# Review Guide for the PR Stack

Nothing from this work is on `main` yet. This guide is for the person approving it. `main` is protected and needs an approving review.

## The stack

Each PR targets the branch below it. Merge from the bottom, and retarget the next PR to `main` after each merge (`gh pr edit <n> --base main`).

| Order | PR | Branch | Size | What it does |
|-------|----|--------|------|--------------|
| 1 | [#2](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/2) | `feature/violation-concept-ids` | +97 / -1, 6 files | Adds a code-neutral skill ID (concept ID) to every violation |
| 2 | [#3](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/3) | `feature/code-profile` | +226 / -55, 12 files | `ICodeProfile`: the NEC database becomes one installation code among several |
| 3 | [#4](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/4) | `feature/profile-tables` | +369 / -108, 9 files | Electrical tables and compliance rule settings move into profile data |
| 4 | [#5](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/5) | `feature/scenario-data` | +950 / -1158, 13 files | Scenario content moves from editor generators into JSON |
| 5 | [#6](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/6) | `feature/credential-roadmap` | +827 / -116, 18 files | Logic tests, CI workflow, roadmap |
| 6 | [#7](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/7) | `feature/violation-citations` | +563 / -108, 21 files | One citation per installation code on each violation |
| 7 | [#8](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/8) | `feature/skill-progress` | +1214 / -104, 24 files | Progress tracked per skill (see the history note below) |
| 8 | [#9](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/9) | `feature/neutral-wording` | +4712 / -2162, 99 files | Neutral wording and units, plus everything merged into it since |
| side | [#1](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/1) | `docs/certification-bodies` | +188, 2 files | Certification research and the design doc. Targets `main` directly |

## How to review

- **#2 to #7** are small and focused. Read the diff normally.
- **#8** was written when credentials were still part of the app. A later PR removes them (see below). Skim it for the skill progress logic (`Scripts/Skills/`, `ProgressManager`) and judge the final state in #9.
- **#9** is large because fourteen PRs were merged into it after it was opened. Review it by area, using the merged PRs as the unit, not as one 99-file diff.

### Inside #9, in the order it was built

| PR | What to look at |
|----|-----------------|
| (base of #9) | Terminology per code, neutral names, scenario sections derived from citations, units from profile tables, `docs/CONTENT_POLICY.md` |
| [#10](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/10) | All 42 violation texts and 98 article texts rewritten in new words; the wording check now fails on statutory wording |
| [#11](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/11) | `DataCodeProfile` and `CodeProfileLibrary` (any folder under `StreamingAssets/Codes/` loads as a code); Canada (`Codes/cec/`) added as data. This PR also added a credential profile, removed by #12 |
| [#12](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/12) | Removes credentials from the app. The app holds skills and installation codes only |
| [#13](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/13) | Code picker screen (`CodeProfilePickerPanel`, `CodeProfileChoices`) |
| [#14](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/14), [#15](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/15) | Editor tools that build the picker and the main menu scene (nothing runs without Unity) |
| [#16](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/16) | Expert review packet (`docs/expert-review/`) and its generator |
| [#18](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/18), [#19](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/19) | Scene design rules, then scene values and limits for four shared violations (`SceneFacts`) |
| [#20](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/20), [#21](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/21), [#22](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/22) | `artSet` on profiles; scenarios hidden under a code with no art; an empty-list message on the menu |
| [#23](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/23), [#24](https://github.com/jdonnelly-zspace/nec-code-inspector/pull/24) | `.gitignore` for test output; TODO refresh |

PR #17 updated the design doc and was merged into #1's branch.

### The final diff of #9 by area

| Area | Added / removed | Files |
|------|-----------------|-------|
| `StreamingAssets/Codes` (NEC and CEC data) | +1029 / -0 | 6 |
| `tests/` | +941 / -298 | 15 |
| `Scripts/Codes` (profiles, matcher, terminology, tables) | +754 / -14 | 11 |
| `docs/` | +546 / -10 | 10 |
| `Scripts/Editor` (importer and scene tools) | +535 / -37 | 6 |
| `Scripts/UI` | +239 / -53 | 8 |
| `Content/Scenarios` | +198 / -64 | 4 |
| `Scripts/Data` | +186 / -4 | 6 |
| Removed: old `NECDatabase` JSON, credential scripts and data | -1,326 | 5 |

The data and test files are large but mechanical. The logic to read closely is in `Scripts/Codes`, `Scripts/Skills`, `Scripts/Data` and `ProgressManager`.

## What to run

```bash
dotnet run --project tests/LogicTests
```

Needs the .NET 8 SDK only, not Unity. At the time of writing: 1309 checks, 0 failed, 15 warnings (13 dangling related-article references and 2 skill-coverage notes about gaps in the draft CEC content). CI runs the same command on every PR.

## What has not been verified

- **Nothing has been compiled or run in Unity.** The logic tests compile engine-independent scripts only. Scripts using uGUI, TextMeshPro or the zSpace SDK are not checked: `MainMenuPanel`, `CodeProfilePickerPanel`, `CodePickerSceneSetup`, `MainMenuSceneGenerator`, and the UI renames.
- **No scene file exists.** `Scenes/MainMenu/` is empty until someone runs *NEC Inspector > Scene Setup > Generate Main Menu Scene* in the Editor.
- **`FormerlySerializedAs`** keeps old serialized values through the renames, but only an Editor session can confirm that existing assets still load.

## What a code reviewer cannot judge

These need a credential expert (packet in `docs/expert-review/`):
- Accuracy and edition of the paraphrased NEC texts, and of the draft CEC entries written from public summaries.
- Seven suspected wrong or swapped NEC references, listed in `docs/CONTENT_REVIEW.md`.
- Whether both codes measure receptacle spacing the way the scene values assume.
- Which of the 44 NEC articles not tied to a violation to keep.

## History note

The history shows two things that were later reversed: credential profiles (added in #8 and #11, removed in #12) and credential-specific files. The final state has none, and `ContentPolicyTests` fails if credential content returns. Two ways to merge:

1. **Merge as is** (recommended): keeps each step reviewable and matches the PR descriptions. Credentials appear in intermediate commits only.
2. **Squash-merge #8 and #9**: keeps credentials out of `main`'s history, but needs each upstream PR retargeted by hand and loses the step-by-step trail.

## Decisions waiting on the owner

See "Open decisions" in `TODO.md`: how to compile in Unity in CI, the first code after Canada, localization, concept-level scoring, and which panels belong in the menu scene.
