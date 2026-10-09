# Pools and draws

How an inspection session picks its violations, so replays differ and students cannot memorise a scenario.
Part of phase 0 of `docs/CONTENT_PLAN.md`.

## The idea

A scenario's violations are a **pool**. Each session **draws** a smaller set from it. Which violations are in the pool
depends on the student's code and difficulty; how many are drawn is the scenario's `sessionSize`.

1. **Pool.** The scenario's violations that have a citation for the active code (`ViolationDefinitionSO.AppliesTo`) and
   are visible at the difficulty being played (`ViolationPools.IsActive`): a violation is visible at its
   `minimumDifficulty` and above, and a subtle one only at Expert.
2. **Draw.** `ViolationDraw.Choose` picks `sessionSize` violations from the pool:
   - never more than the size, and the whole pool if the size is 0 or larger than the pool;
   - skills are spread as evenly as the pool allows (one from each skill in turn);
   - within a skill, violations the student saw recently come last (`ProgressManager.GetRecentViolationIds`, the last
     60 shown);
   - the same seed gives the same draw (used in tests); in play the seed is the clock;
   - the result keeps the pool's order.
3. **Scene.** For every violation in the scenario, `InspectionManager` calls `ViolationVariant.Apply` on the scene object it
   names: the violating state for drawn violations, the compliant state for the rest.

## Scenario file

```json
"sessionSize": { "beginner": 6, "standard": 8, "expert": 10 }
```

Optional. A missing or zero value means "all of them", which is how every scenario works today, so nothing changes until a
scenario sets it. Values cannot be negative. Set it when a scenario's pool is clearly larger than what one session should
show; as a starting point use about half the pool, never fewer than the number of skills the scenario covers.

## What a scene has to do

A violation that is not drawn must look compliant, or flagging it would be counted as a false positive for something the
student could see was wrong. So **every part that can carry a violation needs two states**, a violating one and a compliant
one, switched by a `ViolationVariant` on the object (or a parent of it) named by `componentObjectName`:

- `_whenViolating`: objects shown when the violation is part of this session (a missing bonding clamp, a handle at 7 ft 4 in).
- `_whenCompliant`: objects shown when it is not (the clamp is there, the handle is at 6 ft 3 in).

Build these as kits (a panel with swappable parts, a pipe with or without a clamp), not as one scene per violation. The scene
facts rule in `docs/SCENE_DESIGN.md` still applies to both states: the violating state breaks every code the violation
cites, and the compliant state passes every code it is offered under.

An object with no `ViolationVariant` is left as it is. Objects that are never a violation do not need one.

## What it does not do yet

- Nothing in the scenes uses it: no scene exists, and no scenario file sets `sessionSize`.
- The pool counts do not yet reach the targets in `tests/LogicTests/content-targets.json`; the content tests report the gaps
  as warnings until each code is marked `enforce`.
- Draws are per session and independent. There is no adaptive choice by weak skill yet.
- Recently seen violations are remembered across scenarios. A scenario whose pool is as small as its session size ignores
  the memory because everything is drawn.

## Where it lives

`Scripts/Data/ViolationPools.cs`, `ViolationDraw.cs`, `SessionSize.cs`; `Scripts/Inspection/ViolationVariant.cs` and
`InspectionManager.Initialize`; tests in `tests/LogicTests/ViolationDrawTests.cs` and `ContentTargetsTests.cs`.
