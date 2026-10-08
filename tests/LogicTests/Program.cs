using System;

namespace NECInspector.LogicTests
{
    public static class Program
    {
        public static int Main()
        {
            var ctx = new TestContext();

            ElectricalTablesTests.Run(ctx);
            LoadCalculatorTests.Run(ctx);
            CitationMatcherTests.Run(ctx);
            CodeProfileTests.Run(ctx);
            SkillProgressTests.Run(ctx);
            SkillCoverageTests.Run(ctx);
            TerminologyTests.Run(ctx);
            ContentPolicyTests.Run(ctx);
            ScenarioDataTests.Run(ctx);
            CodeProfileFilesTests.Run(ctx);
            DataCodeProfileTests.Run(ctx);
            CodeProfileChoicesTests.Run(ctx);
            ComplianceRulesTests.Run(ctx);
            ProtectionScopeTests.Run(ctx);
            ContentFilesTests.Run(ctx);
            MetricProfileTests.Run(ctx);
            CitationCreditTests.Run(ctx);
            LicenseTests.Run(ctx);

            foreach (string warning in ctx.Warnings)
                Console.WriteLine($"WARN  {warning}");
            foreach (string failure in ctx.Failures)
                Console.WriteLine($"FAIL  {failure}");

            Console.WriteLine($"{ctx.Checks} checks, {ctx.Failures.Count} failed, {ctx.Warnings.Count} warnings");
            return ctx.Failures.Count == 0 ? 0 : 1;
        }
    }
}
