# Electrician Certification Bodies: US and Canada

Research notes for mapping the app's three levels (Beginner/CTE, Standard/Apprentice, Expert/Licensed) to real credentials.
Items marked *(unverified)* come from background knowledge, not a source checked during research. Verify before using in learner-facing content.

## Key finding

Neither country has one national electrician license. Licensing is state-level (US) or provincial/territorial (Canada). Canada layers a national exam endorsement (Red Seal) on top.

## United States

### Licensing authorities (the legal right to work)
- State electrical licensing boards and local jurisdictions (city/county). Rules, exams and reciprocity differ by state.
- Typical tiers: **apprentice -> journeyman -> master**. Masters can design systems, pull permits, supervise and run a contracting business.
- NEC edition varies by state and even by city. As of late 2025 about 20 states had adopted NEC 2023; the rest are on 2020 or 2017. NEC 2026 adoption has barely begun. Track via [IAEI code adoption](https://www.iaei.org/page/nec-code-adoption).

### Training and credential bodies
| Body | Role | Maps to app level |
|------|------|-------------------|
| NCCER | 4-level Electrical curriculum (L1-L4); written exam plus performance assessment; common in high-school CTE | Beginner (L1-2), Standard (L3-4) |
| IBEW / Electrical Training Alliance (ex-NJATC) | Union apprenticeship, ~10,000 OJT hours | Standard |
| ABC, IEC | Non-union apprenticeships, ~8,000 OJT hours | Standard |
| State journeyman / master exams | Legal licensure, often via Prometric or PSI *(unverified)* | Standard -> Expert |
| IAEI (CEI-R, CEI-M), ICC/IAEI joint certification (released Feb 2023) | Electrical inspector certification | Expert |
| NICET *(unverified)* | Fire alarm / low-voltage specialist levels | Out of scope for now |

NCCER level content (useful for scenario design):
- **L1:** safety, theory, intro to circuits and NEC, boxes, conduit bending, conductors, drawings, residential services, test equipment.
- **L2:** AC, motors, lighting, conduit bending, pull/junction boxes, grounding and bonding, breakers and fuses, controls.
- **L3:** branch circuit loads, conductor selection, hazardous locations, overcurrent protection, transformers.
- **L4:** feeder/service load calcs, health care facilities, standby/emergency systems, fire alarm, advanced controls.

## Canada

### National standard
- **Red Seal Program:** Construction Electrician (309A). Sponsored by ESDC, with the standard set by the Canadian Council of Directors of Apprenticeship (CCDA). Passing the interprovincial exam adds the Red Seal endorsement to a provincial trade certificate. See the [Red Seal occupational standard](https://www.red-seal.ca/_conf/assets/custom/docms/const-elect/rsos.pdf).
- Related trades: Industrial Electrician (442A), Domestic and Rural Electrician (309C, Ontario).

### Provincial and territorial authorities
- Ontario: Skilled Trades Ontario (replaced the Ontario College of Trades in 2022).
- British Columbia: Skilled Trades BC.
- Alberta: Apprenticeship and Industry Training *(unverified)*.
- Quebec: CCQ *(unverified)*.
- Other provinces and territories run their own apprenticeship agencies.

### Training structure
- Typical 309A school training is four levels: 270 / 270 / 270 / 240 hours (1,050 hours total). Ontario's new curriculum standard started phasing in on 2024-09-01, with Levels 1-3 as common core for all electrical trades and Level 4 specific to 309A ([Skilled Trades Ontario curriculum guide](https://www.skilledtradesontario.ca/wp-content/uploads/1970/01/Supplemental-Resource-Guide-Electrical-Trades-309A-309C-442A-Curriculum-L1234-Dec-22-2023-EN.pdf)).
- On-the-job hours are about 9,000 *(unverified)*, in addition to school time.
- Exam prep materials reference the Canadian Electrical Code (CEC, CSA C22.1) 2021 and 2024 editions.

### Code difference
Canada uses the **CEC**, not the NEC. A Canadian track would need CEC rule content (CSA copyright applies) and enforcement context from provincial safety authorities.

## Suggested level mapping

| App level | US anchor | Canada anchor |
|-----------|-----------|---------------|
| Beginner (CTE) | NCCER Electrical L1-L2 | Apprenticeship Level 1-2 |
| Standard (Apprentice) | NCCER L3-L4 / journeyman exam prep | Level 3-4 / Red Seal 309A exam prep |
| Expert (Licensed) | Master exam, IAEI/ICC inspector certs, NEC 2026 changes | Red Seal holders / inspector roles |

## Open questions
1. Should content be NEC-only with a Canadian CEC mode later, or both from the start?
2. Which states or provinces matter most for the first release? Adoption lag means one NEC edition will not fit every state.
3. Do we align to a credential's published exam blueprint (for example NCCER) or stay general? Check licensing terms before using proprietary exam content.
