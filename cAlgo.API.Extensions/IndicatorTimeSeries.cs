using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using cAlgo.API;

namespace cAlgo.API.Extensions
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

        public int GetIndexByExactTime(DateTime dateTime)
        {
            return _series.IndexOf(dateTime);
        }

        public int GetIndexByTime(DateTime dateTime)
        {
            return _series.FindIndex(dt => dt == dateTime);
        }

        public DateTime Last(int index)
        {
            return _series[_series.Count - index];
        }

        public void Add(DateTime time)
        {
            _series.Add(time);
        }

        public void Insert(int index, DateTime time)
        {
            _series.Insert(index, time);
        }

        #endregion Methods
    }
}