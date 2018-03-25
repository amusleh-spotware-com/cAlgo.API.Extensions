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

        public static List<Divergence> GetDivergence(
            this DataSeries firstSeries, int index, DataSeries secondSeries, int periods = 20, int lag = 3, int maxValuesToScan = 100)
        {
            List<Divergence> result = new List<Divergence>();

            // Subtracting the lag number from index
            index = index - lag;

            // First Condition For Up Divergence: The current value must be higher than x (periods) previous and y (lag) after values
            bool isHigherHigh = firstSeries.IsHigherHigh(index, periods, lag);

            // First Condition For Down Divergence: The current value must be lower than x (periods) previous and y (lag) after values
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
                        bool IsSecondSeriesThisValueLowerThanAllValuesInBetween = secondSeries[i] < secondSeries.Maximum(i + 1, index);

                        // Sixth Condition For Up Divergence: Is the second series this (i) value higher than lowest value up to current
                        // value (index)
                        bool IsSecondSeriesThisValueHigherThanAllValuesInBetween = secondSeries[i] > secondSeries.Minimum(i + 1, index);

                        // Checking back all conditions of down divergence
                        if (isHigherHigh &&
                            isThisValueHigherHigh &&
                            isThisValueHigherThanAllValuesInBetween &&
                            isSecondSeriesThisValueHigherHighToo &&
                            isSecondSeriesCurrentValueHigherThanThisValue &&
                            IsSecondSeriesThisValueLowerThanAllValuesInBetween)
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
                            IsSecondSeriesThisValueHigherThanAllValuesInBetween)
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
    }
}