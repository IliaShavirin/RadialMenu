using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace BaseProj.ListViewLayoutManager
{
    // ------------------------------------------------------------------------
    public abstract class ConverterGridViewColumn : GridViewColumn, IValueConverter
    {
        // members

        // ----------------------------------------------------------------------
        protected ConverterGridViewColumn(Type bindingType)
        {
            if (bindingType == null) throw new ArgumentNullException("bindingType");

            BindingType = bindingType;

            // binding
            var binding = new Binding();
            binding.Mode = BindingMode.OneWay;
            binding.Converter = this;
            DisplayMemberBinding = binding;
        } // ConverterGridViewColumn

        // ----------------------------------------------------------------------
        public Type BindingType { get; }

        // ----------------------------------------------------------------------
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!BindingType.IsInstanceOfType(value)) throw new InvalidOperationException();
            return ConvertValue(value);
        } // IValueConverter.Convert

        // ----------------------------------------------------------------------
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        } // IValueConverter.ConvertBack

        // ----------------------------------------------------------------------
        protected abstract object ConvertValue(object value);
    } // class ConverterGridViewColumn
}