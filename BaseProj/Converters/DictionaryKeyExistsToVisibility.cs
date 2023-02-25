using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    /// <summary>
    ///     In IValueConverter mode it expects key as a ConverterParameter and Dictionary as Value
    ///     In IMultiValueConverter mode it expects Dictionary as values[0] and key as values[1]
    ///     IMultiValueConverter done because ConverterParameter is static value and cannot be bound do
    /// </summary>
    public class DictionaryKeyExistsToVisibility : MarkupExtension, IMultiValueConverter, IValueConverter
    {
        private static DictionaryKeyExists _helperConverter;
        private static DictionaryKeyExistsToVisibility _converter;

        public DictionaryKeyExistsToVisibility()
        {
            _helperConverter = new DictionaryKeyExists();
        }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string && ((string)parameter).ToLower() == "hidden")
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

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)_helperConverter.Convert(value, targetType, parameter, culture)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new DictionaryKeyExistsToVisibility());
        }
    }
}