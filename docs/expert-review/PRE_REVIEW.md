# Pre-review of the NEC and CEC content (not an expert sign-off)

Date: 2026-10-08. This is a desk check of the paraphrased content in the app against public sources, followed by the corrections that were safe to make. **It is not a licensed review.** The code books (NFPA 70, CSA C22.1) are copyrighted and were not available, so every finding rests on public articles, safety-authority bulletins, CSA's public index and general knowledge. Nothing here is marked "reviewed" in the app: the NEC profile stays `app-defined` and the CEC profile stays `draft`.

How to read the confidence levels:
- **high**: several independent public sources agree, or the source is CSA's or a safety authority's own document;
- **medium**: one good source or consistent secondary sources;
- **low**: memory or a single weak source. Low-confidence items were not changed; they are listed for the expert.

Edition limit: the app targets the 2026 NEC. NEC numbering below is verified for the **2020 and 2023** editions only; public 2026 text was too thin to confirm renumbering. The CEC target edition is **C22.1:24** (see section 6).

## 1. Result at a glance

| Question from the review packet | Result |
|---|---|
| The 7 suspected reference problems | All 7 confirmed and corrected (section 2) |
| Receptacle spacing: farthest point to nearest receptacle | Consistent with public summaries for both codes; expert to confirm (section 4) |
| The 44 articles not tied to a violation | 15 dropped, 29 kept, 1 added (240.6); decisions in section 5 |
| The 13 dangling related links | None left: 1 entry added, the rest dropped (section 5) |
| CEC rule numbers | 5 of the 7 drafts used old numbering; all updated to C22.1:24 (section 6) |
| CEC rules for hidden violations | 5 added with public support; 28 stay hidden, with reasons (section 7) |
| CEC tables | Not entered: copyrighted, no safe public source (section 8) |
| Red Seal task-to-skill mapping | Task numbers, titles and counts confirmed; mapping extended (section 9) |
| GFCI/AFCI scope per code | Table below; too uncertain to build an automatic check (section 10) |

Test status after the changes: 1367 logic checks pass (warnings fell from 15 to 2: the two left are the known CEC skill-coverage gaps).

## 2. The seven suspected problems

| Item | Finding | Confidence | Change made |
|---|---|---|---|
| 210.11(C) numbers | (C)(1) is the small-appliance circuits, (C)(2) laundry, (C)(3) bathroom, (C)(4) garage. Our bathroom violation cited (C)(1) and the kitchen one (C)(3); the two article titles were also swapped, and 210.52(B) pointed at the wrong item | high | Both violations and both articles corrected; 210.52(B) fixed |
| Motor disconnects | The within-sight rule is 430.102 ((B) for the motor and driven machinery). The rating rule is 430.110 (115% of full-load current); horsepower and locked-rotor suitability are 430.109 and 430.110(C). The two violations had each other's reference, and the 430.102(B) article mixed in 430.103 content | medium-high | Violations swapped to the right rules with matching text; article reworded |
| 180 VA per outlet | The rule is 220.14(I) (non-dwellings, per receptacle yoke). 220.44 is the demand factor. Dwellings do not count outlets one by one (220.14(J)) | high | Violation now cites 220.14; the article no longer says "every occupancy" |
| 210.8(A) item numbers | Kitchens are (6), basements (5), sinks (7), tubs and showers (9), laundry (10), indoor damp and wet locations (11). Our kitchen was (5) and laundry (7). The 2023 edition widened kitchens to all kitchen receptacles | high (2020/2023) | Kitchen and laundry renumbered, kitchen text widened. 2026 numbering unverified |
| 430.32 percentages | 125% applies to a service factor of 1.15 or more or a marked temperature rise of 40 C or less; 115% for other motors; 430.32(C) allows up to 140% and 130% if the first setting cannot start the motor | high | Article rewritten. The scene motor is now a service factor 1.0 motor, so the 30 A relay on a 22 A motor (limit 25.3 A) is still a clear violation |
| `220.44` vs `220.14` | See "180 VA" above | high | Done |
| Counter spacing measurement | See section 4 | medium-high | Wording kept; expert to confirm |

## 3. Other NEC corrections made

