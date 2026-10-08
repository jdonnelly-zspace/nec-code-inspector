using System.Collections.Generic;
using NECInspector.PanelSandbox;

namespace NECInspector.LogicTests
{
    /// <summary>
    /// Expected values come from the pre-refactor NEC arithmetic (3 VA/ft2, Table 220.42 tiers,
    /// 75% fixed-appliance factor from four appliances) so they pin behavior across the tables refactor.
    /// </summary>
    public static class LoadCalculatorTests
    {
        public static void Run(TestContext t)
        {
            DemandFactorTiers(t);
            TotalServiceLoad(t);
            ConversionsAndServiceSize(t);
        }

        private static void DemandFactorTiers(TestContext t)
        {
            t.Begin("load demand factor");
            t.Near(0, LoadCalculator.ApplyDemandFactor(0), "0 VA");
            t.Near(2000, LoadCalculator.ApplyDemandFactor(2000), "below first tier");
            t.Near(3000, LoadCalculator.ApplyDemandFactor(3000), "exactly first tier");
            t.Near(5450, LoadCalculator.ApplyDemandFactor(10000), "3000 + 7000 * 0.35");
            t.Near(51450, LoadCalculator.ApplyDemandFactor(150000), "3000 + 117000 * 0.35 + 30000 * 0.25");
        }

        private static void TotalServiceLoad(TestContext t)
        {
            t.Begin("load total service");

            // 2000 ft2: lighting 6000 + small appliance 3000 + laundry 1500 = 10500 VA
            // demand: 3000 + 7500 * 0.35 = 5625; dryer 5000 + range 8000 = 13000 (2 appliances, no 75% factor)
            t.Near(18625, LoadCalculator.CalculateTotalServiceLoad(2000f), "2000 ft2 default dwelling");

            // Two extra loads make four fixed appliances: (5000 + 8000 + 1000 + 2000) * 0.75 = 12000
            var extras = new List<LoadCalculator.CircuitLoad>
            {
                new LoadCalculator.CircuitLoad("water heater", 1000f),
                new LoadCalculator.CircuitLoad("dishwasher", 2000f)
            };
            t.Near(5625 + 12000, LoadCalculator.CalculateTotalServiceLoad(2000f, additionalLoads: extras), "75% factor with four appliances");

            t.Near(3000, LoadCalculator.CalculateSmallApplianceLoad(), "two small-appliance circuits");
            t.Near(1500, LoadCalculator.CalculateLaundryLoad(), "laundry");
            t.Near(6000, LoadCalculator.CalculateGeneralLighting(2000f), "general lighting");
        }

        private static void ConversionsAndServiceSize(TestContext t)
        {
            t.Begin("load conversions");
            t.Near(10, LoadCalculator.ConvertVAToAmps(2400f), "default voltage is the 240 V service");
            t.Near(20, LoadCalculator.ConvertVAToAmps(2400f, 120f), "explicit 120 V");
            t.Near(0, LoadCalculator.ConvertVAToAmps(2400f, 0f), "zero voltage returns 0");

            t.Equal(200, LoadCalculator.GetMinimumServiceAmps(48000f), "200 A exactly");
            t.Equal(225, LoadCalculator.GetMinimumServiceAmps(48001f), "rounds up to next standard size");
            t.Equal(60, LoadCalculator.GetMinimumServiceAmps(100f), "smallest standard size");
            t.Equal(400, LoadCalculator.GetMinimumServiceAmps(500000f), "capped at the largest standard size");
        }
    }
}
