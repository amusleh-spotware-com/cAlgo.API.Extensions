using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Helpers
{
    public class CustomMarketSeries : IndicatorMarketSeries
    {
        #region Fields

        private readonly MarketSeries _marketSeries;

        private readonly TimeSpan _gmtOffset;

        private int _startBarIndex, _endBarIndex;

        private Bar _lastBar;

        #endregion Fields

        public CustomMarketSeries(MarketSeries marketSeries, TimeFrame timeFrame, TimeSpan gmtOffset, Algo algo) : base(timeFrame, marketSeries.SymbolCode, new IndicatorTimeSeries(), algo)
        {
            _marketSeries = marketSeries;

            _gmtOffset = gmtOffset;
        }

        #region Properties

        public int StartBarIndex => _startBarIndex;

        public int EndBarIndex => _endBarIndex;

        public Bar LastBar => _lastBar;

        #endregion Properties

        #region Methods

        public void Calculate(int index)
        {
            DateTime barOpenTime = _marketSeries.OpenTime[index].Add(-_gmtOffset);

            if (_lastBar == null || barOpenTime.TimeOfDay == TimeSpan.FromHours(0))
            {
                _startBarIndex = index;

                _endBarIndex = index + 1;

                _lastBar = new Bar
                {
                    Open = _marketSeries.Open[index],
                    Index = Index + 1,
                    Time = _marketSeries.OpenTime[index].Add(-_gmtOffset)
                };
            }
            else
            {
                _endBarIndex = index + 1;
            }

            _lastBar.High = _marketSeries.High.Maximum(_startBarIndex, index);
            _lastBar.Low = _marketSeries.Low.Minimum(_startBarIndex, index);
            _lastBar.Close = _marketSeries.Close[index];

            Insert(_lastBar);
        }

        #endregion Methods
    }
}