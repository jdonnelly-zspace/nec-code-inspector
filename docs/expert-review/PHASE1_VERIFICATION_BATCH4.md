# Phase 1 verification notes, batch 4

Each violation added in content plan phase 1 was drafted from memory and then checked by a second pass against public
sources. **No official NEC text could be opened** (the NFPA viewer needs a login), so every item still needs a credential
expert. Evidence notes are paraphrased. Items that could not be confirmed, or whose scene was arguable, were held out and
are listed at the end.

## BC-REPLACE-RECEPT-001 (arc-fault-protection)

- Verdict: **corrected**, reference `406.4(D)(4)`
- 2026: Search snippets (not opened pages) say 2026 keeps the replacement rule and adds a readily-accessible requirement for replacement AFCI/GFCI devices. Treat as unconfirmed; no 2026 change found that breaks this item.
- Note: Safe to ship if the scene avoids the invented tag and the expert confirms no 2023 exception applies. A plain breaker in a bedroom also overlaps BC-AFCI-BEDROOM-001, so students may answer 210.12(B) instead.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: 2023 summary: replacement receptacles in dwelling units need arc-fault protection via outlet-type receptacle, receptacle behind one, or combination breaker.
- Source: https://www.nyeia.com/wp-content/uploads/2025/10/2023-NEC_AFCI-Requirements.pdf says: 2023 text: replacement receptacle where AFCI is required must be OBC AFCI receptacle, protected by one, or on combination breaker; no exceptions shown.
- Source: https://captaincode2020.leviton.com/node/249 says: For 2020 the no-available-product exception was removed because AFCI and dual-function receptacles exist. Confirms three options.
- Source: https://www.ecmweb.com/national-electrical-code/article/21182073/practically-speaking-triggered-by-definitions says: 2020-edition commentary: wording triggers on replacing the receptacle outlet; author expected 2023 to clarify it covers simple device swaps.
- Check (right): 406.4(D)(4) is the 2023 replacement-receptacle AFCI rule. Two 2023 sources cite 406.4(D)(4) with the same three options.
- Check (right): Three compliance options: OBC AFCI receptacle, receptacle behind an OBC AFCI receptacle, combination-type breaker. St. Paul and NYEIA 2023 documents list exactly these.
- Check (unclear): 2023 has exceptions for old ungrounded circuits and for no listed device. Leviton says the no-listed-device exception was deleted in 2020 as products exist. Neither 2023 summary shows any exception. St. Paul lists a 2-prong item that looks like a local gloss; expert should read the NFPA text.
- Check (right): Scene: bedroom, 120 V, 15 A, plain breaker. Bedrooms are a listed 210.12 area; rule is triggered by location, not circuit age.
- Check (wrong): Scene shows a replaced device (fresh-install tag). Nothing visible distinguishes a replaced receptacle; the student can only see a non-AFCI device on a plain breaker.

## GND-GEC-SPLICE-001 (earthing-bonding)

- Verdict: **confirmed**, reference `250.64(C)`
- 2026: 2026 allows splicing with listed grounding and bonding equipment at an accessible location, keeping compression and exothermic for any location (Texas TDLR summary at denisontx.gov and license-renewal page). One blog claims a buried-run exception was removed. A wire nut is still not listed grounding or bonding equipment (my inference), so the item stands.
- Note: The draft's '2023 text' evidence page is actually the 2026 text; 2023 wording rests on EC&M, forums and a search summary, not the NFPA viewer. Consider adding a 2026 note.
- Source: https://forum.nachi.org/t/spliced-ground/23044 says: Inspectors agree a wire nut is not acceptable for splicing a GEC; only irreversible compression connectors, exothermic welds or busbar methods.
- Source: https://ecmweb.com/national-electrical-code/whats-wrong-here/article/20896026/whats-wrong-here says: 250.64(C) allows splicing only by irreversible compression connectors listed for grounding and bonding or exothermic welding; edition not stated.
- Source: https://inspectorsjournal.com/topic/9109-spliced-grounding-electrode-conductors says: 2008-era thread; posters cite compression or weld splices and busbar sections, none endorse wire nuts.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1887 says: This page is 2026 text, not 2023: adds listed equipment at accessible splices, drops 'if necessary'.
- Check (right): 2023 250.64(C): GEC in one continuous length; wire-type splice only by irreversible compression connector listed as grounding and bonding equipment, or exothermic weld. Search summary of 2023 wording, EC&M and forum quotes agree; busbar, structural frame and water-pipe cases also permitted.
- Check (right): Busbar sections and metal frame or water-pipe connections are separate allowed cases. Listed as options (2) to (4) after the compression or weld option.
- Check (right): A wire nut does not qualify. Inspectors say wire nuts can melt open under surge; not an irreversible compression connector or weld.
- Check (right): Split bolts are also barred for splicing the main GEC. Forum says split bolt is acceptable only for a tap, not to splice the conductor; not tested against NFPA text.
- Check (right): Scene is visible and breaks the rule. A twisted pair in a wire nut midway on the conductor is a clear splice with no compression sleeve or weld.

