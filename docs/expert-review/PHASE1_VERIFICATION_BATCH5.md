# Phase 1 verification notes, batch 5

Seven NEC violations were drafted from public sources by one agent and then checked by a second agent that opened its own sources. **No official NEC text could be opened** (the NFPA viewer needs a login); NFPA's public first-draft working documents were used where they helped. Every item still needs a credential expert, and every 2026 statement is unconfirmed against the final text.

## BC-AFCI-HOMERUN-001 (arc-fault-protection, Master)

- Verdict: **corrected**, 2023 reference `210.12(A)(5) (metal raceway/MC/AC home run + outlet AFCI at first outlet)`, 2026 `unconfirmed; NFPA CMP-2 first draft inserts a new item so the metal home-run option becomes (A)(6) and concrete-encased becomes (A)(7). Final 2026 text not read.`
- Changes applied: (1) nec-2026 citation: do not claim (A)(5). Cite 210.12(A) without a sub-item for 2026, or use (A)(6) only if the final text is confirmed. (2) Soften 'only when' in the violation and article text: 2023 (A)(4) also lets an outlet AFCI at the first outlet pair with a plain breaker if the wire is continuous, 50 ft max (14 AWG) / 70 ft max (12 AWG), the first box is marked, and the pair is listed as a system combination-type. Suggested wording: 'A plain breaker with an arc-fault device at the first outlet is accepted on plastic-sheathed cable only under narrow conditions (marked first outlet, length limit, a pair listed as a system); the usual route is a metal home run (metal conduit, Type MC or Type AC, with metal boxes).' (3) inspectionNote: drop 'which is why this setup needs a metal wiring method'; say the first stretch is plain cable in plastic boxes and the breaker and receptacle are not a listed pair. (4) Scene: show no system-listing label or first-outlet marking, so the (A)(4) route is not open.
- Note: Rule is real and (A)(5) is right for 2023. Scene is subtle and somewhat arguable because (A)(4) exists on paper; IAEI calls (A)(4) unrealistic because no 'system combination' definition or listings exist. Master is fair.
- Source: https://iaeimagazine.org/issue/winter-2024/its-time-to-allow-more-ways-for-afcis-to-protect-us/ says: Lists six 2023 methods; (A)(5) is standard breaker plus AFCI outlet with Type MC/AC and metal boxes; (A)(4) is the system combination-type route, (A)(6) is concrete.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P02_FD_PrelimFR.pdf says: I read this PDF text. Draft 210.12(A) has seven items; new (5) (outlet AFCI with OCPD at service equipment, 50/70 ft) is inserted, metal raceway/MC/AC with metal boxes shown as (6), concrete (7). Draft only.
- Source: WebSearch result quoting 2023 text (Mike Holt forum / EC&M summaries) says: 2023 210.12(A)(5) wording: metal raceway, wireways, auxiliary gutters, Type MC or AC meeting 250.118 with metal boxes, conduit bodies and enclosures between the OCPD and first outlet, then listed outlet branch-circuit AFCI at first outlet.

## BC-AFCI-LIGHTING-001 (arc-fault-protection, Master)

- Verdict: **confirmed**, 2023 reference `210.12(B) (dwelling units; branch circuits supplying 'outlets or devices')`, 2026 `unconfirmed final text; 210.12(B) number appears unchanged (2026 change summaries mention only (D) and (E)).`
- Changes applied: none
- Note: Mike Holt 2023 changes book (read) shows 210.12(B) Dwelling Units: AFCI for 15 and 20 A, 120 V circuits supplying outlets or devices in bedrooms etc. Lighting and fan outlets count (EC&M, St. Paul DSI). Scene must be clearly lighting-only. Rule is basic once known; Journeyman would also be fair, Master/subtle is acceptable. Check scope.json handling before shipping.
- Source: https://www.mikeholt.com/files/PDF/23_CC_210.12.pdf says: 2023 210.12(B) lists bedrooms etc. for circuits supplying outlets or devices.
- Source: https://www.stpaul.gov/sites/default/files/2023-06/DSI.Bldg_Electrical_AFCI%20Protection.pdf says: City guide (search summary): outlets include receptacles, lighting outlets, fans, smoke alarms; bedroom lighting circuits need AFCI.
- Source: https://www.ecmweb.com/national-electrical-code/article/20894244/code-qa says: EC&M Q&A: lighting outlets are outlets for AFCI purposes.

