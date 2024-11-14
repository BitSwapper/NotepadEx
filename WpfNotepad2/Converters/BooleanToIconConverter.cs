using System.Globalization;
using System.Windows.Data;

namespace NotepadEx.Converters;
public class BooleanToIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        bool isMaximized = (bool)value;
        return isMaximized ? "🗗" : "🗗"; // Use "🗖" for restore and "🗗" for maximize //Maximize and Overlap are actual names but i can't seem to get overlap to work so i used pc icon lmao
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
}
