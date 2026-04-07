using System;
using System.Collections.Generic;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// Static utility for NEC Article 220 residential load calculations.
    /// Uses the standard method (not optional calculation).
    /// </summary>
    public static class LoadCalculator
    {
        public const float LIGHTING_VA_PER_SQFT = 3f;        // Table 220.12
        public const float SMALL_APPLIANCE_VA = 1500f;        // Art. 220.52
        public const float LAUNDRY_VA = 1500f;                // Art. 220.52
        public const float DRYER_VA = 5000f;                  // Art. 220.54
        public const float RANGE_DEMAND_VA = 8000f;           // Table 220.55 (single, ≤12kW)

        [Serializable]
        public struct CircuitLoad
        {
            public string name;
            public float va;

            public CircuitLoad(string name, float va)
            {
                this.name = name;
                this.va = va;
            }
        }

        /// <summary>
        /// Calculate general lighting load per NEC Art. 220.12.
        /// 3 VA per square foot for dwelling units.
        /// </summary>
        public static float CalculateGeneralLighting(float squareFootage)
        {
            return squareFootage * LIGHTING_VA_PER_SQFT;
        }

        /// <summary>
        /// Calculate small-appliance circuit load per NEC Art. 220.52.
        /// 1,500 VA per required 20A small-appliance branch circuit.
        /// </summary>
        public static float CalculateSmallApplianceLoad(int circuitCount = 2)
        {
            return circuitCount * SMALL_APPLIANCE_VA;
        }

        /// <summary>
        /// Calculate laundry circuit load per NEC Art. 220.52.
        /// 1,500 VA for the laundry branch circuit.
        /// </summary>
        public static float CalculateLaundryLoad()
        {
            return LAUNDRY_VA;
        }

        /// <summary>
        /// Apply Table 220.42 demand factors to general lighting +
        /// small-appliance + laundry combined load.
        /// First 3,000 VA at 100%, 3,001-120,000 VA at 35%, over 120,000 VA at 25%.
        /// </summary>
        public static float ApplyDemandFactor(float totalVA)
        {
            if (totalVA <= 3000f)
                return totalVA;

            float result = 3000f; // First 3,000 at 100%

            if (totalVA <= 120000f)
            {
                result += (totalVA - 3000f) * 0.35f;
            }
            else
            {
                result += (120000f - 3000f) * 0.35f;
                result += (totalVA - 120000f) * 0.25f;
            }

            return result;
        }

        /// <summary>
        /// Calculate total service load for a dwelling unit using NEC standard method.
        /// Combines general lighting (with demand factor), fixed appliances, and large loads.
        /// </summary>
        public static float CalculateTotalServiceLoad(
            float squareFootage,
            int smallApplianceCircuits = 2,
            bool hasLaundry = true,
            bool hasDryer = true,
            bool hasRange = true,
            List<CircuitLoad> additionalLoads = null)
        {
            // Step 1: General lighting + small appliance + laundry
            float lightingVA = CalculateGeneralLighting(squareFootage);
            float smallAppVA = CalculateSmallApplianceLoad(smallApplianceCircuits);
            float laundryVA = hasLaundry ? CalculateLaundryLoad() : 0f;

            // Apply demand factor to combined lighting/SA/laundry
            float combinedVA = lightingVA + smallAppVA + laundryVA;
            float demandVA = ApplyDemandFactor(combinedVA);

            // Step 2: Add fixed appliance loads at 100% (or 75% if 4+ appliances)
            float fixedApplianceVA = 0f;
            int fixedCount = 0;

            if (hasDryer) { fixedApplianceVA += DRYER_VA; fixedCount++; }
            if (hasRange) { fixedApplianceVA += RANGE_DEMAND_VA; fixedCount++; }

            if (additionalLoads != null)
            {
                foreach (var load in additionalLoads)
                {
                    fixedApplianceVA += load.va;
                    fixedCount++;
                }
            }

            // If 4 or more fixed appliances (other than range/dryer/AC),
            // apply 75% demand to the non-range/dryer appliances
            // Simplified: we count all fixed appliances
            if (fixedCount >= 4)
            {
                fixedApplianceVA *= 0.75f;
            }

            return demandVA + fixedApplianceVA;
        }

        /// <summary>
        /// Convert VA to amperes at a given voltage.
        /// </summary>
        public static float ConvertVAToAmps(float va, float voltage = 240f)
        {
            return voltage > 0f ? va / voltage : 0f;
        }

        /// <summary>
        /// Calculate the minimum service size in amps for a given load.
        /// Rounds up to the next standard breaker size.
        /// </summary>
        public static int GetMinimumServiceAmps(float totalVA, float voltage = 240f)
        {
            float amps = ConvertVAToAmps(totalVA, voltage);
            int[] standardSizes = { 60, 100, 125, 150, 200, 225, 300, 400 };

            foreach (int size in standardSizes)
            {
                if (size >= amps) return size;
            }

            return 400; // Maximum standard residential
        }
    }
}
