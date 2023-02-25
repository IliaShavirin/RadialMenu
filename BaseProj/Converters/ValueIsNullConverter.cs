using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValueIsNullConverter : MarkupExtension, IValueConverter
    {
        private static ValueIsNullConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValueIsNullConverter());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = value == null || value == DependencyProperty.UnsetValue;
            var param = parameter?.ToString().ToLower();

#if DEBUG
            if (param == "debug") Debugger.Break();
#endif

            var invert = param == "invert";

            var output = !invert ? bValue : !bValue;
            return output;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}