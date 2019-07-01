using System;

namespace cAlgo.API.Extensions
{
    public static class ChartAreaExtensions
    {
        /// <summary>
        /// Shows a message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The message</param>
        public static ChartStaticText ShowMessage(this ChartArea chartArea, string message)
        {
            string objectSuffix = string.Format("No_Suffix_{0}", DateTime.Now.Ticks);

            return chartArea.ShowMessage(message, objectSuffix);
        }

        /// <summary>
        /// Shows a message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The message</param>
        /// <param name="objectSuffix">The optional suffix that will be appended at the end of message object</param>
        public static ChartStaticText ShowMessage(this ChartArea chartArea, string message, string objectSuffix)
        {
            return chartArea.ShowMessage(message, objectSuffix, VerticalAlignment.Center, HorizontalAlignment.Center, Color.Red);
        }

        /// <summary>
        /// Shows a message on your chart
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="message">The message</param>
        /// <param name="objectSuffix">The optional suffix that will be appended at the end of message object</param>
        /// <param name="verticalAlignment">The vertical alignment of message on chart</param>
        /// <param name="horizontalAlignment">The horizontal alignment of message on chart</param>
        /// <param name="color">The color of message on chart</param>
        public static ChartStaticText ShowMessage(this ChartArea chartArea, string message, string objectSuffix,
            VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Color color)
        {
            objectSuffix = objectSuffix ?? string.Format("No_Suffix_{0}", DateTime.Now.Ticks);

            string chartObjectName = string.Format("Message_{0}", objectSuffix);

            return chartArea.DrawStaticText(chartObjectName, message, verticalAlignment, horizontalAlignment, color);
        }

        /// <summary>
        /// Shows a message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">The parameter value</param>
        public static ChartStaticText ShowInvalidParameterMessage(this ChartArea chartArea, string parameterName, object parameterValue)
        {
            return chartArea.ShowInvalidParameterMessage(parameterName, parameterValue, VerticalAlignment.Center, HorizontalAlignment.Center,
                Color.Red);
        }

        /// <summary>
        /// Shows a message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">The parameter value</param>
        /// <param name="chartObjectsSuffix">The chart objects suffix</param>
        public static ChartStaticText ShowInvalidParameterMessage(this ChartArea chartArea, string parameterName, object parameterValue,
            string chartObjectsSuffix)
        {
            return chartArea.ShowInvalidParameterMessage(parameterName, parameterValue, chartObjectsSuffix, VerticalAlignment.Center,
                HorizontalAlignment.Center, Color.Red);
        }

        /// <summary>
        /// Shows a message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">the parameter value</param>
        /// <param name="verticalAlignment">The vertical alignment of message on chart</param>
        /// <param name="horizontalAlignment">The horizontal alignment of message on chart</param>
        /// <param name="color">The color of message on chart</param>
        public static ChartStaticText ShowInvalidParameterMessage(this ChartArea chartArea, string parameterName, object parameterValue,
            VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Color color)
        {
            string chartObjectsSuffix = string.Format("InvalidParameterValue_{0}_{1}_{2}", parameterName, parameterValue, DateTime.Now.Ticks);

            return chartArea.ShowInvalidParameterMessage(parameterName, parameterValue, chartObjectsSuffix, verticalAlignment,
                horizontalAlignment, color);
        }

        /// <summary>
        /// Shows a message with the provided parameter name and value to notice the user that his/her provided value isn't valid
        /// </summary>
        /// <param name="chart">The chart</param>
        /// <param name="parameterName">The parameter name</param>
        /// <param name="parameterValue">the parameter value</param>
        /// <param name="verticalAlignment">The vertical alignment of message on chart</param>
        /// <param name="horizontalAlignment">The horizontal alignment of message on chart</param>
        /// <param name="color">The color of message on chart</param>
        public static ChartStaticText ShowInvalidParameterMessage(this ChartArea chartArea, string parameterName, object parameterValue,
            string chartObjectsSuffix, VerticalAlignment verticalAlignment, HorizontalAlignment horizontalAlignment, Color color)
        {
            string message = string.Format("The value ({0}) you provided for '{1}' isn't valid", parameterValue, parameterName);

            return chartArea.ShowMessage(message, chartObjectsSuffix, verticalAlignment, horizontalAlignment, color);
        }
    }
}