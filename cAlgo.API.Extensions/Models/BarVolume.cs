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

        public double BullishVolumePercent
        {
            get
            {
                return BullishVolume / TotalVolume * 100;
            }
        }

        public double BearishVolumePercent
        {
            get
            {
                return BearishVolume / TotalVolume * 100;
            }
        }

        public double TotalVolume
        {
            get
            {
                return BullishVolume + BearishVolume;
            }
        }
    }
}