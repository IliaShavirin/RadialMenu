using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class MultiBooleanParametrizedConverterToVisibilityConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiBooleanParametrizedConverter _helperConverter;
        private static MultiBooleanParametrizedConverterToVisibilityConverter _converter;

        public MultiBooleanParametrizedConverterToVisibilityConverter()
        {
            _helperConverter = new MultiBooleanParametrizedConverter();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
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
            return _converter ?? (_converter = new MultiBooleanParametrizedConverterToVisibilityConverter());
        }
    }
}