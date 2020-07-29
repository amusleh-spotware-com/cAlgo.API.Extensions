using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions
{
    public static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the covariance between two collection
        /// </summary>
        /// <param name="current">First collection</param>
        /// <param name="other">Second collection</param>
        /// <returns>double</returns>
        public static double Covariance(this IEnumerable<double> current, IEnumerable<double> other)
        {
            if (current.Count() != other.Count())
            {
                throw new InvalidOperationException("current and other collections count must be equal");
            }

            double xMean = current.Average();
            double yMean = other.Average();

            double xAndySum = current.Zip(other, (value1, value2) => (value1 - xMean) * (value2 - yMean)).Sum();

            return xAndySum / (current.Count() - 1);
        }

        /// <summary>
        /// Returns Pearson correlation coefficient between two collection
        /// </summary>
        /// <param name="x">First collection</param>
        /// <param name="y">Second collection</param>
        /// <returns>double</returns>
        public static double Correlation(this IEnumerable<double> x, IEnumerable<double> y)
        {
            if (!x.Any() || !y.Any())
            {
                return double.NaN;
            }

            double xSum = x.Sum();
            double ySum = y.Sum();

            double xSumSquared = Math.Pow(xSum, 2);
            double ySumSquared = Math.Pow(ySum, 2);

            double xSquaredSum = x.Select(value => Math.Pow(value, 2)).Sum();
            double ySquaredSum = y.Select(value => Math.Pow(value, 2)).Sum();

            double xAndyProductSum = x.Zip(y, (value1, value2) => value1 * value2).Sum();

            double n = x.Count();

            return ((n * xAndyProductSum) - (xSum * ySum)) / Math.Sqrt(((n * xSquaredSum) - xSumSquared) * ((n * ySquaredSum) - ySumSquared));
        }

        /// <summary>
        /// Returns the variance of a collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <param name="isSample">Set this True if its a sample not a population</param>
        /// <returns>double</returns>
        public static double Variance(this IEnumerable<double> collection, bool isSample = false)
        {
            if (!collection.Any())
            {
                return double.NaN;
            }

            int collectionCount = isSample ? collection.Count() - 1 : collection.Count();

            return collection.Select(value => Math.Pow(value - collection.Average(), 2)).Sum() / collectionCount;
        }

        /// <summary>
        /// Returns standard deviation of a collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <param name="isSample">Set this True if its a sample not a population</param>
        /// <returns>double</returns>
        public static double StandardDeviation(this IEnumerable<double> collection, bool isSample = false)
        {
            if (!collection.Any())
            {
                return double.NaN;
            }

            return Math.Sqrt(collection.Variance(isSample));
        }

        /// <summary>
        /// Returns median value of a collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>double</returns>
        public static double Median(this IEnumerable<double> collection)
        {
            if (!collection.Any())
            {
                return double.NaN;
            }

            List<double> sortedCollection = collection.OrderBy(value => value).ToList();

            int median = (int)(((sortedCollection.Count() + 1) / 2) - 1);

            return sortedCollection.Count() % 2 == 0 ? (sortedCollection[median] + sortedCollection[median + 1]) / 2 :
                sortedCollection[median];
        }

        /// <summary>
        /// Returns a percentile value in a collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <param name="percent">Percent (1-100)</param>
        /// <returns>double</returns>
        public static double Percentile(this IEnumerable<double> collection, double percent)
        {
            if (!collection.Any())
            {
                return double.NaN;
            }

            List<double> sortedCollection = collection.OrderBy(value => value).ToList();

            double percentReal = percent / 100;

            double entryIndex = percentReal * (sortedCollection.Count - 1) + 1;

            int entryIndexInt = (int)entryIndex;

            double indexDiff = entryIndex - entryIndexInt;

            if (entryIndexInt - 1 >= 0 && entryIndexInt < sortedCollection.Count)
            {
                double entryDataDiff = sortedCollection[entryIndexInt] - sortedCollection[entryIndexInt - 1];

                return sortedCollection[entryIndexInt - 1] + indexDiff * entryDataDiff;
            }
            else if (entryIndexInt - 1 < 0)
            {
                return sortedCollection.FirstOrDefault();
            }
            else if (entryIndexInt >= sortedCollection.Count)
            {
                return sortedCollection.LastOrDefault();
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Returns a value percentile rank in a collection
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <param name="value">The value</param>
        /// <returns>double</returns>
        public static double PercentileRank(this IEnumerable<double> collection, double value)
        {
            if (!collection.Any())
            {
                return double.NaN;
            }

            int numberOfValuesLessThanGivenValue = collection.Where(iValue => iValue <= value).Count();

            return 100.0 * numberOfValuesLessThanGivenValue / collection.Count();
        }

        /// <summary>
        /// Returns a collection range (max - min)
        /// </summary>
        /// <param name="collection">Collection</param>
        /// <returns>double</returns>
        public static double Range(this IEnumerable<double> collection) => collection.Max() - collection.Min();

        /// <summary>
        /// Returns cumulative sum of a collection items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector">A function that gets T and returns a double value</param>
        /// <param name="function">A funtion that gets a T and cumulative sum, and returns back a T</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> CumSum<T>(this IEnumerable<T> collection, Func<T, double> selector, Func<T, double, T> function)
        {
            double cum = 0;

            List<T> result = new List<T>();

            foreach (T item in collection)
            {
                cum += selector(item);

                result.Add(function(item, cum));
            }

            return result;
        }

        /// <summary>
        /// Returns cumulative product of a collection items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector">A function that gets T and returns a double value</param>
        /// <param name="function">A funtion that gets a T and cumulative product, and returns back a T</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> CumProduct<T>(this IEnumerable<T> collection, Func<T, double> selector, Func<T, double, T> function)
        {
            double cum = 1;

            List<T> result = new List<T>();

            foreach (T item in collection)
            {
                cum *= selector(item);

                result.Add(function(item, cum));
            }

            return result;
        }

        /// <summary>
        /// Returns difference of a collection items
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="selector">A function that gets T and returns a double value</param>
        /// <param name="function">A funtion that gets a T and difference amount, and returns back a T</param>
        /// <param name="lag">Number of lag to calculate the difference, default is 1</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> Diff<T>(this IEnumerable<T> collection, Func<T, double> selector, Func<T, double, T> function, int lag = 1)
        {
            List<T> collectionList = collection.ToList();

            List<T> result = new List<T>();

            for (int i = lag; i < collectionList.Count; i++)
            {
                double itemValue = selector(collectionList[i]);
                double lagItemValue = selector(collectionList[i - lag]);

                result.Add(function(collectionList[i], itemValue - lagItemValue));
            }

            return result;
        }

        /// <summary>
        /// Returns the percentage change or return between items in a collection
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="selector">A function that gets T and returns a double value</param>
        /// <param name="function">A function that gets a T and percentage change amount, and returns back a T</param>
        /// <param name="periods">The percentage change look back or lag number</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> PercentChange<T>(this IEnumerable<T> collection, Func<T, double> selector, Func<T, double, T> function,
            int periods = 1)
        {
            return collection.Skip(periods)
                .Zip(collection, (current, previous) =>
                {
                    double currnetValue = selector(current);
                    double previousValue = selector(previous);

                    double change = previousValue == 0 ? 0 : Math.Round(((currnetValue - previousValue) / previousValue) * 100, 2);

                    return function(current, change);
                });
        }

        /// <summary>
        /// Generates a bootstrap of the input data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection">Input data</param>
        /// <param name="count">The number of output data</param>
        /// <param name="seed">Random generator seed</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> Bootstrap<T>(this IEnumerable<T> collection, int count, int seed = 1)
        {
            Random random = new Random(seed);

            List<T> result = new List<T>();

            int maxIndex = collection.Count() - 1;

            List<T> collectionList = collection.ToList();

            for (int i = 1; i <= count; i++)
            {
                int index = random.Next(0, maxIndex);

                result.Add(collectionList[index]);
            }

            return result;
        }

        /// <summary>
        /// Changes the scale of data based on a minimum and maximum value
        /// </summary>
        /// <param name="collection"></param>
        /// <param name="minAllowed">The minimum allowed value</param>
        /// <param name="maxAllowed">The maximum allowed value</param>
        /// <param name="selector">A function that gets T and returns a double value</param>
        /// <param name="function">A funtion that gets a T and percentage change amount, and returns back a T</param>
        /// <returns>IEnumerable<T></returns>
        public static IEnumerable<T> MinMax<T>(this IEnumerable<T> collection, double minAllowed, double maxAllowed, Func<T, double> selector,
            Func<T, double, T> function)
        {
            double min = collection.Min(item => selector(item));
            double max = collection.Max(item => selector(item));

            double bValue = (max - min) != 0 ? max - min : 1 / max;

            List<T> result = new List<T>();

            foreach (T item in collection)
            {
                double itemValue = selector(item);
                double uninterpolate = (itemValue - min) / bValue;
                double itemScaledValue = minAllowed * (1 - uninterpolate) + maxAllowed * uninterpolate;

                T newItem = function(item, itemScaledValue);

                result.Add(newItem);
            }

            return result;
        }

        /// <summary>
        /// Returns the Pearson's coefficient of skewness
        /// </summary>
        /// <typeparam name="T">Data object type</typeparam>
        /// <param name="collection">The data collection</param>
        /// <param name="selector">The selector function that returns a double value from data object</param>
        /// <returns>double</returns>
        public static double Skewness<T>(this IEnumerable<T> collection, Func<T, double> selector)
        {
            IEnumerable<double> data = collection.Select(iDataPoint => selector(iDataPoint));

            var dataMean = data.Average();

            var dataMedian = data.Median();

            var dataStd = data.StandardDeviation();

            return (3 * (dataMean - dataMedian)) / dataStd;
        }

        /// <summary>
        /// Returns the xth moment about the mean
        /// </summary>
        /// <typeparam name="T">Data object type</typeparam>
        /// <param name="collection">The data collection</param>
        /// <param name="selector">The selector function that returns a double value from data object</param>
        /// <returns>double</returns>
        public static double MeanMoment<T>(this IEnumerable<T> collection, Func<T, double> selector, double moment)
        {
            IEnumerable<double> data = collection.Select(iDataPoint => selector(iDataPoint));

            var dataMean = data.Average();

            int count = data.Count();

            return data.Select(iDataPoint => Math.Pow(iDataPoint - dataMean, moment)).Sum() / count;
        }

        /// <summary>
        /// Returns the Pearson's kurtosis
        /// </summary>
        /// <typeparam name="T">Data object type</typeparam>
        /// <param name="collection">The data collection</param>
        /// <param name="selector">The selector function that returns a double value from data object</param>
        /// <returns>double</returns>
        public static double Kurtosis<T>(this IEnumerable<T> collection, Func<T, double> selector)
        {
            IEnumerable<double> data = collection.Select(iDataPoint => selector(iDataPoint));

            return collection.MeanMoment(selector, 4) / Math.Pow(data.Variance(), 2);
        }
    }
}