| Where | Finding | Confidence |
|---|---|---|
| RP-BUS-EXCEED-001 and the panel-spaces rule | "More breakers than spaces" is 408.54 (maximum number of overcurrent devices), not 408.36. Changed in the violation, the article, the sandbox rule table, its built-in default, the quick-reference card and two docs | high |
| 220.83 | The first tier is 8 kVA at 100%, not 10 kVA (10 kVA is 220.82) | high |
| 220.82 | The method is in 220.82, not a "Table 220.82" | medium-high |
| 210.52 lettering | Hallways are 210.52(H), not (F) (which is laundry). Islands are (C)(2) and peninsulas (C)(3); (C)(5) is the mounting height | high (2020/2023) |
| Dishwasher GFCI | Required since the 2014 edition, so "new in 2026" was wrong. Text, hint and note rewritten, the new-in-edition flag cleared | medium-high |
| 210.8(A)(3) outdoors | Removed the attached-garage clause, which is not an outdoor rule | medium |
| 250.52(A)(5) ground rod | 1/2 in is allowed for listed stainless or nonferrous rods; 5/8 in otherwise | medium |
| 334.80 and 240.21(B)(1) | Added the left-out conditions (sealed opening for bundled NM cable; the four tap conditions) | medium |

## 4. Receptacle spacing: how both codes measure

Public sources for the NEC (210.52(A) and (C)(1)) and the CEC (26-722 a) and d)(iii)) describe the same method: no point along the wall line may be farther than the limit from a receptacle (6 ft and 24 in in the NEC; 1.8 m and 900 mm in the CEC). The distance is measured along the wall line from the nearest receptacle, so the largest gap between two receptacles is twice the limit. That is how the scene values are defined (`max-distance-to-receptacle`). Confidence: medium-high. The expert should confirm that, and that counter receptacles are also needed where there is no wall behind the counter.

## 5. Untied NEC articles and dangling links

**Rule applied** (also in the field-relevance notes): keep an article if it supports the panel sandbox (load, sizing, panel spaces, service rating) or belongs to one of the app's 11 skills and is commonly tested. Drop it if it needs a skill the app does not have, because the content policy limits content to the app's skills.

**Dropped (15):** pools and spas 680.7, 680.12, 680.22(A)(1), 680.26, 680.44; storage batteries 480.3, 480.4, 480.7; 406.9(A) and (B) (weatherproof covers); 410.10(A) (luminaires in wet locations); 225.18 (overhead clearances); 404.2(C) (doubtful reference); 210.23 and 220.18 (overlap with entries kept). Several of these also had wrong numbering or titles (Article 480 and Article 680), which settles them.

**Kept although not tied to a violation (29):** the 220.x load calculation steps, 230.79, 210.3, 210.19(A), the grounding entries (250.4(A)(5), 250.66, 250.118), the GFCI and receptacle-location entries, 110.14(A) and (B), 110.3(B), 200.6, 406.4(D), 430.6(A)(1), 210.12(B), and **four wiring-method entries (300.4(A), 300.5, 314.16, 334.30)**. Wiring methods are the heaviest area of the Red Seal exam (the largest work activity) but the app has no wiring-methods skill, so these are parked. **Decision for the owner and expert:** add a wiring-methods skill (a 12th, with an alignment note), or drop these four.

**Added (1):** 240.6, standard breaker and fuse ratings. It supports the sandbox and resolves the link from 240.4(B).

**Dangling links:** 13 before. After the changes there are none: 240.6 was added, and the links to entries that were dropped or out of scope (210.8(B), 230.71, 230.72, 240.24(B), 250.52(A)(2), 250.64(B), 334.12, 406.4(B), 422.10, 314.28) were removed. The reference panel skipped these anyway, so nothing visible changes. Worth adding later if the app grows: 230.71 (number of service disconnects) and the concrete-encased electrode.

The NEC article count is now 84 (98, minus 15, plus 1).

## 6. CEC numbers: the draft used old numbering

CSA's own public C22.1:21 index and 2024 bulletins from Alberta, Saskatchewan and Ontario show that several rules moved. Skilled Trades BC says the Red Seal exam provides the 2024 code book from June 2025, so the profile is written against **C22.1:24** (medium-high; red-seal.ca itself does not name an edition).

