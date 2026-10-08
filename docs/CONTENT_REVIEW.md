# Content Review Notes

For the credential expert who reviews the paraphrased content. See `docs/CONTENT_POLICY.md` for the rules.
This file deliberately contains no code wording: compare each text in the app with the code book yourself.

## What was rewritten

- All 42 violation citation texts (`Assets/_Project/Content/Scenarios/*.json`, `citations[].text`).
- All 98 reference article texts (`Assets/_Project/StreamingAssets/Codes/nec/articles.json`, `text`); 100 remain after the pre-review and the restore of the dropped topics (section 12 of the pre-review).

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

## Reference problems found, and what was done

Seven suspected problems were raised while paraphrasing (the swapped `210.11(C)` pair, the swapped motor disconnect
pair, the `220.44` per-outlet citation, the `210.8(A)` item numbers, the `430.32` percentage, and two doubts about
spacing and kitchen GFCI). A desk check against public sources (`docs/expert-review/PRE_REVIEW.md`) confirmed all
seven and found more (408.36 should be 408.54; 220.83's first tier is 8 kVA; the 210.52 hallway and island
lettering; the dishwasher GFCI rule dates to 2014, not 2026). The confident ones were corrected in the data; the
worksheets flag each corrected row as "Corrected in pre-review" for the expert to confirm against the 2026 NEC.
The numbering is verified for the 2020 and 2023 editions only. Doubts that were left unchanged are listed in
section 11 of the pre-review (the several-motors violation, the 250.24(A) sub-item, the single-rod supplement rule,
the letter of the 210.12 extensions rule, and the `isNewInEdition` flags).

Spacing: `BC-SPACING-WALL-001` and `BC-SPACING-COUNTER-001` measure the distance from the farthest point on the wall
line to the nearest receptacle, with scene values and limits per code (`docs/SCENE_DESIGN.md`). Public sources describe
both codes that way (medium-high); please confirm. The counter scene was redrawn as an 84 in stretch without a
receptacle (42 in from the nearest one) because the old 36 in gap was only about 15 mm past the CEC limit.

## Canada (CEC) content, draft

`Assets/_Project/StreamingAssets/Codes/cec/` and the citations marked `"profileId": "cec"` were written from public
sources (CSA's public C22.1:21 index, provincial safety authority bulletins, trade-press guides), not from the code
book, and explained in our own words. The pre-review (`docs/expert-review/PRE_REVIEW.md`, sections 6 to 9) found that
the first draft used older rule numbers, and updated them to **C22.1:24**, the edition the Red Seal exam provides:
10-700 became 10-102, 26-700 became 26-704, 26-712 became 26-722 (and 26-722(d)(iii) for counters), and 26-724(f)
became 26-658; 2-308 and 14-104 were already right. Please check every entry:

- **Edition.** Confirm that the Red Seal exam is written against C22.1:24. The 2018 numbering is uncertain (provincial
  bulletins disagree), so it is not recorded.
- **Entries (12) and violations that apply (14 of 42).** The scene facts must break the CEC rule as well as the NEC
  rule. Five violations were added with public support: motor disconnect (28-604, with its 9 m limit), service
  disconnect location (6-206), water pipe bonding (10-700), electrode interconnection (10-104) and breaker handle
  height (26-600, 1.7 m in a dwelling against 6 ft 7 in; the scene was redrawn and has scene values and limits).
- **Not mapped on purpose (28 violations):** no verified CEC rule, or only the section is known, or the scene would
  differ materially (reasons in section 7 of the pre-review). They stay hidden under the CEC until the expert supplies
  the rule.
- **Tables:** not entered. CSA's tables are copyrighted; they need a licensed copy and written permission.
- **Red Seal alignment draft** (`docs/credential-alignment/red-seal-309a-draft.json`, not shipped in the app): task
  numbers, titles and question counts were checked against the occupational standard; the task-to-skill mapping was
  extended (D-22 and conductor sizing, D-24 and overcurrent protection, B-7 and earthing and bonding). The C-17 link
  to shock, arc-fault and overcurrent protection is inferred and is the least certain. Please confirm the mapping.

## Articles not tied to any current violation

Content policy limits content to the app's skills. The pre-review dropped 15 NEC articles whose topics had no skill, and at the owner's request they were restored together with two new skills, `wiring-methods` and `special-locations` (see `docs/credential-alignment/README.md` and section 12 of the pre-review). 36 entries still have no violation: they support the panel sandbox, belong to one of the 13 skills, or are restored topics without a scenario yet (batteries, spas, weatherproof devices).

`110.14(A)`, `110.14(B)`, `110.3(B)`, `200.6`, `210.12(B)`, `210.19(A)`, `210.23`, `210.3`, `210.52(C)(2)`, `210.52(D)`, `210.52(E)(1)`, `210.52(G)`, `210.52(H)`, `210.8(A)(10)`, `210.8(A)(3)`, `210.8(A)(9)`, `220.18`, `220.52`, `220.54`, `220.55`, `220.83`, `230.79`, `240.6`, `250.118`, `250.66`, `404.2(C)`, `406.4(D)`, `410.10(A)`, `430.6(A)(1)`, `480.10`, `480.3`, `480.4`, `480.7`, `680.12`, `680.44`, `680.7`

The Article 480 (storage batteries) entries are medium-low confidence and have no violations. The 13 related-article references that pointed at missing entries are resolved.

## Not covered by this pass

- Quick-reference card text, panel sandbox descriptions and certificate text still live in editor scripts
  (tracked in `TODO.md`); they are written in an instructional voice without statutory wording, but have
  not been through the same paraphrase check.
- Article titles and the violation `description`, `hintText` and `inspectionNote` fields (checked only for
  statutory wording).
