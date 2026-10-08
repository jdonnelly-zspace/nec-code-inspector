# NEC Code Inspector - TODO

## Roadmap: a skill-based app that helps people get any electrician credential

Goal: a skill-based app that helps people build the skills to get an electrician credential (US, Canada, UK, EU, ...). **The app contains skills and installation codes, never credentials**: no credential names, blueprints, exam breakdowns or readiness scores ship in it (the tests enforce this). How skills line up with a credential is worked out outside the app (`docs/credential-alignment/`).
**Acceptance test:** a new installation code (Canada's CEC was the first) is added using only data files: a code profile folder, citations on the scenarios, and region-specific scenes where needed. If C# has to change, the abstraction has a leak.

Status key: `[x]` done and merged into the stack branch, `[~]` built and in review (a PR is open) or waiting on someone outside the code, `[ ]` not started, `[!]` blocked or needs a decision.
**Update these statuses in the same PR as the work.**

**Where the work lives.** Nothing below is on `main` yet. The stack is PRs #2 to #9, each targeting the branch below it (`main` needs an approving review), plus the docs PR #1 (`docs/CERTIFICATION_BODIES.md`, `docs/CREDENTIAL_FRAMEWORK.md`, already updated to the skill-based design). Everything merged since (PRs #10 to #16 and #18 to #23: paraphrased content, Canada, credential removal, picker, scene tools, expert packet, scene values, `artSet`, art-set blocking, empty-list message) was merged into PR #9's branch, `feature/neutral-wording`, so PR #9 is large: review it commit by commit. The logic tests (`dotnet run --project tests/LogicTests`, .NET 8 SDK) pass locally and in CI.

### What is left, grouped by who can do it
- **Project owner and reviewers:** approve and merge the PR stack (#2 to #9, #1); answer the open decisions below.
- **Credential expert** (packet in `docs/expert-review/`): accuracy and edition of the paraphrased NEC and draft CEC content; the suspected reference problems; how both codes measure receptacle spacing; which untied articles to keep; CEC rules for violations still hidden under the CEC; the Red Seal task-to-skill mapping.
- **Unity Editor** (nothing here has been compiled in Unity): run `Generate Main Menu Scene` and commit the scene; add a ZCamera rig and check layouts; compile-check `CodePickerSceneSetup`, `MainMenuSceneGenerator`, `MainMenuPanel` and `CodeProfilePickerPanel`; build the 3D scenes and the North American art set; rename NEC-named MonoBehaviours with references visible.
- **Code, can start now:** scene facts for the shared violations whose rules are tables or scope; tests for the `ComplianceChecker` rules; the dangling `related` references; `{term:key}` tokens in scenario text; moving editor-script content to JSON; the alignment report outside the app.
- **Blocked on a decision:** Unity compile in CI; first code after Canada; localization; concept-level scoring policy.

### Phase A - Foundations (refactor steps 1-4)
- [~] A1. Code-neutral concept IDs on violations (PR #2)
- [~] A2. `ICodeProfile`, `CodeArticle`, `CodeProfiles` registry; `NECDatabase` is the NEC profile (PR #3)
- [~] A3. Electrical tables and compliance rule settings as profile data (PR #4)
- [~] A4. Scenario content in JSON with a validating importer (PR #5)

### Phase B - Verify, then finish the data layer (current)

**B1. CI and tests**
- [~] Logic test project (`tests/LogicTests`) and GitHub Actions workflow (`.github/workflows/logic-tests.yml`): tables JSON matches built-in defaults, load calculator parity, citation matching, scenario and article JSON validation
- [~] Fix compile error found by a local compile: `QuickReferenceCardPanel` used `NECReferencePanel` without `using NECInspector.Inspection`
- [~] Move `ViolationSeverity` into its own file and share the scenario file validator between the importer and the tests
- [ ] Get the PR stack approved and merged into `main` in order: #2 to #9 (retarget each to `main` as the one below merges), plus the docs PR #1. `main` requires an approving review, so this needs a reviewer. PR #9 also carries everything merged into it since (#10 to #16, #18 to #23)
- [ ] Fix the 13 dangling `related` references in `Codes/nec/articles.json` (13 of the 15 test warnings; the reference panel silently skips them). Needs the expert to say whether to add the missing entries or drop the links
- [x] Tests for the 10 panel compliance rules: the rules moved into `ComplianceRules` (plain data in, results out, tables passed in) and `ComplianceChecker` only reads the scene objects into that data (52 checks in `ComplianceRulesTests`; boundary mutations were confirmed to fail them). The reader in `ComplianceChecker` still needs a Unity compile check; the public `Check*` methods it used to have were only called from inside the class and are gone
- [x] Document how to run the tests locally (CONTRIBUTING.md > Tests); `tests/**/bin` and `obj` are git-ignored
- [!] Unity compile gate in CI (a local check now exists: `tools/unity-compile-check`). Blocked: the repo has no `ProjectSettings/` or `Packages/`, the zSpace SDK is proprietary and git-ignored, and CI would need a Unity license secret. Decision needed on how to make the project compilable in CI (stub the SDK, or a self-hosted runner)

**B2. Finish the data layer**
- [~] Per-profile citations on violations: `citations` list (profileId, reference, text) replaces `necArticle` / `necArticleText`; scoring compares the student's citation to the active profile's reference (this branch)
- [ ] Concept-aware scoring: let a citation in the same concept count (for example 210.8(A)(1) vs 210.8(A)(5)). Needs article-to-concept tags in each profile's article data and a policy decision on how much credit it earns
- [~] Skill-based progress (decision: progress is per skill, never per credential). Evidence from inspections and the sandbox updates a running mastery per skill and tier; a skill is attained at 3+ attempts and 80% mastery; a higher tier covers a lower one (this branch)
- [x] Credentials removed from the app (decision: it is a skill-based app that helps people get the credential). Deleted credential profiles, readiness, deferred requirements and the credential files; progress is per skill and the student's chosen installation code is stored in the progress file and applied at startup. Alignment notes live outside the app in `docs/credential-alignment/` (this branch)
- [~] Certificates are earned from skills (`requiredSkills` + `requiredTier`) instead of NEC chapters; the dead chapter-mastery code is removed; the dashboard shows skill mastery
- [ ] Alignment notes outside the app for other credentials (NCCER Electrical, ...), drafted in our own words and reviewed by a credential expert; the Red Seal 309A draft is in `docs/credential-alignment/`
- [~] The student's chosen installation code is stored in the progress file and applied at startup (`ProgressManager.SetActiveCodeProfile`). The picker screen is built (`CodeProfilePickerPanel`, see Phase D); it still has to be wired into the main menu scene in the Unity Editor
- [ ] Finer skill taxonomy: the 11 concepts are the skills for now; motors or hazardous locations may need sub-skills (each new skill needs a documented reason in `docs/credential-alignment/`)
- [x] An alignment report outside the app: `python tools/alignment-report/report.py PROGRESS.json ALIGNMENT.json` reads the progress file against an alignment file and shows what is attained, what the app can still teach, and what waits on content (unit tests run in CI)
- [x] Update `docs/CREDENTIAL_FRAMEWORK.md` (PR #1): rewritten to the skill-based design (PR #17, merged into PR #1's branch). It describes the stacked branches, so keep it in step as they merge
- [~] Violation and scenario applicability, derived from citations: a violation applies to a profile only if it has a citation for it; a scenario applies if any violation does. `InspectionManager` skips non-applicable violations and the main menu hides non-applicable scenarios (this branch)
- [~] Rename leftover NEC-named API from the citation change (`FlagViolation(... necArticle)`, `citedNECArticle`, `FlaggedNECArticle`): done in Phase C (this branch)
- [ ] Move remaining editor-script content to JSON: panel sandbox circuits, quick-reference cards, certificate templates, difficulty settings

### Phase C - Neutral wording and units (old refactor step 5)
- [~] Neutral names (this branch): `NECCitationMode` -> `CitationMode`, `highlight2026Changes` -> `highlightNewInEdition`, `isNewIn2026` -> `isNewInEdition` (JSON and DTO), `necReference(s)` -> `codeReference(s)`, `dwellingSquareFootage` -> `dwellingArea`, `citedNECArticle` / `FlaggedNECArticle` -> `citedReference` / `FlaggedReference`. Serialized fields keep their old names through `FormerlySerializedAs`
- [~] Scenario sections are derived from the violations' citations in the active profile; the stored NEC `necChapters` lists (scenario and panel definitions) are removed
- [~] Terminology per profile (`CodeTerminology`, `terminology.json`): code name, reference format, section names, dropdown prompt and vocabulary (grounding/earthing, panel, breaker, ...). UI strings ("NEC Citations", "NEC Chapters", "Art.", "Chapter") now come from the active profile, and `{code}` / `{term:key}` tokens work in UI text
- [~] Units and defaults from the profile tables: area unit label, default conductor, single-pole voltage for measurements; the HUD no longer prints "Art. General Practice"
- [~] `{term:key}` and `{code}` tokens in scenario and violation text are filled from the active code's terminology (`ViolationDefinitionSO` getters, scenario title and description in the menu and intro step). One violation (`BC-GFCI-BATH-001`) uses them as the working example, and a test checks every key used exists in the NEC terms and warns when another code lacks it. Still to do: convert more text when a code with different words arrives (UK earthing, consumer unit)
- [ ] Metric profile data: mm2 conductor table, 230/400 V, m and m2 labels (needed with the first non-NEC profile, Phase D/G)
- [ ] Move the remaining NEC wording out of editor-script content (quick-reference cards, panel sandbox descriptions, certificate text) into JSON content files (see B2)
- [ ] Rename MonoBehaviour classes, scene-serialized fields and files that carry NEC names (`NECReferencePanel`, `NECReviewStep`, `_necDropdown`, ...). Deferred on purpose: it can break scene and prefab references, so do it in the Unity Editor with the references visible
- [ ] Decide on renaming the `NECInspector` namespace (cosmetic, defer)

### Phase D - Prove it with Canada (CEC)
- [~] CEC code profile (`StreamingAssets/Codes/cec/`, draft, partial): 7 entries explained in our own words from public summaries, terminology (Rule, Section), no tables yet. Edition numbering differs (older editions use 26-722, newer 26-712); **a credential expert must check every entry against the edition the Red Seal exam uses**
- [x] Red Seal 309A draft moved out of the app to `docs/credential-alignment/red-seal-309a-draft.json` (not shipped). It maps 8 exam tasks (48 of 100 questions) to the app's skills by estimate; the app covers about 13 of the 100 questions' worth under the CEC so far
- [~] CEC citations on 9 of the 42 violations, with per-code wording (metres, 1.8 m, 900 mm, 3 m rods, 1 m working space) where the shared text carries NEC numbers. The other 33 violations are hidden when the CEC is active
- [~] Acceptance test: Canada was added with data files (profile folder and citations); the profile tests discover new folders without any test code. The code changes it needed are listed below as defects found in the abstraction
- [ ] **CEC content expansion** (needs verified rules, expert help): conductor sizing, load calculation, disconnecting means, equipment installation, identification and marking have no CEC violations; shock protection, earthing and bonding, and working space only have Foundation-level ones. The skill coverage test reports these gaps as warnings
- [ ] CEC tables (conductor ampacities, demand factors) so the panel sandbox can count toward the CEC; the sandbox is gated by `HasOwnTables` until then
- [ ] Confirm the Red Seal task-to-skill mapping and weights in `docs/credential-alignment/` with a credential expert; read the full 309A occupational standard (the PDF could not be read here, only the exam breakdown page)
- [ ] CSA licensing check before shipping CEC content (see Phase F)
- [~] Code picker in the UI: `CodeProfilePickerPanel` lists the loaded codes with their review status, scenario coverage and a sandbox note, applies the choice at once and saves it; `MainMenuPanel` has `ShowCodePicker()`, an optional `_activeCodeText` label, and refreshes the scenario list and difficulty text when the code changes. Choice wording is tested without Unity (`CodeProfileChoices`). **Needs Unity Editor work** (see Alpha, Active)

Defects found in the abstraction while adding Canada (all fixed here unless noted):
- The NEC was a C# class, so a second code needed code. Fixed once: `DataCodeProfile` and `CodeProfileLibrary` load any folder under `StreamingAssets/Codes/`; the NEC data moved to `Codes/nec/` and is loaded the same way
- Shared violation wording carried NEC numbers (feet, 36 in, 8 ft rods): citations can now override description, hint and note per code
- (Removed again) Credentials could require skills a code's content does not cover, so requirements got a `deferred` flag; this went away when credentials left the app. The skill coverage test now reports the same gaps per code
- Sandbox results were always checked against NEC tables: sandbox evidence now needs the active code to have its own tables
- The citation matcher accepted `26-70` for `26-700`: it now matches only at a level boundary (this also closes the matching item in Phase F)
- Facts differ between codes (GFCI scope, rod length and count, spacing limits), so a scene must break the rule under every code it is offered for, or be shown to one code only. Rules accepted in `docs/SCENE_DESIGN.md` (margin, pass-both, split when incompatible, SI units, automatic check). Done: measured values, limits and the logic test for the four single-number violations (wall and counter spacing, ground rod, working space), and the counter gap violation redone (`SceneFacts`). Still to do: shared violations whose rules are tables or scope (wire sizes and breaker ratings, GFCI and AFCI areas) need a different kind of check than a single limit
- [ ] Scene facts for the five other violations shared by the NEC and CEC (`BC-GFCI-BATH-001`, `BC-WIRE-14AWG-001`, `RP-WIRE-OVER-001`, `BC-AFCI-BEDROOM-001`, `BC-AFCI-LIVING-001`): design the check (table lookup for wire and breaker pairs; room scope for GFCI and AFCI)

### Phase E - Hard problems (decide early)
- [x] Decision: region-specific scene art and prefab variants: accepted in `docs/SCENE_DESIGN.md` (one art set per region group, North American set first, other groups blocked until a second code is committed). Done: `artSet` in `profile.json` with validation (`ArtSets`), and the menu and the picker offer no scenarios under a code whose art set has no art (`ArtSets.IsAvailable`)
- [ ] Build the North American art set (Unity Editor): panels, receptacles, breakers, wire colours. Scenes must follow the accepted scene design rules: scene values break every listed code's limit by 10%, compliant parts pass every offered code
- [ ] Expert to confirm both spacing violations measure from the farthest point on the wall line to the nearest receptacle (asked in `docs/CONTENT_REVIEW.md`); the counter scene was redrawn on that reading
- [ ] Generalize or add a panel sandbox model beyond split-phase US panels (UK consumer units with RCD/RCBO, 230 V single phase)
- [ ] Pluggable compliance rule sets per profile (rule logic is still in C#; only numbers, citations and on/off are data)

### Phase F - Governance
- [~] Paraphrase pass (this branch): all 42 violation citation texts and 98 article texts rewritten in new words (facts and references kept, shorter than the source, no six-word runs copied); the wording check in `ContentPolicyTests` now fails on any statutory wording. Notes for the reviewer are in `docs/CONTENT_REVIEW.md`
- [~] **Credential expert review** (packet ready in `docs/expert-review/`: worksheets for 51 violation citations, 98 NEC and 7 CEC articles, regenerated with `tools/make_expert_review.py`; waiting on the expert) of the paraphrases for accuracy and edition (2026), and of the suspected reference problems in `docs/CONTENT_REVIEW.md` (two swapped citation pairs, a mismatched load-calc citation, `210.8(A)` numbering)
- [ ] Run the same paraphrase check on the text in editor scripts (quick-reference cards, sandbox descriptions, certificates) when it moves to JSON
- [ ] Trim `Codes/nec/articles.json` to articles that support the app's skills: 44 of 98 are not tied to any current violation (list in `docs/CONTENT_REVIEW.md`, flagged in `docs/expert-review/3-articles-nec.csv`); the expert decides which to keep
- [ ] Licensing status per profile (NEC, CEC, BS 7671 are copyrighted); record it in the profile
- [~] Content policy documented and enforced: content only for skills in `ConceptIds`, no credentials or credential code in the app (both fail the tests), and statutory wording fails the tests
- [~] Skill coverage per installation code: the tests fail if the NEC lacks Practitioner-level content for any skill and warn about gaps in other codes. A report that compares coverage with an alignment file belongs outside the app
- [x] Naming: public constants and static data (`ConceptIds.ShockProtection`, `ArtSets.NorthAmerica`) are PascalCase as in the rest of C#; private and local constants stay UPPER_SNAKE_CASE (`SCENE_PATH`). `.claude/CLAUDE.md` now says so
- [x] Citation matcher is segment-aware (`250.2` no longer matches `250.24`, `26-70` no longer matches `26-700`) (Phase D)

### Phase G - Later: UK and Europe
- [ ] BS 7671 profile (UK; metric conductors, 230 V, different terminology) as the stress test
- [ ] France (NF C 15-100), Germany (DIN VDE 0100) and other HD 60364 national codes
- [ ] Decide whether UI language localization (French, German) is in scope

### Open decisions
1. How to get a Unity compile in CI (Phase B1)
2. First installation code after Canada, if a customer is waiting on the UK or EU
3. Language localization in or out of scope
4. Scoring policy for concept-level credit (Phase B2)
5. Which panels beyond mode, scenarios, difficulty, settings and code picker go into the menu scene (progress dashboard, reference panels)

Decided: region-specific art is one set per region group (`docs/SCENE_DESIGN.md`).

## Current Phase: Alpha - Unity Editor Integration

### Active (Unity Editor Required)
- [ ] Boot scene (not started) and MainMenu scene (generator below; the scene file still has to be generated and committed)
- [ ] Residential panel 3D scene with embedded violations + prefab wiring
- [ ] Kitchen/bathroom/living area 3D scene (Branch Circuit scenario)
- [ ] Run NEC Inspector > Import Scenario Data + PanelDesignSandboxGenerator in Editor (run it after any change to the scenario JSON)
- [~] Compile the project in Unity and fix anything the logic tests cannot see. `python tools/unity-compile-check/check.py` compiles 84 of the 106 scripts against the installed Unity 6000.4.1 libraries with uGUI/TextMeshPro stand-ins and finds no errors, including `CodePickerSceneSetup`, `MainMenuSceneGenerator`, `MainMenuPanel`, `CodeProfilePickerPanel` and the panel sandbox scripts. Still needed in the Unity project itself: the 22 scripts that need the zSpace SDK, PrimeTween or Localization (input, legacy step code, `InspectionScenarioRunner`, `PanelDesignRunner`, two UI panels), and the exact uGUI and TextMeshPro shapes
- [ ] Panel sandbox scene with 3D panel, breaker tray, slot GameObjects
- [ ] Wire Quick Reference Cards to inspection HUD
- [ ] Certificate UI panel visual design
- [ ] Audio clips (SFX, ambient)
- [~] Main menu scene: `NEC Inspector > Scene Setup > Generate Main Menu Scene` (`MainMenuSceneGenerator`) builds and saves `Scenes/MainMenu/MainMenu.unity` (mode, scenario, difficulty, settings panels, wired to `MainMenuPanel`) and adds the code picker with `CodePickerSceneSetup`. The scene file does not exist until someone runs it in the Unity Editor; then add a ZCamera rig, check the layout for stylus/stereo, add the scene to Build Settings, and commit the `.unity` file. Neither editor tool has been compiled in Unity yet. Progress dashboard and reference panels are not in the menu scene yet
- [x] Main menu: explain an empty scenario list (PR #22). `MainMenuPanel` has an optional `_noScenariosText`, filled from `ScenarioListMessage` (no art for the code's region, no scenarios cover the code, or no code loaded). The scene generator adds and assigns it; existing scenes need the field assigned by hand
- [ ] Performance testing on zSpace hardware + Windows build

### Next: Alpha Content
- [ ] Scenario 4: Commercial Installation
- [ ] Scenario 5: Outdoor/Wet Location
- [ ] Virtual clamp meter tool
- [ ] Tutorial system
- [ ] Expand to 200+ NEC articles

### Future
See docs/CONTENT_ROADMAP.md and docs/COMPLETED_STEPS.md