## RP-WIRE-10AWG-40A-001 (overcurrent-protection, Apprentice)

- Verdict: **confirmed**, 2023 reference `240.4(D) (study guide numbers 10 AWG copper as (D)(7))`, 2026 `unconfirmed; 240.4(D) was revised for 2026 per a continuing-ed summary but the 14/12/10 AWG limits appear unchanged. Final not read.`
- Changes applied: none
- Note: 10 AWG copper max 30 A holds in both opened sources. 240.4(B) next-size-up cannot rescue it because (D) overrides for small conductors. Values well separated (40 vs 30 A, 33 percent over). Apprentice fair. Overlap with existing items is topical only.
- Source: https://open-exam-prep.com/study-guides/mi-electrician/overcurrent-protection-tap-rules/small-conductor-rules says: (D)(7) 10 AWG copper 30 A; 25 A for 10 AWG aluminum/CCA; 240.4(G) table exceptions are for motors, HVAC, welders etc.
- Source: WebSearch summary of 2023 240.4(D) says: 15 A for 14 AWG, 20 A for 12 AWG, 30 A for 10 AWG copper; 15 A for 12 and 25 A for 10 AWG aluminum/CCA, after correction factors.

## RP-WIRE-10AWG-AL-30A-001 (overcurrent-protection, Journeyman)

- Verdict: **confirmed**, 2023 reference `240.4(D) (10 AWG aluminum/CCA 25 A; study guide numbers it (D)(6), repo uses (D)(5) for aluminum, so keep plain 240.4(D))`, 2026 `unconfirmed (same caveat as slot 3)`
- Changes applied: none
- Note: 25 A limit vs 30 A breaker is 20 percent over, clear. Scene needs a readable AL marking on the jacket. 25 A is a standard rating so a compliant variant exists. Water heater is not a Table 240.4(G) exemption. Journeyman fair. Overlaps COM-ALUM-OCPD-001 in rule only.
- Source: https://open-exam-prep.com/study-guides/mi-electrician/overcurrent-protection-tap-rules/small-conductor-rules says: (D)(6) 10 AWG aluminum/CCA 25 A, (D)(4) 12 AWG aluminum 15 A.
- Source: WebSearch summary of 2023 240.4(D) says: Same limits as in slot 3.

## RP-WORKLIGHT-001 (working-space-access, Master)

- Verdict: **confirmed**, 2023 reference `110.26(D)`, 2026 `110.26(D) number kept in NFPA CMP-1 first draft (FR-9225); wording changes to 'Control of all luminaire(s) by automatic means shall not be permitted to control all illumination within the working space' and 'enclosed panelboards'. Final unconfirmed.`
- Changes applied: none
- Note: Rule real; a lone motion-sensor luminaire with no override violates both the current and the proposed wording. The exception (adjacent light source or switched receptacle per 210.70(A)(1) Ex. 1) can excuse real rooms, so the scene must clearly show no other light. Article text is accurate. Minor severity and Master level are defensible; Journeyman also fair. One search snippet hinted at a dwelling-unit carve-out; I could not confirm it in any source, so low risk but keep the scene in a garage or utility room.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P01_FD_PrelimFR.pdf says: I read this PDF text. First Revision 9225 to 110.26(D): illumination for working spaces; adjacent light source or 210.70(A)(1) Ex.1 sentence kept; automatic control sentence reworded.
- Source: https://www.ecmweb.com/code-quiz-of-day/article/20899187/illumination-in-working-spaces says: Illumination not controlled by automatic means only; adjacent light exception.
- Source: https://open-exam-prep.com/study-guides/ak-electrician/ch03/ch03-sec03 says: 110.26(D) manual control must be available.

## SL-POOL-PUMPCORD-001 (special-locations, Apprentice)

