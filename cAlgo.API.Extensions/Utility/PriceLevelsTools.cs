using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;

namespace cAlgo.API.Extensions.Utility
{
    public static class PriceLevelsTools
    {

        /// <summary>
        /// Combines the input price levels data based on provided width
        /// </summary>
        /// <param name="data">The input data</param>
        /// <param name="width">Width in term of price not Pips</param>
        /// <param name="symbol">The symbol of price levels</param>
        /// <returns>List<PriceLevel></returns>
        public static List<PriceLevel> GetCombinedLevels(List<PriceLevel> data, double width)
        {
            List<PriceLevel> ordered = data.OrderBy(priceLevel => priceLevel.Level).ToList();

            List<PriceLevel> dataCombined = new List<PriceLevel>();

            PriceLevel currentLevel = new PriceLevel
            {
                Level = ordered.First().Level,
                Profile = new List<int>()
            };

            ordered.ForEach(priceLevel =>
            {
                if (priceLevel.Level >= currentLevel.Level && priceLevel.Level <= currentLevel.Level + width)
                {
                    // Market profile
                    if (priceLevel.Profile != null)
                    {
                        currentLevel.Profile.AddRange(priceLevel.Profile);
                    }

                    // Volume profile
                    currentLevel.BearishVolume += priceLevel.BearishVolume;
                    currentLevel.BullishVolume += priceLevel.BullishVolume;
                }
                else
                {
                    currentLevel = new PriceLevel
                    {
                        Level = priceLevel.Level,
                        Profile = new List<int>()
                    };
                }

                if (!dataCombined.Contains(currentLevel))
                {
                    dataCombined.Add(currentLevel);
                }
            });

            return dataCombined;
        }
    }
}
