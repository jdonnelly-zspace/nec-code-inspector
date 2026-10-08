import json
import os
import unittest

import report

HERE = os.path.dirname(os.path.abspath(__file__))


def stat(skill, tier, attempts, mastery):
    return {"skillId": skill, "tier": tier, "attempts": attempts, "mastery": mastery}


def violation(skill, difficulty, profiles):
    return {"conceptId": skill, "minimumDifficulty": difficulty, "citations": [{"profileId": p} for p in profiles]}


ALIGNMENT = {
    "id": "demo", "displayName": "Demo credential", "codeProfileId": "cec", "reviewStatus": "draft",
    "requirements": [
        {"skillId": "a", "tier": "Practitioner", "weight": 6, "deferred": False},
        {"skillId": "b", "tier": "Practitioner", "weight": 3, "deferred": False},
        {"skillId": "c", "tier": "Practitioner", "weight": 1, "deferred": True},
    ],
}
SCENARIOS = [{"violations": [
    violation("a", "Standard", ["nec", "cec"]),
    violation("b", "Beginner", ["cec"]),       # Foundation only: cannot teach a Practitioner requirement
    violation("b", "Expert", ["nec"]),         # not cited for the credential's code
]}]


class AttainmentTests(unittest.TestCase):
    def test_attained_needs_attempts_and_mastery(self):
        self.assertTrue(report.is_attained(stat("a", 1, 3, 0.8)))
        self.assertFalse(report.is_attained(stat("a", 1, 2, 0.95)))
        self.assertFalse(report.is_attained(stat("a", 1, 5, 0.79)))

    def test_higher_tier_covers_lower_requirement(self):
        progress = {"skills": {"stats": [stat("a", 2, 4, 0.9)]}}
        rows = report.build_report(progress, ALIGNMENT, SCENARIOS)["rows"]
        self.assertTrue(rows[0]["attained"])

    def test_lower_tier_does_not_cover_higher_requirement(self):
        progress = {"skills": {"stats": [stat("a", 0, 9, 1.0)]}}
        rows = report.build_report(progress, ALIGNMENT, SCENARIOS)["rows"]
        self.assertFalse(rows[0]["attained"])

    def test_empty_progress(self):
        r = report.build_report({}, ALIGNMENT, SCENARIOS)
        self.assertEqual(r["attained_share"], 0)
        self.assertTrue(all(not row["attained"] for row in r["rows"]))


class ContentTests(unittest.TestCase):
    def test_content_counts_only_the_credentials_code_and_the_required_tier(self):
        rows = report.build_report({}, ALIGNMENT, SCENARIOS)["rows"]
        self.assertEqual(rows[0]["content"], 1)   # a: one Standard violation citing cec
        self.assertEqual(rows[1]["content"], 0)   # b: the cec one is Foundation, the Expert one cites only nec
        self.assertEqual(rows[2]["content"], 0)   # c: none

    def test_shares_are_weighted(self):
        progress = {"skills": {"stats": [stat("a", 1, 3, 0.9)]}}
        r = report.build_report(progress, ALIGNMENT, SCENARIOS)
        self.assertAlmostEqual(r["attained_share"], 0.6)
        self.assertAlmostEqual(r["teachable_share"], 0.6)


class FormatTests(unittest.TestCase):
    def test_report_sections_and_notes(self):
        progress = {"skills": {"stats": [stat("a", 1, 1, 0.5)]}}
        text = report.format_report(report.build_report(progress, ALIGNMENT, SCENARIOS))
        self.assertIn("Demo credential (installation code: cec)", text)
        self.assertIn("estimates until an expert confirms", text)
        self.assertIn("a at Practitioner (weight 6): 1 attempts, mastery 0.50", text)
        self.assertIn("b at Practitioner (weight 3): not started", text)
        # b has no content but the file does not say deferred
        self.assertIn("Note: b has no content", text)

    def test_progress_at_a_lower_tier_is_shown_but_does_not_count(self):
        progress = {"skills": {"stats": [stat("a", 0, 6, 0.95)]}}
        r = report.build_report(progress, ALIGNMENT, SCENARIOS)
        self.assertFalse(r["rows"][0]["attained"])
        self.assertIn("a at Practitioner (weight 6): practised at Foundation only (6 attempts, mastery 0.95)", report.format_report(r))

    def test_real_files(self):
        alignment = report.load_json(os.path.join(report.REPO, "docs", "credential-alignment", "red-seal-309a-draft.json"))
        files = [report.load_json(os.path.join(report.SCENARIOS, f)) for f in os.listdir(report.SCENARIOS) if f.endswith(".json")]
        text = report.format_report(report.build_report({}, alignment, files))
        self.assertIn("Red Seal", text)
        self.assertIn("Waiting on content", text)


if __name__ == "__main__":
    unittest.main()
