using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions.Helpers
{
    public class RenkoMarketSeries : IndicatorMarketSeries
    {
        #region Fields

        private readonly Symbol _symbol;

        private readonly double _size;

        #endregion Fields

        public RenkoMarketSeries(double sizeInPips, Symbol symbol, Algo algo) : base(TimeFrame.Minute, symbol.Name, new IndicatorTimeSeries(), algo)
        {
            _symbol = symbol;

            _size = sizeInPips * _symbol.PipSize;
        }

        #region Delegates

        public delegate void OnBarHandler(object sender, Bar newBar, Bar oldBar);

        #endregion Delegates

        #region Events

        public event OnBarHandler OnBar;

        #endregion Events

        #region Methods

        public void OnTick()
        {
            double price = _symbol.Bid;

            if (double.IsNaN(Open.LastValue))
            {
                Insert(0, price, price, price, price, 0, Algo.Server.TimeInUtc);
            }

            double range = Math.Abs(this.GetBarRange(Index, true));

            if ((range >= _size && (Index == 0 || this.GetBarType(Index) == this.GetBarType(Index - 1))) || (range >= _size * 2))
            {
                Bar bar = new Bar
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

                OnBar?.Invoke(this, bar, this.GetBar(Index));
            }

            Insert(Index, _symbol.ToTicks(range), SeriesType.TickVolume);

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