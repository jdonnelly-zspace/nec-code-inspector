"""Build the app's occupation file from the research data.

    python tools/occupations/build_app_data.py

Reads docs/occupations/data/[0-9]*.json (the research) and writes Assets/_Project/StreamingAssets/Occupations/occupations.json, the part of it a student sees:
the occupation, its procedures (steps, hazards, controls) and the skills each uses. Student-facing text must not name a code, a publisher, a regulator or a standard, so
references are replaced with generic words and the script fails if any remain. Run build_docs.py afterwards if the research data changed.
"""
import glob
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DATA = os.path.join(ROOT, "docs", "occupations", "data")
OUT = os.path.join(ROOT, "Assets", "_Project", "StreamingAssets", "Occupations", "occupations.json")

# What the owner's decisions put in each release phase (docs/product/DECISIONS.md, D-19: all six core occupations are in the MVP, phase 1)
PHASE = {"47-2111": 1, "47-3013": 1, "47-4011": 1, "49-9051": 1, "47-2231": 1, "49-2098": 1}
ID = {"47-2111": "electricians", "47-3013": "electrician-helpers", "47-4011": "inspectors", "47-2231": "solar-installers",
      "49-2098": "alarm-installers", "49-9051": "line-installers"}
TITLE = {"47-2111": "Electrician", "47-3013": "Electrician's helper", "47-4011": "Electrical inspector",
         "47-2231": "Solar installer", "49-2098": "Fire and security alarm installer", "49-9051": "Power-line installer"}
# Written for students, in our own words (the research summaries name codes and agencies)
SUMMARY = {
    "47-2111": "Installs, maintains and repairs the wiring, lighting, power and control systems of homes and buildings, working from drawings and the code, and tests the result.",
    "47-3013": "Helps electricians by preparing materials, running wire and conduit and keeping the site safe, under supervision, while learning the trade.",
    "47-4011": "Inspects electrical work against the code and the plans, records what is wrong, and decides whether work can be approved.",
    "47-2231": "Installs solar panels, wiring, inverters and disconnects on roofs and buildings, and tests and connects the system.",
    "49-2098": "Installs and tests fire alarm and security systems: devices, panels, low-voltage wiring and their power supplies.",
    "49-9051": "Builds and maintains the lines, poles, transformers and cables that carry power from the utility to customers.",
}

REPLACE = [
    (r"\(often an ICC/IAEI E3-type role\)", ""), (r"\bICC/IAEI\b", "inspector"),
    (r"Follow the NEC worksheet method in order", "Follow the code's worksheet method in order"),
    (r"Fill limits from NEC Chapter 9", "Fill limits from the code's conduit fill tables"),
    (r"Find the adopted edition of the NEC for the jurisdiction", "Find the adopted edition of the code for the jurisdiction"),
    (r"no GFCI or AFCI protection where the NEC forbids it", "no ground-fault or arc-fault protection where the code forbids it"),
    (r"per OSHA", "per the safety regulations"), (r"\(UL 1741, IEEE 1547\)", ""), (r"UL 1741", "the required listing"), (r"listed to UL 9540", "listed for battery storage"),
    (r"UL 9540", "the battery storage listing"),
]
BANNED = re.compile(r"\b(NEC|CEC|NFPA|CSA|IET|BSI|BS ?7671|C22\.1|NESC|IEEE|OSHA|UL \d+|ICC|IAEI|NCCER|IBEW|NECA)\b")


def clean(text):
    for pat, rep in REPLACE:
        text = re.sub(pat, rep, text)
    return re.sub(r"\s{2,}", " ", text).replace(" ,", ",").replace(" .", ".").strip()


def level(coverage):
    word = (coverage or "").strip().split(":")[0].split(" ")[0].lower().strip(" .,")
    return word if word in ("none", "partial", "good") else "none"


def main():
    occupations = []
    problems = []
    # Display order: by release phase, then by occupation code
    for path in sorted(glob.glob(os.path.join(DATA, "[0-9]*.json")), key=lambda q: (PHASE[os.path.basename(q)[:7]], os.path.basename(q))):
        d = json.load(open(path, encoding="utf-8"))
        soc = d["soc"]
        procs = []
        for i, p in enumerate(d["procedures"], 1):
            fields = {k: clean(p[k]) for k in ("name", "purpose", "whenUsed", "role")}
            steps = [clean(s) for s in p["steps"]]
            hazards = [clean(s) for s in p["hazards"]]
            controls = [clean(s) for s in p["controls"]]
            for label, text in [(k, v) for k, v in fields.items()] + [("step", s) for s in steps] + [("hazard", s) for s in hazards] + [("control", s) for s in controls]:
                if BANNED.search(text):
                    problems.append(f"{soc} {p['name'][:40]} {label}: {text[:100]}")
            skills = list(p["appSkills"])
            planned = list(p.get("plannedSkills", []))
            sources = len(p.get("evidence", []))
            # Practice (ordering the steps) is offered only when every skill the procedure needs exists and two sources support it
            practice = bool(skills) and not planned and sources >= 2
            procs.append({"id": f"{ID[soc]}-{i}", "name": fields["name"], "purpose": fields["purpose"], "whenUsed": fields["whenUsed"], "role": fields["role"],
                          "steps": steps, "hazards": hazards, "controls": controls, "skills": skills, "coverage": level(p["appCoverage"]),
                          "practice": practice, "reviewStatus": "draft"})
        occupations.append({"id": ID[soc], "socCode": soc, "title": TITLE[soc], "summary": SUMMARY[soc], "phase": PHASE[soc],
                            "available": PHASE[soc] == 1, "reviewStatus": "draft", "procedures": procs})
    if problems:
        print("student-facing text still names a code, regulator or standard:", file=sys.stderr)
        for p in problems:
            print("  " + p, file=sys.stderr)
        return 2
    os.makedirs(os.path.dirname(OUT), exist_ok=True)
    with open(OUT, "w", encoding="utf-8", newline="\n") as f:
        f.write(json.dumps({"occupations": occupations}, indent=2, ensure_ascii=False) + "\n")
    n = sum(len(o["procedures"]) for o in occupations)
    on = sum(1 for o in occupations for p in o["procedures"] if p["practice"])
    print(f"wrote {len(occupations)} occupations, {n} procedures, {on} with practice")
    return 0


if __name__ == "__main__":
    sys.exit(main())