| Topic | Draft number | Now (C22.1:21 and :24) | Confidence |
|---|---|---|---|
| Grounding electrodes | 10-700 | **10-102** (10-700 to 10-708 are now equipotential bonding) | high |
| GFCI near sinks, tubs, showers | 26-700 | **26-704** (also outdoors within 2.5 m of grade) | high |
| Dwelling receptacles | 26-712 | **26-722** (1.8 m confirmed) | high |
| Kitchen counter receptacles | 26-712(d)(iii) | **26-722(d)(iii)** (900 mm confirmed) | medium-high |
| Arc-fault protection | 26-724(f) | **26-658** | high |
| Working space | 2-308 | 2-308 (unchanged, confirmed) | high |
| Small conductor limits | 14-104 | 14-104 (unchanged; sub-rule letter not confirmed) | high for the rule |

The C22.1:18 numbering is uncertain (Alberta and Ontario bulletins disagree), so older numbers are not recorded.

**Content corrected:** the arc-fault entry listed rooms in NEC style; the CEC rule is every dwelling branch circuit with 125 V receptacles at 20 A or less, with listed exemptions (refrigerator, kitchen counter, bathroom receptacles near the basin). The dwelling-receptacle entry lost an unsupported room list. The electrode entry now says a water pipe electrode depends on the province (Ontario allows it, Saskatchewan does not). The profile note no longer states the numbering backwards.

**Still low confidence:** the 3 m rod length and spacing in current 10-102, the 15/20/30 A caps in 14-104, and the Class A trip values.

## 7. CEC rules for the hidden violations

Of the 33 violations that were hidden under the CEC, five have a rule with public support and were added (the CEC now covers 14 of 42 violations):

| Violation | CEC rule | Support | Differs from the NEC |
|---|---|---|---|
| Motor disconnect not in sight | 28-604 | supported | adds a 9 m limit |
| Service disconnect not accessible | 6-206 | supported | similar |
| Water pipe not bonded | 10-700 | supported (ESA bulletin) | similar |
| Electrodes not bonded together | 10-104 | index-level | similar |
| Breaker handle too high | 26-600 | supported | **1.7 m in a dwelling, against 6 ft 7 in**; the scene was redrawn (handle at 7 ft 4 in; 11% past the NEC limit and 31% past the CEC limit) and carries scene values and limits |

**28 stay hidden.** Reasons, so the expert can fill them in:
- *No verified CEC equivalent found:* dishwasher and garage GFCI, kitchen GFCI (the CEC rule is distance from a sink, so the NEC scene would not be a violation), dedicated bathroom and kitchen circuits, panel directory, workmanship, aluminum on copper-only lugs, the bundled NM cable derating, intersystem bonding, the aluminum grounding conductor and system grounding objective.
- *Rule-level only (section known, exact rule not):* motor conductors (Section 28, 28-106), motor overcurrent (28-200 and Table 29), motor overload (28-306), several motors on one circuit (28-206), feeder tap (14-100), load calculation (Section 8), equipment grounding conductor size (Table 16).
- *Materially different from the NEC (scenes would need redrawing):* the single ground rod (the CEC base rule is two rods), the 180 VA per outlet load (the CEC uses area-based loads and Table 14), motor overload percentages, tap lengths, load calculation structure.

## 8. CEC tables

Not entered. The ampacity tables (1 to 4, with 5A to 5D corrections), Table 13, Table 14 and the motor tables are CSA copyright; the public web gives fragments only. Entering them needs a licensed C22.1:24 and written reproduction permission from CSA, then a cross-check by a qualified electrician. Until then the panel sandbox does not count toward the CEC (`HasOwnTables` stays false).

## 9. Red Seal 309A mapping

