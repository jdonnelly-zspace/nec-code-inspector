# NEC Code Inspector - TODO

## Credential-Agnostic Roadmap

Goal: the app works for any electrician credential (US, Canada, UK, EU, ...) by adding data, not code.
**Acceptance test:** a new credential (starting with Canada's Red Seal) is added using only data files (code profile, credential profile, scenario JSON, and region-specific scenes where needed). If C# has to change, the abstraction has a leak.

Status key: `[x]` done and merged, `[~]` built and in review (PR open), `[ ]` not started, `[!]` blocked or needs a decision.
**Update these statuses in the same PR as the work.** Background: the credential framework design is in PR #1 (`docs/CREDENTIAL_FRAMEWORK.md`).

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
- [ ] Merge the PR stack in order: #2, #3, #4, #5 (retarget each to `main` as the one below merges), plus the docs PR #1 and the B-phase PR
- [ ] Fix the 13 dangling `related` references in `Codes/nec/articles.json` (reported as warnings by the tests; the reference panel silently skips them)
- [ ] Tests for the 10 `ComplianceChecker` rules (needs the rules separated from the MonoBehaviours and ScriptableObjects they read)
- [~] Document how to run the tests locally (CONTRIBUTING.md > Tests)
- [!] Unity compile gate in CI. Blocked: the repo has no `ProjectSettings/` or `Packages/`, the zSpace SDK is proprietary and git-ignored, and CI would need a Unity license secret. Decision needed on how to make the project compilable in CI (stub the SDK, or a self-hosted runner)

**B2. Finish the data layer**
- [~] Per-profile citations on violations: `citations` list (profileId, reference, text) replaces `necArticle` / `necArticleText`; scoring compares the student's citation to the active profile's reference (this branch)
- [ ] Concept-aware scoring: let a citation in the same concept count (for example 210.8(A)(1) vs 210.8(A)(5)). Needs article-to-concept tags in each profile's article data and a policy decision on how much credit it earns
- [~] Skill-based progress (decision: progress is per skill, never per credential). Evidence from inspections and the sandbox updates a running mastery per skill and tier; a skill is attained at 3+ attempts and 80% mastery; a higher tier covers a lower one (this branch)
- [~] Credential profile as a view over skills: `StreamingAssets/Credentials/*.json` lists the skills and tiers a credential requires, its code profile, and its tier names; readiness is computed from skill progress. Ships with one app-defined credential, `core-skills` (this branch)
- [~] Certificates are earned from skills (`requiredSkills` + `requiredTier`) instead of NEC chapters; the dead chapter-mastery code is removed; the dashboard shows skills and credential readiness (this branch)
- [~] Real credential files, drafted from the research and reviewed by a credential expert: Red Seal 309A drafted (Phase D); NCCER Electrical still to do
- [~] Active credential is stored in the progress file and selects the code profile at startup (Phase D); the dashboard shows readiness for it. A picker screen is still missing
- [ ] Finer skill taxonomy: the 11 concepts are the skills for now; credentials such as motors or hazardous locations may need sub-skills
- [ ] Credential-specific readiness reports and certificates (a printable view of a credential's requirements against skills)
- [ ] Update `docs/CREDENTIAL_FRAMEWORK.md` (PR #1) with the skill-based progress decision
- [~] Violation and scenario applicability, derived from citations: a violation applies to a profile only if it has a citation for it; a scenario applies if any violation does. `InspectionManager` skips non-applicable violations and the main menu hides non-applicable scenarios (this branch)
- [~] Rename leftover NEC-named API from the citation change (`FlagViolation(... necArticle)`, `citedNECArticle`, `FlaggedNECArticle`): done in Phase C (this branch)
- [ ] Move remaining editor-script content to JSON: panel sandbox circuits, quick-reference cards, certificate templates, difficulty settings

### Phase C - Neutral wording and units (old refactor step 5)
- [~] Neutral names (this branch): `NECCitationMode` -> `CitationMode`, `highlight2026Changes` -> `highlightNewInEdition`, `isNewIn2026` -> `isNewInEdition` (JSON and DTO), `necReference(s)` -> `codeReference(s)`, `dwellingSquareFootage` -> `dwellingArea`, `citedNECArticle` / `FlaggedNECArticle` -> `citedReference` / `FlaggedReference`. Serialized fields keep their old names through `FormerlySerializedAs`
- [~] Scenario sections are derived from the violations' citations in the active profile; the stored NEC `necChapters` lists (scenario and panel definitions) are removed
- [~] Terminology per profile (`CodeTerminology`, `terminology.json`): code name, reference format, section names, dropdown prompt and vocabulary (grounding/earthing, panel, breaker, ...). UI strings ("NEC Citations", "NEC Chapters", "Art.", "Chapter") now come from the active profile, and `{code}` / `{term:key}` tokens work in UI text
- [~] Units and defaults from the profile tables: area unit label, default conductor, single-pole voltage for measurements; the HUD no longer prints "Art. General Practice"
- [ ] Use `{term:key}` tokens in scenario and violation text so one scenario reads correctly for each code (grounding vs earthing)
- [ ] Metric profile data: mm2 conductor table, 230/400 V, m and m2 labels (needed with the first non-NEC profile, Phase D/G)
- [ ] Move the remaining NEC wording out of editor-script content (quick-reference cards, panel sandbox descriptions, certificate text) into JSON content files (see B2)
- [ ] Rename MonoBehaviour classes, scene-serialized fields and files that carry NEC names (`NECReferencePanel`, `NECReviewStep`, `_necDropdown`, ...). Deferred on purpose: it can break scene and prefab references, so do it in the Unity Editor with the references visible
- [ ] Decide on renaming the `NECInspector` namespace (cosmetic, defer)

### Phase D - Prove it with Canada (CEC + Red Seal)
- [~] CEC code profile (`StreamingAssets/Codes/cec/`, draft, partial): 7 entries explained in our own words from public summaries, terminology (Rule, Section), no tables yet. Edition numbering differs (older editions use 26-722, newer 26-712); **a credential expert must check every entry against the edition the Red Seal exam uses**
- [~] Red Seal Construction Electrician (309A) credential (draft): built from the official exam breakdown; 8 tasks (48 of 100 questions) mapped to the app's skills by estimate. 3 skills are teachable under the CEC at Practitioner level today; 8 are marked `deferred`. The app covers about 13 of the 100 questions' worth so far
- [~] CEC citations on 9 of the 42 violations, with per-code wording (metres, 1.8 m, 900 mm, 3 m rods, 1 m working space) where the shared text carries NEC numbers. The other 33 violations are hidden when the CEC is active
- [~] Acceptance test: Canada was added with data files (profile folder, credential, citations); the profile and credential tests discover the new folders without any test code. The code changes it needed are listed below as defects found in the abstraction
- [ ] **CEC content expansion** (needs verified rules, expert help): conductor sizing, load calculation, disconnecting means, equipment installation, identification and marking have no CEC content; shock protection, earthing and bonding, and working space only have Foundation-level violations. Until then the Red Seal credential cannot be completed in the app
- [ ] CEC tables (conductor ampacities, demand factors) so the panel sandbox can count toward the Red Seal credential; the sandbox is gated by `HasOwnTables` until then
- [ ] Confirm the Red Seal task-to-skill mapping and weights with a credential expert; read the full 309A occupational standard (the PDF could not be read here, only the exam breakdown page)
- [ ] CSA licensing check before shipping CEC content (see Phase F)
- [ ] Credential picker in the UI: `ProgressManager.SetActiveCredential` already stores the choice and switches the code profile; there is no screen to call it

Defects found in the abstraction while adding Canada (all fixed here unless noted):
- The NEC was a C# class, so a second code needed code. Fixed once: `DataCodeProfile` and `CodeProfileLibrary` load any folder under `StreamingAssets/Codes/`; the NEC data moved to `Codes/nec/` and is loaded the same way
- Shared violation wording carried NEC numbers (feet, 36 in, 8 ft rods): citations can now override description, hint and note per code
- A credential can require skills the code's content does not cover: requirements can be `deferred`, readiness counts only what the app can teach and reports coverage
- Sandbox results were always checked against NEC tables: sandbox evidence now needs the active code to have its own tables
- The citation matcher accepted `26-70` for `26-700`: it now matches only at a level boundary (this also closes the matching item in Phase F)
- Facts differ between codes (GFCI scope, rod length and count, spacing limits), so a scene must break the rule under every code it is offered for, or be shown to one code only. Not solved: needs the scene-design guidance from Phase E

### Phase E - Hard problems (decide early)
- [!] Decision: region-specific scene art and prefab variants. No 3D scenes exist in the repo yet (only `.gitkeep`), so decide before building them: US panels, receptacles and wire colors will not suit EU or UK installations
- [ ] Generalize or add a panel sandbox model beyond split-phase US panels (UK consumer units with RCD/RCBO, 230 V single phase)
- [ ] Pluggable compliance rule sets per profile (rule logic is still in C#; only numbers, citations and on/off are data)

### Phase F - Governance
- [~] Paraphrase pass (this branch): all 42 violation citation texts and 98 article texts rewritten in new words (facts and references kept, shorter than the source, no six-word runs copied); the wording check in `ContentPolicyTests` now fails on any statutory wording. Notes for the reviewer are in `docs/CONTENT_REVIEW.md`
- [ ] **Credential expert review** of the paraphrases for accuracy and edition (2026), and of the suspected reference problems in `docs/CONTENT_REVIEW.md` (two swapped citation pairs, a mismatched load-calc citation, `210.8(A)` numbering)
- [ ] Run the same paraphrase check on the text in editor scripts (quick-reference cards, sandbox descriptions, certificates) when it moves to JSON
- [ ] Trim `Codes/nec/articles.json` to articles that support skills required by a credential: 43 of 98 are not tied to any current violation (list in `docs/CONTENT_REVIEW.md`); decide which to keep
- [ ] Licensing status per profile (NEC, CEC, BS 7671 are copyrighted); record it in the profile
- [~] Content policy documented and enforced: scope (content only for skills a credential requires) fails the tests; wording is reported as a warning (this branch)
- [ ] Coverage report: each credential's blueprint vs the concepts and scenarios that cover it
- [ ] Naming: `ConceptIds` constants are PascalCase but `.claude/CLAUDE.md` says UPPER_SNAKE_CASE; decide and align
- [x] Citation matcher is segment-aware (`250.2` no longer matches `250.24`, `26-70` no longer matches `26-700`) (Phase D)

### Phase G - Later: UK and Europe
- [ ] BS 7671 profile (UK; metric conductors, 230 V, different terminology) as the stress test
- [ ] France (NF C 15-100), Germany (DIN VDE 0100) and other HD 60364 national codes
- [ ] Decide whether UI language localization (French, German) is in scope

### Open decisions
1. Region-specific scenes vs one adaptive scene (Phase E)
2. How to get a Unity compile in CI (Phase B1)
3. First credential after Canada, if a customer is waiting on the UK or EU
4. Language localization in or out of scope
5. Scoring policy for concept-level credit (Phase B2)

## Current Phase: Alpha - Unity Editor Integration

### Active (Unity Editor Required)
- [ ] Boot + MainMenu scenes
- [ ] Residential panel 3D scene with embedded violations + prefab wiring
- [ ] Kitchen/bathroom/living area 3D scene (Branch Circuit scenario)
- [ ] Run NEC Inspector > Import Scenario Data + PanelDesignSandboxGenerator in Editor
- [ ] Panel sandbox scene with 3D panel, breaker tray, slot GameObjects
- [ ] Wire Quick Reference Cards to inspection HUD
- [ ] Certificate UI panel visual design
- [ ] Audio clips (SFX, ambient)
- [ ] Performance testing on zSpace hardware + Windows build

### Next: Alpha Content
- [ ] Scenario 4: Commercial Installation
- [ ] Scenario 5: Outdoor/Wet Location
- [ ] Virtual clamp meter tool
- [ ] Tutorial system
- [ ] Expand to 200+ NEC articles

### Future
See docs/CONTENT_ROADMAP.md and docs/COMPLETED_STEPS.md
