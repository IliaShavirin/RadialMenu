using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BooleanToVisibilityConverter : MarkupExtension, IValueConverter, IMultiValueConverter
    {
        private static BooleanToVisibilityConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new BooleanToVisibilityConverter());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is string sParam)
            {
#if DEBUG
                if (sParam == "debug")
                    Debugger.Break();
#endif

                var parameters = sParam.Split(' ');

                if (parameters.Any(p => p.ToLowerInvariant() == "hidden"))
                {
                    if (parameters.Length > 1) return (bool)value ? Visibility.Hidden : Visibility.Visible;

                    return (bool)value ? Visibility.Visible : Visibility.Hidden;
                }
            }

            //reverse conversion (false=>Visible, true=>collapsed) on any given parameter
            if (value != null)
            {
                var input = null == parameter ? (bool)value : !(bool)value;
                return input ? Visibility.Visible : Visibility.Collapsed;
            }

            return (bool?)value != false ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var result = false;
            if ((parameter as string)?.ToLower() == "or")
                result = Array.Find(values, value => value is bool && (bool)value) != null;
            else
                result = Array.TrueForAll(values, value => value is bool && (bool)value);


            return result ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}