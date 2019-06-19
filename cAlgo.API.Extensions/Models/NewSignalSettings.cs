using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class NewSignalSettings
    {
        public IndicatorDataSeries BuySignal { get; set; }

        public IndicatorDataSeries SellSignal { get; set; }

        public MarketSeries MarketSeries { get; set; }

        public double SignalDistance { get; set; }

        public Action<int, TradeType> AlertCallback { get; set; }
    }
}