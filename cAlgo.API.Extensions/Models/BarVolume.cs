namespace cAlgo.API.Extensions.Models
{
    public class BarVolume
    {
        public double BullishVolume { get; set; }

        public double BearishVolume { get; set; }

        public double Delta
        {
            get
            {
                return BullishVolume - BearishVolume;
            }
        }
    }
}