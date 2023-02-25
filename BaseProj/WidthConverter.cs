using System;
using System.Globalization;
using System.Windows.Data;

namespace BaseProj
{
    /// <summary>
    ///     Calculates the column width required to fill the view in a GridView
    /// </summary>
    public class WidthConverter : IValueConverter
    {
        /// <summary>
        ///     Converts the specified value.
        /// </summary>
        /// <param name="value">The parent TreeListView.</param>
        /// <param name="type">The type.</param>
        /// <param name="parameter">
        ///     If no parameter is given, the remaning with will be returned.
        ///     If the parameter is an integer acts as MinimumWidth, the remaining with will be returned only if it's greater than
        ///     the parameter
        ///     If the parameter is anything else, it's taken to be a percentage. Eg: 0.3* = 30%, 0.15* = 15%
        /// </param>
        /// <param name="culture">The culture.</param>
        /// <returns>The width, as calculated by the parameter given</returns>
        public object Convert(object value, Type type, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            var relativeSource = parameter as RelativeSource;
            var tree = relativeSource.ProvideValue(null) as TreeListView;
            var listView = value as TreeListView;
            var grdViewColumns = listView.Columns;
            var minWidth = 0;
            var widthIsPercentage = parameter != null && !int.TryParse(parameter.ToString(), out minWidth);
            if (widthIsPercentage)
            {
                var widthParam = parameter.ToString();
                var sub = widthParam.Substring(0, widthParam.Length - 1);

                double percentage = 0;
                while (!double.TryParse(sub, out percentage))
                {
                    if (sub.IndexOf('.') >= 0) // temp
                    {
                        sub = sub.Replace('.', ',');
                        continue;
                    }

                    if (sub.IndexOf(',') >= 0) //temp
                        sub = sub.Replace(',', '.');
                }

                return listView.ActualWidth * percentage;
            }

            double total = 0;
            for (var i = 0; i < grdViewColumns.Count - 1; i++) total += grdViewColumns[i].ActualWidth;
            var remainingWidth = listView.ActualWidth - total;
            if (remainingWidth > minWidth)
                // fill the remaining width in the ListView
                return remainingWidth;
            // fill remaining space with MinWidth
            return minWidth;
        }

        public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}