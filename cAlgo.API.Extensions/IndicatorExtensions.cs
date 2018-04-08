using System;
using System.Collections.Generic;
using System.Text;

namespace cAlgo.API.Extensions
{
    public static class IndicatorExtensions
    {
        /// <summary>
        /// Plots a legend on indicator window based on given outputs
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="outputs">The collection of outputs</param>
        /// <param name="legendChar">The character that will be added before output name</param>
        /// <param name="position">Position of legend on indicator window</param>
        public static void PlotLegend(this Indicator indicator, List<LegendOutput> outputs, string legendChar = "-")
        {
            foreach (LegendOutput output in outputs)
            {
                if (string.IsNullOrEmpty(output.Name))
                {
                    continue;
                }

                string objName = string.Format("{0} {1}", output.Name, output.Color);

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i <= outputs.IndexOf(output); i++)
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendFormat("{0} {1}", legendChar, output.Name);

                indicator.ChartObjects.DrawText(objName, stringBuilder.ToString(), StaticPosition.TopRight, output.Color);
            }
        }

        /// <summary>
        /// Converts the color string to Colors object
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="color">Color string</param>
        /// <returns>Colors</returns>
        public static Colors ParseColor(this Indicator indicator, string color)
        {
            return (Colors)Enum.Parse(typeof(Colors), color, true);
        }
    }
}