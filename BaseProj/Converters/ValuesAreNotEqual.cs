using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    // If anyone wonders why not use ValuesAreEqual with invert parameter
    // Because they can be used as IValueConverter so that another check parameter is static and passed via ConverterParameter so it's not possible to invert via ConverterParameter
    // Because it's already used
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValuesAreNotEqual : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static ValuesAreNotEqual _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValuesAreNotEqual());
        }

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == DependencyProperty.UnsetValue) return true;

            return !Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion // End - IValueConverter

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var equals = false;
            if (values.Any(v => v == DependencyProperty.UnsetValue))
            {
                equals = false;
            }
            else
            {
                var firstValue = values[0];
                equals = values.Skip(1).All(value => Equals(value, firstValue));
            }


            return parameter is string && ((string)parameter).ToLower() == "invert" ? equals : !equals;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}