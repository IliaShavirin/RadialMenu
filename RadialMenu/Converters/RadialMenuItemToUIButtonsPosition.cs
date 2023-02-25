using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using RadialMenu.CustomControls;

namespace RadialMenu.Converters
{
    internal class RadialMenuItemToUIButtonsPosition : MarkupExtension, IMultiValueConverter
    {
        private static RadialMenuItemToUIButtonsPosition _converter;

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 3)
                throw new ArgumentException(
                    "RadialMenuItemToArrowPosition converter needs 7 values (double centerX, double centerY, double arrowWidth, double arrowHeight, double arrowRadius) !",
                    "values");
            if (parameter == null)
                throw new ArgumentNullException("parameter",
                    "RadialMenuItemToArrowPosition converter needs the parameter (string axis) !");

            var axis = (string)parameter;

            if (axis != "X" && axis != "Y")
                throw new ArgumentException("RadialMenuItemToArrowPosition parameter needs to be 'X' or 'Y' !",
                    "parameter");

            var radialMenu = (CustomControls.RadialMenu)values[0];
            var allignPosition = (string)values[1];

            var radialMenuItem = radialMenu.CurrentItem;

            if (radialMenuItem != null && radialMenuItem.RadialMenu != null)
            {
                var centerX = radialMenuItem.CenterX;
                var centerY = radialMenuItem.CenterY;
                var buttonWidth = 32;
                var buttonHeight = 32;
                var startAngle = radialMenuItem.StartAngle;
                var angleDelta = radialMenuItem.AngleDelta;

                double contentRadius = 0;

                var arcCenter = new Point(centerX, centerY);

                var buttonPoint = new Point();

                switch (allignPosition.ToLower())
                {
                    case "left":
                        contentRadius = CalculateContentRadius(radialMenu, radialMenuItem);
                        buttonPoint = ComputeCartesianCoordinate(arcCenter, startAngle, contentRadius);
                        break;

                    case "right":
                        contentRadius = CalculateContentRadius(radialMenu, radialMenuItem);
                        buttonPoint = ComputeCartesianCoordinate(arcCenter, startAngle + angleDelta, contentRadius);
                        break;

                    case "top":
                        contentRadius = radialMenuItem.ArrowRadius + buttonWidth / 3;
                        buttonPoint = ComputeCartesianCoordinate(arcCenter, startAngle + angleDelta / 2, contentRadius);
                        break;

                    case "down":
                        contentRadius = radialMenuItem.InnerRadius;
                        buttonPoint = ComputeCartesianCoordinate(arcCenter, startAngle + angleDelta / 2, contentRadius);
                        break;
                }

                if (axis == "X") return buttonPoint.X - buttonWidth / 2;

                return buttonPoint.Y - buttonHeight / 2;
            }

            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double CalculateContentRadius(CustomControls.RadialMenu radialMenu, RadialMenuItem radialMenuItem)
        {
            var maxR = radialMenu.MaxStrokeThickness;
            var minR = radialMenu.MinStrokeThickness;
            var coef = (maxR - minR) / 13;
            var sectrorsCount = radialMenu.Items.Count;
            var innerRadius = minR - (16 - sectrorsCount) * coef;
            var outerRadius = radialMenuItem.OuterRadius;

            var contentRadius = innerRadius + (outerRadius - innerRadius) / 2;
            return contentRadius;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new RadialMenuItemToUIButtonsPosition());
        }

        private static Point ComputeCartesianCoordinate(Point center, double angle, double radius)
        {
            // Converts to radians
            var radiansAngle = Math.PI / 180.0 * (angle - 90);
            var x = radius * Math.Cos(radiansAngle);
            var y = radius * Math.Sin(radiansAngle);
            return new Point(x + center.X, y + center.Y);
        }
    }
}