# Content Review Notes

For the credential expert who reviews the paraphrased content. See `docs/CONTENT_POLICY.md` for the rules.
This file deliberately contains no code wording: compare each text in the app with the code book yourself.

## What was rewritten

- All 42 violation citation texts (`Assets/_Project/Content/Scenarios/*.json`, `citations[].text`).
- All 98 reference article texts (`Assets/_Project/StreamingAssets/Codes/nec/articles.json`, `text`).

Each new text keeps the facts of the text it replaced (numbers, limits, conditions, which parts of the code
it points to) in new words, is shorter than the original, and has no run of six or more identical
consecutive words with it. Code references are kept exactly. Article titles were not rewritten.
The check that did this is described in the pull request; it ran once against the old text, which is no
longer in the repository. The standing check (`ContentPolicyTests`) fails if any student-facing text
uses statutory wording.

## What the reviewer should check

1. **Accuracy.** Does each paraphrase say what the referenced rule says? Paraphrasing can lose a condition
   or an exception. Pay attention to texts with numbers, exceptions or lists (`240.4(D)`, `250.53(A)(2)`,
   `220.42`, `220.55`, `430.110`, `210.52` items).
2. **Edition.** The data targets the 2026 NEC. Confirm numbers and exceptions against that edition, and
   confirm which items were added or changed in it (`isNewInEdition` flags).
3. **Neutrality of explanation.** Plain language a student can follow; no hidden new rules.

## Suspected reference problems (existing data, not changed here)

These came to light while paraphrasing. The reference numbers were left as they were.

- `BC-DEDICATED-BATH-001` cites `210.11(C)(1)` and `BC-DEDICATED-KITCHEN-001` cites `210.11(C)(3)`. The
  descriptions suggest the two references are swapped. The article data is also inconsistent with itself:
  the `220.52` article points to `210.11(C)(1)` for small-appliance circuits.
- `COM-DISC-SIGHT-001` (disconnect not in sight) cites `430.110`, and `COM-MOTOR-CTRL-001` (disconnect
  rating) cites `430.102(B)`. The citation texts describe the other violation, so these also look swapped.
- `COM-RECPT-LOAD-001` (a per-outlet load amount) cites `220.44`, which is about demand factors; the
  per-outlet amount is in the `220.14` article.
- `BC-SPACING-COUNTER-001` (and the wall spacing violation) now measure the distance from the farthest point on the wall line to the nearest receptacle, with scene values and limits per code (`docs/SCENE_DESIGN.md`). Please confirm that is how both codes measure it; the counter scene was redrawn as an 84 in stretch without a receptacle (42 in from the nearest one) because the old 36 in gap was only about 15 mm past the CEC limit.
- `BC-GFCI-KITCHEN-001` cites `210.8(A)(5)`. Check the numbering of the `210.8(A)` items (kitchen,
  laundry, bathtub, outdoor) against the 2026 edition; the article data uses `(A)(5)`, `(A)(7)`, `(A)(9)`
  and `(A)(3)`.

- `COM-MOTOR-OL-001` and the `430.32` text: the existing data says the overload limit is 115 percent for motors
  with a service factor of 1.15 or more. To the best of my knowledge the NEC gives a higher limit for those motors
  and 115 percent for the others. Please check this against the edition; the scenario was built on the same figure.

## Canada (CEC) content, draft

`Assets/_Project/StreamingAssets/Codes/cec/` and the citations marked `"profileId": "cec"` were written from public
summaries (provincial safety authority bulletins, trade-press guides to the code), not from the code book, and
explained in our own words. Please check every entry:

- **Edition.** Rule numbers move between editions (older editions number the dwelling receptacle rules 26-722; newer ones
  26-712). The profile says 2024; confirm which edition the Red Seal exam is written against. The AFCI rule (26-724(f))
  was expanded from bedrooms to most living spaces in a recent edition; confirm it for that edition.
- **Rules used:** 2-308 (working space, 1 m), 10-700 (grounding electrodes, 3 m rods), 14-104 (overcurrent rating, small
  conductors), 26-700 (GFCI within 1.5 m of sinks, tubs and showers), 26-712 and 26-712(d)(iii) (dwelling and counter
  receptacle spacing, 1.8 m and 900 mm), 26-724(f) (AFCI). Sub-item letters were taken from secondary sources.
- **Violations that apply to the CEC (9 of 42):** the scene facts must break the CEC rule as well as the NEC rule. The
  14 ft wall gap (limit 3.6 m), the 36 in counter gap (about 915 mm against 900 mm, a narrow margin), the 6 ft ground rod
  (3 m rods) and the 24 in panel clearance (1 m) were checked against the figures in the summaries.
- **Not mapped on purpose:** violations whose CEC rule I could not confirm from public sources (GFCI in kitchens and garages,
  motor rules, grounding and bonding details, service disconnect, panel directory, and others). Rather than guess a rule
  number, they stay hidden under the CEC until an expert supplies the rule.
- **Red Seal alignment draft** (`docs/credential-alignment/red-seal-309a-draft.json`, not shipped in the app): the task list and
  question counts come from the published exam breakdown. Which skills each task exercises, and the weights that follow, are my
  estimate; please confirm or correct them. The full occupational standard could not be read, so task contents are inferred
  from their titles.

## Articles not tied to any current violation

Content policy limits content to the app's skill list. The articles below are neither cited by a
violation nor listed as related to a cited article. Some support the panel sandbox load calculation
(`220.x`, `230.79`, `210.x`); others belong to skills the app does not have yet (pool, battery, outdoor
and wet locations, wiring methods). They were paraphrased, not removed. Decide which to keep.

`110.14(A)`, `110.14(B)`, `110.3(B)`, `200.6`, `210.3`, `210.8(A)(3)`, `210.8(A)(7)`, `210.8(A)(9)`,
`210.12(B)`, `210.19(A)`, `210.23`, `210.52(C)(5)`, `210.52(D)`, `210.52(E)(1)`, `210.52(F)`, `210.52(G)`,
`220.18`, `220.52`, `220.54`, `220.55`, `220.83`, `225.18`, `230.79`, `250.4(A)(5)`, `250.66`, `250.118`,
`300.4(A)`, `300.5`, `314.16`, `334.30`, `404.2(C)`, `406.4(D)`, `406.9(A)`, `406.9(B)`, `410.10(A)`,
`430.6(A)(1)`, `480.3`, `480.4`, `480.7`, `680.7`, `680.12`, `680.22(A)(1)`, `680.26`, `680.44`.

Also open: 13 related-article references point to articles that are not in the data (the logic tests list
them as warnings).

## Not covered by this pass

- Quick-reference card text, panel sandbox descriptions and certificate text still live in editor scripts
  (tracked in `TODO.md`); they are written in an instructional voice without statutory wording, but have
  not been through the same paraphrase check.
- Article titles and the violation `description`, `hintText` and `inspectionNote` fields (checked only for
  statutory wording).
