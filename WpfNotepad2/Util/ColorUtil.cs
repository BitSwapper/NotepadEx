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
}
