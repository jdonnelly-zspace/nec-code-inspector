# Credential-Agnostic Three-Tier Design

Goal: one app with three competency tiers that can be aligned to any electrician credential (US, Canada, France, Germany, UK, and others) by swapping data, not code.

Items marked *(unverified)* come from background knowledge, not a source checked during research.

## Core idea: separate four things that are currently fused

| Layer | What it is | Example values |
|-------|-----------|----------------|
| **Tier** | Competency level, independent of any credential | Foundation, Practitioner, Authority |
| **Concept** | A code-neutral electrical topic or hazard | shock protection, overcurrent protection, earthing/grounding, conductor sizing, wiring methods, special locations, load calculation, testing and verification |
| **Code profile** | An installation code and how it expresses each concept | NEC, CEC, BS 7671, NF C 15-100, DIN VDE 0100, HD 60364 / IEC 60364 |
| **Credential profile** | A real qualification mapped onto tiers, topics and a code profile | NCCER L1-L4, Red Seal 309A, CAP Electricien, Gesellenprüfung, NVQ L3 + AM2 |

Today the README ties tiers directly to NEC articles. Under this design a scenario's violation is authored against a **concept**, and each code profile supplies the citation, limits and wording. A student on the UK profile and a student on the US profile can inspect the same 3D scene and cite different rules.

## Tier definitions (credential-neutral)

| Tier | Learner outcome | Typical credential stage |
|------|-----------------|--------------------------|
| Foundation | Recognize hazards, name basic components, apply simple rules with a reference open | Pre-apprenticeship, vocational school year 1-2, CTE |
| Practitioner | Select, size and install to code; find violations unaided; run basic load calcs and tests | Apprentice to journeyman / Gesellenprüfung / NVQ L3 + AM2 |
| Authority | Design, verify, inspect and certify; handle special locations, edge cases and code changes | Master, Meister, inspector, designer |

Aligning to a framework gives a neutral cross-check: EQF has eight levels described by knowledge, skills, and responsibility and autonomy ([DQR and EQF](https://www.dqr.de/dqr/en/the-dqr/dqr-and-eqf/dqr-and-eqf_node.html)). Germany's DQR references a three-year initial vocational qualification to level 4, and a Meister or bachelor's to level 6.

## Credential map: Europe and UK

| Country | Entry / journeyman level | Higher level | Code | Notes |
|---------|-------------------------|--------------|------|-------|
| Germany | Elektroniker/-in für Energie- und Gebäudetechnik: 3.5-year dual training, Gesellenprüfung in two parts, issued via the Handwerkskammer ([BIBB](https://www.bibb.de/dienst/berufesuche/de/index_berufesuche.php/certificate_supplement/en/elektroniker_fr_energietechnik_e.pdf)) | Elektrotechnikermeister *(unverified)* | DIN VDE 0100 | Graduates count as qualified electrical personnel under accident prevention rules |
| France | CAP Électricien; Bac Pro MELEC (3 years) | BTS Électrotechnique (Bac+2) | NF C 15-100 | Safety authorization (*habilitation électrique*, NF C 18-510, codes such as B0/H0/BR/BC) *(unverified)*; EU diplomas recognized via the CMA ([service-public.fr](https://entreprendre.service-public.fr/vosdroits/F38552?lang=en)) |
| UK | City & Guilds 2357 Level 3 NVQ plus AM2 practical assessment ([source](https://www.logic4training.co.uk/courses/electrical/electrical-level-3-nvq/am2-assessment/)) | Inspection and testing quals (for example 2391) *(unverified)* | BS 7671 (18th Ed.) | ECS/JIB Gold Card and competent-person schemes (NICEIC, NAPIT); not in the EU |
| Netherlands | Not researched | Not researched | NEN 1010 | Dutch implementation of HD 60364 ([source](https://vanmoofer.com/wiresketch/standards/nen-1010/)) |
| Spain / Italy | Not researched | Not researched | REBT / CEI 64-8 | |
| Ireland | Not researched | Not researched | ETCI rules *(unverified)* | |

Europe's national codes share one skeleton of sections (shock protection, thermal effects, wiring systems, special locations) because they derive from IEC 60364 / HD 60364. That shared skeleton is the basis for the concept layer.

## Proposed tier-to-credential alignment

| Tier | US | Canada | Germany | France | UK |
|------|----|--------|---------|--------|----|
| Foundation | NCCER L1-L2 | Apprenticeship L1-L2 | Year 1-2, Part 1 exam prep | CAP Électricien | NVQ L2 / early apprentice |
| Practitioner | NCCER L3-L4, journeyman exam | L3-L4, Red Seal 309A | Gesellenprüfung Part 2 | Bac Pro MELEC | NVQ L3 + AM2 |
| Authority | Master exam, inspector certs | Red Seal holders, inspectors | Meister *(unverified)* | BTS Électrotechnique and above | Inspection and testing, design |

Alignment is approximate. Credentials are not equivalent, so each profile should state its own coverage gaps.

## Proposed data model

Keep this data-driven so a new credential is a content pack, not a code change. The repo already stores the NEC database as JSON in `StreamingAssets` and scenario data as ScriptableObjects; extend that pattern.

```
Concept            id, name, tier-introduced, description (neutral wording)
CodeProfile        id, name, edition, region, units, voltage/frequency defaults,
                   wiring colour codes, per-concept rule references
CredentialProfile  id, name, region, issuing body, exam format,
                   tier mapping, concept weights, code profile id, coverage gaps
Scenario           3D scene + violations, each violation = concept id + parameters
Violation          concept id, severity, per-code rule citation + explanation text
```

Variation that must be data, not hard-coded:
- **Units:** AWG vs mm² conductors; ft vs m.
- **Electrical system:** 120/240 V 60 Hz vs 230/400 V 50 Hz.
- **Conductor colour codes** and symbols.
- **Terminology:** grounding vs earthing; breaker vs MCB; panel vs consumer unit / distribution board.
- **Inspection and exam style:** written exam only, practical assessment, or both.

## Risks and open questions
1. **Copyright.** NEC, CEC, BS 7671 and the VDE/NF standards are copyrighted. Store clause numbers and paraphrased explanations only, and check licensing before using exam blueprints.
2. **Scenario reuse.** Some violations are code-specific (for example GFCI placement rules). Each violation needs an "applies to profiles" list, not an assumption that it works everywhere.
3. **Localization scope.** Language is separate from credential. Decide whether French and German UI text is in scope.
4. **Existing code.** This design has not been checked against the current scripts (`NECDatabase`, `DifficultyManager`, scenario ScriptableObjects). The first step is a review of how tightly NEC is coupled.
5. **Research gaps.** Netherlands, Spain, Italy, Ireland and the German Meister and French habilitation details still need sourcing.

## Suggested next steps
1. Review the current `NEC/`, `Data/` and `Core/` scripts for NEC coupling.
2. Define the concept list from the topics shared by IEC 60364 and the NEC and tag existing scenarios with it.
3. Build `CodeProfile` and `CredentialProfile` data types, with NEC and one other profile (BS 7671 is the easiest second code to add).
4. Convert one scenario end to end as a proof of concept.
