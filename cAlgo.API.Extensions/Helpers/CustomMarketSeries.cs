using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Helpers
{
    public class CustomMarketSeries : IndicatorMarketSeries
    {
        #region Fields

        private readonly MarketSeries _marketSeries;

        private readonly TimeSpan _gmtOffset, _timeFrameSpan, _marketSeriesTimeFrameSpan;

        private int _barStartIndex, _barEndIndex;

        private DateTime _barStartTime, _barEndTime;

        private Bar _lastBar;

        #endregion Fields

        public CustomMarketSeries(MarketSeries marketSeries, TimeFrame timeFrame, TimeSpan gmtOffset) : base(timeFrame, marketSeries.SymbolCode)
        {
            _marketSeries = marketSeries;

            _gmtOffset = gmtOffset;

            _timeFrameSpan = timeFrame.GetSpan();

            _marketSeriesTimeFrameSpan = marketSeries.TimeFrame.GetSpan();
        }

        #region Properties

        public int BarStartIndex => _barStartIndex;

        public int BarEndIndex => _barEndIndex;

        public DateTime BarStartTime => _barStartTime;

        public DateTime BarEndTime => _barEndTime;

        public Bar LastBar => _lastBar;

        public Action<string> Print { get; set; }

        #endregion Properties

        #region Methods

        public void Calculate(int barIndex)
        {
            DateTime barOpenTime = _marketSeries.OpenTime[barIndex].Add(-_gmtOffset);

            if (_lastBar == null || _barEndTime - _barStartTime >= _timeFrameSpan)
            {
                _barStartIndex = barIndex;

                _barEndIndex = barIndex + 1;

                _barStartTime = _marketSeries.OpenTime[barIndex];

                _barEndTime = _marketSeries.OpenTime[barIndex].Add(_marketSeriesTimeFrameSpan);

                _lastBar = new Bar
                {
                    Open = _marketSeries.Open[barIndex],
                    Index = Index + 1,
                    Time = _marketSeries.OpenTime[barIndex].Add(-_gmtOffset)
                };
            }
            else
            {
                _barEndIndex = barIndex + 1;
                _barEndTime = _marketSeries.OpenTime[barIndex].Add(_marketSeriesTimeFrameSpan);
            }

            _lastBar.High = _marketSeries.High.Maximum(_barStartIndex, barIndex);
            _lastBar.Low = _marketSeries.Low.Minimum(_barStartIndex, barIndex);
            _lastBar.Close = _marketSeries.Close[barIndex];

            Insert(_lastBar);
        }

        #endregion Methods
    }
}