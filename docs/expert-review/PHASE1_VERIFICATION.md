# Phase 1 verification notes

Each violation added in content plan phase 1 was drafted from memory and then checked by a second pass against public
sources. **No official NEC text could be opened** (the NFPA viewer needs a login), so every item still needs a credential
expert. Evidence notes are paraphrased. Items that could not be confirmed, or whose scene was arguable, were held out and
are listed at the end.

## BC-HALLWAY-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: Same lettering as 2023: (A) means, (B) dwelling units (ELR 2026 page). Room list unchanged there. Two blog pages claim 2026 adds bathrooms and garages; that conflicts with the ELR 2026 list and is unresolved.
- Note: Existing app entry and violations that use 210.12(A) as the dwelling-room rule are also off by a letter for 2023. 2026 bathrooms/garages claim from blogs is unverified.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: City of St Paul 2023 guidance cites 210.12(B) for dwellings and lists hallways, closets, kitchens, laundry among rooms.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1429 says: 2023 210.12: (A) is means of protection, (B) is dwelling units; 10 A circuits added to 15/20 A.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 text labelled 210.12(B) Dwelling Units, same room list including hallways, closets, laundry; protection by 210.12(A) means.
- Check (right): Hallways are on the dwelling-unit AFCI room list. Listed in 2023 (St Paul) and 2026 (ELR) lists.
- Check (wrong): Rule is in 210.12(A). In 2023, (A) is the list of protection means; dwelling-unit room rule is (B).
- Check (unclear): Applies to 15 A and 20 A, 120 V. 2023 added 10 A circuits; 15/20 A statement is right but incomplete.
- Check (right): Scene (plain breaker, 120 V 15 A hallway receptacle) breaks the rule. New-construction dwelling; no listed exception applies.

## BC-DINING-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: Lettering and dining room entry unchanged in 2026 per ELR page.
- Note: Only the subsection letter and the 10 A mention need fixing.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 dwelling rule is 210.12(B); dining rooms listed.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 210.12(B) list includes dining rooms.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1429 says: 2023 structure: (A) means, (B) dwelling units.
- Check (right): Dining rooms are on the room list. Listed in 2023 and 2026 sources.
- Check (wrong): Rule is in 210.12(A). Room rule is 210.12(B) in 2023.
- Check (right): Scene (15 A dining receptacle, plain breaker) is a violation. No exception applies.

## BC-FAMILYROOM-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: No change to lettering or this room in the ELR 2026 text.
- Note: Subsection letter fix only.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 210.12(B) lists family rooms among dwelling areas needing AFCI.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 210.12(B) list includes family rooms.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1429 says: 2023 (B) covers dwelling units with 10/15/20 A circuits.
- Check (right): Family rooms are on the room list. Listed in 2023 and 2026 sources.
- Check (wrong): Rule is in 210.12(A). Room rule is 210.12(B).
- Check (right): Scene is a violation. Plain single-pole breaker, no listed exception.

## BC-CLOSET-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: Closets still listed in 2026 per ELR page.
- Note: Answers the closet question: yes, on the dwelling room list (now (B), not (A)).
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: Dwelling list under 2023 210.12(B) includes closets.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 210.12(B) list includes closets.
- Check (right): Closets are on the room list. Closets appear in the 2023 St Paul list and the 2026 ELR list.
- Check (wrong): Rule is in 210.12(A). Room list is in 210.12(B) in 2023.
- Check (right): A walk-in closet receptacle is a realistic scene. Receptacle in a closet is allowed and visible.

## BC-KITCHEN-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: Kitchens still listed; no change found.
- Note: Drop any 'added in 2020' wording from learner text; I could not confirm it.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 210.12(B) list opens with kitchens.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 210.12(B) list includes kitchens.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1429 says: 2023 (B) lists 14 locations for dwelling units.
- Check (right): Kitchens are on the room list. Listed in 2023 and 2026 sources.
- Check (unclear): Kitchens were added in the 2020 edition (doubts field). Not confirmed in any page I opened.
- Check (right): GFCI receptacle on a plain breaker lacks AFCI. GFCI does not satisfy the arc-fault rule.

