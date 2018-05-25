using cAlgo.API;
using cAlgo.API.Internals;
using System;

namespace cAlgo.API.Extensions
{
    public class IndicatorMarketSeries : MarketSeries
    {
        #region Fields

        private readonly IndicatorDataSeries _open, _close, _high, _low, _tickVolume, _median, _typical, _weighted, _weightedClose;

        private readonly IndicatorTimeSeries _openTime;

        private readonly TimeFrame _timeFrame;

        private readonly string _symbolCode;

        #endregion Fields

        #region Constructor

        public IndicatorMarketSeries(TimeFrame timeFrame, string symbolCode, Algo algo)
        {
            _open = algo.CreateDataSeries();
            _close = algo.CreateDataSeries();
            _high = algo.CreateDataSeries();
            _low = algo.CreateDataSeries();
            _tickVolume = algo.CreateDataSeries();
            _median = algo.CreateDataSeries();
            _typical = algo.CreateDataSeries();
            _weighted = algo.CreateDataSeries();
            _weightedClose = algo.CreateDataSeries();

            _openTime = new IndicatorTimeSeries();

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
            switch (seriesType)
            {
                case SeriesType.Open:
                    _open[index] = value;
                    break;

                case SeriesType.High:
                    _high[index] = value;
                    break;

                case SeriesType.Low:
                    _low[index] = value;
                    break;

                case SeriesType.Close:
                    _close[index] = value;
                    break;

                case SeriesType.Median:
                    _median[index] = value;
                    break;

                case SeriesType.TickVolume:
                    _tickVolume[index] = value;
                    break;

                case SeriesType.Typical:
                    _typical[index] = value;
                    break;

                case SeriesType.Weighted:
                    _weighted[index] = value;
                    break;

                case SeriesType.WeightedClose:
                    _weightedClose[index] = value;
                    break;

                default:
                    break;
            }
        }

        public void Insert(int index, DateTime value)
        {
            _openTime.Insert(index, value);
        }

        public void AddNewBar(int index, double openPrice, DateTime openTime)
        {
            Insert(index, openPrice, SeriesType.Open);
            Insert(index, openPrice, SeriesType.High);
            Insert(index, openPrice, SeriesType.Low);
            Insert(index, openPrice, SeriesType.Close);
            Insert(index, openTime);
        }

        #endregion Methods
    }
}