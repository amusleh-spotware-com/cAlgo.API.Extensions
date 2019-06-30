using cAlgo.API.Internals;
using System;
using System.Xml.Serialization;

namespace cAlgo.API.Extensions.Models
{
    public class Signal
    {
        public TradeType TradeType { get; set; }

        public DateTime Time { get; set; }

        [XmlIgnore]
        public int Index { get; set; }

        [XmlIgnore]
        public double MaxUpMove { get; set; }

        [XmlIgnore]
        public double MaxDownMove { get; set; }

        [XmlIgnore]
        public Symbol Symbol { get; set; }

        [XmlIgnore]
        public double RewardRiskRatio
        {
            get
            {
                if (TradeType == TradeType.Buy)
                {
                    return MaxDownMove > 0 ? MaxUpMove / MaxDownMove : MaxUpMove / Symbol.TickSize;
                }
                else
                {
                    return MaxUpMove > 0 ? MaxDownMove / MaxUpMove : MaxDownMove / Symbol.TickSize;
                }
            }
        }

        [XmlIgnore]
        public string Comment { get; set; }

        [XmlIgnore]
        public bool Exited { get; set; }

        [XmlIgnore]
        public int ExitIndex { get; set; }

        [XmlIgnore]
        public TimeSpan HoldingTime { get; set; }
    }
}