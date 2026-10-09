"""Scaffold a new violation for a scenario, with the fields every violation needs and a reminder of the checklist.

    python tools/new_violation.py --scenario branch-circuits --skill shock-protection --difficulty Beginner \\
        --id BC-GFCI-OUTDOOR-001 --codes nec,cec

Prints the violation JSON and the reference-entry stubs to stdout. With --write the violation is appended to the
scenario file. Every placeholder starts with TODO, and the logic tests fail while any TODO is left in the content, so
a half-written violation cannot ship. See docs/AUTHORING_VIOLATIONS.md for the checklist.
"""
import argparse
import glob
import json
import os
import re
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
SCENARIOS = os.path.join(ROOT, "Assets", "_Project", "Content", "Scenarios")
CODES = os.path.join(ROOT, "Assets", "_Project", "StreamingAssets", "Codes")
CONCEPTS = os.path.join(ROOT, "Assets", "_Project", "Scripts", "Data", "ConceptIds.cs")
DIFFICULTIES = ["Beginner", "Standard", "Expert"]
SEVERITIES = ["Minor", "Major", "Critical"]


def skills():
    text = open(CONCEPTS, encoding="utf-8").read()
    return re.findall(r'public const string \w+ = "([a-z-]+)";', text)


def load(path):
    raw = open(path, encoding="utf-8-sig", newline="").read()
    return json.loads(raw), "\r\n" in raw


def save(path, data, crlf):
    out = json.dumps(data, indent=2, ensure_ascii=False) + "\n"
    if crlf:
        out = out.replace("\n", "\r\n")
    with open(path, "w", encoding="utf-8", newline="") as f:
        f.write(out)


def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--scenario", required=True, help="scenario file name without .json, e.g. branch-circuits")
    ap.add_argument("--skill", required=True, help="skill id from ConceptIds.cs")
    ap.add_argument("--difficulty", required=True, choices=DIFFICULTIES, help="lowest difficulty the violation appears at")
    ap.add_argument("--id", required=True, dest="vid", help="violation id, e.g. BC-GFCI-OUTDOOR-001")
    ap.add_argument("--codes", required=True, help="comma-separated code ids to cite, e.g. nec,cec")
    ap.add_argument("--severity", default="Major", choices=SEVERITIES)
    ap.add_argument("--subtle", action="store_true", help="only visible at Expert")
    ap.add_argument("--write", action="store_true", help="append to the scenario file instead of only printing")
    a = ap.parse_args()

    errors = []
    if a.skill not in skills():
        errors.append(f"unknown skill '{a.skill}' (skills: {', '.join(skills())})")
    if not re.fullmatch(r"[A-Za-z0-9_-]+", a.vid):
        errors.append("violation id may only contain letters, digits, '_' and '-'")
    path = os.path.join(SCENARIOS, a.scenario + ".json")
    if not os.path.exists(path):
        errors.append(f"no scenario file {path}")
    codes = [c.strip() for c in a.codes.split(",") if c.strip()]
    for c in codes:
        if not os.path.isdir(os.path.join(CODES, c)):
            errors.append(f"no code folder StreamingAssets/Codes/{c}")
    if not codes:
        errors.append("name at least one code")

    taken = set()
    for f in glob.glob(os.path.join(SCENARIOS, "*.json")):
        d, _ = load(f)
        taken.update(v["violationId"] for v in d["violations"])
    if a.vid in taken:
        errors.append(f"violation id '{a.vid}' already exists")

    if errors:
        for e in errors:
            print("error:", e, file=sys.stderr)
        return 2

    violation = {
        "violationId": a.vid,
        "conceptId": a.skill,
        "description": "TODO: what the student should find wrong, in plain words (use {term:key} for words that differ by region)",
        "citations": [
            {"profileId": c, "reference": "TODO: the rule number as written in this code",
             "text": "TODO: the rule in your own words (never copy the code's wording)"} for c in codes
        ],
        "severity": a.severity,
        "minimumDifficulty": a.difficulty,
        "isSubtle": a.subtle,
        "componentObjectName": "TODO: scene object name",
        "hintText": "TODO: a nudge for Beginner",
        "componentType": "TODO: Breaker, Receptacle, Conductor...",
        "inspectionNote": "TODO: what the student should observe"
    }

    print(json.dumps(violation, indent=2, ensure_ascii=False))
    print("\nReference-entry stubs for StreamingAssets/Codes/<code>/articles.json (skip any entry that already exists):")
    for c in codes:
        print(f"  {c}: " + json.dumps({"reference": "TODO", "title": "TODO", "text": "TODO: own words", "section": 0,
                                       "keywords": ["TODO"], "related": [], "conceptId": a.skill, "isNewInEdition": False}))

    if a.write:
        d, crlf = load(path)
        d["violations"].append(violation)
        save(path, d, crlf)
        print(f"\nAppended {a.vid} to {a.scenario}.json. Fill in every TODO, then run:")
    else:
        print("\nNot written (add --write to append it to the scenario file). After adding it, run:")
    print("  dotnet run --project tests/LogicTests      (fails while a TODO is left, and checks citations and scene facts)")
    print("  python tools/make_expert_review.py          (refreshes the expert worksheets)")
    print("Checklist: docs/AUTHORING_VIOLATIONS.md")
    return 0


if __name__ == "__main__":
    sys.exit(main())
