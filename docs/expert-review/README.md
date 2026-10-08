# Expert review packet

For the credential expert checking the app's content against the code books. Background and the full list of
concerns are in `docs/CONTENT_REVIEW.md`; the rules the content follows are in `docs/CONTENT_POLICY.md`.

The worksheets hold **our own paraphrases** and the references we cite. They contain no code-book wording, so
compare each row with the book yourself. Open them in Excel or any spreadsheet tool.

## What to review, in order

| File | Rows | What it is |
|---|---|---|
| `1-violation-citations.csv` | 51 | Each violation in the four inspection scenarios and the rule it cites, per code (42 NEC, 9 CEC). Rows with a concern come first. |
| `2-articles-cec.csv` | 7 | Draft Canadian Electrical Code reference entries. Written from public summaries, not the book. |
| `3-articles-nec.csv` | 98 | NEC reference entries. Rows with a concern first, then cited ones, then 44 not tied to any violation. |

## What to check

1. **Reference.** Is the cited number and sub-item the right rule for what the scene shows? Seven NEC rows are flagged
   as suspected wrong or swapped; every CEC row is flagged as draft.
2. **Accuracy.** Does the paraphrase keep every number, condition and exception of the rule? Paraphrasing can lose one.
3. **Edition.** The NEC data targets the 2026 edition. For the CEC, tell us which edition the credential exam uses; rule
   numbers move between editions.
4. **Scene.** For CEC rows, does the scene (the "What the scene shows" column) actually break that rule? Some margins are
   narrow (a 36 in counter gap against a 900 mm limit).

## Filling it in

Use the blank columns on the right: *Reference correct?*, *Text accurate?*, *Correct reference* (when it is wrong),
*Edition notes* and *Comments*. Leave a row blank if you did not check it. Return the files as they are; we apply the
changes to the JSON data.

## Decisions we need from you

- **Untied articles.** The 44 NEC articles marked "not tied to a violation": keep for the panel sandbox, keep for future
  skills, or drop?
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