- Verdict: **confirmed**, 2023 reference `680.8(A) (cord length, 900 mm / 3 ft, other than storable pools)`, 2026 `680.8(A) in NFPA CMP-17 first draft (FR-9049 retitles (A) Cord Length, keeps 900 mm). Final unconfirmed.`
- Changes applied: Do not add a second entry on top of the repo's 680.7 entry: renumber the existing entry to 680.8 (see entry680_7), or keep the new 680.8(A) entry and fix the old one. Limit and values are fine (2.5 ft is 17 percent under 3 ft; 8 ft is far over).
- Note: Rule real. 680.8 is cord-and-plug equipment in 2020 and 2023 per IAEI outline and NFPA draft. Scene must be a permanently installed pool (storable pools have no length limit). Apprentice fair. Article text is accurate; could add 'other than underwater luminaires'.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P17_FD_PrelimFR.pdf says: I read this PDF text. FR-9049: 680.8 Cord-and-Plug-Connected Equipment, (A) cord length not over 900 mm (3 ft) other than storable pools; committee says existing 680.7(B) holds grounding and bonding for cord connections.
- Source: https://secure.utah.gov/ce-public/files/objectives/10647.pdf says: IAEI pool course outline for the 2020 NEC: 680.7 Bonding Terminals, 680.8 Cord-and-Plug Connected Equipment, 680.9 Overhead Conductor Clearance.
- Source: https://www.mikeholt.com/instructor2/img/product/pdf/1685974939.pdf says: Understanding the 2023 NEC vol. 2: 680.7 Grounding and Bonding with (B) Cord-and-Plug Connections (grounding conductor, 12 AWG), and 680.9 Overhead Conductor Clearance.

## SL-POOL-JBOX-001 (special-locations, Master)

- Verdict: **corrected**, 2023 reference `680.24(A)(2)`, 2026 `unconfirmed; CMP-17 first draft retitles 680.24 (uses GFCI) and edits (B), (D), (E), (F) only; no (A) change seen. Final unconfirmed.`
- Changes applied: Qualify the rule text: it applies to a junction box connected to conduit running to a forming shell or no-niche mounting bracket, for a luminaire operating above the low-voltage contact limit. Suggested wording: 'For a pool light above the low-voltage contact limit, the junction box must be at least 4 in above the deck or 8 in above the highest water level, whichever is higher, and at least 4 ft from the pool wall unless a barrier separates it.' In the scene, place the box well over 4 ft from the pool wall so only the height is wrong, and do not mark it low-voltage/potted. Sub-item lettering (A)(2) rests on two sources, so keep it but flag for an official-text check.
- Note: Limits 4 in, 8 in and 4 ft agree across sources. 1 in is 75 percent under 4 in; compliant 6 in is 50 percent over 4 in (and must also be 8 in above water). Master is fair for an obscure number.
- Source: https://open-exam-prep.com/study-guides/mi-electrician/special-occupancies-equipment-conditions/swimming-pools-bonding says: 680.24(A)(2): at least 4 in above ground/deck or 8 in above max water level, whichever is higher, and 4 ft from the inside wall.
- Source: https://www.ecmweb.com/code-quiz-day/pool-luminaire-requirements says: EC&M (search summary): same 4 in / 8 in / 4 ft rule for luminaires over the low-voltage contact limit; low-voltage potted boxes may be flush if 4 ft from the wall.
- Source: https://docinfofiles.nfpa.org/files/AboutTheCodes/70/70_A2025_NEC_P17_FD_PrelimFR.pdf says: I read this PDF text. FR-9101 retitles 680.24; no change to (A) text seen.

## Entry renumbered: 680.7 to 680.8

CONFIRMED mis-numbered. The repo entry '680.7 Pool Equipment - Cord-and-Plug Connections' uses 2017 numbering. For the 2020 and 2023 NEC the cord-and-plug section is 680.8 (Cord-and-Plug-Connected Equipment): (A) cord length, 3 ft max for other than storable pools; (B) 12 AWG copper equipment grounding conductor with grounding-type plug; (C) construction. Evidence: NFPA CMP-17 first-draft text (2023-based) shows 680.8 with (A) 900 mm (3 ft); IAEI course outline for 2020 lists 680.7 Bonding Terminals, 680.8 Cord-and-Plug, 680.9 Overhead Conductor Clearance (the repo already uses 680.9(A) for overhead clearance, which fits 2020/2023 numbering, not 2017). Section 680.7 in 2023 is the grounding and bonding section (Mike Holt 2023: 680.7 Grounding and Bonding, (A) insulated EGC, (B) cord-and-plug grounding conductor, (C) terminals; the NFPA draft refers to 'existing 680.7(B)'); its exact 2023 title differs between sources (Bonding Terminals vs Grounding and Bonding). Fix: renumber the entry to 680.8 in both nec-2023 and nec-2026 articles.json (line about 1771), keep the 3 ft and 12 AWG text, and use 680.8(A) for the 3 ft limit. The 2026 number for 680.8 is unconfirmed beyond the first draft keeping it.
