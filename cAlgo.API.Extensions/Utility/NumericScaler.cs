namespace cAlgo.API.Extensions.Utility
{
    public static class NumericScaler
    {
        public static double MinMax(double number, double min, double max, double minAllowedNumber, double maxAllowedNumber)
        {
            var b = max - min != 0 ? max - min : 1 / max;
            var uninterpolate = (number - min) / b;
            var result = minAllowedNumber * (1 - uninterpolate) + maxAllowedNumber * uninterpolate;

            return result;
        }
    }
}