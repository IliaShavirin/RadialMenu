using System;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfSvgColor : SvgColor
    {
        private readonly SvgStyleableElement _element;
        private readonly string _propertyName;

        public WpfSvgColor(SvgStyleableElement elm, string propertyName)
            : base(elm.GetComputedStyle("").GetPropertyValue(propertyName))
        {
            _element = elm;
            _propertyName = propertyName;
        }

        public Color Color
        {
            get
            {
                SvgColor colorToUse;
                if (ColorType == SvgColorType.CurrentColor)
                {
                    var sCurColor = _element.GetComputedStyle("").GetPropertyValue("color");
                    colorToUse = new SvgColor(sCurColor);
                }
                else if (ColorType == SvgColorType.Unknown)
                {
                    colorToUse = new SvgColor("black");
                }
                else
                {
                    colorToUse = this;
                }

                var rgbColor = colorToUse.RgbColor;
                var red = Convert.ToInt32(rgbColor.Red.GetFloatValue(CssPrimitiveType.Number));
                var green = Convert.ToInt32(rgbColor.Green.GetFloatValue(CssPrimitiveType.Number));
                var blue = Convert.ToInt32(rgbColor.Blue.GetFloatValue(CssPrimitiveType.Number));

                return Color.FromArgb((byte)Alpha, (byte)red, (byte)green, (byte)blue);
            }
        }

        public int Alpha
        {
            get
            {
                string propName;
                if (_propertyName.Equals("stop-color"))
                    propName = "stop-opacity";
                else if (_propertyName.Equals("flood-color"))
                    propName = "flood-opacity";
                else
                    return 255;

                double alpha = 255;
                string opacity;

                opacity = _element.GetPropertyValue(propName);
                if (opacity.Length > 0)
                    alpha *= SvgNumber.ParseNumber(opacity);

                alpha = Math.Min(alpha, 255);
                alpha = Math.Max(alpha, 0);

                return Convert.ToInt32(alpha);
            }
        }

        public double Opacity
        {
            get
            {
                string propName;
                if (_propertyName.Equals("stop-color"))
                    propName = "stop-opacity";
                else if (_propertyName.Equals("flood-color"))
                    propName = "flood-opacity";
                else
                    return 1.0f;

                double alpha = 1.0f;
                string opacity;

                opacity = _element.GetPropertyValue(propName);
                if (opacity != null && opacity.Length > 0) alpha = SvgNumber.ParseNumber(opacity);

                alpha = Math.Min(alpha, 1.0f);
                alpha = Math.Max(alpha, 0.0f);

                return alpha;
            }
        }
    }
}