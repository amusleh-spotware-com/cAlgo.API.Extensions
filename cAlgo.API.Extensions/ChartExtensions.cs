using System;

namespace cAlgo.API.Extensions
{
    public static class ChartExtensions
    {
        public static void ScrollX(this Chart chart, TimeSpan interval)
        {
            var visibleBarTime = chart.Bars.GetOpenTime(interval > TimeSpan.Zero 
                ? chart.LastVisibleBarIndex
                : chart.FirstVisibleBarIndex);

            var scrollBarTime = visibleBarTime.Add(interval);

            chart.ScrollXTo(scrollBarTime);
        }
    }
}