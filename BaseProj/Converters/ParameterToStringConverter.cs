using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    public class ParameterToStringConverter : MarkupExtension, IValueConverter
    {
        private static ParameterToStringConverter _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter != null && parameter is string str) return str;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("RadialMenuItemToArrowPosition is a One-Way converter only !");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ParameterToStringConverter());
        }
    }
}