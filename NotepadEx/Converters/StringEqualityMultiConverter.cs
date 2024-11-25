using System.Globalization;
using System.Windows.Data;

namespace NotepadEx.Converters;
public class StringEqualityMultiConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if(values.Length != 2 || values[0] == null || values[1] == null)
            return false;

        return values[0].ToString() == values[1].ToString();
    }

    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
}

//public class StringEqualityMultiConverter : IMultiValueConverter
//{
//    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
//    {
//        if(values == null || values.Length != 2)
//            return false;

//        // Handle cases where either value might be null
//        if(values[0] == null || values[1] == null)
//            return values[0] == values[1];

//        // Compare the string values
//        return values[0].ToString().Equals(values[1].ToString(), StringComparison.OrdinalIgnoreCase);
//    }

//    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
//    {
//        throw new NotImplementedException();
//    }
//}
