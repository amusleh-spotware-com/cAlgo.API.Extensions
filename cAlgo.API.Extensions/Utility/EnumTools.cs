using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cAlgo.API.Extensions.Utility
{
    public static class EnumTools
    {
        public static void TryParse<TEnum>(string value, string parameterName, Chart chart, string chartObjectsSuffix, out TEnum result) 
            where TEnum: struct
        {
            bool parseResult = Enum.TryParse<TEnum>(value, true, out result);

            if (!parseResult)
            {
                chart.ShowInvalidParameterMessage(parameterName, value, chartObjectsSuffix);
            }
        }
    }
}
