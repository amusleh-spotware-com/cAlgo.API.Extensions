using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.API.Extensions
{
    public static class DataSeriesExtensions
    {
        /// <summary>
        /// Creates a List<double> from a DataSeries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <returns>List<double></returns>
        public static List<double> ToList(this DataSeries dataSeries)
        {
            List<double> data = new List<double>();

            for (int i = 0; i < dataSeries.Count; i++)
            {
                data.Add(i);
            }

            return data;
        }

        /// <summary>
        /// Returns the maximum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static double Maximum(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            double max = double.NegativeInfinity;

            for (int i = startIndex; i <= endIndex; i++)
            {
                max = Math.Max(dataSeries[i], max);
            }

            return max;
        }

        /// <summary>
        /// Returns the minimum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static double Minimum(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            double min = double.PositiveInfinity;

            for (int i = startIndex; i <= endIndex; i++)
            {
                min = Math.Min(dataSeries[i], min);
            }

            return min;
        }

        /// <summary>
        /// Checks if the index value is higher than x previous and future values in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="previousValues">The number of index previous values to check</param>
        /// <param name="futureValues">The number of index future values to check</param>
        /// <param name="equal">Check for equality</param>
        /// <returns>bool</returns>
        public static bool IsHigherHigh(
            this DataSeries dataSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            double previousBarsHighest = previousValues > 0 ? dataSeries.Maximum(index - previousValues, index - 1) : double.NegativeInfinity;
            double futureBarsHighest = futureValues > 0 ? dataSeries.Maximum(index + 1, index + futureValues) : double.NegativeInfinity;

            if (equal)
            {
                return dataSeries[index] >= previousBarsHighest && dataSeries[index] >= futureBarsHighest;
            }
            else
            {
                return dataSeries[index] > previousBarsHighest && dataSeries[index] > futureBarsHighest;
            }
        }

        /// <summary>
        /// Checks if the index value is lower than x previous and future values in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="previousValues">The number of index previous values to check</param>
        /// <param name="futureValues">The number of index future values to check</param>
        /// <param name="equal">Check for equality</param>
        /// <returns>bool</returns>
        public static bool IsLowerLow(
            this DataSeries dataSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            double previousBarsLowest = previousValues > 0 ? dataSeries.Minimum(index - previousValues, index - 1) : double.NegativeInfinity;
            double futureBarsLowest = futureValues > 0 ? dataSeries.Minimum(index + 1, index + futureValues) : double.NegativeInfinity;

            if (equal)
            {
                return dataSeries[index] <= previousBarsLowest && dataSeries[index] <= futureBarsLowest;
            }
            else
            {
                return dataSeries[index] < previousBarsLowest && dataSeries[index] < futureBarsLowest;
            }
        }

        /// <summary>
        /// Returns the distance of index (parameter) value with highest value in it's x (periods) previous values in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="periods">The number of index previous values to check</param>
        /// <returns>double</returns>
        public static double GetDistanceWithHigherHigh(this DataSeries dataSeries, int index, int periods)
        {
            double highValue = dataSeries.Maximum(index - periods, index - 1);

            return dataSeries[index] - highValue;
        }

        /// <summary>
        /// Returns the distance of index (parameter) value with lowest value in it's x (periods) previous values
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="periods">The number of index previous values to check</param>
        /// <returns>double</returns>
        public static double GetDistanceWithLowerLow(this DataSeries dataSeries, int index, int periods)
        {
            double lowValue = dataSeries.Minimum(index - periods, index - 1);

            return dataSeries[index] - lowValue;
        }

        /// <summary>
        /// Returns the divergence between two dataseries
        /// </summary>
        /// <param name="firstSeries"></param>
        /// <param name="index">Index of the value you want to get its divergence</param>
        /// <param name="secondSeries">The second dataseries</param>
        /// <param name="periods">This number of previous values from index will be used in calculation</param>
        /// <param name="lag">The number of values to wait for confirmation of divergence</param>
        /// <param name="maxValuesToScan">Maximum number of previous values from index to scan for finding divergence</param>
        /// <returns>List<Divergence></returns>
        public static List<Divergence> GetDivergence(
            this DataSeries firstSeries, int index, DataSeries secondSeries, int periods = 20, int lag = 3, int maxValuesToScan = 100)
        {
            List<Divergence> result = new List<Divergence>();

            // Subtracting the lag number from index
            index = index - lag;

            // First Condition For Down Divergence: The current value must be higher than x (periods) previous and y (lag) after values
            bool isHigherHigh = firstSeries.IsHigherHigh(index, periods, lag);

            // First Condition For Up Divergence: The current value must be lower than x (periods) previous and y (lag) after values
            bool isLowerLow = firstSeries.IsLowerLow(index, periods, lag);

            if (isHigherHigh || isLowerLow)
            {
                for (int i = index - 1; i >= index - maxValuesToScan; i--)
                {
                    // Second Condition For Down Divergence: This (i) previous value must be an higher high too
                    bool isThisValueHigherHigh = firstSeries.IsHigherHigh(i, periods, index - i);

                    // Second Condition For Up Divergence: This (i) previous value must be a lower low too
                    bool isThisValueLowerLow = firstSeries.IsLowerLow(i, periods, index - i);

                    // Third Condition For Down Divergence: This (i) previous value must be higher than all values upto current value (index)
                    bool isThisValueHigherThanAllValuesInBetween = firstSeries[i] > firstSeries.Maximum(i + 1, index);

                    // Third Condition For Up Divergence: This (i) previous value must be lower than all values upto current value (index)
                    bool isThisValueLowerThanAllValuesInBetween = firstSeries[i] < firstSeries.Minimum(i + 1, index);

                    if ((isThisValueHigherHigh && isThisValueHigherThanAllValuesInBetween) ||
                        (isThisValueLowerLow && isThisValueLowerThanAllValuesInBetween))
                    {
                        // Fourth Condition For Down Divergence: Is the second series current value an higher high
                        bool isSecondSeriesThisValueHigherHighToo = secondSeries.IsHigherHigh(index, periods, lag, equal: false);

                        // Fourth Condition For Up Divergence: Is the second series current value a lower low
                        bool isSecondSeriesThisValueLowerLowToo = secondSeries.IsLowerLow(index, periods, lag, equal: false);

                        // Fifth Condition For Down Divergence: Is the second series current value (index) higher than this (i) value
                        bool isSecondSeriesCurrentValueHigherThanThisValue = secondSeries[index] > secondSeries[i];

                        // Fifth Condition For Up Divergence: Is the second series current value (index) lower than this (i) value
                        bool isSecondSeriesCurrentValueLowerThanThisValue = secondSeries[index] < secondSeries[i];

                        // Sixth Condition For Down Divergence: Is the second series this (i) value lower than highest value up to current
                        // value (index)
                        bool isSecondSeriesThisValueLowerThanAllValuesInBetween = secondSeries[i] < secondSeries.Maximum(i + 1, index);

                        // Sixth Condition For Up Divergence: Is the second series this (i) value higher than lowest value up to current
                        // value (index)
                        bool isSecondSeriesThisValueHigherThanAllValuesInBetween = secondSeries[i] > secondSeries.Minimum(i + 1, index);

                        int halfIndex = index - ((index - i) / 2);

                        // Seventh Condition For Down Divergence: Is the current value in first series higher than half of the values in
                        // between
                        bool isCurrentValueHigherThanHalfOfValuesInBetween = firstSeries[index] > firstSeries.Maximum(halfIndex, index - 1);

                        // Seventh Condition For Up Divergence: Is the current value in first series lower than half of the values in
                        // between
                        bool isCurrentValueLowerThanHalfOfValuesInBetween = firstSeries[index] < firstSeries.Minimum(halfIndex, index - 1);

                        // Eighth Condition For Down Divergence: Is this value in first series higher than half of the values in between
                        bool isThisValueHigherThanHalfOfValuesInBetween = firstSeries[i] > firstSeries.Maximum(i + 1, halfIndex);

                        // Eighth Condition For Up Divergence: Is this value in first series lower than half of the values in between
                        bool isThisValueLowerThanHalfOfValuesInBetween = firstSeries[i] < firstSeries.Minimum(i + 1, halfIndex);

                        // Checking back all conditions of down divergence
                        if (isHigherHigh &&
                            isThisValueHigherHigh &&
                            isThisValueHigherThanAllValuesInBetween &&
                            isSecondSeriesThisValueHigherHighToo &&
                            isSecondSeriesCurrentValueHigherThanThisValue &&
                            isSecondSeriesThisValueLowerThanAllValuesInBetween &&
                            isCurrentValueHigherThanHalfOfValuesInBetween &&
                            isThisValueHigherThanHalfOfValuesInBetween)
                        {
                            Divergence divergence = new Divergence
                            {
                                StartIndex = i,
                                EndIndex = index,
                                Type = DivergenceType.Down
                            };

                            result.Add(divergence);
                        }

                        // Checking back all conditions of up divergence
                        if (isLowerLow &&
                            isThisValueLowerLow &&
                            isThisValueLowerThanAllValuesInBetween &&
                            isSecondSeriesThisValueLowerLowToo &&
                            isSecondSeriesCurrentValueLowerThanThisValue &&
                            isSecondSeriesThisValueHigherThanAllValuesInBetween &&
                            isCurrentValueLowerThanHalfOfValuesInBetween &&
                            isThisValueLowerThanHalfOfValuesInBetween)
                        {
                            Divergence divergence = new Divergence
                            {
                                StartIndex = i,
                                EndIndex = index,
                                Type = DivergenceType.Up
                            };

                            result.Add(divergence);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns sorted values of a dataseries in ascending order
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <returns>List<double></returns>
        public static List<double> Sort(this DataSeries dataSeries)
        {
            return dataSeries.ToList().OrderBy(value => value).ToList();
        }

        /// <summary>
        /// Returns a percentile value in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="percent">Percent (1-100)</param>
        /// <returns>double</returns>
        public static double GetPercentile(this DataSeries dataSeries, double percent)
        {
            List<double> sortedDataSeries = dataSeries.Sort();

            double percentReal = percent / 100;

            double entryIndex = percentReal * (sortedDataSeries.Count - 1) + 1;

            int entryIndexInt = (int)entryIndex;

            double indexDiff = entryIndex - entryIndexInt;

            if (entryIndexInt - 1 >= 0 && entryIndexInt < sortedDataSeries.Count)
            {
                double entryDataDiff = sortedDataSeries[entryIndexInt] - sortedDataSeries[entryIndexInt - 1];

                return sortedDataSeries[entryIndexInt - 1] + indexDiff * entryDataDiff;
            }
            else if (entryIndexInt - 1 < 0)
            {
                return sortedDataSeries.FirstOrDefault();
            }
            else if (entryIndexInt >= sortedDataSeries.Count)
            {
                return sortedDataSeries.LastOrDefault();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns a dataseries correlation with another dataseries
        /// </summary>
        /// <param name="firstSeries"></param>
        /// <param name="secondSeries">Other Dataseries</param>
        /// <returns>double</returns>
        public static double GetCorrelation(this DataSeries firstSeries, DataSeries secondSeries)
        {
            double[] values1 = new double[firstSeries.Count];
            double[] values2 = new double[firstSeries.Count];

            for (int i = 0; i < firstSeries.Count; i++)
            {
                values1[i] = firstSeries.Last(i);
                values2[i] = secondSeries.Last(i);
            }

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            double result = Math.Round(sum / Math.Sqrt(sumSqr1 * sumSqr2), 2) * 100;

            return result;
        }
    }
}