using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.Converters
{
    public class SVGControlDrawingIdResolveConverter : MarkupExtension, IValueConverter
    {
        private static SVGControlDrawingIdResolveConverter _converter;

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Drawing drawing)
            {
                var id = SvgObject.GetId(drawing);
                return id;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _converter ?? (_converter = new SVGControlDrawingIdResolveConverter());
        }
    }
}