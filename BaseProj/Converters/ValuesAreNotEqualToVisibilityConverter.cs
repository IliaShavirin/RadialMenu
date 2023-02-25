using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ValuesAreNotEqualToVisibilityConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static ValuesAreNotEqual _helperConverter;
        private static ValuesAreNotEqualToVisibilityConverter _converter;

        public ValuesAreNotEqualToVisibilityConverter()
        {
            _helperConverter = new ValuesAreNotEqual();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValuesAreNotEqualToVisibilityConverter());
        }

        #region IValueConverter

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

        #endregion // End - IValueConverter

        #region IMultiValueConverter Members

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