# NEC Code Inspector — Content Roadmap

## Prioritization Framework

Each content area is scored on three axes (1-5 scale):

| Axis | What it measures |
|------|-----------------|
| **zSpace AR/VR Advantage** | How much does 3D stereoscopic interaction improve learning vs. textbook/video? Topics that are inherently spatial, hidden from view, or require physical manipulation score highest. |
| **Exam Importance** | How heavily tested on journeyman and master electrician licensing exams (PSI/Prometric)? |
| **Safety Impact** | How critical is the topic for preventing real-world electrical hazards, injuries, or fire? |

**Priority tiers:**
- **P0** — Ship first. Highest combined score. The reason someone buys this product.
- **P1** — Alpha milestone. High-value content that rounds out the core experience.
- **P2** — Beta milestone. Broadens coverage for exam prep completeness.
- **P3** — Post-release. Deep specialization and advanced topics.

**Status key:** Built = in current codebase | Partial = some articles exist | Planned = not yet built

---

## P0 — Ship First (Core Product)

These topics deliver the strongest "you can't learn this from a textbook" moment on zSpace.

### 1. Electrical Panel Internals & Design
**zSpace: 5 | Exam: 5 | Safety: 5**

Students reach inside a 3D electrical panel — open the cover, examine bus bars, trace circuits, manipulate breakers with the stylus. This is the single most compelling zSpace demo. A textbook shows a flat photo; zSpace lets you look behind the breakers.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| **Inspection: Residential Service Panel** | | |
| Mechanical workmanship | Art. 110.12 | Built |
| Electrical connections / terminals | Art. 110.14, 110.14(A) | Built |
| Service disconnect location | Art. 230.70(A) | Built |
| Service disconnect rating | Art. 230.79 | Built |
| Overcurrent protection of conductors | Art. 240.4, 240.4(B), 240.4(D) | Built |
| Overcurrent device accessibility | Art. 240.24(A) | Built |
| Circuit directory / labeling | Art. 408.4 (2026 change) | Built |
| Panelboard bus rating / spaces | Art. 408.36 | Built |
| **Sandbox: Panel Design** | | |
| Breaker/conductor matching | Art. 240.4 | Built |
| Required branch circuits | Art. 210.11(C)(1-3) | Built |
| GFCI breaker selection | Art. 210.8 | Built |
| AFCI breaker selection | Art. 210.12 | Built |
| Load balance (bus sides) | General practice | Built |
| Main breaker sizing | Art. 230.79 | Built |
| Panel space limits | Art. 408.36 | Built |
| Wire connections check | General practice | Built |
| Conductor ampacity match | Art. 310.14 | Built |
| No double-tapped breakers | Art. 110.14 | Built |
| **Sandbox: Load Calculation** | | |
| General lighting (3 VA/sqft) | Art. 220.12 | Built |
| Small-appliance loads | Art. 220.52 | Built |
| Demand factors | Art. 220.42 (Table) | Built |
| Dryer load | Art. 220.54 | Built |
| Range/cooking demand | Art. 220.55 | Built |
| Optional calculation | Art. 220.82, 220.83 | Built |
| Service/feeder calculations | Art. 220.40 | Built |

**zSpace moment:** Student opens panel door with stylus, leans in to read breaker labels in stereo 3D, traces a wire from breaker to its junction box, uses virtual multimeter to test voltage. No other training tool offers this.

---

### 2. Working Space & Clearances
**zSpace: 5 | Exam: 4 | Safety: 5**

The #1 topic that benefits from 3D spatial understanding. Students stand in front of a virtual panel and see the required clearance envelope rendered as a translucent 3D volume. They can walk around it, see what's violating the space, and understand the 30"×36"×6.5' zone intuitively — something impossible on paper.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Minimum clear distance (depth) | Art. 110.26(A)(1) | Built |
| Working space width | Art. 110.26(A)(2) | Built |
| Working space height | Art. 110.26(A)(3) | Built |
| Dedicated equipment space | Art. 110.26(E) | Planned |
| Illumination of working space | Art. 110.26(D) | Planned |
| Entrance to working space | Art. 110.26(C) | Planned |
| Guarding of live parts | Art. 110.27 | Planned |

**zSpace moment:** A translucent blue box shows the required clearance zone. Red highlights appear on objects (shelving, water heater) that intrude into the space. Student can rotate around the panel to see violations from every angle.

---

### 3. GFCI & AFCI Protection
**zSpace: 4 | Exam: 5 | Safety: 5**

