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
            return marketSeries.Close.Count - 1;
        }

        /// <summary>
        /// Returns the volume profile of x latest values in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="periods">The number of latest values</param>
        /// <param name="priceStep">The step (Pips) that is used for increament of price</param>
        /// <param name="symbol">The market series symbol</param>
        /// <returns>List<PriceVolume></returns>
        public static List<PriceVolume> GetPriceVolume(this MarketSeries marketSeries, int periods, double priceStep, Symbol symbol)
        {
            List<PriceVolume> result = new List<PriceVolume>();

            int index = marketSeries.GetIndex();

            double priceStepInPrice = symbol.ToPips(priceStep);

            for (int i = index; i > index - periods; i--)
            {
                double barRange = symbol.ToPips(marketSeries.High[i] - marketSeries.Low[i]) / priceStepInPrice;

                long volumePerPriceLevel = (long)(marketSeries.TickVolume[i] / barRange);

                for (double price = marketSeries.Low[i]; price <= marketSeries.High[i]; price += priceStep)
                {
                    price = Math.Round(price, symbol.Digits);

                    PriceVolume priceVolume = result.FirstOrDefault(pVolume => pVolume.Price == price);

                    if (priceVolume == null)
                    {
                        priceVolume = new PriceVolume() { Price = price };

                        result.Add(priceVolume);
                    }

                    if (marketSeries.Close[i] > marketSeries.Open[i])
                    {
                        priceVolume.BullishVolume += volumePerPriceLevel;
                    }
                    else if (marketSeries.Close[i] < marketSeries.Open[i])
                    {
                        priceVolume.BearishVolume += volumePerPriceLevel;
                    }
                    else
                    {
                        priceVolume.NeutralVolume += volumePerPriceLevel;
                    }
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
            return marketSeries.Close[index] > marketSeries.Open[index] ? BarType.Up : BarType.Down;
        }

        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="useOpenClose">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this MarketSeries marketSeries, int index, bool useOpenClose = false)
        {
            return useOpenClose ? marketSeries.Open[index] - marketSeries.Close[index] : marketSeries.High[index] - marketSeries.Low[index];
        }

        /// <summary>
        /// Returns the range of a bar in a market series in Pips
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">Bar index in market series</param>
        /// <param name="useOpenClose">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRangeInPips(this MarketSeries marketSeries, Symbol symbol, int index, bool useOpenClose = false)
        {
            return symbol.ToPips(marketSeries.GetBarRange(index, useOpenClose));
        }

        /// <summary>
        /// Returns the maximum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useOpenClose">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMaxBarRange(this MarketSeries marketSeries, int index, int periods, bool useOpenClose = false)
        {
            double maxRange = double.MinValue;

            for (int i = index; i >= index - periods; i--)
            {
                maxRange = Math.Max(maxRange, marketSeries.GetBarRange(i, useOpenClose));
            }

            return maxRange;
        }

        /// <summary>
        /// Returns the minimum bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useOpenClose">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMinBarRange(this MarketSeries marketSeries, int index, int periods, bool useOpenClose = false)
        {
            double minRange = double.MaxValue;

            for (int i = index; i >= index - periods; i--)
            {
                minRange = Math.Min(minRange, marketSeries.GetBarRange(i, useOpenClose));
            }

            return minRange;
        }

        /// <summary>
        /// Returns the mean bar range in a market series
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The start bar index</param>
        /// <param name="periods">The number of previous bars</param>
        /// <param name="useOpenClose">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetMeanBarRange(this MarketSeries marketSeries, int index, int periods, bool useOpenClose = false)
        {
            List<double> ranges = new List<double>();

            for (int i = index; i >= index - periods; i--)
            {
                ranges.Add(marketSeries.GetBarRange(i, useOpenClose));
            }

            return ranges.Average();
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

            double meanBarRange = marketSeries.GetMeanBarRange(index - 1, 50);

            if (barBodyRange / barRange < 0.3 && barRange > meanBarRange)
            {
                double barMiddle = (barRange * 0.5) + marketSeries.Low[index];
                double barFirstQuartile = (barRange * 0.25) + marketSeries.Low[index];
                double barThirdQuartile = (barRange * 0.75) + marketSeries.Low[index];

                if ((marketSeries.Open[index] > barMiddle && marketSeries.Close[index] > barThirdQuartile && barType == BarType.Up) ||
                    (marketSeries.Open[index] < barMiddle && marketSeries.Close[index] < barFirstQuartile && barType == BarType.Down))
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

            double meanBarRange = marketSeries.GetMeanBarRange(index - 1, 50);

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
        /// Returns True if the index bar is a three reversal bar
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="index">The bar index number in a market series</param>
        /// <returns>bool</returns>
        public static bool IsThreeReversalBar(this MarketSeries marketSeries, int index)
        {
            bool result = false;

            BarType barType = marketSeries.GetBarType(index);
            BarType previousBarType = marketSeries.GetBarType(index - 1);

            if (barType == BarType.Up && previousBarType == BarType.Down && marketSeries.GetBarType(index - 2) == BarType.Down)
            {
                if (marketSeries.Low[index - 1] < marketSeries.Low[index - 2] && marketSeries.Low[index - 1] < marketSeries.Low[index])
                {
                    if (marketSeries.Close[index] > marketSeries.Open[index - 1])
                    {
                        result = true;
                    }
                }
            }
            else if (barType == BarType.Down && previousBarType == BarType.Up && marketSeries.GetBarType(index - 2) == BarType.Up)
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
        public static List<CandlePattern> GetCandlePattern(this MarketSeries marketSeries, int index)
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
            if (marketSeries.IsThreeReversalBar(index))
            {
                patterns.Add(CandlePattern.ThreeReversalBars);
            }

            return patterns;
        }
    }
}