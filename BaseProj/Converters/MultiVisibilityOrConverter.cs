using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class MultiVisibilityOrConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiVisibilityOrConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiVisibilityOrConverter());
        }

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Array.Find(values, value => value is Visibility && (Visibility)value == Visibility.Visible) !=
                        null;

            if (parameter is string && parameter.ToString().ToLower().Contains("invert")) input = !input;

            return input ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}