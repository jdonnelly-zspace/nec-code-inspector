# Phase 1 verification notes, batch 3

Each violation added in content plan phase 1 was drafted from memory and then checked by a second pass against public
sources. **No official NEC text could be opened** (the NFPA viewer needs a login), so every item still needs a credential
expert. Evidence notes are paraphrased. Items that could not be confirmed, or whose scene was arguable, were held out and
are listed at the end.

## BC-GFCI-OUTDOOR-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(3)`
- 2026: Item becomes (4) in 2026 after the garage/accessory split (inferred; full 2026 list text not opened). 2026 also raises the 210.8(F) outlet threshold from 50 A to 60 A (Denison TX change summary).
- Note: Only the scene voltage needs fixing. 2026 number is inferred from the confirmed garage split.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 dwelling list: outdoors is item (3); GFCI by breaker or receptacle both acceptable.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: 2023 article lists outdoors third among 12 dwelling locations.
- Check (right): Outdoors is 210.8(A)(3) in 2023. Third item in the 2023 list; also (3) in 2020.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.
- Check (right): necText: GFCI receptacle or breaker both allowed. Holt says either device type may provide protection.

## BC-GFCI-BASEMENT-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(5)`
- 2026: Item becomes (6) in 2026 (inferred from the garage/accessory split).
- Note: Reference right; fix the 'no longer' wording and voltage. Holt's non-dwelling (B)(12) still says unfinished basements, a different list.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 item (5): basement receptacles need GFCI; no basement exception listed.
- Source: https://captaincode2020.leviton.com/node/119 says: 2020 change: all dwelling basements covered, no longer only unfinished portions; this was item (5).
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: Basements listed fifth in 2023 dwelling list.
- Check (right): Basements are (A)(5) in 2023. Fifth item in 2023 and in 2020.
- Check (wrong): necText: 'The 2023 list no longer limits this' to unfinished areas. The widening happened in 2020, not 2023. Reword.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-LAUNDRY-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(11)`
- 2026: Item becomes (12) in 2026 (inferred).
- Note: Existing app entry 210.8(A)(10) laundry is 2020 numbering. newEntry text is correct.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 item (11): receptacles in a dwelling laundry area need GFCI.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: 2023 article numbers laundry areas as item (11) of 12.
- Check (right): Laundry areas are (A)(11) in 2023. 2023 list has 12 items; laundry is 11th. In 2020 it was (10).
- Check (right): Overlap with BC-LAUNDRY-001. No clash: BC-LAUNDRY-001 is 210.12(B) arc-fault (washer receptacle already has GFCI). Different rule, same room; keep scenes distinct.
- Check (right): necText 'whether or not a sink is nearby'. Laundry item has no sink condition.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-CRAWL-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(4)`
- 2026: Item becomes (5) in 2026 (inferred).
- Note: Scene must show crawl floor at or below grade. Fix voltage.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 item (4): crawl spaces at or below grade need GFCI.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: Crawl spaces listed fourth in the 2023 dwelling list.
- Check (right): Crawl spaces at or below grade are (A)(4). Fourth in 2023 and 2020.
- Check (right): floorAtOrBelowGrade true. Matches the at-or-below-grade condition.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-WETBAR-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(8)`
- 2026: Sinks likely become (9) in 2026 (inferred). One search summary says 2026 widens the sink item to any indoor sink; not confirmed.
- Note: Wet bar is ambiguous: a bar sink with beverage provisions arguably cites (7). Make it a plain sink or accept (7) or (8).
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 item (7) covers areas with sinks and permanent food, beverage or cooking provisions; item (8) covers sinks within 6 ft.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: 2023 article lists food or beverage prep areas as item (7), sinks eighth.
- Check (right): Sinks within 6 ft are (A)(8) in 2023. Item (7) is the food-preparation item; sinks moved to (8).
- Check (right): 6 ft from top inside edge of bowl. 2023 wording measures to the top inside edge of the bowl.
- Check (right): 1.2 m is about 4 ft. 1.2 m is 3.9 ft, inside 1.83 m.
- Check (unclear): Wet bar is only a plain sink. A wet bar with beverage-prep provisions falls under the 2023 food-preparation item (7), which needs no 6 ft distance.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-SUMP-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(5)`
- 2026: No 2026 information found for 210.8(D) numbering; basement item likely (6) in 2026 (inferred).
- Note: Two valid rules apply, (A)(5) and (D)(6). The checker only models (A)(5) here since sump pump is not in the appliance vocabulary.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 210.8(D) lists sump pumps as item (6), cord-and-plug or hardwired, up to 60 A.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: Basement item (5) shows no exception for dedicated receptacles.
- Source: https://buildingcodegeek.com/gfci-protection-requirements/ says: 2023 appliance list includes sump pumps; basements covered finished or unfinished.
- Source: https://forum.nachi.org/t/gfci-for-sump-pump/186673?page=2 says: Old thread discusses a former exception for dedicated basement appliance receptacles, now gone.
- Check (right): Sump pumps are 210.8(D)(6) in 2023. Holt's 2023 (D) list: vacuums, coolers, spray washers, tire inflators, vending, sump pumps (6), dishwashers (7).
- Check (wrong): Dedicated-appliance receptacle in a basement is exempt in 2023. No basement exception in 2023 (A)(5). Remaining exceptions cover snow melting, alarms and exhaust fans.
- Check (wrong): appliance 'sump-pump' is in app vocabulary. ScopeVocabulary.Appliances allows only dishwasher, refrigerator, freezer, other. Use 'other' or empty.
- Check (right): Dedicated exception existed in older editions. IAEI and an old forum thread describe it; removal edition (reported as 2008) rests on one search summary.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-SINKPATH-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(8)`
- 2026: Sink item likely (9) in 2026 (inferred).
- Note: Show an open doorway with no door leaf. Sources do not address a closed door.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: Distance is the shortest cord path without piercing a floor, wall, ceiling or fixed barrier; window and door wording removed.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: 2023 revision dropped window and door references to the measurement.
- Check (right): Sink item is (A)(8) in 2023. See wet-bar item.
- Check (right): Distance follows shortest cord path; door/window wording removed in 2023. Holt comment confirms removal of window and door wording; ECM repeats it.
- Check (right): 1.5 m is about 5 ft, under 6 ft. 1.5 m is 4.9 ft; limit 1.83 m.
- Check (right): room 'hallway' in vocabulary. Valid room; the sink rule has no room filter, so distance triggers it.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-GFCI-SHOWER-001 (shock-protection)

- Verdict: **corrected**, reference `210.8(A)(10)`
- 2026: Item likely (11) in 2026 (inferred).
- Note: With room indoor-damp-wet the damp-location item also applies; fine for training. Existing app entry (9) is 2020 numbering.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: 2023 item (10): receptacles within 6 ft of outside edge of tub or shower stall not in a bathroom.
- Source: https://www.mikeholt.com/files/PDF/23UNEC1_210.8.pdf says: Bathroom area means a sink plus toilet, urinal, tub, shower, bidet or similar.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21280067/nec-requirements-for-gfcis-and-afcis says: 2023 article places tub/shower item tenth of 12.
- Check (right): Tub/shower outside a bathroom is (A)(10) in 2023. 2020 number was (9); 2023 is (10) after the food-prep item was added.
- Check (wrong): room 'mudroom' in vocabulary. Not in ScopeVocabulary.Rooms. Use indoor-damp-wet (Leviton cites mudrooms as damp areas) or other.
- Check (right): 6 ft from outside edge; 1.2 m about 4 ft. Holt says outside edge of tub or shower stall.
- Check (right): Shower with no basin is not a bathroom. 2023 bathroom area definition needs a sink plus another fixture.
- Check (wrong): sceneScope volts 120. App checker rules need volts 125 to 250; 120 fails the check. Existing entries use 125. Set volts to 125.

## BC-OUTDOOR-HEIGHT-001 (branch-circuit-requirements)

- Verdict: **confirmed**, reference `210.52(E)(1)`
- 2026: no 2026 information found (the 2026 change lists I opened name only 210.52(A)(2), (A)(5), (C)(4) and 210.63; I did not open the 2026 text itself)
- Note: Rule and numbers hold. Optional wording tweak: it covers one- and two-family dwellings, not only a one-family house.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_210.52.pdf says: 2023 commentary: one outdoor receptacle at front and back of each dwelling, readily accessible from grade, no more than 6.5 ft above grade.
- Source: https://ecmweb.com/national-electrical-code/code-basics/article/20901808/receptacles-in-dwellings says: 2017 article cites (E)(1) with the same 6.5 ft above-grade ceiling for outdoor receptacles.
- Check (right): 2023 reference is 210.52(E)(1) (one- and two-family dwellings). Holt's 2023 text and a 2017 ECM article both place the front and back outdoor receptacles at (E)(1).
- Check (right): Max 6.5 ft above grade, readily accessible from grade. Both opened sources give 6.5 ft; accessibility from grade also stated.
- Check (right): Scene 8 ft violates, compliant 4 ft passes. 8 ft exceeds 6.5 ft; 4 ft is under it.
- Check (unclear): Measured to the centre of the device from finished grade. Sources do not state the measuring point; the centre convention is the app's own choice.

## BC-GARAGE-HEIGHT-001 (branch-circuit-requirements)

- Verdict: **corrected**, reference `210.52(G)(1)`
- 2026: no 2026 information found (not named in the 2026 change summaries I opened)
- Note: Number and reference are solid; only the 'attached garage' wording needs widening. Scene must make the 7 ft device the only receptacle in the bay.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_210.52.pdf says: 2023: each vehicle bay of a powered garage needs a receptacle no higher than 5 ft 6 in above the floor.
- Source: https://www.ecmweb.com/national-electrical-code/article/20902875/code-qa-receptacle-requirements-for-dwelling-unit-garages-and-accessory-buildings says: 2017 Q&A: attached or detached garages with power, receptacle per vehicle bay, no higher than 5 ft 6 in; no lower limit given.
- Source: https://captaincode2020.leviton.com/node/132 says: 2020 edition: each vehicle bay, not more than 1.7 m above the floor; no minimum height mentioned.
- Check (right): Reference 210.52(G)(1) in 2023. Holt 2023, ECM (2017) and Leviton (2020) all place garages at (G)(1).
- Check (right): No higher than 5.5 ft above the floor, one in each vehicle bay. All three opened sources give 5 ft 6 in (1.7 m) in each vehicle bay.
- Check (right): Scene 7 ft violates, 4 ft passes. 7 ft exceeds 5.5 ft.
- Check (wrong): Possible 18 in lower bound (local amendment snippet). The NEC sources I opened state no lower limit; the 18 in figure is not in the national text.
- Check (wrong): necText says 'attached garage' only. The 2017 ECM Q&A says attached or detached garages with power; the exception excludes only unattached multifamily garages.

## BC-BATH-BASIN-001 (branch-circuit-requirements)

- Verdict: **confirmed**, reference `210.52(D)`
- 2026: no 2026 information found (210.52(D) not named in the 2026 change summaries I opened)
- Note: NEAR-DUPLICATE of BATH-LOWOUTLET-001: same rule, same room, both end as 'no compliant receptacle at the sink', and both claim to be the bathroom's only receptacle so they cannot share one scene. Not a duplicate of BC-DEDICATED-BATH-001 (circuit) or the GFCI bathroom entry.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_210.52.pdf says: 2023: at least one receptacle within 3 ft of each bathroom sink's outside edge, on adjacent wall, counter, or cabinet side or face.
- Source: https://forum.nachi.org/t/one-outlet-for-two-bathroom-sinks/157926 says: Posters quote: receptacle within 900 mm (3 ft) of outside edge of each basin; edition not stated.
- Source: https://ecmweb.com/national-electrical-code/code-basics/article/20901808/receptacles-in-dwellings says: 2017 article: within 3 ft of the bathroom basin outside edge and not over 12 in below the basin top.
- Check (right): Reference 210.52(D) bathrooms (dwelling units). Holt 2023 and ECM 2017 both cite (D).
- Check (right): At least one 125 V 15 or 20 A receptacle within 3 ft of the outside edge of each basin. 3 ft from outside edge confirmed by Holt 2023, ECM 2017 and a forum quote of the rule.
- Check (right): Location: wall/partition adjacent to basin counter, on the countertop, or side/face of cabinet. Holt 2023 says 'wall or partition adjacent to the sink counter surface'; older-edition quote says 'basin or basin countertop'. I could not open the NFPA text for exact 2023 words.
- Check (right): Scene 48 in violates, compliant 24 in. 48 in exceeds 36 in; 24 in is within it.
- Check (unclear): Measured horizontally. The rule says 'within 3 ft of the outside edge' without naming a measuring method; horizontal is the app's convention.

## BC-COUNTER-HEIGHT-001 (branch-circuit-requirements)

- Verdict: **confirmed**, reference `210.52(C)(3)`
- 2026: 2026 keeps the 20 in above-counter location in (C)(3) and adds (C)(4) for prohibited locations below and beside countertops, with a drawer exception; (A)(5) mirrors it for work surfaces. Sources: ecmweb key-revisions article, Champlin MN FAQ, Broomfield list. Summaries disagree on exact below-counter numbers (24 in); NFPA text not opened.
- Note: Distinct from BC-SPACING-COUNTER-001 (that one is the 24 in spacing along the counter; this is height) but both end with a counter gap, so word the scene so the 28 in outlet is clearly the only one counted. 2026 does not make under-counter receptacles count as required ones; do not teach that.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_210.52.pdf says: 2023 (C)(3): required countertop receptacles on or not more than 20 in above the counter, or in countertop via listed assemblies.
- Source: https://aibd.org/islands/ says: 2023 island rules point to (C)(3): receptacles, if installed, may be at most 20 in above the counter.
- Source: https://www.ecmweb.com/qampa/stumped-code-using-floor-receptacles-and-wall-outlet-requirements-placing-receptacles-countert says: 2011 Q&A: serve countertops on or no more than 20 in above the surface.
- Check (right): 2023 lettering is 210.52(C)(3), item (1) on or above counter. Holt 2023 shows (C)(3) 'Countertop Receptacle Location' with item (1) up to 20 in above; item (2) listed assemblies in the countertop.
- Check (right): Max 20 in above the countertop. Holt 2023, aibd.org (2023 island article) and 2011 ECM Q&A all give 20 in.
- Check (right): Scene 28 in violates, 14 in passes. 28 in exceeds 20 in.
- Check (right): Old (C)(5) lettering, and below-counter option. Earlier editions used (C)(5) with a 12 in below-counter option; 2023 removed that option (aibd.org and a search summary), so below-counter does not satisfy the rule in 2023.

## BC-HVAC-RECEPT-001 (branch-circuit-requirements)

- Verdict: **corrected**, reference `210.63(A)`
- 2026: 2026 210.63: load-side-of-disconnect language removed and a 150 V limit added (IECI, Broomfield 2026 changes list). The 25 ft and same-level wording is not reported changed, but the 2026 text was not opened.
- Note: I could not open the 2023 text of 210.63 itself; the 2023 wording is inferred from the 2020 text plus no reported 2023 change, so the reviewer should check it against the book. Distance 'straight-line' is an app convention; the rule just says within 25 ft.
- Source: https://forum.nachi.org/t/required-outlet-at-exterior-units/53400 says: Quotes rule: 125 V single-phase 15 or 20 A receptacle at accessible location, same level, within 25 ft of the equipment.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=826 says: 2020 NEC 210.63(A): same level, within 25 ft, not on the load side of the disconnect; (B) covers other equipment.
- Source: https://ieci.org/national-electrical-code-2026-changes-to-chapter-two/ says: 2026 210.63 drops load-side language, limits to 150 V or less, and condenses the section to two sentences.
- Check (wrong): Reference is 210.63. Since the 2020 edition 210.63 has (A) for HVAC equipment and (B) for other equipment such as indoor service equipment, so the HVAC item is 210.63(A).
- Check (right): Same level and within 25 ft, 125 V 15 or 20 A. A forum quote of the rule and a 2020 NEC summary both state same level, within 25 ft, 125 V single-phase.
- Check (wrong): 2023 wording lists attics, crawl spaces and rooftops. Search summaries say those words were deleted in an earlier cycle; the opened 2020 text names no locations. The rule applies in those places anyway, so the newEntry sentence is true in effect.
- Check (wrong): Missing condition: not on the load side of the equipment disconnect. That language is in 2023 and was removed in 2026 (IECI, Broomfield list); the draft's necText omits it.
- Check (right): Exception for evaporative coolers in one- and two-family dwellings. Exists in 2020 text; not relevant to a furnace scene.
- Check (right): Scene 40 ft violates, 10 ft passes. 40 ft exceeds 25 ft; the scene must keep both items on the same attic level.

## BC-FLOOR-RECEPT-001 (branch-circuit-requirements)

- Verdict: **confirmed**, reference `210.52(A)(3)`
- 2026: no 2026 information found for (A)(3); the 2026 summaries mention (A)(2) and (A)(5) only; text not opened.
- Note: Overlaps BC-SPACING-WALL-001 (the wall ends up with no counted receptacle) and is a NEAR-DUPLICATE of WALL-HIGHOUTLET-001; ship at most one. Scene must show no other counted receptacle on that wall.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_210.52.pdf says: 2023 (A)(3): floor receptacles within 18 in of the wall can be counted as the required wall-space receptacle.
- Source: https://open-exam-prep.com/study-guides/mi-electrician/branch-circuits-outlets/dwelling-receptacle-spacing says: 2023-based guide: floor receptacles do not count unless within 18 in (450 mm) of the wall.
- Source: https://www.ecmweb.com/qampa/stumped-code-using-floor-receptacles-and-wall-outlet-requirements-placing-receptacles-countert says: 2011 Q&A cites (A)(3): floor outlets over 18 in from the wall are not counted.
- Check (right): 2023 lettering 210.52(A)(3) Floor Receptacles. Holt 2023 shows (A)(3) after (A)(1) spacing and (A)(2) wall space definition; (A)(4) is countertop receptacles.
- Check (right): Counts as a wall-space receptacle only within 18 in of the wall. Holt 2023, a 2023-labelled study guide and a 2011 ECM Q&A all give 18 in.
- Check (right): Scene 30 in violates, 12 in passes. 30 in exceeds 18 in.

## RP-OCPD-STAIRS-001 (working-space-access)

- Verdict: **confirmed**, reference `240.24(F)`
- 2026: Only 240.24(E) is reported changed in 2026 (new exception for existing bathroom panelboards); nothing found for (F). Sources: https://www.dli.mn.gov/sites/default/files/pdf/NEC-2026-adoption-review-092525-minutes.pdf and https://secure.utah.gov/ce-public/files/objectives/13437.pdf (change outlines, full text not seen).
- Note: Duplicate of the held-out working-space-access STAIR-PANEL-001 (same 240.24(F)); that one was single-source, this now has three sources, so ship only one. It belongs in working-space-access with the other 240.24 items (RP-PANEL-HEIGHT-001, RP-CLOSET-PANEL-001, RP-BATH-PANEL-001), not overcurrent-protection.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=3121 says: Lists 240.24(F) as barring overcurrent devices directly above stairway steps, next to (D) closets and (E) bathrooms.
- Source: https://iaeimagazine.org/2013/marchapril-2013/article-240-part-2-overcurrent-protection/ says: Names devices over steps as a prohibited location, for stable level footing (older edition).
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch07/ch07-sec01 says: 2020-edition study guide gives (F) as the no-devices-above-steps rule.
- Check (right): 240.24 lettering is (A) accessibility, (B) occupancy, (C) physical damage, (D) ignitible material, (E) bathrooms, (F) over steps. Letters agree across an ecmweb 2017 article (A-C), a North Dakota course page (D-F) and a 2020-edition study guide. 2023 text itself not opened.
- Check (right): A panel over stairway steps is the 240.24(F) rule. (F) bars overcurrent devices over steps; closet is (D), bathroom (E), so no letter clash with existing entries.
- Check (right): Scene: panel on wall above garage-to-house steps. Plainly over the treads breaks (F). A panel beside the steps or on a landing would be arguable, so keep it directly above.

## RP-OCPD-MAIN-OVER-PANEL-001 (overcurrent-protection)

- Verdict: **confirmed**, reference `408.36`
- 2026: no 2026 information found (not listed in the 2026 change outlines I opened; absence there is not proof of no change)
- Note: Distinct from both existing entries, so not a duplicate. Fits overcurrent-protection (device rating vs equipment rating); I could not open the 2023 text for exceptions.
- Source: https://expertce.com/learn-articles/panelboard-sizing-ocpd-rules-nec-408/ says: Every panelboard needs an integral or upstream overcurrent device rated no more than the panelboard; edition not stated.
- Source: https://viox.com/mcb-vs-mlo-panelboard-selection-guide/ says: Says 408.36 needs a supply-side device not exceeding the panelboard rating for main-lug panels; edition not stated.
- Source: https://iaeimagazine.org/?p=13232 says: 2015 article on the 2011 NEC: protecting device must not exceed panel rating; also covers the transformer-secondary case.
- Check (right): 408.36 requires OCPD rating not above the panelboard rating, in or on the supply side. Several secondary articles state this for main-breaker and main-lug panels. Exceptions and subsections not seen in 2023 text.
- Check (right): Overlap with RP-BUS-EXCEED-001 and RP-DISC-SVC-RATING-001. No overlap: RP-BUS-EXCEED-001 is 408.54 (more breakers than spaces); RP-DISC-SVC-RATING-001 is 230.79 (service disconnect below 100 A minimum).
- Check (unclear): Scene: 200 A main handle on a panel labelled 125 A. Visible and clearly wrong, but real panels ship with a matching main, so the scene is staged. State the nameplate rating plainly.

## RP-OCPD-PHYSDAMAGE-001 (working-space-access)

- Verdict: **confirmed**, reference `240.24(C)`
- 2026: Only 240.24(E) is reported changed in 2026; nothing found for (C). Sources: https://www.dli.mn.gov/sites/default/files/pdf/NEC-2026-adoption-review-092525-minutes.pdf and https://secure.utah.gov/ce-public/files/objectives/13437.pdf (outlines only).
- Note: No existing duplicate. It is a 240.24 location item, so it fits working-space-access (where the other 240.24 items live) better than overcurrent-protection.
- Source: https://ecmweb.com/ops-maintenance/where-can-you-put-breakers says: 2017 article lists 240.24(C) as keeping devices away from impact hazards, with bollards possible.
- Source: https://iaeimagazine.org/2013/marchapril-2013/article-240-part-2-overcurrent-protection/ says: Devices must not be subject to physical damage; bollard posts suggested for vehicles (older edition).
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch07/ch07-sec01 says: 2020-edition guide gives (C) as the physical-damage location rule.
- Check (right): (C) is the physical-damage letter. ecmweb 2017 article and a 2020-edition guide both give (C); letters otherwise match (D) closet, (E) bathroom.
- Check (right): 240.24(C) covers a garage panel in a vehicle path. Rule says devices go where not exposed to physical damage; bollards cited as a fix in vehicle areas. Judgment-based, the AHJ decides.
- Check (right): Scene: panel at bumper height at front of parking bay, dented cover, no guard. Visible and defensible; the dents and clear vehicle path make it unarguable.

## RP-OCPD-EDISON-FUSE-001 (overcurrent-protection)

- Verdict: **corrected**, reference `240.52`
- 2026: no 2026 information found (not listed in the 2026 change outlines I opened; absence there is not proof of no change)
- Note: Reference right; drop the 30 A oversize fuse from the scene to stay distinct from RP-WIRE-OVER-001 and BC-WIRE-14AWG-001. Fits overcurrent-protection.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/20897257/code-qa-edison-base-fuses-and-type-s-adapters says: Bare Edison-base holders are a violation; holders need adapters; 240.54 adapters cannot be removed once installed.
- Source: https://www.ecmweb.com/code-quiz-day/edison-base-fuseholder-requirements says: Edison-base fuseholders installed only where made to accept Type S fuses by adapters.
- Source: https://up.codes/s/edison-base-fuseholders says: 2023 AI summary: Edison-base holders used only with Type S fuses through adapters; full text not shown.
- Check (right): 240.52 requires adapters on Edison-base fuseholders. Two ecmweb pages and an up.codes 2023 summary all say holders only if made to accept Type S fuses via adapters.
- Check (unclear): A 30 A plug fuse on a small branch circuit. 30 A is the Edison-base maximum, but an oversize fuse adds an overfusing fault that muddies this item; remove it.
- Check (right): Edison-base fuses are replacement-only in existing installations (240.51(B)). That is about fuses; the fuseholder rule is separate. Scene must not read as a legal existing case.
- Check (unclear): componentType 'Breaker' for a fuseholder. Use a fuse-appropriate type if the app has one.

## WM-NM-BEND-001 (wiring-methods)

- Verdict: **confirmed**, reference `334.24`
- 2026: no 2026 information found for 334.24 (UpCodes lists only the 2023 edition for this section; the 2026 chapter 3 coverage found concerns listed staples and straps in 334.30, not bend radius)
- Note: Solid. Scene must display the cable width so the student can compute 5 x 0.5 in.
- Source: https://up.codes/s/bending-radius says: NFPA 70 2023 334.24: inner-edge radius at least five times diameter; major diameter used for flat cable.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1481.0 says: 2023 334.24 requires not less than five times the diameter; flat cable uses major dimension, new versus 2020.
- Source: https://expertce.com/learn-articles/navigating-nec-article-334-romex/ says: Summarizes 2023-era 334.24 as five times the major diameter for flat cable.
- Check (right): Minimum bend radius is five times the cable diameter. Three opened pages agree on five times, inner edge, during or after installation.
- Check (right): Flat cable uses major diameter. UpCodes and electricallicenserenewal both give major dimension for flat cable; the latter says 2023 added it.
- Check (right): 2.5 in. limit for a 0.5 in. cable. 5 x 0.5 = 2.5. Scene must show the cable's wide dimension is about 0.5 in. (typical 14/2 or 12/2 NM-B is roughly that).
- Check (right): Scene 1 in. radius is a violation; 3.0 in. compliant. 1 < 2.5 and 3.0 >= 2.5.

## WM-FREE-CONDUCTOR-001 (wiring-methods)

- Verdict: **confirmed**, reference `300.14`
- 2026: UpCodes' edition list shows this section as 300.16 in NFPA 70-2026 with no 2026 text displayed (single source). A general search and a tradesmance 2026 course page did not confirm or deny it.
- Note: Use 300.14 for the 2023 app. Reviewer should confirm the 2026 renumbering to 300.16 from the NFPA viewer before any 2026 mapping.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1464 says: 2023 300.14: 6 in. free conductor, may be spliced or unspliced; under 8 in. opening needs 3 in. outside; unspliced pass-through exempt.
- Source: https://up.codes/s/length-of-free-conductors-at-outlets-junctions-and-switch-points says: Same 6 in. and 3 in. figures for 2023; edition list shows 300.14 for 2023 and 300.16 for 2026.
- Source: https://captaincode2023.leviton.com/node/327 says: 2023 added wording that the 6 in. free conductor may be spliced; under-8-in. openings need conductors 3 in. outside.
- Check (right): At least 6 in. of free conductor at outlet, junction, switch points. Measured from where conductor leaves cable sheath or raceway inside the box.
- Check (right): Opening under 8 in. in any dimension: each conductor extends at least 3 in. outside. Two opened sources and Leviton 2023 change page quote 75 mm (3 in.) and 200 mm (8 in.).
- Check (right): Numbering 300.14 in 2023. UpCodes lists 300.14 for 2014-2023 editions.
- Check (unclear): 2026 number is 300.16. Only UpCodes' edition list shows 300.16 for 2026; no 2026 text opened. One source, so treat as likely but unconfirmed.
- Check (right): Scene 1 in. outside a small switch box breaks the rule. 1 < 3 in.; switch box opening is well under 8 in. Rule applies only to conductors spliced or terminated there.

## WM-NM-FLOOR-001 (wiring-methods)

- Verdict: **corrected**, reference `334.15(B)`
- 2026: UpCodes shows 334.15(B) Protection From Physical Damage still present at the same number in the 2026 Texas Electrical Code (NFPA 70-2026 based); no change in content found.
- Note: Keep the floor scene only; the 6 in. is not a rule for surface-run wall cable. Also drop 'heavy-wall PVC' in newEntry text and use Schedule 80 PVC.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-do-2023-nec-updates-protect-nm-and-nmc-cable-from-physical-damage/ says: Floor passage needs listed raceway or approved means extending 6 in. above floor; wall surface protection only where necessary.
- Source: https://captaincode2023.leviton.com/node/332 says: 2023 334.15(B): 6 in. above floor; new bushing or adapter requirement; surface wall cable protected only where necessary.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1479 says: 2023 334.15(B) protection where necessary, 6 in. above floor, and abrasion bushing or adapter at conduit ends.
- Source: https://up.codes/s/protection-from-physical-damage says: Texas residential code copy of the rule: 6 in. above floor; page cross-links Texas Electrical Code 2026 334.15(B).
- Check (right): Cable passing through a floor must be enclosed at least 6 in. above the floor. Four opened pages (ExpertCE, electricallicenserenewal, Leviton, UpCodes residential copy) give 150 mm (6 in.).
- Check (wrong): Enclosure types listed as 'heavy-wall PVC'. Rule names Schedule 80 PVC (and RTRC-XW), not plain heavy-wall PVC.
- Check (wrong): Does the 6 in. figure apply to cable on a wall surface?. No. Surface wall runs need protection only where subject to physical damage; 6 in. applies only to floor penetrations.
- Check (unclear): Exception exists. No exception found in the opened pages; 2023 added bushing or adapter at raceway entry and exit.
- Check (right): Scene 2 in. sleeve fails; 7 in. passes. 2 < 6 <= 7.

## WM-EMT-SUPPORT-001 (wiring-methods)

- Verdict: **confirmed**, reference `358.30(A)`
- 2026: no 2026 information found
- Note: Numbers are fine. Reviewer may want to read the 2023 358.30(A) exception list from the NFPA viewer; I could not open that text.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/21270216/stumped-by-the-code-nec-requirements-for-using-and-installing-emt says: EMT fastened within 3 ft of boxes and every 10 ft; 5 ft where structure prevents 3 ft; fastening measured at terminations.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/20898549/stumped-by-the-code-rules-for-securing-and-supporting-raceways says: 358.30 requires 3 ft from termination and 10 ft spacing; 5 ft exception for unbroken run when structure prevents it (2011 NEC article).
- Source: https://expertce.com/learn-articles/nec-article-358-emt-installation-support/ says: Support at no more than 10 ft and within 3 ft of boxes and terminations, with limited fishing circumstances.
- Check (right): Fasten within 3 ft of each box or termination. Three opened pages agree.
- Check (right): Supports at no more than 10 ft intervals. ECM and ExpertCE both give 10 ft.
- Check (right): 5 ft allowed where structural members prevent 3 ft. ECM pages cite this as the exception; one ECM page is based on 2011 NEC. 2023 wording not opened directly.
- Check (unclear): Other exceptions. A fished-conduit exception exists in recent editions per common knowledge but I did not open a page stating it; ExpertCE alludes to fishing before finish work.
- Check (right): Scene 6 ft to first strap breaks the rule even with 5 ft allowance. 6 > 5 > 3; open framing in scene so no exception applies.

## WM-PVC-EXPANSION-001 (wiring-methods)

- Verdict: **corrected**, reference `352.44(A)`
- 2026: UpCodes lists 352.44 in NFPA 70 2017, 2020, 2023 and 2026 under the same number; no 2026 text or change found.
- Note: Table values themselves were read from the 2020 edition; the 2023 table was not opened but the coefficient is unchanged in all sources. 100 ft is well above the 1/4 in. threshold either way.
- Source: https://www.mikeholt.com/files/PDF/20UNEC1_352.44.pdf says: 2020 Table 352.44: 70 F change gives 2.84 in. per 100 ft; 100 F gives 4.06; fitting needed at 1/4 in. or more.
- Source: https://www.jupiter.fl.us/DocumentCenter/View/4681/Expansion-Joints-For-Rigid-Polyvinyl-9-24-10 says: 2008 NEC note: 70 degree range gives 2.84 in. per 100 ft; add 30 degrees in direct sun (about 4.1 in.).
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-do-expansion-fittings-protect-underground-pvc-conduits/ says: 2023 352.44 now has (A) thermal expansion and new (B) earth movement for buried PVC emerging from grade.
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch05/ch05-sec02 says: 2020-based guide: 4.06 in. per 100 ft at 100 F, 0.25 in. threshold for expansion fittings.
- Check (right): Fitting needed when expected length change is 1/4 in. or more. Mike Holt (2020), ECM Q&A, Jupiter FL (2008) and open-exam-prep all agree.
- Check (right): Table value 2.84 in. per 100 ft at 70 F. Read the Mike Holt table text: 70 F = 2.84, 100 F = 4.06, 5 F = 0.20 per 100 ft.
- Check (wrong): Compliant 0.2 in. for a 5 ft run at 70 F. 5 ft at 70 F is about 0.14 in. The 0.20 figure is the table entry for a 5 F change over 100 ft.
- Check (unclear): 70 F swing is the code temperature. Code gives only the table; 70 F is a building-department assumption and Carlon adds 30 F for direct sun (giving 100 F, 4.06 in.). Drop 'in sun' from description.
- Check (wrong): Reference 352.44. 2023 split it: thermal expansion is 352.44(A); new 352.44(B) covers earth movement of buried PVC. Draft correctly avoids (B).

## WM-BOX-SETBACK-001 (wiring-methods)

- Verdict: **confirmed**, reference `314.20`
- 2026: no 2026 information found
- Note: The drywall question is settled by the rule naming gypsum. Reviewer may want the NFPA viewer for the 2023 sentence.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/21265003/stumped-by-the-code-nec-requirements-for-flush-mounted-boxes says: 2023: noncombustible finish permits up to 1/4 in. setback; combustible finish needs box flush or projecting; gypsum treated as noncombustible.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=285.0 says: 2017 text: gypsum and other noncombustible surfaces allow 1/4 in. setback; combustible surfaces flush or projecting.
- Check (right): Noncombustible surface: setback no more than 1/4 in.. ECM (2023) and electricallicenserenewal (2017) agree.
- Check (right): Combustible surface: box flush or projecting. Both pages say so; no setback is allowed.
- Check (right): Drywall counts as noncombustible so 1/4 in. applies. Rule text names gypsum among noncombustible finishes; ECM treats drywall under the 1/4 in. limit.
- Check (right): Scene 0.5 in. setback violates; 0.125 in. complies. 0.5 > 0.25 >= 0.125.
- Check (right): Edition numbering. 314.20 Flush-Mounted Installations in 2023.

## Held out

- branch-circuit-requirements BATH-LOWOUTLET-001: near-duplicate of BATH-BASIN-001 (same room and rule, different measurement)
- branch-circuit-requirements WALL-HIGHOUTLET-001: near-duplicate of FLOOR-RECEPT-001 (both are a receptacle that does not count toward wall spacing) and cited at a lead-in item
- overcurrent-protection OCPD-HORIZONTAL-001: verification: single-source. Sources conflict on whether 'impracticable' survives in 2023, so the correction avoids that phrase; the scene is a violation either way. Expert should check the 2023 text of 240.33 and its exceptions.
- overcurrent-protection OCPD-HANDLETIE-001: verification: single-source. Subsection number rests on one opened article (Mike Holt pages were blocked), so the expert should confirm (B)(2) in the 2023 text. Fits overcurrent-protection; no existing duplicate.
- wiring-methods NM-BOX-SECURE-001: verification: single-source. Yes, as drafted (14 in.) it is a partial near-duplicate of the existing 334.30 item because it also fails the 12 in. rule; change the scene to 10 in. to make it distinct. The 2023 text of the exception rests on a search snippet and older sources, so the expert should confirm it.
