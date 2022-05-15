using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace PhotoOrganizings.Converters;

public class BooleanToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return booleanValue ? Visibility.Visible : Visibility.Collapsed;
        }

        throw new ArgumentException("BooleanToVisibilityConverter");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}