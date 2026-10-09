# Content plan

What content the app needs before it can be called sufficient, how much of it exists, and the order to build it.
Numbers come from the scenario, article, card and sandbox files on `main`. Targets are proposals for the owner to
change; nothing here has been reviewed by a credential expert. Rules come from `docs/CONTENT_POLICY.md`
(skills only, own words, no credentials in the app) and `docs/SCENE_DESIGN.md` (a scene must break every code it is
offered under, by a 10% margin).

## 1. Where we are

Violations that carry a citation for each code, by skill. The "Beginner / Standard / Expert" columns are the
lowest difficulty each violation appears at, for the NEC.

| Skill | NEC | of which Beginner / Standard / Expert | CEC | BS 7671 |
|---|---|---|---|---|
| Earthing and bonding | 11 | 4 / 5 / 2 | 3 | 3 |
| Overcurrent protection | 6 | 3 / 1 / 2 | 2 | 0 |
| Wiring methods | 5 | 1 / 3 / 1 | 0 | 0 |
| Shock protection | 4 | 3 / 0 / 1 | 1 | 2 |
| Branch circuit requirements | 4 | 1 / 3 / 0 | 2 | 0 |
| Conductor sizing | 3 | 1 / 1 / 1 | 0 | 0 |
| Disconnecting means | 3 | 1 / 1 / 1 | 2 | 1 |
| Equipment installation | 3 | 2 / 0 / 1 | 0 | 1 |
| Special locations | 3 | 1 / 2 / 0 | 0 | 1 |
| Arc-fault protection | 2 | 0 / 2 / 0 | 2 | 0 |
| Load calculation | 2 | 0 / 2 / 0 | 0 | 0 |
| Working space and access | 2 | 1 / 1 / 0 | 2 | 0 |
| Identification and marking | 2 | 0 / 0 / 2 | 0 | 1 |
| **Total** | **50** | | **14** | **9** |

Other content: 101 NEC, 12 CEC and 29 BS 7671 reference entries; 9 quick reference cards (NEC only); 1 sandbox
panel design; 6 scenarios; no tutorial or per-skill teaching content.

### Why this is not enough

- **Attainment needs repetition on a thin pool.** A skill tier is attained after 3 or more attempts at 80% mastery
  (`docs/SKILL_SCORING.md`), and evidence at a tier only comes from violations that appear at that difficulty. With 2 to
  4 violations per skill, a student replays the same ones and memorises them.
- **Tiers cannot be reached for some skills.** Identification and marking has only Expert violations; arc-fault
  protection and load calculation start at Standard. A Beginner-level student gets no Foundation evidence for them.
- **Only the NEC is usable.** The CEC lacks six skills outright and has no tables (no sandbox). BS 7671 has no art.
- **There is no teaching.** The app tests; it does not yet introduce a skill.

## 2. Targets

### 2.1 Violation pools (NEC first)

A tier's pool is every violation that appears at that difficulty or lower: Foundation uses the Beginner-level
violations, Practitioner adds the Standard ones, and Authority adds the Expert ones.

| Phase | Foundation pool | Practitioner pool | Authority pool | NEC total |
|---|---|---|---|---|
| Now | 0 to 4 per skill | 0 to 9 | 2 to 11 | 50 |
| **Target A (minimum for a pilot)** | **at least 4** | **at least 8** | **at least 12** | **156 (+106)** |
| Target B (stretch) | at least 6 | at least 12 | at least 18 | 234 (+184) |

New NEC violations needed to reach Target A, by skill and by the lowest difficulty they appear at:

| Skill | Add Beginner | Add Standard | Add Expert | Add total |
|---|---|---|---|---|
| Arc-fault protection | 4 | 2 | 4 | 10 |
| Load calculation | 4 | 2 | 4 | 10 |
| Identification and marking | 4 | 4 | 2 | 10 |
| Working space and access | 3 | 3 | 4 | 10 |
| Conductor sizing | 3 | 3 | 3 | 9 |
| Disconnecting means | 3 | 3 | 3 | 9 |
| Special locations | 3 | 2 | 4 | 9 |
| Branch circuit requirements | 3 | 1 | 4 | 8 |
| Equipment installation | 2 | 4 | 3 | 9 |
| Wiring methods | 3 | 1 | 3 | 7 |
| Shock protection | 1 | 4 | 3 | 8 |
| Overcurrent protection | 1 | 3 | 2 | 6 |
| Earthing and bonding | 0 | 0 | 1 | 1 |
| **Total** | **34** | **32** | **40** | **106** |

