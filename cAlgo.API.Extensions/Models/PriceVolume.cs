namespace cAlgo.API.Extensions
{
    public class PriceVolume
    {
        #region Properties

        public double Price { get; set; }
        public long BullishVolume { get; set; }
        public long BearishVolume { get; set; }
        public long NeutralVolume { get; set; }

        public long TotalVolume
        {
            get
            {
                return BullishVolume + BearishVolume + NeutralVolume;
            }
        }

        #endregion Properties

        #region Methods

        public static bool operator !=(PriceVolume obj1, PriceVolume obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                return !object.ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(PriceVolume obj1, PriceVolume obj2)
        {
            if (object.ReferenceEquals(obj1, null))
            {
                return object.ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is PriceVolume))
            {
                return false;
            }

            return Equals((PriceVolume)obj);
        }

        public bool Equals(PriceVolume other)
        {
            return other != null && other.Price == Price;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + Price.GetHashCode();

            return hash;
        }

        #endregion Methods
    }
}