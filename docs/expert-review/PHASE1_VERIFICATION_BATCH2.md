# Phase 1 verification notes, batch 2

Each violation added in content plan phase 1 was drafted from memory and then checked by a second pass against public
sources. **No official NEC text could be opened** (the NFPA viewer needs a login), so every item still needs a credential
expert. Evidence notes are paraphrased. Items that could not be confirmed, or whose scene was arguable, were held out and
are listed at the end.

## COM-SERVICE-CONDUCTOR-001 (conductor-sizing)

- Verdict: **corrected**, reference `230.90(A)`
- 2026: Search summaries (tradesmance) say 230.42(A)(1) and (A)(2) are carried in the 2026 NEC; no information found on 230.90(A) changes.
- Note: The drafted reference misattributes the disconnect-rating comparison to 230.42(A); recommend 230.90(A) as the primary reference. Reviewer should confirm 230.42(B) or 230.79 does not also carry a disconnect-rating rule in 2023, since I could not open the NFPA text.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/55021883/nec-requirements-for-services-part-2 says: 230.42(A)(1) is 125% continuous plus 100% noncontinuous; 230.90 says the service overcurrent device rating cannot exceed phase conductor ampacity.
- Source: https://open-exam-prep.com/study-guides/va-electrician/services-and-feeders/service-entrance says: 2020-based guide: 230.90 requires the service overcurrent device not to exceed conductor ampacity; it also ties disconnect rating to 230.79.
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch04-conductors-ampacity/conductor-types-table-310-16 says: Table 310.16 copper: 3 AWG is 85, 100 and 115 A in the 60, 75 and 90 C columns.
- Check (wrong): 230.42(A) compares service conductor ampacity with the service disconnect rating. In 2023, 230.42(A)(1) is the 125%-continuous load formula and (A)(2) is adjustment/correction factors; both are load-based. A 2020-based guide put disconnect rating under 230.79.
- Check (right): 3 AWG on a 200 A main is wrong. Wrong because of 230.90(A) (and 240.4), not 230.42(A), unless the stated load is also above 100 A.
- Check (right): 3 AWG copper is 100 A (75 C). Table 310.16: 85 / 100 / 115 A at 60 / 75 / 90 C. Source is an open-exam-prep table page; 100 A at 75 C also seen in a second page, though unreliable.
- Check (right): 230.42 was rewritten in 2023 with (A)(1) and (A)(2). ECM 2024 article cites both paragraphs; search summaries agree.

## COM-ALUM-OCPD-001 (conductor-sizing)

- Verdict: **confirmed**, reference `240.4(D)(5)`
- 2026: no 2026 information found (a 2026 page only mentions Table 310.16 gaining a 14 AWG CCA row).
- Note: Use the item-level (D)(5) reference only if the app tracks sub-paragraphs; old guides list this as (D)(4). The newEntry text is fine but does not mention the new 2023 14 AWG CCA 15 A limit.
- Source: https://open-exam-prep.com/study-guides/id-electrician/conductors-raceways-box-calculations/conductor-ampacity-tables says: 2023 guide: 12 AWG aluminum maximum 15 A overcurrent protection, 10 AWG aluminum 25 A.
- Source: https://www.portlandiaelectric.supply/blogs/electrical-accessories/wire-ampacity-chart-nec says: Supplier chart: 12 AWG aluminum at 75 C shows 20 A but is capped at 15 A under 240.4(D).
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/can-i-use-14-awg-copper-clad-aluminum-conductors-in-my-led-lighting-circuits/ says: 2023 NEC added 14 AWG CCA as 240.4(D)(3), shifting later sizes to (4) through (8).
- Source: https://open-exam-prep.com/study-guides/mi-electrician/overcurrent-protection-tap-rules/small-conductor-rules says: 2023 Michigan guide: small-conductor limits apply after any correction or adjustment factors; lists 12 AWG aluminum at 15 A.
- Check (right): 12 AWG aluminum or CCA is limited to 15 A overcurrent protection. Two guides (2023-based) and a wire-chart page agree; the 20 A table ampacity is capped at 15 A.
- Check (right): 10 AWG aluminum or CCA is limited to 25 A. Same sources.
- Check (right): Limit applies after correction and adjustment factors. Open-exam-prep 2023 Michigan guide states the limits apply after correction or adjustment factors.
- Check (right): Subparagraph (D)(5) for 12 AWG aluminum in 2023. 2023 added a 14 AWG CCA item as (D)(3), pushing 12 AWG aluminum to (D)(5) and 10 AWG aluminum to (D)(7). Open-exam-prep pages still show older numbering.
- Check (right): Copper 12 AWG may use 20 A. (D)(6) in 2023 numbering; 12 AWG copper 20 A, 14 AWG copper 15 A, 10 AWG copper 30 A.

## COM-RACEWAY-DERATE-001 (conductor-sizing)

- Verdict: **corrected**, reference `310.15(C)(1)`
- 2026: no confirmed 2026 information. One low-rigor site (ecalpro) claims the 7-9 conductor factor changes to 0.65; unsupported by any NFPA source.
- Note: As drafted the scene may be legal for non-receptacle loads via 240.4(B), so the corrected scene uses receptacle circuits. Reviewer should rule on whether 240.4(B) round-up applies after derating.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/55277245/nec-requirements-for-conductors says: With four or more current-carrying conductors, adjust the 90 C Table 310.16 values using Table 310.15(C)(1).
- Source: https://elliottelectric.com/StaticPages/ElectricalReferences/ElectricalTables/Derate_Conductors.aspx says: Adjustment factors: 4-6 is 80%, 7-9 is 70%, 10-20 is 50%, 21-30 is 45%.
- Source: https://open-exam-prep.com/study-guides/id-electrician/conductors-raceways-box-calculations/conductor-ampacity-tables says: 2023 guide cites 310.15(C)(1) for bundling adjustment and 240.4(D) limits applying after adjustment.
- Check (right): Table 310.15(C)(1) factors are 80% for 4-6, 70% for 7-9, 50% for 10-20. Elliott table (2020 labelled) and 2025 EC&M page agree; 2023 factors believed unchanged but I did not see a 2023 table.
- Check (right): Adjustment starts from the 90 C column (30 A for 12 AWG THHN) giving 15 A at 50%. 30 x 0.5 = 15 A; the 75 C value (25 A) is higher, so 15 A governs.
- Check (unclear): A 15 A adjusted conductor on a 20 A breaker is a violation. 240.4(B) round-up may allow 20 A on non-receptacle circuits; whether it applies after derating is disputed. Only a search summary (not opened) says it does not.
- Check (right): Count of ten current-carrying conductors. Five 2-wire circuits give ten; grounding conductors are not counted. Rule needs the raceway to exceed 24 in. (not stated in scene).
- Check (right): 310.15(C)(1) is the 2023 reference. 2025 EC&M article cites Table 310.15(C)(1) with the 90 C column starting point.

