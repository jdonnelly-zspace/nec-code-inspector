using System;

namespace NECInspector.PanelSandbox
{
    [Serializable]
    public class ComplianceResult
    {
        public string ruleId;
        public string ruleName;
        public string codeReference;
        public bool passed;
        public string message;

        public ComplianceResult(string ruleId, string ruleName, string codeReference, bool passed, string message)
        {
            this.ruleId = ruleId;
            this.ruleName = ruleName;
            this.codeReference = codeReference;
            this.passed = passed;
            this.message = message;
        }
    }
}
