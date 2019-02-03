namespace cAlgo.API.Extensions.Factories
{
    public static class ColorFactory
    {
        public static Color GetColor(string colorString, int alpha = 255)
        {
            Color color = colorString[0] == '#' ? Color.FromHex(colorString) : Color.FromName(colorString);

            return Color.FromArgb(alpha, color);
        }
    }
}