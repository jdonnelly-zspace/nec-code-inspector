using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    public static class CitationMatcherTests
    {
        public static void Run(TestContext t)
        {
            t.Begin("citation matcher");

            t.IsTrue(CitationMatcher.Default("250.24(A)(1)", "250.24(A)(1)"), "exact match");
            t.IsTrue(CitationMatcher.Default("250.24", "250.24(A)(1)"), "student cites the parent article");
            t.IsTrue(CitationMatcher.Default("Art. 250.24", "250.24(A)(1)"), "Art. prefix is ignored");
            t.IsTrue(CitationMatcher.Default("art. 250.24 (A)(1)", "250.24(A)(1)"), "spaces and lowercase prefix are ignored");

            t.IsTrue(!CitationMatcher.Default("250.24(A)(1)", "250.24"), "citing a child of the expected article does not match");
            t.IsTrue(!CitationMatcher.Default("210.8", "250.24(A)(1)"), "unrelated article");
            t.IsTrue(!CitationMatcher.Default("", "250.24"), "empty citation");
            t.IsTrue(!CitationMatcher.Default("250.24", ""), "empty expected");
            t.IsTrue(!CitationMatcher.Default(null, "250.24"), "null citation");

            // Known limitation of the string-prefix rule: 250.2 is a string prefix of 250.24
            t.IsTrue(CitationMatcher.Default("250.2", "250.24"), "string-prefix rule also accepts 250.2 for 250.24 (documented limitation)");
        }
    }
}
