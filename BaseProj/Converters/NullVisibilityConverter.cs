using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(bool))]
    public class NullVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static NullVisibilityConverter _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var visibility = Visibility.Hidden;
            if ((string)parameter == "Collapsed" || (string)parameter == "collapsed") visibility = Visibility.Collapsed;

            if ((string)parameter == "Invert" || (string)parameter == "invert")
                return value != null ? visibility : Visibility.Visible;
            var ret = value == null ? visibility : Visibility.Visible;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new NullVisibilityConverter());
        }
    }
}