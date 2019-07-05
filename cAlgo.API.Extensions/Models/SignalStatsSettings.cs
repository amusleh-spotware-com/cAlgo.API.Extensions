using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class SignalStatsSettings
    {
        public IndicatorDataSeries ProfitableSignal { get; set; }

        public IndicatorDataSeries LosingSignal { get; set; }

        public MarketSeries MarketSeries { get; set; }

        public double SignalDistance { get; set; } = 1;

        public int MaxLookupBarsNumber { get; set; } = 20;

        public double MinRewardRiskRatio { get; set; }

        public Func<Signal, bool> ExitFunction { get; set; }

        public Chart Chart { get; set; }

        public string ChartObjectNamesSuffix { get; set; }

        public bool ShowExits { get; set; }

        public IndicatorDataSeries BuyExit { get; set; }

        public IndicatorDataSeries SellExit { get; set; }

        public Color BuySignalExitLineColor { get; set; } = Color.Lime;

        public Color SellSignalExitLineColor { get; set; } = Color.Red;

        public Color StatsColor { get; set; } = Color.Red;

        public VerticalAlignment StatsVerticalAlignment { get; set; } = VerticalAlignment.Top;

        public HorizontalAlignment StatsHorizontalAlignment { get; set; } = HorizontalAlignment.Left;
    }
}