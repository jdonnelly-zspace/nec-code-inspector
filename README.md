# NEC Code Inspector - zSpace Inspire 2

Interactive 3D educational application for learning the National Electrical Code (NEC/NFPA 70, 2026 Edition), built for the zSpace Inspire 2 AR/VR platform.

## Overview

Students use the zSpace Stylus to inspect virtual electrical installations for code violations and design electrical panels with automated NEC compliance checking. The app supports three audience levels:

- **Beginner (CTE)**: High school Career & Technical Education students learning electrical fundamentals
- **Standard (Apprentice)**: Trade school students and apprentices preparing for journeyman exams
- **Expert (Licensed)**: Licensed electricians and inspectors studying NEC 2026 updates

## Features

### Code Inspection Scenarios
Walk through pre-built 3D electrical installations and identify NEC violations using virtual inspection tools. Flag violations, cite the relevant NEC article, and receive scored feedback.

### Panel Design Sandbox
Design electrical panels from scratch: select panel type, add circuits with properly sized breakers, route wires, calculate loads (Art. 220), and get automated NEC compliance checking.

### NEC Reference System
Searchable in-app NEC article database with quick-reference cards for common code topics.

## NEC Coverage

| Scenario | Key NEC Articles |
|----------|-----------------|
| Residential Service Panel | Art. 110, 230, 240, 408 |
| Branch Circuit Wiring | Art. 210, 220, 310 |
| Grounding & Bonding | Art. 250 |
| Commercial Installation | Art. 220, 230, 240, 430 |
| Outdoor/Wet Location | Art. 406, 410, 680 |

## Requirements

- **Unity**: 6.3 LTS (6000.3)
- **zSpace SDK**: zCore 6.3 Unity Plugin (from [developer.zspace.com](https://developer.zspace.com/downloads))
- **Target Platform**: Windows 11
- **Target Hardware**: zSpace Inspire 2
- **Editor Development**: Supported on Mac and Windows (mouse fallback mode)

## Setup

1. Clone this repository
2. Open in Unity 6.3 LTS
3. Import zCore 6.3 plugin from [developer.zspace.com](https://developer.zspace.com/downloads)
4. Open `Assets/_Project/Scenes/Boot/BootScene.unity`
5. Press Play (Mac: mouse fallback) or Build for Windows (File > Build Settings > Windows x86_64)

## Project Structure

```
Assets/_Project/
  Scenes/           Boot, MainMenu, Inspection scenarios, Panel Sandbox
  Scripts/
    Core/           GameManager, DifficultyManager, ProgressManager
    NEC/            NECDatabase, NECArticle, NECSearchEngine
    Inspection/     Scenario runner, violation detection, inspection steps
    PanelSandbox/   Panel builder, circuit manager, load calculator, compliance checker
    Inputs/         Clickable, Grabbable, DropTarget (zSpace stylus + mouse input)
    Tools/          Virtual inspection tools (flashlight, multimeter, etc.)
    UI/             World-space UI managers and panels
    Utils/          StateMachine, highlighting, UI helpers
    Data/           ScriptableObject definitions
  ScriptableObjects/ Scenarios, violations, difficulty settings
  Prefabs/          Components, tools, UI panels, environments
  StreamingAssets/   NEC article database (JSON)
```

## Development

- **Mac**: Use mouse fallback for layout and logic testing. Left-click = primary, right-click = secondary.
- **Windows + zSpace**: Full stylus, stereo rendering, and haptic testing.
- See `docs/` for architecture details and NEC coverage mapping.
- See `.claude/CLAUDE.md` for AI coding guidelines.

## Asset Attribution

3D models sourced from GrabCAD, Sketchfab (CC licensed), CGTrader (free), and 3D ContentCentral.
See `docs/ASSET_SOURCES.md` for full attribution list.

## License

Educational use. See LICENSE file for details.
