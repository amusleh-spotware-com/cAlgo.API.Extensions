using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions
{
    public static class DataSeriesExtensions
    {
        /// <summary>
        /// Creates a List from a DataSeries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <returns>List</returns>
        public static List<double> ToList(this DataSeries dataSeries)
        {
            var data = new List<double>();

            for (var i = 0; i < dataSeries.Count; i++)
            {
                data.Add(dataSeries[i]);
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
            var max = double.NegativeInfinity;

            for (var i = startIndex; i <= endIndex; i++)
            {
                max = Math.Max(dataSeries[i], max);
            }

            return max;
        }

        /// <summary>
        /// Returns the bar index that has the maximum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static int MaximumBarIndex(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            var max = double.NegativeInfinity;

            var maxBarIndex = startIndex;

            for (var i = startIndex; i <= endIndex; i++)
            {
                if (dataSeries[i] >= max)
                {
                    max = dataSeries[i];
                    maxBarIndex = i;
                }
            }

            return maxBarIndex;
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
            var min = double.PositiveInfinity;

            for (var i = startIndex; i <= endIndex; i++)
            {
                min = Math.Min(dataSeries[i], min);
            }

            return min;
        }

        /// <summary>
        /// Returns the bar index that has the minimum value between start and end (inclusive) index in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">Start index (Ex: 1)</param>
        /// <param name="endIndex">End index (Ex: 10)</param>
        /// <returns>double</returns>
        public static int MinimumBarIndex(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            var min = double.PositiveInfinity;

            var minBarIndex = startIndex;

            for (var i = startIndex; i <= endIndex; i++)
            {
                if (dataSeries[i] <= min)
                {
                    min = dataSeries[i];
                    minBarIndex = i;
                }
            }

            return minBarIndex;
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
        public static bool IsHigher(
            this DataSeries dataSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            var previousBarsHighest = previousValues > 0 ? dataSeries.Maximum(index - previousValues, index - 1) : double.NegativeInfinity;
            var futureBarsHighest = futureValues > 0 ? dataSeries.Maximum(index + 1, index + futureValues) : double.NegativeInfinity;

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
        public static bool IsHigher(
            this DataSeries dataSeries, DataSeries otherSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            var previousBarsHighest = previousValues > 0 ? otherSeries.Maximum(index - previousValues, index - 1) : double.NegativeInfinity;
            var futureBarsHighest = futureValues > 0 ? otherSeries.Maximum(index + 1, index + futureValues) : double.NegativeInfinity;

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
        public static bool IsLower(
            this DataSeries dataSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            var previousBarsLowest = previousValues > 0 ? dataSeries.Minimum(index - previousValues, index - 1) : double.PositiveInfinity;
            var futureBarsLowest = futureValues > 0 ? dataSeries.Minimum(index + 1, index + futureValues) : double.PositiveInfinity;

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
        public static bool IsLower(
            this DataSeries dataSeries, DataSeries otherSeries, int index, int previousValues = 0, int futureValues = 0, bool equal = true)
        {
            var previousBarsLowest = previousValues > 0 ? otherSeries.Minimum(index - previousValues, index - 1) : double.PositiveInfinity;
            var futureBarsLowest = futureValues > 0 ? otherSeries.Minimum(index + 1, index + futureValues) : double.PositiveInfinity;

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
            var highValue = dataSeries.Maximum(index - periods, index - 1);

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
            var lowValue = dataSeries.Minimum(index - periods, index - 1);

            return dataSeries[index] - lowValue;
        }

        /// <summary>
        /// Returns the divergences between two data series based on provided index
        /// </summary>
        /// <param name="firstSeries">The first data series</param>
        /// <param name="secondSeries">The second data series</param>
        /// <param name="index">Index of the value you want to get its divergences</param>
        /// <param name="periods">This number of previous values from index will be checked to find divergence in both data series</param>
        /// <param name="minDistance">The minimum distance in bars between start and end of divergence</param>
        /// <returns>List of divergences</returns>
        public static List<Divergence> GetDivergence(
            this DataSeries firstSeries, DataSeries secondSeries, int index, int periods, int minDistance)
        {
            var result = new List<Divergence>();

            for (var i = index - minDistance; i >= index - periods; i--)
            {
                var isDiverged = firstSeries.IsDiverged(secondSeries, i, index);

                if (!isDiverged)
                {
                    continue;
                }

                var isHigherHigh = firstSeries.IsHigher(i, minDistance);
                var isLowerLow = firstSeries.IsLower(i, minDistance);

                if (firstSeries[i] < firstSeries[index] && firstSeries.IsConnectionPossible(i, index, Direction.Up) &&
                    secondSeries.IsConnectionPossible(i, index, Direction.Up) && isLowerLow)
                {
                    var divergence = new Divergence
                    {
                        StartIndex = i,
                        EndIndex = index,
                        Type = DivergenceType.Up
                    };

                    result.Add(divergence);
                }
                else if (firstSeries[i] > firstSeries[index] && firstSeries.IsConnectionPossible(i, index, Direction.Down) &&
                    secondSeries.IsConnectionPossible(i, index, Direction.Down) && isHigherHigh)
                {
                    var divergence = new Divergence
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
        /// <param name="startIndex">The first point index in data series</param>
        /// <param name="endIndex">The second point index in data series</param>
        /// <returns></returns>
        public static bool IsDiverged(this DataSeries firstSeries, DataSeries secondSeries, int startIndex, int endIndex)
        {
            if (startIndex >= endIndex)
            {
                throw new ArgumentException("The 'startIndex' must be less than 'secondPointIndex'");
            }

            if (firstSeries[startIndex] >= firstSeries[endIndex] && secondSeries[startIndex] < secondSeries[endIndex])
            {
                return true;
            }

            if (firstSeries[startIndex] <= firstSeries[endIndex] && secondSeries[startIndex] > secondSeries[endIndex])
            {
                return true;
            }

            if (firstSeries[startIndex] > firstSeries[endIndex] && secondSeries[startIndex] <= secondSeries[endIndex])
            {
                return true;
            }

            if (firstSeries[startIndex] < firstSeries[endIndex] && secondSeries[startIndex] >= secondSeries[endIndex])
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns True if connecting two provided data point based on cross side is possible otherwise False
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">The first point index in data series</param>
        /// <param name="endIndex">The second point index in data series</param>
        /// <param name="direction">The line direction, is it on up direction or low direction?</param>
        /// <returns>bool</returns>
        public static bool IsConnectionPossible(this DataSeries dataSeries, int startIndex, int endIndex, Direction direction)
        {
            if (startIndex >= endIndex)
            {
                throw new ArgumentException("The 'startIndex' must be less than 'secondPointIndex'");
            }

            var slope = dataSeries.GetSlope(startIndex, endIndex);

            var counter = 0;

            for (var i = startIndex + 1; i <= endIndex; i++)
            {
                counter++;

                if (direction == Direction.Up && dataSeries[i] < dataSeries[startIndex] + slope * counter)
                {
                    return false;
                }
                else if (direction == Direction.Down && dataSeries[i] > dataSeries[startIndex] + slope * counter)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the amount of slope between two level in a data series
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">The first point index in data series</param>
        /// <param name="endIndex">The second point index in data series</param>
        /// <returns>double</returns>
        public static double GetSlope(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            return (dataSeries[endIndex] - dataSeries[startIndex]) / (endIndex - startIndex);
        }

        /// <summary>
        /// Returns sorted values of a dataseries in ascending order
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <returns>List of sorted data values</returns>
        public static List<double> Sort(this DataSeries dataSeries)
        {
            return dataSeries.ToList().OrderBy(value => value).ToList();
        }

        /// <summary>
        /// Returns a percentile value in a data series
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="percent">Percent (1-100)</param>
        /// <param name="periods">The x number of previous values that will be used for calculation</param>
        /// <returns>double</returns>
        public static double GetPercentile(this DataSeries dataSeries, double percent, int periods)
        {
            var data = dataSeries.ToList().Skip(dataSeries.Count - periods).OrderBy(value => value).ToList();

            var percentFraction = percent / 100;

            var entryIndex = percentFraction * (data.Count - 1) + 1;

            var entryIndexInt = (int)entryIndex;

            var indexDiff = entryIndex - entryIndexInt;

            if (entryIndexInt - 1 >= 0 && entryIndexInt < data.Count)
            {
                var entryDataDiff = data[entryIndexInt] - data[entryIndexInt - 1];

                return data[entryIndexInt - 1] + indexDiff * entryDataDiff;
            }
            else if (entryIndexInt - 1 < 0)
            {
                return data.FirstOrDefault();
            }
            else if (entryIndexInt >= data.Count)
            {
                return data.LastOrDefault();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns a dataseries correlation with another dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="otherDataSeries">Other Dataseries</param>
        /// <param name="periods">The x previous values number</param>
        /// <param name="lag">The number of values that will be skipped from the other series</param>
        /// <param name="percentageChange">Should it use the series percentage change instead raw values</param>
        /// <returns>double</returns>
        public static double GetCorrelation(this DataSeries dataSeries, DataSeries otherDataSeries, int periods, int lag, bool percentageChange)
        {
            var firstSeries = new double[periods];
            var secondSeries = new double[periods];

            for (var i = 0; i < periods; i++)
            {
                if (percentageChange)
                {
                    firstSeries[i] = double.IsNaN(dataSeries.Last(i + 1)) ? 0 : (dataSeries.Last(i) - dataSeries.Last(i + 1)) / dataSeries.Last(i + 1) * 100;
                    secondSeries[i] = double.IsNaN(otherDataSeries.Last(i + 1 + lag)) ? 0 : (otherDataSeries.Last(i + lag) - otherDataSeries.Last(i + 1 + lag)) / otherDataSeries.Last(i + 1 + lag) * 100;
                }
                else
                {
                    firstSeries[i] = dataSeries.Last(i);
                    secondSeries[i] = otherDataSeries.Last(i + lag);
                }
            }

            var avg1 = firstSeries.Average();
            var avg2 = secondSeries.Average();

            var sum = firstSeries.Zip(secondSeries, (x1, y1) => (x1 - avg1) * (y1 - avg2)).Sum();

            var sumSqr1 = firstSeries.Sum(x => Math.Pow(x - avg1, 2.0));
            var sumSqr2 = secondSeries.Sum(y => Math.Pow(y - avg2, 2.0));

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
            var yDiff = dataSeries[index] - dataSeries[index - periods];

            return Math.Atan2(yDiff, xDiff) * 180.0 / Math.PI;
        }

        /// <summary>
        /// Returns the direction of trend if there is any in between start and end index bars
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="startIndex">The start bar index in a data Series</param>
        /// <param name="endIndex">The end bar index in a data series</param>
        /// <param name="step">The step that will be used for comparison of bars</param>
        /// <returns>Direction</returns>
        public static Direction GetTrendDirection(this DataSeries dataSeries, int startIndex, int endIndex, int step = 3)
        {
            var isTrendingUp = true;
            var isTrendingDown = true;

            for (var i = startIndex + step; i <= endIndex; i += step)
            {
                if (isTrendingDown && dataSeries[i] >= dataSeries[i - step])
                {
                    isTrendingDown = false;
                }

                if (isTrendingUp && dataSeries[i] <= dataSeries[i - step])
                {
                    isTrendingUp = false;
                }
            }

            if (isTrendingUp)
            {
                return Direction.Up;
            }
            else if (isTrendingDown)
            {
                return Direction.Down;
            }
            else
            {
                return Direction.None;
            }
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
            var data = new List<double>();

            for (var i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            var average = data.Average();

            double n = endIndex - startIndex - 1;

            if (n <= 0)
            {
                n = 1;
            }

            return data.Select(value => Math.Pow(value - average, 2)).Sum() / n;
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
            var variance = dataSeries.GetVariance(startIndex, endIndex);

            return Math.Sqrt(variance);
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
            var data = new List<double>();

            for (var i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            data.Sort();

            var median = (data.Count() + 1) / 2 - 1;

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
            var data = new List<double>();

            for (var i = startIndex; i <= endIndex; i++)
            {
                data.Add(dataSeries[i]);
            }

            return data.Max() - data.Min();
        }

        /// <summary>
        /// Returns the slope and intercept by using the least squares method
        /// </summary>
        /// <param name="dataSeries">Data series</param>
        /// <param name="startIndex">The first point index</param>
        /// <param name="endIndex">The second point index</param>
        /// <returns>LeastSquares</returns>
        public static LeastSquares GetLeastSquaresRegression(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            var xValues = new List<int>();
            var yValues = new List<double>();

            for (var x = startIndex; x <= endIndex; x++)
            {
                xValues.Add(x);
                yValues.Add(dataSeries[x]);
            }

            var xSquared = xValues.Select(x => Math.Pow(x, 2)).ToList();
            var xyProducts = xValues.Zip(yValues, (x, y) => x * y).ToList();

            double xSum = xValues.Sum();
            var ySum = yValues.Sum();

            var xSqauredSum = xSquared.Sum();

            var xyProductsSum = xyProducts.Sum();

            var n = xValues.Count;

            var slope = (n * xyProductsSum - xSum * ySum) / (n * xSqauredSum - Math.Pow(xSum, 2));
            var intercept = (ySum - slope * xSum) / n;

            return new LeastSquares { Slope = slope, Intercept = intercept };
        }

        /// <summary>
        /// Returns the sum of values between to index
        /// </summary>
        /// <param name="dataSeries">The data series</param>
        /// <param name="startIndex">The first index</param>
        /// <param name="endIndex">The second index</param>
        /// <returns>double</returns>
        public static double Sum(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            double sum = 0;

            for (var iIndex = startIndex; iIndex <= endIndex; iIndex++)
            {
                sum += dataSeries[iIndex];
            }

            return sum;
        }

        /// <summary>
        /// Returns the percentage change amount between two value on a data series
        /// </summary>
        /// <param name="dataSeries">The data series</param>
        /// <param name="startIndex">The first index</param>
        /// <param name="endIndex">The second index</param>
        /// <returns>double</returns>
        public static double PercentageChange(this DataSeries dataSeries, int startIndex, int endIndex)
        {
            return (dataSeries[endIndex] - dataSeries[startIndex]) / dataSeries[startIndex] * 100;
        }

    }
}