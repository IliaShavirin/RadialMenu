using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using RadialMenu.CustomControls;

namespace RadialMenu.Converters
{
    internal class RadialMenuItemsCountToStrokeThickness : MarkupExtension, IValueConverter
    {
        private static RadialMenuItemsCountToStrokeThickness _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var radialMenuItem = value as RadialMenuItem;
            CustomControls.RadialMenu radialMenu;

            if (radialMenuItem != null)
                radialMenu = radialMenuItem.RadialMenu;
            else
                radialMenu = value as CustomControls.RadialMenu;

            if (radialMenu != null)
            {
                var param = parameter != null ? int.Parse((string)parameter) : 0;

                var maxR = radialMenu.MaxStrokeThickness;
                var minR = radialMenu.MinStrokeThickness;

                var coef = (maxR - minR) / 13;
                var sectrorsCount = radialMenu.Items.Count;
                if (parameter == null)
                {
                    return minR + (16 - sectrorsCount) * coef;
                }

                radialMenuItem.InnerRadius = minR - (16 - sectrorsCount) * coef + param;
                return minR - (16 - sectrorsCount) * coef + param;
            }

            return 110;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new InvalidOperationException("RadialMenuItemToArrowPosition is a One-Way converter only !");
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new RadialMenuItemsCountToStrokeThickness());
        }
    }
}