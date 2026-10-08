# Scene Design Across Codes

Status: **proposed**, for the project owner to confirm. No 3D scenes exist in the repo yet, so this is the time to set the rules.

Two questions: how a scene stays correct when the facts differ between installation codes, and whether each region needs its own art.

## 1. Facts that differ between codes

Nine of the 42 violations apply to both the NEC and the CEC. Comparing them shows three kinds of difference:

| Kind | Example | What it means for a scene |
|------|---------|---------------------------|
| Same fact, different limit | Wall receptacle spacing: 6 ft (NEC) vs 1.8 m (CEC). Working space: 3 ft vs 1 m | A scene value can break one code and pass the other |
| Different requirement | Ground rod: 8 ft (NEC) vs 3 m rods, at least two (CEC) | The "right" scene differs, not just the limit |
| Different scope | AFCI and GFCI areas, exceptions | Some rooms are in scope for one code only |

### Decisions

1. **A shared violation must break every code it lists, with a margin.** The scene's measured value has to be outside each listed code's limit by at least 10%. The 36 in counter gap is about 915 mm against the CEC's 900 mm, which is too narrow; it should be redrawn with a larger gap or listed for the NEC only.
2. **Compliant parts must pass every code the scene is offered for.** A correct receptacle or rod in a scene cannot be a violation under another offered code. If it would be, either change the value so it passes both, or hide the scene under the code it fails.
3. **If a fact cannot satisfy both, split it.** Author two violations, one per code, each with a single citation. The existing rule (a violation applies only to codes it cites) hides the other one. Do not stretch one scene across incompatible requirements. The ground rod is the first case.
4. **Store scene dimensions in SI units**, shown in the profile's units. The scene holds one physical value; each code's limit is compared against it.
5. **Make the check automatic.** For violations with a numeric fact, add the scene's measured value (quantity, value, unit) to the violation and a limit (operator, value, unit) to each citation. A logic test converts units and fails if the scene does not break each listed code by the margin, or if a compliant value breaks an offered code. Violations without a number (for example "no AFCI") need no limit.
6. **Scope differences stay as data.** Rooms or circuits in scope for only one code are separate violations or are hidden, never shared.

Why not simply show each scene to one code only? It would halve the reuse that makes a second code cheap. The margin rule keeps sharing where it is honest and splits where it is not.

### Open question for the expert

For the counter spacing violation, check whether the scene's "gap" is the distance between two receptacles or from a point on the wall to the nearest one. Both codes measure from a point to a receptacle, so a 36 in gap between receptacles would be compliant under both.

## 2. Region-specific art

North America and the rest differ most:

| Group | Panel and wiring | Codes |
|-------|------------------|-------|
| North America | Split-phase 120/240 V, breaker panels, same device shapes in the US and Canada | NEC, CEC |
| UK | Consumer units with RCD/RCBO, 230 V, different plugs and sockets | BS 7671 |
| Continental Europe | 230/400 V, distribution boards, different devices and colours | NF C 15-100, DIN VDE 0100 |

### Decisions

1. **One art set per region group, not per code.** NEC and CEC share the North American set. Each profile names its set (`artSet` in `profile.json`, for example `north-america`). A scene's prefabs resolve through the set, so a new set is a content pack.
2. **Build only the North American set now.** Building EU or UK art before a second code is committed is wasted work. Until then, offering a scene under a code from another group is blocked, not approximated with US art.
3. **Keep scenes layout-neutral where it is cheap**: rooms, circuits and violations are described in data, and only device and panel prefabs come from the art set.
4. **Panel sandbox follows the same split.** The split-phase model stays North American. A consumer unit with RCD/RCBO is a separate model, added with the first UK or European code (`TODO.md`, Phase E).

## Impact on the repo

- `ViolationDefinitionSO` / scenario JSON: optional measured value on violations, optional limit on citations.
- A logic test for the margin and pass-both rules, over the real scenario files.
- `profile.json`: `artSet` field, validated by the profile validator.
- No scene or art work yet.
