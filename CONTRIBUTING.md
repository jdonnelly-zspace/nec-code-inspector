# Contributing to NEC Code Inspector

## Prerequisites

- **Unity 6.3 LTS** (URP template)
- **Git LFS** (`git lfs install`)
- **zSpace SDK** — download zCore 6.3.3.5 and zView 6.1.0 from [developer.zspace.com](https://developer.zspace.com) (not included in repo)
- **Windows 11** on zSpace Inspire 2 hardware (or Windows dev machine for editor work)

## Setup

```bash
git clone https://github.com/jdonnelly-zspace/nec-code-inspector.git
cd nec-code-inspector
git lfs pull
```

1. Open Unity Hub, add the project folder, and open with Unity 6.3 LTS
2. Import `zCore-6.3.3.5-Unity2018.4.36.unitypackage` and `zView-6.1.0.unitypackage` from the repo root
3. Import TextMeshPro essentials when prompted
4. Run **NEC Code Inspector > Generate All** from the Unity Editor menu to create ScriptableObject assets

## Key Documentation

| File | What it covers |
|------|---------------|
| `.claude/CLAUDE.md` | Architecture rules, naming conventions, input patterns, performance targets |
| `TODO.md` | Current phase and active tasks |
| `docs/COMPLETED_STEPS.md` | History of Steps 1-7 (script-complete phase) |
| `docs/ALPHA_BUILD_PLAN.md` | Detailed Unity setup guide and World Labs integration |

## Git Workflow

- Branch from `main` using `feature/description` or `fix/description`
- Open a PR, get 1 review, squash merge to `main`
- `main` must always be buildable
- Commit after each logical work unit
- Use Git LFS for all binary assets (already configured in `.gitattributes`)

## Unity Rules (Read Before Coding)

These are hard constraints from the zSpace platform — not style preferences:

- **World Space canvases only** — Screen Space breaks stereo rendering
- **URP shaders only** (Lit, Unlit) — no Built-in/Standard shaders
- **ZCamera only** — never add a regular Unity Camera component
- **TextMeshPro** for all text rendering
- **90fps minimum** — zSpace stereo requires high framerate
- Stylus (6DOF + 3 buttons) is primary input; mouse is editor fallback only

## Hands-Off Zones

These scripts came from another project and should not be refactored:

- `Assets/_Project/Scripts/Inputs/` — Clickable, Grabbable, DropTarget, etc.
- `Assets/_Project/Scripts/Legacy/` — Archived scripts, kept for reference only

## Architecture Quick Reference

- **GameManager** is the only singleton (`GameManager.Instance`)
- Each scenario is its own Unity scene
- Content data lives in ScriptableObjects under `Assets/_Project/ScriptableObjects/`
- NEC articles are JSON in `Assets/_Project/StreamingAssets/NECDatabase/`
- Scenarios use the StateMachine pattern with named StateIDs and Step sequences
