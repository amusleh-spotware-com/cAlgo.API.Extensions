using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using cAlgo.API.Internals;
using System;
using System.Collections;
using System.Collections.Generic;

namespace cAlgo.API.Extensions.Helpers
{
    public class IndicatorBars : Bars
    {
        #region Fields

        private readonly Algo _algo;

        private readonly IndicatorDataSeries _open, _close, _high, _low, _tickVolume, _median, _typical, _weighted, _weightedClose;

        private readonly TimeSeries _openTime;

        private readonly TimeFrame _timeFrame;

        private readonly string _symbolName;

        public event Action<BarsHistoryLoadedEventArgs> HistoryLoaded;
        public event Action<BarsHistoryLoadedEventArgs> Reloaded;
        public event Action<BarsTickEventArgs> Tick;
        public event Action<BarOpenedEventArgs> BarOpened;

        #endregion Fields

        #region Constructor

        public IndicatorBars(TimeFrame timeFrame, string symbolName)
        {
            _open = new CustomDataSeries();
            _close = new CustomDataSeries();
            _high = new CustomDataSeries();
            _low = new CustomDataSeries();
            _tickVolume = new CustomDataSeries();
            _median = new CustomDataSeries();
            _typical = new CustomDataSeries();
            _weighted = new CustomDataSeries();
            _weightedClose = new CustomDataSeries();

            _openTime = new IndicatorTimeSeries();

            _timeFrame = timeFrame;

            _symbolName = symbolName;
        }

        public IndicatorBars(TimeFrame timeFrame, string symbolName, Algo algo) : this(timeFrame, symbolName, new IndicatorTimeSeries(), algo)
        {
        }

        public IndicatorBars(TimeFrame timeFrame, string symbolName, TimeSeries timeSeries, Algo algo)
        {
            _algo = algo;

            _open = algo.CreateDataSeries();
            _close = algo.CreateDataSeries();
            _high = algo.CreateDataSeries();
            _low = algo.CreateDataSeries();
            _tickVolume = algo.CreateDataSeries();
            _median = algo.CreateDataSeries();
            _typical = algo.CreateDataSeries();
            _weighted = algo.CreateDataSeries();
            _weightedClose = algo.CreateDataSeries();

            _openTime = timeSeries;

            _timeFrame = timeFrame;

            _symbolName = symbolName;
        }

        #endregion Constructor

        #region Properties

        protected Algo Algo
        {
            get
            {
                return _algo;
            }
        }

        public DataSeries Open
        {
            get
            {
                return _open;
            }
        }

        public DataSeries High
        {
            get
            {
                return _high;
            }
        }

        public DataSeries Low
        {
            get
            {
                return _low;
            }
        }

        public DataSeries Close
        {
            get
            {
                return _close;
            }
        }

        public DataSeries TickVolume
        {
            get
            {
                return _tickVolume;
            }
        }

        public DataSeries Median
        {
            get
            {
                return _median;
            }
        }

        public DataSeries Typical
        {
            get
            {
                return _typical;
            }
        }

        public DataSeries Weighted
        {
            get
            {
                return _weighted;
            }
        }

        public DataSeries WeightedClose
        {
            get
            {
                return _weightedClose;
            }
        }

        public TimeSeries OpenTime
        {
            get
            {
                return _openTime;
            }
        }

        public TimeFrame TimeFrame
        {
            get
            {
                return _timeFrame;
            }
        }

        public int Index
        {
            get
            {
                return Close.Count - 1;
            }
        }

        public string SymbolName
        {
            get
            {
                return _symbolName;
            }
        }

        public string SymbolCode
        {
            get
            {
                return SymbolName;
            }
        }

        public Bar LastBar => throw new NotImplementedException();

        public int Count => Close.Count;

        public DataSeries OpenPrices => Open;

        public DataSeries HighPrices => High;

        public DataSeries LowPrices => Low;

        public DataSeries ClosePrices => Close;

        public DataSeries TickVolumes => TickVolume;

        public DataSeries MedianPrices => Median;

        public DataSeries TypicalPrices => Typical;

        public DataSeries WeightedPrices => Weighted;

        public TimeSeries OpenTimes => OpenTime;

        public Bar this[int index] => throw new NotImplementedException();

        #endregion Properties

        #region Methods

        public void Insert(int index, double value, SeriesType seriesType)
        {
            ((IndicatorDataSeries)this.GetSeries(seriesType))[index] = value;
        }