## BC-LAUNDRY-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(B)`
- 2026: Laundry areas still listed in 2026; no change found.
- Note: The edition-history wording in necText and hint was not confirmed, so it is removed.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 210.12(B) list includes laundry areas.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1853 says: 2026 210.12(B) list includes laundry areas.
- Check (right): Laundry areas are on the room list. Listed in 2023 and 2026 sources.
- Check (unclear): Laundry joined the list in 2020 / 'changed in recent editions'. Not confirmed in opened sources; hint reworded to avoid it.
- Check (right): GFCI alone does not meet the AFCI rule. Different hazards, separate requirements.

## BC-EXTENSION-001 (arc-fault-protection)

- Verdict: **corrected**, reference `210.12(E)`
- 2026: Still 210.12(E); the outlet-type AFCI may now sit at the first receptacle outlet or first switch (ELR 2026 page).
- Note: Neither the drafter's (D) nor the app's (B) is right for 2023 or 2026; (E) is. 2023 exception wording itself not seen, so expert should confirm it.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 guidance pairs (C)/(D) room rules with (E) for new, extended or modified circuits.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1855 says: 210.12(E) is Branch Circuit Wiring Extensions, Modifications, or Replacements; 2026 adds 'or switch' to the first-outlet option.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=816 says: In the 2020 NEC the extensions rule was (D); 6 ft exception excludes conductors inside enclosures.
- Check (wrong): Extensions rule is 210.12(D). (D) was the 2020 letter; in 2023 it is (E) (St Paul cites (E)); (D) is guest rooms.
- Check (wrong): Existing app entry uses 210.12(B). (B) was the 2017 letter; in 2023 and 2026 (B) is dwelling units.
- Check (unclear): Options are combination device at origin or OBC AFCI at first outlet. Actually any 210.12(A) means, or OBC AFCI at first receptacle outlet.
- Check (right): 6 ft exception, no added outlets/devices. Seen in 2020 text; 2023 (E) exception existence supported by ELR, wording not seen.
- Check (right): Scene (>6 ft, adds two receptacles) triggers rule. Exception requires both 6 ft or less and no added outlets.

## RP-CLOSET-PANEL-001 (working-space-access)

- Verdict: **confirmed**, reference `240.24(D)`
- 2026: no 2026 information found for 240.24(D). Only 240.24(E) is reported changed in 2026 (MN DLI summary, Utah course outline).
- Note: Safe to ship as drafted. Make sure the scene shows ignitible material touching or right next to the panel, not just a closet.
- Source: https://forums.mikeholt.com/threads/panel-location-240-24-d.2585590/ says: Quotes 240.24(D) as not in the vicinity of easily ignitible material, such as clothes closets; posters say closet is an example, not the whole rule.
- Source: https://www.ecmag.com/magazine/articles/article-detail/codes-standards-safely-installing-electrical-and-service-panels-residential-closets says: 2001 column cites 240.24(D) with same closet wording; code panel refused a walk-in closet exception. Old, but lettering still matches.
- Check (right): 240.24(D) is the rule against overcurrent devices near easily ignitible material, with clothes closets as the example. Wording matches a recent forum quote and a magazine column quoting the same rule.
- Check (right): Scene (coats hanging against the panel in a closet) breaks the rule. The rule is about nearby ignitible material; the closet is only an example. Coats against the panel clearly fit.

## RP-BATH-PANEL-001 (working-space-access)

