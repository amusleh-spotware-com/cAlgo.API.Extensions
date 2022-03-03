using cAlgo.API.Extensions.Enums;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions
{
    public static class SymbolExtensions
    {
        /// <summary>
        /// Returns a symbol pip value
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns>double</returns>
        public static double GetPip(this Symbol symbol)
        {
            return symbol.TickSize / symbol.PipSize * Math.Pow(10, symbol.Digits);
        }

        /// <summary>
        /// Returns a price value in terms of pips
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double ToPips(this Symbol symbol, double price)
        {
            return price * symbol.GetPip();
        }

        /// <summary>
        /// Returns a price value in terms of ticks
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double ToTicks(this Symbol symbol, double price)
        {
            return price * Math.Pow(10, symbol.Digits);
        }

        /// <summary>
        /// Returns a symbol price based value to either pips or ticks
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="priceValue">The price based value</param>
        /// <param name="returnType">The price value return type</param>
        /// <returns>double</returns>
        public static double ChangePriceValueType(this Symbol symbol, double priceValue, PriceValueType returnType)
        {
            switch (returnType)
            {
                case PriceValueType.Pips:
                    return symbol.ToPips(priceValue);

                case PriceValueType.Ticks:
                    return symbol.ToTicks(priceValue);

                default:

                    return priceValue;
            }
        }

        /// <summary>
        /// Returns the amount of risk percentage based on stop loss amount
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="stopLossInPips">Stop loss amount in Pips</param>
        /// <param name="accountBalance">The account balance</param>
        /// <param name="volume">The volume amount in units (Not lots)</param>
        /// <returns>double</returns>
        public static double GetRiskPercentage(this Symbol symbol, double stopLossInPips, double accountBalance, double volume)
        {
            return Math.Abs(stopLossInPips) * symbol.PipValue / accountBalance * 100.0 * volume;
        }

        /// <summary>
        /// Returns the amount of stop loss in Pips based on risk percentage amount
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="riskPercentage">Risk percentage amount</param>
        /// <param name="accountBalance">The account balance</param>
        /// <param name="volume">The volume amount in units (Not lots)</param>
        /// <returns>double</returns>
        public static double GetStopLoss(this Symbol symbol, double riskPercentage, double accountBalance, double volume)
        {
            return riskPercentage / (symbol.PipValue / accountBalance * 100.0 * volume);
        }

        /// <summary>
        /// Returns the amount of volume based on your provided risk percentage and stop loss
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="riskPercentage">Risk percentage amount</param>
        /// <param name="accountBalance">The account balance</param>
        /// <param name="stopLossInPips">Stop loss amount in Pips</param>
        /// <returns>double</returns>
        public static double GetVolume(this Symbol symbol, double riskPercentage, double accountBalance, double stopLossInPips)
        {
            return symbol.NormalizeVolumeInUnits(riskPercentage / (Math.Abs(stopLossInPips) * symbol.PipValue / accountBalance * 100));
        }

        /// <summary>
        /// Rounds a price level to the number of symbol digits
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double Round(this Symbol symbol, double price)
        {
            return Math.Round(price, symbol.Digits);
        }

        /// <summary>
        /// Normalize x Pips amount decimal places to something that can be used as a stop loss or take profit for an order.
        /// For example if symbol is EURUSD and you pass to this method 10.456775 it will return back 10.5
        /// </summary>
        /// <param name="symbol">The symbol</param>
        /// <param name="pips">The amount of Pips</param>
        /// <returns>double</returns>
        public static double NormalizePips(this Symbol symbol, double pips)
        {
            var currentPrice = Convert.ToDecimal(symbol.Bid);

            var pipSize = Convert.ToDecimal(symbol.PipSize);

            var pipsDecimal = Convert.ToDecimal(pips);

            var pipsAddedToCurrentPrice = Math.Round((pipsDecimal * pipSize) + currentPrice, symbol.Digits);

            var tickSize = Convert.ToDecimal(symbol.TickSize);

            var result = (pipsAddedToCurrentPrice - currentPrice) * (tickSize / pipSize * Convert.ToDecimal(Math.Pow(10, symbol.Digits)));

            return decimal.ToDouble(result);
        }
    }
}