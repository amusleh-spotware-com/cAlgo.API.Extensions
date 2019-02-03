namespace cAlgo.API.Extensions.Utility
{
    public static class NumericScaler
    {
        public static double MinMax(double number, double min, double max, double minAllowedNumber, double maxAllowedNumber)
        {
            double b = (max - min) != 0 ? max - min : 1 / max;
            double uninterpolate = (number - min) / b;
            double result = minAllowedNumber * (1 - uninterpolate) + maxAllowedNumber * uninterpolate;

            return result;
        }
    }
}