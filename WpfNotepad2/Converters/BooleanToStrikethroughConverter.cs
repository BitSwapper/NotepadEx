using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotepadEx.Converters;

public class BooleanToStrikethroughConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool isDecorated && isDecorated)
        {
            return TextDecorations.Strikethrough;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value != null;
    }
}
