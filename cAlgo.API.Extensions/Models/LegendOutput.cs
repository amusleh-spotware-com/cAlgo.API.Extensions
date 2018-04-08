using System;

namespace cAlgo.API.Extensions
{
    public class LegendOutput
    {
        #region Constructors

        public LegendOutput()
        {
        }

        public LegendOutput(string name, Colors color)
        {
            Name = name;

            Color = color;
        }

        #endregion Constructors

        #region Properties

        public string Name { get; set; }

        public Colors Color { get; set; }

        #endregion Properties
    }
}