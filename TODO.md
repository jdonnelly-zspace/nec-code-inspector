# NEC Code Inspector - TODO

## Roadmap: a skill-based app that helps people get any electrician credential

Goal: a skill-based app that helps people build the skills to get an electrician credential (US, Canada, UK, EU, ...). **The app contains skills and installation codes, never credentials**: no credential names, blueprints, exam breakdowns or readiness scores ship in it (the tests enforce this). How skills line up with a credential is worked out outside the app (`docs/credential-alignment/`).
**Acceptance test:** a new installation code (Canada's CEC was the first) is added using only data files: a code profile folder, citations on the scenarios, and region-specific scenes where needed. If C# has to change, the abstraction has a leak.

Status key: `[x]` done and on `main`, `[~]` built and on `main` but still waiting on someone outside the code (the credential expert, or work in the Unity Editor), `[ ]` not started, `[!]` blocked or needs a decision.
**Update these statuses in the same PR as the work.**

**Where the work lives.** All of it is on `main`. Nothing has had a second person's review: since the stack landed, PRs were merged with an admin override because the author cannot approve their own PR (`gh pr list --state merged` lists every PR; this file does not), #3 to #8 were closed because #9 contained them, and PRs #10 to #30 were merged into the stack branch without review. The merged branches are deleted. `docs/REVIEW_GUIDE.md` maps the history for a reviewer. The app has three installation codes (the NEC, a draft CEC and a draft BS 7671 that has no art or tables yet), and its scenarios, difficulty settings, certificates, quick reference cards and sandbox design are data (JSON) with tests. The logic tests (`dotnet run --project tests/LogicTests`, .NET 8 SDK) pass locally and in CI.

### What is left, grouped by who can do it
- **Project owner and reviewers:** a second person to review everything merged with the admin override (`docs/REVIEW_GUIDE.md`, `docs/expert-review/PRE_REVIEW.md`); answer the open decisions below, including whether to keep the two new skills; decide the licensing status of each of the three codes with counsel (`docs/CONTENT_POLICY.md`, section 7).
- **Credential expert, UK-qualified for BS 7671** (start with `docs/expert-review/PRE_REVIEW.md`): confirm every row marked "Corrected in pre-review" against the 2026 NEC and C22.1:24; the doubts left unchanged (section 11); the two new skills (`wiring-methods`, `special-locations`) and the restored articles, especially Article 480 (medium-low confidence); every `scope.json` rule (CEC rows are draft, the 2026 NEC scope is unverified); the CEC rules for the violations still hidden under the CEC and for the two new skills; every BS 7671 entry and the edition (`docs/expert-review/5-articles-bs7671.csv`, section 14 of the pre-review; doubts: 421.1.7, 514.12, 544.1, 462, 722); the Red Seal task-to-skill mapping (C-17 is inferred); which edition the exam uses; and the receptacle spacing measurement.
- **Unity Editor** (the project has never been opened in the Editor): run `Generate Main Menu Scene` and commit the scene; add a ZCamera rig and check layouts; open the project once to compile the scripts that need the zSpace SDK, PrimeTween or Localization (a local check already compiles the rest against Unity 6000.4.1: `tools/unity-compile-check`); run NEC Inspector > Generate All Data (the scenarios, difficulty settings, certificates, quick reference cards and sandbox design now come from JSON, and these importers have never run in the Editor) and Generate Scenario Catalog, then open the quick reference card panel to check the per-code filter, and the panel sandbox (the HUD, breaker labels and compliance checks now take their wording, voltage and rule set from the active code, and the sandbox runner has not run since the rules became data); build the scenarios' 3D scenes and the North American art set; rename NEC-named MonoBehaviours with references visible.
- **Code, can start now:** running the scope checker on scene objects once scenes exist. Still US-shaped, and BS 7671 has no tables, so a real metric code cannot yet exercise them: the dwelling load method (`LoadCalculator` is shaped like Article 220), the left/right bus balance rule, and the split-phase meaning of single and double pole. UK-worded citations for the bathroom and dishwasher GFCI violations (the tests warn about both). Waiting on others: scene facts for the two wire and breaker violations need a CEC conductor table; battery violations need the expert's Article 480 numbering; CEC tables need a CSA licence.
- **Blocked on a decision:** whether a region with several codes needs a way to choose between them (today the reviewed, then app-defined, then newest-by-id code wins); whether certificates stay in the UI (see open decision 5); Unity compile in CI; first code after Canada; localization; which menu panels beyond the five built.

### Phase A - Foundations (refactor steps 1-4)
- [x] A1. Code-neutral concept IDs on violations
- [x] A2. `ICodeProfile`, `CodeArticle`, `CodeProfiles` registry; `NECDatabase` is the NEC profile
- [x] A3. Electrical tables and compliance rule settings as profile data
- [x] A4. Scenario content in JSON with a validating importer

### Phase B - Verify, then finish the data layer (current)

**B1. CI and tests**
- [x] Logic test project (`tests/LogicTests`) and GitHub Actions workflow (`.github/workflows/logic-tests.yml`): tables JSON matches built-in defaults, load calculator parity, citation matching, scenario and article JSON validation
- [x] Fix compile error found by a local compile: `QuickReferenceCardPanel` used `NECReferencePanel` without `using NECInspector.Inspection`
- [x] Move `ViolationSeverity` into its own file and share the scenario file validator between the importer and the tests
- [x] The PR stack (#2 to #9) and the docs PR #1 are on `main` (admin merge; #3 to #8 closed because #9 contained them). Still open as a follow-up: an independent review of the merged code
- [x] The 13 dangling `related` references are resolved (pre-review: 240.6 added, the other links removed); the logic tests warn only about the two CEC skill-coverage gaps now
- [x] Tests for the 10 panel compliance rules: the rules moved into `ComplianceRules` (plain data in, results out, tables passed in) and `ComplianceChecker` only reads the scene objects into that data (52 checks in `ComplianceRulesTests`; boundary mutations were confirmed to fail them). The reader in `ComplianceChecker` still needs a Unity compile check; the public `Check*` methods it used to have were only called from inside the class and are gone
- [x] Document how to run the tests locally (CONTRIBUTING.md > Tests); `tests/**/bin` and `obj` are git-ignored
- [!] Unity compile gate in CI (a local check now exists: `tools/unity-compile-check`). Blocked: the repo has no `ProjectSettings/` or `Packages/`, the zSpace SDK is proprietary and git-ignored, and CI would need a Unity license secret. Decision needed on how to make the project compilable in CI (stub the SDK, or a self-hosted runner)

**B2. Finish the data layer**
- [x] Per-profile citations on violations: `citations` list (profileId, reference, text) replaces `necArticle` / `necArticleText`; scoring compares the student's citation to the active profile's reference
- [x] Concept-aware scoring: every code entry carries a `conceptId`; citing another entry of the violation's skill earns 0.75 of the evidence (full credit 1, other wrong citation 0.5, missed 0). `CitationCredit`, `docs/SKILL_SCORING.md`, every NEC and CEC entry tagged, tests that every violation's citations belong to its skill. The 0.75 is a policy choice for the owner to change in `SkillOutcomes`; the expert confirms the tags (worksheet column "Skill tag")
- [x] Skill-based progress (decision: progress is per skill, never per credential). Evidence from inspections and the sandbox updates a running mastery per skill and tier; a skill is attained at 3+ attempts and 80% mastery; a higher tier covers a lower one
- [x] Credentials removed from the app (decision: it is a skill-based app that helps people get the credential). Deleted credential profiles, readiness, deferred requirements and the credential files; progress is per skill and the student's chosen installation code is stored in the progress file and applied at startup. Alignment notes live outside the app in `docs/credential-alignment/`
- [x] Certificates are earned from skills (`requiredSkills` + `requiredTier`) instead of NEC chapters; the dead chapter-mastery code is removed; the dashboard shows skill mastery
- [ ] Alignment notes outside the app for other credentials (NCCER Electrical, ...), drafted in our own words and reviewed by a credential expert; the Red Seal 309A draft is in `docs/credential-alignment/`
- [~] The student's chosen installation code is stored in the progress file and applied at startup (`ProgressManager.SetActiveCodeProfile`). The picker screen is built (`CodeProfilePickerPanel`, see Phase D); it still has to be wired into the main menu scene in the Unity Editor
- [~] Skill taxonomy: `wiring-methods` and `special-locations` were added (reasons in `docs/credential-alignment/README.md`; the expert reviews them). Motors or hazardous locations may need sub-skills later, each with a documented reason
- [x] An alignment report outside the app: `python tools/alignment-report/report.py PROGRESS.json ALIGNMENT.json` reads the progress file against an alignment file and shows what is attained, what the app can still teach, and what waits on content (unit tests run in CI)
- [x] Update `docs/CREDENTIAL_FRAMEWORK.md` (PR #1): rewritten to the skill-based design (PR #17, merged into PR #1's branch). It describes the stacked branches, so keep it in step as they merge
- [x] Violation and scenario applicability, derived from citations: a violation applies to a profile only if it has a citation for it; a scenario applies if any violation does. `InspectionManager` skips non-applicable violations and the main menu hides non-applicable scenarios
- [x] Rename leftover NEC-named API from the citation change (`FlagViolation(... necArticle)`, `citedNECArticle`, `FlaggedNECArticle`): done in Phase C
- [x] Editor-script content moved to JSON: difficulty settings, certificate templates, quick reference cards and the panel sandbox design are under `Assets/_Project/Content` with validators and tests (`ContentFileData`, `ContentFilesTests`); the four generators read them. Needs a run in the Unity Editor (`Generate All Data`) and the card panel check

### Phase C - Neutral wording and units (old refactor step 5)
- [x] Neutral names: `NECCitationMode` -> `CitationMode`, `highlight2026Changes` -> `highlightNewInEdition`, `isNewIn2026` -> `isNewInEdition` (JSON and DTO), `necReference(s)` -> `codeReference(s)`, `dwellingSquareFootage` -> `dwellingArea`, `citedNECArticle` / `FlaggedNECArticle` -> `citedReference` / `FlaggedReference`. Serialized fields keep their old names through `FormerlySerializedAs`
- [x] Scenario sections are derived from the violations' citations in the active profile; the stored NEC `necChapters` lists (scenario and panel definitions) are removed
- [x] Terminology per profile (`CodeTerminology`, `terminology.json`): code name, reference format, section names, dropdown prompt and vocabulary (grounding/earthing, panel, breaker, ...). UI strings ("NEC Citations", "NEC Chapters", "Art.", "Chapter") now come from the active profile, and `{code}` / `{term:key}` tokens work in UI text
- [x] Units and defaults from the profile tables: area unit label, default conductor, single-pole voltage for measurements; the HUD no longer prints "Art. General Practice"
- [~] `{term:key}` and `{code}` tokens in scenario and violation text are filled from the active code's terminology (`ViolationDefinitionSO` getters, scenario title and description in the menu and intro step). One violation (`BC-GFCI-BATH-001`) uses them as the working example, and a test checks every key used exists in the NEC terms and warns when another code lacks it. Still to do: convert more text when a code with different words arrives (UK earthing, consumer unit)
- [~] Metric profile scaffold: a synthetic 230 V, mm2 code (`tests/fixtures/codes/demo-metric`, never shipped) runs through the engine in `MetricProfileTests`: tables, units, terminology, compliance rules, picker and scenario list. It found and fixed the US wording that was hard-coded in the panel rules (GFCI, AFCI, breaker, panel, the 240 V label, wire gauge): they now use terminology tokens, with a new capitalised `{Term:key}` token. Still US-shaped and not generalised: the dwelling load method (`LoadCalculator` is Article 220: small-appliance and laundry circuits, dryer and range, demand tiers), the left/right bus balance rule (a profile can switch it off), and the split-phase meaning of single and double pole. BS 7671 now exists as a draft but ships no tables (copyrighted), so real metric tables are still untested
- [x] NEC wording moved out of editor-script content into JSON (see above); the overall certificate no longer names the NEC, and cards carry the code they are written for
- [x] Difficulty levels are generic: Beginner, Standard and Expert describe what the student can do (building familiarity, working knowledge, full fluency), with no pathway or credential-body names; a test keeps those words out of `DifficultyLevel.cs` and `MainMenuPanel.cs`. The out-of-app notes `docs/CERTIFICATION_BODIES.md` and `docs/PROJECT_BRIEF.md` still use the old labels (CTE, apprentice, licensed); update them if they are shared
- [ ] Rename MonoBehaviour classes, scene-serialized fields and files that carry NEC names (`NECReferencePanel`, `NECReviewStep`, `_necDropdown`, ...). Deferred on purpose: it can break scene and prefab references, so do it in the Unity Editor with the references visible
- [ ] Decide on renaming the `NECInspector` namespace (cosmetic, defer)

### Phase D - Prove it with Canada (CEC)
- [~] CEC code profile (`StreamingAssets/Codes/cec/`, draft, partial): entries explained in our own words from public sources, numbered for C22.1:24 (the pre-review found the first draft used older numbers), terminology (Rule, Section), no tables. **A credential expert must check every entry**
- [x] Red Seal 309A draft moved out of the app to `docs/credential-alignment/red-seal-309a-draft.json` (not shipped). It maps the exam tasks it covers to the app's skills by estimate; the report in `tools/alignment-report` says how much of that the app can teach under the CEC
- [~] CEC citations on the violations that have a verified CEC rule, with per-code wording (metres, 1.8 m, 900 mm, 3 m rods, 1 m working space, 1.7 m handle height, 9 m disconnect) where the shared text carries NEC numbers. The rest are hidden when the CEC is active
- [x] Acceptance test: Canada was added with data files (profile folder and citations); the profile tests discover new folders without any test code. The code changes it needed are listed below as defects found in the abstraction
- [ ] **CEC content expansion** (needs verified rules, expert help): the violations hidden under the CEC have no verified CEC rule, only a section, or a scene that would differ materially (reasons in `docs/expert-review/PRE_REVIEW.md` section 7). Conductor sizing, load calculation, equipment installation and identification and marking still have no CEC violations; shock protection has only Foundation-level ones. The skill coverage test reports these gaps as warnings
- [ ] CEC content for `wiring-methods` and `special-locations` (CEC Section 12 and the pool and battery rules; no verified numbers yet), and verification of every `scope.json` rule by the expert (`Codes/nec/scope.json` is from the 2023 NEC; `Codes/cec/scope.json` is draft)
- [x] GFCI/AFCI scope checker: `scope.json` per code, `ProtectionScopeChecker` and its validator, `sceneScope` on six violations, tests that each scene breaks every code it cites and warnings for codes that could carry a citation (see `docs/SCENE_DESIGN.md`)
- [!] CEC tables (conductor ampacities, demand factors) so the panel sandbox can count toward the CEC. Blocked: CSA's tables are copyrighted; needs a licensed C22.1:24 and written reproduction permission from CSA, then an electrician's cross-check. The sandbox is gated by `HasOwnTables` until then
- [~] Red Seal task-to-skill mapping: task numbers, titles and question counts were confirmed against the occupational standard in the pre-review, and the mapping was extended (D-22 conductor sizing, D-24 overcurrent protection, B-7 earthing and bonding). The expert still confirms it, especially the inferred C-17 link; the app can teach about 24 of the 48 mapped questions' worth under the CEC
- [ ] **Licensing decision (owner and counsel):** clear the NEC and the CEC for release, or obtain permission, and record it in each `profile.json` (`license.status`, `evidence`, `checkedOn`). CSA's tables need a separate licence. Until then both codes are `unreviewed`
- [~] Region picker in the UI: students choose a region (United States, Canada, United Kingdom) and the app applies that region's installation code without naming it (`RegionChoices`, `ProgressManager.SetRegion`, `profile.json` `region` and `units`). `CodeProfilePickerPanel` (class name kept so scenes keep working) lists regions with their units, scenario coverage and a sandbox note; `MainMenuPanel` shows "Region: ...". Student-facing text, content and entries name no code (tested). **Needs Unity Editor work** (see Alpha, Active), and the picker class, `CodePickerSceneSetup` and `_activeCodeText` could be renamed for region when the scenes are open

Defects found in the abstraction while adding Canada (all fixed here unless noted):
- The NEC was a C# class, so a second code needed code. Fixed once: `DataCodeProfile` and `CodeProfileLibrary` load any folder under `StreamingAssets/Codes/`; the NEC data moved to `Codes/nec/` and is loaded the same way
- Shared violation wording carried NEC numbers (feet, 36 in, 8 ft rods): citations can now override description, hint and note per code
- (Removed again) Credentials could require skills a code's content does not cover, so requirements got a `deferred` flag; this went away when credentials left the app. The skill coverage test now reports the same gaps per code
- Sandbox results were always checked against NEC tables: sandbox evidence now needs the active code to have its own tables
- The citation matcher accepted `26-70` for `26-700`: it now matches only at a level boundary (this also closes the matching item in Phase F)
- Facts differ between codes (GFCI scope, rod length and count, spacing limits), so a scene must break the rule under every code it is offered for, or be shown to one code only. Rules accepted in `docs/SCENE_DESIGN.md` (margin, pass-both, split when incompatible, SI units, automatic check). Done: measured values, limits and the logic test for the four single-number violations (wall and counter spacing, ground rod, working space), and the counter gap violation redone (`SceneFacts`). Still to do: shared violations whose rules are tables or scope (wire sizes and breaker ratings, GFCI and AFCI areas) need a different kind of check than a single limit
- [~] Scene facts for the violations shared by the NEC and CEC: the four numeric ones carry scene values and limits, and the GFCI and AFCI ones describe their receptacle and are checked against each code's `scope.json`. Still to do: `BC-WIRE-14AWG-001` and `RP-WIRE-OVER-001` (wire and breaker pairs need a conductor table the CEC does not have yet)

### Phase E - Hard problems (decide early)
- [x] Decision: region-specific scene art and prefab variants: accepted in `docs/SCENE_DESIGN.md` (one art set per region group, North American set first, other groups blocked until a second code is committed). Done: `artSet` in `profile.json` with validation (`ArtSets`), and the menu and the picker offer no scenarios under a code whose art set has no art (`ArtSets.IsAvailable`)
- [ ] Build the North American art set (Unity Editor): panels, receptacles, breakers, wire colours. Scenes must follow the accepted scene design rules: scene values break every listed code's limit by 10%, compliant parts pass every offered code
- [~] Expert to confirm both spacing violations measure from the farthest point on the wall line to the nearest receptacle: public sources describe both codes that way (medium-high, pre-review section 4); the counter scene was redrawn on that reading
- [ ] Generalize or add a panel sandbox model beyond split-phase US panels (UK consumer units with RCD/RCBO, 230 V single phase). BS 7671 is gated out of the sandbox until it has tables, which need a licence
- [x] Pluggable compliance rule sets per profile: a profile's `tables.json` lists its rules with a `kind`, id, name, citation, skill and parameters (`maxRatio`, `margin`, `maxImbalance`, `protection`); the same kind can run twice, rules run in the profile's order, and the validator rejects unknown kinds (`docs/COMPLIANCE_RULES.md`, `ComplianceRuleSetTests`). A new kind of check still needs C#, and the left/right bus rule and the meaning of single and double pole stay North American-shaped

### Phase F - Governance
- [x] Paraphrase pass: all 42 violation citation texts and 98 article texts rewritten in new words (facts and references kept, shorter than the source, no six-word runs copied); the wording check in `ContentPolicyTests` now fails on any statutory wording. Notes for the reviewer are in `docs/CONTENT_REVIEW.md`
- [~] **Credential expert review**: a desk check against public sources (`docs/expert-review/PRE_REVIEW.md`) confirmed and corrected the seven suspected problems and more, with confidence levels; the expert now confirms the corrected rows against the 2026 NEC and C22.1:24, and answers the items in section 11 of the pre-review. Packet in `docs/expert-review/` (worksheets regenerated with `tools/make_expert_review.py`)
- [~] Paraphrase check on the moved text: tests reject statutory wording and unverified 2026 claims in the cards, certificates and sandbox text; an expert still reviews the wording (`docs/expert-review/4-cards-and-sandbox.csv`)
- [x] `Codes/nec/articles.json`: the pre-review dropped 15 articles that needed skills the app lacked; at the owner's request they were restored (corrected) together with two new skills, `wiring-methods` and `special-locations`. Article 480 (storage batteries) is medium-low confidence and has no violations yet
- [x] Licensing status per profile: `license` block in `profile.json` (status, holder, note, evidence, date), validated, with a warning in the tests and in the editor for any code not cleared (`docs/CONTENT_POLICY.md`, section 7). `nec` and `cec` are `unreviewed`: **the owner and counsel decide** (own-words, or a licence) before release
- [x] Content policy documented and enforced: content only for skills in `ConceptIds`, no credentials or credential code in the app (both fail the tests), and statutory wording fails the tests
- [x] Skill coverage per installation code: the tests fail if the NEC lacks Practitioner-level content for any skill and warn about gaps in other codes. A report that compares coverage with an alignment file belongs outside the app
- [x] Naming: public constants and static data (`ConceptIds.ShockProtection`, `ArtSets.NorthAmerica`) are PascalCase as in the rest of C#; private and local constants stay UPPER_SNAKE_CASE (`SCENE_PATH`). `.claude/CLAUDE.md` now says so
- [x] Citation matcher is segment-aware (`250.2` no longer matches `250.24`, `26-70` no longer matches `26-700`) (Phase D)

### Phase G - Later: UK and Europe
- [~] BS 7671 profile (UK; metric conductors, 230 V, different terminology) as the stress test: a draft with 29 own-words entries, UK terminology, an RCD scope file and nine UK-worded violation citations (`StreamingAssets/Codes/bs7671`, `docs/expert-review/PRE_REVIEW.md` section 14). No tables (copyrighted), no UK art, so it offers no scenarios and no sandbox. Needs a UK-qualified expert, the licence decision, UK art (art set `uk`), and UK violations for seven skills (arc-fault, overcurrent, conductor sizing, branch circuits, load calculation, working space, wiring methods). A code with no art is a test warning, not a failure
- [ ] France (NF C 15-100), Germany (DIN VDE 0100) and other HD 60364 national codes
- [ ] Decide whether UI language localization (French, German) is in scope

### Open decisions
1. How to get a Unity compile in CI (Phase B1)
2. First installation code after Canada, if a customer is waiting on the UK or EU
3. Language localization in or out of scope
4. Which panels beyond mode, scenarios, difficulty, settings and code picker go into the menu scene (progress dashboard, reference panels)
5. Certificates: they are earned from skills (`Content/Certificates`, shown on the progress dashboard), but a "certificate" can read as a credential. Keep as is, rename (for example skill milestones), or remove from the UI. The wireframe already leaves them out

Decided: region-specific art is one set per region group (`docs/SCENE_DESIGN.md`); citing another entry of the violation's skill earns 0.75 (`docs/SKILL_SCORING.md`).

## Current Phase: Alpha - Unity Editor Integration

### Active (Unity Editor Required)
- [ ] Boot scene (not started) and MainMenu scene (generator below; the scene file still has to be generated and committed)
- [ ] Residential panel 3D scene with embedded violations + prefab wiring
- [ ] Wiring methods and special locations 3D scenes (`WiringMethodsInspection`, `SpecialLocationsInspection`): framed house with cables and boxes, a trench, an overhead feeder, a backyard pool; re-run Import Scenario Data and Generate Scenario Catalog after adding the scenario files
- [ ] Kitchen/bathroom/living area 3D scene (Branch Circuit scenario)
- [ ] Run NEC Inspector > Generate All Data in the Editor (scenarios, difficulty settings, certificates, quick reference cards, sandbox design); run it after any change to the files under `Assets/_Project/Content`
- [~] Compile the project in Unity and fix anything the logic tests cannot see. `python tools/unity-compile-check/check.py` compiles every script it can against the installed Unity 6000.4.1 libraries with uGUI/TextMeshPro stand-ins and finds no errors, including `CodePickerSceneSetup`, `MainMenuSceneGenerator`, `MainMenuPanel`, `CodeProfilePickerPanel`, the panel sandbox scripts and the scope checker. Still needed in the Unity project itself: the scripts that need the zSpace SDK, PrimeTween or Localization (input, legacy step code, `InspectionScenarioRunner`, `PanelDesignRunner`, two UI panels), and the exact uGUI and TextMeshPro shapes
- [ ] Panel sandbox scene with 3D panel, breaker tray, slot GameObjects
- [ ] Wire Quick Reference Cards to inspection HUD
- [ ] Certificate UI panel visual design
- [ ] Audio clips (SFX, ambient)
- [~] Main menu scene: `NEC Inspector > Scene Setup > Generate Main Menu Scene` (`MainMenuSceneGenerator`) builds and saves `Scenes/MainMenu/MainMenu.unity` (mode, scenario, difficulty, settings panels, wired to `MainMenuPanel`) and adds the code picker with `CodePickerSceneSetup`. The scene file does not exist until someone runs it in the Unity Editor; then add a ZCamera rig, check the layout for stylus/stereo, add the scene to Build Settings, and commit the `.unity` file. Both editor tools compile against Unity 6000.4.1's libraries (with uGUI and TextMeshPro stand-ins) but have never been run in the Editor. Progress dashboard and reference panels are not in the menu scene yet
- [x] Main menu: explain an empty scenario list (PR #22). `MainMenuPanel` has an optional `_noScenariosText`, filled from `ScenarioListMessage` (no art for the code's region, no scenarios cover the code, or no code loaded). The scene generator adds and assigns it; existing scenes need the field assigned by hand
- [ ] Performance testing on zSpace hardware + Windows build

### Next: Alpha Content
- [~] Scenario 4: Commercial Installation: the data exists (`commercial.json`); the 3D scene does not
- [~] Scenario 5: Outdoor/Wet Location: covered in data by the Special Locations scenario (`special-locations.json`: pool luminaire height, pool bonding, weatherproof cover) and the Wiring Methods scenario; more violations (spas, outdoor GFCI scope, batteries) wait on the expert's numbering; no 3D scene yet
- [ ] Virtual clamp meter tool
- [ ] Tutorial system
- [ ] Expand the NEC entries in `Codes/nec/articles.json`, only for topics inside the app's skills (content policy)

### Ideas (not started)
- [ ] Wireframe of the UI panels: a first clickable HTML version exists (published as an artifact, not in the repo). Ideas for later: a Figma version if a designer takes over the screens, a Lucid screen-flow diagram (menu, picker, scenario, inspection, review, sandbox), and an inline widget for quick previews in chat. Any of these would need the Unity scenes built to show the real 3D layouts

### Future
See docs/CONTENT_ROADMAP.md and docs/COMPLETED_STEPS.md
