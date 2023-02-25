using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [ValueConversion(typeof(object), typeof(bool))]
    public class HashSumMultiConverter : MarkupExtension, IMultiValueConverter
    {
        private static HashSumMultiConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new HashSumMultiConverter());
        }


        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
#if DEBUG
            if (parameter as string == "debug") Debugger.Break();
#endif

            var hash = 25;

            foreach (var value in values)
            {
                if (value == DependencyProperty.UnsetValue)
                    continue;

                hash = hash * 11 + value.GetHashCode();
            }

            return hash;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}