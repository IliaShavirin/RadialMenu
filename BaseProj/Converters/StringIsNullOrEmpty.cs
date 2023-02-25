using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(bool))]
    public class StringIsNullOrEmpty : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static StringIsNullOrEmpty _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new StringIsNullOrEmpty());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If parameter is set converter is inverted
            var bResult = true;

            if (value is string) bResult = string.IsNullOrWhiteSpace((string)value);

            return parameter == null ? bResult : !bResult;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Array.TrueForAll(values, value => value != null);

            return input;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}