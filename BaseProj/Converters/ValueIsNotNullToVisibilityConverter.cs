using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ValueIsNotNullToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static ValueIsNotNullToVisibilityConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValueIsNotNullToVisibilityConverter());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bValue = value != null;
            var strVal = value as string;
            var hidden = parameter?.ToString() == "hidden";
            //reverse conversion (false=>Visible, true=>collapsed) on any given parameter
            if (strVal != null && strVal == "") bValue = false;

            var output = !hidden && parameter == null ? bValue : !bValue;
            return output ? Visibility.Visible : hidden ? Visibility.Hidden : Visibility.Collapsed;
            //return ((bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}