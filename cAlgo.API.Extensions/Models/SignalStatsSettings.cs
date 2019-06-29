using cAlgo.API.Internals;
using cAlgo.API.Extensions.Enums;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class SignalStatsSettings
    {
        public IndicatorDataSeries ProfitableSignal { get; set; }

        public IndicatorDataSeries LosingSignal { get; set; }

        public MarketSeries MarketSeries { get; set; }

        public double SignalDistance { get; set; }

        public int MaxLookupBarsNumber { get; set; }

        public double MinRewardRiskRatio { get; set; }

        public Func<Signal, bool> ExitFunction { get; set; }
    }
}