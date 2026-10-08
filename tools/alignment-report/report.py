"""Compare a student's skill progress with a credential alignment file. Not part of the app.

    python tools/alignment-report/report.py PROGRESS.json ALIGNMENT.json

PROGRESS.json is the app's progress file (nec_progress.json in the app's data folder).
ALIGNMENT.json is a file from docs/credential-alignment/, for example red-seal-309a-draft.json.

The report says, for each skill the credential asks for, whether the student has attained it at the
required tier, how far along they are, and whether the app has content to teach it under the
credential's installation code. The attainment rule mirrors Scripts/Skills/SkillProgress.cs
(SkillPolicy): at least 3 attempts and a mastery of at least 0.8, and a higher tier covers a lower one.
The tier of a violation comes from its minimum difficulty (Scripts/Skills/SkillTier.cs).
"""
import glob
import json
import os
import sys

HERE = os.path.dirname(os.path.abspath(__file__))
REPO = os.path.abspath(os.path.join(HERE, "..", ".."))
SCENARIOS = os.path.join(REPO, "Assets", "_Project", "Content", "Scenarios")

# Keep in step with SkillPolicy in Scripts/Skills/SkillProgress.cs
MIN_ATTEMPTS = 3
MASTERY_THRESHOLD = 0.8

TIERS = ["Foundation", "Practitioner", "Authority"]
DIFFICULTY_TIER = {"Beginner": 0, "Standard": 1, "Expert": 2}


def load_json(path):
    with open(path, encoding="utf-8-sig") as f:
        return json.load(f)


def content_by_skill(scenario_files, profile_id):
    """{skill: {tier index: number of violations}} for violations that cite the profile."""
    counts = {}
    for data in scenario_files:
        for v in data.get("violations", []):
            if not any(c.get("profileId") == profile_id for c in v.get("citations", [])):
                continue
            tier = DIFFICULTY_TIER.get(v.get("minimumDifficulty"), 1)
            by_tier = counts.setdefault(v["conceptId"], {})
            by_tier[tier] = by_tier.get(tier, 0) + 1
    return counts


def is_attained(stat):
    return stat.get("attempts", 0) >= MIN_ATTEMPTS and stat.get("mastery", 0.0) >= MASTERY_THRESHOLD


def build_report(progress, alignment, scenario_files):
    stats = (progress.get("skills") or {}).get("stats") or []
    content = content_by_skill(scenario_files, alignment.get("codeProfileId"))

    rows = []
    for req in alignment.get("requirements", []):
        skill, tier_name = req["skillId"], req["tier"]
        tier = TIERS.index(tier_name)

        mine = [s for s in stats if s.get("skillId") == skill and s.get("tier", 0) >= tier]
        attained = any(is_attained(s) for s in mine)
        best = max(mine, key=lambda s: s.get("mastery", 0.0), default=None)

        lower = [x for x in stats if x.get("skillId") == skill and x.get("tier", 0) < tier]
        lower_best = max(lower, key=lambda x: x.get("tier", 0), default=None)

        available = sum(n for t, n in content.get(skill, {}).items() if t >= tier)
        rows.append({
            "skill": skill,
            "tier": tier_name,
            "weight": float(req.get("weight", 0)),
            "attained": attained,
            "attempts": best.get("attempts", 0) if best else 0,
            "mastery": best.get("mastery", 0.0) if best else 0.0,
            "content": available,
            "lower_tier": TIERS[lower_best["tier"]] if lower_best else None,
            "lower_attempts": lower_best.get("attempts", 0) if lower_best else 0,
            "lower_mastery": lower_best.get("mastery", 0.0) if lower_best else 0.0,
            "file_says_deferred": bool(req.get("deferred", False)),
        })

    total = sum(r["weight"] for r in rows) or 1.0
    return {
        "name": alignment.get("displayName", alignment.get("id", "credential")),
        "code": alignment.get("codeProfileId"),
        "status": alignment.get("reviewStatus", "unknown"),
        "rows": rows,
        "attained_share": sum(r["weight"] for r in rows if r["attained"]) / total,
        "teachable_share": sum(r["weight"] for r in rows if r["content"] > 0) / total,
    }


def format_report(report):
    lines = [f"{report['name']} (installation code: {report['code']})"]
    if report["status"] != "reviewed":
        lines.append(f"  Alignment status: {report['status']}. The skill mapping and weights are estimates until an expert confirms them.")

    lines.append(f"  Skills attained at the required tier: {report['attained_share']:.0%} of the weight")
    lines.append(f"  Weight the app can teach under this code today: {report['teachable_share']:.0%}")

    rows = report["rows"]
    todo = sorted((r for r in rows if not r["attained"] and r["content"] > 0), key=lambda r: -r["weight"])
    blocked = sorted((r for r in rows if not r["attained"] and r["content"] == 0), key=lambda r: -r["weight"])
    done = [r for r in rows if r["attained"]]

    def line(r):
        if r["attempts"]:
            progress = f"{r['attempts']} attempts, mastery {r['mastery']:.2f}"
        elif r["lower_tier"]:
            progress = f"practised at {r['lower_tier']} only ({r['lower_attempts']} attempts, mastery {r['lower_mastery']:.2f})"
        else:
            progress = "not started"
        return f"    {r['skill']} at {r['tier']} (weight {r['weight']:g}): {progress}"

    lines.append("  Next to learn (the app has content for these):")
    lines.extend(line(r) for r in todo) if todo else lines.append("    nothing left that the app can teach")
    lines.append("  Waiting on content (the app has none for this skill and tier under this code):")
    lines.extend(line(r) for r in blocked) if blocked else lines.append("    nothing")
    lines.append("  Attained:")
    lines.extend(line(r) for r in done) if done else lines.append("    nothing yet")

    # Keep the alignment file honest about content gaps
    for r in rows:
        if r["file_says_deferred"] != (r["content"] == 0):
            now = "has no content" if r["content"] == 0 else "now has content"
            lines.append(f"  Note: {r['skill']} {now} under this code, but the alignment file's 'deferred' flag says {r['file_says_deferred']}.")
    return "\n".join(lines)


def main(argv):
    if len(argv) != 3:
        print(__doc__)
        return 2
    progress = load_json(argv[1])
    alignment = load_json(argv[2])
    scenario_files = [load_json(p) for p in sorted(glob.glob(os.path.join(SCENARIOS, "*.json")))]
    print(format_report(build_report(progress, alignment, scenario_files)))
    return 0


if __name__ == "__main__":
    sys.exit(main(sys.argv))
