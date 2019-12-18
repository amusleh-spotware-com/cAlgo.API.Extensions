using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.API.Extensions.Helpers
{
    public class IndicatorTimeSeries : TimeSeries
    {
        #region Fields

        private List<DateTime> _series;

        #endregion Fields

        #region Constructor

        public IndicatorTimeSeries()
        {
            _series = new List<DateTime>();
        }

        #endregion Constructor

        #region Properties

        public DateTime this[int index]
        {
            get
            {
                return _series[index];
            }
            set
            {
                _series[index] = value;
            }
        }

        public DateTime LastValue
        {
            get
            {
                return _series.LastOrDefault();
            }
        }

        public int Count
        {
            get
            {
                return _series.Count;
            }
        }

        public int Index
        {
            get
            {
                return Count - 1;
            }
        }

        #endregion Properties

        #region Methods

        public void Insert(int index, DateTime dateTime)
        {
            if (_series.Count > index)
            {
                _series.RemoveAt(index);
            }

            _series.Insert(index, dateTime);
        }

        public int GetIndexByExactTime(DateTime time)
        {
            return _series.IndexOf(time);
        }

        public int GetIndexByTime(DateTime time)
        {
            TimeSpan minTimeDiff = _series.Where(iTime => iTime <= time).Select(iTime => time - iTime).Min();

            return _series.FindIndex(iTime => time - iTime == minTimeDiff);
        }

        public DateTime Last(int index)
        {
            return _series[_series.Count - index];
        }

        public void Populate(TimeSeries timeSeries)
        {
            for (int i = 0; i < timeSeries.Count; i++)
            {
                _series.Add(timeSeries[i]);
            }
        }

        /*
        public IEnumerator<DateTime> GetEnumerator()
        {
            return _series.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _series.GetEnumerator();
        }
        */

        #endregion Methods
    }
}