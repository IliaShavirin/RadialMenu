using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(bool), typeof(bool))]
    public class MultiBooleanOrConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiBooleanOrConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiBooleanOrConverter());
        }

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var input = Array.Find(values, value => value is bool && (bool)value) != null;

            if (parameter is string && parameter.ToString().Contains("invert")) input = !input;

            return input;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }

        #endregion
    }
}