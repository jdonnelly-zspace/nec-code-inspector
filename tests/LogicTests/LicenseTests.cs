using System.Linq;
using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// The licensing record every code profile carries (profile.json, license). The tests make sure no code can be
    /// added without one, that a recorded decision is complete, and they keep the open question visible: a warning
    /// names every shipped code that has not been cleared.
    /// </summary>
    public static class LicenseTests
    {
        public static void Run(TestContext t)
        {
            ValidatorRules(t);
            ProfilesCarryALicense(t);
        }

        private static CodeLicense Good(string status = CodeLicense.Unreviewed)
        {
            return new CodeLicense
            {
                status = status, holder = "Holder", note = "What is used.",
                evidence = status == CodeLicense.Licensed ? "Agreement 12" : "",
                checkedOn = status == CodeLicense.Unreviewed ? "" : "2026-10-08"
            };
        }

        private static void ValidatorRules(TestContext t)
        {
            t.Begin("license record rules");

            t.Equal(0, CodeLicenseValidator.Validate(Good()).Count, "an unreviewed record with a holder and a note is valid");
            t.Equal(0, CodeLicenseValidator.Validate(Good(CodeLicense.OwnWords)).Count, "an own-words record with a date is valid");
            t.Equal(0, CodeLicenseValidator.Validate(Good(CodeLicense.Licensed)).Count, "a licensed record with evidence and a date is valid");

            t.IsTrue(CodeLicenseValidator.Validate(null).Count > 0, "a missing record is rejected");
            t.IsTrue(CodeLicenseValidator.Validate(new CodeLicense()).Any(e => e.Contains("status")), "a record with no status is rejected");

            var bad = Good(); bad.status = "maybe";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("must be one of")), "an unknown status is rejected");

            bad = Good(); bad.holder = "";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("holder")), "a record without a copyright holder is rejected");

            bad = Good(); bad.note = " ";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("note")), "a record without a note is rejected");

            bad = Good(CodeLicense.OwnWords); bad.checkedOn = "";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("checkedOn")), "a decision without a date is rejected");

            bad = Good(CodeLicense.Licensed); bad.evidence = "";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("evidence")), "a licence without evidence is rejected");

            bad = Good(CodeLicense.OwnWords); bad.checkedOn = "10/08/2026";
            t.IsTrue(CodeLicenseValidator.Validate(bad).Any(e => e.Contains("yyyy") || e.Contains("date like")), "a date in another format is rejected");

            t.IsTrue(!Good().IsCleared && Good(CodeLicense.OwnWords).IsCleared && Good(CodeLicense.Licensed).IsCleared,
                "only own-words and licensed count as cleared to ship");
        }

        private static void ProfilesCarryALicense(TestContext t)
        {
            t.Begin("profile licences");

            var profiles = ProfileFiles.LoadAll(TestContext.RepoRoot());
            foreach (var p in profiles)
            {
                t.IsTrue(p.manifest.license != null, $"Codes/{p.folder}: profile.json has a license record");
                t.Equal(0, CodeLicenseValidator.Validate(p.manifest.license).Count, $"Codes/{p.folder}: the license record is complete");
            }

            // A code that leaves the record out cannot load
            var nec = profiles.First(p => p.folder == CodeProfileIds.Nec);
            var withoutLicense = new CodeProfileManifest
            {
                id = nec.manifest.id, displayName = nec.manifest.displayName, edition = nec.manifest.edition, region = nec.manifest.region,
                artSet = nec.manifest.artSet, reviewStatus = nec.manifest.reviewStatus, license = null
            };
            t.IsTrue(CodeProfileValidator.Validate(withoutLicense, nec.articles, nec.tables, nec.terminology).Any(e => e.Contains("license")),
                "a profile without a license record is rejected");

            t.Equal("National Fire Protection Association (NFPA)", nec.Build().License.holder, "the profile exposes its licence record");

            // Keep the open question visible without failing the build
            var uncleared = profiles.Where(p => !(p.manifest.license?.IsCleared ?? false)).Select(p => p.folder).ToList();
            if (uncleared.Count > 0)
                t.Warn($"licensing: {string.Join(", ", uncleared)} not cleared to ship (status unreviewed): the owner and counsel need to decide (docs/CONTENT_POLICY.md, Licensing)");
        }
    }
}
