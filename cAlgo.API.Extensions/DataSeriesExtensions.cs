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
        /// Checks if the index value is higher than x (periods) previous values in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="periods">The number of index previous values to check</param>
        /// <param name="bothSides">Check both previous and next values (by default it checks only previous values)</param>
        /// <returns>bool</returns>
        public static bool IsHigherHigh(this DataSeries dataSeries, int index, int periods, bool bothSides = false)
        {
            if (!bothSides)
            {
                return dataSeries[index] > dataSeries.Maximum(index - periods, index - 1);
            }
            else
            {
                bool leftSideResult = dataSeries[index] > dataSeries.Maximum(index - periods, index - 1);
                bool rightSideResult = dataSeries[index] > dataSeries.Maximum(index + 1, index + periods);

                return leftSideResult && rightSideResult;
            }
        }

        /// <summary>
        /// Checks if the index value is lower than x (periods) previous values in a dataseries
        /// </summary>
        /// <param name="dataSeries"></param>
        /// <param name="index">Dataseries value index</param>
        /// <param name="periods">The number of index previous values to check</param>
        /// <param name="bothSides">Check both previous and next values (by default it checks only previous values)</param>
        /// <returns>bool</returns>
        public static bool IsLowerLow(this DataSeries dataSeries, int index, int periods, bool bothSides = false)
        {
            if (!bothSides)
            {
                return dataSeries[index] < dataSeries.Minimum(index - periods, index - 1);
            }
            else
            {
                bool leftSideResult = dataSeries[index] < dataSeries.Minimum(index - periods, index - 1);
                bool rightSideResult = dataSeries[index] < dataSeries.Minimum(index + 1, index + periods);

                return leftSideResult && rightSideResult;
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
    }
}