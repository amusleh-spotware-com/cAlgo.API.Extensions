using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace cAlgo.API.Extensions
{
    public static class IndicatorExtensions
    {
        /// <summary>
        /// Plots a legend on indicator window based on given outputs
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="outputs">The collection of outputs</param>
        /// <param name="verticalAlignment">The vertical alignement of legend text</param>
        /// <param name="horizontalAlignment">The horizontal alignment of legend text</param>
        /// <param name="legendChar">The character that will be added before output name</param>
        public static void PlotLegend(this Indicator indicator, List<LegendOutput> outputs, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, string legendChar = "-")
        {
            List<LegendOutput> validLegendOutputs = outputs.Where(o => !string.IsNullOrEmpty(o.Name)).ToList();

            foreach (LegendOutput output in validLegendOutputs)
            {
                string objName = string.Format("{0} {1}", output.Name, output.Color);

                StringBuilder stringBuilder = new StringBuilder();

                for (int i = 0; i <= validLegendOutputs.IndexOf(output); i++)
                {
                    stringBuilder.AppendLine();
                }

                stringBuilder.AppendFormat("{0} {1}", legendChar, output.Name);

                indicator.Chart.DrawStaticText(objName, stringBuilder.ToString(), verticalAlignment, horizontalAlignment, output.Color);
            }
        }

        /// <summary>
        /// Converts the color string to Colors object
        /// </summary>
        /// <param name="indicator"></param>
        /// <param name="color">Color string</param>
        /// <returns>Color</returns>
        public static Color ParseColor(this Indicator indicator, string color)
        {
            return (Color)Enum.Parse(typeof(Color), color, true);
        }
    }
}