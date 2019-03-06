using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace cAlgo.API.Extensions.Utility
{
    public static class EnumTools
    {
        public static void Parse<TEnum>(string value, Chart chart, string parameterName, out TEnum result) where TEnum: struct
        {
            bool parseResult = Enum.TryParse<TEnum>(value, true, out result);

            if (!parseResult)
            {
                chart.ShowInvalidParameterMessage(parameterName, value);
            }
        }
    }
}
