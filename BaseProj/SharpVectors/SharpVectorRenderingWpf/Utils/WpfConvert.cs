using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.Fills;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils
{
    public static class WpfConvert
    {
        /// <summary>
        ///     A GDI Color representation of the RgbColor
        /// </summary>
        public static Color? ToColor(ICssColor color)
        {
            if (color == null) return null;

            var dRed = color.Red.GetFloatValue(CssPrimitiveType.Number);
            var dGreen = color.Green.GetFloatValue(CssPrimitiveType.Number);
            var dBlue = color.Blue.GetFloatValue(CssPrimitiveType.Number);
            var dAlpha = color.Alpha.GetFloatValue(CssPrimitiveType.Number) * 255;

            if (double.IsNaN(dRed) || double.IsInfinity(dRed)) return null;
            if (double.IsNaN(dGreen) || double.IsInfinity(dGreen)) return null;
            if (double.IsNaN(dBlue) || double.IsInfinity(dBlue)) return null;
            if (double.IsNaN(dAlpha) || double.IsInfinity(dAlpha)) return null;

            return Color.FromArgb(Convert.ToByte(dAlpha), Convert.ToByte(dRed), Convert.ToByte(dGreen),
                Convert.ToByte(dBlue));
        }

        public static Rect ToRect(ICssRect rect)
        {
            if (rect == null) return Rect.Empty;

            var x = rect.Left.GetFloatValue(CssPrimitiveType.Px);
            var y = rect.Top.GetFloatValue(CssPrimitiveType.Px);
            var width = rect.Right.GetFloatValue(CssPrimitiveType.Px) - x;
            var height = rect.Bottom.GetFloatValue(CssPrimitiveType.Px) - y;

            return new Rect(x, y, width, height);
        }


        /// <summary>
        ///     This converts the specified <see cref="Rect" /> structure to a
        ///     <see cref="SvgRectF" /> structure.
        /// </summary>
        /// <param name="rect">The <see cref="Rect" /> structure to convert.</param>
        /// <returns>
        ///     The <see cref="SvgRectF" /> structure that is converted from the
        ///     specified <see cref="Rect" /> structure.
        /// </returns>
        public static Rect ToRect(SvgRectF rect)
        {
            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static Rect ToRect(ISvgRect rect)
        {
            if (rect == null) return Rect.Empty;

            return new Rect(rect.X, rect.Y, rect.Width, rect.Height);
        }

        public static GradientSpreadMethod ToSpreadMethod(SvgSpreadMethod sm)
        {
            switch (sm)
            {
                case SvgSpreadMethod.Pad:
                    return GradientSpreadMethod.Pad;
                case SvgSpreadMethod.Reflect:
                    return GradientSpreadMethod.Reflect;
                case SvgSpreadMethod.Repeat:
                    return GradientSpreadMethod.Repeat;
            }

            return GradientSpreadMethod.Pad;
        }
    }
}