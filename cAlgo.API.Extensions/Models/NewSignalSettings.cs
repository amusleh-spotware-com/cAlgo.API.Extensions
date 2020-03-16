using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class NewSignalSettings
    {
        public IndicatorDataSeries BuyEntry { get; set; }

        public IndicatorDataSeries SellEntry { get; set; }

        public Bars Bars { get; set; }

        public double SignalDistance { get; set; }

        public Action<int, TradeType> AlertCallback { get; set; }

        public bool IsExportEnabled { get; set; }

        public string ExportFilePath { get; set; }

        public int SignalsNumberToExport { get; set; } = 10;
    }
}