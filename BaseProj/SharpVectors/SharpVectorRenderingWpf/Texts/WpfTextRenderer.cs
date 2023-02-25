using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Text;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public abstract class WpfTextRenderer
    {
        #region Constructors and Destructor

        protected WpfTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
        {
            if (textElement == null)
                throw new ArgumentNullException("textElement",
                    "The SVG text element is required, and cannot be null (or Nothing).");
            if (textRendering == null)
                throw new ArgumentNullException("textRendering",
                    "The text rendering object is required, and cannot be null (or Nothing).");

            _textElement = textElement;
            _textRendering = textRendering;
        }

        #endregion

        #region Protected Fields

        protected const string Whitespace = " ";

        protected static readonly Regex _tabNewline = new Regex(@"[\n\f\t]");
        protected static readonly Regex _decimalNumber = new Regex(@"^\d");

        protected string _actualFontName;

        protected DrawingContext _textContext;
        protected SvgTextElement _textElement;

        protected WpfDrawingContext _drawContext;
        protected WpfTextRendering _textRendering;

        #endregion

        #region Public Properties

        public bool IsInitialized => _textContext != null && _drawContext != null;

        public DrawingContext TextContext => _textContext;

        public SvgTextElement TextElement => _textElement;

        public WpfDrawingContext DrawContext => _drawContext;

        #endregion

        #region Protected Properties

        protected bool IsMeasuring
        {
            get
            {
                if (_textRendering != null) return _textRendering.IsMeasuring;

                return false;
            }
        }

        protected bool IsTextPath
        {
            get
            {
                if (_textRendering != null) return _textRendering.IsTextPath;

                return false;
            }
            set
            {
                if (_textRendering != null) _textRendering.IsTextPath = value;
            }
        }

        protected double TextWidth
        {
            get
            {
                if (_textRendering != null) return _textRendering.TextWidth;

                return 0;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(DrawingContext textContext, WpfDrawingContext drawContext)
        {
            if (textContext == null)
                throw new ArgumentNullException("textContext",
                    "The text context is required, and cannot be null (or Nothing).");
            if (drawContext == null)
                throw new ArgumentNullException("drawContext",
                    "The drawing context is required, and cannot be null (or Nothing).");

            _textContext = textContext;
            _drawContext = drawContext;
        }

        public virtual void Uninitialize()
        {
            _textContext = null;
            _drawContext = null;
        }

        public abstract void RenderSingleLineText(SvgTextContentElement element,
            ref Point startPos, string text, double rotate, WpfTextPlacement placement);

        public abstract void RenderTextRun(SvgTextContentElement element,
            ref Point startPos, string text, double rotate, WpfTextPlacement placement);

        #region TRef/TSpan Methods

        public static string TrimText(SvgTextContentElement element, string val)
        {
            if (element.XmlSpace != "preserve")
                val = val.Replace("\n", string.Empty);
            val = _tabNewline.Replace(val, " ");

            if (element.XmlSpace == "preserve" || element.XmlSpace == "default")
                return val;
            return val.Trim();
        }

        public static string GetText(SvgTextContentElement element, XmlNode child)
        {
            return TrimText(element, child.Value);
        }

        public static string GetTRefText(SvgTRefElement element)
        {
            var refElement = element.ReferencedElement;
            if (refElement != null)
                return TrimText(element, refElement.InnerText);
            return string.Empty;
        }

        #endregion

        #region TextPosition/Size Methods

        public static WpfTextPlacement GetCurrentTextPosition(SvgTextPositioningElement posElement, Point p)
        {
            var xValues = posElement.X.AnimVal;
            var yValues = posElement.Y.AnimVal;
            var dxValues = posElement.Dx.AnimVal;
            var dyValues = posElement.Dy.AnimVal;
            var rValues = posElement.Rotate.AnimVal;

            var requiresGlyphPositioning = false;
            var isXYGlyphPositioning = false;
            var isDxyGlyphPositioning = false;
            var isRotateGlyphPositioning = false;

            var xValue = p.X;
            var yValue = p.Y;
            double rValue = 0;
            double dxValue = 0;
            double dyValue = 0;

            WpfTextPlacement textPlacement = null;

            if (xValues.NumberOfItems > 0)
            {
                if (xValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                xValue = xValues.GetItem(0).Value;
                p.X = xValue;
            }

            if (yValues.NumberOfItems > 0)
            {
                if (yValues.NumberOfItems > 1)
                {
                    isXYGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                yValue = yValues.GetItem(0).Value;
                p.Y = yValue;
            }

            if (dxValues.NumberOfItems > 0)
            {
                if (dxValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                dxValue = dxValues.GetItem(0).Value;
                p.X += dxValue;
            }

            if (dyValues.NumberOfItems > 0)
            {
                if (dyValues.NumberOfItems > 1)
                {
                    isDxyGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                dyValue = dyValues.GetItem(0).Value;
                p.Y += dyValue;
            }

            if (rValues.NumberOfItems > 0)
            {
                if (rValues.NumberOfItems > 1)
                {
                    isRotateGlyphPositioning = true;
                    requiresGlyphPositioning = true;
                }

                rValue = rValues.GetItem(0).Value;
            }

            if (requiresGlyphPositioning)
            {
                var xCount = xValues.NumberOfItems;
                var yCount = yValues.NumberOfItems;
                var dxCount = dxValues.NumberOfItems;
                var dyCount = dyValues.NumberOfItems;
                var rCount = rValues.NumberOfItems;

                List<WpfTextPosition> textPositions = null;

                var isRotateOnly = false;

                if (isXYGlyphPositioning)
                {
                    var itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount = Math.Max(itemCount, rCount);
                    textPositions = new List<WpfTextPosition>((int)itemCount);

                    double xLast = 0;
                    double yLast = 0;

                    for (uint i = 0; i < itemCount; i++)
                    {
                        var xNext = i < xCount ? xValues.GetItem(i).Value : xValue;
                        var yNext = i < yCount ? yValues.GetItem(i).Value : yValue;
                        var rNext = i < rCount ? rValues.GetItem(i).Value : rValue;
                        var dxNext = i < dxCount ? dxValues.GetItem(i).Value : dxValue;
                        var dyNext = i < dyCount ? dyValues.GetItem(i).Value : dyValue;

                        if (i < xCount)
                            xLast = xNext;
                        else
                            xNext = xLast;
                        if (i < yCount)
                            yLast = yNext;
                        else
                            yNext = yLast;

                        var textPosition = new WpfTextPosition(
                            new Point(xNext + dxNext, yNext + dyNext), rNext);

                        textPositions.Add(textPosition);
                    }
                }
                else if (isDxyGlyphPositioning)
                {
                }
                else if (isRotateGlyphPositioning)
                {
                    isRotateOnly = true;
                    var itemCount = Math.Max(Math.Max(xCount, yCount), Math.Max(dxCount, dyCount));
                    itemCount = Math.Max(itemCount, rCount);
                    textPositions = new List<WpfTextPosition>((int)itemCount);

                    for (uint i = 0; i < itemCount; i++)
                    {
                        var rNext = i < rCount ? rValues.GetItem(i).Value : rValue;

                        var textPosition = new WpfTextPosition(p, rNext);

                        textPositions.Add(textPosition);
                    }
                }

                if (textPositions != null && textPositions.Count != 0)
                    textPlacement = new WpfTextPlacement(p, rValue, textPositions, isRotateOnly);
                else
                    textPlacement = new WpfTextPlacement(p, rValue);
            }
            else
            {
                textPlacement = new WpfTextPlacement(p, rValue);
            }

            return textPlacement;
        }

        public static double GetComputedFontSize(SvgTextContentElement element)
        {
            var str = element.GetPropertyValue("font-size");
            double fontSize = 12;
            if (str.EndsWith("%"))
            {
                // percentage of inherited value
            }
            else if (_decimalNumber.IsMatch(str))
            {
                // svg length
                fontSize = new SvgLength(element, "font-size",
                    SvgLengthDirection.Viewport, str, "10px").Value;
            }
            else if (str == "larger")
            {
            }
            else if (str == "smaller")
            {
            }

            // check for absolute value
            return fontSize;
        }

        #endregion

        #endregion

        #region Protected Methods

        #region Helper Methods

        protected void SetTextWidth(double textWidth)
        {
            if (_textRendering != null && textWidth != 0) _textRendering.SetTextWidth(textWidth);
        }

        protected void AddTextWidth(double textWidth)
        {
            if (_textRendering != null && textWidth != 0) _textRendering.AddTextWidth(textWidth);
        }

        protected Brush GetBrush()
        {
            var paint = new WpfSvgPaint(_drawContext, _textElement, "fill");

            return paint.GetBrush();
        }

        protected Pen GetPen()
        {
            var paint = new WpfSvgPaint(_drawContext, _textElement, "stroke");

            return paint.GetPen();
        }

        /// <summary>
        ///     This will extract a <see cref="PathGeometry" /> that is nested into GeometryGroup, which
        ///     is normally created by the FormattedText.BuildGeometry() method.
        /// </summary>
        /// <param name="sourceGeometry"></param>
        /// <returns></returns>
        protected static Geometry ExtractTextPathGeometry(Geometry sourceGeometry)
        {
            var outerGroup = sourceGeometry as GeometryGroup;
            if (outerGroup != null && outerGroup.Children.Count == 1)
            {
                var innerGroup = outerGroup.Children[0] as GeometryGroup;
                if (innerGroup != null && innerGroup.Children.Count == 1) return innerGroup.Children[0];

                return innerGroup;
            }

            return sourceGeometry;
        }

        #endregion

        #region FontWeight Methods

        protected FontWeight GetTextFontWeight(SvgTextContentElement element)
        {
            var fontWeight = element.GetPropertyValue("font-weight");
            if (string.IsNullOrEmpty(fontWeight)) return FontWeights.Normal;

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Normal;
                case "bold":
                    return FontWeights.Bold;
                case "100":
                    return FontWeights.Thin;
                case "200":
                    return FontWeights.ExtraLight;
                case "300":
                    return FontWeights.Light;
                case "400":
                    return FontWeights.Normal;
                case "500":
                    return FontWeights.Medium;
                case "600":
                    return FontWeights.SemiBold;
                case "700":
                    return FontWeights.Bold;
                case "800":
                    return FontWeights.ExtraBold;
                case "900":
                    return FontWeights.Black;
                case "950":
                    return FontWeights.UltraBlack;
            }

            if (string.Equals(fontWeight, "bolder", StringComparison.OrdinalIgnoreCase))
            {
                var parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!string.IsNullOrEmpty(fontWeight)) return GetBolderFontWeight(fontWeight);
                }

                return FontWeights.ExtraBold;
            }

            if (string.Equals(fontWeight, "lighter", StringComparison.OrdinalIgnoreCase))
            {
                var parentElement = element.ParentNode as SvgTransformableElement;
                if (parentElement != null)
                {
                    fontWeight = parentElement.GetPropertyValue("font-weight");
                    if (!string.IsNullOrEmpty(fontWeight)) return GetLighterFontWeight(fontWeight);
                }

                return FontWeights.Light;
            }

            return FontWeights.Normal;
        }

        protected FontWeight GetBolderFontWeight(string fontWeight)
        {
            if (string.IsNullOrEmpty(fontWeight)) return FontWeights.Normal;

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Bold;
                case "bold":
                    return FontWeights.ExtraBold;
                case "100":
                    return FontWeights.ExtraLight;
                case "200":
                    return FontWeights.Light;
                case "300":
                    return FontWeights.Normal;
                case "400":
                    return FontWeights.Bold;
                case "500":
                    return FontWeights.SemiBold;
                case "600":
                    return FontWeights.Bold;
                case "700":
                    return FontWeights.ExtraBold;
                case "800":
                    return FontWeights.Black;
                case "900":
                    return FontWeights.UltraBlack;
                case "950":
                    return FontWeights.UltraBlack;
            }

            return FontWeights.Normal;
        }

        protected FontWeight GetLighterFontWeight(string fontWeight)
        {
            if (string.IsNullOrEmpty(fontWeight)) return FontWeights.Normal;

            switch (fontWeight)
            {
                case "normal":
                    return FontWeights.Light;
                case "bold":
                    return FontWeights.Normal;

                case "100":
                    return FontWeights.Thin;
                case "200":
                    return FontWeights.Thin;
                case "300":
                    return FontWeights.ExtraLight;
                case "400":
                    return FontWeights.Light;
                case "500":
                    return FontWeights.Normal;
                case "600":
                    return FontWeights.Medium;
                case "700":
                    return FontWeights.SemiBold;
                case "800":
                    return FontWeights.Bold;
                case "900":
                    return FontWeights.ExtraBold;
                case "950":
                    return FontWeights.Black;
            }

            return FontWeights.Normal;
        }

        #endregion

        #region FontStyle/Stretch Methods

        protected FontStyle GetTextFontStyle(SvgTextContentElement element)
        {
            var fontStyle = element.GetPropertyValue("font-style");
            if (string.IsNullOrEmpty(fontStyle)) return FontStyles.Normal;

            if (fontStyle == "normal") return FontStyles.Normal;
            if (fontStyle == "italic") return FontStyles.Italic;
            if (fontStyle == "oblique") return FontStyles.Oblique;

            return FontStyles.Normal;
        }

        protected FontStretch GetTextFontStretch(SvgTextContentElement element)
        {
            var fontStretch = element.GetPropertyValue("font-stretch");
            if (string.IsNullOrEmpty(fontStretch)) return FontStretches.Normal;

            switch (fontStretch)
            {
                case "normal":
                    return FontStretches.Normal;
                case "ultra-condensed":
                    return FontStretches.UltraCondensed;
                case "extra-condensed":
                    return FontStretches.ExtraCondensed;
                case "condensed":
                    return FontStretches.Condensed;
                case "semi-condensed":
                    return FontStretches.SemiCondensed;
                case "semi-expanded":
                    return FontStretches.SemiExpanded;
                case "expanded":
                    return FontStretches.Expanded;
                case "extra-expanded":
                    return FontStretches.ExtraExpanded;
                case "ultra-expanded":
                    return FontStretches.UltraExpanded;
            }

            return FontStretches.Normal;
        }

        #endregion

        #region Other Text/Font Attributes

        protected TextDecorationCollection GetTextDecoration(SvgTextContentElement element)
        {
            var textDeco = element.GetPropertyValue("text-decoration");
            if (textDeco == "line-through") return TextDecorations.Strikethrough;
            if (textDeco == "underline") return TextDecorations.Underline;
            if (textDeco == "overline") return TextDecorations.OverLine;

            return null;
        }

        protected FontFamily GetTextFontFamily(SvgTextContentElement element, double fontSize)
        {
            _actualFontName = null;

            var fontFamily = element.GetPropertyValue("font-family");
            string[] fontNames = fontNames = fontFamily.Split(',');

            FontFamily family;

            foreach (var fn in fontNames)
                try
                {
                    var fontName = fn.Trim(' ', '\'', '"');

                    if (string.Equals(fontName, "serif", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericSerif;
                    }
                    else if (string.Equals(fontName, "sans-serif", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericSansSerif;
                    }
                    else if (string.Equals(fontName, "monospace", StringComparison.OrdinalIgnoreCase))
                    {
                        family = WpfDrawingSettings.GenericMonospace;
                    }
                    else
                    {
                        family = new FontFamily(fontName);
                        _actualFontName = fontName;
                    }

                    return family;
                }
                catch
                {
                }

            // no known font-family was found => default to Arial
            return WpfDrawingSettings.DefaultFontFamily;
        }

        protected WpfTextStringFormat GetTextStringFormat(SvgTextContentElement element)
        {
            var sf = WpfTextStringFormat.Default;

            var doAlign = true;
            if (element is SvgTSpanElement || element is SvgTRefElement)
            {
                var posElement = (SvgTextPositioningElement)element;
                if (posElement.X.AnimVal.NumberOfItems == 0)
                    doAlign = false;
            }

            var dir = element.GetPropertyValue("direction");
            var isRightToLeft = dir == "rtl";
            sf.Direction = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            if (doAlign)
            {
                var anchor = element.GetPropertyValue("text-anchor");

                if (isRightToLeft)
                {
                    if (anchor == "middle")
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (anchor == "end")
                        sf.Anchor = WpfTextAnchor.Start;
                    else
                        sf.Anchor = WpfTextAnchor.End;
                }
                else
                {
                    if (anchor == "middle")
                        sf.Anchor = WpfTextAnchor.Middle;
                    else if (anchor == "end")
                        sf.Anchor = WpfTextAnchor.End;
                }
            }
            else
            {
                var textElement = element.ParentNode as SvgTextElement;
                if (textElement != null)
                {
                    var anchor = textElement.GetPropertyValue("text-anchor");
                    if (isRightToLeft)
                    {
                        if (anchor == "middle")
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (anchor == "end")
                            sf.Anchor = WpfTextAnchor.Start;
                        else
                            sf.Anchor = WpfTextAnchor.End;
                    }
                    else
                    {
                        if (anchor == "middle")
                            sf.Anchor = WpfTextAnchor.Middle;
                        else if (anchor == "end")
                            sf.Anchor = WpfTextAnchor.End;
                    }
                }
            }

            //if (isRightToLeft)
            //{
            //    if (sf.Alignment == TextAlignment.Right)
            //        sf.Alignment = TextAlignment.Left;
            //    else if (sf.Alignment == TextAlignment.Left)
            //        sf.Alignment = TextAlignment.Right;

            //    //sf.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            //}

            //dir = element.GetPropertyValue("writing-mode");
            //if (dir == "tb")
            //{
            //    sf.FormatFlags = sf.FormatFlags | StringFormatFlags.DirectionVertical;
            //}

            //sf.FormatFlags = sf.FormatFlags | StringFormatFlags.MeasureTrailingSpaces;

            return sf;
        }

        #endregion

        #endregion
    }
}