## COM-CONTINUOUS-WIRE-001 (conductor-sizing)

- Verdict: **corrected**, reference `210.19(A)(1)(a)`
- 2026: no 2026 information found (search summaries only say 210.19(A)(1) has clarifying changes).
- Note: The rule is right but the sign-circuit scene conflicts with 600.5(B); an EV charger (625.40, 625.41) fits a 48 A continuous load. 2023 lettering (a) is single-source.
- Source: https://www.ecmweb.com/national-electrical-code/article/21182900/the-nec-and-branch-circuit-ratings-part-1 says: Branch-circuit conductor ampacity must cover noncontinuous load plus 125% of continuous load under 210.19(A)(1).
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch12/ch12-sec01 says: Sign circuits are capped at 20 A or 30 A by 600.5(B)(1); EVSE overcurrent protection is 125% of load under 625.41.
- Source: https://www.portlandiaelectric.supply/blogs/electrical-accessories/wire-ampacity-chart-nec says: Chart shows 8 AWG copper 50 A and 6 AWG 65 A in the 75 C column.
- Check (right): 210.19(A)(1)(a) requires conductors at 125% of continuous plus 100% of noncontinuous load. EC&M (2020 NEC) and a search summary agree; (a) lettering for 2023 only from a search summary.
- Check (right): 48 x 1.25 = 60 A. Arithmetic correct.
- Check (right): 8 AWG copper is 50 A at 75 C, 6 AWG is 65 A. Table 310.16 75 C column; supplier chart shows 8 AWG 50 A and 6 AWG 65 A.
- Check (wrong): Sign lighting transformer circuit of 48 A is realistic. 600.5(B) caps sign branch circuits at 20 A (ballasts, transformers, drivers) or 30 A (lamps); a 48 A sign branch circuit is not realistic.
- Check (unclear): 60 A breaker on 50 A conductors is allowed by 240.4(B). 50 A rounds up to 60 A as the next standard size, so the conductors, not the breaker, are the fault; also keeps 625.41 breaker rule satisfied.

## COM-PARALLEL-SMALL-001 (conductor-sizing)

- Verdict: **confirmed**, reference `310.10(G)(1)`
- 2026: Search summaries (Mike Holt thread title, not opened) say a 2026 proposal moves only Exception 1 (control power, 360 Hz) of 310.10(G)(1) to a new 240.21 control tap provision. The 1/0 AWG minimum appears to stay. Final 2026 numbering not confirmed.
- Note: Page labels on 2020 versus 2023 numbering disagree with each other, so the (H) to (G) move is not firmly verified. The 2026 section number may change; check before 2026 release.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/21260929/stumped-by-the-code-nec-requirements-paralleling-of-conductors says: Parallel phase and neutral conductors must be 1/0 AWG or larger; grounding conductors need not be.
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch04/ch04-sec05 says: 310.10(G)(1) permits paralleling of 1/0 AWG and larger; same length, material, size, insulation, termination.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/55277245/nec-requirements-for-conductors says: 2025 article cites 310.10(G) requirements for parallel conductors.
- Check (right): Parallel conductors must be 1/0 AWG or larger. Two opened pages say 1/0 AWG minimum for phase and neutral; grounding conductors are not subject to it.
- Check (right): 2023 reference is 310.10(G)(1). 2025 EC&M article and open-exam-prep use (G); a Mike Holt thread title (search result only) also uses 310.10(G)(1). Earlier editions used (H) from my recollection, unverified.
- Check (right): Two 2 AWG per phase is a violation. 2 AWG is below 1/0; the exceptions (control power, 360 Hz and above) do not apply to a sub-panel feeder.
- Check (right): Each set must match in length, material, size, insulation, termination. Matches both pages.

## COM-TERMINAL-60C-001 (conductor-sizing)

- Verdict: **confirmed**, reference `110.14(C)(1)(a)`
- 2026: no 2026 information found.
- Note: Scene is rare in real equipment (most modern lugs are 75 C marked), but it is a legitimate label-reading item. The 40 A figure rests on one opened table page.
- Source: https://open-exam-prep.com/study-guides/id-electrician/conductors-raceways-box-calculations/conductor-ampacity-tables says: 2023 guide: circuits of 100 A or less use the 60 C column unless terminals are listed and marked for 75 C.
- Source: https://ecmweb.com/national-electrical-code/article/21274530/ecm-tech-talk-video-requirements-for-electrical-connections says: 2023 NEC 110.14 talk: THHN on 14 through 1 AWG at 100 A or less uses the 60 C column.
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch04-conductors-ampacity/conductor-types-table-310-16 says: 8 AWG copper: 40 A at 60 C, 50 A at 75 C, 55 A at 90 C.
- Check (right): For circuits of 100 A or less, use the 60 C column unless terminals are listed and marked for 75 C. 2023-based guide and EC&M Tech Talk (2023) agree; 14 AWG THHN example uses the 60 C column.
- Check (right): 8 AWG copper 60 C ampacity is 40 A. Open-exam-prep Table 310.16 lists 40, 50, 55 A for 8 AWG copper. Only one opened page gives the 60 C value.
- Check (right): A 45 A load on 8 AWG on 60 C terminals violates the limit. 40 A is less than 45 A; with 75 C-marked terminals 8 AWG would be 50 A and pass, which makes the label the discriminator.
- Check (unclear): Letter (a) is the 2023 paragraph. Sources give 110.14(C)(1) generally; (a) from search summaries. Reviewer should confirm (a)(1) versus (a).

## COM-ROOFTOP-ADDER-001 (conductor-sizing)

