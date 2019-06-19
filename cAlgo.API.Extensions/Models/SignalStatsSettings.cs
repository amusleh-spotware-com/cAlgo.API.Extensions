using cAlgo.API.Internals;

namespace cAlgo.API.Extensions.Models
{
    public class SignalStatsSettings
    {
        public IndicatorDataSeries ProfitableSignal { get; set; }

        public IndicatorDataSeries LosingSignal { get; set; }

        public MarketSeries MarketSeries { get; set; }

        public double SignalDistance { get; set; }

        public bool IsCloseBased { get; set; }

        public int MaxLookupBarsNumber { get; set; }

        public double MinRewardRiskRatio { get; set; }
    }
}