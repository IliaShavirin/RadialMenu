using System;
using System.Windows;
using System.Windows.Controls;

namespace BaseProj.ListViewLayoutManager
{
    // ------------------------------------------------------------------------
    public abstract class LayoutColumn
    {
        // ----------------------------------------------------------------------
        protected static bool HasPropertyValue(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null) throw new ArgumentNullException("column");
            var value = column.ReadLocalValue(dp);
            if (value != null && value.GetType() == dp.PropertyType) return true;

            return false;
        } // HasPropertyValue

        // ----------------------------------------------------------------------
        protected static double? GetColumnWidth(GridViewColumn column, DependencyProperty dp)
        {
            if (column == null) throw new ArgumentNullException("column");
            var value = column.ReadLocalValue(dp);
            if (value != null && value.GetType() == dp.PropertyType) return (double)value;

            return null;
        } // GetColumnWidth
    } // class LayoutColumn
}