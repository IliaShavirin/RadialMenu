using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IValueConverter))]
    [ValueConversion(typeof(object), typeof(bool))]
    public class ValueSatisfiesRuleParameter : MarkupExtension, IValueConverter
    {
        private static ValueSatisfiesRuleParameter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new ValueSatisfiesRuleParameter());
        }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var rBool = false;
            if (parameter == null)
                throw new ArgumentException(
                    "CommandParameter cannot be null for ValueSatisfiesRuleParameter Converter");

            var paramsSplit = parameter.ToString().Split(' ');

            var operand = paramsSplit[0];
            var checkValue = paramsSplit[1];

            if (operand == "==" && value?.ToString() == checkValue) return true;

            try
            {
                var inputValueAsInt = System.Convert.ToInt32(value);
                var checkValueAsInt = System.Convert.ToInt32(checkValue);

                switch (operand)
                {
                    case "<=":
                        rBool = inputValueAsInt <= checkValueAsInt;
                        break;

                    case ">=":
                        rBool = inputValueAsInt >= checkValueAsInt;
                        break;

                    case "<":
                        rBool = inputValueAsInt < checkValueAsInt;
                        break;

                    case ">":
                        rBool = inputValueAsInt > checkValueAsInt;
                        break;

                    case "==":
                        rBool = inputValueAsInt == checkValueAsInt;
                        break;

                    case "!=":
                        rBool = inputValueAsInt != checkValueAsInt;
                        break;
                }
            }
            catch (Exception)
            {
                return false;
            }

            // If parameter is set converter is inverted
            return rBool;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}