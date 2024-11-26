using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotepadEx.Converters;

public class BooleanToTextDecorationConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool isDecorated && isDecorated)
        {
            return TextDecorations.Underline;
        }
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value != null;
}
