# Expert review packet

For the credential expert checking the app's content against the code books. **Start with `PRE_REVIEW.md`**: a desk
check against public sources that has already corrected the confident problems, with its confidence levels and the
items left for you. Background and the earlier list of concerns are in `docs/CONTENT_REVIEW.md`; the rules the content follows are in `docs/CONTENT_POLICY.md`.

The worksheets hold **our own paraphrases** and the references we cite. They contain no code-book wording, so
compare each row with the book yourself. Open them in Excel or any spreadsheet tool.

## What to review, in order

| File | Rows | What it is |
|---|---|---|
| `1-violation-citations.csv` | 162 | Each violation in the six inspection scenarios and the rule it cites, per code (139 NEC, 14 CEC, 9 BS 7671). 89 of the NEC rows are phase 1 additions, flagged with their doubts; see `PHASE1_VERIFICATION.md` and its `_BATCH2`, `_BATCH3` and `_BATCH4` companions. Rows with a concern come first. |
| `2-articles-cec.csv` | 12 | Draft Canadian Electrical Code reference entries (C22.1:24 numbering). Written from public sources, not the book. |
| `3-articles-nec.csv` | 161 | NEC reference entries. Rows with a concern first (including the entries restored for the two new skills and the 60 added in phase 1), then cited ones, then those not tied to any violation. |

`5-articles-bs7671.csv` (29 rows) holds the draft BS 7671 entries, written from public sources. A UK-qualified reviewer is needed; every row is flagged and the doubtful ones say why.

A fourth worksheet, `4-cards-and-sandbox.csv`, lists the quick reference cards and the panel sandbox circuits (content that moved out of editor scripts) with the references they cite.

## What to check

1. **Reference.** Is the cited number and sub-item the right rule for what the scene shows? Rows marked "Corrected in
   pre-review" were changed by the desk check and need your confirmation against the 2026 NEC; rows marked "Doubt" were
   left unchanged; every CEC row is flagged as draft.
2. **Accuracy.** Does the paraphrase keep every number, condition and exception of the rule? Paraphrasing can lose one.
3. **Edition.** The NEC data targets the 2026 edition. For the CEC, tell us which edition the credential exam uses; rule
   numbers move between editions.
4. **Scene.** For CEC rows, does the scene (the "What the scene shows" column) actually break that rule? Some margins are
   narrow; the counter scene was redrawn for that reason.

## Filling it in

Use the blank columns on the right: *Reference correct?*, *Text accurate?*, *Correct reference* (when it is wrong),
*Edition notes* and *Comments*. Leave a row blank if you did not check it. Return the files as they are; we apply the
changes to the JSON data.

## Decisions we need from you

- **Untied articles.** The 37 NEC articles marked "not tied to a violation" were kept; confirm. The dropped articles were restored with two new skills (wiring methods and special locations): please review those skills
  and the restored entries, especially Article 480.
- **CEC rules we could not confirm.** Violations hidden under the CEC until a rule is supplied (GFCI in kitchens and garages,
  motor rules, grounding and bonding details, service disconnect, panel directory and others). A reference and a one-line
  description of the rule is enough.
- **Credential alignment.** `docs/credential-alignment/red-seal-309a-draft.json` (not part of the app): the task list and
  question counts come from the published breakdown; which skills each task exercises is our estimate.

## Regenerating

The worksheets are built from the app's content files:

```bash
python tools/make_expert_review.py
```

Re-run it after content changes, and do not edit the CSVs by hand if you want them to stay in step.