## WM-BOX-ACCESS-001 (wiring-methods)

- Verdict: **corrected**, reference `314.29(A)`
- 2026: 2026 expands 314.29(A) with sub-items for boxes recessed into or behind finished surfaces, needing openings in covers and finish (IEC and Vector summaries, First Revision 7516); basic accessibility rule is unchanged.
- Note: Cite 314.29(A) not bare 314.29. Sub-item numbering in 2023 versus 2026 is the only open point.
- Source: https://www.mikeholt.com/files/PDF/23_UNEC1_314.29.pdf says: 2023 textbook: boxes in buildings must be installed so contents are accessible; accessible defined in Article 100 as exposed without damaging finish.
- Source: https://www.ecmweb.com/national-electrical-code/article/55129213/practically-speaking-lets-dig-into-sec-31429 says: Describes reorganised 314.29 with (A) for buildings and (B) for underground; wiring must stay accessible without removing structure.
- Source: https://ieci.org/national-electrical-code-2026-changes-to-chapter-three/ says: 2026 expands 314.29 for recessed boxes with specified opening sizes; calls the old section one sentence long.
- Check (right): 2023 314.29 requires boxes to be accessible; building rule is subsection (A). Mike Holt's 2023 text puts the buildings rule in (A) and handhole enclosures in (B).
- Check (right): A box covered by painted drywall with no opening fails. Article 100 'accessible' excludes boxes permanently closed in by building finish or needing damage to reach.
- Check (unclear): 2023 split 314.29 into sub-items beyond (A) and (B). Mike Holt 2023 shows (A) as one sentence; EC&M describes (A)(1) to (A)(3). Sub-items appear to be the 2026 text; expert should confirm 2023.
- Check (right): Scene: spliced box between studs under drywall, no cover or access panel. Realistic and clearly visible, but a student may also flag a missing cover (314.25); keep the cover implied, not the issue.
- Check (right): Removable finish allowances exist. Accessible includes being exposed by removing a panel or ceiling tile without damage; scene must show permanent drywall.

## RP-NEUTRALFUSE-001 (overcurrent-protection)

- Verdict: **confirmed**, reference `240.22`
- 2026: no 2026 information found for this rule; an opened IAEI page on proposed 2026 changes (iaeimagazine.org/columns/nfpa-code-talk/look-at-proposed-2026-nec-changes/) did not mention it
- Note: Rule text confirmed from one opened article plus the 2023 section title; wording has been stable for many editions. No duplicate in the existing context.
- Source: https://iaeimagazine.org/?p=8742 says: No overcurrent device in series with a grounded conductor unless it opens all conductors together, or for motor overload under 430.36/430.37. Edition not stated.
- Source: https://secure.utah.gov/ce-public/files/objectives/12208.pdf says: 2023 NEC overcurrent course outline lists 240.22 titled Grounded Conductor; section and title exist in 2023.
- Check (right): No overcurrent device in series with an intentionally grounded conductor unless it opens all conductors together with no pole acting alone. IAEI page states both conditions; stable rule.
- Check (right): Other exception is motor overload per 430.36/430.37. IAEI page gives the same; irrelevant to a plain garage feeder.
- Check (right): A fuse in a neutral never qualifies under the multipole condition. A fuse is single-pole and cannot open all conductors together, so only the motor-overload exception could apply (fuse in grounded leg of 3-wire 3-phase, 430.36).
- Check (right): Scene: third fuse on white neutral of a feeder to a sub-panel. Clear violation; keep the scene free of motors so the exception cannot apply.