### 2.2 Other regions

Because a scene must break every code it is offered under, most new violations should be written once with a citation
for each code that has a matching rule. A violation with no matching rule in a code is hidden under that code.

| Code | Now | Minimum bar (2 / 4 / 6 per skill) | Also needed |
|---|---|---|---|
| CEC | 14 | add about 64, starting with the six empty skills (conductor sizing, load calculation, equipment installation, marking, wiring methods, special locations) | tables (needs a CSA licence) for the sandbox; reference entries from 12 to about 40 |
| BS 7671 | 9 | add about 69, with arc-fault, overcurrent, conductor sizing, branch circuits, load calculation, working space and wiring methods first | a UK art set; the sandbox needs tables, which are copyrighted |

### 2.3 Everything that is not a violation

| Content | Now | Target A |
|---|---|---|
| Skill introductions (a short lesson before a skill's first scenario) | 0 | 13, one per skill, region-aware wording |
| Tutorial scenario (stylus, flagging, citing, hints) | 0 | 1 |
| Panel sandbox designs | 1 | 6: two per difficulty, at different dwelling sizes and loads |
| Quick reference cards | 9 (NEC) | 13 (one per skill) per region that has cards |
| Reference entries, NEC | 101 | about 130, so every new violation has an entry and related entries |
| Scenarios | 6 | a pool and draw design (section 4), about 10 themed scenes |

## 3. Candidate topics for the new violations

These are seed ideas by skill, from general electrical practice. They are not rules: every one needs its rule found,
paraphrased and verified per code by an expert before it is written, and its scene fact checked against the 10% margin.
Topics marked "all" probably exist in every code; "US" topics are likely NEC only.

| Skill | Beginner (Foundation) | Standard (Practitioner) | Expert (Authority) |
|---|---|---|---|
| Shock protection | outdoor, garage and bathroom outlets without protection (all) | kitchen island, dishwasher, disposal circuits; EV charger | protection-device feed-through wired with line and load swapped; test button fails |
| Arc-fault protection | bedroom and living room circuits (US, CEC) | extending an existing circuit; wrong device type | shared neutral on a multiwire circuit; protection placed downstream of an outlet |
| Overcurrent protection | breaker too large for the wire (all) | continuous load not allowed for; single-pole breaker on a 240 V load | tap rules; handle ties on multiwire circuits; wrong-rated fuse |
| Conductor sizing | undersized wire on a 20 A circuit | bundled cables not derated; hot attic | neutral size; voltage drop on a long run; aluminium terminations |
| Earthing and bonding | missing electrode or bonding clamp (all) | water and gas pipe bonding; subpanel neutral and ground | separately derived systems; communications bonding |
| Branch circuits | outlet spacing on walls and counters (US, CEC) | required kitchen, bathroom and laundry circuits | dedicated circuits for fixed appliances; islands and peninsulas |
| Load calculation | undersized main for a simple house | demand factors; range and dryer loads | adding an EV charger or heat pump to an existing service |
| Disconnecting means | disconnect not reachable (all) | more handles than allowed; disconnect not within sight | remote disconnects; lockable provisions |
| Working space and access | storage in front of a panel (all) | panel in a closet or bathroom; headroom | dedicated space above a panel; door swing |
| Equipment installation | open knockouts; missing cable clamps | loose terminations; unsecured boxes | torque and listing issues; damaged breakers |
| Identification and marking | panel directory missing (all) | unlabelled disconnects | high-leg and arc-flash labels; conductor colours |
| Wiring methods | cable unsupported; no protection through studs | burial depth; box fill | conduit fill; bending radius |
| Special locations | pool bonding and luminaire height | hot tubs and spas; outdoor covers | solar rapid shutdown; generator interlocks |

## 4. What the app has to do to use this content

Content volume alone does not fix repetition. Four changes are needed in the app, none of them started:

1. **Pools and draws.** A scenario holds a larger pool of violations, and each session draws a set for the chosen
   difficulty (for example 8 per session), seeded so a student sees different ones on replay. This replaces the fixed
   violation list per scenario. It touches `ScenarioFileData`, the importers and `InspectionManager`.
2. **Scene variants.** Each violation needs a swappable state on a scene object (a missing clamp, a handle at the wrong
   height). Plan the scene art as kits with states, not one scene per violation, or the art cost is multiplied by the
   pool size.
3. **Coverage tests.** Tests fail when the NEC pool for any skill and tier falls below the target, and warn for the other
   codes (today they warn about gaps by skill only).
4. **Teaching content.** Skill introductions and the tutorial need a small data format (a skill id, region-aware text,
   an optional illustration), so lessons are data and follow the same content policy and tests.

## 5. Order of work

Each phase ends when its exit test passes. Work that needs an expert or art is marked.

| Phase | Work | Needs | Exit |
|---|---|---|---|
| 0 | Pool-and-draw design; coverage tests with the Target A numbers (failing at first); an authoring checklist and a scaffold for a violation with its citations, scene fact and entries | code only | tests exist and fail for the right skills |
| 1 | Author the 106 NEC violations (grouped by the skills with the biggest gaps: arc-fault, load calculation, marking, working space, then the rest), with reference entries and cards | expert verification; scene kits | every NEC skill at 4 / 8 / 12; worksheets regenerated for review |
| 2 | Teaching: 13 skill introductions, the tutorial, 4 more cards and 5 more sandbox designs | wording review | a new student can finish the tutorial and a first scenario unaided |
| 3 | CEC parity: verified rules for the six empty skills, violations written with NEC and CEC citations together, entries to about 40; the tables need a CSA licence | CEC expert; CSA licence for the sandbox | CEC at 2 / 4 / 6 per skill |
| 4 | BS 7671: UK art set, violations for the seven empty skills, a decision on tables | UK expert; UK art | BS 7671 at 2 / 4 / 6 and scenarios offered |
| 5 | Target B pools; decide on extra skills (motors and controls, transformers and separately derived systems, hazardous locations, test instruments and troubleshooting), each with a documented reason | owner decision | a recorded decision per candidate skill |

**Phase 0 status.** Built: pools and draws (`docs/POOL_AND_DRAW.md`: `ViolationPools`, `ViolationDraw`, `sessionSize` in the
scenario file, `InspectionManager` drawing and remembering recent violations, `ViolationVariant` for scene states); coverage tests
against the Target A numbers (`tests/LogicTests/content-targets.json`; gaps are warnings until a code is marked `enforce`, and the
NEC warning reports the 106 new violations needed); an authoring checklist and scaffold (`docs/AUTHORING_VIOLATIONS.md`,
`tools/new_violation.py`, and a test that fails while any `TODO` is left in content). Not done: no scenario sets `sessionSize`
yet and no scene uses `ViolationVariant`, since neither the scenes nor the extra violations exist. Mark a code `enforce` when it
reaches its target.

**Phase 1 status (first batch).** 22 violations in arc-fault protection (7), working space and access (6) and identification and
marking (9) are in, each drafted and then checked in a second pass against public sources (`docs/expert-review/PHASE1_VERIFICATION.md`),
with 12 new reference entries. The NEC pool shortfall fell from 106 to 84. Held out: items that rested on one source, a case with an
unworkable scene, and the whole load-calculation batch, which waits on an edition decision (the sources disagree on 2023 and 2026
numbering for Articles 200 and 220). Still to author: load calculation, conductor sizing, disconnecting means, equipment installation,
branch circuits, overcurrent protection, shock protection, special locations, wiring methods and earthing, plus the rest of the three
skills above. Lesson for the rest: the first drafts were written from memory and about a third needed their reference letter or number
corrected, so every batch goes through the same two steps, and agents must load web tools before they research.

Phases 0 and 1 can start now. Phase 1 content can be written as data before the 3D scenes exist, but it cannot be
played or checked against the scene rule until the matching scene state is built.

## 6. Quality gates for every new violation

- The skill is in `ConceptIds`; the text passes the paraphrase tests; no code, publisher or credential name appears.
- Each citation is verified by an expert for that code and edition, and is listed in the review worksheets.
- The scene fact breaks every code it cites by 10%, and the compliant case passes all of them (`docs/SCENE_DESIGN.md`).
- A GFCI or AFCI violation also passes the protection scope checker.
- The violation has a reference entry, and a card or related entry if it is a core rule.

## 7. What this plan does not cover

- **Credential preparation beyond skills.** Hand calculations, trade theory, blueprint reading and safety practice are
  not skills in the app. The Red Seal draft maps 48 of 100 exam questions to the 12 skills that carry a weight; the rest
  is outside the app's scope unless the owner adds skills (phase 5).
- **Effort and dates.** These are counts and dependencies. Authoring speed depends on expert availability and on how
  fast the scene kits get built, and neither has been estimated.
- **Classroom reporting.** Teacher dashboards and class progress are not in this plan.
