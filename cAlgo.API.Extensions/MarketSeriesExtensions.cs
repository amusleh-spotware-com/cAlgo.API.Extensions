using cAlgo.API.Internals;
using System.Collections.Generic;
using System.Linq;
using System;

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
    }
}