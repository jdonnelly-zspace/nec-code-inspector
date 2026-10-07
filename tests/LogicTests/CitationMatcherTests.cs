using NECInspector.Codes;

namespace NECInspector.LogicTests
{
    public static class CitationMatcherTests
    {
        public static void Run(TestContext t)
        {
            Matching(t);
            Parents(t);
        }

        private static void Matching(TestContext t)
        {
            t.Begin("citation matcher");

            // NEC style numbering
            t.IsTrue(CitationMatcher.Default("250.24(A)(1)", "250.24(A)(1)"), "exact match");
            t.IsTrue(CitationMatcher.Default("250.24", "250.24(A)(1)"), "student cites the parent article");
            t.IsTrue(CitationMatcher.Default("Art. 250.24", "250.24(A)(1)"), "Art. prefix is ignored");
            t.IsTrue(CitationMatcher.Default("art. 250.24 (A)(1)", "250.24(A)(1)"), "spaces and lowercase prefix are ignored");
            t.IsTrue(CitationMatcher.Default("250.24(a)(1)", "250.24(A)(1)"), "letter case is ignored");

            t.IsTrue(!CitationMatcher.Default("250.24(A)(1)", "250.24"), "citing a child of the expected article does not match");
            t.IsTrue(!CitationMatcher.Default("210.8", "250.24(A)(1)"), "unrelated article");
            t.IsTrue(!CitationMatcher.Default("", "250.24"), "empty citation");
            t.IsTrue(!CitationMatcher.Default("250.24", ""), "empty expected");
            t.IsTrue(!CitationMatcher.Default(null, "250.24"), "null citation");
            t.IsTrue(!CitationMatcher.Default("Art.", "250.24"), "a label alone is not a citation");

            // A string prefix is not a parent: the next level must start at a boundary
            t.IsTrue(!CitationMatcher.Default("250.2", "250.24"), "250.2 is not a parent of 250.24");
            t.IsTrue(!CitationMatcher.Default("210.1", "210.12(A)"), "210.1 is not a parent of 210.12(A)");
            t.IsTrue(CitationMatcher.Default("210.12", "210.12(A)"), "210.12 is a parent of 210.12(A)");

            // Hyphenated numbering with lowercase subrules (CEC style)
            t.IsTrue(CitationMatcher.Default("26-712", "26-712"), "CEC exact match");
            t.IsTrue(CitationMatcher.Default("Rule 26-712", "26-712(d)(iii)"), "Rule prefix is ignored and the parent matches");
            t.IsTrue(CitationMatcher.Default("26-712(D)(III)", "26-712(d)(iii)"), "CEC letter case is ignored");
            t.IsTrue(!CitationMatcher.Default("26-71", "26-712"), "26-71 is not a parent of 26-712");
            t.IsTrue(CitationMatcher.Default("26", "26-712"), "a section number is a parent of its rules");
            t.IsTrue(!CitationMatcher.Default("2", "26-712"), "2 is not a parent of 26-712");
        }

        private static void Parents(TestContext t)
        {
            t.Begin("citation parents");

            t.IsTrue(CitationMatcher.IsParentOf("110.26", "110.26(A)(1)"), "article is a parent of its subsection");
            t.IsTrue(CitationMatcher.IsParentOf("110.26(A)", "110.26(A)(1)"), "subsection is a parent of its item");
            t.IsTrue(!CitationMatcher.IsParentOf("110.26(A)(1)", "110.26(A)(1)"), "a reference is not its own parent");
            t.IsTrue(!CitationMatcher.IsParentOf("110.26(A)(1)", "110.26"), "a child is not a parent");
            t.IsTrue(!CitationMatcher.IsParentOf(null, "110.26"), "null parent");
            t.IsTrue(!CitationMatcher.IsParentOf("110.26", null), "null child");
        }
    }
}
