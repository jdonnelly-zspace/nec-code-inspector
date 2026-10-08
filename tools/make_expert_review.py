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

# Known concerns from docs/CONTENT_REVIEW.md, keyed by (violationId, profileId)
VIOLATION_FLAGS = {
    ("BC-DEDICATED-BATH-001", "nec"): "Reference may be swapped with BC-DEDICATED-KITCHEN-001",
    ("BC-DEDICATED-KITCHEN-001", "nec"): "Reference may be swapped with BC-DEDICATED-BATH-001",
    ("COM-DISC-SIGHT-001", "nec"): "Reference may be swapped with COM-MOTOR-CTRL-001",
    ("COM-MOTOR-CTRL-001", "nec"): "Reference may be swapped with COM-DISC-SIGHT-001",
    ("COM-RECPT-LOAD-001", "nec"): "Per-outlet load may belong to the 220.14 article, not 220.44",
    ("BC-GFCI-KITCHEN-001", "nec"): "Check the numbering of the 210.8(A) items",
    ("COM-MOTOR-OL-001", "nec"): "Check the overload percentage for motors with a service factor of 1.15 or more",
}
CEC_NOTE = "Draft from public summaries: check rule number, sub-item and edition"

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
                    flag = CEC_NOTE
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
        flag = CEC_NOTE if profile_id == "cec" else ""
        rows.append([flag, ref, a["title"], a["text"], "yes" if a.get("isNewInEdition") else "", tied])
    rows.sort(key=lambda r: (r[0] == "", r[5] == "not tied to a violation"))
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
    for i, pid in enumerate(sorted(d for d in os.listdir(CODES) if os.path.isdir(os.path.join(CODES, d))), start=2):
        counts[pid] = write_csv(f"{i}-articles-{pid}.csv",
                                ["Concern", "Reference", "Title", "Our paraphrase", "New in edition", "Use in app"],
                                article_rows(pid, cited.get(pid, set())))
    print("violation citations:", n_v, "| articles:", counts)


if __name__ == "__main__":
    main()
