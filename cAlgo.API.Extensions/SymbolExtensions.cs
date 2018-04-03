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
        /// Returns a symbol price level in pips
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price">The price level</param>
        /// <returns>double</returns>
        public static double ToPips(this Symbol symbol, double price)
        {
            return price * symbol.GetPip();
        }
    }
}