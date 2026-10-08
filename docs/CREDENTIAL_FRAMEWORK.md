# Skill-Based, Credential-Agnostic Design

Goal: an app that helps people build the skills they need to **get** an electrician credential. The app teaches and measures skills. It does not contain, name or award any credential. It works under any installation code (NEC, CEC, BS 7671, ...) by swapping data, not code.

Items marked *(unverified)* come from background knowledge, not a source checked during research.

> This document describes the design as built on the stacked branches (PRs #2 to #9) and what is still open. Research on certification bodies is in `CERTIFICATION_BODIES.md`. The rules the content follows are in `CONTENT_POLICY.md` on those branches.

## Principles

1. **Skills, not credentials.** Progress is tracked per skill. No credential, exam, licence or certification body appears in the app's code, data or screens.
2. **Credential alignment lives outside the app.** A separate document or report compares a person's skill progress with what a credential asks for. Today that is `docs/credential-alignment/` (a Red Seal draft, not shipped in the app).
3. **Content follows the skills.** The app only teaches skills that credentials require, explained in our own words. No statutory wording, no "shall".
4. **Codes are data.** An installation code is a folder of JSON files. Adding a code means adding a folder.

## Four layers

| Layer | What it is | In the app? |
|-------|-----------|-------------|
| **Skill** | A code-neutral electrical topic (11 today, below) | Yes: `ConceptIds`, `Scripts/Skills/` |
| **Tier** | Competency level in a skill: Foundation, Practitioner, Authority | Yes: `SkillTier` |
| **Code profile** | An installation code and how it expresses each skill | Yes: `StreamingAssets/Codes/<id>/` |
| **Credential alignment** | A real qualification mapped onto skills and tiers | **No.** Lives in `docs/credential-alignment/` |

The earlier version of this design had a fourth in-app layer, the *credential profile*. It was removed: a credential is something a person works towards, not something the app contains.

### Skills

The 13 skills (concept IDs): shock protection, arc-fault protection, overcurrent protection, conductor sizing, earthing and bonding, branch-circuit requirements, load calculation, disconnecting means, working space and access, equipment installation, identification and marking, wiring methods, and special locations. The last two were added after the first eleven, with reasons in `docs/credential-alignment/README.md`.

Europe's national codes share one skeleton of sections (shock protection, thermal effects, wiring systems, special locations) because they derive from IEC 60364 / HD 60364. That shared skeleton is why skills can be code-neutral. A skill still to add is testing and verification.

### Tiers

| Tier | Learner outcome | Typical credential stage |
|------|-----------------|--------------------------|
| Foundation | Recognize hazards, name basic components, apply simple rules with a reference open | Pre-apprenticeship, vocational school year 1-2, CTE |
| Practitioner | Select, size and install to code; find violations unaided; run basic load calcs and tests | Apprentice to journeyman / Gesellenprüfung / NVQ L3 + AM2 |
| Authority | Design, verify, inspect and certify; handle special locations, edge cases and code changes | Master, Meister, inspector, designer |

The tiers are credential-neutral. EQF gives a neutral cross-check: eight levels described by knowledge, skills, and responsibility and autonomy ([DQR and EQF](https://www.dqr.de/dqr/en/the-dqr/dqr-and-eqf/dqr-and-eqf_node.html)).

## How progress works

- Each violation and sandbox rule belongs to one skill and has a minimum tier.
- Evidence from an attempt is recorded against the skill: a violation found with the right citation counts 1.0, found with the wrong citation 0.5, missed 0. Sandbox rules count too, but only under codes that have their own tables.
- Each skill keeps a running average. A tier is **attained** in a skill after at least 3 attempts and 80% mastery at that tier; a higher tier covers the lower ones.
- Progress is stored per skill (`ProgressData.skills`), so it carries across codes and has nothing to do with any credential.

Citation matching is code-aware: a citation counts if it matches the active code's reference at the right level (a parent only at a level boundary), ignoring labels and case.

## Code profiles

Each code lives in `Assets/_Project/StreamingAssets/Codes/<id>/`:

| File | Purpose |
|------|---------|
| `profile.json` | id, display name, edition, region, review status (`app-defined`, `draft`, `reviewed`) |
| `articles.json` | Reference entries: reference, title, our paraphrase, section, keywords, related articles |
| `terminology.json` | Code name, reference prefix, section names, vocabulary (grounding vs earthing, breaker vs MCB) |
| `tables.json` | Optional: voltages, conductors, demand tiers, rule configuration, units |

`CodeProfileLibrary` loads every folder, validates it and activates one. The student's choice is saved with their progress and applied at startup. A picker screen lists the codes with their review status and scenario coverage.

Variation that is data, not code: units (AWG vs mm², ft vs m), electrical system (120/240 V 60 Hz vs 230/400 V 50 Hz), terminology, reference numbering, and the rules and tables behind the panel sandbox. Text may use `{code}` and `{term:key}` tokens.

### Violations across codes

A violation is authored against a skill. It carries one **citation per code** (reference, our paraphrase, and optional description, hint and inspection-note overrides). A violation applies to a code only if it has a citation for it, and a scenario applies if any of its violations does. This answers the earlier open question about code-specific rules: nothing is assumed to work everywhere.

Today: 42 violations, all with NEC citations, 9 with CEC citations. The commercial scenario is hidden under the CEC until an expert supplies rules for the rest.

## Alignment outside the app

To help someone get a credential, a report outside the app takes their skill progress and compares it with a credential's task list. `docs/credential-alignment/red-seal-309a-draft.json` is a first draft for the Red Seal 309A exam. Which skills each task exercises, and the weights, are estimates awaiting an expert. Alignment is approximate: credentials are not equivalent, so each alignment should state its own gaps.

## Credential map (research reference)

Not part of the app. This is the research behind alignment work; details and sources are in `CERTIFICATION_BODIES.md`.

| Country | Entry / journeyman level | Higher level | Code | Notes |
|---------|-------------------------|--------------|------|-------|
| Germany | Elektroniker/-in für Energie- und Gebäudetechnik: 3.5-year dual training, Gesellenprüfung in two parts, issued via the Handwerkskammer ([BIBB](https://www.bibb.de/dienst/berufesuche/de/index_berufesuche.php/certificate_supplement/en/elektroniker_fr_energietechnik_e.pdf)) | Elektrotechnikermeister *(unverified)* | DIN VDE 0100 | Graduates count as qualified electrical personnel under accident prevention rules |
| France | CAP Électricien; Bac Pro MELEC (3 years) | BTS Électrotechnique (Bac+2) | NF C 15-100 | Safety authorization (*habilitation électrique*, NF C 18-510) *(unverified)*; EU diplomas recognized via the CMA ([service-public.fr](https://entreprendre.service-public.fr/vosdroits/F38552?lang=en)) |
| UK | City & Guilds 2357 Level 3 NVQ plus AM2 practical assessment ([source](https://www.logic4training.co.uk/courses/electrical/electrical-level-3-nvq/am2-assessment/)) | Inspection and testing quals (for example 2391) *(unverified)* | BS 7671 (18th Ed.) | ECS/JIB Gold Card and competent-person schemes; not in the EU |
| Netherlands | Not researched | Not researched | NEN 1010 | Dutch implementation of HD 60364 ([source](https://vanmoofer.com/wiresketch/standards/nen-1010/)) |
| Spain / Italy | Not researched | Not researched | REBT / CEI 64-8 | |
| Ireland | Not researched | Not researched | ETCI rules *(unverified)* | |

Tier-to-stage mapping is approximate:

| Tier | US | Canada | Germany | France | UK |
|------|----|--------|---------|--------|----|
| Foundation | NCCER L1-L2 | Apprenticeship L1-L2 | Year 1-2, Part 1 exam prep | CAP Électricien | NVQ L2 / early apprentice |
| Practitioner | NCCER L3-L4, journeyman exam | L3-L4, Red Seal 309A | Gesellenprüfung Part 2 | Bac Pro MELEC | NVQ L3 + AM2 |
| Authority | Master exam, inspector certs | Red Seal holders, inspectors | Meister *(unverified)* | BTS Électrotechnique and above | Inspection and testing, design |

## NEC coupling review: outcome

The original review (NEC names in 414 places across 60 files) found the NEC hard-coded in four areas. Status:

| Coupling found | Status |
|----------------|--------|
| Violations authored against NEC citations | Done: skill IDs plus per-code citations; scenario content moved from C# generators to JSON (`Content/Scenarios`) |
| One global NEC database singleton | Done: `ICodeProfile` / `CodeProfiles` registry, data-driven profiles, citation matcher |
| Rules and numbers hard-coded in logic (compliance rules, load constants, AWG ampacities) | Done for data: rules and tables come from the active profile. Open: the panel sandbox only runs under codes that ship tables, and the CEC has none yet |
| Tiers, difficulty labels and UI strings named for the NEC | Done: neutral wording and `{code}` tokens. Open: quick-reference cards, sandbox descriptions and certificate text still live in editor scripts |

## Risks and open questions

1. **Copyright.** NEC, CEC, BS 7671 and the VDE/NF standards are copyrighted. The app stores reference numbers and our own paraphrases only, under `CONTENT_POLICY.md`. All 42 violation texts and 98 NEC articles were rewritten and a standing test rejects statutory wording. Licensing for exam blueprints still needs checking, and the paraphrases need an expert's accuracy review (packet in `docs/expert-review/`).
2. **Facts that differ between codes.** Per-code citation text covers wording. How scenes are designed when the facts differ (a gap that breaks one code's limit but not another's) is an open decision.
3. **Sandbox under other codes.** It needs per-code tables before it counts as evidence.
4. **Localization.** Language is separate from the installation code. Decide whether French and German UI text is in scope.
5. **Research gaps.** Netherlands, Spain, Italy, Ireland and the German Meister and French habilitation details still need sourcing.

## Next steps

1. Expert review of the paraphrased NEC content and the draft CEC content (packet ready).
2. Scene-design rule for facts that differ between codes.
3. More CEC content and CEC tables (needs verified rules).
4. Add a second European code (BS 7671 is the easiest) to prove the data-only path.
5. Alignment reports outside the app for other credentials, reviewed by a credential expert.