- Verdict: **confirmed**, reference `240.24(E)`
- 2026: 240.24(E) gets a new exception (adding OCPDs to an existing, previously compliant panelboard in a bathroom). Letter (E) unchanged. Source: MN DLI minutes and Utah course outline. Full text not seen.
- Note: Rule and lettering are right; I could not open the exact 2023 sentence, only quotes of it. Scene should make clear it is a dwelling bathroom, not an existing-installation case.
- Source: https://forums.mikeholt.com/threads/existing-panelboard-in-a-commercial-bathroom.130536/ says: Reply says 240.24(E) does not prohibit a panel in a non-dwelling-unit bathroom, implying it does for dwelling units.
- Source: https://www.dli.mn.gov/sites/default/files/pdf/NEC-2026-adoption-review-092525-minutes.pdf says: Lists 240.24(E) Not Located in Bathrooms as the subsection receiving a 2026 exception for existing panelboards.
- Source: https://secure.utah.gov/ce-public/files/objectives/13507.pdf says: 2026 changes course outline lists 240.24(E) Exception, Overcurrent Protection Not Located in Bathrooms.
- Check (right): 240.24(E) bars overcurrent devices from bathrooms in dwelling units. A forum answer states it does not bar a non-dwelling bathroom panel, so the dwelling limit is real. Supplementary protection is excluded.
- Check (right): 2026 changes (E). 2026 adds an exception allowing more breakers in an existing, previously compliant bathroom panelboard. A new install is still a violation.
- Check (unclear): Scene: panel in bathroom 'next to the garage wall' in a house. A bathroom is not the garage; the shared wall makes the setting plausible but the panel is not 'in the garage' as the skill framing says.

## RP-PIPE-ABOVE-001 (working-space-access)

- Verdict: **confirmed**, reference `110.26(E)(1)(a)`
- 2026: no 2026 information found for 110.26(E)(1). Note: 2023 already added 'service equipment' to the (E) list; 2026 changes found elsewhere in 110.26 only (door swing/egress, (C)(2), possibly (A)(1)).
- Note: Reference is correct. In the 3D scene the pipe must visibly pass inside the panel's footprint column, and an unfinished ceiling above a drywall ceiling is not what students will see.
- Source: https://zigtech.ai/wp-content/uploads/2025/08/Working-space-per-110.26-NEC-2023.pdf says: Holt 2023 changes: footprint space to 6 ft above equipment or structural ceiling is dedicated; no foreign piping or ducts; suspended-ceiling exception.
- Source: https://forums.mikeholt.com/threads/dedicated-space-above-panelboard.113463/ says: Quotes (E)(1)(a) with 'leak protection apparatus' in the barred list, plus (b) and (c); lettering matches 2023 summary.
- Source: https://www.ecmweb.com/national-electrical-code/article/21277365/dedicated-electrical-spaces-about-electrical-equipment says: Describes the dedicated space as equipment width and depth, floor to 6 ft above equipment or structural ceiling, whichever is lower.
- Check (right): (E)(1)(a) reserves footprint-wide, footprint-deep space to 6 ft above equipment or the structural ceiling. Matches the 2023 summary and a verbatim older-edition quote; the 'lower of' wording matters and the draft omits 'structural'.
- Check (right): Sprinkler piping has a separate allowance in (E)(1)(c). (c) lets sprinkler protection piping into the dedicated space if it complies. It is not a general allowance for pipes serving the area.
- Check (right): Scene: copper supply and drain pipe pass vertically over the panel. Breaks the rule only if the pipe is inside the panel's width and depth footprint, not merely nearby.
- Check (wrong): A drip pan or leak shield could be placed in the dedicated zone as a fix. Leak protection apparatus is itself barred from the zone. Protection belongs above it under (b).

## RP-WIDTH-001 (working-space-access)

