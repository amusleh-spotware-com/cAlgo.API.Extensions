namespace cAlgo.API.Extensions
{
    public class Divergence
    {
        public Types.DivergenceType Type { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public int BarsInBetween
        {
            get
            {
                return EndIndex - StartIndex;
            }
        }

        public string DrawingObjectName
        {
            get
            {
                return string.Format("{0} {1} {2}", Type, StartIndex, EndIndex);
            }
        }
    }
}