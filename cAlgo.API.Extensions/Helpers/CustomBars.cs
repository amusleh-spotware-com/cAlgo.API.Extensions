using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Helpers
{
    public class CustomBars : IndicatorBars
    {
        #region Fields

        private readonly Bars _bars;

        private readonly TimeSpan _gmtOffset, _timeFrameSpan, _marketSeriesTimeFrameSpan;

        private int _barStartIndex, _barEndIndex;

        private DateTime _barStartTime, _barEndTime, _nextBarTime;

        private OhlcBar _lastBar;

        #endregion Fields

        public CustomBars(Bars bars, TimeFrame timeFrame, TimeSpan gmtOffset) : base(timeFrame, bars.SymbolName)
        {
            _bars = bars;

            _gmtOffset = gmtOffset;

            _timeFrameSpan = timeFrame.GetSpan();

            _marketSeriesTimeFrameSpan = bars.TimeFrame.GetSpan();
        }

        #region Properties

        public int BarStartIndex => _barStartIndex;

        public int BarEndIndex => _barEndIndex;

        public DateTime BarStartTime => _barStartTime;

        public DateTime BarEndTime => _barEndTime;

        public OhlcBar LastOhlcBar => _lastBar;

        public Action<string> Print { get; set; }

        #endregion Properties

        #region Methods

        public void Calculate(int barIndex)
        {
            DateTime barOpenTime = _bars.OpenTimes[barIndex].Add(-_gmtOffset);

            if (_lastBar == null || barOpenTime >= _nextBarTime)
            {
                _barStartIndex = barIndex;
                _barEndIndex = barIndex + 1;

                _barStartTime = _bars.OpenTimes[_barStartIndex];
                _barEndTime = _bars.OpenTimes[_barStartIndex].Add(_marketSeriesTimeFrameSpan);

                _lastBar = new OhlcBar
                {
                    Open = _bars.OpenPrices[barIndex],
                    Index = Index + 1,
                    Time = _bars.OpenTimes[barIndex].Add(-_gmtOffset)
                };

                _nextBarTime = _lastBar.Time.Add(_timeFrameSpan);
            }
            else
            {
                _barEndIndex = barIndex + 1;
                _barEndTime = _bars.OpenTimes[barIndex].Add(_marketSeriesTimeFrameSpan);
            }

            _lastBar.High = _bars.HighPrices.Maximum(_barStartIndex, barIndex);
            _lastBar.Low = _bars.LowPrices.Minimum(_barStartIndex, barIndex);
            _lastBar.Close = _bars.ClosePrices[barIndex];

            Insert(_lastBar);
        }

        #endregion Methods
    }
}