- Verdict: **confirmed**, reference `110.26(A)(2)`
- 2026: no 2026 information found for 110.26(A)(2).
- Note: Existing entry 110.26(A)(2) already exists in the app, so no new entry is needed. Consider noting that (A)(2) holds both the width and door rules.
- Source: https://zigtech.ai/wp-content/uploads/2025/08/Working-space-per-110.26-NEC-2023.pdf says: Holt 2023 changes: width at least 30 in and never less than equipment width; may be left, right or centered.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=3076 says: 110.26(A)(2): at least 30 in or equipment width, whichever greater; doors must open at least 90 degrees.
- Source: https://expertce.com/learn-articles/nec-working-clearance-requirements-110-26/ says: Width is the greater of 30 in or equipment width, with doors able to open a full 90 degrees.
- Check (right): Minimum working space width is 30 in or the equipment width, whichever is greater. Same in the 2023 summary and two course pages.
- Check (right): Space may be centered or offset. Holt commentary says it can be measured either side or centered, and can overlap other equipment's working space.
- Check (right): 20 in scene vs 30 in limit; 32 in compliant; 15 in cover. 20 is under 30; 32 passes; typical panel cover is near 14 to 15 in, so 30 in governs.
- Check (right): Water heater and shelf crowding both sides is visible and breaks the rule. Both are obstructions in the width zone. They may also eat into the 36 in depth, which is a separate rule (A)(1).

## RP-HEADROOM-001 (working-space-access)

- Verdict: **corrected**, reference `110.26(A)(3)`
- 2026: no 2026 information found for 110.26(A)(3).
- Note: Ship only if the scene states new construction or the panel is over 200 A, otherwise an inspector could say the exception applies. The drafter's own doubt about the exception was right.
- Source: https://zigtech.ai/wp-content/uploads/2025/08/Working-space-per-110.26-NEC-2023.pdf says: Holt 2023: 6.5 ft or equipment height; Ex 2 exempts service disconnect or panelboards of 200 A or less in an existing dwelling unit.
- Source: https://forums.mikeholt.com/threads/nec-110-26-a-working-space-dwelling-unit-existing.2590007/ says: Quotes Exception No. 2: existing dwelling units, service equipment or enclosed panelboards up to 200 A may have headroom under 6.5 ft.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=3076 says: 110.26(A)(3): working space height at least 6 ft 6 in or equipment height, whichever is greater.
- Check (right): Headroom of at least 6.5 ft (2.0 m) or equipment height, whichever is greater. Confirmed in 2023 summary and a forum quote of the rule.
- Check (right): Exception for existing dwelling units, 200 A or less. It exists in (A)(3) as an exception (listed as Ex 2 in the 2023 summary; older text says 'enclosed panelboards'). The draft omitted it.
- Check (unclear): Scene (new house) means the exception does not apply. Only holds if the scene is clearly new work. A student cannot tell new from existing; also unclear whether an attached garage counts as part of the dwelling unit.
- Check (right): 5.5 ft sceneFact and 7 ft compliantFact. 5.5 is under 6.5; 7 ft passes. A duct directly over the panel footprint would also sit in the (E)(1) zone, so keep it in front of the panel.

## RP-DOOR-SWING-001 (working-space-access)

- Verdict: **confirmed**, reference `110.26(A)(2)`
- 2026: 110.26 door language clarified in 2026: equipment doors are judged for blocking access or egress regardless of door position, removability, or opening past 90 degrees. This concerns the egress rule, not renumbering of the 90 degree rule. Source: MN DLI minutes.
- Note: Reference is right for 2023. Avoid confusing with the new egress-path door rule if the scene is reused.
- Source: https://zigtech.ai/wp-content/uploads/2025/08/Working-space-per-110.26-NEC-2023.pdf says: Under (A)(2) width: working space must be wide, deep and high enough for equipment doors to open at least 90 degrees.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=3076 says: 110.26(A)(2): width at least 30 in; doors or hinged panels must open at least 90 degrees.
- Source: https://expertce.com/learn-articles/nec-working-clearance-requirements-110-26/ says: 110.26(A)(2) also requires equipment doors to open a full 90 degrees.
- Check (right): The 90 degree door-opening rule sits in 110.26(A)(2), with width. Holt 2023 places it directly with the width paragraph; two course pages put it in (A)(2). I could not view the NFPA text itself.
- Check (right): The working space must let equipment doors open at least 90 degrees. All three sources agree on at least 90 degrees.
- Check (right): 2023 moved door rules around. 2023 moved the 'open doors must not impede access/egress' rule (24 in by 6.5 ft path) into the 110.26 parent text. That is a different rule from the 90 degree opening.
- Check (right): Scene: door stops near 60 degrees against a rack or wall corner. Realistic and fails the rule. A rack within the working space may also trip the clear-space rule 110.26(B).

