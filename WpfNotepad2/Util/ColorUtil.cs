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

        int gradientStopCount = Random.Shared.Next(2, 6);
        var gradientStops = new List<GradientStop>();
        for(int i = 0; i < gradientStopCount; i++)
        {
            double offset = i == 0 ? 0.0 : (i == gradientStopCount - 1 ? 1.0 : Random.Shared.NextDouble());

            Color randomColor = Color.FromArgb((byte)Random.Shared.Next(minAlpha, maxAlpha + 1), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256), (byte)Random.Shared.Next(256));

            gradientStops.Add(new GradientStop(randomColor, offset));
        }

        //foreach(var stop in gradientStops.OrderBy(gs => gs.Offset))    // Sort the gradient stops by offset and add them to the brush
        //    linearGradientBrush.GradientStops.Add(stop);

        foreach(var stop in gradientStops)
            linearGradientBrush.GradientStops.Add(stop);

        return linearGradientBrush;
    }

    public static Color? GetColorFromHex(string hex)
    {
        if(hex == null || hex.Length > 9) return null;
        if(hex.StartsWith("#"))
            hex = hex.Substring(1); // Remove the #

        if(hex.Length == 6)
            hex = "FF" + hex; // Add alpha if missing

        if(!(hex.Length == 6 || hex.Length == 8))
            return null;

        byte a, r, g, b;

        if(!byte.TryParse(hex.Substring(0, 2), NumberStyles.HexNumber, null, out a)) a = 0;
        if(!byte.TryParse(hex.Substring(2, 2), NumberStyles.HexNumber, null, out r)) r = 0;
        if(!byte.TryParse(hex.Substring(4, 2), NumberStyles.HexNumber, null, out g)) g = 0;
        if(!byte.TryParse(hex.Substring(6, 2), NumberStyles.HexNumber, null, out b)) b = 0;

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

    public static string SerializeGradient(LinearGradientBrush gradient)
    {
        var serializedData = new List<string>
        {
            $"StartPoint:{gradient.StartPoint.X},{gradient.StartPoint.Y}",
            $"EndPoint:{gradient.EndPoint.X},{gradient.EndPoint.Y}",
            $"SpreadMethod:{gradient.SpreadMethod}",
            $"MappingMode:{gradient.MappingMode}",
            $"ColorInterpolationMode:{gradient.ColorInterpolationMode}"
        };

        var stops = gradient.GradientStops.Select(stop =>
            $"{ColorUtil.ColorToHexString(stop.Color)}:{stop.Offset}");
        serializedData.Add($"GradientStops:{string.Join("|", stops)}");

        return string.Join(";", serializedData);
    }

    public static LinearGradientBrush DeserializeGradient(string gradientString)
    {
        var parts = gradientString.Split(';');
        var brush = new LinearGradientBrush();
        bool wasValid = false;
        foreach(var part in parts)
        {
            var keyValue = part.Split(':');
            if(keyValue.Length < 2) continue;

            switch(keyValue[0])
            {
                case "StartPoint":
                    var startPoint = keyValue[1].Split(',');
                    brush.StartPoint = new Point(double.Parse(startPoint[0]), double.Parse(startPoint[1]));
                    break;
                case "EndPoint":
                    var endPoint = keyValue[1].Split(',');
                    brush.EndPoint = new Point(double.Parse(endPoint[0]), double.Parse(endPoint[1]));
                    break;
                case "SpreadMethod":
                    brush.SpreadMethod = (GradientSpreadMethod)Enum.Parse(typeof(GradientSpreadMethod), keyValue[1]);
                    break;
                case "MappingMode":
                    brush.MappingMode = (BrushMappingMode)Enum.Parse(typeof(BrushMappingMode), keyValue[1]);
                    break;
                case "ColorInterpolationMode":
                    brush.ColorInterpolationMode = (ColorInterpolationMode)Enum.Parse(typeof(ColorInterpolationMode), keyValue[1]);
                    break;
                case "GradientStops":
                    var stopsData = string.Join(":", keyValue.Skip(1));  // Rejoin in case there were colons in the color data
                    var stops = stopsData.Split('|').Select(stop =>
                    {
                        var stopParts = stop.Split(':');
                        if (stopParts.Length != 2) throw new FormatException($"Invalid gradient stop format: {stop}");
                        return new GradientStop(ColorUtil.GetColorFromHex(stopParts[0]).Value, double.Parse(stopParts[1]));
                    });

                    if(stops.Count() > 0)
                        wasValid = true;

                    brush.GradientStops = new GradientStopCollection(stops.ToList());  // Convert to List before creating GradientStopCollection
                    break;
            }
        }
        if(!wasValid) return null;
        return brush;
    }
}
