using System;

namespace cAlgo.API.Extensions.Utility
{
    public static class EnumTools
    {
        public static bool TryParse<TEnum>(string value, string parameterName, Chart chart, string chartObjectsSuffix, out TEnum result) 
            where TEnum: struct
        {
            var parseResult = Enum.TryParse<TEnum>(value, true, out result);

            if (!parseResult)
            {
                chart.ShowInvalidParameterMessage(parameterName, value, chartObjectsSuffix);
            }

            return parseResult;
        }
    }
}