## RP-SERIESLABEL-001 (overcurrent-protection)

- Verdict: **confirmed**, reference `110.22(C)`
- 2026: no 2026 information found for this rule; an opened IAEI page on proposed 2026 changes (iaeimagazine.org/columns/nfpa-code-talk/look-at-proposed-2026-nec-changes/) did not mention it
- Note: Only change is dropping the two-brands detail so the scene shows one fault. Mixed-brand breakers would be a second, unrelated violation.
- Source: https://ecmweb.com/national-electrical-code/quizzes/article/55136318/test-your-code-iq-september-2024 says: 2023 NEC 110.22(C): enclosures with series-rated breakers or fuses per 240.86(B) are legibly marked in the field.
- Source: https://code-authorities.ul.com/wp-content/uploads/2014/04/ul_PanelboardShortCircuitRatings.pdf says: UL: 110.22(C) requires the installer's readable field marking with the Caution series combination wording (pre-2023 document).
- Source: https://www.electrical-contractor.net/forums/ubbthreads.php/topics/80647/110-22-series-mrkg.html says: Marking needed only where a series rating is used; fully rated 22 kA panel needs none (2002 NEC discussion).
- Check (right): 110.22(C) is the 2023 lettering for the series-combination field marking. ECM Sept 2024 quiz cites 2023 NEC 110.22(C); UL guidance also cites 110.22(C).
- Check (right): Marking wording: Caution, series combination system rated ___ amperes, identified replacement components required. UL sheet gives this wording; 110.22(C) also points to 240.86(B) series ratings.
- Check (right): Applies only when a series combination is actually used, so the scene must show it. ECN thread (2002 NEC) says fully rated equipment needs no marking; marking applies when a series rating is relied on.
- Check (unclear): Breakers of two brands sit in the bus. Mixing brands is a separate listing problem and muddies a single-rule scene; I removed it.

## COM-ALUM-COLUMN-001 (conductor-sizing)

- Verdict: **confirmed**, reference `310.16`
- 2026: no 2026 information found for the table values; an opened IAEI page mentions new wire sizes in Article 310 for 2026, but nothing on 4 AWG ampacities
- Note: Not a duplicate of COM-FEEDER-SIZE-001 (generic undersized feeder, 215.2, no material fault) or COM-ALUM-OCPD-001 (12 AWG aluminum on 20 A, 240.4(D)(5)); close neighbour of the first, so avoid showing both on one panel.
- Source: https://www.portlandiaelectric.supply/blogs/electrical-accessories/wire-ampacity-chart-nec says: Labelled 2023 Table 310.16, 75 C: 4 AWG copper 85 A, aluminum 65 A (also 1/0 copper 150 A, aluminum 120 A).
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch04-conductors-ampacity/conductor-types-table-310-16 says: 4 AWG copper 70/85/95 A and aluminum 55/65/75 A at 60/75/90 C.
- Source: https://solutions.borderstates.com/resources/allowable-ampacities-insulated-conductors/ says: 1996 table gives the same 4 AWG values, showing they have not changed.
- Check (right): 4 AWG copper 85 A at 75 C. Three opened sources agree, including one citing 2023 Table 310.16.
- Check (right): 4 AWG aluminum 65 A at 75 C. Same sources; 60 C is 55 A and 90 C is 75 A, so even the 90 C column (75 A) is under 80 A.
- Check (right): Scene is a violation: 80 A load on 80 A breaker with 4 AWG aluminum. No column rescues it. The 310.12 dwelling 83 percent rule does not apply to an 80 A feeder.
- Check (unclear): Reference 310.16 is the right citation. The table is correct, but the duty broken is feeder ampacity at least equal to load, 215.2(A)(1), plus 240.4; 310.16 is acceptable as the table source.

