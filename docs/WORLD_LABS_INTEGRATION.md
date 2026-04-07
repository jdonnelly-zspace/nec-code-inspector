# World Labs AI Integration — NEC Code Inspector

## What World Labs Marble Does

World Labs' [Marble API](https://docs.worldlabs.ai/api) generates **photorealistic 3D environments** from text prompts, images, or video. It outputs:

- **3D Gaussian Splats** (SPZ/PLY format) — importable into Unity via [UnityGaussianSplatting](https://github.com/aras-p/UnityGaussianSplatting)
- **Collider meshes** (GLB) — for physics/interaction
- **Panoramic imagery** — for skyboxes and context
- Generation takes ~5 minutes per environment

## Opportunity for NEC Code Inspector

### 1. AI-Generated Inspection Environments (High Impact)
Instead of manually modeling electrical environments (garages, kitchens, utility rooms, commercial spaces), use Marble to generate photorealistic base environments from reference photos or text prompts.

**Use cases:**
- "Residential garage with electrical panel on back wall, exposed wiring, workbench"
- "Kitchen with countertop outlets, under-cabinet lighting, dishwasher alcove"
- "Commercial mechanical room with large electrical service entrance"
- "Outdoor patio with weatherproof receptacle boxes and landscape lighting"

**Workflow:** Generate environment → Export GLB mesh + splat → Import to Unity → Overlay interactive InspectableComponent objects on top of the AI environment

### 2. Scenario Variety at Scale (Medium Impact)
Currently each scenario needs hand-built 3D scenes. With Marble, we could generate dozens of environment variations:
- Different house styles (ranch, colonial, modern)
- Different room layouts
- Different lighting conditions
- Construction vs. finished environments

This means students see fresh environments each time, preventing memorization.

### 3. Student-Created Scenarios (Future / Lower Priority)
Let advanced students describe an environment and generate it, then populate it with violations for peer review.

## Credit Estimate

### Pricing
- **$1.00 USD per 1,250 credits** ([pricing docs](https://docs.worldlabs.ai/api/pricing))
- Marble 1.1: ~1,500-1,600 credits per generation ($1.20-1.28 each)
- Marble 1.1 Plus (larger worlds): ~1,500-3,100 credits ($1.20-2.48 each)
- Credits never expire
- Minimum purchase: $5.00 (6,250 credits)

### Estimated Needs

| Phase | Environments | Iterations | Total Generations | Credits | Cost |
|-------|:-----------:|:----------:|:-----------------:|:-------:|-----:|
| **Prototyping** (test pipeline) | 3 | 5 each | 15 | 24,000 | ~$19 |
| **MVP Scenes** (5 scenarios) | 5 | 8 each | 40 | 64,000 | ~$51 |
| **Alpha Variations** (3 per scenario) | 15 | 3 each | 45 | 72,000 | ~$58 |
| **Beta Polish** (refinement passes) | 20 | 2 each | 40 | 64,000 | ~$51 |
| **Buffer** (failures, experiments) | — | — | 30 | 48,000 | ~$38 |
| **Total** | | | **170** | **272,000** | **~$218** |

**Recommendation:** Start with $50 (62,500 credits) for prototyping + MVP. Total project budget: **$250** for all environment generation through beta.

### Subscription Alternative
- Pro tier: $35/month for 25 generations + commercial rights
- Over 4 months of development: $140 for 100 generations
- API credits more cost-effective if generating in bursts

## Technical Integration Path

1. **Generate** environments via API (text prompt + reference photos)
2. **Export** GLB mesh (for colliders) + SPZ splat (for visuals)
3. **Import** into Unity using [UnityGaussianSplatting plugin](https://github.com/aras-p/UnityGaussianSplatting)
4. **Overlay** interactive GameObjects (InspectableComponent, BreakerSlot, etc.) positioned within the AI environment
5. **Bake** lighting and optimize for 90fps zSpace stereo target

### Risks & Mitigations
| Risk | Mitigation |
|------|-----------|
| Gaussian splats too heavy for 90fps stereo | Use 500k splat (vs 2M), or bake to mesh |
| Generated rooms don't match NEC layout needs | Use reference photos of real electrical installations as input |
| Interactive objects clip with AI geometry | Use collider mesh for placement, tune per-scene |
| Visual quality inconsistent | Iterate with prompt refinement, keep best generations |

## Other AI Integration Opportunities

Beyond World Labs, consider these AI enhancements:

| Feature | AI Technology | Impact |
|---------|--------------|--------|
| **Adaptive difficulty** | ML model on student performance data | Auto-adjusts violation count, hint frequency, time limits |
| **NEC natural language Q&A** | Claude API (RAG over NEC database) | Students ask "why is GFCI required here?" and get contextual answers |
| **Procedural violation placement** | Rule-based + ML | Generate unique violation combinations per session |
| **Student performance analytics** | Clustering on score patterns | Identify weak NEC chapters, recommend targeted practice |
| **Voice narration** | TTS (ElevenLabs, Azure) | Instructor-style guidance for Beginner mode |
| **Photo-to-scenario** | Vision AI + Marble | Teacher uploads classroom photo → becomes inspection scenario |
