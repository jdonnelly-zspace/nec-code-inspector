# Stakeholder deck: outline for agreement

Status: draft for the owner, refreshed 2026-10-13 after review. This is the flow, talking points and tone for a Product Strategy, Roadmap and Marketing deck of **six slides at most**. Nothing is designed yet. Agree the tone, the points and the value first; then the deck is built from this file.

The deck describes the product we are delivering (the MVP and the phases after it), not how far along the build is today. Worker and opening figures are in `docs/product/MARKET_FACTS.md` (checked on the live O*NET and BLS pages on 2026-10-13). The market sizes and prices are the owner's and have not been checked (see the claims ledger in section 5).

## 1. Who the deck is for and what it must do

- **Audience:** stakeholders who fund, sell or support the product (leadership, sales, partnerships, education programs). Assumed to know zSpace and the Inspire 2; not assumed to know electrical training.
- **Job of the deck:** get agreement on three things: what the product is and the value it gives, the order we will build it in, and how we will deliver it.
- **What a reader should say afterwards:** "It teaches the actual work of the electrical trades, safely and in 3D, and I know the order it comes in and how it will be built."

## 2. Tone (please confirm or change)

| Do | Do not |
|---|---|
| Plain and confident. Short sentences. Say what the product does for a student and a teacher. | Hype ("revolutionary", "AI-powered", "disrupt"). |
| Speak of the MVP as what we are delivering. Use "will" for anything not yet shipped. | Show build status, counts of assets or what is "drafted so far". |
| Skills and competencies first, codes second. Use the trade's own level names (Apprentice, Journeyman, Master) as stages of skill. | Name an installation code or an exam to students, promise "exam prep", or imply the app awards a licence class. The product rule is skills, not credentials. |
| Show the plan as order and what has to be true, not dates we cannot defend. | Quote a market size as fact. |

Voice in one line: *a capable colleague showing a plan they believe in and are straight about.*

## 3. The story in one paragraph

Electrical work is learned by doing it safely, and schools cannot give every student a live panel to inspect. The Code Inspector puts a student at a 3D panel, with a stylus, to learn how electricians, helpers, inspectors, power-line installers, solar installers and alarm installers actually work: the 44 standard procedures of those six occupations, the skills behind them and the rules that apply where they live. The student chooses an occupation and a region; the app handles the rest. Progress is measured by skill and can be followed outside the app.

## 4. The six slides

### Slide 1. The opportunity: skilled trades, taught safely, in 3D

- **Headline:** Students learn electrical work by doing it, and schools cannot let them do it live.
- **Points**
  - Electricians are the largest group: about 821,000 workers in the US (BLS, 2025). The six core occupations employ about 1.3 million people; eighteen adjacent ones, which need foundational electrical knowledge, add more (23 distinct occupations in all; `docs/product/MARKET_FACTS.md`).
  - The work is dangerous to learn on live equipment and hard to show in a textbook.
  - The Inspire 2 stylus display puts a 3D panel in front of the student, which is what this needs.
- **Visual idea:** one image of a 3D panel with the stylus ray on a part, and three numbers: 1.3 million workers in the six core occupations, 6 core and 18 adjacent occupations, and the program count to be sized (no dollar figure until the market view is corrected).
- **Caveat to say aloud:** the market sizing in the original market view is the owner's estimate and is being corrected before outside use.

### Slide 2. The product and its value

- **Headline:** Learn the work of an occupation, not a code book.
- **Points**
  - The student chooses an **occupation**: electrician, electrician's helper, electrical inspector, power-line installer, solar installer or fire and security alarm installer. The app teaches its **procedures** (44 in all), the **skills** behind them and the rules that apply in the student's **region**, without naming a code.
  - Four ways to learn: a short lesson, step-by-step practice of a real procedure, inspection of a 3D building, and designing a panel. A quick reference is always at hand.
  - Three levels that follow the trade's own ladder, **Apprentice, Journeyman and Master**, each described by what the student can already do (section 7).
  - For teachers: progress by skill that can be followed outside the app; for students: a plain screen saying what is tracked and how to export or delete it.
- **Why it is different (say these):** it is built around an occupation's procedures, not a code book; it measures skills across every kind of practice; it is stylus-first.
- **Visual idea:** the mockup's occupation screen next to a practice screen (`docs/design/mockup.html`).

### Slide 3. How it works and why it can be trusted

- **Headline:** Procedures, skills and rules, built from public sources and checked.
- **Points**
  - One chain: occupation, then procedure, then skill, then practice. A student always knows why they are practising something.
  - Content is written in our own words from public sources and goes through a documented source check, with Claude verifying each step against the sources, before it is used. It teaches skills and competencies; it does not reproduce standards.
  - Regions set the rules, units and wiring conventions behind the scenes. English first; the other priority languages follow.
  - Progress is recorded by skill and tier, with analytics through RudderStack and Mixpanel (slide 6), under a generic ID with no personal data in any event.
- **Visual idea:** the chain as four boxes, with one real procedure as the example (establishing an electrically safe work condition).

### Slide 4. Roadmap: three phases, in order

- **Headline:** Three phases, each building on the one before.
- **Phase 1 (the MVP): electricians, electrician's helpers, electrical inspectors**
  - The 3D scenes and art, the panels in the app, analytics, and the progress export for teachers.
  - Ready when: the three occupations can be taught end to end in 3D with progress visible.