The most heavily tested safety topic on licensing exams. zSpace advantage: walk room-by-room through a dwelling, inspecting each receptacle in context — bathroom sink, kitchen countertop, garage workbench, outdoor deck. The spatial room-by-room walkthrough is far more memorable than a table of requirements.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| **GFCI — Dwelling Unit Locations** | | |
| GFCI general requirement | Art. 210.8(A) (2026: expanded voltage) | Built |
| Bathrooms | Art. 210.8(A)(1) | Built |
| Garages / accessory buildings | Art. 210.8(A)(2) | Built |
| Outdoors | Art. 210.8(A)(3) | Built |
| Kitchens (countertop) | Art. 210.8(A)(5) | Built |
| Laundry areas | Art. 210.8(A)(7) | Built |
| Bathtubs / shower stalls (6 ft rule) | Art. 210.8(A)(9) | Built |
| Dishwashers (2026 NEW) | Art. 210.8(D) | Built |
| Crawl spaces | Art. 210.8(A)(4) | Planned |
| Unfinished basements | Art. 210.8(A)(6) | Planned |
| Boathouses | Art. 210.8(A)(8) | Planned |
| **AFCI — Dwelling Unit** | | |
| AFCI required rooms | Art. 210.12(A) (2026: updated list) | Built |
| Branch circuit extensions | Art. 210.12(B) | Built |
| **GFCI — Non-Dwelling** | | |
| Commercial kitchens | Art. 210.8(B)(1) | Planned |
| Sinks (non-dwelling) | Art. 210.8(B)(5) | Planned |
| GFCI replacement receptacles | Art. 406.4(D) | Built |
| **Violations (Scenario 2)** | | |
| Missing GFCI — bathroom | BC-GFCI-BATH-001 (Beginner) | Built |
| Missing GFCI — kitchen | BC-GFCI-KITCHEN-001 (Beginner) | Built |
| Missing GFCI — garage | BC-GFCI-GARAGE-001 (Beginner) | Built |
| Missing GFCI — dishwasher (2026) | BC-GFCI-DISHWASHER-001 (Expert) | Built |
| Missing AFCI — bedroom | BC-AFCI-BEDROOM-001 (Standard) | Built |
| Missing AFCI — living room | BC-AFCI-LIVING-001 (Standard) | Built |

**zSpace moment:** Walk through each room of a house. In the bathroom, lean down to inspect the receptacle at the sink — no TEST/RESET buttons. Flag it, cite Art. 210.8(A)(1). Move to the kitchen, check the countertop outlet behind the coffee maker.

---

### 4. Grounding & Bonding
**zSpace: 5 | Exam: 5 | Safety: 5**

