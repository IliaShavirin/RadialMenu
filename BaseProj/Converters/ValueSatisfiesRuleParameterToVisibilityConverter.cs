using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class ValueSatisfiesRuleParameterToVisibilityConverter : MarkupExtension, IValueConverter
    {
        private static ValueSatisfiesRuleParameter _helperConverter;
        private static ValueSatisfiesRuleParameterToVisibilityConverter _converter;

        public ValueSatisfiesRuleParameterToVisibilityConverter()
        {
            _helperConverter = new ValueSatisfiesRuleParameter();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValueSatisfiesRuleParameterToVisibilityConverter());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)_helperConverter.Convert(value, targetType, parameter, culture)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}