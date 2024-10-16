using System.Globalization;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using Point = System.Windows.Point;

namespace NotepadEx.Util;

public static class ColorUtil
{
    public static SolidColorBrush GetRandomColorBrush(byte minAlpha = 0, byte maxAlpha = 255) => new SolidColorBrush(Color.FromArgb((byte)Random.Shared.Next(minAlpha, maxAlpha + 1), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256)));

    public static LinearGradientBrush GetRandomLinearGradientBrush(byte minAlpha = 0, byte maxAlpha = 255)
    {
        LinearGradientBrush linearGradientBrush = new ();

        linearGradientBrush.StartPoint = new Point(Random.Shared.NextDouble(), Random.Shared.NextDouble());
        linearGradientBrush.EndPoint = new Point(Random.Shared.NextDouble(), Random.Shared.NextDouble());

        int gradientStopCount = Random.Shared.Next(2, 5);
        var gradientStops = new List<GradientStop>();
        for(int i = 0; i < gradientStopCount; i++)
        {
            double offset = i == 0 ? 0.0 : (i == gradientStopCount - 1 ? 1.0 : Random.Shared.NextDouble());

            Color randomColor = Color.FromArgb((byte)Random.Shared.Next(minAlpha, maxAlpha + 1), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256));

            gradientStops.Add(new GradientStop(randomColor, offset));
        }

        foreach(var stop in gradientStops.OrderBy(gs => gs.Offset))    // Sort the gradient stops by offset and add them to the brush
            linearGradientBrush.GradientStops.Add(stop);

        return linearGradientBrush;
    }

    public static Color? GetColorFromHex(string hex)
    {
        if(hex == null) return null;
        if(hex.StartsWith("#"))
            hex = hex.Substring(1); // Remove the #

        if(hex.Length == 6)
            hex = "FF" + hex; // Add alpha if missing

        byte a = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
        byte r = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber);

        return Color.FromArgb(a, r, g, b);
    }

    public static string ColorToHexString(Color color) => $"#{color.A:X2}{color.R:X2}{color.G:X2}{color.B:X2}";

    public static (double H, double S, double V) RgbToHsv(Color color)
    {
        double r = color.R / 255.0;
        double g = color.G / 255.0;
        double b = color.B / 255.0;

        double max = Math.Max(r, Math.Max(g, b));
        double min = Math.Min(r, Math.Min(g, b));
        double delta = max - min;

        double h = 0;
        if(delta != 0)
        {
            if(max == r)
                h = (g - b) / delta % 6;
            else if(max == g)
                h = (b - r) / delta + 2;
            else
                h = (r - g) / delta + 4;
        }
        h /= 6;

        double s = max == 0 ? 0 : delta / max;
        double v = max;

        return (h, s, v);
    }

    public static Color HsvToRgb(double h, double s, double v, byte a = 255)
    {
        double r, g, b;

        int hi = (int)(h * 6) % 6;
        double f = h * 6 - hi;
        double p = v * (1 - s);
        double q = v * (1 - f * s);
        double t = v * (1 - (1 - f) * s);

        switch(hi)
        {
            case 0: r = v; g = t; b = p; break;
            case 1: r = q; g = v; b = p; break;
            case 2: r = p; g = v; b = t; break;
            case 3: r = p; g = q; b = v; break;
            case 4: r = t; g = p; b = v; break;
            default: r = v; g = p; b = q; break;
        }

        return Color.FromArgb(a, (byte)(r * 255), (byte)(g * 255), (byte)(b * 255));
    }
}
