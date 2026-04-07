# NEC Code Inspector — Alpha Build Plan

## Goal
Get from "73 scripts with no scenes" to a **demonstrable alpha on zSpace Inspire 2** with at least 2 playable inspection scenarios, the panel sandbox, and a main menu.

## Budget Justification — World Labs Credits

> **Request:** $50 initial purchase (62,500 credits) for environment prototyping, with authorization for up to $250 total through alpha.
>
> World Labs Marble generates photorealistic 3D electrical environments from text prompts in ~5 minutes at ~$1.28 each, replacing weeks of manual 3D modeling per environment. Our app requires 5+ unique environments (garage, kitchen, utility room, commercial space, outdoor), each of which would take a 3D artist 2-3 weeks to model, texture, and light — roughly $5K-10K in artist time per scene. Marble produces the equivalent for $1.28 per generation with ~10 iterations needed per final environment, totaling ~$65 for all 5 MVP scenes. Credits never expire and the output (GLB meshes + Gaussian splats) imports directly into our Unity pipeline via an open-source plugin. The $250 total covers prototyping, MVP, alpha variations, and buffer through beta — a 98% cost reduction vs. manual 3D art.

---

## Prerequisites

Before starting, ensure you have:
- [ ] Unity 6.3 LTS installed (Unity Hub → Installs → 6.3 LTS)
- [ ] zCore 6.3 package (from zSpace developer portal)
- [ ] Git LFS enabled (`git lfs install`)
- [ ] World Labs account with API key (https://platform.worldlabs.ai/api-keys)
- [ ] $50 in World Labs credits purchased (https://platform.worldlabs.ai/billing)

---

## Phase 1: Unity Project Setup (Day 1)

### 1.1 Create the Unity Project

1. Open Unity Hub → **New Project** → select **3D (URP)** template → set location to `C:\Users\Jilldonnelly\Documents\nec-code-inspector`
   - If the folder already exists with scripts, Unity Hub may prompt — choose to create the project in this location
   - Alternatively: create the URP project in a temp folder, then copy the generated `Packages/`, `ProjectSettings/`, and `Assets/Settings/` folders into the existing repo
2. Unity will initialize with URP and import all 73 existing scripts. Wait for compilation.
3. If compilation errors appear, check:
   - TextMeshPro: **Window → TextMeshPro → Import TMP Essential Resources**

### 1.2 Install zSpace & Additional Packages

1. **Import zCore:** **Assets → Import Package → Custom Package** → select `zCore-6.3.3.5-Unity2018.4.36.unitypackage` → Import All
2. **Import zView:** **Assets → Import Package → Custom Package** → select `zView-6.1.0.unitypackage` → Import All
3. **Install Gaussian Splatting:** **Window → Package Manager → + → Add package from git URL** → `https://github.com/aras-p/UnityGaussianSplatting.git`
4. **Verify URP is active:** **Edit → Project Settings → Graphics** → confirm a URP Asset is assigned as the default render pipeline

### 1.3 Run All Generator Scripts

These create the ScriptableObject assets from the data baked into the editor scripts:

1. **Menu: NEC Inspector → Generate Branch Circuit Scenario**
   - Creates: 12 ViolationDefinitionSO assets + 1 ScenarioDefinitionSO
   - Location: `Assets/_Project/ScriptableObjects/Violations/BranchCircuits/` and `Scenarios/`

2. **Menu: NEC Inspector → Generate Grounding Scenario**
   - Creates: 10 ViolationDefinitionSO assets + 1 ScenarioDefinitionSO
   - Location: `Assets/_Project/ScriptableObjects/Violations/Grounding/` and `Scenarios/`

3. **Menu: NEC Inspector → Generate Panel Sandbox Data**
   - Creates: 1 PanelDesignDefinitionSO (Residential 200A, 12 circuits)
   - Location: `Assets/_Project/ScriptableObjects/Scenarios/`

4. **Menu: NEC Inspector → Generate Quick Reference Cards**
   - Creates: 10 QuickReferenceCardSO assets
   - Location: `Assets/_Project/ScriptableObjects/QuickReferenceCards/`

5. **Create ScenarioCatalog manually:**
   - Right-click in `Assets/_Project/ScriptableObjects/` → **Create → NEC Inspector → Scenario Catalog**
   - Name it `ScenarioCatalog`
   - Drag both ScenarioDefinition assets into its `scenarios` list

6. **Create DifficultySettings manually (3 assets):**
   - Right-click in `Assets/_Project/ScriptableObjects/DifficultySettings/`
   - **Create → NEC Inspector → Difficulty Settings** × 3
   - Name them: `DifficultySettings_Beginner`, `DifficultySettings_Standard`, `DifficultySettings_Expert`
   - Configure each per the table:

   | Field | Beginner | Standard | Expert |
   |-------|----------|----------|--------|
   | level | Beginner | Standard | Expert |
   | displayName | Beginner | Standard | Expert |
   | showHighlightHints | true | false | false |
   | showScaffolding | true | false | false |
   | scaffoldingTimeoutSeconds | 30 | -1 | -1 |
   | citationMode | Dropdown | SearchableDropdown | FreeText |
   | enableTimeLimit | false | false | true |
   | timeLimitSeconds | 0 | 0 | 1200 |
   | penalizeFalsePositives | false | false | true |
   | falsePositivePenalty | 0 | 0 | 0.1 |
   | showSimplifiedTerminology | true | false | false |
   | highlight2026Changes | false | false | true |
   | includeSubtleViolations | false | false | true |

### 1.4 Create the GameManager Prefab

1. In any scene, create an empty GameObject named `GameManager`
2. Add the `GameManager` component
3. Drag the 3 DifficultySettings assets into its `_difficultySettings` array
4. Drag to `Assets/_Project/Prefabs/` to create a prefab
5. This prefab will be placed in the Boot scene

---

## Phase 2: Generate Environments with World Labs (Day 1-2)

### 2.1 API Setup

```bash
# Set your API key
export WLT_API_KEY="your-key-from-platform.worldlabs.ai"
```

### 2.2 Environment Prompts

Generate each environment with the Marble API. For each prompt, run 3-5 variations and pick the best result.

**Environment 1: Residential Garage with Electrical Panel (Scenario 1)**
```bash
curl -X POST "https://api.worldlabs.ai/marble/v1/worlds:generate" \
  -H "WLT-Api-Key: $WLT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "model": "marble-1.1",
    "prompt": "Interior of a residential attached garage, single car, concrete floor, drywall walls. A 200-amp electrical service panel is mounted on the back wall, panel cover is open showing circuit breakers inside. Fluorescent shop light overhead. Wooden workbench along the right wall with a standard duplex receptacle above it. Exposed NM cable (Romex) running along ceiling joists to a junction box. Water heater in the corner near the panel. Realistic residential construction, warm overhead lighting, slight clutter typical of a working garage."
  }'
```

**Environment 2: Kitchen and Bathroom (Scenario 2 — Branch Circuits)**
```bash
curl -X POST "https://api.worldlabs.ai/marble/v1/worlds:generate" \
  -H "WLT-Api-Key: $WLT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "model": "marble-1.1",
    "prompt": "Open-plan residential interior showing a modern kitchen and adjacent hallway leading to a bathroom. Kitchen has granite countertops with multiple duplex receptacle outlets visible along the backsplash, under-cabinet lighting, a dishwasher alcove, and a garbage disposal under the sink. The bathroom door is open showing a vanity with a mirror and a duplex outlet next to the sink. Bedroom doorway visible down the hall. Standard residential construction with drywall, hardwood floors in kitchen, tile in bathroom. Overhead recessed lighting. Electrical outlet cover plates are clearly visible."
  }'
```

**Environment 3: Utility Room / Grounding (Scenario 3)**
```bash
curl -X POST "https://api.worldlabs.ai/marble/v1/worlds:generate" \
  -H "WLT-Api-Key: $WLT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "model": "marble-1.1",
    "prompt": "Residential utility room and exterior service entrance area. Inside: an electrical sub-panel on the wall, copper water pipes running along the ceiling and down the wall, a ground wire (bare copper) running from the panel down to the floor. The wall has an opening or cutaway showing the exterior side where a copper ground rod is driven into the earth next to the foundation, with a ground clamp and conductor visible. A water meter is visible on the main water pipe. Concrete block foundation walls, exposed ceiling joists with NM cables. Utilitarian fluorescent lighting. Construction-phase look with some unfinished areas."
  }'
```

**Environment 4: Panel Design Workshop (Sandbox)**
```bash
curl -X POST "https://api.worldlabs.ai/marble/v1/worlds:generate" \
  -H "WLT-Api-Key: $WLT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "model": "marble-1.1",
    "prompt": "Electrical training workshop classroom. A large wooden workbench in the center with a blank 200-amp electrical panel mounted on a board at comfortable working height. The panel door is open and the interior is empty, ready for breaker installation. A parts tray to the left holds various circuit breakers organized by size. Spools of different colored NM cable (white 14-gauge, yellow 12-gauge, orange 10-gauge) on a rack behind the bench. Well-lit industrial classroom with pegboard tool storage on the walls, good overhead lighting. Clean and organized training environment."
  }'
```

**Environment 5: Main Menu Environment**
```bash
curl -X POST "https://api.worldlabs.ai/marble/v1/worlds:generate" \
  -H "WLT-Api-Key: $WLT_API_KEY" \
  -H "Content-Type: application/json" \
  -d '{
    "model": "marble-1.1",
    "prompt": "Professional electrical training facility lobby. Modern industrial design with exposed brick accent wall, polished concrete floor, and warm lighting. A large workbench display shows an opened electrical panel with neatly organized breakers and color-coded wiring as a showpiece. Motivational posters about electrical safety on the walls. Clean, professional, and inviting atmosphere. Reception area feel with good depth for a 3D menu environment."
  }'
```

### 2.3 Check Generation Status

```bash
# Replace OPERATION_ID with the ID returned from the generate call
curl -X GET "https://api.worldlabs.ai/marble/v1/operations/OPERATION_ID" \
  -H "WLT-Api-Key: $WLT_API_KEY"
```

Wait ~5 minutes per generation. When `status` is `"SUCCEEDED"`, proceed.

### 2.4 Download Assets

```bash
# Get world details (includes download URLs)
curl -X GET "https://api.worldlabs.ai/marble/v1/worlds/WORLD_ID" \
  -H "WLT-Api-Key: $WLT_API_KEY"
```

From the response, download:
- **`collider_mesh_url`** → save as `Assets/_Project/Models/Environments/{SceneName}_collider.glb`
- **`splat_url`** (or `splat_500k_url` for performance) → save as `Assets/_Project/Models/Environments/{SceneName}.spz`
- **`pano_url`** → save as `Assets/_Project/Textures/Environments/{SceneName}_pano.jpg` (for skybox)

### 2.5 Import to Unity

1. GLB collider meshes import automatically when placed in Assets
2. For Gaussian splats:
   - Select the .spz file in Project window
   - Unity (with GaussianSplatting package) will create a GaussianSplatAsset
   - Drag into scene to create a GaussianSplatRenderer object
3. For performance on zSpace:
   - **Use the 500k splat** (not the 2M full resolution) — critical for 90fps stereo
   - If 500k is still too heavy, export the collider mesh and use it as the visual with standard Unity materials instead

---

## Phase 3: Build Scenes (Days 2-5)

### 3.1 Boot Scene

1. **File → New Scene** → save as `Assets/_Project/Scenes/Boot/Boot.unity`
2. Delete the default Camera
3. Add ZCamera (from zCore package)
4. Create empty GO → add `BootSequence` component
5. Drag GameManager prefab into scene
6. Add NECDatabase prefab (create empty GO with `NECDatabase` component)
7. **File → Build Settings → Add Open Scenes** (Boot must be scene index 0)

### 3.2 MainMenu Scene

1. **File → New Scene** → save as `Assets/_Project/Scenes/MainMenu/MainMenu.unity`
2. Delete default Camera, add ZCamera
3. Place the main menu World Labs environment (splat + collider)
4. Create a World Space Canvas (1920×1080, scale 0.001):
   - **Ensure Canvas → Render Mode = World Space** (critical for zSpace)
   - Add `MainMenuPanel` component
   - Create child panels: ModeSelection, ScenarioSelection, ScenarioDetail, Difficulty, Settings
   - Wire all TMP text fields and buttons to MainMenuPanel serialized fields
5. Create a second World Space Canvas for `ProgressDashboardPanel`
6. Create a third for `QuickReferenceCardPanel`
7. Add `SceneTransitionManager` to a DontDestroyOnLoad GO (or the GameManager prefab)
8. Add `AudioManager` to the GameManager prefab
9. Wire `MainMenuPanel._scenarioCatalog` → ScenarioCatalog asset

### 3.3 Inspection Scene — Residential Panel (Scenario 1 / Branch Circuits)

This is the most important scene. Build it first as the template for all other inspection scenes.

1. **File → New Scene** → save as `Assets/_Project/Scenes/Inspection/BranchCircuitInspection.unity`
2. Delete default Camera, add ZCamera
3. **Place World Labs environment:**
   - Drag the kitchen/bathroom splat into the scene
   - Position the collider mesh at the same location (for physics)
   - Set collider mesh renderer to invisible (just colliders)
4. **Create interactive electrical components** as child GameObjects. Each needs:
   - `InspectableComponent` script
   - A simple 3D mesh (cube/cylinder placeholder OR modeled receptacle)
   - A `Collider` (BoxCollider or MeshCollider)
   - `Clickable` component (for pointer interaction)
   - **GameObject name must EXACTLY match `componentObjectName` in ViolationDefinitionSO**

   Create these GameObjects for Branch Circuit scenario:
   ```
   BathroomReceptacle_Sink        (cube, position at bathroom sink)
   KitchenReceptacle_Counter1     (cube, position at kitchen counter)
   GarageReceptacle_Workbench     (cube, position at workbench)
   LivingRoom_WallGap             (transparent plane, mark the gap area)
   KitchenCircuit_Wire            (cylinder, wire running from breaker)
   BedroomCircuit_Breaker         (cube, inside panel)
   LivingRoomCircuit_Breaker      (cube, inside panel)
   KitchenCountertop_Gap          (transparent plane, counter gap)
   BathroomCircuit_SharedBreaker  (cube, inside panel)
   KitchenPanel_SmallAppliance    (cube, inside panel)
   KitchenWall_CableBundleHole    (small cube, in wall framing)
   KitchenDishwasher_Circuit      (cube, behind dishwasher)
   ```

5. **Add the inspection system to the scene root:**
   - Create empty GO named `InspectionSystem`
   - Add `InspectionManager` component → wire `_scenarioDefinition` to ScenarioDefinition_BranchCircuits asset
   - Add `InspectionScenarioRunner` component
   - Add `InspectionPointerController` component

6. **Create UI panels** (all World Space Canvas):
   - `InspectionHUD` — intro panel + inspection panel + timer
   - `ViolationFlaggingPanel` — shown when student clicks a component
   - `InspectionReviewPanel` — shown after inspection complete
   - `NECReferencePanel` — searchable code reference

   Wire each panel's serialized fields (TMP text, buttons, content areas) to the Runner.

7. **Add virtual tools:**
   - Create `ToolBelt` GO with Flashlight and Multimeter child objects
   - Wire `ObjectPointer` to attach tools to stylus tip

8. **Test the flow:**
   - Press Play
   - IntroStep should appear showing scenario description
   - Click Continue → FreeInspectionStep starts
   - Hover over components → cyan highlight
   - Click component → ViolationFlaggingPanel appears
   - Submit a violation → flagged count updates
   - Click Finish → ReviewStep shows results
   - NECReviewStep walks through missed violations
   - ScoreStep shows letter grade

### 3.4 Inspection Scene — Grounding (Scenario 3)

Same pattern as above but with the utility room environment:

1. New scene: `Assets/_Project/Scenes/Inspection/GroundingInspection.unity`
2. Place utility room World Labs environment
3. Create GameObjects matching Grounding violation names:
   ```
   GroundRod_Main
   GEC_ServicePanel
   WaterPipe_Bonding
   EGC_SubPanel
   Electrode_BondingJumper
   ServiceGround_Connection
   GEC_AluminumRun
   WaterPipe_Underground
   ServicePanel_IntersystemBond
   GroundRod_Supplemental
   ```
4. Add InspectionManager → wire to ScenarioDefinition_Grounding
5. Add Runner + UI panels (can be prefabs reused from Scenario 2)

### 3.5 Panel Sandbox Scene

1. New scene: `Assets/_Project/Scenes/PanelSandbox/PanelSandbox.unity`
2. Place workshop World Labs environment
3. **Create the panel board:**
   - Empty GO `PanelBoard` with child `BreakerSlot` GOs (20 left + 20 right = 40 slots)
   - Each BreakerSlot needs: `BreakerSlot` script, `DropTarget` script, BoxCollider
   - Arrange in 2 columns (left bus, right bus), evenly spaced vertically
4. **Create the breaker tray:**
   - GO `BreakerTray` with child breaker prefabs
   - Each breaker: `PlacedBreaker` script, `SnappingDraggablePlane` script, visual mesh
   - Create breaker types: 15A, 20A, 20A-GFCI, 20A-AFCI, 20A-DualFunction, 30A-2P, 50A-2P
5. **Create wire connection points:**
   - For each breaker slot, create a `WireConnection` GO with LineRenderer
   - Create load terminal points on the environment walls (one per required circuit)
6. **Add PanelDesignManager** to scene root → wire `_definition` to PanelDesign_Residential200A, wire `_slots` array
7. **Add PanelDesignRunner** → wire `_hud` to PanelDesignHUD
8. **Create PanelDesignHUD** World Space Canvas with all 5 panels

---

## Phase 4: Prefabs & Reuse (Day 3-4)

### 4.1 Create Reusable Prefabs

Save these as prefabs for reuse across scenes:

| Prefab | Components | Location |
|--------|-----------|----------|
| `InspectionUI` | InspectionHUD + ViolationFlaggingPanel + ReviewPanel + NECReferencePanel | `Prefabs/UI/` |
| `ToolBelt` | ToolBelt + Flashlight + Multimeter + ObjectPointer | `Prefabs/Tools/` |
| `Receptacle_Standard` | Mesh + InspectableComponent + Clickable + Collider | `Prefabs/Components/` |
| `Receptacle_GFCI` | Same + GFCI visual (TEST/RESET buttons) | `Prefabs/Components/` |
| `Breaker_Single` | Mesh + PlacedBreaker + SnappingDraggablePlane | `Prefabs/Components/` |
| `Breaker_Double` | Same but 2-pole visual | `Prefabs/Components/` |
| `BreakerSlot` | Mesh + BreakerSlot + DropTarget + Collider | `Prefabs/Components/` |
| `WireSegment` | WireConnection + LineRenderer + Grabbable endpoints | `Prefabs/Components/` |
| `GroundRod` | Mesh + InspectableComponent + Clickable | `Prefabs/Components/` |

### 4.2 Create Placeholder 3D Meshes

If World Labs environments don't include inspectable electrical components (they likely won't), create simple placeholder meshes:

- **Receptacle:** Flattened cube (0.07 × 0.12 × 0.02 m) with face texture
- **Breaker:** Small rectangle (0.02 × 0.05 × 0.08 m)
- **Wire:** Cylinder (radius 0.003, length variable)
- **Ground rod:** Long cylinder (radius 0.008, length 2.4 m)
- **Junction box:** Cube (0.1 × 0.1 × 0.05 m)

These can be Unity primitives with colored materials — functional for alpha, replaced with modeled assets later.

---

## Phase 5: Build Settings & Testing (Day 5-6)

### 5.1 Configure Build Settings

1. **File → Build Settings**
2. Add scenes in order:
   ```
   0: Boot
   1: MainMenu
   2: BranchCircuitInspection
   3: GroundingInspection
   4: PanelSandbox
   ```
3. **Platform: Windows** (already default)
4. **Player Settings:**
   - Company Name: zSpace
   - Product Name: NEC Code Inspector
   - Default resolution: 1920×1080
   - Fullscreen Mode: Fullscreen Window
   - Color Space: Linear (required for zSpace and URP)
   - Scripting Backend: IL2CPP (for release) or Mono (for faster iteration)
   - Graphics: URP Asset assigned in **Edit → Project Settings → Graphics**
   - Quality: URP Renderer set for each quality level

### 5.2 zSpace Configuration

1. Ensure ZCamera is in every scene (replacing default Camera)
2. Verify ZCore initializes in Boot scene
3. Set ZCamera's near clip to 0.01, far clip to 100
4. Configure stylus button mapping:
   - Button 0 (front): Primary action (inspect/flag)
   - Button 1 (middle): Secondary action (mark compliant)
   - Button 2 (back): Tool toggle

### 5.3 Performance Checklist

- [ ] Gaussian splats using 500k (not 2M) resolution
- [ ] Draw calls < 200 per frame (check with Frame Debugger)
- [ ] UI canvases batching properly (no per-frame rebuilds)
- [ ] Object pooling for instantiated UI list items
- [ ] Disable shadows on non-essential objects
- [ ] Profiler shows consistent 90fps in stereo

### 5.4 Playtest Checklist

- [ ] Boot → MainMenu transition works
- [ ] Difficulty selection persists
- [ ] Scenario 2 (Branch Circuits): all 12 violations detectable at Expert, 5 at Beginner
- [ ] Scenario 3 (Grounding): all 10 violations detectable at Expert, 4 at Beginner
- [ ] Panel Sandbox: can place all 12 required breakers, compliance checker runs, score saves
- [ ] Violation flagging panel shows correct citation mode per difficulty
- [ ] NEC Reference Panel search works
- [ ] Quick Reference Cards display correctly
- [ ] Progress saves across sessions (quit and relaunch)
- [ ] Stylus hover/click works on all InspectableComponents
- [ ] Virtual flashlight/multimeter toggle and function

---

## Phase 6: Alpha Polish (Days 6-8)

### 6.1 Audio

Source free/licensed audio clips and assign to AudioManager:

| Slot | Description | Suggested Source |
|------|-------------|-----------------|
| _buttonClick | UI click feedback | Short click/tap |
| _buttonHover | Hover feedback | Soft tick |
| _success | Correct flag/pass | Pleasant chime |
| _error | False positive/fail | Low buzz |
| _flagViolation | Flag submitted | Stamp/mark sound |
| _compliancePass | Rule passed | Positive ding |
| _complianceFail | Rule failed | Warning tone |
| _flashlightToggle | Light on/off | Switch click |
| _multimeterProbe | Probe touch | Electronic beep |
| _breakerSnap | Breaker placed | Mechanical snap |
| _breakerRemove | Breaker removed | Mechanical release |
| _wireConnect | Wire connected | Insertion click |
| _ambientElectrical | Background hum | 60Hz electrical hum, very low |
| _ambientWorkshop | Workshop background | Quiet workshop ambience |

### 6.2 Tutorial Overlay

For the alpha demo, create a simple tutorial as an IntroStep enhancement:
- Show stylus button mapping diagram on first launch
- "Hover to inspect, click to open flagging panel"
- "Use the NEC Reference button for code lookup"
- Stored in PlayerPrefs so it only shows once

### 6.3 Build & Deploy

```
File → Build and Run
```

Test on zSpace Inspire 2 hardware. Verify stereo rendering, stylus tracking, and 90fps.

---

## Alpha Milestone Checklist

| Feature | Target |
|---------|:------:|
| Boot → MainMenu flow | Working |
| Difficulty selection (3 levels) | Working |
| Scenario 2: Branch Circuits (12 violations) | Playable |
| Scenario 3: Grounding & Bonding (10 violations) | Playable |
| Panel Design Sandbox (12 circuits, 10 rules) | Playable |
| Scoring + letter grades | Working |
| NEC Reference Panel (66 articles) | Working |
| Quick Reference Cards (10) | Working |
| Progress persistence | Working |
| Virtual Flashlight + Multimeter | Working |
| zSpace stereo rendering at 90fps | Verified |
| Stylus input for all interactions | Verified |
| World Labs environments (5) | Integrated |
| Audio feedback (SFX + ambient) | Present |

**Estimated total time:** 6-8 working days from opening Unity to alpha build.

---

## World Labs Credit Budget Summary

| Use | Generations | Credits | Cost |
|-----|:----------:|:-------:|-----:|
| Environment 1: Garage/Panel (5 variations) | 5 | 7,900 | $6.32 |
| Environment 2: Kitchen/Bathroom (5 variations) | 5 | 7,900 | $6.32 |
| Environment 3: Utility Room/Grounding (5 variations) | 5 | 7,900 | $6.32 |
| Environment 4: Workshop/Sandbox (5 variations) | 5 | 7,900 | $6.32 |
| Environment 5: Main Menu Lobby (3 variations) | 3 | 4,740 | $3.79 |
| Refinement iterations | 15 | 23,700 | $18.96 |
| Buffer / experiments | 10 | 15,800 | $12.64 |
| **Alpha Total** | **48** | **75,840** | **~$61** |

Initial $50 purchase (62,500 credits) covers the first 4 environments. One additional $15-25 purchase covers the rest.
