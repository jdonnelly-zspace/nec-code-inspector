# NEC Code Inspector - Development Guide

## Project Overview
zSpace Inspire 2 educational app for NEC/NFPA 70 (2026 edition) code training.
Unity 6.3 LTS + zCore 6.3. Target: Windows 11 on zSpace Inspire 2.

## Critical Constraints

- **Render Pipeline: URP (Universal Render Pipeline).** zCore 6.3.3.5 and zView 6.1.0 are URP-compatible. Use URP shaders (Lit, Unlit, etc.), not Built-in/Standard shaders.
- ALL UI Canvases MUST use World Space render mode. NEVER use Screen Space Overlay or Screen Space Camera. zSpace stereo rendering requires World Space.
- ZCamera replaces the standard Unity camera. Never add a regular Camera component to scenes.
- Use TextMeshPro for ALL text rendering.
- Target 90fps minimum (zSpace stereo requires high framerate).
- Stylus is primary input. All interactions must work with 6DOF pointer + 3 buttons. Mouse is fallback only for editor development.

## Architecture Rules

- **GameManager** is the only persistent singleton (DontDestroyOnLoad). Access via `GameManager.Instance`.
- Each inspection scenario and sandbox mode is its own scene.
- All content data is defined in ScriptableObjects under `Assets/_Project/ScriptableObjects/`.
- NEC article data lives in `StreamingAssets/NECDatabase/` as JSON, loaded by `NECDatabase` singleton.
- Scenarios extend `StateMachine` from `Utils/StateMachine.cs` with named `StateID`s and `Step` sequences.

## Input Patterns

- Use `Clickable` from `Scripts/Inputs/` for all interactive objects (pointer events via `ZPointerEventData`).
- Use `Grabbable` from `Scripts/Inputs/` for draggable/rotatable objects (panel doors, breakers, wires).
- Use `DropTarget` from `Scripts/Inputs/` for snap-point validation (breaker slots, wire terminals).
- Input works via `ZPointerEventData` - handles both `ZStylus` and `ZMouse` paths automatically.
- Never reference zSpace types directly outside of `Scripts/Inputs/` folder.
- Scripts in Inputs/ and Utils/ came from career-explorer. Do not refactor them.

## Naming Conventions

- Private fields: `_camelCase` with underscore prefix
- ScriptableObjects: PascalCase with SO suffix (`ScenarioDefinitionSO.cs`)
- Constants: UPPER_SNAKE_CASE
- All code under `Assets/_Project/Scripts/` by domain. One class per file, filename = class name.

## Performance Targets

- 90fps minimum (zSpace stereo)
- Keep draw calls under 200 per frame
- Particle systems capped at 5,000 particles per emitter
- Use object pooling for frequently spawned/destroyed objects

## Git Workflow

- Main branch is always buildable
- Feature branches: `feature/inspection-scenario-1`
- Bugfix branches: `fix/panel-snap-alignment`
- Use Git LFS for: `*.fbx *.glb *.obj *.png *.jpg *.wav *.mp3`
- Commit after each logical work unit with plan step reference

## Testing

- EditMode tests for NEC database search, compliance checker rules, load calculations
- PlayMode tests for scenario step progression
- Test on zSpace hardware weekly for stylus interactions and stereo rendering
