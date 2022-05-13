using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;

namespace PhotoOrganizings.Converters;

public class PhotoTaskStatusToColorConverter : DependencyObject, IValueConverter
{
    public static readonly DependencyProperty RunningColorProperty =
        DependencyProperty.Register(
            nameof(RunningColor),
            typeof(SolidColorBrush),
            typeof(PhotoTaskStatusToColorConverter),
            new PropertyMetadata(new SolidColorBrush(Colors.Gray)));

    public static readonly DependencyProperty SuccessedColorProperty =
        DependencyProperty.Register(
            nameof(SuccessedColor),
            typeof(SolidColorBrush),
            typeof(PhotoTaskStatusToColorConverter),
            new PropertyMetadata(new SolidColorBrush(Colors.LightGreen)));

    public static readonly DependencyProperty ErrorColorProperty =
        DependencyProperty.Register(
            nameof(ErrorColor),
            typeof(SolidColorBrush),
            typeof(PhotoTaskStatusToColorConverter),
            new PropertyMetadata(new SolidColorBrush(Colors.Red)));

    public SolidColorBrush RunningColor
    {
        get => (SolidColorBrush)GetValue(RunningColorProperty);
        set => SetValue(RunningColorProperty, value);
    }

    public SolidColorBrush SuccessedColor
    {
        get => (SolidColorBrush)GetValue(SuccessedColorProperty);
        set => SetValue(SuccessedColorProperty, value);
    }

    public SolidColorBrush ErrorColor
    {
        get => (SolidColorBrush)GetValue(ErrorColorProperty);
        set => SetValue(ErrorColorProperty, value);
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is PhotoTaskResult status)
        {
            return status switch
            {
                PhotoTaskResult.Running => RunningColor,
                PhotoTaskResult.Successed => SuccessedColor,
                PhotoTaskResult.Error => ErrorColor,
                _ => throw new ArgumentException($"PhotoTaskStatusToColorConverter Invalid status: {status}")
            };
        }

        throw new NotImplementedException("PhotoTaskStatusToColorConverter");
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException("PhotoTaskStatusToColorConverter ConvertBack");
    }
}