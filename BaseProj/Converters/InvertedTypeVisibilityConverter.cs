using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class InvertedTypeVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static InvertedTypeVisibilityConverter _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return Visibility.Collapsed;

            var ret = !value.GetType().Equals(parameter) ? Visibility.Visible : Visibility.Collapsed;
            return ret;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new InvertedTypeVisibilityConverter());
        }
    }
}