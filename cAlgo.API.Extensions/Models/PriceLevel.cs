using System.Collections.Generic;

namespace cAlgo.API.Extensions
{
    public class PriceLevel
    {
        #region Properties

        public double Level { get; set; }
        public long BullishVolume { get; set; }
        public long BearishVolume { get; set; }

        public long TotalVolume
        {
            get
            {
                return BullishVolume + BearishVolume;
            }
        }

        public List<int> Profile { get; set; }

        #endregion Properties

        #region Methods

        public static bool operator !=(PriceLevel obj1, PriceLevel obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                return !object.ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(PriceLevel obj1, PriceLevel obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                return object.ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PriceLevel))
            {
                return false;
            }

            return Equals((PriceLevel)obj);
        }

        public bool Equals(PriceLevel other)
        {
            return other != null && other.Level == Level;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + Level.GetHashCode();

            return hash;
        }

        #endregion Methods
    }
}