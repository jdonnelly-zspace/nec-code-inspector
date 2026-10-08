"""Builds the expert review worksheets in docs/expert-review/ from the app's content files.

Run from the repository root:  python tools/make_expert_review.py
The worksheets hold our paraphrased texts and the references we cite, with blank columns for the
reviewer. They contain no code-book wording.
"""
import csv
import glob
import json
import os
import re

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
OUT = os.path.join(ROOT, "docs", "expert-review")
CODES = os.path.join(ROOT, "Assets", "_Project", "StreamingAssets", "Codes")
SCENARIOS = os.path.join(ROOT, "Assets", "_Project", "Content", "Scenarios")
CONTENT = os.path.join(ROOT, "Assets", "_Project", "Content")

# Known concerns from docs/CONTENT_REVIEW.md, keyed by (violationId, profileId)
VIOLATION_FLAGS = {
    # Corrected in the pre-review (docs/expert-review/PRE_REVIEW.md); the expert confirms against the 2026 book
    ("BC-DEDICATED-BATH-001", "nec"): "Corrected in pre-review: was 210.11(C)(1); confirm 2026 numbering",
    ("BC-DEDICATED-KITCHEN-001", "nec"): "Corrected in pre-review: was 210.11(C)(3); confirm 2026 numbering",
    ("COM-DISC-SIGHT-001", "nec"): "Corrected in pre-review: was 430.110; confirm (A) or (B) of 430.102",
    ("COM-MOTOR-CTRL-001", "nec"): "Corrected in pre-review: was 430.102(B); confirm whether 430.109 should also be cited",
    ("COM-RECPT-LOAD-001", "nec"): "Corrected in pre-review: was 220.44; the rule is 220.14(I), cited at article level",
    ("BC-GFCI-KITCHEN-001", "nec"): "Corrected in pre-review: was 210.8(A)(5); text widened to all kitchen receptacles (2023 wording); confirm 2026",
    ("COM-MOTOR-OL-001", "nec"): "Corrected in pre-review: 125% applies to service factor 1.15 or more; scene now uses a service factor 1.0 motor",
    ("RP-BUS-EXCEED-001", "nec"): "Corrected in pre-review: was 408.36; the number-of-devices rule is 408.54",
    ("BC-GFCI-DISHWASHER-001", "nec"): "Corrected in pre-review: dishwasher GFCI is not new in 2026 (it dates to 2014); confirm",
    ("GND-ELECTRODE-001", "nec"): "Text now notes 1/2 in is allowed for listed stainless or nonferrous rods; confirm",
    ("BC-WIRE-KITCHEN-001", "nec"): "Text now includes the sealed-opening condition (medium confidence); confirm",
    ("COM-FEEDER-TAP-001", "nec"): "Text now lists all four tap conditions (medium confidence); confirm",
    # Doubts left for the expert
    ("COM-MULTI-MOTOR-001", "nec"): "Doubt: title is about overcurrent protection but 430.24 covers conductors; protection may be 430.53 or 430.62",
    ("GND-GEC-001", "nec"): "Doubt: confirm the sub-item of 250.24(A) for connecting the grounding electrode conductor",
    ("GND-SUPPLEMENT-001", "nec"): "Doubt: confirm the single-rod supplement rule and its exception in the 2026 edition",
}
CEC_NOTE = "Draft from public summaries: check rule number, sub-item and edition"
UK_NOTE = "Draft from public summaries: check regulation number, amendment and edition (18th edition, amendments to 2026 unchecked)"
# BS 7671 entries where public sources disagree or the amendment history is unclear
UK_ARTICLE_FLAGS = {
    "421.1.7": "Doubt: sources disagree on whether AFDDs are required or recommended, and for which buildings",
    "514.12": "Doubt: confirm the number of the RCD test notice (514.12.2) and its wording",
    "544.1": "Doubt: confirm the main bonding conductor size rule and the 6 mm2 / 10 mm2 figures",
    "462": "Doubt: confirm Section 462 versus Chapter 46 numbering for the main switch and isolation",
    "722": "Doubt: Amendment 4 reportedly changes the residual direct current wording",
}

# NEC articles restored at the owner's request for the wiring-methods and special-locations skills
RESTORED = "Restored for the wiring-methods or special-locations skill; confirm number, title and text"
ARTICLE_FLAGS = {
    "480.3": RESTORED + " (Article 480 numbering is medium-low confidence)",
    "480.4": RESTORED + " (Article 480 numbering is medium-low confidence; text is generic)",
    "480.7": RESTORED + " (Article 480 numbering is medium-low confidence; the 60 V threshold is unconfirmed)",
    "480.10": RESTORED + " (Article 480 numbering is medium-low confidence)",
    "680.7": RESTORED + " (cord-and-plug equipment; medium-high)",
    "680.12": RESTORED,
    "680.22(B)": RESTORED + " (was 680.22(A)(1); medium-high)",
    "680.26": RESTORED,
    "680.44": RESTORED + " (spas and hot tubs; text is generic)",
    "404.2(C)": RESTORED + " (rewritten as the neutral conductor at switches; medium)",
    "225.18": RESTORED + " (clearances rewritten; 2026 unverified)",
    "440.4": "Added for the sandbox air-conditioning circuit (found by the content tests); confirm number and text",
    "406.9(A)": RESTORED, "406.9(B)": RESTORED, "410.10(A)": RESTORED, "210.23": RESTORED, "220.18": RESTORED,
}

