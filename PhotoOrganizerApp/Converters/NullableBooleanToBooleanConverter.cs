using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

namespace PhotoOrganizings.Converters;

public class NullableBooleanToBooleanConverter : DependencyObject, IValueConverter
{
    public bool NullBooleanValue
    {
        get => (bool)GetValue(NullBooleanValueProperty);
        set => SetValue(NullBooleanValueProperty, value);
    }

    public static readonly DependencyProperty NullBooleanValueProperty =
        DependencyProperty.Register(
            nameof(NullBooleanValue),
            typeof(bool),
            typeof(NullableBooleanToBooleanConverter),
            new PropertyMetadata(false));

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

    public object Convert(object? value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return IsInversed is false ? boolValue : !boolValue;
        }

        return NullBooleanValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        if (value is bool booleanValue)
        {
            return (bool?)booleanValue;
        }

        throw new ArgumentException("NullableBooleanToBooleanConverter ConvertBack");
    }
}