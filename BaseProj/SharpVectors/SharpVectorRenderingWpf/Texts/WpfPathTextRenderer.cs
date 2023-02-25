using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.Text;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts
{
    public sealed class WpfPathTextRenderer : WpfTextRenderer
    {
        #region Constructors and Destructor

        public WpfPathTextRenderer(SvgTextElement textElement, WpfTextRendering textRendering)
            : base(textElement, textRendering)
        {
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Methods

        public override void RenderSingleLineText(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
            RenderTextPath((SvgTextPathElement)element, ref ctp, rotate, placement);
        }

        public override void RenderTextRun(SvgTextContentElement element,
            ref Point ctp, string text, double rotate, WpfTextPlacement placement)
        {
        }

        #endregion

        #region Private Methods

        private void RenderTextPath(SvgTextPathElement textPath, ref Point ctp,
            double rotate, WpfTextPlacement placement)
        {
            if (textPath.ChildNodes == null || textPath.ChildNodes.Count == 0) return;

            var targetPath = textPath.ReferencedElement as SvgElement;
            if (targetPath == null) return;

            var pathGeometry = WpfRendering.CreateGeometry(targetPath, true) as PathGeometry;
            if (pathGeometry == null) return;

            IsTextPath = true;

            var pathDrawing = new WpfTextOnPathDrawing();

            pathDrawing.BeginTextPath();

            var nodeType = XmlNodeType.None;

            foreach (XmlNode child in textPath.ChildNodes)
            {
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text)
                {
                    RenderTextPath(textPath, pathDrawing, GetText(textPath, child),
                        new Point(ctp.X, ctp.Y), rotate, placement);
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    var nodeName = child.Name;
                    if (string.Equals(nodeName, "tref"))
                        RenderTRefPath((SvgTRefElement)child, pathDrawing, ref ctp);
                    else if (string.Equals(nodeName, "tspan"))
                        RenderTSpanPath((SvgTSpanElement)child, pathDrawing, ref ctp);
                }
            }

            var stringFormat = GetTextStringFormat(_textElement);

            var pathOffset = textPath.StartOffset;
            var pathMethod = (SvgTextPathMethod)textPath.Method.BaseVal;
            var pathSpacing = (SvgTextPathSpacing)textPath.Spacing.BaseVal;
            pathDrawing.DrawTextPath(_textContext, pathGeometry, pathOffset,
                stringFormat.Alignment, pathMethod, pathSpacing);

            pathDrawing.EndTextPath();
        }

        private void RenderTRefPath(SvgTRefElement element, WpfTextOnPathDrawing pathDrawing,
            ref Point ctp)
        {
            var placement = GetCurrentTextPosition(element, ctp);
            ctp = placement.Location;
            var rotate = placement.Rotation;
            if (!placement.HasPositions) placement = null; // Render it useless.

            RenderTextPath(element, pathDrawing, GetTRefText(element),
                new Point(ctp.X, ctp.Y), rotate, placement);
        }

        private void RenderTSpanPath(SvgTSpanElement element, WpfTextOnPathDrawing pathDrawing,
            ref Point ctp)
        {
            var placement = GetCurrentTextPosition(element, ctp);
            ctp = placement.Location;
            var rotate = placement.Rotation;
            if (!placement.HasPositions) placement = null; // Render it useless.

            var sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                var textElement = (SvgTextElement)element.SelectSingleNode("ancestor::svg:text",
                    element.OwnerDocument.NamespaceManager);

                var textFontSize = GetComputedFontSize(textElement);
                if (sBaselineShift.EndsWith("%"))
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100f * textFontSize;
                else if (sBaselineShift == "sub")
                    shiftBy = -0.6F * textFontSize;
                else if (sBaselineShift == "super")
                    shiftBy = 0.6F * textFontSize;
                else if (sBaselineShift == "baseline")
                    shiftBy = 0;
                else
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
            }

            foreach (XmlNode child in element.ChildNodes)
                if (child.NodeType == XmlNodeType.Text)
                {
                    ctp.Y -= shiftBy;
                    RenderTextPath(element, pathDrawing, GetText(element, child),
                        new Point(ctp.X, ctp.Y), rotate, placement);
                    ctp.Y += shiftBy;
                }
        }

        private void RenderTextPath(SvgTextContentElement element, WpfTextOnPathDrawing pathDrawing,
            string text, Point origin, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text)) return;

            var emSize = GetComputedFontSize(element);
            var fontFamily = GetTextFontFamily(element, emSize);

            var fontStyle = GetTextFontStyle(element);
            var fontWeight = GetTextFontWeight(element);

            var fontStretch = GetTextFontStretch(element);

            var stringFormat = GetTextStringFormat(element);

            // Fix the use of Postscript fonts...
            var fontFamilyVisitor = _drawContext.FontFamilyVisitor;
            if (!string.IsNullOrEmpty(_actualFontName) && fontFamilyVisitor != null)
            {
                var currentFamily = new WpfFontFamilyInfo(fontFamily, fontWeight,
                    fontStyle, fontStretch);
                var familyInfo = fontFamilyVisitor.Visit(_actualFontName,
                    currentFamily, _drawContext);
                if (familyInfo != null && !familyInfo.IsEmpty)
                {
                    fontFamily = familyInfo.Family;
                    fontWeight = familyInfo.Weight;
                    fontStyle = familyInfo.Style;
                    fontStretch = familyInfo.Stretch;
                }
            }

            var fillPaint = new WpfSvgPaint(_drawContext, element, "fill");
            var textBrush = fillPaint.GetBrush();

            var strokePaint = new WpfSvgPaint(_drawContext, element, "stroke");
            var pen = strokePaint.GetPen();

            var textDecors = GetTextDecoration(element);
            var alignment = stringFormat.Alignment;

            pathDrawing.FontSize = emSize;

            pathDrawing.FontFamily = fontFamily;
            pathDrawing.FontWeight = fontWeight;
            pathDrawing.FontStretch = fontStretch;

            pathDrawing.Foreground = textBrush;

            pathDrawing.AddTextPath(text, origin);
        }

        #endregion
    }
}