- **Phase 2: power-line installers and adjacent occupations**
  - A utility code profile and new utility scenes (poles, vaults, bucket trucks), then literacy packages for maintenance, HVAC and construction roles.
  - Starts when: Phase 1 is live.
- **Phase 3: solar PV installers, fire and security alarm installers**
  - Two new skills (solar PV and storage; low-voltage and fire alarm) and a second code profile for alarm work.
  - Starts when: Phase 2 is under way.
- **Runs across all phases:** more regions (Canada, UK), more languages (English first, the others next), accessibility settings.
- **Visual idea:** a three-lane strip, each lane with its occupations and what it adds.

### Slide 5. Marketing: who we say what to

- **Headline:** One promise, told to three audiences.
- **Promise:** Students learn the real work of an electrician, safely, in 3D. (Say "will" until the scenes ship.)
- **Audiences and the line for each**
  - *Teachers and programs* (CTE, apprenticeship, community college): "A safe place to practise the procedures your students will do on the job, with progress you can see by skill."
  - *Administrators and buyers:* "Stylus-driven lessons for the zSpace Inspire 2, with progress you can follow outside the app and no accounts to manage in the first release."
  - *Industry and partners:* "Skills mapped to the real procedures of the trade, built from public sources and checked step by step."
- **Proof points to use:** the occupation procedures; the mockup; the worker figures from `MARKET_FACTS.md`.
- **Things we do not say:** "exam prep", a credential or code name to students, any market size as fact.
- **Visual idea:** three audience cards with one line each and one proof point.

### Slide 6. How we will deliver it

- **Headline:** The delivery plan is set.
- **Analytics:** RudderStack and Mixpanel, specified in the same format as zSpace's in-app analytics requirements (spec to be written by the owner). Events carry a generic ID and no personal data.
- **Art:** 3DFrame by Vection Technologies for the 3D scenes and art; GameCoders Studio, our art studio, where higher-fidelity art is needed.
- **Content:** built from public information and verified with Claude against the sources.
- **Skills, not standards:** the app teaches skills and competencies in its own words.
- **Ask:** agree the phase order (slide 4).
- **Risks, stated once:** the market sizing needs correcting before outside use; delivery depends on the art pipeline and the analytics spec.
- **Visual idea:** four short cards (analytics, art, content, scope) and one line for the ask.

## 5. Claims ledger: what we can and cannot say

| Claim | Status | Source |
|---|---|---|
| About 821,000 electricians in the US | Can say, cite BLS 2025 | `docs/product/MARKET_FACTS.md` |
| About 1.3 million workers in the six core occupations; 23 distinct occupations, about 7.3 million workers, counting each code once | Can say with the basis stated (O*NET and BLS 2025), after a person spot-checks the pages | `docs/product/MARKET_FACTS.md` |
| The app covers 44 standard procedures across six core occupations | Can say (the procedures are drafted from public sources) | `docs/occupations/` |
| Primary education market of $26M, adjacent $34.6M, combined $60.6M | **Owner's estimate, not checked.** Its inputs do not hold: the worker total is about 7.3 million, not 6.7 million; "Bright Outlook" is wrong for five adjacent occupations; 49-2098 is counted twice (23 distinct occupations, not 24); openings are about 616,000, not 700,000 | `docs/product/MARKET_FACTS.md` |
| "Reviewed by electrical experts" | **Do not say**: no expert review is planned; content is built from public sources and verified with Claude | `PRD.md` |
| Aligned to a credential or exam | **Do not say**: the app contains skills, never credentials | `docs/CONTENT_POLICY.md` |
| Dates for any phase | **Not claimed** in the deck | `PRD.md` section 7 |

## 6. Questions to settle before building the deck

1. Is the tone in section 2 right? Anything to add or soften?
2. Should slide 1 show a market size at all, given the figures are unchecked? (Recommendation: show workers and occupations, not dollars, until the market view is corrected.)
3. Is "order, no dates" right for the roadmap, or do stakeholders expect quarters? (Recommendation: order, with a "target" column left blank for you to fill.)
4. Who presents, and is this a read-ahead or a live talk? (A read-ahead needs more words per slide; a live talk needs fewer.)
5. Should the deck be one artifact you share by link, or a downloadable file? (Decks made from the Slides type can be exported to .pptx or PDF.)
6. Should the MVP include all six core occupations, or only phase 1 (electricians, helpers, inspectors)? The outline keeps the phases as decided (D-13); if all 44 procedures are in the MVP, slide 4 collapses to the MVP plus what comes after.

## 7. One naming system for levels (decided as D-12; the points below are open)

The owner's direction is one vocabulary, the trade's own ladder, **Apprentice, Journeyman, Master**, in the deck and in the app (`TODO.md`, "One naming system for levels"). Points to settle before it is built into the app:

1. **Stages of skill, not licence classes.** In most places these are licence or card classes. The app awards none, so the wording must say they describe what the student can do. Keep the rule: no credential is awarded or implied.
2. **Other occupations.** Helpers sit before an apprentice; inspectors, solar and alarm installers and power-line workers have their own ladders. Recommendation: the same three labels for every occupation, each described by skill.
3. **Other regions.** Canada and the UK use other ladders (for example trade certification and the UK's qualification levels). Recommendation: the same three labels in every region, with region wording only in the lesson text.
4. **Does Master mean a hard bar?** Expert today means full fluency, subtle violations and free-text answers. Master should mean the same, so no content changes.
