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
        /// Returns true if the bars between start and end index trending up otherwise false
        /// </summary>
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">The start bar index in a market series</param>
        /// <param name="endIndex">The end bar index in a market series</param>
        /// <param name="step">The step that will be used for comparison of bars</param>
        /// <returns></returns>
        public static bool IsTrendingUp(this MarketSeries marketSeries, int startIndex, int endIndex, int step = 3)
        {
            bool result = true;

            for (int i = startIndex + step; i <= endIndex; i += step)
            {
                if (marketSeries.High[i] < marketSeries.High[i - step])
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
        /// <param name="marketSeries"></param>
        /// <param name="startIndex">The start bar index in a market series</param>
        /// <param name="endIndex">The end bar index in a market series</param>
        /// <param name="step">The step that will be used for comparison of bars</param>
        /// <returns></returns>
        public static bool IsTrendingDown(this MarketSeries marketSeries, int startIndex, int endIndex, int step = 3)
        {
            bool result = true;

            for (int i = startIndex + step; i <= endIndex; i += step)
            {
                if (marketSeries.Low[i] > marketSeries.Low[i - step])
                {
                    result = false;

                    break;
                }
            }

            return result;
        }
    }
}