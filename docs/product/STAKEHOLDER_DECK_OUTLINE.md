# Stakeholder deck: outline for agreement

Status: draft for the owner. This is the flow, talking points and tone for a Product Strategy, Roadmap and Marketing deck of **six slides at most**. Nothing is designed yet. Agree the tone, the points and the value first; then the deck is built from this file.

Every number below comes from a file in the repository (named in each slide) except the market figures, which are the owner's and have not been checked (see "Claims ledger").

## 1. Who the deck is for and what it must do

- **Audience:** stakeholders who fund, sell or support the product (leadership, sales, partnerships, education programs). Assumed to know zSpace and the Inspire 2; not assumed to know electrical training.
- **Job of the deck:** get agreement on three things: what the product is, the order we will build it in, and the decisions we need from them.
- **What a reader should say afterwards:** "It teaches the actual work of an electrician, safely and in 3D, and I know what is done, what is next, and what you need from me."

## 2. Tone (please confirm or change)

| Do | Do not |
|---|---|
| Plain and confident. Short sentences. Say what the product does for a student and a teacher. | Hype ("revolutionary", "AI-powered", "disrupt"). |
| Honest about status: say "built", "built, not yet in a scene" and "planned" and mean them. | Imply the app teaches in 3D today. It does not: the 3D scenes are not built. |
| Skills and occupations first, codes second. Use the trade's own level names (Apprentice, Journeyman, Master) as stages of skill. | Name an installation code or an exam to students, promise "exam prep", or imply the app awards a licence class. The product rule is skills, not credentials. |
| Show the plan as order and gates (what has to be true), not dates we cannot defend. | Promise all six occupations for the first launch. The research does not support it. |

Voice in one line: *a capable colleague showing a plan they believe in and are straight about.*

## 3. The story in one paragraph

Electrical work is learned by doing it safely, and schools cannot give every student a live panel to inspect. The Code Inspector puts a student at a 3D panel, with a stylus, to learn how an electrician, a helper or an inspector actually works: the procedures, the skills behind them and the rules that apply where they live. The student chooses an occupation and a region; the app handles the rest. We have built the content engine and the first occupations' worth of content; the next step is the 3D scenes and putting the first three occupations in front of real classrooms.

## 4. The six slides

### Slide 1. The opportunity: skilled trades, taught safely, in 3D

- **Headline:** Students learn electrical work by doing it, and schools cannot let them do it live.
- **Points**
  - Electricians are the largest group: about 821,000 workers in the US (BLS, 2025), with helpers and inspectors beside them. Six core occupations and eighteen adjacent ones need foundational electrical knowledge (`docs/occupations/`).
  - The work is dangerous to learn on live equipment and hard to show in a textbook.
  - The Inspire 2 stylus display puts a 3D panel in front of the student, which is what this needs.
- **Visual idea:** one image of a 3D panel with the stylus ray on a part, and three numbers (workers, occupations, schools to be sized).
- **Caveat to say aloud:** the market sizing in the original market view is the owner's estimate and is being corrected before outside use.

### Slide 2. The product and its value

- **Headline:** Learn the work of an occupation, not a code book.
- **Points**
  - The student chooses an **occupation** (electrician, helper, inspector first). The app teaches its **procedures**, the **skills** behind them and the rules that apply in the student's **region**, without naming a code.
  - Four ways to learn: a short lesson, step-by-step practice of a real procedure, inspection of a 3D building, and designing a panel. A quick reference is always at hand.
  - Three levels that follow the trade's own ladder, **Apprentice, Journeyman and Master**, each described by what the student can already do (the app's current labels, Beginner, Standard and Expert, are being replaced; see section 7).
  - For teachers: a clear progress file by skill, no accounts to manage in the first release; for students: a plain screen saying what is tracked and how to export or delete it.
- **Why it is different (say these, with proof):** it is built around an occupation's procedures; it measures skills across the 3D practice; it is stylus-first.
- **Visual idea:** the mockup's occupation screen next to a practice screen (`docs/design/mockup.html`).

### Slide 3. Where we are today (the proof)

- **Headline:** The engine and the first content are built; the 3D scenes are next.
- **Built and tested** (`docs/product/PRD.md`, `TODO.md`)
  - 20 skills with tiers; 20 short lessons.
  - Six occupations researched, **44 procedures** written down; step-by-step practice for 28 of them.
  - 139 inspection violations across 6 scenarios; 17 quick reference cards, one for every skill; 6 panel designs; an 8-step tutorial.
  - Regions with their own codes and units (US built; Canada and UK drafted); two NEC editions as separate profiles, hidden from students.
  - A content pipeline: draft, then a separate source check, then an expert.
- **Not yet built** (say this plainly)
  - The 3D scenes and art; nothing has been opened in the Unity Editor.
  - Hands-on trainers (a virtual meter), analytics, the teacher export.
  - Expert review of the content; licence decisions for the codes.
- **Visual idea:** two columns, "Built" and "Next", with the counts above.

### Slide 4. Roadmap: order and gates, not dates

- **Headline:** Three phases, each one gated by what has to be true.
- **Now (Phase 1: electricians, helpers, inspectors)**
  - Build the 3D scenes; wire the panels; add the analytics and the teacher export; expert review of the content.
  - Gate: scenes built and a classroom pilot; expert sign-off on the first occupations.
