using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Helpers
{
    public class RangeBars : IndicatorBars
    {
        #region Fields

        private readonly Symbol _symbol;

        private readonly double _size;

        private double _previousBidPrice;

        #endregion Fields

        public RangeBars(double sizeInPips, Symbol symbol, Algo algo) : base(TimeFrame.Minute, symbol.Name, new IndicatorTimeSeries(), algo)
        {
            _symbol = symbol;

            _size = sizeInPips * _symbol.PipSize;
        }

        #region Delegates

        public delegate void OnBarHandler(object sender, OhlcBar newBar, OhlcBar oldBar);

        #endregion Delegates

        #region Events

        public event OnBarHandler OnBar;

        #endregion Events

        #region Methods

        public void OnTick()
        {
            var price = _symbol.Bid;

            if (price == _previousBidPrice)
            {
                return;
            }

            _previousBidPrice = price;

            if (double.IsNaN(Open.LastValue))
            {
                Insert(0, price, price, price, price, 0, Algo.Server.TimeInUtc);
            }

            var range = Math.Abs(this.GetBarRange(Index, true));

            if (range >= _size)
            {
                var bar = new OhlcBar
                {
                    Index = Index + 1,
                    Open = Close[Index],
                    High = Close[Index],
                    Low = Close[Index],
                    Close = Close[Index],
                    Volume = 0,
                    Time = Algo.Server.TimeInUtc
                };

                Insert(bar);

                OnBar?.Invoke(this, bar, this.GetBar(Index - 1));
            }

            Insert(Index, TickVolume.LastValue + 1, SeriesType.TickVolume);

            Insert(Index, price, SeriesType.Close);

            if (price > High[Index])
            {
                Insert(Index, price, SeriesType.High);
            }

            if (price < Low[Index])
            {
                Insert(Index, price, SeriesType.Low);
            }
        }

        #endregion Methods
    }
}