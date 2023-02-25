using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(bool), typeof(bool))]
    public class MultiBooleanAndConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiBooleanAndConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiBooleanAndConverter());
        }

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (parameter as string == "debug") Debugger.Break();
#endif
            var input = Array.TrueForAll(values, value => value is bool && (bool)value);

            if (parameter is string && parameter.ToString().ToLower().Contains("invert")) input = !input;

            return input;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return new[] { value };
        }

        #endregion
    }
}