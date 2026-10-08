# How inspections turn into skill progress

The app tracks progress per skill (a concept ID, see `ConceptIds`), never per credential. This page is the rule
set; the code is in `Scripts/Skills/` and the tests are `SkillProgressTests` and `CitationCreditTests`.

## Evidence from an inspection

Each active violation produces one piece of evidence for its own skill, at the tier of its difficulty
(Beginner = Foundation, Standard = Practitioner, Expert = Authority):

| What the student did | Outcome |
|---|---|
| Missed the violation | 0 |
| Found it and cited something else | 0.5 |
| Found it and cited **another entry of the same skill** | **0.75** |
| Found it and cited the expected entry, or one that matches it (same entry, or a parent or child at a level boundary) | 1 |

"Another entry of the same skill" means the cited reference resolves to a code entry whose `conceptId` equals the
violation's skill. A sub-item the data only holds at entry level resolves to that entry. Example: a bathroom GFCI
violation expects `210.8(A)(1)`; citing `210.8(A)(6)` (kitchens) earns 0.75 because both are shock-protection
entries, while citing `250.50` (grounding) earns 0.5.

The 0.75 is a policy choice (`SkillOutcomes.FoundSameSkillCitation`): the student knew where the rule lives but
picked a neighbouring one. Change it there; the tests state the value and will need the same edit.

The inspection score shown to the student still counts only exact citations; same-skill credit affects skill
evidence only.

## Mastery and attainment

Each skill keeps a running mastery per tier: each outcome moves it toward the outcome by 0.3
(`SkillPolicy.NewEvidenceWeight`). A skill is attained at a tier after at least 3 attempts and a mastery of 0.8,
and a higher tier covers a lower one. The panel sandbox adds evidence too, but only under a code that ships its
own tables.

## Tagging code entries

Every entry in a code's `articles.json` carries a `conceptId`. Tests require it for every shipped code and check
that each violation's citation belongs to the violation's own skill, so the credit rule cannot contradict the
content. When adding a code, tag each entry with the skill it teaches. A code with no tags still works: it just
gives the old half credit for any wrong citation.

Known limit: the several-motors violation (`COM-MULTI-MOTOR-001`) is a skill-of-overcurrent violation that cites a
conductor rule (430.24), which is tagged to match; the expert is asked to resolve the citation
(`docs/expert-review/PRE_REVIEW.md`, section 11).
