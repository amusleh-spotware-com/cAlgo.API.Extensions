using System;

namespace cAlgo.API.Extensions
{
    public static class TimeFrameExtensions
    {
        /// <summary>
        /// Returns the approximate amount of time each time frame represent
        /// This method only works for time based time frames
        /// </summary>
        /// <param name="timeFrame"></param>
        /// <returns>TimeSpan</returns>
        public static TimeSpan GetSpan(this TimeFrame timeFrame)
        {
            if (timeFrame == TimeFrame.Minute)
            {
                return TimeSpan.FromMinutes(1);
            }
            else if (timeFrame == TimeFrame.Minute2)
            {
                return TimeSpan.FromMinutes(2);
            }
            else if (timeFrame == TimeFrame.Minute3)
            {
                return TimeSpan.FromMinutes(3);
            }
            else if (timeFrame == TimeFrame.Minute4)
            {
                return TimeSpan.FromMinutes(4);
            }
            else if (timeFrame == TimeFrame.Minute5)
            {
                return TimeSpan.FromMinutes(5);
            }
            else if (timeFrame == TimeFrame.Minute6)
            {
                return TimeSpan.FromMinutes(6);
            }
            else if (timeFrame == TimeFrame.Minute7)
            {
                return TimeSpan.FromMinutes(7);
            }
            else if (timeFrame == TimeFrame.Minute8)
            {
                return TimeSpan.FromMinutes(8);
            }
            else if (timeFrame == TimeFrame.Minute9)
            {
                return TimeSpan.FromMinutes(9);
            }
            else if (timeFrame == TimeFrame.Minute10)
            {
                return TimeSpan.FromMinutes(10);
            }
            else if (timeFrame == TimeFrame.Minute15)
            {
                return TimeSpan.FromMinutes(15);
            }
            else if (timeFrame == TimeFrame.Minute20)
            {
                return TimeSpan.FromMinutes(20);
            }
            else if (timeFrame == TimeFrame.Minute30)
            {
                return TimeSpan.FromMinutes(30);
            }
            else if (timeFrame == TimeFrame.Minute45)
            {
                return TimeSpan.FromMinutes(45);
            }
            else if (timeFrame == TimeFrame.Hour)
            {
                return TimeSpan.FromHours(1);
            }
            else if (timeFrame == TimeFrame.Hour2)
            {
                return TimeSpan.FromHours(2);
            }
            else if (timeFrame == TimeFrame.Hour3)
            {
                return TimeSpan.FromHours(3);
            }
            else if (timeFrame == TimeFrame.Hour4)
            {
                return TimeSpan.FromHours(4);
            }
            else if (timeFrame == TimeFrame.Hour6)
            {
                return TimeSpan.FromHours(6);
            }
            else if (timeFrame == TimeFrame.Hour8)
            {
                return TimeSpan.FromHours(8);
            }
            else if (timeFrame == TimeFrame.Hour12)
            {
                return TimeSpan.FromHours(12);
            }
            else if (timeFrame == TimeFrame.Daily)
            {
                return TimeSpan.FromDays(1);
            }
            else if (timeFrame == TimeFrame.Day2)
            {
                return TimeSpan.FromDays(2);
            }
            else if (timeFrame == TimeFrame.Day3)
            {
                return TimeSpan.FromDays(3);
            }
            else if (timeFrame == TimeFrame.Weekly)
            {
                return TimeSpan.FromDays(7);
            }
            else if (timeFrame == TimeFrame.Monthly)
            {
                return TimeSpan.FromDays(30);
            }
            else
            {
                throw new NotSupportedException("The provided time frame type isn't supported");
            }
        }

        /// <summary>
        /// Returns the next date time value based on a give current time and time frame spane
        /// </summary>
        /// <param name="timeFrame"></param>
        /// <param name="currentTime">The current time which will be advanved one unit based on time frame span amount</param>
        /// <returns>DateTime</returns>
        public static DateTime GetNext(this TimeFrame timeFrame, DateTime currentTime)
        {
            return timeFrame == TimeFrame.Monthly ? currentTime.AddMonths(1) : currentTime.Add(timeFrame.GetSpan());
        }
    }
}