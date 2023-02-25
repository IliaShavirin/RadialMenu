using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ValuesAreEqualToVisibilityConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static ValuesAreEqual _helperConverter;
        private static ValuesAreEqualToVisibilityConverter _converter;

        public ValuesAreEqualToVisibilityConverter()
        {
            _helperConverter = new ValuesAreEqual();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValuesAreEqualToVisibilityConverter());
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