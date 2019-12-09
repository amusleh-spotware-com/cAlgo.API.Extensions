using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions
{
    public static class MarketSeriesExtensions
    {
        /// <summary>
        /// Returns the last bar index in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <returns>int</returns>
        public static int GetIndex(this MarketSeries marketSeries)
        {
            return marketSeries.Close.Count > 0 ? marketSeries.Close.Count - 1 : marketSeries.Close.Count;
        }

        /// <summary>
        /// Returns the volume profile of x latest bars in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Last Bar Index</param>
        /// <param name="periods">Number of previous bars before provided index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <returns>List<PriceVolume></returns>
        public static List<PriceLevel> GetVolumeProfile(this MarketSeries marketSeries, int index, int periods, Symbol symbol, double stepInPips)
        {
            List<PriceLevel> result = new List<PriceLevel>();

            double step = stepInPips * symbol.PipSize;

            for (int i = index; i > index - periods; i--)
            {
                double barRange = marketSeries.GetBarRange(i);

                double barVolume = marketSeries.TickVolume[i];

                if (barRange <= 0 || barVolume <= 0)
                {
                    continue;
                }

                double percentageAboveBarClose = (marketSeries.High[i] - marketSeries.Close[i]) / barRange;
                double percentageBelowBarClose = (marketSeries.Close[i] - marketSeries.Low[i]) / barRange;

                double bullishVolume = barVolume * percentageBelowBarClose;
                double bearishVolume = barVolume * percentageAboveBarClose;

                double barRangeInPips = symbol.ToPips(barRange);

                double bullishVolumePerPips = bullishVolume / barRangeInPips;
                double bearishVolumePerPips = bearishVolume / barRangeInPips;

                long bullishVolumePerLevel = (long)(bullishVolumePerPips * stepInPips);
                long bearishVolumePerLevel = (long)(bearishVolumePerPips * stepInPips);

                for (double level = marketSeries.Low[i]; level <= marketSeries.High[i]; level += step)
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

                    priceLevel.BullishVolume += bullishVolumePerLevel;
                    priceLevel.BearishVolume += bearishVolumePerLevel;
                }
            }

            return result;
        }

        /// <summary>
        /// Returns the amount of percentage change of a value in comparison with it's previous value in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The value index</param>
        /// <returns>double</returns>
        public static double GetPercentageChange(this MarketSeries marketSeries, int index)
        {
            return -((marketSeries.Open[index] - marketSeries.Close[index]) / marketSeries.Open[index]) * 100;
        }

        /// <summary>
        /// Returns the bar type
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Index of bar</param>
        /// <returns>BarType</returns>
        public static BarType GetBarType(this MarketSeries marketSeries, int index)
        {
            if (marketSeries.Close[index] > marketSeries.Open[index])
            {
                return BarType.Bullish;
            }
            else if (marketSeries.Close[index] < marketSeries.Open[index])
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
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this MarketSeries marketSeries, int index, bool useBarBody = false)
        {
            return useBarBody ? Math.Abs(marketSeries.Open[index] - marketSeries.Close[index])
                : marketSeries.High[index] - marketSeries.Low[index];
        }

        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="symbol">The market series symbol</param>
        /// <param name="returnType">The return type</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this MarketSeries marketSeries, int index, Symbol symbol, PriceValueType returnType, bool useBarBody = false)
        {
            double range = marketSeries.GetBarRange(index, useBarBody);

            return symbol.ChangePriceValueType(range, returnType);
        }

        /// <summary>
        /// Returns the maximum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMaxBarRange(this MarketSeries marketSeries, int index, int periods, bool useBarBody = false)
        {
            double maxRange = double.MinValue;

            for (int i = index; i >= index - periods; i--)
            {
                maxRange = Math.Max(maxRange, marketSeries.GetBarRange(i, useBarBody: useBarBody));
            }

            return maxRange;
        }

        /// <summary>
        /// Returns the maximum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMaxBarRange(this MarketSeries marketSeries, int index, int periods, BarType barType, bool useBarBody = false)
        {
            double maxRange = double.MinValue;

            for (int i = index; i >= index - periods; i--)
            {
                if (marketSeries.GetBarType(i) != barType)
                {
                    continue;
                }

                maxRange = Math.Max(maxRange, marketSeries.GetBarRange(i, useBarBody: useBarBody));
            }

            return maxRange;
        }

        /// <summary>
        /// Returns the minimum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMinBarRange(this MarketSeries marketSeries, int index, int periods, bool useBarBody = false)
        {
            double minRange = double.MaxValue;

            for (int i = index; i >= index - periods; i--)
            {
                minRange = Math.Min(minRange, marketSeries.GetBarRange(i, useBarBody: useBarBody));
            }

            return minRange;
        }

        /// <summary>
        /// Returns the minimum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMinBarRange(this MarketSeries marketSeries, int index, int periods, BarType barType, bool useBarBody = false)
        {
            double minRange = double.MaxValue;

            for (int i = index; i >= index - periods; i--)
            {
                if (marketSeries.GetBarType(i) != barType)
                {
                    continue;
                }

                minRange = Math.Min(minRange, marketSeries.GetBarRange(i, useBarBody: useBarBody));
            }

            return minRange;
        }

        /// <summary>
        /// Returns True if the index bar is an engulfing bar
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsEngulfingBar(this MarketSeries marketSeries, int index)
        {
            double barBodyRange = marketSeries.GetBarRange(index, true);
            double previousBarRange = marketSeries.GetBarRange(index - 1);

            BarType barType = marketSeries.GetBarType(index);
            BarType previousBarType = marketSeries.GetBarType(index - 1);

            return barBodyRange > previousBarRange && barType != previousBarType ? true : false;
        }

        /// <summary>
        /// Returns True if the index bar is a rejection bar
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsRejectionBar(this MarketSeries marketSeries, int index)
        {
            double barBodyRange = marketSeries.GetBarRange(index, true);
            double barRange = marketSeries.GetBarRange(index);

            BarType barType = marketSeries.GetBarType(index);

            double meanBarRange = marketSeries.GetAverageBarRange(index - 1, 50);

            if (barBodyRange / barRange < 0.3 && barRange > meanBarRange)
            {
                double barMiddle = (barRange * 0.5) + marketSeries.Low[index];
                double barFirstQuartile = (barRange * 0.25) + marketSeries.Low[index];
                double barThirdQuartile = (barRange * 0.75) + marketSeries.Low[index];

                if ((marketSeries.Open[index] > barMiddle && marketSeries.Close[index] > barThirdQuartile && barType == BarType.Bullish) ||
                    (marketSeries.Open[index] < barMiddle && marketSeries.Close[index] < barFirstQuartile && barType == BarType.Bearish))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns True if the index bar is a doji bar
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsDojiBar(this MarketSeries marketSeries, int index)
        {
            double barBodyRange = marketSeries.GetBarRange(index, true);
            double barRange = marketSeries.GetBarRange(index);

            double meanBarRange = marketSeries.GetAverageBarRange(index - 1, 50);

            return barRange < meanBarRange / 3 && barBodyRange / barRange < 0.5 ? true : false;
        }

        /// <summary>
        /// Returns True if the index bar is an inside bar
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsInsideBar(this MarketSeries marketSeries, int index)
        {
            BarType barType = marketSeries.GetBarType(index);
            BarType previousBarType = marketSeries.GetBarType(index - 1);

            if (marketSeries.High[index] < marketSeries.High[index - 1] &&
                marketSeries.Low[index] > marketSeries.Low[index - 1] &&
                barType != previousBarType)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Returns True if the index bar is a three bar reversal
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsThreeBarReversal(this MarketSeries marketSeries, int index)
        {
            bool result = false;

            BarType barType = marketSeries.GetBarType(index);
            BarType previousBarType = marketSeries.GetBarType(index - 1);

            if (barType == BarType.Bullish && previousBarType == BarType.Bearish && marketSeries.GetBarType(index - 2) == BarType.Bearish)
            {
                if (marketSeries.Low[index - 1] < marketSeries.Low[index - 2] && marketSeries.Low[index - 1] < marketSeries.Low[index])
                {
                    if (marketSeries.Close[index] > marketSeries.Open[index - 1])
                    {
                        result = true;
                    }
                }
            }
            else if (barType == BarType.Bearish && previousBarType == BarType.Bullish && marketSeries.GetBarType(index - 2) == BarType.Bullish)
            {
                if (marketSeries.High[index - 1] > marketSeries.High[index - 2] && marketSeries.High[index - 1] > marketSeries.High[index])
                {
                    if (marketSeries.Close[index] < marketSeries.Open[index - 1])
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
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>List<CandlePattern></returns>
        public static List<CandlePattern> GetCandlePatterns(this MarketSeries marketSeries, int index)
        {
            List<CandlePattern> patterns = new List<CandlePattern>();

            // Engulfing
            if (marketSeries.IsEngulfingBar(index))
            {
                patterns.Add(CandlePattern.Engulfing);
            }

            // Rejection
            if (marketSeries.IsRejectionBar(index))
            {
                patterns.Add(CandlePattern.Rejection);
            }

            // Doji
            if (marketSeries.IsDojiBar(index))
            {
                patterns.Add(CandlePattern.Doji);
            }

            // InsideBar
            if (marketSeries.IsInsideBar(index))
            {
                patterns.Add(CandlePattern.InsideBar);
            }

            // Three Reversal Bars
            if (marketSeries.IsThreeBarReversal(index))
            {
                patterns.Add(CandlePattern.ThreeBarReversal);
            }

            return patterns;
        }

        /// <summary>
        /// Returns True if the provided bar matches any of the provided patterns otherwise false
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <param name="patternsToMatch">List of candle patterns to match</param>
        /// <returns>bool</returns>
        public static bool IsCandlePatternMatchesAny(this MarketSeries marketSeries, int index, List<CandlePattern> patternsToMatch)
        {
            List<CandlePattern> barPatterns = marketSeries.GetCandlePatterns(index);

            return patternsToMatch.Any(pattern => barPatterns.Contains(pattern));
        }

        /// <summary>
        /// Returns True if the provided bar matches all of the provided patterns otherwise false
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <param name="patternsToMatch">List of candle patterns to match</param>
        /// <returns>bool</returns>
        public static bool IsCandlePatternMatchesAll(this MarketSeries marketSeries, int index, List<CandlePattern> patternsToMatch)
        {
            List<CandlePattern> barPatterns = marketSeries.GetCandlePatterns(index);

            return patternsToMatch.All(pattern => barPatterns.Contains(pattern));
        }

        /// <summary>
        /// Returns the largest bar index number between an interval in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>int</returns>
        public static int GetLargestBarIndex(this MarketSeries marketSeries, int startIndex, int endIndex)
        {
            double maxBarRange = double.MinValue;

            int result = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double currentBarRange = marketSeries.High[i] - marketSeries.Low[i];

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
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <returns>int</returns>
        public static int GetSmallestBarIndex(this MarketSeries marketSeries, int startIndex, int endIndex)
        {
            double minBarRange = double.MinValue;

            int result = 0;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double currentBarRange = marketSeries.High[i] - marketSeries.Low[i];

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
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="useBarBody">Use bar body (open and close) instead of shadows (high and low)</param>
        /// <returns>double</returns>
        public static double GetRange(this MarketSeries marketSeries, int startIndex, int endIndex, bool useBarBody = false)
        {
            double min = double.MaxValue, max = double.MinValue;

            for (int i = startIndex; i <= endIndex; i++)
            {
                double barLow, barHigh;

                if (useBarBody)
                {
                    barLow = marketSeries.GetBarType(i) == BarType.Bullish ? marketSeries.Open[i] : marketSeries.Close[i];
                    barHigh = marketSeries.GetBarType(i) == BarType.Bullish ? marketSeries.Close[i] : marketSeries.Open[i];
                }
                else
                {
                    barLow = marketSeries.Low[i];
                    barHigh = marketSeries.High[i];
                }

                min = Math.Min(min, barLow);
                max = Math.Max(max, barHigh);
            }

            return max - min;
        }

        /// <summary>
        /// Returns the range between an index interval in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <param name="returnType">The return type</param>
        /// <param name="useBarBody">Use bar body (open and close) instead of shadows (high and low)</param>
        /// <returns>double</returns>
        public static double GetRange(this MarketSeries marketSeries, int startIndex, int endIndex, Symbol symbol, PriceValueType returnType, bool useBarBody = false)
        {
            double range = marketSeries.GetRange(startIndex, endIndex, useBarBody);

            return symbol.ChangePriceValueType(range, returnType);
        }

        /// <summary>
        /// Returns true if the bars on provided index interval is flat
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">Start index</param>
        /// <param name="endIndex">End index</param>
        /// <param name="maxStd">Maximum allowed standard deviation in range high and low</param>
        /// <returns>bool</returns>
        public static bool IsFlat(this MarketSeries marketSeries, int startIndex, int endIndex, double maxStd)
        {
            double highStd = marketSeries.High.GetStandardDeviation(startIndex, endIndex);
            double lowStd = marketSeries.Low.GetStandardDeviation(startIndex, endIndex);

            return highStd <= maxStd && lowStd <= maxStd;
        }

        /// <summary>
        /// Returns a market series specific data series based on provided series type
        /// </summary>
        /// <param name="marketSeries">The market series</param>
        /// <param name="seriesType">Series type</param>
        /// <returns>DataSeries</returns>
        public static DataSeries GetSeries(this MarketSeries marketSeries, SeriesType seriesType)
        {
            switch (seriesType)
            {
                case SeriesType.Open:
                    return marketSeries.Open;

                case SeriesType.High:
                    return marketSeries.High;

                case SeriesType.Low:
                    return marketSeries.Low;

                case SeriesType.Close:
                    return marketSeries.Close;

                case SeriesType.Median:
                    return marketSeries.Median;

                case SeriesType.TickVolume:
                    return marketSeries.TickVolume;

                case SeriesType.Typical:
                    return marketSeries.Typical;

                case SeriesType.WeightedClose:
                    return marketSeries.WeightedClose;

                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns a bar open time by giving its index, it supports both past and future bars but the future bars provided
        /// open time is an approximation based on previous bars time differences not exact open time
        /// </summary>
        /// <param name="marketSeries">The market series</param>
        /// <param name="barIndex">The bar index</param>
        /// <returns>DateTime</returns>
        public static DateTime GetOpenTime(this MarketSeries marketSeries, double barIndex)
        {
            int currentIndex = marketSeries.GetIndex();

            TimeSpan timeDiff = marketSeries.GetTimeDiff();

            double indexDiff = barIndex - currentIndex;

            double indexDiffAbs = Math.Abs(indexDiff);

            DateTime result = indexDiff <= 0 ? marketSeries.OpenTime[(int)barIndex] : marketSeries.OpenTime[currentIndex];

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
        /// <param name="marketSeries"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan GetTimeDiff(this MarketSeries marketSeries)
        {
            int index = marketSeries.GetIndex();

            if (index < 4)
            {
                throw new InvalidOperationException("Not enough data in market series to calculate the time difference");
            }

            List<TimeSpan> timeDiffs = new List<TimeSpan>();

            for (int i = index; i >= index - 4; i--)
            {
                timeDiffs.Add(marketSeries.OpenTime[i] - marketSeries.OpenTime[i - 1]);
            }

            return timeDiffs.GroupBy(diff => diff).OrderBy(diffGroup => diffGroup.Count()).Last().First();
        }

        /// <summary>
        /// Returns the market profile of x latest bars in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Last Bar Index</param>
        /// <param name="periods">Number of previous bars before provided index</param>
        /// <param name="symbol">The market series symbol</param>
        /// <param name="step">The price increment step in Pips</param>
        /// <returns>List<PriceVolume></returns>
        public static List<PriceLevel> GetMarketProfile(this MarketSeries marketSeries, int index, int periods, Symbol symbol, double stepInPips)
        {
            double step = stepInPips * symbol.PipSize;

            List<PriceLevel> result = new List<PriceLevel>();

            for (int i = index; i > index - periods; i--)
            {
                for (double level = marketSeries.Low[i]; level <= marketSeries.High[i]; level += step)
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
        /// <param name="marketSeries">Market series</param>
        /// <param name="index">Bar index in market series</param>
        /// <returns>BarVolume</returns>
        public static BarVolume GetBarVolume(this MarketSeries marketSeries, int index)
        {
            double barRange = marketSeries.High[index] - marketSeries.Low[index];

            double percentageAboveBarClose = (marketSeries.High[index] - marketSeries.Close[index]) / barRange;
            double percentageBelowBarClose = (marketSeries.Close[index] - marketSeries.Low[index]) / barRange;

            return new BarVolume
            {
                BullishVolume = marketSeries.TickVolume[index] * percentageBelowBarClose,
                BearishVolume = marketSeries.TickVolume[index] * percentageAboveBarClose
            };
        }

        /// <summary>
        /// Returns the average bar range of x previous bars on a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetAverageBarRange(this MarketSeries marketSeries, int index, int periods, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                double iBarRange = marketSeries.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Average();
        }

        /// <summary>
        /// Returns the average bar range of x previous bars on a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series, the calculation will begin from this bar</param>
        /// <param name="periods">The number of x previous bars or look back bars</param>
        /// <param name="barType">The type of bars</param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetAverageBarRange(this MarketSeries marketSeries, int index, int periods, BarType barType, bool useBarBody = false)
        {
            List<double> barRanges = new List<double>();

            for (int iBarIndex = index; iBarIndex >= index - periods; iBarIndex--)
            {
                if (marketSeries.GetBarType(iBarIndex) != barType)
                {
                    continue;
                }

                double iBarRange = marketSeries.GetBarRange(iBarIndex, useBarBody);

                barRanges.Add(iBarRange);
            }

            return barRanges.Average();
        }

        /// <summary>
        /// Returns the high value of a bar body
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="barIndex">The bar index in market series</param>
        /// <returns>double</returns>
        public static double GetBarBodyHigh(this MarketSeries marketSeries, int barIndex)
        {
            return marketSeries.Close[barIndex] > marketSeries.Open[barIndex] ? marketSeries.Close[barIndex] : marketSeries.Open[barIndex];
        }

        /// <summary>
        /// Returns the low value of a bar body
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="barIndex">The bar index in market series</param>
        /// <returns>double</returns>
        public static double GetBarBodyLow(this MarketSeries marketSeries, int barIndex)
        {
            return marketSeries.Close[barIndex] < marketSeries.Open[barIndex] ? marketSeries.Close[barIndex] : marketSeries.Open[barIndex];
        }

        /// <summary>
        /// Returns the amount of slope between two level in a market series based on bar bodies
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="firstPointIndex">The first point index in market series</param>
        /// <param name="secondPointIndex">The second point index in market series</param>
        /// <param name="direction"></param>
        /// <returns>double</returns>
        public static double GetBodyBaseSlope(this MarketSeries marketSeries, int firstPointIndex, int secondPointIndex, Direction direction)
        {
            double firstPoint, secondPoint;

            if (direction == Direction.Up)
            {
                firstPoint = marketSeries.GetBarBodyLow(firstPointIndex);
                secondPoint = marketSeries.GetBarBodyLow(secondPointIndex);
            }
            else if (direction == Direction.Down)
            {
                firstPoint = marketSeries.GetBarBodyHigh(firstPointIndex);
                secondPoint = marketSeries.GetBarBodyHigh(secondPointIndex);
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
        /// <param name="marketSeries"></param>
        /// <param name="firstPointIndex">The first point index in market series</param>
        /// <param name="secondPointIndex">The second point index in market series</param>
        /// <param name="direction">The line direction, is it on up direction or low direction?</param>
        /// <returns>bool</returns>
        public static bool IsBodyConnectionPossible(this MarketSeries marketSeries, int firstPointIndex, int secondPointIndex, Direction direction)
        {
            if (firstPointIndex >= secondPointIndex)
            {
                throw new ArgumentException("The 'firstPointIndex' must be less than 'secondPointIndex'");
            }

            double slope = marketSeries.GetBodyBaseSlope(firstPointIndex, secondPointIndex, direction);

            double firstPoint;

            if (direction == Direction.Up)
            {
                firstPoint = marketSeries.GetBarBodyLow(firstPointIndex);
            }
            else if (direction == Direction.Down)
            {
                firstPoint = marketSeries.GetBarBodyHigh(firstPointIndex);
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
                    iPoint = marketSeries.GetBarBodyLow(i);

                    if (iPoint < firstPoint + (slope * counter))
                    {
                        return false;
                    }
                }
                else if (direction == Direction.Down)
                {
                    iPoint = marketSeries.GetBarBodyHigh(i);

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
        /// <param name="marketSeries">Market series</param>
        /// <param name="index">The bar index in market series</param>
        /// <returns>Bar</returns>
        public static Bar GetBar(this MarketSeries marketSeries, int index)
        {
            var result = marketSeries.Close.Count > index ? new Bar
            {
                Index = index,
                Time = marketSeries.OpenTime[index],
                Open = marketSeries.Open[index],
                High = marketSeries.High[index],
                Low = marketSeries.Low[index],
                Close = marketSeries.Close[index],
                Volume = marketSeries.TickVolume[index],
                Type = marketSeries.GetBarType(index)
            } : null;

            return result;
        }

        /// <summary>
        /// Transform a market series data to a Bar objects collection
        /// </summary>
        /// <param name="marketSeries">Market series</param>
        /// <returns>List<Bar></returns>
        public static List<Bar> GetBars(this MarketSeries marketSeries)
        {
            var result = new List<Bar>();

            for (int iBarIndex = 0; iBarIndex < marketSeries.Close.Count; iBarIndex++)
            {
                var bar = marketSeries.GetBar(iBarIndex);

                result.Add(bar);
            }

            return result;
        }

        /// <summary>
        /// Returns a bar volume strength
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index</param>
        /// <returns>double</returns>
        public static double GetBarVolumeStrength(this MarketSeries marketSeries, int index)
        {
            return marketSeries.GetBarRange(index) / marketSeries.TickVolume[index]; ;
        }
    }
}