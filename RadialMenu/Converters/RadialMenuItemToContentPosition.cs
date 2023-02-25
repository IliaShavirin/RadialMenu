using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using RadialMenu.CustomControls;

namespace RadialMenu.Converters
{
    internal class RadialMenuItemToContentPosition : MarkupExtension, IMultiValueConverter
    {
        private static RadialMenuItemToContentPosition _converter;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 7)
                throw new ArgumentException(
                    "RadialMenuItemToContentPosition converter needs 6 values (double angle, double centerX, double centerY, double contentWidth, double contentHeight, double contentRadius) !",
                    "values");
            if (parameter == null)
                throw new ArgumentNullException("parameter",
                    "RadialMenuItemToContentPosition converter needs the parameter (string axis) !");

            var axis = (string)parameter;

            if (axis != "X" && axis != "Y")
                throw new ArgumentException("RadialMenuItemToContentPosition parameter needs to be 'X' or 'Y' !",
                    "parameter");

            var angle = (double)values[0];
            var centerX = (double)values[1];
            var centerY = (double)values[2];
            var contentWidth = (double)values[3];
            var contentHeight = (double)values[4];
            var radialMenuItem = (RadialMenuItem)values[5];
            if (radialMenuItem.RadialMenu != null)
            {
                var maxR = radialMenuItem.RadialMenu.MaxStrokeThickness;
                var minR = radialMenuItem.RadialMenu.MinStrokeThickness;
                var coef = (maxR - minR) / 13;
                var sectrorsCount = radialMenuItem.RadialMenu.Items.Count;
                var innerRadius = minR - (16 - sectrorsCount) * coef;
                var outerRadius = radialMenuItem.OuterRadius;

                var contentRadius = innerRadius + (outerRadius - innerRadius) / 2;

                var contentPosition = ComputeCartesianCoordinate(new Point(centerX, centerY), angle, contentRadius);

                if (axis == "X") return contentPosition.X - contentWidth / 2;

                return contentPosition.Y - contentHeight / 2;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("RadialMenuItemToContentPosition is a One-Way converter only !");
        }

        private static Point ComputeCartesianCoordinate(Point center, double angle, double radius)
        {
            // Converts to radians
            var radiansAngle = Math.PI / 180.0 * (angle - 90);
            var x = radius * Math.Cos(radiansAngle);
            var y = radius * Math.Sin(radiansAngle);
            return new Point(x + center.X, y + center.Y);
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new RadialMenuItemToContentPosition());
        }
    }
}