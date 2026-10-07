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
- [ ] Fix the 13 dangling `relatedArticles` references in `nec_articles.json` (reported as warnings by the tests; the reference panel silently skips them)
- [ ] Tests for the 10 `ComplianceChecker` rules (needs the rules separated from the MonoBehaviours and ScriptableObjects they read)
- [~] Document how to run the tests locally (CONTRIBUTING.md > Tests)
- [!] Unity compile gate in CI. Blocked: the repo has no `ProjectSettings/` or `Packages/`, the zSpace SDK is proprietary and git-ignored, and CI would need a Unity license secret. Decision needed on how to make the project compilable in CI (stub the SDK, or a self-hosted runner)

**B2. Finish the data layer**
- [ ] Per-profile citations on violations: replace `necArticle` / `necArticleText` with a citation map, and score by concept + active profile
- [ ] Credential profile: tier names and mapping per credential, topic weights, exam style, code profile ID
- [ ] Record progress and certificates per credential
- [ ] Scenario and violation applicability (`appliesTo` profiles), since some rules are code-specific
- [ ] Move remaining editor-script content to JSON: panel sandbox circuits, quick-reference cards, certificate templates, difficulty settings

### Phase C - Neutral wording and units (old refactor step 5)
- [ ] Rename NEC-specific names: `NECCitationMode`, `necChapters`, `highlight2026Changes`, `isNewIn2026` JSON field, and UI strings such as "NEC Citations" and "NEC Chapters"
- [ ] Terminology table per profile (grounding/earthing, panel/consumer unit, breaker/MCB)
- [ ] Unit system: AWG or mm2, ft or m, 120/240 V or 230/400 V; fix remaining hard-coded defaults (`WireConnection` "12 AWG", `Multimeter` 120 V, HUD "Art." prefix)
- [ ] Decide on renaming the `NECInspector` namespace (cosmetic, defer)

### Phase D - Prove it with Canada (CEC + Red Seal)
- [ ] CEC code profile: article data and electrical tables (CEC, CSA C22.1)
- [ ] Red Seal Construction Electrician (309A) credential profile and tier mapping
- [ ] Adapt or add scenarios for CEC; confirm applicability rules work
- [ ] CSA licensing check before storing CEC text (see Phase F)
- [ ] Acceptance test: Canada added by data only; record any code changes it needed as defects in the abstraction

### Phase E - Hard problems (decide early)
- [!] Decision: region-specific scene art and prefab variants. No 3D scenes exist in the repo yet (only `.gitkeep`), so decide before building them: US panels, receptacles and wire colors will not suit EU or UK installations
- [ ] Generalize or add a panel sandbox model beyond split-phase US panels (UK consumer units with RCD/RCBO, 230 V single phase)
- [ ] Pluggable compliance rule sets per profile (rule logic is still in C#; only numbers, citations and on/off are data)

### Phase F - Governance
- [ ] Licensing status per profile (NEC, CEC, BS 7671 are copyrighted); store clause numbers and paraphrased text unless licensed; check the verbatim NEC text in `nec_articles.json` and `necArticleText`
- [ ] Coverage report: each credential's blueprint vs the concepts and scenarios that cover it
- [ ] Naming: `ConceptIds` constants are PascalCase but `.claude/CLAUDE.md` says UPPER_SNAKE_CASE; decide and align
- [ ] Citation matcher accepts string prefixes (`250.2` matches `250.24`); tighten to segment-aware matching

### Phase G - Later: UK and Europe
- [ ] BS 7671 profile (UK; metric conductors, 230 V, different terminology) as the stress test
- [ ] France (NF C 15-100), Germany (DIN VDE 0100) and other HD 60364 national codes
- [ ] Decide whether UI language localization (French, German) is in scope

### Open decisions
1. Region-specific scenes vs one adaptive scene (Phase E)
2. How to get a Unity compile in CI (Phase B1)
3. First credential after Canada, if a customer is waiting on the UK or EU
4. Language localization in or out of scope

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