- Verdict: **confirmed**, reference `310.15(B)(2)`
- 2026: no 2026 information found (a June 2026 manufacturer page mentions no 2026 change to the 3/4 in. threshold).
- Note: Near-duplicate of AMBIENT-BOILER-001 (same 8 AWG THHN, 50 A breaker, 90 C ampacity times a factor). Prefer keeping this one; it has the measurable 0.75 in. limit. Drop the XHHW-2 sentence from newEntry unless confirmed for 2023.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1466 says: 2023 NEC 310.15(B)(2): adder applies when the raceway bottom is under 3/4 in. above the roof, down from 7/8 in.
- Source: https://www.miroind.com/2026/06/03/conduit-roof-supports-nec-requirements/ says: Manufacturer guide: 3/4 in. under 2023 NEC, 7/8 in. under 2017 and 2020; adder is 33 C (60 F).
- Source: https://ecmweb.com/national-electrical-code/article/21173523/stumped-by-the-code-nec-requirements-for-ambient-temperature-correction-to-conductor-ampacity says: 2020 NEC: under 7/8 in. needs a 60 F adder; XHHW-2 is not subject to it.
- Source: https://static.elliottelectric.com/StaticPages/ElectricalReferences/ElectricalTables/ambient-temperature-correction-factors.aspx says: 2020 table: 66-70 C row gives 0.33 at 75 C and 0.58 at 90 C.
- Check (right): 2023 threshold is 3/4 in. (was 7/8 in. in 2020). Two sources agree; the CE provider quotes 2023 text and says the change was to suit strut dimensions.
- Check (right): Adder is 33 C (60 F). All three pages agree.
- Check (right): 2023 paragraph is 310.15(B)(2). CE provider and the 2020-era EC&M article cite (B)(2); the older paragraph was (B)(3)(c).
- Check (right): 35 C + 33 C = 68 C gives a 0.58 factor (90 C column). 66-70 C row: 0.58 at 90 C, 0.33 at 75 C. 55 x 0.58 is about 32 A, below the 50 A breaker.
- Check (unclear): XHHW-2 is exempt from the adder. Stated in a 2020 NEC EC&M article; the 2023 CE page does not mention an exemption. Unconfirmed for 2023.
- Check (right): 0.5 in. is below the threshold, 1.0 in. compliant, limit 0.75 in. min, inches. Units are inches as in the file; values consistent.

## RP-DISC-AC-NONE-001 (disconnecting-means)

- Verdict: **confirmed**, reference `440.14`
- 2026: NFPA's Amendment 70-48 ballot (July 2025) concerns 440.14 for 2026; body text is identical in every version shown. A residential 'Exception No. 3' (non-fused, 250 V or less, 60 A or less: in sight and accessible) was in the second draft; the vote appears to revert to 2023 text, but I could not tell whether it survived in the published 2026 book. A scene with no disconnect at all is unaffected either way.
- Note: Safe to ship. Scene must show no disconnect anywhere on or near the condenser and no plug-and-receptacle arrangement.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P11_70-48_AmendBallotFinal.pdf says: NFPA ballot reproduces 440.14 (previous-edition text): in sight, readily accessible, on or within, 110.26(A) space, not on access panels or nameplate.
- Source: https://www.ecmweb.com/whats-wrong-here/whats-wrong-here-222 says: Quotes within-sight and readily-accessible location rule, the access-panel and nameplate restriction, and two industrial/plug exceptions.
- Source: https://forum.nachi.org/t/ac-and-refrigerating-equipment/7902 says: Poster quotes the within-sight, readily-accessible rule and the no-access-panel restriction.
- Check (right): Disconnect must be within sight of and readily accessible from the AC unit, and may be on or within it. Body text of 440.14 shown in NFPA's own ballot document as the 2023 text; also quoted by ECM and a forum.
- Check (right): newEntry: not on access panels or over the nameplate, plus working space. Both sentences appear in the NFPA document text and in the ECM page.
- Check (right): Exceptions (cord and plug, industrial lockable case) do not apply to a house condenser. 2023 has two exceptions: industrial with written procedures, and attachment plug/receptacle per 440.13. Neither fits a house.
- Check (right): A panel breaker out of sight cannot substitute. 440.14 has no lockable-breaker alternative outside the industrial exception, unlike 422.31(B) for appliances.

## RP-DISC-HANDLE-INVERTED-001 (disconnecting-means)

- Verdict: **corrected**, reference `404.7`
- 2026: no 2026 information found (IAEI 2026 summary does not mention 404.7)
- Note: Rule is sound but the pool pump is not in the scene; I suggest moving it to the AC disconnect (or shed switch). Reviewer should confirm the 2023 exceptions from the book.
- Source: https://www.ecmweb.com/national-electrical-code/violations/article/20896786/illustrated-code-catastrophes-sections-11026-24024-3122-and-art-404 says: Illustrates a disconnect installed upside down so up reads off; vertical handles must be installed with up as on.
- Source: https://forum.nachi.org/t/service-box-upside-down/184615 says: Up-is-on applies to vertically moving handles in enclosures; horizontal-moving breakers have no directional requirement.
- Check (right): 404.7 is the right section in the 2023 NEC. Title 'Indicating'; ECM and a forum thread both quote the up-is-on rule under 404.7.
- Check (right): Applies only to vertically operated handles. Rotary and side-to-side handles have no direction requirement, per forum discussion.
- Check (unclear): Exceptions exist. Search snippet (not opened) lists vertically operated double-throw switches and busway tap switches; neither fits an ordinary single-throw disconnect.
- Check (wrong): Scene contains a pool pump. The stated scene is a house, garage panel, outdoor AC, water heater and shed. No pool exists, so the pool pump disconnect cannot be placed.
- Check (right): Upside-down disconnect is something a student can see. ECM shows a real photo of an upside-down weatherproof disconnect with down = on.

## RP-DISC-SHED-NONE-001 (disconnecting-means)

- Verdict: **corrected**, reference `225.31`
- 2026: 225.31 is revised in 2026: an outside disconnect may be on or within sight of the building (ECM key-revisions article and a 2026 study guide, both opened). Number unchanged. 225.36 text for 2026 not directly seen.
- Note: The drafter's snap-switch doubt rests on outdated text; the fix is only to the newEntry wording. I did not open any 2023 or later page quoting 225.36 itself, so the reviewer should check it.
- Source: https://www.electrical-contractor.net/forums/ubbthreads.php/topics/93774.html says: 2005 thread: separate buildings treated like service entrances; up to six breakers in one enclosure; old residential snap-switch exception quoted.
- Source: https://captaincode2014.leviton.com/node/30 says: By the 2014 cycle 225.36 allows breaker, molded case, general-use or snap switch; the residential-only 3-way exception was removed and SUSE dropped.
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch07-feeders-services/outside-feeders-separate-buildings says: 2026 study guide: six disconnects max grouped; 2026 outside disconnect must be on or within sight of the building.
- Check (right): 225.31 requires a disconnect for a building supplied by a feeder. Forum and study guide both restate it; location is covered by 225.32 and the 2026 text of 225.31(B).
- Check (wrong): 225.36 allows a snap switch only for residential outbuildings. That residential-only exception is older text. Leviton's 2014 NEC analysis says the cycle replaced it with a general list including snap switch, for all occupancies. Not opened for 2023 text.
- Check (right): A shed panel with six or fewer breakers satisfies the rule. 225.33 allows up to six switches or breakers grouped; a forum thread states this. The scene must show no panel, switch or breaker at all.
- Check (right): Scene shows feeder entering shed with nothing at the entry point. This breaks 225.31 as written; it must also show no disconnect elsewhere in the shed, since 225.32 allows inside nearest the entry point.