## RP-IDMARK-DISC-001 (identification-marking)

- Verdict: **confirmed**, reference `110.22(A)`
- 2026: 110.22 still exists in 2026 (UpCodes lists it, ECM cites it for EVSE shutoff marking); no change to (A) found.
- Note: Scene must keep the disconnect clearly away from the equipment it serves, or the 'purpose evident' carve-out applies. Entry text is fine; 408.4 'related' link is reasonable.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1412 says: 2023 110.22(A): legible purpose marking unless evident; durable; source marking added only for non-one/two-family buildings.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/21149002/code-qa-proper-marking-of-disconnects says: 2020-based Q&A confirming purpose marking, the 'evident' carve-out, and durability for the environment.
- Source: https://up.codes/s/identification-of-disconnecting-means says: Lists 110.22 text in 2023 and 2026 editions with the same purpose-marking and durability wording.
- Check (right): 110.22(A) requires each disconnecting means to be legibly marked with its purpose unless location and arrangement make it evident. Matches 2023 wording on two sources; the 'evident' carve-out is real, so scene must show the disconnect away from its load.
- Check (right): Marking must be durable for the environment. Same sentence of 110.22(A); this is why LABEL-DURABLE-001 overlaps.
- Check (right): Extra source-identification duty does not apply here. 2023 added circuit-source marking, but only for other than one- or two-family dwellings; a house garage is excluded.

## RP-IDMARK-NEUTRAL-RED-001 (identification-marking)

- Verdict: **confirmed**, reference `200.6(A)`
- 2026: Renumbered: 200.6 becomes 200.7 in the 2026 NEC (UpCodes, Utah course outline, Minnesota DLI summary). 2026 also adds a new 200.7(A)(9) allowing a single non-green stripe; a solid red wire still fails.
- Note: Reference is 200.6(A) for 2023 but must be 200.7(A) if the app targets 2026. Existing 200.6 entry title matches the 2023 title.
- Source: https://forums.mikeholt.com/threads/nec-and-marking-the-grounded-conductor.124094/ says: Thread distinguishes 200.6(A) (6 AWG and smaller, continuous finish) from 200.6(B)(4) (marking at terminations, 4 AWG and larger).
- Source: https://up.codes/s/means-of-identifying-grounded-conductors says: Shows this section as 200.6 in the 2023 NEC and renumbered 200.7 in the 2026 NEC.
- Source: https://secure.utah.gov/ce-public/files/objectives/13441.pdf says: 2026 Chapter 2 outline lists 200.7 as Means of Identifying Grounded Conductors, with (A) 6 AWG or smaller and (B) 4 AWG or larger.
- Check (right): Grounded conductor 6 AWG or smaller must be white/gray (or three white/gray stripes) over its whole length. Forum discussion of the code text: sizes 6 AWG and smaller need the continuous outer finish; tape at the end is not allowed.
- Check (right): No re-identification allowance for 6 AWG or smaller. Termination marking is only an option for 4 AWG and larger; the multiconductor-cable exception needs qualified maintenance and does not fit a dwelling.
- Check (right): A solid red 12 AWG on a 120 V neutral bar really breaks the rule as written. Red is not a permitted grounded-conductor colour; the scene is a genuine 200.6(A) violation.
- Check (right): 12 AWG, 120 V in the scene. Both consistent with a branch-circuit neutral under the 6 AWG threshold.

## RP-IDMARK-EGC-GREEN-001 (identification-marking)

