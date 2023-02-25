using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class MultiBooleanOrConverterToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiBooleanOrConverter _helperConverter;
        private static MultiBooleanOrConverterToVisibilityConverter _converter;

        public MultiBooleanOrConverterToVisibilityConverter()
        {
            _helperConverter = new MultiBooleanOrConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string && ((string)parameter).ToLower().Contains("hidden"))
                return (bool)_helperConverter.Convert(values, targetType, parameter, culture)
                    ? Visibility.Visible
                    : Visibility.Hidden;

            return (bool)_helperConverter.Convert(values, targetType, parameter, culture)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiBooleanOrConverterToVisibilityConverter());
        }
    }
}