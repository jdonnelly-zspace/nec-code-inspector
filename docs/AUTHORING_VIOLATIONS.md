# Authoring a violation

Checklist for adding a violation to the content pool (`docs/CONTENT_PLAN.md`). The scaffold writes the skeleton; the
tests catch most mistakes; the expert catches the rest.

## 1. Scaffold it

```
python tools/new_violation.py --scenario branch-circuits --skill shock-protection --difficulty Beginner \
    --id BC-GFCI-OUTDOOR-001 --codes nec,cec --write
```

This checks the skill, the code folders and that the id is unused, appends the violation with `TODO` placeholders, and
prints reference-entry stubs. The logic tests fail while any `TODO` is left in the content, so fill every one in before
committing.

## 2. Pick the skill, difficulty and pool

- The skill must be one of the 13 in `ConceptIds.cs`. A topic outside them needs a documented reason first
  (`docs/CONTENT_POLICY.md`, section 1).
- `minimumDifficulty` is the lowest level the violation shows at. It also joins every higher level's pool, so check the pool
  gaps for the skill in the test warnings (`dotnet run --project tests/LogicTests`, "violation pools") and fill the lowest
  short tier first.
- `isSubtle` hides it below Expert. Use it for things a careful student would still miss, not for trick questions.

## 3. Write the content

- **Own words.** No copied wording, and no "shall". The tests check.
- **No code or publisher names** in student-facing text (NEC, CEC, NFPA, CSA, BS 7671, IET, BSI). Say "the code". Words that
  differ by region (grounding or earthing, panel or consumer unit) use `{term:key}`; add the key to each code's
  `terminology.json` if it is new.
- **One citation per code the rule exists in.** No citation means the violation is hidden under that code. When the shared
  text carries one code's numbers or units, give the other citations their own `description`, `hintText` and
  `inspectionNote`.
- Cite the rule at the level a student could be asked for, and check the reference exists in that code's `articles.json`. If
  it does not, add an entry in your own words with its skill (`conceptId`), a title, keywords and related entries.

## 4. Make the scene fact checkable

- If the rule is a measured number (a height, a spacing, a clearance), add `sceneFact` and `compliantFact`. The violating
  value must break every code the violation cites by at least 10%, and the compliant value must pass all of them
  (`docs/SCENE_DESIGN.md`).
- If the rule is about GFCI or AFCI protection, add `sceneScope` describing the receptacle; each code's `scope.json`
  decides whether it breaks.
- Name the scene object in `componentObjectName`. It needs a `ViolationVariant` with a violating and a compliant state
  (`docs/POOL_AND_DRAW.md`).

## 5. Run the checks

```
dotnet run --project tests/LogicTests
python tools/unity-compile-check/check.py        # if you changed C#
python tools/make_expert_review.py               # refresh the expert worksheets
```

A new violation and its entries should appear in the worksheets with the doubts that apply to them. Add known doubts to the
flag lists in `tools/make_expert_review.py`.

## 6. Hand it to the expert

Nothing is verified until a credential expert checks the rule number, edition and wording per code. Mark anything you are
unsure of in the worksheet flags and in the pull request. Do not clear a code's licence status (`docs/CONTENT_POLICY.md`,
section 7): that is the owner's decision.

## Gate for every violation

- [ ] Skill is in `ConceptIds`; difficulty fills a short tier
- [ ] Own words; no code, publisher or credential names; no `TODO`
- [ ] A citation for each code the rule exists in, each reference present in that code's entries
- [ ] Scene fact breaks every cited code by 10%; compliant fact passes all; scope set for GFCI or AFCI
- [ ] Scene object has a violating and a compliant state
- [ ] Tests pass; worksheets regenerated; unsure points flagged for the expert