- **Confirmed (high):** the eight tasks, their titles and question counts (B-7 4, B-8 4, B-9 4, B-11 4, C-16 9, C-17 9, D-22 8, D-24 6: 48 of 100), against the Red Seal Occupational Standard 2021 and the exam page.
- **Mapping extended:** D-22 now includes conductor sizing, D-24 overcurrent protection, and B-7 earthing and bonding (all high confidence from the standard's text). The C-17 link to shock, arc-fault and overcurrent is marked **inferred** (the standard's C-17 text does not name them; B-8 fits better).
- **Weights recomputed** from the new mapping; the `deferred` flags are recomputed from the actual content. The app can now teach about 24 of the 48 mapped questions' worth under the CEC (it was about 13). Not mapped on purpose (low confidence): B-15 transformers, B-10 surge protection.
- The file states its edition: occupational standard 2021, exam code book C22.1:24.

## 10. GFCI and AFCI scope, NEC against CEC

All CEC rows are low to medium confidence; the 2026 NEC rows are low.

| Location | NEC (2023) | CEC (C22.1:24) | Matters for scenes |
|---|---|---|---|
| Bathrooms | all receptacles | within 1.5 m of a sink, tub or shower (26-704) | distance against room |
| Kitchens | all receptacles (2023 widened from counter only) | within 1.5 m of a sink; no blanket kitchen rule found | a counter outlet far from the sink is a violation under the NEC only |
| Garages | in scope | likely via the 2.5 m grade rule; not confirmed | different tests |
| Outdoors | in scope | within 2.5 m of finished grade | roughly aligned |
| Basements, crawl spaces | in scope | not found | likely NEC only |
| Laundry areas | all receptacles | only near a laundry tub or sink | a laundry with no tub: NEC only |
| Dishwasher | required (dwelling) | none found | NEC only |
| AFCI | listed rooms; may widen in 2026 (unverified) | all dwelling branch circuits with 125 V receptacles at 20 A or less, minus listed exemptions | list of rooms against list of exemptions |

**Conclusion:** the two codes use different tests (rooms against distances and exemptions), the CEC rows are not firm, and the 2026 NEC AFCI scope is unknown. An automatic scope check would need per-code room types, fixture distances, grade height and exemptions on every scene object. It is not worth building until the expert confirms the rows above. The needed data model (rule filters and scene tags) is described in the field-relevance notes and in `TODO.md`.

## 11. What the expert still needs to decide

1. Confirm every change in sections 2, 3 and 6 against the 2026 NEC and C22.1:24 (the worksheets flag each row: "Corrected in pre-review").
2. The 2026 NEC numbering for 210.8(A), 210.11(C), 210.52 and 210.12, where the extensions rule letter looks stale (we left `210.12(B)` unchanged; the letter is probably (D) or later).
3. Doubts left unchanged: the several-motors violation (title is about protection, 430.24 is about conductors), the 250.24(A) sub-item, the single-rod supplement rule, and which `isNewInEdition` flags are right for 2026.
4. Wiring-methods skill: add or drop the four parked articles.
5. Which edition the Red Seal exam is written against, and whether 14-104 sub-rule letters and the 3 m rod numbers are right.
6. The rows in section 7 marked "no verified rule", and the CEC GFCI and AFCI scope in section 10.

## 12. Follow-up: the dropped articles, two new skills and the scope checker

At the owner's request the work in sections 5 and 10 was extended after the pre-review:

