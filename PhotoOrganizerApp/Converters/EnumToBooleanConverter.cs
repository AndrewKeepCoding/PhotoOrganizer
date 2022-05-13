using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace PhotoOrganizings.Helpers;

public class EnumToBooleanConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is ElementTheme enumValue &&
            parameter is string enumString && Enum.TryParse(enumString, out ElementTheme enumParameter) is true)
        {
            return enumValue.Equals(enumParameter);
        }

        throw new ArgumentException("ExceptionEnumToBooleanConverter");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException("ExceptionEnumToBooleanConverter ConvertBack");
    }
}