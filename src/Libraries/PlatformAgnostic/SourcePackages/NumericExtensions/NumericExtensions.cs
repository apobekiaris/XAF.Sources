using System;

namespace XAF.SourcePackages.NumericExtensions{
    /// <summary>
    /// Summary description for NumericHelper.
    /// </summary>
    public static class NumericExtensions {
        public static bool GreaterOrEqual(this double value1, double value2, double unimportantDifference = 0.0001) {
            return value1.NearlyEquals(value2) || value1 > value2;
        }

        public static bool NearlyEquals(this double value1, double value2, double unimportantDifference = 0.0001) {
            return Math.Abs(value1 - value2) < unimportantDifference;
        }
        public static double Round(this double d, int decimals = 0) {
            return Math.Round(d, decimals);
        }

        public static decimal RoundNumber(this decimal d, int decimals = 0) {
            return Math.Round(d, decimals);
        }

        
    }
}