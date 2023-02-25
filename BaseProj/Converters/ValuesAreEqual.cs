using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValuesAreEqual : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static ValuesAreEqual _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValuesAreEqual());
        }

        #region IValueConverter

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (parameter as string == "debug") Debugger.Break();
#endif
            if (value == DependencyProperty.UnsetValue) return false;

            return Equals(value, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion // End - IValueConverter

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (parameter as string == "debug") Debugger.Break();
#endif
            if (values.Any(v => v == DependencyProperty.UnsetValue)) return false;
            //else
            //{
            //    for (int i = 0; i < values.Length; i++)
            //    {
            //        if (values[i] == DependencyProperty.UnsetValue)
            //        {
            //            values[i] = null;
            //        }
            //    }
            //}
            var firstValue = values[0];
            var equals = values.Skip(1).All(value => Equals(value, firstValue));

            return parameter is string && ((string)parameter).ToLower() == "invert" ? !equals : equals;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}