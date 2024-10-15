namespace NotepadEx.Extensions;
public static class PointExtensions
{
    public static void Deconstruct(this System.Windows.Point point, out double x, out double y)
    {
        x = point.X;
        y = point.Y;
    }
}

