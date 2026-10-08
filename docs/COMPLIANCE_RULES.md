# Panel design compliance rules

The panel sandbox checks a student's design with a rule set. The rule set belongs to the installation code: each
profile's `tables.json` lists its rules in `complianceRules`, so a code chooses which checks run, in what order,
how many of each, and with what ids, names, citations and parameters. The checks themselves are in
`Scripts/PanelSandbox/ComplianceRules.cs` and are tested without Unity (`ComplianceRulesTests`,
`ComplianceRuleSetTests`).

## A rule

```json
{ "ruleId": "DM-MAIN-25", "enabled": true, "kind": "main-breaker-sizing", "reference": "132.17",
  "conceptId": "load-calculation", "name": "Main {term:breaker} with a 25% margin", "margin": 1.25 }
```

| Field | Meaning |
|---|---|
| `ruleId` | Unique in the profile. The NEC keeps `RULE-01` to `RULE-10`; a code may use its own ids |
| `enabled` | `false` switches the rule off without deleting it |
| `kind` | Which check runs (below). A built-in id with no kind gets its kind from the id, so older files still work |
| `reference` | The citation shown for the rule in this code |
| `conceptId` | The skill the rule gives evidence of in the sandbox (`ConceptIds`) |
| `name` | Display name; may use terminology tokens such as `{term:breaker}` and `{Term:panel}`. Empty uses the kind's default name |
| `protection`, `maxRatio`, `margin`, `maxImbalance` | Parameters for the kinds that take them (below) |

Rules run in the order listed. A profile with no `complianceRules` runs the NEC's ten.

## Kinds

| Kind | Checks | Parameters |
|---|---|---|
| `breaker-conductor-match` | A breaker's rating is within its wire's ampacity | `maxRatio`: allowed multiple of the ampacity (default 1) |
| `required-circuits` | Every required circuit is present at its required rating | |
| `protection-required` | Circuits that need GFCI or AFCI (RCD or AFDD in other codes) have it or a dual-function device | `protection`: `gfci` or `afci` (required) |
| `load-balance` | The load is balanced between the two bus sides | `maxImbalance`: overrides the table's limit |
| `main-breaker-sizing` | The main covers the total load | `margin`: required multiple of the load (default 1) |
| `double-tap` | No breaker takes more slots than it has poles | |
| `conductor-ampacity` | Each wire carries its breaker's rating | `maxRatio` |
| `panel-spaces` | The breakers fit in the panel | |
| `wire-connections` | Every breaker has a wire | |

The same kind can appear more than once, for example two protection rules (one per protection) or two main
sizing rules with different margins.

## What is data and what is code

Data: which kinds run, their order, ids, names, citations, skills and the parameters above. The limits come from
the same tables (conductor ampacities, voltages, standard sizes).

Code: the kinds themselves, and the wording of each kind's messages (they use terminology tokens, so a code's own
words show). A check that is not one of the nine kinds, or a message with a different shape, needs a new kind in
`ComplianceRules.cs` and in `ComplianceRuleKinds`; the profile validator rejects an unknown kind, so a data file
can never reference a check that does not exist. Two checks are still shaped for a North American panel: the
left/right bus balance (a profile can switch it off) and the meaning of single and double pole.

## Adding a rule to a code

1. Add an entry to `complianceRules` with a new `ruleId`, a `kind`, a `reference` and the skill it teaches.
2. Run the logic tests: the profile validator checks ids, kinds, parameters and skills.
3. Add the reference to the code's `articles.json` if students should be able to look it up.
