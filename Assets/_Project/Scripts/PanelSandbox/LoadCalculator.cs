using System;
using System.Collections.Generic;
using NECInspector.Codes;

namespace NECInspector.PanelSandbox
{
    /// <summary>
    /// Static utility for dwelling-unit load calculations using the standard method
    /// (not optional calculation). Constants, demand factors and standard sizes come from
    /// the active code profile's ElectricalTables (NEC Art. 220 for the NEC profile).
    /// </summary>
    public static class LoadCalculator
    {
        private static ElectricalTables Tables => CodeProfiles.Tables;

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
        /// Calculate general lighting load (NEC Table 220.12: 3 VA per square foot for dwelling units).
        /// </summary>
        public static float CalculateGeneralLighting(float squareFootage)
        {
            return squareFootage * Tables.lightingVAPerArea;
        }

        /// <summary>
        /// Calculate small-appliance circuit load (NEC 220.52: 1,500 VA per required 20A circuit).
        /// </summary>
        public static float CalculateSmallApplianceLoad(int circuitCount = 2)
        {
            return circuitCount * Tables.smallApplianceVA;
        }

        /// <summary>
        /// Calculate laundry circuit load (NEC 220.52: 1,500 VA for the laundry branch circuit).
        /// </summary>
        public static float CalculateLaundryLoad()
        {
            return Tables.laundryVA;
        }

        /// <summary>
        /// Apply the profile's demand-factor tiers to the combined general load.
        /// NEC Table 220.42: first 3,000 VA at 100%, 3,001-120,000 VA at 35%, over 120,000 VA at 25%.
        /// </summary>
        public static float ApplyDemandFactor(float totalVA)
        {
            float result = 0f;
            float lower = 0f;

            foreach (var tier in Tables.generalLoadDemandTiers)
            {
                float upper = tier.upToVA < 0f ? float.MaxValue : tier.upToVA;
                float slice = Math.Min(totalVA, upper) - lower;
                if (slice <= 0f) break;

                result += slice * tier.factor;
                lower = upper;
            }

            return result;
        }

        /// <summary>
        /// Calculate total service load for a dwelling unit using the standard method.
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

            // Step 2: Add fixed appliance loads at 100%, reduced when there are enough of them
            float fixedApplianceVA = 0f;
            int fixedCount = 0;

            if (hasDryer) { fixedApplianceVA += Tables.dryerVA; fixedCount++; }
            if (hasRange) { fixedApplianceVA += Tables.rangeDemandVA; fixedCount++; }

            if (additionalLoads != null)
            {
                foreach (var load in additionalLoads)
                {
                    fixedApplianceVA += load.va;
                    fixedCount++;
                }
            }

            // NEC 220.53: with 4 or more fixed appliances (other than range/dryer/AC),
            // apply 75% demand to the non-range/dryer appliances.
            // Simplified: we count all fixed appliances
            if (fixedCount >= Tables.fixedApplianceDemandThreshold)
            {
                fixedApplianceVA *= Tables.fixedApplianceDemandFactor;
            }

            return demandVA + fixedApplianceVA;
        }

        /// <summary>
        /// Convert VA to amperes at a given voltage (defaults to the profile's service voltage).
        /// </summary>
        public static float ConvertVAToAmps(float va, float? voltage = null)
        {
            float v = voltage ?? Tables.serviceVoltage;
            return v > 0f ? va / v : 0f;
        }

        /// <summary>
        /// Calculate the minimum service size in amps for a given load.
        /// Rounds up to the next standard size in the profile's table.
        /// </summary>
        public static int GetMinimumServiceAmps(float totalVA, float? voltage = null)
        {
            float amps = ConvertVAToAmps(totalVA, voltage);
            int[] standardSizes = Tables.standardServiceSizes;

            foreach (int size in standardSizes)
            {
                if (size >= amps) return size;
            }

            return standardSizes[standardSizes.Length - 1]; // Maximum standard size
        }
    }
}
