// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>

using System.Globalization;
using System.Text.RegularExpressions;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Rectangles are defined as consisting of a (x,y) coordinate pair
    ///     identifying a minimum X value, a minimum Y value, and a width
    ///     and height, which are usually constrained to be non-negative.
    /// </summary>
    public sealed class SvgRect : ISvgRect
    {
        #region Static Fields

        public static readonly SvgRect Empty = new SvgRect(0, 0, 0, 0);

        #endregion

        #region Public Properties

        public bool IsEmpty => Width <= 0 || Height <= 0;

        #endregion

        #region Public Methods

        public override string ToString()
        {
            return "{X=" + X.ToString(CultureInfo.CurrentCulture)
                         + ",Y=" + Y.ToString(CultureInfo.CurrentCulture)
                         + ",Width=" + Width.ToString(CultureInfo.CurrentCulture)
                         + ",Height=" + Height.ToString(CultureInfo.CurrentCulture) + "}";
        }

        #endregion

        #region Private Fields

        #endregion

        #region Constructors

        public SvgRect(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public SvgRect(string str)
        {
            var replacedStr = Regex.Replace(str, @"(\s|,)+", ",");
            var tokens = replacedStr.Split(',');
            if (tokens.Length == 2)
            {
                X = 0;
                Y = 0;
                Width = SvgNumber.ParseNumber(tokens[0]);
                Height = SvgNumber.ParseNumber(tokens[1]);
            }
            else if (tokens.Length == 4)
            {
                X = SvgNumber.ParseNumber(tokens[0]);
                Y = SvgNumber.ParseNumber(tokens[1]);
                Width = SvgNumber.ParseNumber(tokens[2]);
                Height = SvgNumber.ParseNumber(tokens[3]);
            }
            else
            {
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr,
                    "Invalid SvgRect value: " + str);
            }
        }

        #endregion

        #region ISvgRect Interface

        public double X { get; set; }

        public double Y { get; set; }

        public double Width { get; set; }

        public double Height { get; set; }

        #endregion
    }
}