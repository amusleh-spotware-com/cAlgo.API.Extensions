using cAlgo.API.Extensions.Enums;
using System;

namespace cAlgo.API.Extensions.Models
{
    public class TdBar: IComparable
    {
        #region Properties

        public int Index { get; set; }
        public int Number { get; set; }
        public BarType Type { get; set; }


        #endregion

        #region Methods

        public int CompareTo(object obj)
        {
            return Index.CompareTo((obj as TdBar).Index);
        }

        #endregion
    }
}