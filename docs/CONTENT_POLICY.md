# Content Policy

Rules for everything students read in the app: scenarios, violations, citations, reference articles,
quick-reference cards, certificates and terminology. They exist because the installation codes (NEC, CEC,
BS 7671 and others) are copyrighted, and because the app should teach what credentials actually require.

## 1. Scope: only skills that a credential requires

- A skill is a concept ID (`ConceptIds`). Content may only teach skills that at least one credential file in
  `StreamingAssets/Credentials/` requires.
- A violation tests exactly one skill (`conceptId`); a sandbox rule gives evidence for exactly one skill.
  Adding content for a skill no credential requires is out of scope. Add the credential requirement first,
  with a credential expert's review, or leave the content out.
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

## 4. Credentials

- A credential file lists the skills and tiers it requires. Draft it from the issuing body's published
  outline of what it assesses, in your own words, and set `reviewStatus` to `draft` until a credential
  expert has reviewed it.
- Do not reproduce exam questions or answer keys.

## Current status

The existing citation texts and article texts were written before this policy and mostly use statutory
wording. They need a paraphrase pass (tracked in `TODO.md`, Phase F). Until then the content test reports
how many texts still use statutory wording, and the number should go to zero.

## Adding or changing content: checklist

1. Which credential requires the skill? (Scope)
2. Is the explanation in my own words, and shorter than the source? (Wording)
3. Does the UI text come from the profile terminology, not a hard-coded code name? (Labels)
4. Do the logic tests pass: `dotnet run --project tests/LogicTests`?
