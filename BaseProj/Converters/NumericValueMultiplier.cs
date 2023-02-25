using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    public class NumericValueMultiplier : MarkupExtension, IValueConverter
    {
        private static NumericValueMultiplier _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new NumericValueMultiplier());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var castParameter = double.Parse(parameter.ToString(), CultureInfo.InvariantCulture);

            if (value is Thickness margin)
                return new Thickness(margin.Left * castParameter, margin.Top * castParameter,
                    margin.Right * castParameter, margin.Bottom * castParameter);

            double parsed;
            if (double.TryParse(value.ToString(), out parsed)) return parsed * castParameter;

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}