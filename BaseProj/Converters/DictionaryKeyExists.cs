using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    /// <summary>
    ///     In IValueConverter mode it expects key as a ConverterParameter and Dictionary as Value
    ///     In IMultiValueConverter mode it expects Dictionary as values[0] and key as values[1]
    ///     IMultiValueConverter done because ConverterParameter is static value and cannot be bound do
    /// </summary>
    public class DictionaryKeyExists : MarkupExtension, IMultiValueConverter, IValueConverter
    {
        private static DictionaryKeyExists _converter;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var rValue = false;

            var dictionary = values[0] as IDictionary;
            if (dictionary != null)
            {
                var key = values[1];
                rValue = key != null && dictionary.Contains(key);
            }

            return rValue;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rValue = false;

            var dictionary = value as IDictionary;
            if (dictionary != null)
            {
                var key = parameter;
                rValue = key != null && dictionary.Contains(key);
            }

            return rValue;
        }


        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new DictionaryKeyExists());
        }
    }
}