- **Next (Phase 2: power-line installers and adjacent occupations)**
  - A utility code profile and new utility scenes (poles, vaults, bucket trucks), the largest new content in the plan, then literacy packages for maintenance, HVAC and construction roles.
  - Gate: Phase 1 live; licence decision for the utility code; the utility scenes planned and funded.
- **Later (Phase 3: solar PV installers, fire and security alarm installers)**
  - Two new skills (solar PV and storage; low-voltage and fire alarm) and a second code profile for alarm work.
  - Gate: Phase 2 under way; licence decision for the alarm standard; the 2026 code numbering verified for the PV content.
- **Also running across all phases:** more regions (Canada, UK), more languages (English first, the others next), accessibility settings.
- **Visual idea:** a three-lane Now / Next / Later strip, each with its gate underneath.

### Slide 5. Marketing: who we say what to

- **Headline:** One promise, told to three audiences.
- **Promise:** Students learn the real work of an electrician, safely, in 3D.
- **Audiences and the line for each**
  - *Teachers and programs* (CTE, apprenticeship, community college): "A safe place to practise the procedures your students will do on the job, with progress you can see by skill."
  - *Administrators and buyers:* "Stylus-driven lessons for the zSpace Inspire 2, with no accounts to manage in the first release."
  - *Industry and partners:* "Skills mapped to the real procedures of the trade, drafted from public sources, with expert review planned before release."
- **Proof points to use now:** the counts on slide 3; the occupation procedures; the mockup.
- **Things we do not say:** "exam prep", a credential or code name to students, "all six occupations at launch", any market size as fact.
- **Visual idea:** three audience cards with one line each and one proof point.

### Slide 6. Decisions and asks

- **Headline:** What we need from you to move.
- **Decisions needed**
  1. Licences for the installation codes and standards (NEC, CEC, BS 7671, the alarm standard, the utility code). This gates shipping and Phase 3 and 3.
  2. Which analytics service the other zSpace apps use, and what the generic student account is.
  3. Funding and owners for the 3D scenes and art, the biggest remaining piece of work.
  4. An occupational expert and a code expert to review the content.
  5. The first pilot classrooms for Phase 1.
- **Risks, stated once:** content outpaces art; the code licences are undecided; the original market sizing needs correcting before outside use.
- **Visual idea:** a short decisions table with an owner and a "needed by" column to fill in.

## 5. Claims ledger: what we can and cannot say

| Claim | Status | Source |
|---|---|---|
| 20 skills, 20 lessons, 44 procedures documented, 28 with step practice | Can say (counts from the repository) | `PRD.md`, `occupations.json` |
| 139 inspection violations, 17 cards, 6 panel designs, 8-step tutorial | Can say | `Content/`, `TODO.md` |
| About 821,000 electricians in the US | Can say, cite BLS 2025 | `docs/occupations/` |
| "Teaches in 3D today" | **Cannot say**: scenes are not built | `TODO.md` |
| "Reviewed by electrical experts" | **Cannot say yet**: nothing has had an expert review | `PRD.md` |
| Primary education market of $26M, adjacent $34.6M, combined $60.6M | **Owner's estimate, not checked.** Its inputs do not hold: the worker total is about 7.3 million, not 6.7 million; "Bright Outlook" is wrong for five adjacent occupations; 49-2098 is counted twice (23 distinct occupations, not 24); openings are about 616,000, not 700,000 | `docs/product/MARKET_FACTS.md` |
| Launch of all six occupations in the first half of 2027 | **Cannot say**: the research supports Phase 1 only | `PRD.md` section 7 |
| Aligned to a credential or exam | **Cannot say**: the app contains skills, never credentials | `docs/CONTENT_POLICY.md` |

## 6. Questions to settle before building the deck

1. Is the tone in section 2 right? Anything to add or soften?
2. Should slide 1 show a market size at all, given the figures are unchecked? (Recommendation: show workers and occupations, not dollars, until the market view is corrected.)
3. Is "no dates, only gates" right for the roadmap, or do stakeholders expect quarters? (Recommendation: gates, with a "target" column left blank for you to fill.)
4. Who presents, and is this a read-ahead or a live talk? (A read-ahead needs more words per slide; a live talk needs fewer.)
5. Should the deck be one artifact you share by link, or a downloadable file? (Decks made from the Slides type can be exported to .pptx or PDF.)
6. Any audience missing (for example, a school district buyer, an industry association or a funder)?

## 7. One naming system for levels (open)

Today the deck's occupations (electrician, helper, inspector) and the app's levels (Beginner, Standard, Expert) use two vocabularies. The owner's direction: use one, the trade's own ladder, **Apprentice, Journeyman, Master**, in the deck and in the app (`TODO.md`, "One naming system for levels"). The deck says so from now on. Points to settle before it is built into the app:

1. **Stages of skill, not licence classes.** In most places these are licence or card classes. The app awards none, so the wording must say they describe what the student can do. Keep the rule: no credential is awarded or implied.
2. **Other occupations.** Helpers sit before an apprentice; inspectors, solar and alarm installers and power-line workers have their own ladders. Recommendation: the same three labels for every occupation, each described by skill.
3. **Other regions.** Canada and the UK use other ladders (for example trade certification and the UK's qualification levels). Recommendation: the same three labels in every region, with region wording only in the lesson text.
4. **Does Master mean a hard bar?** Expert today means full fluency, subtle violations and free-text answers. Master should mean the same, so no content changes.
