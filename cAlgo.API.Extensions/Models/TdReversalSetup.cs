using cAlgo.API.Extensions.Enums;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class TdReversalSetup: IComparable
    {
        public TdReversalSetupType Type { get; set; }
        public int FirstSequentialBarIndex { get; set; }
        public int LastSequentialBarIndex { get; set; }
        public int FirstCountdownBarIndex { get; set; }
        public int LastCountdownBarIndex { get; set; }
        public int CountdownBarNumber { get; set; }
        public int EighthCountdownBarIndex { get; set; }

        public int CompareTo(object obj)
        {
            return FirstSequentialBarIndex.CompareTo((obj as TdReversalSetup).FirstSequentialBarIndex).CompareTo(LastSequentialBarIndex.CompareTo((obj as TdReversalSetup).LastSequentialBarIndex));
        }
    }
}