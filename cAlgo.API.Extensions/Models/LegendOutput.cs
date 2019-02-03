namespace cAlgo.API.Extensions
{
    public class LegendOutput
    {
        #region Constructors

        public LegendOutput()
        {
        }

        public LegendOutput(string name, Color color)
        {
            Name = name;

            Color = color;
        }

        #endregion Constructors

        #region Properties

        public string Name { get; set; }

        public Color Color { get; set; }

        #endregion Properties
    }
}