## RP-DISC-WH-SIGHT-001 (disconnecting-means)

- Verdict: **confirmed**, reference `422.31(B)`
- 2026: no 2026 information found
- Note: Rule is solid. Scene needs a breaker with no lock-off clip or hasp, no line of sight, and no unit switch on the heater.
- Source: https://www.ecmweb.com/code-quiz-of-day/article/20899319/disconnect-requirements-for-permanently-connected-appliances says: Over 300 VA: breaker serves if within sight or lockable open, with locking provision permanently in place.
- Source: https://secure.in.gov/dhs/files/Section-422.31B-Lock-location.pdf says: Indiana interpretation quoting 422.31(B) (2009 code): lock provision must be on or at the breaker itself, not the panel door.
- Source: https://ecmweb.com/national-electrical-code/qa/article/21123327/code-qa-appliance-disconnecting-means says: 422.31(B) over 300 VA: within sight or lockable with provisions staying in place; notes a 2020 code change.
- Check (right): For permanently connected appliances over 300 VA a breaker can be the disconnect if in sight or lockable open. Stated in ECM quiz, ECM Q&A, a forum, and an Indiana state interpretation (2009 text with the same wording).
- Check (right): Lock provision must stay in place with or without a lock. Quoted in the Indiana interpretation; ECM Q&A calls the wording a 2020 change.
- Check (right): The (B) paragraph, not (A) or (C), is the right one for a water heater. (A) covers 300 VA or less; motor-driven appliances over 1/8 hp are a separate paragraph. A resistive water heater is over 300 VA.
- Check (unclear): In sight means visible and within 50 ft. Appears only in a search snippet I did not open; it is the Article 100 definition as I know it, so the scene should show no line of sight.
- Check (unclear): A unit switch under 422.34 is an alternative. Search snippet says it counts only where another disconnect is provided (one-family: the service disconnect). Not confirmed from an opened page; scene should show no unit switch.

## RP-DISC-SVC-SEVEN-001 (disconnecting-means)

- Verdict: **corrected**, reference `230.71(B)`
- 2026: 230.71 and 230.72 remain in the 2026 code under the same numbers (2026 study guide and a search summary); 230.85 was deleted and its content moved into 230.70. No renumbering of 230.71 found.
- Note: Reference should be 230.71(B). Place the seven mains on outdoor equipment so the scene is also valid under 2026 230.70.
- Source: https://www.mikeholt.com/files/PDF/20CC_230.71.pdf says: 2020 rewrite: (A) general, (B) two to six service disconnects per service; handle-tied single-pole sets no longer permitted.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/20904311/services-and-the-nec-part-2-of-2 says: No more than six service disconnects per service; reason is quick access for first responders.
- Source: https://expertce.com/learn-articles/service-disconnect-rules-nec-230-part-vi/ says: Describes (A) one disconnect and (B) allowing up to six, applying in 2020 and 2023.
- Check (right): Maximum of six disconnects per service. ECM, Mike Holt's 2020 changes and ExpertCE all give six per service; two services could have twelve.
- Check (wrong): Reference is plain 230.71. Since the 2020 edition, (A) is general (one disconnect, listed-equipment items excluded) and the two-to-six allowance and the six cap are in (B). 2023 keeps this layout per ExpertCE.
- Check (wrong): Sets of breakers with handle ties count as one. Holt says the 2020 cycle removed handle-tie single-pole sets as a single disconnect; seven separate handles is clearly over the limit.
- Check (unclear): Service equipment 'beside the meter' in the garage. 2026 230.70 puts the service disconnect for one- and two-family dwellings outdoors (IAEI, ECM); the 2023 rule required an outdoor emergency disconnect. An indoor garage board is risky for the scene.

## RP-DISC-SVC-RATING-001 (disconnecting-means)

- Verdict: **confirmed**, reference `230.79(C)`
- 2026: 230.79 keeps its number in 2026 per a 2026 study guide; 100 A minimum unchanged. Whether the 2026 text extends 100 A to two-family dwellings is unclear (sources differ).
- Note: Keep the house as single-family and new. Reviewer should confirm the (C) letter from the 2023 book.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/20904311/services-and-the-nec-part-2-of-2 says: One-family dwelling needs at least 100 A, 3-wire; other minimums 15 A, 30 A and 60 A.
- Source: https://open-exam-prep.com/study-guides/mn-electrician/ch07-feeders-services/service-equipment-disconnects says: 2026 guide: one-family minimum 100 A, 3-wire; others 60 A; single-circuit 15 A.
- Source: https://open-exam-prep.com/study-guides/tx-electrician/services-service-equipment/service-disconnecting-means says: Lists 100 A for one- and two-family dwellings and 60 A for all other installations.
- Check (right): One-family dwelling service disconnect must be at least 100 A, 3-wire. ECM and two open-exam-prep guides (one 2026-based) all state 100 A for one-family dwellings.
- Check (unclear): The paragraph letter is (C). Search snippets give (A) 15 A, (B) 30 A, (C) 100 A one-family, (D) 60 A others; I did not see the lettering on a page I opened.
- Check (right): A 60 A main is wrong for a new house. 60 A is the minimum for non-dwelling 'other installations'. The scene must read as new construction, not a legacy house.
- Check (right): Rule is about the equipment rating, not a breaker setting. 230.79 sets the disconnect's rating; label and handle should both show 60 A.

## RP-DISC-AC-PANEL-001 (disconnecting-means)

- Verdict: **confirmed**, reference `440.14`
- 2026: Same sentence is in the NFPA 2026 amendment ballot text; no change to it found. See DISC-AC-NONE-001 for the exception question.
- Note: Safe to ship. Make sure the nameplate is visibly partly hidden so the second limb is observable.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P11_70-48_AmendBallotFinal.pdf says: NFPA ballot text for 440.14 includes the no-access-panel, no-obscured-nameplate sentence in both versions shown.
- Source: https://www.ecmweb.com/whats-wrong-here/whats-wrong-here-222 says: Quotes the access-panel and nameplate restriction in 440.14.
- Source: https://forum.nachi.org/t/ac-and-refrigerating-equipment/7902 says: Disconnect cannot be mounted on panels designed to give access to the equipment.
- Check (right): Disconnect must not be on panels designed to give access to the equipment or obscure the nameplate. Present in the 2023-edition text reproduced in NFPA's ballot document and quoted by ECM.
- Check (unclear): Nameplate wording was added in 2023. The ECM page (older) already quotes the nameplate wording, so it may predate 2023. Not needed for the item.
- Check (right): Scene (disconnect over the compressor access cover, partly covering nameplate) is visible and breaks the rule. Both limbs of the sentence are broken by that mounting.

