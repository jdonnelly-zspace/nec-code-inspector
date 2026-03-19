# 3D Asset Sources & Attribution

## Electrical Components

| Asset | Source | License | Format | Notes |
|-------|--------|---------|--------|-------|
| Electrical Panel | TBD - GrabCAD / Sketchfab | CC-BY / Free | FBX/OBJ | 200A residential panel |
| Circuit Breakers | TBD - CGTrader / 3D ContentCentral | Free | FBX/OBJ | Single-pole, double-pole, GFCI, AFCI |
| Conduit (EMT) | Procedural + GrabCAD | N/A / Free | Procedural mesh | LineRenderer-based |
| NM-B Cable | Procedural | N/A | Procedural mesh | CableRenderer-based |
| Receptacles | TBD - Sketchfab CC | CC-BY | FBX/OBJ | Standard, GFCI, weather-resistant |
| Switches | TBD - CGTrader free | Free | FBX/OBJ | Single-pole, 3-way |
| Junction Boxes | TBD - GrabCAD | Free | FBX/OBJ | Metal and plastic |
| Ground Rods | Simple custom model | N/A | Unity primitives | Cylinder + clamp |

## Virtual Tools

| Asset | Source | License | Format |
|-------|--------|---------|--------|
| Flashlight | TBD - Sketchfab CC | CC-BY | FBX |
| Digital Multimeter | TBD - Sketchfab CC | CC-BY | FBX |
| Clamp Meter | TBD - GrabCAD | Free | FBX |
| NCV Tester | Simple custom model | N/A | Unity primitives |
| Inspection Mirror | Simple custom model | N/A | Unity primitives |

## Environments

| Asset | Source | License | Notes |
|-------|--------|---------|-------|
| Garage Interior | TBD - Unity Asset Store free | Free | Residential service panel location |
| Kitchen/Bathroom | TBD - Unity Asset Store free | Free | Branch circuit scenario |
| Utility Room | TBD - Unity Asset Store free | Free | Grounding scenario |
| Mechanical Room | TBD - Unity Asset Store free | Free | Commercial scenario |

## Model Import Notes

- CAD models (STEP format) convert via FreeCAD or Blender to FBX before Unity import
- Set import scale to 0.01 if model is in centimeters
- Generate lightmap UVs on import
- Apply URP Lit shader materials
- Add MeshCollider or BoxCollider as appropriate
- MVP uses placeholder primitives (cubes/cylinders with colored materials) until real models are sourced
