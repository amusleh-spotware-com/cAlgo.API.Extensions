using System;

namespace cAlgo.API.Extensions
{
    public static class ChartExtensions
    {
        /// <summary>
        /// Shows an error message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The error message</param>
        public static void ShowError(this Chart chart, string message)
        {
            string objectSuffix = string.Format("No_Suffix_{0}", DateTime.Now.Ticks);

            chart.ShowError(message, objectSuffix);
        }

        /// <summary>
        /// Shows an error message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The error message</param>
        /// <param name="objectSuffix">The optional suffix that will be appended at the end of error message object</param>
        public static void ShowError(this Chart chart, string message, string objectSuffix)
        {
            chart.ShowError(message, objectSuffix, VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);
        }

        /// <summary>
        /// Shows an error message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The error message</param>
        /// <param name="objectSuffix">The optional suffix that will be appended at the end of error message object</param>
        /// <param name="verticalAlignment">The vertical alignment of error message on chart</param>
        /// <param name="horizontalAlignment">The horizontal alignment of error message on chart</param>
        /// <param name="color">The color of error message on chart</param>
        public static void ShowError(this Chart chart, string message, string objectSuffix, VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalAlignment, Color color)
        {
            objectSuffix = objectSuffix ?? string.Format("No_Suffix_{0}", DateTime.Now.Ticks);

            string chartObjectName = string.Format("Error_{0}", objectSuffix);

            chart.DrawStaticText(chartObjectName, message, VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);
        }

        /// <summary>
        /// Shows an error message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">the parameter value</param>
        public static void ShowInvalidParameterMessage(this Chart chart, string parameterName, object parameterValue)
        {
            string message = string.Format("The value ({0}) you provided for '{1}' isn't valid", parameterValue, parameterName);

            string objectSuffix = string.Format("Error_{0}_{1}_{2}", parameterName, parameterValue, DateTime.Now.Ticks);

            chart.ShowInvalidParameterMessage(parameterName, parameterValue, VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);
        }

        /// <summary>
        /// Shows an error message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">the parameter value</param>
        /// <param name="verticalAlignment">The vertical alignment of error message on chart</param>
        /// <param name="horizontalAlignment">The horizontal alignment of error message on chart</param>
        /// <param name="color">The color of error message on chart</param>
        public static void ShowInvalidParameterMessage(this Chart chart, string parameterName, object parameterValue, VerticalAlignment verticalAlignment,
            HorizontalAlignment horizontalAlignment, Color color)
        {
            string message = string.Format("The value ({0}) you provided for '{1}' isn't valid", parameterValue, parameterName);

            string objectSuffix = string.Format("Error_{0}_{1}_{2}", parameterName, parameterValue, DateTime.Now.Ticks);

            chart.ShowError(message, objectSuffix, verticalAlignment, horizontalAlignment, color);
        }
    }
}