## RP-DISC-AC-WORKSPACE-001 (disconnecting-means)

- Verdict: **corrected**, reference `440.14 (depth from 110.26(A)(1))`
- 2026: 110.26(A) link in 440.14 is unchanged in NFPA's 2026 amendment ballot text; no change to the 36 in figure found.
- Note: The 36 in figure comes from Table 110.26(A)(1), not 440.14 itself; the 110.26(A) newEntry may duplicate the existing panel clearance entry (RP-CLEAR-FRONT-001), so check the main database. I did not open the table text itself, only guides and articles.
- Source: https://ecmweb.com/national-electrical-code/violations/article/21262652/illustrated-catastrophes-no-room-for-error says: 2023 revised 440.14: AC disconnects need the 110.26(A) space, 3 ft deep, 30 in wide, 6.5 ft high.
- Source: https://dailyreporter.com/2025/09/19/nec-2023-updates-working-space-for-electrical-equipment/ says: 440.14 reiterates that AC disconnects must comply with 110.26 working space; depth ranges 3 to 5 ft by voltage and condition.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P11_70-48_AmendBallotFinal.pdf says: 440.14 text includes 'shall meet the working space requirements of 110.26(A)'.
- Check (right): 2023 440.14 requires the disconnect to meet 110.26(A) working space. Shown in NFPA's ballot text, ECM and Daily Reporter; earlier editions had no such link.
- Check (right): 36 in minimum depth at 120/240 V. 0-150 V to ground gives 3 ft in all three conditions of Table 110.26(A)(1); 120/240 V is 120 V to ground.
- Check (right): The disconnect rule really uses the 110.26 depth. 440.14 refers to 110.26(A) as a whole, which includes depth (1), width (2) and height (3).
- Check (right): 20 in measured, 40 in compliant. 20 is below 36; 40 is above 36 with margin.
- Check (wrong): Description says fence and shrub bed but note shows only fence. Internal mismatch; shrub bed is not in the inspection note. I removed it.

## RP-KNOCKOUT-001 (equipment-installation)

- Verdict: **confirmed**, reference `110.12(A)`
- 2026: no 2026 information found (the Chapter 1 2026 EC&M summary I opened does not mention 110.12; a search summary claiming a wording change was not verifiable)
- Note: Both opened pages are older and may share a source; I could not open an NFPA 2023 page. Letter (A) is a long-stable subdivision, so risk is low.
- Source: https://forum.nachi.org/t/whats-wrong-here/128 says: Quotes 110.12(A): unused cable or raceway openings must be effectively closed to equal the equipment wall; 408.7 handles breaker spaces.
- Source: https://www.ecmweb.com/whats-wrong-here/whats-wrong-here-25 says: Photo quiz cites 110.12(A) for unused cable/raceway openings and 408.7 for open breaker spaces (older edition, about 2005).
- Check (right): 110.12(A) covers unused cable or raceway openings in enclosures. Two pages quote it as closing unused openings to protect about as well as the equipment wall.
- Check (right): Breaker-space openings are 408.7, not 110.12(A). The EC&M quiz cites both rules separately in one panelboard photo.
- Check (right): Scene (open punched-out knockout, no plug) breaks the rule as written. A knockout removed with nothing in it is an unused opening.

## RP-BLANKFILLER-001 (equipment-installation)

- Verdict: **confirmed**, reference `408.7`
- 2026: no 2026 information found
- Note: Opened sources are older-edition quotes, no 2023-specific page opened; rule wording has been stable for many editions.
- Source: https://www.ecmweb.com/whats-wrong-here/whats-wrong-here-25 says: Open unused breaker spaces in a panelboard violate 408.7; closures must match the enclosure wall's protection.
- Source: https://forum.nachi.org/t/whats-wrong-here/128 says: Quotes 408.7 requiring identified closures or other approved means for unused breaker/switch openings.
- Check (right): 408.7 requires unused breaker/switch openings closed with identified closures or approved means equal to the enclosure wall. Both opened pages give the same requirement.
- Check (right): Does not duplicate RP-BUS-EXCEED-001 (408.54). That one is too many breakers; this is an open space with nothing installed.
- Check (right): Scene visible to a student. An open slot in the dead front is plainly visible.

## RP-CABLECLAMP-001 (equipment-installation)

- Verdict: **corrected**, reference `312.5(C)`
- 2026: no 2026 information found (2026 Chapter 3 EC&M article returned 403; a search snippet says 312.8(A) became 312.11(A) in 2026, so Article 312 numbering did shift, but 312.5(C) specifically not confirmed)
- Note: Two sources agree on the rule, but neither is 2023 NEC text, so the literal 'listed fitting' phrase was softened. Reviewer should check actual 312.5(C) wording if possible; note 312.5 could move in 2026 as 312.4 to 312.7 did.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/20886926/stumped-by-the-code says: 2008 Q&A: cables secured with fittings designed for them; NM exception via top-entry raceway; sheath extends at least 1/4 in. into panel.
- Source: https://forum.nachi.org/t/help-with-one-more/182448 says: Forum quotes the exception text (12 in. fastening) and discusses NM cable being secured at the panel; ambiguity only about distance.
- Check (right): 312.5(C) is the cable-securing rule for cabinets/panel enclosures. Two sources discuss it for NM cable entering a panel.
- Check (unclear): necText says a 'fitting made and listed for the cable' is required. Only the 2008-edition EC&M Q&A uses 'designed and listed' wording; I could not see the 2023 text, so the safe claim is just 'secured to the enclosure'.
- Check (right): Raceway exception does not apply to a cable through a bare knockout. Exception covers only NM cable entering the top of a surface enclosure through a raceway (about 18 in. to 10 ft).
- Check (right): No numbers used in the item. Nothing to check.

## RP-WALLGAP-001 (equipment-installation)