- Verdict: **corrected**, reference `250.119(A)`
- 2026: no 2026 information found
- Note: Use 250.119(A) as the reference. The newEntry should be refreshed so the 4 AWG note points to (B) and the EGC-LARGE item uses 250.119(B).
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1618 says: Reproduces 2023 250.119: (A) General includes the sentence barring green or green-yellow from circuit conductors; (B) is 4 AWG and larger.
- Source: https://forums.mikeholt.com/threads/identification-of-ungrounded-conductors.24802/ says: Quotes 250.119 with the same prohibition, added in 2005; green is reserved for grounding and bonding conductors.
- Check (right): The ban on using green conductors as circuit conductors sits in 250.119. In the 2023 text it is the last sentence of 250.119(A) General; drafter's bare '250.119' lacked the letter.
- Check (right): Drafter's entry text says EGCs are 'bare, covered or insulated' with green finish and green kept off circuit conductors. Consistent with 2023 (A); the 4 AWG re-marking option is in (B), not (A).
- Check (right): Scene: solid green on a breaker carrying load. Clear violation; (A) has exceptions (e.g. Class 2/3 and communications cables) that do not fit this scene.

## RP-IDMARK-WHITE-HOT-001 (identification-marking)

- Verdict: **corrected**, reference `200.7`
- 2026: Renumbered: this rule moves from 200.7 to 200.8 in the 2026 NEC (UpCodes, Utah 2026 Chapter 2 outline listing 200.8(C)(1) cable assemblies and (C)(2) flexible cords).
- Note: Keep reference as 200.7 for 2023 (200.7(C) for the re-marking detail). Entry title already matches the 2023 title; 2026 title and number differ.
- Source: https://forums.mikeholt.com/threads/reidentification-of-the-grounded-conductor-of-a-cable-assembly.136644/ says: Quotes 200.7(A), (B), (C): white as ungrounded allowed only if in a cable assembly and permanently re-marked at terminations.
- Source: https://up.codes/s/circuits-of-50-volts-or-more says: Lists this as 200.7(C) in 2014 through 2023 editions and 200.8(C) in 2026; covers cable reidentification, switch loops, flexible cords.
- Check (right): General rule is in 200.7(A) and exceptions in 200.7(C). In 2023, 200.7(A) general, (B) under 50 V, (C) 50 V or more with (C)(1) cable assemblies/switch loops and (C)(2) flexible cords.
- Check (unclear): Re-identification can cure a white hot conductor. Only for a conductor that is part of a cable assembly; one source says a loose conductor in a raceway cannot be re-identified, so scene wording matters.
- Check (right): Scene is a plain single-pole 120 V circuit so no exception applies. Even if cable is used, the lack of re-marking is the violation. Stating cable or conduit removes ambiguity.

## RP-IDMARK-SVCDISC-001 (identification-marking)

- Verdict: **confirmed**, reference `230.70(B)`
- 2026: Changed. 230.70(B) now requires 'SERVICE DISCONNECT' marking on or beside each service disconnect, and one- and two-family dwellings also need 'EMERGENCY DISCONNECT' (red background, white text, at least 1/2 in.). 230.85 was deleted and moved into 230.70; new 230.70(D) requires a plaque for other sources. Sources: electricallicenserenewal 1873 and 1878, ECM top-25 article, Minnesota DLI summary. ECM and the CE site differ slightly on which wording applies to which building type.
- Note: Fine for 2023. If the app moves to 2026, the dwelling wording and compliantFact need updating. Check whether the app also has an exterior emergency-disconnect violation to avoid overlap.
- Source: https://forums.mikeholt.com/threads/service-equipment.123955/ says: A poster quotes 230.70(B): each service disconnect permanently marked to identify it as a service disconnect.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1873 says: 2023 text of 230.70(B) is the permanent service-disconnect marking; 2026 revises it to a required 'SERVICE DISCONNECT' wording.
- Check (right): 230.70(B) is the Marking subsection requiring a permanent service-disconnect marking. 2023 text: each service disconnect permanently marked to identify it as a service disconnect; quoted on a forum and reproduced by a CE site.
- Check (right): Scene: main breaker with only circuit labels, no 'service disconnect' marking. A bare 'MAIN' label is accepted by some inspectors, so note the scene has no service-disconnect wording at all.