REVIEW_COLUMNS = ["Reference correct? (Y/N)", "Text accurate? (Y/N)", "Correct reference", "Edition notes", "Comments"]


def load_json(path):
    with open(path, encoding="utf-8") as f:
        return json.load(f)


def write_csv(name, header, rows):
    path = os.path.join(OUT, name)
    with open(path, "w", encoding="utf-8-sig", newline="") as f:
        w = csv.writer(f)
        w.writerow(header + REVIEW_COLUMNS)
        for r in rows:
            w.writerow(r + [""] * len(REVIEW_COLUMNS))
    return len(rows)


_TERMS = {}


def fill_tokens(text, profile_id):
    """Fill {code} and {term:key} the way the app does, with the profile's own terminology."""
    if profile_id not in _TERMS:
        _TERMS[profile_id] = load_json(os.path.join(CODES, profile_id, "terminology.json"))
    t = _TERMS[profile_id]
    words = {e["key"]: e["text"] for e in t.get("terms", [])}
    text = text.replace("{code}", t.get("codeName", ""))
    return re.sub(r"\{term:([A-Za-z0-9-]+)\}", lambda m: words.get(m.group(1), m.group(1)), text)


def violation_rows():
    rows = []
    for path in sorted(glob.glob(os.path.join(SCENARIOS, "*.json"))):
        scenario = load_json(path)
        for v in scenario["violations"]:
            for c in v["citations"]:
                flag = VIOLATION_FLAGS.get((v["violationId"], c["profileId"]), "")
                if not flag and c["profileId"] != "nec":
                    flag = UK_NOTE if c["profileId"] == "bs7671" else CEC_NOTE
                rows.append([
                    flag, scenario["displayName"], v["violationId"], v["conceptId"], v["severity"],
                    c["profileId"], c["reference"], c["text"],
                    fill_tokens(c.get("description") or v["description"], c["profileId"]),
                ])
    # flagged rows first, then by scenario order
    rows.sort(key=lambda r: r[0] == "")
    return rows


def article_rows(profile_id, cited):
    data = load_json(os.path.join(CODES, profile_id, "articles.json"))["articles"]
    related = {r for a in data if a["reference"] in cited for r in a.get("related", [])}
    rows = []
    for a in data:
        ref = a["reference"]
        tied = "cited" if ref in cited else ("related to a cited article" if ref in related else "not tied to a violation")
        if profile_id == "cec":
            flag = CEC_NOTE
        elif profile_id == "bs7671":
            flag = UK_ARTICLE_FLAGS.get(ref, UK_NOTE)
        else:
            flag = ARTICLE_FLAGS.get(ref, "")
        rows.append([flag, ref, a["title"], a["text"], "yes" if a.get("isNewInEdition") else "", tied, a.get("conceptId", "")])
    rows.sort(key=lambda r: (r[0] == "", r[5] == "not tied to a violation"))
    return rows


def card_and_sandbox_rows():
    """Quick reference cards and sandbox circuits (content that used to live in editor scripts)."""
    rows = []
    for path in sorted(glob.glob(os.path.join(CONTENT, "QuickReference", "*.json"))):
        for c in load_json(path)["cards"]:
            rows.append(["", "Quick reference card", c["profileId"], c["cardId"], c["title"], ", ".join(c["codeReferences"]),
                         c["summary"] + " Key rule: " + c["keyRule"]])
    for path in sorted(glob.glob(os.path.join(CONTENT, "Sandbox", "*.json"))):
        for d in load_json(path)["designs"]:
            for c in d["requiredCircuits"]:
                protection = " and ".join(p for p, on in (("GFCI", c["requiresGFCI"]), ("AFCI", c["requiresAFCI"])) if on) or "none"
                rows.append(["", "Sandbox circuit", d["profileId"], d["assetName"], c["circuitName"], c["codeReference"],
                             f'{c["ampsRequired"]} A on {c["wireGauge"]}, {c["poleCount"]}-pole, protection: {protection}. {c["description"]}'])
    return rows


def main():
    os.makedirs(OUT, exist_ok=True)
    vrows = violation_rows()
    n_v = write_csv("1-violation-citations.csv",
                    ["Concern", "Scenario", "Violation ID", "Skill", "Severity", "Code", "Reference cited",
                     "Our paraphrase", "What the scene shows"], vrows)

    cited = {}
    for r in vrows:
        cited.setdefault(r[5], set()).add(r[6])

    counts = {}
    n_extra = write_csv("4-cards-and-sandbox.csv", ["Concern", "Kind", "Code", "Id", "Title or circuit", "References", "What it says"], card_and_sandbox_rows())
    # Numbers 2 and 3 stay with the CEC and NEC and 4 with the cards worksheet, so existing references keep working; later codes follow
    folders = sorted(d for d in os.listdir(CODES) if os.path.isdir(os.path.join(CODES, d)))
    numbered = [(2, 'cec'), (3, 'nec')] + [(5 + i, d) for i, d in enumerate(d for d in folders if d not in ('cec', 'nec'))]
    for i, pid in [(n, d) for n, d in numbered if d in folders]:
        counts[pid] = write_csv(f"{i}-articles-{pid}.csv",
                                ["Concern", "Reference", "Title", "Our paraphrase", "New in edition", "Use in app", "Skill tag"],
                                article_rows(pid, cited.get(pid, set())))
    print("violation citations:", n_v, "| articles:", counts, "| cards and sandbox circuits:", n_extra)


if __name__ == "__main__":
    main()