- Verdict: **confirmed**, reference `312.4`
- 2026: Renumbered to 312.7 in the 2026 NEC (EC&M July 2026 Q&A and Texas 2026 code listing); 1/8 in. figure unchanged in those sources.
- Note: Use 312.4 for 2023 and note 312.7 for 2026. Consider setting limit to 3.2 mm or keeping 3 mm with inches as the authority; the scene needs the drywall cue to stay noncombustible.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/21262935/stumped-by-the-code-nec-requirements-related-to-cabinets-for-panelboards-in-walls says: 2023 NEC, 312.4: recessed panelboard cabinets in noncombustible surfaces (plaster, drywall, plasterboard) may have no gap over 1/8 in.
- Source: https://www.ecmweb.com/national-electrical-code/violations/article/21151441/illustrated-catastrophes-layers-of-violations says: 2020 NEC 312.4: no gaps over 1/8 in. at the cabinet edge; photo showed gaps near 3 in. around a panelboard.
- Source: https://up.codes/s/repairing-noncombustible-surfaces says: Texas code mirror shows 1/8 in. (3.2 mm) rule, numbered 312.4 in the 2023 and 2020 electrical codes and 312.7 in Texas 2026.
- Source: https://www.ecmweb.com/national-electrical-code/qa/article/55392151/code-qa-noncombustible-surfaces says: July 2026 Q&A uses 312.7 for the same 1/8 in. gap rule for noncombustible surfaces.
- Check (right): 312.4 is the 2023 number for repairing noncombustible surfaces. EC&M 2023-NEC Q&A and the Texas 2023 code list use 312.4; the July 2026 EC&M Q&A and Texas 2026 list use 312.7.
- Check (right): Limit is 1/8 in. at the edge of a flush-cover cabinet. Three pages give 1/8 in.; the IRC mirror gives 3.2 mm. One summary page saying 1/4 in. is unreliable and I ignored it.
- Check (unclear): Limit 3 mm in metric. 1/8 in. is 3.2 mm; the NEC metric rounding was not seen. Scene 8 mm and compliant 2 mm are on the correct side of either value.
- Check (right): Applies to noncombustible surfaces only. Rule text covers broken or incomplete noncombustible surfaces; combustible walls use the 312.3 flush/setback rule.
- Check (right): Drywall counts as noncombustible so the item is not wrong. EC&M 2023 article lists plaster, drywall and plasterboard as examples; the real violation photo involved gypsum board.
- Check (wrong): 312.7 is the 2023 number. 312.7 in 2023 is Space in Enclosures; the gap rule moves to 312.7 only in 2026.

## RP-PAINTBUS-001 (equipment-installation)

- Verdict: **confirmed**, reference `110.12(B)`
- 2026: no 2026 information found
- Note: The EC&M photo also showed white paint on conductors (200.7); keep scene to bus and terminals to avoid a second violation.
- Source: https://www.ecmweb.com/whats-wrong-here/article/20903415/whats-wrong-here-hint-a-white-washed-wall-panel says: Painted panel interior cited under 110.12(B); contaminated busbars must be repaired or replaced; paint on conductors also hits 200.7.
- Source: https://inspectorsjournal.com/topic/8569-paint-in-a-panel says: Poster quotes 110.12(B) listing paint among foreign materials; bus and ground bars behind breakers must be cleaned too.
- Check (right): 110.12(B): internal parts (busbars, terminals, insulators) not damaged or contaminated by paint etc.. Forum quotes the sentence and EC&M applies it to paint on busbars.
- Check (right): Distinct from RP-WORK-PANEL-001 (110.12 general). Different condition: contamination vs unsecured conductors.
- Check (right): Scene: paint on bus and terminals visible when cover is off. Matches the EC&M photo of painted panel interior.

## RP-NICKEDWIRE-001 (equipment-installation)

- Verdict: **confirmed**, reference `110.14(A)`
- 2026: no firm 2026 information found (a search summary says 2026 swaps 'suitable' for 'identified' in 110.14, not verified)
- Note: Applying the clause to nicked strands is an inspector's interpretation; the expert may want that noted in the hint. Cue must be visible cut copper, which is hard at stylus scale.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-are-2023-nec-terminal-connection-requirements-different/ says: 2023 110.14(A) now requires a mechanically secure electrical connection instead of 'thoroughly good'; friction-type connections addressed.
- Source: https://ecmweb.com/national-electrical-code/article/55306617/understanding-general-requirements-of-the-nec-part-9 says: Describes 110.14(A) terminals as connections mechanically secure without damaging the conductors.
- Source: https://www.electrical-contractor.net/forums/ubbthreads.php/posts/174651.html says: Older wording: terminal connections must be thoroughly good without damaging the conductors.
- Check (right): 110.14(A) requires connection without damaging the conductors. Pre-2023 forum quote and the 2023 EC&M article both include that clause.
- Check (right): 2023 changed 'thoroughly good' to 'mechanically secure'. Confirmed by ExpertCE and consistent with EC&M's 2023-era article.
- Check (right): Rule applies to cut strands from stripping. Not discussed by name, but nicking while stripping is damage to the conductor; fair reading, no source states it.
- Check (right): Not a duplicate of any existing item. Context has no nicked-conductor entry.

## RP-TERMTORQUE-001 (equipment-installation)

- Verdict: **corrected**, reference `110.14(A)`
- 2026: no firm 2026 information found for 110.14(D)
- Note: I refiled this under 110.14(A) because torque cannot be seen; if the expert prefers 110.14(D), the cue must be a visible label stating torque plus a loose screw, which still does not prove the method. Overlaps in reference with NICKEDWIRE-001 and DOUBLETAP-001, but the faults differ.
- Source: https://blog.hubbell.com/en/wiringdevice-kellems/screw-terminations-vs.-torque-requirements says: 110.14(D) from 2017; 2020 and later require approved means to reach manufacturer torque; values come from device instructions.
- Source: https://iaeimagazine.org/?p=14027 says: New in 2017: where manufacturer gives a torque value, a calibrated torque tool must be used.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-are-2023-nec-terminal-connection-requirements-different/ says: 2023 110.14(A) requires mechanically secure connections, supporting that filing for a loose terminal.
- Check (right): 110.14(D) requires reaching marked torque with an approved means (2020/2023). Hubbell says 2017 required a calibrated torque tool and 2020 changed it to approved means.
- Check (unclear): A visibly loose screw is fairly filed under 110.14(D). 110.14(D) governs how tightening is done, not the visible result; a loose connection is better filed under 110.14(A) mechanically secure.
- Check (wrong): 'Marked tightening value shows it was not torqued'. A student cannot see applied torque; remove that cue.
- Check (right): No torque numbers in the item. Real values come from the equipment label or instructions; none supplied here.

## SL-POOL-OVERHEAD-001 (special-locations)

