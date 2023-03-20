using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cAlgo.API.Extensions
{
    public static class BarExtensions
    {
        /// <summary>
        /// Returns the range of a bar in a market series
        /// </summary>
        /// <param name="bar"></param>
        /// <param name="useBarBody">Use bar open and close price instead of high and low?</param>
        /// <returns>double</returns>
        public static double GetBarRange(this Bar bar, bool useBarBody = false)
        {
            return useBarBody ? Math.Abs(bar.Open - bar.Close) : bar.High - bar.Low;
        }
    }
}