## COM-NEUTRAL-NONLINEAR-001 (conductor-sizing)

- Verdict: **confirmed**, reference `310.15(E)(3)`
- 2026: no 2026 information found for 310.15(E)(3); the 2026-labelled study guide still cites 310.15(E), which suggests the lettering is unchanged
- Note: Numbers settled: 136 A is the right derated figure and is below the 140 A load. The scene fails only if loads are mainly nonlinear on a wye; the panel label for servers and LED drivers supports that. Distinct from COM-RACEWAY-DERATE-001 (bundle count, not neutral).
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch04-conductors-ampacity/ampacity-derating-corrections says: 2026-labelled guide: 1/0 THHN 170 A x 0.80 = 136 A, limited by 75 C terminals at 150 A; neutral counted for nonlinear load.
- Source: https://open-exam-prep.com/study-guides/ct-electrician/conductors-ampacity-cables/ampacity-derating says: Cites 310.15(E)(3) for nonlinear neutral counting under the 2020 NEC; 1/0 THHN is 125/150/170 A.
- Source: https://solutions.borderstates.com/resources/allowable-ampacities-insulated-conductors/ says: 1996 table: 1/0 copper 125/150/170 A at 60/75/90 C, unchanged.
- Source: https://www.portlandiaelectric.supply/blogs/electrical-accessories/wire-ampacity-chart-nec says: 2023 Table 310.16 at 75 C: 1/0 copper 150 A.
- Check (right): Reference is 310.15(E)(3) in 2023. Pre-2020 it was 310.15(B)(5)(c)/(4)(c); a 2020-era guide and a 2026-labelled guide both cite (E)(3)/(E), so 2023 sits between.
- Check (right): On a 4-wire wye feeder with mostly nonlinear load the neutral counts as current-carrying. Both opened guides agree; applies only when the major portion of load is nonlinear.
- Check (right): 1/0 AWG THHN copper: 60/75/90 C ampacity. 125 A, 150 A, 170 A per three opened sources.
- Check (right): Arithmetic: 170 x 0.8 = 136 A (not 150 x 0.8 = 120 A). Adjustment starts from the 90 C column for THHN; 75 C 150 A is only the terminal cap. Result 136 A, under the 150 A cap, below the 140 A load.
- Check (right): Four conductors take an 80 percent factor. Table 310.15(C)(1): 4 to 6 current-carrying conductors is 80 percent.

## BC-ISLAND-SIDE-001 (branch-circuit-requirements)

