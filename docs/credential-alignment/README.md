# Credential alignment (not part of the app)

The app is skill based. It helps people build the skills they need to get a credential, and it does
**not** contain any credential: no credential names, blueprints, exam breakdowns or readiness scores
ship in the app, and the tests fail if credential files or code are added to it.

This folder holds working notes for the content team and the credential expert. Nothing here is loaded
by the app or included in a build.

## What is here

- `red-seal-309a-draft.json` - a draft of how the Red Seal Construction Electrician (309A) exam
  breakdown lines up with the app's skills. The task list and question counts come from the published
  exam breakdown; the task-to-skill mapping and weights are an estimate. `deferred` marks skills the app
  has no content for yet under the Canadian code. The full occupational standard could not be read, so
  task contents are inferred from their titles. **Needs a credential expert before anyone relies on it.**

## How it is used

- To decide which skills and which scenarios the app needs next (the content gaps).
- To check that the app's skill list covers what a credential assesses.
- To compare a student's skill progress with a credential, outside the app: the progress file
  (`nec_progress.json` in the app's data folder) records mastery per skill and tier, so any alignment
  file can be read against it.

## Adding another credential

Write an alignment file in this folder in your own words from the issuing body's published outline,
set it to draft, and have it reviewed. If it needs a skill the app does not have, add the skill to
`ConceptIds` with a documented reason and add scenarios for it. Do not copy exam questions, answer keys or
text from commercial study guides.