        public void Insert(OhlcBar bar)
        {
            Insert(bar.Index, bar.Open, bar.High, bar.Low, bar.Close, bar.Volume, bar.Time);
        }

        public void Insert(int index, double open, double high, double low, double close, double volume, DateTime openTime)
        {
            Insert(index, open, SeriesType.Open);
            Insert(index, high, SeriesType.High);
            Insert(index, low, SeriesType.Low);
            Insert(index, close, SeriesType.Close);
            Insert(index, volume, SeriesType.TickVolume);

            if (_openTime is IndicatorTimeSeries)
            {
                (_openTime as IndicatorTimeSeries).Insert(index, openTime);
            }
        }

        public void CalculateHeikenAshi(Bars bars, int index, int periods = 1)
        {
            var seriesIndex = bars.OpenTimes.GetIndexByTime(bars.OpenTimes[index]);

            var barOhlcSum = bars.OpenPrices[seriesIndex] + bars.LowPrices[seriesIndex] +
                             bars.HighPrices[seriesIndex] + bars.ClosePrices[seriesIndex];

            _close[seriesIndex] = _algo.Symbol.Round(barOhlcSum / 4);

            if (_open.Count < periods || double.IsNaN(_open[seriesIndex - 1]))
            {
                _open[seriesIndex] = _algo.Symbol.Round((bars.OpenPrices[seriesIndex] + bars.ClosePrices[seriesIndex]) / 2);
                _high[seriesIndex] = bars.HighPrices[seriesIndex];
                _low[seriesIndex] = bars.LowPrices[seriesIndex];
            }
            else
            {
                _open[seriesIndex] = _algo.Symbol.Round((_open[seriesIndex - periods] + _close[seriesIndex - periods]) / 2);
                _high[seriesIndex] = Math.Max(bars.HighPrices[seriesIndex], Math.Max(_open[seriesIndex], _close[seriesIndex]));
                _low[seriesIndex] = Math.Min(bars.LowPrices[seriesIndex], Math.Min(_open[seriesIndex], _close[seriesIndex]));
            }
        }

        public void CalculateHeikenAshi(Bars bars, int index, int maPeriods, MovingAverageType maType, int periods = 1)
        {
            var seriesIndex = bars.OpenTimes.GetIndexByTime(bars.OpenTimes[index]);

            if (seriesIndex <= maPeriods)
            {
                return;
            }

            var barMaOpen = GetSeriesMovingAverageValue(bars, SeriesType.Open, maPeriods, maType, seriesIndex);
            var barMaHigh = GetSeriesMovingAverageValue(bars, SeriesType.High, maPeriods, maType, seriesIndex);
            var barMaLow = GetSeriesMovingAverageValue(bars, SeriesType.Low, maPeriods, maType, seriesIndex);
            var barMaClose = GetSeriesMovingAverageValue(bars, SeriesType.Close, maPeriods, maType, seriesIndex);

            _close[seriesIndex] = (barMaOpen + barMaClose + barMaHigh + barMaLow) / 4;

            if (seriesIndex < periods || double.IsNaN(_open[seriesIndex - 1]))
            {
                _open[seriesIndex] = (barMaOpen + barMaClose) / 2;
                _high[seriesIndex] = barMaHigh;
                _low[seriesIndex] = barMaLow;
            }
            else
            {
                _open[seriesIndex] = (_open[seriesIndex - periods] + _close[seriesIndex - periods]) / 2;
                _high[seriesIndex] = Math.Max(barMaHigh, Math.Max(_open[seriesIndex], _close[seriesIndex]));
                _low[seriesIndex] = Math.Min(barMaLow, Math.Min(_open[seriesIndex], _close[seriesIndex]));
            }
        }

        public double GetSeriesMovingAverageValue(
            Bars bars, SeriesType seriesType, int periods, MovingAverageType type, int index)
        {
            var series = bars.GetSeries(seriesType);

            var ma = _algo.Indicators.MovingAverage(series, periods, type);

            return ma.Result[index];
        }

        public Bar Last(int index)
        {

            throw new NotImplementedException();
        }

        public int LoadMoreHistory()
        {
            throw new NotImplementedException();
        }

        public void LoadMoreHistoryAsync()
        {
            throw new NotImplementedException();
        }

        public void LoadMoreHistoryAsync(Action<BarsHistoryLoadedEventArgs> callback)
        {
            throw new NotImplementedException();
        }

        public IEnumerator<Bar> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        #endregion Methods
    }
}