- Verdict: **corrected**, reference `210.52(C)(3)`
- 2026: Denison TX 2026 summary (https://denisontx.gov/DocumentCenter/View/4131/2026-National-Electrical-Code-Significant-Changes), opened: 210.52 clarifies no receptacles within 24 in below an island or peninsula counter; drawers excepted.
- Note: Reference and rule are right for the unamended 2023 NEC; AHJs that amended it (Phoenix, possibly others) permit the below-counter outlet, so label the app as model-code based. Near-duplicate territory: BC-COUNTER-HEIGHT-001 also cites 210.52(C)(3) (too high, 28 in above), the opposite direction, so distinct but similar.
- Source: https://www.larimer.gov/sites/default/files/uploads/2023/2023_nec_significant_code_changes.pdf says: 2023 island outlets, if installed, go on or up to 20 in above the counter or in listed assemblies; no below-counter option listed.
- Source: https://web-prod.phoenix.gov/content/dam/phoenix/pddsite/documents/codes-ordinances/amendmentcodes/2023-nec.pdf says: Phoenix amendment (effective Aug 2025) adds back the 12 in below-counter exception, showing the base 2023 text lacks it.
- Source: https://www.nahb.org/blog/2026/03/new-electrical-code-change-for-kitchen-islands says: Where unamended 2023 NEC applies, island side outlets can no longer be used to satisfy countertop outlet provisions.
- Source: https://aibd.org/islands/ says: CONFLICT: claims 2023 still has a 12 in below-counter exception for flat islands; looks like 2020 wording and conflicts with Phoenix.
- Check (right): 2023 base text has no allowance for mounting up to 12 in below an island counter. Base (C)(3) lists only three locations; Phoenix had to adopt a local amendment to restore the below-counter exception.
- Check (right): Receptacle on an island is optional, with a provision for a future one, if none installed. Both the Larimer summary and the Phoenix amendment document quote this (C)(2) wording.
- Check (unclear): Limit 'drop below counter max 0 in' is a fair statement of the rule. Equivalent in effect, but 'height relative to counter, min 0' mirrors the rule (on or above, up to 20 in) and avoids a negative-limit reading.
- Check (right): Scene: duplex outlet on cabinet side face 10 in under counter, nothing on or in the counter. Visible, measurable, and breaks the rule as written because an installed island outlet serving the counter must use a listed location.
- Check (right): 2026 prohibits receptacles within 24 in below an island counter, drawer exception. Stated in the Denison 2026 significant-changes summary I opened; not read from NFPA text.
- Check (right): Lettering: location rule is (C)(3), optional-plus-provision is (C)(2). Matches the Phoenix document quoting 2023 text.

## BC-GARAGE-BAY-001 (branch-circuit-requirements)

- Verdict: **confirmed**, reference `210.52(G)(1)`
- 2026: no 2026 information found (Denison 2026 summary lists no 210.52(G) change).
- Note: Near-duplicate check: BC-GARAGE-HEIGHT-001 cites (G) but tests height, so this is a distinct failure (count per bay). Consider softening the 'cannot be shared' sentence in necText.
- Source: https://stpaul.gov/sites/default/files/2024-10/DSI.Bldg_Electrical_Checklist%20Garage.pdf says: 2023 NEC garage checklist: receptacle needed in each vehicle bay, not above 5.5 ft.
- Source: https://www.ecmweb.com/national-electrical-code/article/20902606/taking-the-guesswork-out-of-garages says: 2017 revision: at least one receptacle in each vehicle bay, not above 5.5 ft; door-opener outlets do not count.
- Source: https://captaincode2023.leviton.com/node/310 says: 2023 page refers to (G)(1) receptacles in attached garages and detached garages with electric power on a 20 A circuit.
- Check (right): At least one receptacle is required in each vehicle bay. St Paul 2023 checklist and an EC&M article both say each bay; Leviton 2023 and expertce snippets agree.
- Check (right): Attached garage, or detached garage with power. Leviton 2023 page opened refers to receptacles required by (G)(1) for attached garages and detached garages with electric power.
- Check (right): Maximum height 5.5 ft above floor. St Paul 2023 checklist: not above 5.5 ft.
- Check (unclear): Search snippets mention an 18 in minimum height in 2023. Not seen in any opened page; item does not use it, so leave out.
- Check (unclear): One outlet cannot be shared between bays. Follows from one-per-bay wording; no source states it explicitly. Safe to soften.
- Check (unclear): Multifamily exception (shared garage spaces) exists. Only in search snippets, not opened; scene is a single dwelling so it does not apply.

## RP-ATTIC-PANEL-001 (working-space-access)

- Verdict: **confirmed**, reference `240.24(A)`
- 2026: no 2026 information found (Denison summary lists no 240.24 change).
- Note: Only forum and trade sources were opened for the attic application; the definition is consistent across both. Scene must show a loose portable ladder and no fixed stair.
- Source: https://ecmweb.com/whats-wrong-here/article/20901782/whats-wrong-here-hint-let-me-vent-my-frustration says: Breakers must be readily accessible; definition rules out tools, climbing over obstacles and portable ladders.
- Source: https://www.garagejournal.com/forum/threads/panel-in-attic-space.174839/ says: Posters: scuttle hole plus ladder fails readily accessible; permanent stairs acceptable like a basement.
- Check (right): Overcurrent devices must be readily accessible; no portable ladders. EC&M and a forum both cite 240.24(A) and the Article 100 definition excluding portable ladders and climbing over obstacles.
- Check (right): Attic panel via scuttle hole with portable ladder fails. Forum consensus (garagejournal) and a trade-magazine search summary agree; permanent stairs are generally accepted.
- Check (right): Pull-down attic stairs may also be challenged by some AHJs. Search summary of Mike Holt discussion; scene correctly avoids this by showing a loose stepladder.
- Check (right): Item is distinct from existing 240.24(A) entries. RP-PANEL-HEIGHT-001 tests handle height (6 ft 7 in); this tests access method. Same section, different failure.

## RP-DISC-SVC-SEPARATED-001 (disconnecting-means)

- Verdict: **confirmed**, reference `230.72(A)`
- 2026: no 2026 information found (Denison summary lists no 230.72 change).
- Note: This is the rebuilt held DISC-SVC-GROUP-001 (separation half only), so a near-duplicate of that held item but it intentionally replaces it. The necText correction narrows the exception wording and adds the plaque.
- Source: https://www.ecmweb.com/code-basics/services-and-nec-part-2-2 says: Disconnects for a service are grouped so responders who find one find all; fire pump and standby ones are remote.
- Source: https://www.ecmweb.com/mro-insider/electrical-services-part-15 says: Cites 230.72(A) grouping, with a remote exception identified at the service by a plaque.
- Source: https://www.inspectorsjournal.com/topic/9078-service-disconnect-two-family-residence/ says: Posters: disconnects must be grouped; panels in separate units do not satisfy it.
- Source: https://expertce.com/learn-articles/service-disconnect-rules-nec-230-part-vi/ says: 2020/2023 article: 230.72 requires multiple disconnects grouped in one location.
- Check (right): 2023 lettering: grouping rule is 230.72(A). EC&M cites 230.72(A); search summary of 2023 shows (A) General and (B) Additional disconnecting means.
- Check (right): Disconnects must be grouped in one location. Two EC&M articles and an inspectors forum agree; 'grouped' is the term, 'same location' is the paraphrase.
- Check (unclear): Exception is a water pump also used for fire protection. Seen in search summary only; EC&M part 15 mentions a remote fire pump disconnect with plaque, which in 2023 is more likely (B) additional means.
- Check (right): Fire pump, emergency and standby disconnects are separate remote ones. EC&M part 2 says these must be remote from normal ones; that is 230.72(B), not an exception to (A).
- Check (right): Max six is 230.71(B). Expertce 2023 lesson cites 230.71(B), matching the existing entry; related 230.71(B) is fine.
- Check (unclear): 2026 moved emergency disconnect text into 230.70. Not found in any page I opened.
- Check (right): Scene: garage and laundry-room disconnects of one service break grouping. Separated locations of the same service fail the grouping rule; make clear it is one service.

## RP-DEADFRONT-001 (equipment-installation)

- Verdict: **confirmed**, reference `408.38`
- 2026: no 2026 information found (a general 2026 search found nothing on 408.38; the only 2026 panelboard item surfaced was surge protection for legally required standby, not this rule)
- Note: Reference and content are right for 2023. Dwelling-garage scene means the qualified-person exception cannot be argued.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-are-2023-nec-panelboard-enclosure-requirements-different/ says: 2023 408.38: cabinets, cutout boxes or identified enclosures, dead-front; qualified-person exception kept; new evaluation line above 10,000 A fault current.
- Source: https://ecmweb.com/national-electrical-code/code-basics/article/55247439/nec-requirements-for-switchboards-and-panelboards says: Panelboards go in cabinets, cutout boxes or identified enclosures with dead-front covers; edition not stated in the article.
- Check (right): 408.38 requires panelboards in a cabinet, cutout box or identified enclosure and dead-front. Two opened pages give the same requirement; one is explicitly about the 2023 text.
- Check (right): Non-dead-front panelboards allowed only where accessible to qualified persons only. The exception is unchanged in 2023; it does not help a dwelling garage panel.
- Check (right): No 2023 change that alters the item. 2023 added an evaluation requirement above 10,000 A available fault current for the panelboard-plus-enclosure; irrelevant to a missing cover.
- Check (right): Scene (open door, cover missing, bus and wire ends reachable) breaks the rule as written. A visible, reachable-live-parts front is a clear dead-front failure. Not the same fault as the empty-slot filler (408.7).

## RP-NEUTRALSHARE-001 (equipment-installation)

- Verdict: **confirmed**, reference `408.41`
- 2026: no 2026 information found
- Note: Rule is long-standing and stable, but I did not open 2023 text itself; the two sources do not name the edition. Different fault from the held double-tap breaker item (110.14(A)).
- Source: https://ecmweb.com/national-electrical-code/code-basics/article/55247439/nec-requirements-for-switchboards-and-panelboards says: Each neutral ends in an individual terminal; parallel conductors may share one identified for more than one. Separate 408.40 covers ground bars.
- Source: https://forum.nachi.org/t/is-this-a-problem/42947 says: Quotes the grounded-conductor rule; discussants say a neutral and a ground may not share a screw. Edition not stated.
- Check (right): 408.41 requires each grounded conductor to end in its own terminal in the panelboard, not shared with another conductor. Both opened sources give the same requirement; neither states the edition outright.
- Check (right): Exception: parallel conductors of one circuit may share a terminal identified for more than one conductor. Both sources give this exception. It covers one circuit's parallel set only, not two circuits.
- Check (right): 2023 lettering of 408.41. No (A)/(B) lettering appeared in any source; it is cited as a single section.
- Check (unclear): A neutral sharing a terminal with a ground wire is the same rule. A forum quote reads 408.41 as barring any other conductor under a neutral screw, including a ground. Rules on ground bars sit in 408.40; 250.24(A)(5) is the service neutral-ground bond. Not this item.
- Check (right): Scene: two white wires from separate circuits under one screw, other screws empty, terminal not marked for two. Breaks the rule as written. Student must tell the terminal is not marked for two conductors, which is hard to see; the scene should show empty screws.

## RP-DIRECTORY-OCCUPANT-001 (identification-marking)

- Verdict: **confirmed**, reference `408.4(A)`
- 2026: no 2026 information found
- Note: Near-duplicate risk: RP-DIR-MISS-001 (408.4, Panel_CircuitDirectory) covers a missing or inaccurate directory; this one is a present, accurate-looking directory whose wording is occupancy-based. Different fault, same reference family and same panel label, so the owner may prefer one directory item.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-to-properly-label-switchgear-switchboards-and-panelboards-per-nec-2023/ says: 2023 408.4: clear, specific descriptions; no dependence on transient occupancy; directory on, inside or next to the door.
- Source: https://ecmweb.com/national-electrical-code/code-basics/article/55247439/nec-requirements-for-switchboards-and-panelboards says: Descriptions must be clear and specific and not rely on transient occupancy such as 'dad's office'; abbreviations must be explained.
- Source: https://captaincode2020.leviton.com/node/254 says: 2020 408.4(A): no transient-occupancy wording; 'Bill's room' and 'Mary's office' are unacceptable; directory location widened.
- Check (right): 408.4(A) requires each circuit identified as to a clear, evident and specific purpose, in a directory. 2023 wording keeps 'specific'. 2023 retitled 408.4 'Descriptions Required' and allows the directory on, inside or adjacent to the panel door.
- Check (right): A description may not depend on transient conditions of occupancy. All three opened pages state this; the 2020 explainer and the trade article give names like 'Bill's room' and 'dad's office' as examples.
- Check (right): Using an occupant's name instead of a room identification breaks the rule. Yes: a person's name ties the entry to current occupancy. A plain room or load name ('Bedroom 2', 'Kitchen receptacles') is fine.
- Check (unclear): One 2023 explainer implies a room name like 'Master Bedroom' is itself a problem. Looks like a loose summary. The rule bars occupancy-dependent wording, not room names; other sources treat room names as acceptable.
- Check (right): Scene (entries like a named person's bedroom and office) breaks the rule. Matches the cited examples. Keep the entry wording like 'Dad's office' so the failure is visible on the directory.

## SL-POOL-LIGHTDEPTH-001 (special-locations)

- Verdict: **confirmed**, reference `680.23(A)(5)`
- 2026: Holt's 2026 guide (URL above) keeps 680.23(A)(5) with the 18 in. rule and the listed-for-lesser-depth allowance. No change found.
- Note: Not a duplicate: SL-POOL-LUMINAIRE-001 is overhead height at 680.22(B). 2023 text itself was not opened; subsection number inferred from 2017 and 2026.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017 guide 680.23(A)(5): top of wall-mounted luminaire lens not less than 18 in. below normal water level.
- Source: https://www.ecmweb.com/home/article/20899039/location-requirements-for-wall-mounted-pool-luminaires says: 2011 NEC 680.23(A)(5): lens top 18 in. below water, listed-for-less exception, never under 4 in.
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/8eed9b9bfcd66c4deeb81e447c595212.pdf says: 2026 guide 680.23(A)(5): 18 in. below normal water level unless listed and identified for lesser depths.
- Check (right): Subsection is 680.23(A)(5) for wall-mounted luminaires. 2011 (EC&M), 2017 and 2026 (Holt) all give (A)(5). 2023 not opened but sits between two matching editions.
- Check (right): Top of the lens at least 18 in. below normal water level. Same number in 2011, 2017 and 2026 sources.
- Check (right): Unless listed for a lesser depth. The 2026 guide and the 2011 article both carry this allowance; 2017 guide omits it. The draft's exception is fine.
- Check (unclear): Separate 4 in. absolute minimum. Seen only in the 2011 article. Not used in the draft; do not add it.
- Check (right): Scene: 6 in. lens depth, no shallow-use marking; compliant 20 in.. 6 is under 18 and 20 is over 18. Student can see a marking or its absence only if the label is modelled.

## Held out

- arc-fault-protection FIRST-OUTLET-002: verification: corrected (keep=false). Drop or merge with held DOWNSTREAM-001; ship at most one of the two. The compliant counterpart depends on a listed branch/feeder breaker or a system-combination pair, whose retail availability I could not confirm.
- arc-fault-protection OBC-LENGTH-001: verification: corrected (keep=false). Rule and limits are right, but only a niche listed pair (Eaton) satisfies the option and NYEIA calls it nonexistent, so this tests a rarely seen setup; expert decides. If shipped, use 55 ft and show the packaged pair.
- overcurrent-protection HANDLETIE-002: verification: single-source. NEAR-DUPLICATE: same scene as the held item OCPD-HANDLETIE-001 (240.15(B)(2), nail through handles); ship only one. The (B)(2) letter is not seen in 2023 text, so the expert should confirm it.
- overcurrent-protection SWD-001: verification: single-source. Three opened sources agree on (D) but none is the 2023 text, so the 2023 letter rests on the rule being unchanged since at least 2008-2020. The scene must show fluorescent fixtures; the draft does say so.
- working-space-access LIGHTING-WORKSPACE-001: verification: single-source (keep=false). Same scene was held before (LIGHT-001): near-duplicate of that held item. Rule likely applies to dwellings, but 2023 text was not opened and the scene overlaps 210.70(A)(2)(a); expert may flip to keep.
- special-locations POOL-CIRCRECEPT-001: near-duplicate of SL-POOL-RECEPT-001, and 680.22(A)(2) is the pump receptacle rule, not the one the draft describes
- special-locations POOL-LIGHTGFCI-001: verification: single-source. Only one author (Holt, two editions) supports the number; no independent 2023 page opened. Conceptually close to SL-POOL-PUMPGFCI-001 (missing GFCI on pool equipment), but different equipment and reference.
