# Content Policy

Rules for everything students read in the app: scenarios, violations, citations, reference articles,
quick-reference cards, certificates and terminology. They exist because the installation codes (NEC, CEC,
BS 7671 and others) are copyrighted, and because the app should teach the skills people need to earn a credential, without containing the credential.

## 1. Scope: the app's skills only, and no credentials in the app

- The app is skill based: it helps people get a credential. It contains **no credentials** (no credential
  names, blueprints, exam breakdowns or readiness scores). The tests fail if credential files or code appear
  in the app.
- A skill is a concept ID (`ConceptIds`). Content may only teach skills from that list.
- A violation tests exactly one skill (`conceptId`); a sandbox rule gives evidence for exactly one skill.
- Adding a skill needs a documented reason in `docs/credential-alignment/` (what a credential assesses that
  the app does not yet teach), reviewed by a credential expert. Alignment notes are never shipped.
- Every entry in a code's `articles.json` carries the skill it belongs to (`conceptId`). Scoring gives partial credit for citing another entry of the violation's skill (`docs/SKILL_SCORING.md`), and the tests check that each violation's citations belong to its skill.
- Enforced by `tests/LogicTests/ContentPolicyTests.cs` (fails the build).

## 2. Wording: paraphrase, never copy

- Write explanations in your own words. Do not copy or closely reword sentences from a code book, a standard,
  an exam blueprint or a commercial study guide.
- A citation identifies where a rule lives (the reference number, for example `250.24(A)(1)`). Its `text` field
  is a short explanation of the idea, written for students, not the code's wording.
- Keep explanations shorter than the source passage and do not rebuild a whole section from pieces.
- No quotation marks around code text. Facts (a number, a limit, a reference) can be stated plainly.
- Statutory phrasing such as "shall" is a sign that text was copied. The test suite counts it and warns.

## 3. Terminology and labels

- Entries in `terminology.json` are generic labels and everyday vocabulary written for this app
  ("grounding", "earthing", "breaker"), not wording from a code.
- UI strings take the code's name, reference format and section names from the active profile
  (`CodeProfiles.Terminology`). Do not hard-code "NEC", "Art." or "Chapter" in UI code.

## 4. Credential alignment (outside the app)

- Alignment files live in `docs/credential-alignment/`. Draft them from the issuing body's published outline
  of what it assesses, in your own words, and keep them marked draft until a credential expert has reviewed them.
- Do not reproduce exam questions or answer keys.

## Current status

The violation citation texts and the article texts have been paraphrased (see `docs/CONTENT_REVIEW.md`),
and `ContentPolicyTests` now fails if any student-facing text in the content files uses statutory wording.
A credential expert still has to review the paraphrases for accuracy, and some references need checking
(listed in `docs/CONTENT_REVIEW.md`). Text that still lives in editor scripts has not been through the
same check yet.

## Adding or changing content: checklist

1. Is the skill in `ConceptIds`, and is any new skill documented in `docs/credential-alignment/`? (Scope)
2. Is the explanation in my own words, and shorter than the source? (Wording)
3. Does the UI text come from the profile terminology, not a hard-coded code name? (Labels)
4. Do the logic tests pass: `dotnet run --project tests/LogicTests`?

## 7. Licensing

Installation codes (NEC, CEC, BS 7671, ...) are copyrighted. The app stores reference numbers and its own
paraphrases and never reproduces code text or tables. Whether that is enough to ship is a decision for the owner
and counsel, so every code profile records it in `profile.json`, in a `license` block:

| Field | Meaning |
|---|---|
| `status` | `unreviewed` (nobody has decided; the starting point for any new code), `own-words` (decided: own-words content and reference numbers need no licence) or `licensed` (a licence or written permission is held) |
| `holder` | Who holds the copyright |
| `note` | What the app uses and what is left to decide |
| `evidence` | For `licensed`: where the agreement or permission is kept |
| `checkedOn` | `yyyy-MM-dd` of the decision (required once the status is not `unreviewed`) |

The tests refuse a profile without a complete record, and the logic test run prints a warning naming every code that
is not yet cleared (`own-words` or `licensed`). The editor logs the same warning when it loads such a code. Neither
fails the build: it is a release gate for the owner, not a developer error. Nothing in the repository decides a
status; an agent or developer must not change `unreviewed` to a cleared status without the owner's decision.

Today `nec` and `cec` are `unreviewed`. CSA's tables are a separate matter: they need a licence and written
permission before they are entered (`docs/expert-review/PRE_REVIEW.md`, section 8).
