using cAlgo.API.Extensions.Enums;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class Bar
    {
        public int Index { get; set; }

        public DateTime Time { get; set; }
        public double Open { get; set; }

        public double High { get; set; }

        public double Low { get; set; }

        public double Close { get; set; }

        public double Volume { get; set; }

        public BarType Type { get; set; }

        public double Range
        {
            get
            {
                return High - Low;
            }
        }

        public double BodyRange
        {
            get
            {
                return Math.Abs(Close - Open);
            }
        }
    }
}