- Verdict: **corrected**, reference `680.9(A)`
- 2026: No 2026 change to the 22.5 ft clearance found. Handout says 2026 had editorial reordering and splitting of Article 680 sections, so subsection letters could move.
- Note: The utility owns the drop (90.2(B)(5)), so the rule bars siting the pool under it; the scene should read as a pool placed under an existing drop. Could not open the Table 680.9(A) row text, so the cabled-with-messenger conductor type is unverified.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017 guide: overhead conductors must clear per Table 680.9(A); distance taken from maximum water level; 22.5 ft for 0-750 V.
- Source: https://expertce.com/courses/electricians-guide-for-nec-2023/lessons/how-are-2023-nec-pool-wiring-clearance-rules-different/ says: 2023 change to 680.9(A): applies to overhead conductors and open wiring not in a raceway.
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/1681920981.pdf says: 2023 textbook contents list 680.9 Overhead Conductor Clearance and no 680.8 clearance section.
- Source: https://cms9files.revize.com/westbendwi/Electrical%20Requirements%20for%20Pools.pdf says: 2017-based city handout: 22.5 ft vertical to water level, 10 ft horizontal from inside pool wall.
- Source: https://open-exam-prep.com/study-guides/id-electrician/special-equipment-emergency-renewables/swimming-pools-hot-tubs-fountains says: Study guide: 22.5 ft above maximum water level, 14.5 ft to diving platforms, 10 ft horizontal envelope.
- Check (right): 22.5 ft minimum for 0-750 V overhead conductors. Holt 2017 text, ECM 2020 article and several city handouts agree; 2023 table value not seen in primary text.
- Check (right): Measured to maximum water level. Holt 2017 states clearance is taken from the maximum water level; city handouts repeat it.
- Check (unclear): 10 ft horizontal beyond pool wall. Only secondary handouts (West Bend, open-exam-prep) state it; I could not open Table 680.9(A) or its figure. Safer to drop from necText.
- Check (wrong): Drafter doubt: 2023 broadened 680.9 to all overhead conductors. 2023 narrowed it: wording now limits it to conductors and open wiring not in a raceway (expertce).
- Check (right): Reference 680.9(A) in 2023. Holt 2023 table of contents lists 680.9 Overhead Conductor Clearance; expertce cites 680.9(A) for 2023.
- Check (right): Scene 15 ft vs 22.5 ft; compliant 24 ft. 15 is below and 24 is above the minimum.

## SL-POOL-RECEPT-001 (special-locations)

- Verdict: **confirmed**, reference `680.22(A)(3)`
- 2026: No 2026 change to the 6 ft distance found; 2026 handout notes only editorial reordering and a storable-pool receptacle change.
- Note: 2023 subsection numbering not read from primary text but three sources agree from 2017 to 2023-era. Worth noting in the hint that GFCI also applies within 20 ft.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017: (A)(3) other receptacles not less than 6 ft from inside walls; (A)(1) required receptacle 6 to 20 ft.
- Source: https://www.nnva.gov/DocumentCenter/View/6693/Residential-Swimming-Pools-Electrical-Wiring-Requirements says: 2020 city overview cites 680.22(A)(3): other receptacles at least 6 ft from pool and outdoor spa walls.
- Source: https://amporalabs.com/blog/swimming-pool-electrical-nec-680 says: 2023-era guide: 6 ft minimum from pool wall; one required receptacle between 6 and 20 ft.
- Check (right): 6 ft minimum for other receptacles. Holt 2017 and Newport News 2020 summary both give 680.22(A)(3) at 6 ft.
- Check (right): 20 ft outer limit. Applies to the one required 125 V 15/20 A receptacle (680.22(A)(1): 6 to 20 ft) and to the GFCI zone (A)(4), not to this rule.
- Check (right): Pump and circulation receptacles also need 6 ft. Those are covered by 680.22(A)(2), same 6 ft plus GFCI; the scene's general-purpose receptacle is correctly (A)(3).
- Check (right): Scene 3 ft, compliant 8 ft. Below and above 6 ft.

## SL-SPA-DISC-001 (special-locations)

- Verdict: **confirmed**, reference `680.13`
- 2026: No 2026 information found.
- Note: 2023 title appears to be Equipment Disconnecting Means (one source), so the new entry title may change. Existing entry 680.12 Maintenance Disconnecting Means is 2014 numbering; the 2023 number is 680.13.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017 text: disconnect readily accessible, within sight, at least 5 ft from spa or hot tub equipment unless barrier.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21237255/keeping-pools-and-spas-safe says: 2020 article: maintenance disconnect required, readily accessible, within sight, 5 ft from water, measured along shortest path.
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/1681920981.pdf says: 2023 textbook contents list 680.13 (titled Equipment Disconnecting Means) and 680.12 Equipment Rooms.
- Source: https://www.electricallicenserenewal.com/Electrical-Continuing-Education-Courses/NEC-Content.php?sectionID=1538 says: 2023 page confirms 680.12 is now Equipment Rooms, Vaults and Pits.
- Source: https://expertce.com/learn-articles/how-to-wire-a-hot-tub/ says: Hot tub guide cites 680.13: disconnect at least 5 ft horizontally from the tub wall.
- Check (right): 680.13 in 2023, 680.12 in 2014. 680.13 in 2017 (Holt) and 2020 (ECM). 2023 contents list 680.13 and a 2023 page shows 680.12 is equipment rooms.
- Check (right): 5 ft minimum, in sight, barrier exception. Holt 2017 text and ECM 2020 article agree; within sight means 50 ft or less.
- Check (unclear): Applies to a self-contained spa. 680.13 covers outdoor spa and hot tub equipment other than lighting, but no source addresses listed self-contained units. Keep the scene as separate heater and pump equipment.
- Check (right): Scene 2 ft, compliant 6 ft. Also breaks the 680.22(C) 5 ft switching device rule.

## SL-POOL-PUMPGFCI-001 (special-locations)