## RP-IDMARK-NEUTRAL-FEEDER-001 (identification-marking)

- Verdict: **confirmed**, reference `200.6(B)`
- 2026: Renumbered to 200.7(B) in the 2026 NEC (UpCodes; Utah 2026 outline lists 200.7 (B) Sizes 4 AWG or Larger). No substantive change found.
- Note: Scene works because the conductor is a neutral by function (lands on the neutral bar). Same edition-numbering issue as the red-neutral item.
- Source: https://forums.mikeholt.com/threads/nec-and-marking-the-grounded-conductor.124094/ says: Quotes 200.6(B): 4 AWG or larger may be identified by a distinctive white or gray marking encircling the conductor at terminations.
- Source: https://up.codes/s/means-of-identifying-grounded-conductors says: Same section is 200.6 in 2023 and 200.7 in 2026, with size-based rules for 6 AWG and smaller versus 4 AWG and larger.
- Check (right): 4 AWG and larger grounded conductors may be identified by white/gray finish, three stripes, or a white/gray marking at terminations. Forum thread quotes 200.6(B) with the termination-marking item; marking must encircle the conductor.
- Check (right): 2 AWG black neutral on the neutral bar with no white marking is a violation. 2 AWG is above the 4 AWG threshold, so a black conductor is acceptable only if marked white or gray at its ends.
- Check (right): Garage panel feeder neutral of 2 AWG. Plausible for a roughly 100 A feeder; no number conflict.

## RP-IDMARK-PV-PLAQUE-001 (identification-marking)

- Verdict: **corrected**, reference `705.10`
- 2026: Plaque duty is expanded in new 230.70(D) (all sources not at the service disconnect listed on a plaque showing location) per Minnesota DLI and ECM. 705.10 itself: one vendor blog says only editorial (110.25 to 110.21(B)); no authoritative 2026 text found.
- Note: Fix necText and entry before shipping; the old wording would teach students an outdated rule. Scene should have the inverter on the garage wall and no plaque at all.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1563 says: 2023 705.10 requires location of each source disconnect, emergency phone numbers, and 'CAUTION: MULTIPLE SOURCES OF POWER' marking per 110.21(B); notes an exception.
- Source: https://forums.mikeholt.com/threads/230-2-690-56-705-10.122920/ says: Inspector on a dwelling cites 705.10 for a plaque at the service disconnect; older wording shown.
- Check (right): 705.10 is the Identification of Power Sources rule requiring a plaque at the service equipment. Reference holds; drafter's text matches the older 2014-era wording, not 2023.
- Check (wrong): Plaque lists 'every electric power source'. 2023 requires location of each source disconnect, off-site emergency phone numbers, and the CAUTION wording.
- Check (right): Applies to a one-family dwelling. Article 705 covers any premises with interconnected sources; a forum thread discusses this plaque on a dwelling.
- Check (unclear): Exception for co-located arrangements. The 2023 text flags an exception but its content was not displayed in any source I opened.

## RP-IDMARK-EGC-LARGE-001 (identification-marking)

