using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class StringIsNullOrEmptyToVisibility : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static StringIsNullOrEmpty _helperConverter;
        private static StringIsNullOrEmptyToVisibility _converter;

        public StringIsNullOrEmptyToVisibility()
        {
            _helperConverter = new StringIsNullOrEmpty();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new StringIsNullOrEmptyToVisibility());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var input = (bool)_helperConverter.Convert(value, targetType, parameter, culture);

            // unnecessary invert because value is already inverted from helper converter
//            if (parameter is string && parameter.ToString().ToLower().Contains("invert"))
//            {
//                input = !input;
//            }

            return input ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultiValueConverter

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

        #endregion
    }
}