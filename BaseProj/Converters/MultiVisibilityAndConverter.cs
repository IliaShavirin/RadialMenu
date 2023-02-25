using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(Visibility), typeof(Visibility))]
    public class MultiVisibilityAndConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiVisibilityAndConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiVisibilityAndConverter());
        }

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Array.TrueForAll(values,
                value => value is Visibility && (Visibility)value == Visibility.Visible);

            if (parameter is string && parameter.ToString().ToLower().Contains("invert")) input = !input;

            if (parameter is string && parameter.ToString().ToLower().Contains("hidden"))
                return input ? Visibility.Visible : Visibility.Hidden;

            return input ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}