using System;
using System.Globalization;

namespace cAlgo.API.Extensions.Utility
{
    public static class TimeSpanTools
    {
        public static bool TryParse(string value, string parameterName, Chart chart, string chartObjectsSuffix,
            out TimeSpan result)
        {
            return TryParse(value, parameterName, chart, chartObjectsSuffix, TimeSpan.FromSeconds(0), out result);
        }

        public static bool TryParse(string value, string parameterName, Chart chart, string chartObjectsSuffix,
            TimeSpan offset, out TimeSpan result)
        {
            var parseResult = TimeSpan.TryParse(value, CultureInfo.InvariantCulture, out result);

            if (!parseResult)
            {
                chart.ShowInvalidParameterMessage(parameterName, value, chartObjectsSuffix);
            }
            else
            {
                result = result.Add(offset);
            }

            return parseResult;
        }
    }
}