- Verdict: **confirmed**, reference `680.21(C)`
- 2026: TIA 70-26-1 (effective April 30, 2025) amends 680.21(C) for variable-speed drive motors and applies to the 2023 and 2026 editions; 2026 also raises the three-phase GFCI limit from 60 A to 100 A. Neither changes this scene.
- Note: Make the GFCI absence unambiguous in the scene (plain receptacle, no protective device anywhere on the circuit). A 240 V pump is more typical but 120 V is covered.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017: GFCI required for outlets supplying pool pump motors on single-phase 120-240 V, by receptacle or direct connection.
- Source: https://www.nnva.gov/DocumentCenter/View/6693/Residential-Swimming-Pools-Electrical-Wiring-Requirements says: 2020 summary: all pool motor outlets at 150 V or less to ground, 60 A or less, need Class A GFCI.
- Source: https://www.thepoolspashow.com/nespa2026/Custom/Handout/Speaker0_Session1551_1.pdf says: 2023 and 2026: TIA 70-26-1 amends 680.21(C) so GFCI is not placed between a variable-speed drive and motor.
- Source: https://iaeimagazine.org/electrical-fundamentals/nec-article-680-diving-into-2023-nec-proposed-changes/ says: 2023 changes: 680.5 rewritten with Class A GFCI at 150 V or less; 680.21(D) widened for repaired pumps.
- Check (right): 680.21(C) covers all single-phase 120-240 V pump motors. Holt 2017 and a 2020 city summary agree; a 2023-era guide says regardless of horsepower.
- Check (right): 60 A or less, Class A. Newport News 2020 summary: pool motor outlets at 150 V or less to ground and 60 A or less need Class A; 2023 routes this through 680.5(B).
- Check (right): Outdoor 120 V 20 A receptacle fits the GFCI scope list. Outdoor dwelling receptacles also need GFCI under 210.8(A)(3) (Holt 2017 comment), so the app's outdoors scope is satisfied either way.
- Check (unclear): No GFCI visible on receptacle. A GFCI breaker could protect it unseen; the scene's no labelled breaker is a weak cue.

## SL-EV-DISC-001 (special-locations)

- Verdict: **corrected**, reference `625.43`
- 2026: IAEI article says 2026 restructures 625.43 into (A) to (D), with a lockable disconnect for permanently connected equipment, plug as disconnect up to 60 A, and emergency shutoff exempting one- and two-family homes. 2026 thresholds need an expert check.
- Note: Reference and numbers are right for 2023; fix the scene so it breaks the rule (no plaque, breaker not lockable). 2026 may differ, so treat this item as 2023 only.
- Source: https://www.ecmweb.com/national-electrical-code/code-basics/article/21276414/nec-requirements-for-ev-equipment says: Nov 2023: equipment over 60 A or 150 V to ground needs readily accessible disconnect lockable open.
- Source: https://ecmweb.com/national-electrical-code/article/21278706/tia-proposed-for-2023-nec-regarding-disconnecting-means-for-evse-and-wpte says: TIA article: threshold over 60 A or 150 V to ground, plaque if remote, lockable per 110.25.
- Source: https://captaincode2023.leviton.com/node/357 says: 2023 Leviton page repeats the threshold, plaque for remote disconnect, and lockable-open requirement.
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/1681920981.pdf says: 2023 textbook contents list 625.43 Disconnecting Means.
- Check (right): Threshold over 60 A or over 150 V to ground. ECM 2023 article, the TIA article and Leviton 2023 page agree; 80 A triggers it though 240 V split-phase is 120 V to ground.
- Check (right): Readily accessible and lockable open. Lockable per 110.25, lock provision stays in place.
- Check (right): Remote location allowed with plaque. Plaque at the equipment shows disconnect location (Leviton 2023, TIA article).
- Check (unclear): Scene breaks the rule as written. A remote lockable breaker with a plaque would comply. The scene must show both no plaque and no lock provision, which the draft only partly says.
- Check (unclear): New DC emergency shutoff exempts one- and two-family dwellings. Seen only in a proposed TIA article; not relevant to this scene.

## SL-POOL-SWITCH-001 (special-locations)

- Verdict: **confirmed**, reference `680.22(C)`
- 2026: No change to the 5 ft rule found; 2026 adds a new 680.22(D) on portable signs and editorial reordering.
- Note: Letter (C) not read from 2023 text but the 2017 text and 2023 section title agree. The same device could also be argued under 680.13 if it is a disconnect.
- Source: https://pacodealliance.com/wp-content/uploads/2023/12/Article-680-Swimming-Pools-by-Mike-Holt.pdf says: 2017: breakers, time clocks, light switches at least 5 ft horizontally from pool wall unless barrier or listed for closer.
- Source: https://amporalabs.com/blog/swimming-pool-electrical-nec-680 says: 2023-era guide: switches at least 5 ft horizontally from pool walls, cites 680.22(C).
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/1681920981.pdf says: 2023 textbook contents title 680.22 Receptacles, Luminaires, and Switches, so switches stay in 680.22.
- Check (right): 5 ft horizontally from inside pool wall. Holt 2017 text and a 2023-era guide agree.
- Check (right): Exceptions: solid fence, wall or permanent barrier, or device listed for use within 5 ft. Both exceptions appear in the 2017 text; scene must show neither.
- Check (right): Covers breakers, time clocks, light switches. Holt 2017 lists circuit breakers, time clocks and pool light switches.
- Check (right): Scene 2 ft, compliant 6 ft. Below and above 5 ft.

## Held out

- conductor-sizing WIRE-MINSIZE-001: verification: single-source. Fine for 2023, but if the app targets 2026 the 16 AWG scene may no longer be a clear violation for a lighting circuit. Reviewer should check the 2026 text of the minimum-size rule.
- conductor-sizing AMBIENT-BOILER-001: verification: single-source. Near-duplicate of ROOFTOP-ADDER-001: both are 310.15(B) ambient correction on 8 AWG THHN on a 50 A breaker using 90 C columns and a 0.58-0.76 factor. Keep only one unless the app wants both; the rooftop item has a measurable limit.
- disconnecting-means DISC-SVC-GROUP-001: verification: single-source. Grouping is confirmed by three sources; only the marking half is single-source, so build the scene around the separation. Reviewer should check the 2023 wording of 230.72(A) for the marking sentence.
- equipment-installation MOUNTING-001: verification: single-source. Only one NEC-specific opened page (2014 edition). The huggingface dataset page that quotes 110.13(A) is of unknown reliability, so I did not count it.
- equipment-installation DOUBLETAP-001: verification: single-source. Not a duplicate of the lug item (different fault: two wires vs wrong metal). The scene must show a visible breaker label without a two-conductor mark.
- special-locations POOL-PUMPCORD-001: verification: unverified (keep=false). The 3 ft rule is solid but its 2023 subsection is not; with the 2023 text in hand this becomes safe to ship. Existing entry 680.7 Cord-and-Plug is wrong for 2017 onward (680.7 is grounding and bonding terminals in 2017/2020, grounding and bonding in 2023); cord rule was 680.8 in 2017 and 2020, 2023 unconfirmed.
- special-locations EV-COUPLER-001: verification: single-source. Only trade articles were readable, no code text. Number and reference look right, but confirm the 2023 wording and that a listed-and-marked exception is not visible on the wall charger.
- special-locations GEN-INLET-001: verification: single-source. Concept and 702.5 reference look right for 2023, but no code text was opened and the portable-generator exception subsection is unresolved; expert should check 702.5 versus 702.6. Describe the scene as no listed interlock or transfer equipment rather than relying on the two-pole breaker.
