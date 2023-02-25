// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;
using BaseProj.SharpVectors.SharpVectorCss.Css;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgColor.
    /// </summary>
    public class SvgColor : CssValue, ISvgColor
    {
        #region Private Level Fields

        protected CssColor _rgbColor;

        #endregion

        #region Protected Methods

        protected void ParseColor(string str)
        {
            str = str.Trim();
            if (str.Equals("currentColor"))
            {
                SetColor(SvgColorType.CurrentColor, null, null);
            }
            else if (str.IndexOf("icc-color(") > -1)
            {
                var iccStart = str.IndexOf("icc-color(");
                var strRgb = str.Substring(0, iccStart).Trim();
                var strIcc = str.Substring(iccStart);

                SetColor(SvgColorType.RgbColorIccColor, strRgb, strIcc);
            }
            else if (str.StartsWith("rgba"))
            {
                SetColor(SvgColorType.RgbaColor, str, string.Empty);
            }
            else
            {
                SetColor(SvgColorType.RgbColor, str, string.Empty);
            }
        }

        #endregion

        #region Constructors and Destructor

        protected SvgColor()
            : base(CssValueType.PrimitiveValue, string.Empty, false)
        {
        }

        public SvgColor(string str)
            : base(CssValueType.PrimitiveValue, str, false)
        {
            ParseColor(str);
        }

        #endregion

        #region Public Properties

        public override string CssText
        {
            get
            {
                string ret;
                switch (ColorType)
                {
                    case SvgColorType.RgbColor:
                        ret = _rgbColor.CssText;
                        break;
                    case SvgColorType.RgbColorIccColor:
                        ret = _rgbColor.CssText;
                        break;
                    case SvgColorType.CurrentColor:
                        ret = "currentColor";
                        break;
                    default:
                        ret = string.Empty;
                        break;
                }

                return ret;
            }
            set
            {
                base.CssText = value;
                ParseColor(value);
            }
        }

        public SvgColorType ColorType { get; private set; }

        public ICssColor RgbColor => _rgbColor;

        public ISvgIccColor IccColor => throw new NotImplementedException();

        #endregion

        #region Public Methods

        public void SetRgbColor(string rgbColor)
        {
            SetColor(SvgColorType.RgbColor, rgbColor, string.Empty);
        }

        public void SetRgbColorIccColor(string rgbColor, string iccColor)
        {
            SetColor(SvgColorType.RgbColorIccColor, rgbColor, iccColor);
        }

        public void SetColor(SvgColorType colorType, string rgbColor, string iccColor)
        {
            ColorType = colorType;
            if (rgbColor != null && rgbColor.Length > 0)
                try
                {
                    _rgbColor = new CssColor(rgbColor);
                }
                catch (DomException domExc)
                {
                    throw new SvgException(SvgExceptionType.SvgInvalidValueErr,
                        "Invalid color value: " + rgbColor, domExc);
                }
            else
                _rgbColor = new CssColor("black");

            //TODO--PAUL: deal with ICC colors
        }

        #endregion
    }
}