- **Two skills added**, `wiring-methods` and `special-locations`, with reasons in `docs/credential-alignment/README.md`. All 15 articles dropped in section 5 were restored (with the corrections found in the pre-review: 680.22(A)(1) is now 680.22(B), 680.7 is the cord-and-plug rule, 680.44 covers spas and hot tubs, 404.2(C) is the neutral conductor at switches, 225.18 lists all four clearance heights, and Article 480 uses the 480.7 and 480.10 numbering) and `480.10` was added. The NEC now has 100 entries. The four wiring-method entries are no longer parked: they belong to the new skill.
- **Eight new violations** in two scenarios (`wiring-methods.json`: bored-hole protection, NM support, burial depth, box fill, overhead clearance; `special-locations.json`: pool luminaire height, pool bonding, weatherproof receptacle cover). They have NEC citations only, so both skills are deferred under the CEC. The Red Seal task C-16 now maps to wiring methods.
- **Confidence.** The wiring-methods and pool facts are high to medium-high. **Article 480 (storage batteries) is medium-low**: the numbering and the 60 V threshold come from a single agent's reading of secondary sources, and the entries are generic. The worksheets flag every restored entry ("Restored for the wiring-methods or special-locations skill"). I did not write battery violations for that reason.
- **Scope checker.** `scope.json` for the NEC (14 rules, from the 2023 NEC as described in public summaries) and the CEC (3 rules and 4 exemptions, from C22.1:24 sources), a validator, and `ProtectionScopeChecker`. Six GFCI and AFCI violations now describe their offending receptacle, and the tests confirm each breaks every code it cites. The comparison in section 10 is now executable; its low-confidence rows are listed under `notModeled` in each file.
- **Still for the expert:** confirm the two skills and their content, the Article 480 entries, every `scope.json` rule (especially the CEC rows and the NEC's 2026 scope), and add the CEC rules for both new skills.

## 13. Follow-up: content moved out of editor scripts

The difficulty settings, certificate templates, quick reference cards and the panel sandbox design now live in JSON under `Assets/_Project/Content` (`Difficulty/`, `Certificates/`, `QuickReference/`, `Sandbox/`) with validators and tests, like the scenarios. Moving them let the tests check them for the first time, and the checks found problems that were fixed in the move:

- **Sandbox references.** The required circuits cited the swapped `210.11(C)` numbers (kitchen as (C)(3), bathroom as (C)(1)) and `210.8(A)(5)` for the garbage disposal; now `210.11(C)(1)`, `(C)(3)` and `210.8(A)(6)`. A new test requires every reference to exist in the code's article data, which also found that `440.4` (air-conditioning nameplate) was missing; it was added (medium-high).
- **Sandbox protection flags.** The dishwasher and garbage disposal circuits did not require AFCI, but under the 2023 NEC kitchens are in the AFCI list. The circuits now name the room they serve, and a test requires the GFCI and AFCI flags to agree with the code's `scope.json`. This changes what the panel sandbox rule for AFCI expects for those two circuits.
- **Claims about 2026.** The dishwasher circuit text said "(2026 NEC)" and the card "Key 2026 NEC Changes" listed changes that are not new in 2026 or cannot be verified. The text was corrected and the card removed; tests now reject unverified "2026 NEC requires/added" claims.
- **Cards** were corrected where the pre-review found errors: kitchen GFCI is `210.8(A)(6)` and includes basements and sinks, islands are `210.52(C)(2)`, bundled NM cable derating needs the sealed opening, and the AFCI card no longer claims "virtually all" rooms. Cards now carry the code they are written for (`profileId`), and the card panel shows only the active code's cards.
- **Certificates** no longer name the NEC in the overall certificate's title and text (the app teaches skills under any code); the overall certificate still requires all 13 skills.
- **Sandbox load check.** The expected load (18,625 VA) is now tested against the load calculator for the design's area, and the conductor and breaker pairs against the code's conductor table.

Still for the expert: the wording of every card and circuit description (worksheet `4-cards-and-sandbox.csv`), the card selection, and the AFCI flags for kitchen appliance circuits against the 2026 NEC.

## Sources (all public)

NEC: ecmweb.com (GFCI and AFCI requirements; key revisions to Chapter 2 of the 2026 NEC; NEC motors series; one-family dwelling load calculations), IAEI Magazine (210.8 GFCI requirements), electricianu.com (2023 NEC 210.11), Mike Holt forums (220.14(I), 408.54), St. Paul building department electrical checklist (210.52).
CEC: CSA C22.1:21 public index (csagroup.org), Alberta Municipal Affairs bulletins 24-ECB-026 and 18-ECB-026 and variance 24-ECV-010-102-GE, Saskatchewan 2024 CEC interpretations, ESA Ontario bulletins 2-9-9, 10-17-8 and 26-18-13, Technical Safety BC (Section 26 directive; top 10 changes in the 2024 code), Nova Scotia bulletins, NT Bulletin 16, electricalindustry.ca guides (Sections 8, 14, 26, 28 and the tables), Skilled Trades BC (exam code book).
Red Seal: red-seal.ca exam information and weightings, and the Red Seal Occupational Standard for Construction Electrician (RSOS 2021).
