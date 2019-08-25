using cAlgo.API.Extensions.Enums;

namespace cAlgo.API.Extensions.Models
{
    public class Bar
    {
        public int Index { get; set; }
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
    }
}