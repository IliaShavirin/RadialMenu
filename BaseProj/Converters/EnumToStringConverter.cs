using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToStringConverter : MarkupExtension, IValueConverter
    {
        private static EnumToStringConverter _converter;

        public object Convert(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var enumValue = default(Enum);
            if (parameter is Type) enumValue = (Enum)Enum.Parse((Type)parameter, value.ToString());
            return enumValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            var returnValue = 0;
            if (parameter is Type) returnValue = (int)Enum.Parse((Type)parameter, value.ToString());
            return returnValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new EnumToStringConverter());
        }
    }
}