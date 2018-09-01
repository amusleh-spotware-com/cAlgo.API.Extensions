using cAlgo.API;
using cAlgo.API.Extensions.Types;
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Checks if the index value is higher than x previous and future values in a data series
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
        /// Checks if the index value is higher than x previous and future values of another data series
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="otherSeries">Other data series</param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="previousValues">The number of index previous values to check</param>
        /// <param name="futureValues">The number of index future values to check</param>
        /// <param name="equal">Check for equality</param>
        /// <returns>bool</returns>
        public static bool IsHigherHigh(
            this DataSeries dataSeries, DataSeries otherSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            double previousBarsHighest = previousValues > 0 ? otherSeries.Maximum(index - previousValues, index - 1) : double.NegativeInfinity;
            double futureBarsHighest = futureValues > 0 ? otherSeries.Maximum(index + 1, index + futureValues) : double.NegativeInfinity;

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
        /// Checks if the index value is lower than x previous and future values in a data series
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
            double previousBarsLowest = previousValues > 0 ? dataSeries.Minimum(index - previousValues, index - 1) : double.PositiveInfinity;
            double futureBarsLowest = futureValues > 0 ? dataSeries.Minimum(index + 1, index + futureValues) : double.PositiveInfinity;

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
        /// Checks if the index value is lower than x previous and future values of another data series
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="otherSeries">Other data series</param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="previousValues">The number of index previous values to check</param>
        /// <param name="futureValues">The number of index future values to check</param>
        /// <param name="equal">Check for equality</param>
        /// <returns>bool</returns>
        public static bool IsLowerLow(
            this DataSeries dataSeries, DataSeries otherSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            double previousBarsLowest = previousValues > 0 ? otherSeries.Minimum(index - previousValues, index - 1) : double.PositiveInfinity;
            double futureBarsLowest = futureValues > 0 ? otherSeries.Minimum(index + 1, index + futureValues) : double.PositiveInfinity;

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
        /// Returns the divergences between two data series based on provided index
        /// </summary>
        /// <param name="firstSeries">The first data series</param>
        /// <param name="secondSeries">The second data series</param>
        /// <param name="index">Index of the value you want to get its divergences</param>
        /// <param name="periods">This number of previous values from index will be checked to find divergence in both data series</param>
        /// <returns>List<Divergence></returns>
        public static List<Divergence> GetDivergence(
            this DataSeries firstSeries, DataSeries secondSeries, int index, int periods = 200, int lag = 20)
        {
            List<Divergence> result = new List<Divergence>();

            for (int i = index - lag; i >= index - periods; i--)
            {
                bool isDiverged = firstSeries.IsDiverged(secondSeries, i, index);

                if (!isDiverged)
                {
                    continue;
                }

                if (firstSeries[i] < firstSeries[index] && firstSeries.IsConnectionPossible(i, index, LineSide.Down) &&
                    secondSeries.IsConnectionPossible(i, index, LineSide.Down))
                {
                    Divergence divergence = new Divergence
                    {
                        StartIndex = i,
                        EndIndex = index,
                        Type = DivergenceType.Up
                    };

                    result.Add(divergence);
                }
                else if (firstSeries[i] > firstSeries[index] && firstSeries.IsConnectionPossible(i, index, LineSide.Up) &&
                    secondSeries.IsConnectionPossible(i, index, LineSide.Up))
                {
                    Divergence divergence = new Divergence
                    {
                        StartIndex = i,
                        EndIndex = index,
                        Type = DivergenceType.Down
                    };

                    result.Add(divergence);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns True if data series moved in cross direction on firstPointIndex and secondPointIndex
        /// </summary>
        /// <param name="firstSeries">The first data series</param>
        /// <param name="secondSeries">The second data series</param>
        /// <param name="firstPointIndex">The first point index in data series</param>
        /// <param name="secondPointIndex">The second point index in data series</param>
        /// <returns></returns>
        public static bool IsDiverged(this DataSeries firstSeries, DataSeries secondSeries, int firstPointIndex, int secondPointIndex)
        {
            if (firstPointIndex >= secondPointIndex)
            {
                throw new ArgumentException("The 'firstPointIndex' must be less than 'secondPointIndex'");
            }

            if (firstSeries[firstPointIndex] > firstSeries[secondPointIndex] && secondSeries[firstPointIndex] < secondSeries[secondPointIndex])
            {
                return true;
            }
            else if (firstSeries[firstPointIndex] < firstSeries[secondPointIndex] && secondSeries[firstPointIndex] > secondSeries[secondPointIndex])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns True if connecting two provided data point based on cross side is possible otherwise False
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="firstPointIndex">The first point index in data series</param>
        /// <param name="secondPointIndex">The second point index in data series</param>
        /// <param name="side">The line side, is it on upper side or lower side?</param>
        /// <returns>bool</returns>
        public static bool IsConnectionPossible(this DataSeries dataSeries, int firstPointIndex, int secondPointIndex, LineSide side)
        {
            if (firstPointIndex >= secondPointIndex)
            {
                throw new ArgumentException("The 'firstPointIndex' must be less than 'secondPointIndex'");
            }

            double slope = (dataSeries[secondPointIndex] - dataSeries[firstPointIndex]) / (secondPointIndex - firstPointIndex);

            int counter = 0;

            for (int i = firstPointIndex + 1; i < secondPointIndex; i++)
            {
                counter++;

                if (side == LineSide.Up && dataSeries[i] > dataSeries[firstPointIndex] + (slope * counter))
                {
                    return false;
                }
                else if (side == LineSide.Down && dataSeries[i] < dataSeries[firstPointIndex] + (slope * counter))
                {
                    return false;
                }
            }

            return true;
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
        /// <param name="otherDataSeries">Other Dataseries</param>
        /// <returns>double</returns>
        public static double GetCorrelation(this DataSeries dataSeries, DataSeries otherDataSeries)
        {
            double[] values1 = new double[dataSeries.Count];
            double[] values2 = new double[dataSeries.Count];

            for (int i = 0; i < dataSeries.Count; i++)
            {
                values1[i] = dataSeries.Last(i);
                values2[i] = otherDataSeries.Last(i);
            }

            var avg1 = values1.Average();
            var avg2 = values2.Average();

            var sum = values1.Zip(values2, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = values1.Sum(x => Math.Pow((x - avg1), 2.0));
            var sumSqr2 = values2.Sum(y => Math.Pow((y - avg2), 2.0));

            return Math.Round(sum / Math.Sqrt(sumSqr1 * sumSqr2), 2);
        }

        /// <summary>
        /// Returns the angle of a value (index) in a dataseries with it's x (periods) previous values
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">The value index</param>
        /// <param name="periods">The number of previous values from index</param>
        /// <returns>double</returns>
        public static double GetAngle(this DataSeries dataSeries, int index, int periods)
        {
            double xDiff = index - (index - periods);
            double yDiff = dataSeries[index] - dataSeries[index - periods];

            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        /// <summary>
        /// Returns true if the bars between start and end index trending up otherwise false
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">The start bar index in a data Series</param>
        /// <param name="endIndex">The end bar index in a data series</param>
        /// <param name="step">The step that will be used for comparison of bars</param>
        /// <returns>bool</returns>
        public static bool IsTrendingUp(this DataSeries dataSeries, int startIndex, int endIndex, int step = 3)
        {
            bool result = true;

            for (int i = startIndex + step; i <= endIndex; i += step)
            {
                if (dataSeries[i] <= dataSeries[i - step])
                {
                    result = false;

                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns true if the bars between start and end index trending down otherwise false
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">The start bar index in a data series</param>
        /// <param name="endIndex">The end bar index in a data series</param>
        /// <param name="step">The step that will be used for comparison of bars</param>
        /// <returns>bool</returns>
        public static bool IsTrendingDown(this DataSeries dataSeries, int startIndex, int endIndex, int step = 3)
        {
            bool result = true;

            for (int i = startIndex + step; i <= endIndex; i += step)
            {
                if (dataSeries[i] >= dataSeries[i - step])
                {
                    result = false;

                    break;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the variance between an index interval in a data series
        /// </summary>
        /// <param name="dataSeries">Data series</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>double</returns>
        public static double GetVariance(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            List<double> data = new List<double>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            return data.Select(value => Math.Pow(value - data.Average(), 2)).Sum() / (endIndex - startIndex);
        }

        /// <summary>
        /// Returns the standard deviation between an index interval in a data series
        /// </summary>
        /// <param name="dataSeries">Data series</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>double</returns>
        public static double GetStandardDeviation(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            return Math.Sqrt(dataSeries.GetVariance(startIndex, endIndex));
        }

        /// <summary>
        /// Returns the median value between an index interval in a data series
        /// </summary>
        /// <param name="dataSeries">Data series</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>double</returns>
        public static double GetMedian(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            List<double> data = new List<double>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            data.Sort();

            int median = (int)(((data.Count() + 1) / 2) - 1);

            return data.Count() % 2 == 0 ? (data[median] + data[median + 1]) / 2 : data[median];
        }

        /// <summary>
        /// Returns the range value between an index interval in a data series
        /// </summary>
        /// <param name="dataSeries">Data series</param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>double</returns>
        public static double GetRange(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            List<double> data = new List<double>();

            for (int i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            return data.Max() - data.Min();
        }
    }
}