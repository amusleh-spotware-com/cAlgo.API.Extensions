using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System;
using cAlgo.API.Extensions.Types;

namespace cAlgo.API.Extensions
{
    public class IndicatorMarketSeries : MarketSeries
    {
        #region Fields

        private readonly Algo _algo;

        private readonly IndicatorDataSeries _open, _close, _high, _low, _tickVolume, _median, _typical, _weighted, _weightedClose;

        private readonly TimeSeries _openTime;

        private readonly TimeFrame _timeFrame;

        private readonly string _symbolCode;

        #endregion Fields

        #region Constructor

        public IndicatorMarketSeries(TimeFrame timeFrame, string symbolCode, Algo algo): this(timeFrame, symbolCode, new IndicatorTimeSeries(), algo)
        {
        }

        public IndicatorMarketSeries(TimeFrame timeFrame, string symbolCode, TimeSeries timeSeries, Algo algo)
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

            _symbolCode = symbolCode;
        }

        #endregion Constructor

        #region Properties

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

        public string SymbolCode
        {
            get
            {
                return _symbolCode;
            }
        }

        public int Index
        {
            get
            {
                return Close.Count - 1;
            }
        }

        #endregion Properties

        #region Methods

        public void Insert(int index, double value, SeriesType seriesType)
        {
            ((IndicatorDataSeries)this.GetSeries(seriesType))[index] = value;
        }

        public void AddNewBar(int index, double openPrice, DateTime openTime)
        {
            Insert(index, openPrice, SeriesType.Open);
            Insert(index, openPrice, SeriesType.High);
            Insert(index, openPrice, SeriesType.Low);
            Insert(index, openPrice, SeriesType.Close);
        }

        public void CalculateHeikenAshi(MarketSeries marketSeries, int periods = 1)
        {
            int index = _algo.MarketSeries.GetIndex();
            int seriesIndex = marketSeries.OpenTime.GetIndexByTime(_algo.MarketSeries.OpenTime[index]);

            double barOhlcSum = marketSeries.Open[seriesIndex] + marketSeries.Low[seriesIndex] +
                marketSeries.High[seriesIndex] + marketSeries.Close[seriesIndex];

            _close[seriesIndex] = Round(barOhlcSum / 4);

            if (_open.Count < periods || double.IsNaN(_open[seriesIndex - 1]))
            {
                _open[seriesIndex] = Round((marketSeries.Open[seriesIndex] + marketSeries.Close[seriesIndex]) / 2);
                _high[seriesIndex] = marketSeries.High[seriesIndex];
                _low[seriesIndex] = marketSeries.Low[seriesIndex];
            }
            else
            {
                _open[seriesIndex] = Round((_open[seriesIndex - 1] + _close[seriesIndex - 1]) / 2);
                _high[seriesIndex] = Math.Max(marketSeries.High[seriesIndex], Math.Max(Open[seriesIndex], Close[seriesIndex]));
                _low[seriesIndex] = Math.Min(marketSeries.Low[seriesIndex], Math.Min(Open[seriesIndex], Close[seriesIndex]));
            }
        }

        public void CalculateHeikenAshiSmoothed(MarketSeries marketSeries, int maPeriods, MovingAverageType maType, int periods = 1)
        {
            int index = _algo.MarketSeries.GetIndex();
            int seriesIndex = marketSeries.OpenTime.GetIndexByTime(_algo.MarketSeries.OpenTime[index]);

            if (seriesIndex <= maPeriods)
            {
                return;
            }

            double barMaOpen = GetSeriesMovingAverageValue(marketSeries, SeriesType.Open, maPeriods, maType, seriesIndex);
            double barMaHigh = GetSeriesMovingAverageValue(marketSeries, SeriesType.High, maPeriods, maType, seriesIndex);
            double barMaLow = GetSeriesMovingAverageValue(marketSeries, SeriesType.Low, maPeriods, maType, seriesIndex);
            double barMaClose = GetSeriesMovingAverageValue(marketSeries, SeriesType.Close, maPeriods, maType, seriesIndex);

            _close[seriesIndex] = (barMaOpen + barMaClose + barMaHigh + barMaLow) / 4;

            if (seriesIndex < periods || double.IsNaN(_open[seriesIndex - 1]))
            {
                _open[seriesIndex] = (barMaOpen + barMaClose) / 2;
                _high[seriesIndex] = barMaHigh;
                _low[seriesIndex] = barMaLow;
            }
            else
            {
                _open[seriesIndex] = (_open[seriesIndex - 1] + _close[seriesIndex - 1]) / 2;
                _high[seriesIndex] = Math.Max(barMaHigh, Math.Max(_open[seriesIndex], _close[seriesIndex]));
                _low[seriesIndex] = Math.Min(barMaLow, Math.Min(_open[seriesIndex], _close[seriesIndex]));
            }
        }

        public double GetSeriesMovingAverageValue(
            MarketSeries marketSeries, SeriesType seriesType, int periods, MovingAverageType type, int index)
        {
            DataSeries series = marketSeries.GetSeries(seriesType);

            MovingAverage ma = _algo.Indicators.MovingAverage(series, periods, type);

            return ma.Result[index];
        }

        public double Round(double input)
        {
            return Math.Round(input, _algo.Symbol.Digits);
        }

        #endregion Methods
    }
}