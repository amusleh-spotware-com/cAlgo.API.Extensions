using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions
{
    public static class BarsExtensions
    {
        /// <summary>
        /// Returns the last bar index in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <returns>int</returns>
        public static int GetIndex(this Bars bars)
        {
            return bars.ClosePrices.Count > 0 ? bars.ClosePrices.Count - 1 : bars.ClosePrices.Count;
        }

        /// <summary>
        /// Returns the volume profile of x latest bars in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Last Bar Index</param>
        /// <param name="periods">Number of previous bars before provided index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <returns>List<PriceVolume></returns>
        public static List<PriceLevel> GetVolumeProfile(this Bars bars, int index, int periods, Symbol symbol)
        {
            List<PriceLevel> result = new List<PriceLevel>();

            for (int i = index; i > index - periods; i--)
            {
                double barRange = bars.GetBarRange(i);

                double barVolume = bars.TickVolumes[i];

                if (barRange <= 0 || barVolume <= 0)
                {
                    continue;
                }

                double percentageAboveBarClose = (bars.HighPrices[i] - bars.ClosePrices[i]) / barRange;
                double percentageBelowBarClose = (bars.ClosePrices[i] - bars.LowPrices[i]) / barRange;

                double bullishVolume = barVolume * percentageBelowBarClose;
                double bearishVolume = barVolume * percentageAboveBarClose;

                double barRangeInPips = symbol.ToPips(barRange);

                double bullishVolumePerPips = bullishVolume / barRangeInPips;
                double bearishVolumePerPips = bearishVolume / barRangeInPips;

                for (double level = bars.LowPrices[i]; level <= bars.HighPrices[i]; level += symbol.PipSize)
                {
                    level = Math.Round(level, symbol.Digits);

                    PriceLevel priceLevel = result.FirstOrDefault(pLevel => pLevel.Level == level);

                    if (priceLevel == null)
                    {
                        priceLevel = new PriceLevel
                        {
                            Level = level
                        };

                        result.Add(priceLevel);
                    }

                    priceLevel.BullishVolume += bullishVolumePerPips;
                    priceLevel.BearishVolume += bearishVolumePerPips;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the amount of percentage change of a value in comparison with it's previous value in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The value index</param>
        /// <returns>double</returns>
        public static double GetPercentageChange(this Bars bars, int index)
        {
            return -((bars.OpenPrices[index] - bars.ClosePrices[index]) / bars.OpenPrices[index]) * 100;
        }

        /// <summary>
        /// Returns the bar type
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Index of bar</param>
        /// <returns>BarType</returns>
        public static BarType GetBarType(this Bars bars, int index)
        {
            if (bars.ClosePrices[index] > bars.OpenPrices[index])
            {
                return BarType.Bullish;
            }
            else if (bars.ClosePrices[index] < bars.OpenPrices[index])
            {
                return BarType.Bearish;
            }
            else
            {
                return BarType.Neutral;
            }
        }

        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this Bars bars, int index, bool useBarBody = false)
        {
            return useBarBody ? Math.Abs(bars.OpenPrices[index] - bars.ClosePrices[index])
                : bars.HighPrices[index] - bars.LowPrices[index];
        }

        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="symbol">The market series symbol</param>
        /// <param name="returnType">The return type</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this Bars bars, int index, Symbol symbol, PriceValueType returnType, bool useBarBody = false)
        {
            double range = bars.GetBarRange(index, useBarBody);

            return symbol.ChangePriceValueType(range, returnType);
        }

        /// <summary>
        /// Returns the maximum bar range in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMaxBarRange(this Bars bars, int index, int periods, bool useBarBody = false)
        {
            double maxRange = double.MinValue;

            for (int i = index; i >= index - periods; i--)
            {
                maxRange = Math.Max(maxRange, bars.GetBarRange(i, useBarBody: useBarBody));
            }

            return maxRange;
        }

        /// <summary>
        /// Returns the maximum bar range in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMaxBarRange(this Bars bars, int index, int periods, BarType barType, bool useBarBody = false)
        {
            double maxRange = double.MinValue;

            for (int i = index; i >= index - periods; i--)
            {
                if (bars.GetBarType(i) != barType)
                {
                    continue;
                }

                maxRange = Math.Max(maxRange, bars.GetBarRange(i, useBarBody: useBarBody));
            }

            return maxRange;
        }

        /// <summary>
        /// Returns the minimum bar range in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMinBarRange(this Bars bars, int index, int periods, bool useBarBody = false)
        {
            double minRange = double.MaxValue;

            for (int i = index; i >= index - periods; i--)
            {
                minRange = Math.Min(minRange, bars.GetBarRange(i, useBarBody: useBarBody));
            }

            return minRange;
        }

        /// <summary>
        /// Returns the minimum bar range in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMinBarRange(this Bars bars, int index, int periods, BarType barType, bool useBarBody = false)
        {
            double minRange = double.MaxValue;

            for (int i = index; i >= index - periods; i--)
            {
                if (bars.GetBarType(i) != barType)
                {
                    continue;
                }

                minRange = Math.Min(minRange, bars.GetBarRange(i, useBarBody: useBarBody));
            }

            return minRange;
        }

        /// <summary>
        /// Returns True if the index bar is an engulfing bar
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsEngulfingBar(this Bars bars, int index)
        {
            double barBodyRange = bars.GetBarRange(index, true);
            double previousBarRange = bars.GetBarRange(index - 1);

            BarType barType = bars.GetBarType(index);
            BarType previousBarType = bars.GetBarType(index - 1);

            return barBodyRange > previousBarRange && barType != previousBarType;
        }

        /// <summary>
        /// Returns True if the index bar is a rejection bar
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsRejectionBar(this Bars bars, int index)
        {
            double barBodyRange = bars.GetBarRange(index, true);
            double barRange = bars.GetBarRange(index);

            BarType barType = bars.GetBarType(index);

            double meanBarRange = bars.GetAverageBarRange(index - 1, 50);

            if (barBodyRange / barRange < 0.3 && barRange > meanBarRange)
            {
                double barMiddle = (barRange * 0.5) + bars.LowPrices[index];
                double barFirstQuartile = (barRange * 0.25) + bars.LowPrices[index];
                double barThirdQuartile = (barRange * 0.75) + bars.LowPrices[index];

                if ((bars.OpenPrices[index] > barMiddle && bars.ClosePrices[index] > barThirdQuartile && barType == BarType.Bullish) ||
                    (bars.OpenPrices[index] < barMiddle && bars.ClosePrices[index] < barFirstQuartile && barType == BarType.Bearish))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns True if the index bar is a doji bar
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsDojiBar(this Bars bars, int index)
        {
            double barBodyRange = bars.GetBarRange(index, true);
            double barRange = bars.GetBarRange(index);

            double meanBarRange = bars.GetAverageBarRange(index - 1, 50);

            return barRange < meanBarRange / 3 && barBodyRange / barRange < 0.5;
        }

        /// <summary>
        /// Returns True if the index bar is an inside bar
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsInsideBar(this Bars bars, int index)
        {
            BarType barType = bars.GetBarType(index);
            BarType previousBarType = bars.GetBarType(index - 1);

            if (bars.HighPrices[index] < bars.HighPrices[index - 1] &&
                bars.LowPrices[index] > bars.LowPrices[index - 1] &&
                barType != previousBarType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns True if the index bar is a three bar reversal
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsThreeBarReversal(this Bars bars, int index)
        {
            bool result = false;

            BarType barType = bars.GetBarType(index);
            BarType previousBarType = bars.GetBarType(index - 1);

            if (barType == BarType.Bullish && previousBarType == BarType.Bearish && bars.GetBarType(index - 2) == BarType.Bearish)
            {
                if (bars.LowPrices[index - 1] < bars.LowPrices[index - 2] && bars.LowPrices[index - 1] < bars.LowPrices[index])
                {
                    if (bars.ClosePrices[index] > bars.OpenPrices[index - 1])
                    {
                        result = true;
                    }
                }
            }
            else if (barType == BarType.Bearish && previousBarType == BarType.Bullish && bars.GetBarType(index - 2) == BarType.Bullish)
            {
                if (bars.HighPrices[index - 1] > bars.HighPrices[index - 2] && bars.HighPrices[index - 1] > bars.HighPrices[index])
                {
                    if (bars.ClosePrices[index] < bars.OpenPrices[index - 1])
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the candle type of given bar index
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>List<CandlePattern></returns>
        public static List<CandlePattern> GetCandlePatterns(this Bars bars, int index)
        {
            List<CandlePattern> patterns = new List<CandlePattern>();

            // Engulfing
            if (bars.IsEngulfingBar(index))
            {
                patterns.Add(CandlePattern.Engulfing);
            }

            // Rejection
            if (bars.IsRejectionBar(index))
            {
                patterns.Add(CandlePattern.Rejection);
            }

            // Doji
            if (bars.IsDojiBar(index))
            {
                patterns.Add(CandlePattern.Doji);
            }

            // InsideBar
            if (bars.IsInsideBar(index))
            {
                patterns.Add(CandlePattern.InsideBar);
            }

            // Three Reversal Bars
            if (bars.IsThreeBarReversal(index))
            {
                patterns.Add(CandlePattern.ThreeBarReversal);
            }

            return patterns;
        }

        /// <summary>
        /// Returns True if the provided bar matches any of the provided patterns otherwise false
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <param name="patternsToMatch">List of candle patterns to match</param>
        /// <returns>bool</returns>
        public static bool IsCandlePatternMatchesAny(this Bars bars, int index, List<CandlePattern> patternsToMatch)
        {
            List<CandlePattern> barPatterns = bars.GetCandlePatterns(index);

            return patternsToMatch.Any(pattern => barPatterns.Contains(pattern));
        }

        /// <summary>
        /// Returns True if the provided bar matches all of the provided patterns otherwise false
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <param name="patternsToMatch">List of candle patterns to match</param>
        /// <returns>bool</returns>
        public static bool IsCandlePatternMatchesAll(this Bars bars, int index, List<CandlePattern> patternsToMatch)
        {
            List<CandlePattern> barPatterns = bars.GetCandlePatterns(index);

            return patternsToMatch.All(pattern => barPatterns.Contains(pattern));
        }

        /// <summary>
        /// Returns the largest bar index number between an interval in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>int</returns>
        public static int GetLargestBarIndex(this Bars bars, int startIndex, int endIndex)
        {
            double maxBarRange = double.MinValue;

            int result = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double currentBarRange = bars.HighPrices[i] - bars.LowPrices[i];

                if (currentBarRange > maxBarRange)
                {
                    maxBarRange = currentBarRange;

                    result = i;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the smallest bar index number between an interval in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>int</returns>
        public static int GetSmallestBarIndex(this Bars bars, int startIndex, int endIndex)
        {
            double minBarRange = double.MinValue;

            int result = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double currentBarRange = bars.HighPrices[i] - bars.LowPrices[i];

                if (currentBarRange < minBarRange)
                {
                    minBarRange = currentBarRange;

                    result = i;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the range between an index interval in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="useBarBody">Use bar body (open and close) instead of shadows (high and low)</param>
        /// <returns>double</returns>
        public static double GetRange(this Bars bars, int startIndex, int endIndex, bool useBarBody = false)
        {
            double min = double.MaxValue, max = double.MinValue;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double barLow, barHigh;

                if (useBarBody)
                {
                    barLow = bars.GetBarType(i) == BarType.Bullish ? bars.OpenPrices[i] : bars.ClosePrices[i];
                    barHigh = bars.GetBarType(i) == BarType.Bullish ? bars.ClosePrices[i] : bars.OpenPrices[i];
                }
                else
                {
                    barLow = bars.LowPrices[i];
                    barHigh = bars.HighPrices[i];
                }

                min = Math.Min(min, barLow);
                max = Math.Max(max, barHigh);
            }

            return max - min;
        }

        /// <summary>
        /// Returns the range between an index interval in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <param name="returnType">The return type</param>
        /// <param name="useBarBody">Use bar body (open and close) instead of shadows (high and low)</param>
        /// <returns>double</returns>
        public static double GetRange(this Bars bars, int startIndex, int endIndex, Symbol symbol, PriceValueType returnType, bool useBarBody = false)
        {
            double range = bars.GetRange(startIndex, endIndex, useBarBody);

            return symbol.ChangePriceValueType(range, returnType);
        }

        /// <summary>
        /// Returns true if the bars on provided index interval is flat
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="maxStd">Maximum allowed standard deviation in range high and low</param>
        /// <returns>bool</returns>
        public static bool IsFlat(this Bars bars, int startIndex, int endIndex, double maxStd)
        {
            double highStd = bars.HighPrices.GetStandardDeviation(startIndex, endIndex);
            double lowStd = bars.LowPrices.GetStandardDeviation(startIndex, endIndex);

            return highStd <= maxStd && lowStd <= maxStd;
        }

        /// <summary>
        /// Returns a market series specific data series based on provided series type
        /// </summary>
        /// <param name="bars">The market series</param>
        /// <param name="seriesType">Series type</param>
        /// <returns>DataSeries</returns>
        public static DataSeries GetSeries(this Bars bars, SeriesType seriesType)
        {
            switch (seriesType)
            {
                case SeriesType.Open:
                    return bars.OpenPrices;

                case SeriesType.High:
                    return bars.HighPrices;

                case SeriesType.Low:
                    return bars.LowPrices;

                case SeriesType.Close:
                    return bars.ClosePrices;

                case SeriesType.Median:
                    return bars.MedianPrices;

                case SeriesType.TickVolume:
                    return bars.TickVolumes;

                case SeriesType.Typical:
                    return bars.TypicalPrices;

                case SeriesType.WeightedClose:
                    return bars.WeightedPrices;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a bar open time by giving its index, it supports both past and future bars but the future bars provided
        /// open time is an approximation based on previous bars time differences not exact open time
        /// </summary>
        /// <param name="bars">The market series</param>
        /// <param name="barIndex">The bar index</param>
        /// <returns>DateTime</returns>
        public static DateTime GetOpenTime(this Bars bars, double barIndex)
        {
            int currentIndex = bars.GetIndex();

            TimeSpan timeDiff = bars.GetTimeDiff();

            double indexDiff = barIndex - currentIndex;

            double indexDiffAbs = Math.Abs(indexDiff);

            DateTime result = indexDiff <= 0 ? bars.OpenTimes[(int)barIndex] : bars.OpenTimes[currentIndex];

            if (indexDiff > 0)
            {
                for (int i = 1; i <= indexDiffAbs; i++)
                {
                    do
                    {
                        result = result.Add(timeDiff);
                    }
                    while (result.DayOfWeek == DayOfWeek.Saturday || result.DayOfWeek == DayOfWeek.Sunday);
                }
            }

            double barIndexFraction = barIndex % 1;

            double barIndexFractionInMinutes = timeDiff.TotalMinutes * barIndexFraction;

            result = result.AddMinutes(barIndexFractionInMinutes);

            return result;
        }

        /// <summary>
        /// Returns the most common time difference between two bar of a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan GetTimeDiff(this Bars bars)
        {
            int index = bars.GetIndex();

            if (index < 4)
            {
                throw new InvalidOperationException("Not enough data in market series to calculate the time difference");
            }

            List<TimeSpan> timeDiffs = new List<TimeSpan>();

            for (int i = index; i >= index - 4; i--)
            {
                timeDiffs.Add(bars.OpenTimes[i] - bars.OpenTimes[i - 1]);
            }

            return timeDiffs.GroupBy(diff => diff).OrderBy(diffGroup => diffGroup.Count()).Last().First();
        }

        /// <summary>
        /// Returns the market profile of x latest bars in a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Last Bar Index</param>
        /// <param name="periods">Number of previous bars before provided index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <returns>List<PriceVolume></returns>
        public static List<PriceLevel> GetMarketProfile(this Bars bars, int index, int periods, Symbol symbol)
        {
            List<PriceLevel> result = new List<PriceLevel>();

            for (int i = index; i > index - periods; i--)
            {
                for (double level = bars.LowPrices[i]; level <= bars.HighPrices[i]; level += symbol.PipSize)
                {
                    level = Math.Round(level, symbol.Digits);

                    PriceLevel priceLevel = result.FirstOrDefault(pLevel => pLevel.Level == level);

                    if (priceLevel == null)
                    {
                        priceLevel = new PriceLevel
                        {
                            Level = level,
                            Profile = new List<int>()
                        };

                        result.Add(priceLevel);
                    }

                    priceLevel.Profile.Add(i);
                }
            }

            return result;
        }

        /// <summary>
        /// Returns a bar bullish/bearish volume amount in a market series
        /// </summary>
        /// <param name="bars">Market series</param>
        /// <param name="index">Bar index in market series</param>
        /// <returns>BarVolume</returns>
        public static BarVolume GetBarVolume(this Bars bars, int index)
        {
            double barRange = bars.HighPrices[index] - bars.LowPrices[index];

            double percentageAboveBarClose = (bars.HighPrices[index] - bars.ClosePrices[index]) / barRange;
            double percentageBelowBarClose = (bars.ClosePrices[index] - bars.LowPrices[index]) / barRange;

            return new BarVolume
            {
                BullishVolume = bars.TickVolumes[index] * percentageBelowBarClose,
                BearishVolume = bars.TickVolumes[index] * percentageAboveBarClose
            };
        }

        /// <summary>
        /// Returns the average bar range of x previous bars on a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetAverageBarRange(this Bars bars, int index, int periods, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                double iBarRange = bars.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Average();
        }

        /// <summary>
        /// Returns the average bar range of x previous bars on a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetAverageBarRange(this Bars bars, int index, int periods, BarType barType, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                if (bars.GetBarType(iBarIndex) != barType)
                {
                    continue;
                }

                double iBarRange = bars.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Average();
        }

        /// <summary>
        /// Returns the median bar range of x previous bars on a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMedianBarRange(this Bars bars, int index, int periods, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                double iBarRange = bars.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Median();
        }

        /// <summary>
        /// Returns the median bar range of x previous bars on a market series
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMedianBarRange(this Bars bars, int index, int periods, BarType barType, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                if (bars.GetBarType(iBarIndex) != barType)
                {
                    continue;
                }

                double iBarRange = bars.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Median();
        }

        /// <summary>
        /// Returns the high value of a bar body
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="barIndex">The bar index in market series</param>
        /// <returns>double</returns>
        public static double GetBarBodyHigh(this Bars bars, int barIndex)
        {
            return bars.ClosePrices[barIndex] > bars.OpenPrices[barIndex] ? bars.ClosePrices[barIndex] : bars.OpenPrices[barIndex];
        }

        /// <summary>
        /// Returns the low value of a bar body
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="barIndex">The bar index in market series</param>
        /// <returns>double</returns>
        public static double GetBarBodyLow(this Bars bars, int barIndex)
        {
            return bars.ClosePrices[barIndex] < bars.OpenPrices[barIndex] ? bars.ClosePrices[barIndex] : bars.OpenPrices[barIndex];
        }

        /// <summary>
        /// Returns the amount of slope between two level in a market series based on bar bodies
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="firstPointIndex">The first point index in market series</param>
        /// <param name="secondPointIndex">The second point index in market series</param>
        /// <param name="direction"></param>
        /// <returns>double</returns>
        public static double GetBodyBaseSlope(this Bars bars, int firstPointIndex, int secondPointIndex, Direction direction)
        {
            double firstPoint, secondPoint;

            if (direction == Direction.Up)
            {
                firstPoint = bars.GetBarBodyLow(firstPointIndex);
                secondPoint = bars.GetBarBodyLow(secondPointIndex);
            }
            else if (direction == Direction.Down)
            {
                firstPoint = bars.GetBarBodyHigh(firstPointIndex);
                secondPoint = bars.GetBarBodyHigh(secondPointIndex);
            }
            else
            {
                throw new InvalidOperationException("Invalid Direction Type");
            }

            return (secondPoint - firstPoint) / (secondPointIndex - firstPointIndex);
        }

        /// <summary>
        /// Returns True if connecting two provided data point based on cross direction is possible otherwise False
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="firstPointIndex">The first point index in market series</param>
        /// <param name="secondPointIndex">The second point index in market series</param>
        /// <param name="direction">The line direction, is it on up direction or low direction?</param>
        /// <returns>bool</returns>
        public static bool IsBodyConnectionPossible(this Bars bars, int firstPointIndex, int secondPointIndex, Direction direction)
        {
            if (firstPointIndex >= secondPointIndex)
            {
                throw new ArgumentException("The 'firstPointIndex' must be less than 'secondPointIndex'");
            }

            double slope = bars.GetBodyBaseSlope(firstPointIndex, secondPointIndex, direction);

            double firstPoint;

            if (direction == Direction.Up)
            {
                firstPoint = bars.GetBarBodyLow(firstPointIndex);
            }
            else if (direction == Direction.Down)
            {
                firstPoint = bars.GetBarBodyHigh(firstPointIndex);
            }
            else
            {
                throw new InvalidOperationException("Invalid Direction Type");
            }

            int counter = 0;

            for (int i = firstPointIndex + 1; i <= secondPointIndex; i++)
            {
                counter++;

                double iPoint;

                if (direction == Direction.Up)
                {
                    iPoint = bars.GetBarBodyLow(i);

                    if (iPoint < firstPoint + (slope * counter))
                    {
                        return false;
                    }
                }
                else if (direction == Direction.Down)
                {
                    iPoint = bars.GetBarBodyHigh(i);

                    if (iPoint > firstPoint + (slope * counter))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Returns a bar object from a market series
        /// </summary>
        /// <param name="bars">Market series</param>
        /// <param name="index">The bar index in market series</param>
        /// <returns>Bar</returns>
        public static OhlcBar GetBar(this Bars bars, int index)
        {
            var result = bars.ClosePrices.Count > index ? new OhlcBar
            {
                Index = index,
                Time = bars.OpenTimes[index],
                Open = bars.OpenPrices[index],
                High = bars.HighPrices[index],
                Low = bars.LowPrices[index],
                Close = bars.ClosePrices[index],
                Volume = bars.TickVolumes[index],
            } : null;

            return result;
        }

        /// <summary>
        /// Transform a market series data to a Bar objects collection
        /// </summary>
        /// <param name="bars">Market series</param>
        /// <returns>List<Bar></returns>
        public static List<OhlcBar> GetBars(this Bars bars)
        {
            var result = new List<OhlcBar>();

            for (int iBarIndex = 0; iBarIndex < bars.ClosePrices.Count; iBarIndex++)
            {
                var bar = bars.GetBar(iBarIndex);

                result.Add(bar);
            }

            return result;
        }

        /// <summary>
        /// Returns a bar volume strength
        /// </summary>
        /// <param name="bars"></param>
        /// <param name="index">Bar index</param>
        /// <returns>double</returns>
        public static double GetBarVolumeStrength(this Bars bars, int index)
        {
            return bars.GetBarRange(index) / bars.TickVolumes[index]; ;
        }

        /// <summary>
        /// Returns a data series of bars based on your provided data source type
        /// </summary>
        /// <param name="bars">Current Bars</param>
        /// <param name="dataSource">The data source type</param>
        /// <returns>DataSeries</returns>
        public static DataSeries GetSeries(this Bars bars, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.Open:
                    return bars.OpenPrices;

                case DataSource.High:
                    return bars.HighPrices;

                case DataSource.Low:
                    return bars.LowPrices;

                case DataSource.Close:
                    return bars.ClosePrices;

                case DataSource.Volume:
                    return bars.TickVolumes;

                case DataSource.Typical:
                    return bars.TypicalPrices;

                case DataSource.Weighted:
                    return bars.WeightedPrices;

                case DataSource.Median:
                    return bars.MedianPrices;

                default:
                    throw new ArgumentOutOfRangeException(nameof(dataSource));
            }
        }

        /// <summary>
        /// Returns the bar elapsed time or open time - close time
        /// </summary>
        /// <param name="bars">Bars</param>
        /// <param name="index">Bar index</param>
        /// <returns>TimeSpane</returns>
        public static TimeSpan GetBarElapsedTime(this Bars bars, int index)
        {
            if (bars.GetIndex() <= index)
            {
                throw new ArgumentException("There must be another bar available after the current bar to calculate its" +
                    " elapsed time");
            }

            return bars.OpenTimes[index + 1] - bars.OpenTimes[index];
        }
    }
}