Grounding is the hardest NEC topic to visualize — conductors disappear underground, connections are hidden behind panels, and the system spans from the utility transformer to the earth electrode. AR/VR makes the invisible visible: show the complete path from service to electrode in transparent 3D.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| System grounding purpose | Art. 250.4(A)(1) | Built |
| Grounding electrode conductor (GEC) | Art. 250.24(A)(1) | Built |
| Grounding electrode system | Art. 250.50 | Built |
| Water pipe electrode (10 ft rule) | Art. 250.52(A)(1) | Built |
| Ground rod (8 ft, 5/8" diameter) | Art. 250.52(A)(5) | Built |
| Supplemental electrode required | Art. 250.53(A)(2) | Built |
| GEC installation — aluminum restriction | Art. 250.64(A) | Built |
| Intersystem bonding termination | Art. 250.94 | Built |
| Bonding of metal water piping | Art. 250.104(A) | Built |
| Equipment grounding conductor sizing | Art. 250.122 | Built |
| Concrete-encased electrode (Ufer) | Art. 250.52(A)(3) | Planned |
| GEC sizing (Table 250.66) | Art. 250.66 | Planned |
| Bonding of structural steel | Art. 250.104(C) | Planned |
| Main bonding jumper | Art. 250.28 | Planned |
| Grounding electrode conductor protection | Art. 250.64(B) | Planned |
| **Violations (Scenario 3)** | | |
| Ground rod too short | GND-ELECTRODE-001 (Beginner) | Built |
| GEC disconnected at panel | GND-GEC-001 (Beginner) | Built |
| Water pipe not bonded | GND-BOND-WATER-001 (Beginner) | Built |
| EGC undersized | GND-EGC-SIZE-001 (Beginner) | Built |
| Electrodes not bonded together | GND-ELECTRODE-SYS-001 (Standard) | Built |
| Corroded system ground | GND-SYSTEM-001 (Standard) | Built |
| Aluminum GEC in earth | GND-GEC-ALUM-001 (Standard) | Built |
| Water pipe insufficient earth contact | GND-WATERPIPE-001 (Standard) | Built |
| Missing intersystem bonding | GND-INTERSYSTEM-001 (Expert) | Built |
| Single rod without supplement | GND-SUPPLEMENT-001 (Expert) | Built |

**zSpace moment:** X-ray view through the foundation wall shows the GEC path underground. Student follows the conductor from panel → through wall → to ground rod. Transparent earth reveals the 8-foot rod depth. A second view shows the water pipe electrode transitioning to plastic — student must determine if it qualifies.

---

### 5. Conductor Sizing & Overcurrent Protection
**zSpace: 3 | Exam: 5 | Safety: 5**

The single most tested topic on licensing exams. While tables are the core skill, zSpace adds value by letting students trace conductors through panels, see wire gauge differences physically, and connect the abstract table lookups to real wires they can inspect and measure with the virtual multimeter.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Conductor ampacity tables | Art. 310.14, 310.16 | Built |
| Small conductor limits (14/12/10 AWG) | Art. 240.4(D) | Built |
| Next standard size overcurrent | Art. 240.4(B) | Built |
| General overcurrent protection | Art. 240.4 | Built |
| NM cable ampacity (bundling derate) | Art. 334.80 | Built |
| Temperature correction factors | Art. 310.15(B) | Planned |
| Adjustment factors (conduit fill) | Art. 310.15(C)(1) | Planned |
| Conductor sizing for motors | Art. 430.22 | Planned |
| Standard fuse/breaker ratings | Art. 240.6 | Planned |
| Tap rules (10 ft / 25 ft) | Art. 240.21(B) | Planned |
| **Violations** | | |
| 14 AWG on 20A breaker | BC-WIRE-14AWG-001 (Beginner) | Built |
| NM cables bundled without derate | BC-WIRE-KITCHEN-001 (Expert) | Built |

**zSpace moment:** Student uses virtual multimeter on a wire, reads 14 AWG. Traces it to the panel — connected to a 20A breaker. The physical act of measuring, tracing, and comparing builds muscle memory for real inspections.

---

## P1 — Alpha Milestone

High-value content that rounds out the core experience for early testing.

### 6. Branch Circuit Requirements & Receptacle Spacing
**zSpace: 4 | Exam: 4 | Safety: 3**

Walking through rooms and measuring receptacle spacing in 3D is inherently more intuitive than calculating on a floor plan. Students learn the 6-foot wall rule and 24-inch countertop rule by physically observing gaps.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| General wall spacing (6 ft rule) | Art. 210.52(A) | Built |
| Small-appliance circuits (2 required) | Art. 210.52(B) | Built |
| Countertop spacing (24 in rule) | Art. 210.52(C) | Built |
| Island/peninsula countertops | Art. 210.52(C)(5) | Built |
| Bathroom receptacles (36 in from basin) | Art. 210.52(D) | Built |
| Outdoor receptacles (front + back) | Art. 210.52(E)(1) | Built |
| Basement/garage receptacles | Art. 210.52(G) | Built |
| Dedicated bathroom circuit | Art. 210.11(C)(1) | Built |
| Dedicated laundry circuit | Art. 210.11(C)(2) | Built |
| Kitchen small-appliance circuits | Art. 210.11(C)(3) | Built |
| Hallway receptacles | Art. 210.52(H) | Planned |
| Dwelling unit HVAC outlet | Art. 210.63 | Planned |
| **Violations (Scenario 2)** | | |
| Wall receptacles 14ft apart | BC-SPACING-WALL-001 (Beginner) | Built |
| Countertop gap exceeds 24" | BC-SPACING-COUNTER-001 (Standard) | Built |
| Shared bathroom circuit | BC-DEDICATED-BATH-001 (Standard) | Built |
| Only one small-appliance circuit | BC-DEDICATED-KITCHEN-001 (Standard) | Built |

---

### 7. Swimming Pools, Spas & Hot Tubs
**zSpace: 5 | Exam: 3 | Safety: 5**

One of the highest-value AR/VR topics. The NEC defines invisible safety zones around pools measured in feet — distances for receptacles, lighting, bonding grids. Seeing these zones as translucent 3D volumes in stereoscopic view is transformative. Students literally stand at the pool edge and see the 5-foot, 10-foot, and 20-foot zones radiating outward.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Luminaire height over pool (12 ft) | Art. 680.22(A)(1) | Built |
| Equipotential bonding grid | Art. 680.26 | Built |
| Receptacles near pools (20 ft rule) | Art. 680.22(A)(5) | Planned |
| GFCI for pool equipment | Art. 680.22(B) | Planned |
| Underground wiring near pools | Art. 680.10 | Planned |
| Spa/hot tub disconnect | Art. 680.41 | Planned |
| Bonding of metal parts | Art. 680.26(B) | Planned |
| Storable pool requirements | Art. 680.31-33 | Planned |

**zSpace moment:** Semi-transparent blue zones radiate from pool edge — 5 ft equipment zone, 10 ft receptacle zone, 20 ft overhead clearance cone. Student spots a receptacle at 8 feet from pool edge (inside the restricted zone), flags it.

---

### 8. Wiring Methods & NM Cable
**zSpace: 4 | Exam: 4 | Safety: 3**

Students can see inside walls — how cables route through studs, securing requirements, bending radius limits. The ability to "x-ray" a wall cavity and see proper vs. improper cable installation is a major zSpace advantage.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| NM cable uses permitted | Art. 334.10 | Built |
| Securing and supporting (4.5 ft / 12 in) | Art. 334.30 | Built |
| NM cable ampacity / bundling derate | Art. 334.80 | Built |
| NM cable bending radius | Art. 334.24 | Planned |
| NM cable protection from damage | Art. 334.15 | Planned |
| AC cable (BX) installation | Art. 320.10, 320.30 | Planned |
| MC cable installation | Art. 330.10, 330.30 | Planned |
| EMT conduit sizing | Art. 358 | Planned |
| Rigid conduit | Art. 344 | Planned |
| Conduit fill calculations (40% rule) | Art. Annex C | Planned |
| Box fill calculations | Art. 314.16 | Planned |

**zSpace moment:** Transparent wall reveals cable runs between studs. Student counts cables through a single stud hole (4 NM cables — derate required). Uses stylus to measure distance from last staple to junction box — more than 12 inches, violation.

---

### 9. Outdoor & Wet Location Installations
**zSpace: 4 | Exam: 3 | Safety: 4**

Weather protection requirements come alive when students can inspect 3D weatherproof covers, in-use covers, and wet-rated boxes from multiple angles.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Receptacles in damp locations | Art. 406.9(A) | Built |
| Receptacles in wet locations | Art. 406.9(B) | Built |
| Luminaires in wet/damp locations | Art. 410.10(A) | Built |
| Wet location box requirements | Art. 314.15 | Planned |
| In-use covers (extra-duty) | Art. 406.9(B)(1) | Planned |
| Outdoor lighting pole disconnect | Art. 410.130(G) | Planned |
| Landscape lighting | Art. 411 | Planned |
| Signs and outline lighting | Art. 600 | Planned |

---

## P2 — Beta Milestone

Broadens coverage for exam prep completeness. Higher NEC chapter coverage.

### 10. Motors & Motor Controls
**zSpace: 4 | Exam: 5 | Safety: 3**

Motors are the second most tested exam topic after conductor sizing. zSpace value: trace the circuit from motor → disconnect → controller → overload → panel, seeing each component in 3D and verifying sizing at each point.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Motor ampere ratings (table values) | Art. 430.6(A)(1) | Built |
| Motor branch-circuit protection | Art. 430.52 | Built |
| Motor conductor sizing | Art. 430.22 | Planned |
| Motor disconnecting means | Art. 430.102 | Planned |
| Motor overload protection | Art. 430.32 | Planned |
| Motor controller requirements | Art. 430.81 | Planned |
| Hermetic motor-compressors | Art. 440 | Planned |
| Motor feeder calculations | Art. 430.24 | Planned |
| Motor nameplate data | Art. 430.7 | Planned |

**zSpace moment:** Trace the motor circuit in 3D — from the HVAC compressor outside, through the disconnect switch on the wall, to the motor controller, through the overload relay, back to the panel. Each component is inspectable. Student verifies conductor size matches Table 430.248 value × 125%.

---

### 11. Commercial Service & Feeder Systems
**zSpace: 4 | Exam: 4 | Safety: 4**

Scaling from residential to commercial. Larger equipment, higher voltages, more complex calculations. zSpace advantage: inspect a commercial electrical room with 480V switchgear — too dangerous to access in real life for training purposes.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Service entrance conductors | Art. 230.40 | Planned |
| Service disconnect (6-disconnect rule) | Art. 230.71 | Planned |
| Feeder sizing | Art. 215.2 | Planned |
| Demand calculations — commercial | Art. 220.40-44 | Partial (220.40) |
| Emergency/standby systems | Art. 700, 701 | Planned |
| Transformer connections | Art. 450 | Planned |
| Busway installations | Art. 368 | Planned |
| Overcurrent coordination | Art. 240.12 | Planned |

**zSpace moment:** Walk into a commercial electrical room — large switchgear panels line the walls. Open a 480V section (safely, in VR). Trace feeders from utility transformer through CT cabinet to distribution panels. This access is impossible in real-world training.

---

### 12. Service Entrance & Metering
**zSpace: 4 | Exam: 3 | Safety: 4**

Follow the path from utility pole/pad to the meter base to the main panel. Students can trace the complete service entrance in 3D, from the weatherhead down to the meter, through the service entrance cable, to the main disconnect.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Service point definition | Art. 230.2 | Planned |
| Service drop clearances | Art. 230.24 | Planned |
| Service entrance conductors | Art. 230.40 | Planned |
| Service disconnect location | Art. 230.70(A) | Built |
| Service disconnect rating | Art. 230.79 | Built |
| Service conductor sizing | Art. 230.42 | Planned |
| Metering equipment | Art. 230.66 | Planned |
| Underground service laterals | Art. 230.30 | Planned |

---

### 13. Appliance & Equipment Circuits
**zSpace: 3 | Exam: 3 | Safety: 3**

Dedicated circuits for common appliances. Lower zSpace advantage (less spatial) but important for exam completeness.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Appliance flexible cord connections | Art. 422.16 | Built |
| Appliance disconnect requirements | Art. 422.30 | Planned |
| Fixed appliance branch circuits | Art. 422.10 | Planned |
| Water heater disconnect | Art. 422.31(B) | Planned |
| HVAC equipment disconnect | Art. 440.14 | Planned |
| Kitchen exhaust / range hood | Art. 422.16 | Built |
| EV charging equipment (EVSE) | Art. 625 | Planned |

---

## P3 — Release & Beyond

Deep specialization, advanced topics, and emerging code areas.

### 14. Transformers
**zSpace: 3 | Exam: 4 | Safety: 3**

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Transformer overcurrent protection | Art. 450.3 | Planned |
| Transformer connections | Art. 450.5 | Planned |
| Transformer vault requirements | Art. 450.41-48 | Planned |
| Low-voltage lighting transformers | Art. 411 | Planned |

### 15. Hazardous Locations
**zSpace: 5 | Exam: 3 | Safety: 5**

Extremely high zSpace value — invisible gas/dust zones rendered as 3D classification areas. Too dangerous to teach hands-on in real environments.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Class I locations (gases/vapors) | Art. 500, 501 | Planned |
| Class II locations (dusts) | Art. 500, 502 | Planned |
| Zone classification system | Art. 505 | Planned |
| Explosion-proof equipment | Art. 501.10 | Planned |

**zSpace moment:** Gas station canopy with classified zones rendered as colored 3D volumes — Class I Division 1 (red) within 18" of dispenser, Division 2 (orange) extending 20 feet. Student identifies improperly rated junction box inside the classified zone.

### 16. Fire Alarm & Low-Voltage Systems
**zSpace: 3 | Exam: 2 | Safety: 4**

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Fire alarm circuits | Art. 760 | Planned |
| Communications circuits | Art. 800 | Planned |
| Network-powered broadband | Art. 840 | Planned |
| Class 2 & 3 circuits | Art. 725 | Planned |

### 17. Solar PV & Energy Storage
**zSpace: 4 | Exam: 3 | Safety: 4**

Growing rapidly in importance. Excellent zSpace candidate — trace DC circuits from roof panels through combiner box, inverter, to AC panel.

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| PV system disconnecting means | Art. 690.13 | Planned |
| PV conductor sizing | Art. 690.8 | Planned |
| Rapid shutdown | Art. 690.12 | Planned |
| Energy storage systems | Art. 706 | Planned |
| PV grounding / bonding | Art. 690.41-50 | Planned |

### 18. Healthcare Facilities
**zSpace: 4 | Exam: 2 | Safety: 5**

| Content | NEC Articles | Status |
|---------|:---:|:---:|
| Essential electrical systems | Art. 517.25-35 | Planned |
| Patient care areas | Art. 517.13 | Planned |
| Wet procedure locations | Art. 517.20 | Planned |
| Ground-fault protection | Art. 517.17 | Planned |

---

## Content Totals

| Priority | NEC Articles | Violations | Scenarios | Sandbox Challenges |
|:--------:|:------------:|:----------:|:---------:|:------------------:|
| **P0** | 42 built + 7 planned | 22 built | 2 (Panel, Branch) | 1 (Residential 200A) |
| **P1** | 24 built + 21 planned | 12 built | 2 (Grounding, Pool/Outdoor) | 0 |
| **P2** | 2 built + 30 planned | 0 | 2 (Commercial, Service) | 1 (Commercial 400A) |
| **P3** | 0 built + 25 planned | 0 | 2 (Hazardous, Solar PV) | 0 |
| **Total** | **66 built + 83 planned = 149** | **34 built** | **8 planned** | **2 planned** |

---

## Quick Reference Cards — Priority Map

| Card | Priority | Status |
|------|:--------:|:------:|
| GFCI Protection Requirements | P0 | Built |
| AFCI Protection Requirements | P0 | Built |
| Wire Gauge & Breaker Sizing | P0 | Built |
| Required Branch Circuits | P0 | Built |
| Receptacle Spacing Rules | P1 | Built |
| Residential Load Calculation | P0 | Built |
| Grounding Electrode System | P0 | Built |
| Panel Design Basics | P0 | Built |
| NM Cable Installation | P1 | Built |
| Key 2026 NEC Changes | P0 | Built |
| Working Space Clearances | P0 | Planned |
| Motor Circuit Protection | P2 | Planned |
| Swimming Pool Safety Zones | P1 | Planned |
| Service Entrance Components | P2 | Planned |
| Conduit Fill & Box Fill | P2 | Planned |
| Wet Location Protection | P1 | Planned |
| Commercial Load Calculations | P2 | Planned |
| Hazardous Location Classes | P3 | Planned |
| Solar PV System Layout | P3 | Planned |
| EV Charging Requirements | P3 | Planned |

---

## Scenario Priority Order

| # | Scenario | Priority | Violations | Key zSpace Feature |
|:-:|----------|:--------:|:----------:|-------------------|
| 1 | Residential Service Panel | P0 | 8+ | Open panel, inspect breakers, trace circuits |
| 2 | Branch Circuit Wiring | P0 | 12 | Room-by-room GFCI/AFCI walkthrough |
| 3 | Grounding & Bonding | P0 | 10 | Underground electrode system in x-ray view |
| 4 | Panel Design Sandbox | P0 | 10 rules | Drag breakers with stylus, snap to slots |
| 5 | Swimming Pool Zones | P1 | 8 planned | 3D safety zone volumes around water |
| 6 | Outdoor/Wet Location | P1 | 6 planned | Weatherproof covers, in-use cover inspection |
| 7 | Commercial Electrical Room | P2 | 10 planned | 480V switchgear, feeder tracing |
| 8 | Motor Installation | P2 | 8 planned | Complete motor circuit trace |
| 9 | Hazardous Location | P3 | 6 planned | Classified zone visualization |
| 10 | Solar PV System | P3 | 6 planned | Roof-to-panel DC circuit trace |

---

## 2026 NEC Changes — Featured Content

These new-in-2026 requirements are highlighted throughout the app with a badge and should be called out in marketing.

| Change | Article | Impact |
|--------|---------|--------|
| GFCI voltage range expanded to 250V | Art. 210.8(A) | All GFCI scenarios affected |
| GFCI required for dishwashers | Art. 210.8(D) | New violation in Scenario 2 |
| AFCI room list updated | Art. 210.12(A) | Broader AFCI coverage |
| Enhanced circuit directory requirements | Art. 408.4 | Panel labeling violations updated |

---

## Appendix: Exam Topic Weighting (Approximate)

Based on published journeyman/master exam prep materials, the approximate topic distribution:

| Topic | Exam Weight | Our Priority |
|-------|:----------:|:------------:|
| Conductor sizing / ampacity | ~15% | P0 |
| Overcurrent protection | ~12% | P0 |
| Grounding & bonding | ~12% | P0 |
| Branch circuits & receptacles | ~10% | P0 |
| Load calculations | ~10% | P0 |
| Motors | ~8% | P2 |
| Wiring methods | ~8% | P1 |
| Services & feeders | ~7% | P2 |
| GFCI / AFCI | ~5% | P0 |
| Special equipment (pools, etc.) | ~5% | P1 |
| Transformers | ~4% | P3 |
| Hazardous locations | ~4% | P3 |

Our P0 content covers approximately **64% of exam weight**.
