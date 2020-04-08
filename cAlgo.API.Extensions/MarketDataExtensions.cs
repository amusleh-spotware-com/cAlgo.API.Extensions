using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions
{
    public static class MarketDataExtensions
    {
        public static TimeFrame GetTimeFrame(this MarketData marketData, string name)
        {
            name = name.ToLowerInvariant();

            switch (name)
            {
                case "minute":
                    return TimeFrame.Minute;

                case "monthly":
                    return TimeFrame.Monthly;

                case "weekly":
                    return TimeFrame.Weekly;

                case "day3":
                    return TimeFrame.Day3;

                case "day2":
                    return TimeFrame.Day2;

                case "daily":
                    return TimeFrame.Daily;

                case "hour12":
                    return TimeFrame.Hour12;

                case "hour8":
                    return TimeFrame.Hour8;

                case "hour6":
                    return TimeFrame.Hour6;

                case "hour4":
                    return TimeFrame.Hour4;

                case "hour3":
                    return TimeFrame.Hour3;

                case "hour":
                    return TimeFrame.Hour;

                case "minute45":
                    return TimeFrame.Minute45;

                case "hour2":
                    return TimeFrame.Hour2;

                case "minute20":
                    return TimeFrame.Minute20;

                case "minute2":
                    return TimeFrame.Minute2;

                case "minute3":
                    return TimeFrame.Minute3;

                case "minute4":
                    return TimeFrame.Minute4;

                case "minute30":
                    return TimeFrame.Minute30;

                case "minute6":
                    return TimeFrame.Minute6;

                case "minute5":
                    return TimeFrame.Minute5;

                case "minute8":
                    return TimeFrame.Minute8;

                case "minute9":
                    return TimeFrame.Minute9;

                case "minute10":
                    return TimeFrame.Minute10;

                case "minute15":
                    return TimeFrame.Minute15;

                case "minute7":
                    return TimeFrame.Minute7;

                default:
                    throw new ArgumentOutOfRangeException("name");
            }
        }
    }
}