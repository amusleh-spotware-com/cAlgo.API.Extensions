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
            return (symbol.TickSize / symbol.PipSize) * Math.Pow(10, symbol.Digits);
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
    }
}