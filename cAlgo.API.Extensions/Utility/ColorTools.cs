namespace cAlgo.API.Extensions.Utility
{
    public static class ColorTools
    {
        public static Color GetColor(string colorString, int alpha = 255)
        {
            var color = colorString[0] == '#' ? Color.FromHex(colorString) : Color.FromName(colorString);

            return Color.FromArgb(alpha, color);
        }
    }
}