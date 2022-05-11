using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace PhotoOrganizings.Converters;

public class NullableBooleanToBooleanConverter : DependencyObject, IValueConverter
{
    public bool IsInversed
    {
        get => (bool)GetValue(IsInversedProperty);
        set => SetValue(IsInversedProperty, value);
    }

    public static readonly DependencyProperty IsInversedProperty =
        DependencyProperty.Register(
            nameof(IsInversed),
            typeof(bool),
            typeof(NullableBooleanToBooleanConverter),
            new PropertyMetadata(false));

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return IsInversed is false ? (bool)value : !(bool)value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return (bool?)value;
    }
}