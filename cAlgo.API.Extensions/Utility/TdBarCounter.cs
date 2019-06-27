using cAlgo.API.Extensions.Enums;
using cAlgo.API.Extensions.Models;
using System.Collections.Generic;
using System;

namespace cAlgo.API.Extensions.Utility
{
    public class TdBarCounter
    {
        #region Fields

        private readonly int _maxBarNumber;

        private readonly int _period;

        private readonly DataSeries _series;

        private SortedSet<TdBar> _bars;

        private TdBar _lastBar;

        #endregion Fields

        public TdBarCounter(int maxBarNumber, int period, DataSeries series)
        {
            _maxBarNumber = maxBarNumber;

            _period = period;

            _series = series;

            _bars = new SortedSet<TdBar>();
        }

        #region Properties

        public SortedSet<TdBar> Bars
        {
            get
            {
                return _bars;
            }
        }

        public TdBar LastBar
        {
            get
            {
                return _lastBar;
            }
        }

        public Action<string> Print { get; set; }

        #endregion Properties

        #region Methods

        public void Calculate(int index)
        {
            // Sets last sequential bar to Null if the setup interrupted
            if (_lastBar != null)
            {
                if (index > _lastBar.Index)
                {
                    _bars.Add(_lastBar);
                }

                CancelCountIfInvalidated(index);
            }

            // Start new counting
            if (_lastBar == null)
            {
                StartNewCount(index);
            }
            // Continue count
            else
            {
                bool continueResult = ContinueCount(index);

                if (!continueResult)
                {
                    StartNewCount(index);
                }
            }
        }

        private void CancelCountIfInvalidated(int index)
        {
            if (_lastBar.Type == BarType.Bullish && _series[index] <= _series[index - _period])
            {
                _lastBar = null;
            }
            else if (_lastBar.Type == BarType.Bearish && _series[index] >= _series[index - _period])
            {
                _lastBar = null;
            }
        }

        private void StartNewCount(int index)
        {
            if (_series[index] > _series[index - _period])
            {
                _lastBar = new TdBar
                {
                    Type = BarType.Bullish
                };
            }
            else if (_series[index] < _series[index - _period])
            {
                _lastBar = new TdBar
                {
                    Type = BarType.Bearish
                };
            }

            if (_lastBar != null)
            {
                _lastBar.Index = index;
                _lastBar.Number = 1;
            }
        }

        private bool ContinueCount(int index)
        {
            bool result = true;

            if (index == _lastBar.Index)
            {
                return result;
            }

            // Setup completed
            if (_lastBar.Number >= _maxBarNumber)
            {
                result = false;
            }
            // Not completed yet
            else
            {
                _lastBar.Index = index;
                _lastBar.Number += 1;
            }

            return result;
        }

        #endregion Methods
    }
}