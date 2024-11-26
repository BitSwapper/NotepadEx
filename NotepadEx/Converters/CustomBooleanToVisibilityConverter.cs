using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NotepadEx.Converters;

public class CustomBooleanToVisibilityConverter : IValueConverter
{
    public Visibility FalseValue { get; set; } = Visibility.Hidden;

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is bool boolValue)
            return boolValue ? Visibility.Visible : FalseValue;

        return FalseValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if(value is Visibility visibility)
            return visibility == Visibility.Visible;

        return false;
    }
}
