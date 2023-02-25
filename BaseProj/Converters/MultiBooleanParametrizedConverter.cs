using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;

namespace BaseProj.Converters
{
    [MarkupExtensionReturnType(typeof(IMultiValueConverter))]
    [ValueConversion(typeof(bool), typeof(bool))]
    public class MultiBooleanParametrizedConverter : MarkupExtension, IMultiValueConverter
    {
        private static MultiBooleanParametrizedConverter _converter;

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new MultiBooleanParametrizedConverter());
        }

        #region IMultiValueConverter

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            for (var i = 0; i < values.Length; i++)
                if (values[i] == DependencyProperty.UnsetValue)
                    values[i] = false;

            //if (values.Contains(DependencyProperty.UnsetValue)) return false;

            //var paramsSplit = parameter.ToString().Split(' ');

            //var bEvalString = string.Empty;
            //var valueIndex = 0;

            //for (var i = 0; i < paramsSplit.Length; i++)
            //{
            //    var operand = paramsSplit[i];
            //    var value = values[valueIndex];
            //    if (operand == "(" || operand == ")")
            //    {
            //        bEvalString += $" {operand}";
            //    }
            //    else
            //    {
            //        bEvalString += $" {operand} {value}";
            //        valueIndex++;
            //    }
            //}

            var bEvalString = string.Format(parameter.ToString(), values);

            return BooleanEvaluator.Evaluate(bEvalString);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}