- Verdict: **corrected**, reference `250.119(B)`
- 2026: no 2026 information found
- Note: Keep with the corrected letter (B). Subtle, Expert difficulty is appropriate; scene needs the unmarked ends visible inside the panel.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1618 says: 2023 text: 250.119(B) covers 4 AWG and larger, (B)(1) marking at ends and accessible points, (B)(2) methods.
- Source: https://forums.mikeholt.com/threads/250-119-identifying-egc.124734/ says: Same rule quoted from an earlier edition as 250.119(A); tape need only encircle at ends and accessible points.
- Check (wrong): 4 AWG and larger identification rule is in 250.119(A). In the 2023 text it is 250.119(B), with (B)(1) the end and accessible-point marking and (B)(2) the methods. (A) is General; older editions used (A).
- Check (right): Marking at each end and every accessible point, by stripping, colouring or green tape. Matches 2023 (B)(1) and (B)(2); exception only for conduit bodies with no splices or unused hubs.
- Check (right): 4 AWG threshold. Conductors 6 AWG and smaller must be green throughout; 4 AWG and larger may be re-marked.

## RP-IDMARK-GENINLET-SIGN-001 (identification-marking)

- Verdict: **corrected**, reference `702.7(C)`
- 2026: 702.7(C) keeps its number in 2026 per UpCodes; no 2026 text change found. 702.7(A) for dwellings points to 230.85, which 2026 deleted, so (A) is probably revised (inference, not seen).
- Note: Keep, but fix the newEntry text and lower its confidence note until the 2026 702.7(A) is checked. Scene works if the transfer switch is visibly next to the inlet.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=189.0 says: 702.7(C), new in 2014, requires an inlet warning naming bonded-neutral separately derived or floating-neutral non-separately derived.
- Source: https://forums.mikeholt.com/threads/portable-generator-signs-one-family-home.2585364/ says: Quotes 2020 702.7(A) dwelling variation; a reply notes the bonded/floating neutral sign still required for portable generators.
- Source: https://up.codes/s/power-inlet says: Lists the power inlet sign as 702.7(C) in 2014, 2017, 2020, 2023 and 2026 editions.
- Check (right): The inlet sign is 702.7(C). Added in the 2014 NEC; every later edition through 2026 still lists it as 702.7(C).
- Check (right): Applies to a one-family dwelling. The dwelling-specific change since 2020 affects 702.7(A) only; the inlet sign has no dwelling exclusion.
- Check (unclear): Sign says which neutral type the generator should be. Sign states the system type the inlet and transfer equipment are wired for, using two fixed warning wordings; drafter's meaning is close.
- Check (wrong): Entry text says one sign at the service identifies the standby source. For dwellings since 2020 that sign sits at the 230.85 disconnect and locates the standby source disconnect; 230.85 is deleted in 2026.

## Held out

- arc-fault-protection DOWNSTREAM-001: verification: single-source. Reference 210.12(A) is correct here since (A) is the means list, but 2023 item wording was not read. Expert should confirm the item and wiring-method conditions.
- arc-fault-protection MULTIWIRE-001: verification: unverified (keep=false). No source opened, and the skill fit is weak. Recommend drop or move to a multiwire-circuits skill; a handle tie also cannot join two single-pole AFCIs for several brands per forum snippets.
- arc-fault-protection FRIDGE-001: verification: single-source. Exception list comes from the 2026 page; the 2023 exception list was not read directly, so expert should confirm.
- working-space-access LIGHT-001: verification: single-source. Both sources are the same forum, so single-source. Safer scene: no light source at all in the garage, or only a motion-sensor-controlled light with no manual override, so the violation is not arguable.
- working-space-access DRIP-PAN-001: verification: corrected (keep=false). Rule and lettering are right, but the scene is hard to make real and visible: it needs a very tall ceiling or a suspended ceiling. Recommend dropping or reworking the setting rather than shipping.
- working-space-access STAIR-PANEL-001: verification: single-source. Both sources are the same forum, so single-source; I could not open the NFPA text. Rewrite the scene so the panel is plainly over the steps (not beside them or on a landing), or the item is arguable.
- identification-marking IDMARK-LABEL-DURABLE-001: verification: confirmed (keep=false). Reference is right but it is a near-duplicate of IDMARK-DISC-001 (same 110.22(A), same answer). Drop it; if a durability item is wanted, merge it into DISC-001 as a variation.
