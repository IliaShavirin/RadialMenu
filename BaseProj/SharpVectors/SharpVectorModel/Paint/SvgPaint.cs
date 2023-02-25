using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Painting;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.Paint
{
    public class SvgPaint : SvgColor, ISvgPaint
    {
        #region Constructors and Destructor

        public SvgPaint(string str)
        {
            Uri = string.Empty;
            ParsePaint(str);
        }

        #endregion

        #region Private Fields

        #endregion

        #region Private methods

        private void ParsePaint(string str)
        {
            var hasUri = false;
            var hasRgb = false;
            var hasIcc = false;
            var hasNone = false;
            var hasCurrentColor = false;

            str = str.Trim();

            if (str.StartsWith("url("))
            {
                hasUri = true;
                var endUri = str.IndexOf(")");
                Uri = str.Substring(4, endUri - 4);
                str = str.Substring(endUri + 1).Trim();
            }

            if (str.Equals("currentColor"))
            {
                ParseColor(str);
                hasCurrentColor = true;
            }
            else if (str.Equals("none"))
            {
                hasNone = true;
            }
            else if (str.Length > 0)
            {
                ParseColor(str);
                hasRgb = true;
                hasIcc = ColorType == SvgColorType.RgbColorIccColor;
            }

            SetPaintType(hasUri, hasRgb, hasIcc, hasNone, hasCurrentColor);
        }

        private void SetPaintType(bool hasUri, bool hasRgb, bool hasIcc,
            bool hasNone, bool hasCurrentColor)
        {
            if (hasUri)
            {
                if (hasRgb)
                {
                    if (hasIcc)
                        PaintType = SvgPaintType.UriRgbColorIccColor;
                    else
                        PaintType = SvgPaintType.UriRgbColor;
                }
                else if (hasNone)
                {
                    PaintType = SvgPaintType.UriNone;
                }
                else if (hasCurrentColor)
                {
                    PaintType = SvgPaintType.UriCurrentColor;
                }
                else
                {
                    PaintType = SvgPaintType.Uri;
                }
            }
            else
            {
                if (hasRgb)
                {
                    if (hasIcc)
                        PaintType = SvgPaintType.RgbColorIccColor;
                    else
                        PaintType = SvgPaintType.RgbColor;
                }
                else if (hasNone)
                {
                    PaintType = SvgPaintType.None;
                }
                else if (hasCurrentColor)
                {
                    PaintType = SvgPaintType.CurrentColor;
                }
                else
                {
                    PaintType = SvgPaintType.Unknown;
                }
            }
        }

        #endregion

        #region ISvgPaint Members

        public override string CssText
        {
            get
            {
                string cssText;
                switch (PaintType)
                {
                    case SvgPaintType.CurrentColor:
                    case SvgPaintType.RgbColor:
                    case SvgPaintType.RgbColorIccColor:
                        cssText = base.CssText;
                        break;
                    case SvgPaintType.None:
                        cssText = "none";
                        break;
                    case SvgPaintType.UriNone:
                        cssText = "url(" + Uri + ") none";
                        break;
                    case SvgPaintType.Uri:
                        cssText = "url(" + Uri + ")";
                        break;
                    case SvgPaintType.UriCurrentColor:
                    case SvgPaintType.UriRgbColor:
                    case SvgPaintType.UriRgbColorIccColor:
                        cssText = "url(" + Uri + ") " + base.CssText;
                        break;
                    default:
                        cssText = string.Empty;
                        break;
                }

                return cssText;
            }
            set => ParsePaint(value);
        }

        public SvgPaintType PaintType { get; private set; }

        public string Uri { get; private set; }

        public void SetUri(string uri)
        {
            PaintType = SvgPaintType.Uri;
            Uri = uri;
        }

        public void SetPaint(SvgPaintType paintType, string uri, string rgbColor, string iccColor)
        {
            PaintType = paintType;

            // check URI
            switch (PaintType)
            {
                case SvgPaintType.Uri:
                case SvgPaintType.UriCurrentColor:
                case SvgPaintType.UriNone:
                case SvgPaintType.UriRgbColor:
                case SvgPaintType.UriRgbColorIccColor:
                    if (uri == null)
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing URI");
                    Uri = uri;
                    break;
                default:
                    if (uri != null) throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "URI must be null");
                    break;
            }

            // check RGB and ICC color
            switch (PaintType)
            {
                case SvgPaintType.CurrentColor:
                case SvgPaintType.UriCurrentColor:
                    ParseColor("currentColor");
                    break;
                case SvgPaintType.RgbColor:
                case SvgPaintType.UriRgbColor:
                    if (rgbColor != null && rgbColor.Length > 0)
                        SetRgbColor(rgbColor);
                    else
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing RGB color");
                    break;
                case SvgPaintType.RgbColorIccColor:
                case SvgPaintType.UriRgbColorIccColor:
                    if (rgbColor != null && rgbColor.Length > 0 &&
                        iccColor != null && iccColor.Length > 0)
                        SetRgbColorIccColor(rgbColor, iccColor);
                    else
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "Missing RGB or ICC color");
                    break;
                default:
                    if (rgbColor != null)
                        throw new SvgException(SvgExceptionType.SvgInvalidValueErr, "rgbColor must be null");
                    break;
            }
        }

        #endregion
    }
}