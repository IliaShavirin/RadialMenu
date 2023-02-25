using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Text;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Texts;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfTextRendering : WpfRendering
    {
        #region Constructors and Destructor

        public WpfTextRendering(SvgElement element)
            : base(element)
        {
            _textElement = element as SvgTextElement;
            if (_textElement == null) throw new InvalidOperationException();

            _horzRenderer = new WpfHorzTextRenderer(_textElement, this);
            _vertRenderer = new WpfVertTextRenderer(_textElement, this);
            _pathRenderer = new WpfPathTextRenderer(_textElement, this);
        }

        #endregion

        #region Private Fields

        private const string Whitespace = " ";

        private static readonly Regex _tabNewline = new Regex(@"[\n\f\t]");
        private static readonly Regex _decimalNumber = new Regex(@"^\d");

        private bool _isGroupAdded;

        private readonly SvgTextElement _textElement;

        private DrawingGroup _drawGroup;
        private DrawingContext _drawContext;
        private WpfDrawingContext _context;

        private WpfHorzTextRenderer _horzRenderer;
        private WpfVertTextRenderer _vertRenderer;
        private WpfPathTextRenderer _pathRenderer;

        #endregion

        #region Public Properties

        public override bool IsRecursive => true;

        public bool IsMeasuring { get; private set; }

        public bool IsTextPath { get; set; }

        public double TextWidth { get; private set; }

        #endregion

        #region Public Methods

        #region BeforeRender Method

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            IsTextPath = false;
            _isGroupAdded = false;
            TextWidth = 0;
            IsMeasuring = false;

            var context = renderer.Context;

            var hint = _svgElement.RenderingHint;
            if (hint == SvgRenderingHint.Clipping) return;
            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (string.Equals(_svgElement.ParentNode.LocalName, "clipPath") &&
                !context.RenderingClipRegion)
                return;

            var sVisibility = _textElement.GetPropertyValue("visibility");
            var sDisplay = _textElement.GetPropertyValue("display");
            if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none")) return;

            _context = renderer.Context;

            SetQuality(context);
            SetTransform(context);

            SetClip(_context);

            SetMask(_context);

            _drawGroup = new DrawingGroup();

            var elementId = GetElementName();
            if (!string.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
            {
                _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);

                context.RegisterId(elementId);

                if (context.IncludeRuntime) SvgObject.SetId(_drawGroup, elementId);
            }

            var textTransform = Transform;
            if (textTransform != null && !textTransform.Value.IsIdentity)
                _drawGroup.Transform = textTransform;
            else
                textTransform = null; // render any identity transform useless...
            var textClip = ClipGeometry;
            if (textClip != null && !textClip.IsEmpty())
                _drawGroup.ClipGeometry = textClip;
            else
                textClip = null; // render any empty geometry useless...
            var textMask = Masking;
            if (textMask != null) _drawGroup.OpacityMask = textMask;

            if (textTransform != null || textClip != null || textMask != null)
            {
                var curGroup = _context.Peek();
                Debug.Assert(curGroup != null);
                if (curGroup != null)
                {
                    curGroup.Children.Add(_drawGroup);
                    context.Push(_drawGroup);

                    _isGroupAdded = true;
                }
            }

            _drawContext = _drawGroup.Open();

            _horzRenderer.Initialize(_drawContext, _context);
            _vertRenderer.Initialize(_drawContext, _context);
            _pathRenderer.Initialize(_drawContext, _context);
        }

        #endregion

        #region Render Method

        public override void Render(WpfDrawingRenderer renderer)
        {
            if (_drawGroup == null || _drawContext == null) return;

            var ctp = new Point(0, 0); // current text position

            var placement = WpfTextRenderer.GetCurrentTextPosition(_textElement, ctp);
            ctp = placement.Location;
            var rotate = placement.Rotation;
            if (!placement.HasPositions) placement = null; // render it useless
            var sBaselineShift = _textElement.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                var textFontSize = WpfTextRenderer.GetComputedFontSize(_textElement);
                if (sBaselineShift.EndsWith("%"))
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift.Substring(0,
                        sBaselineShift.Length - 1)) / 100 * textFontSize;
                else if (sBaselineShift == "sub")
                    shiftBy = -0.6F * textFontSize;
                else if (sBaselineShift == "super")
                    shiftBy = 0.6F * textFontSize;
                else if (sBaselineShift == "baseline")
                    shiftBy = 0;
                else
                    shiftBy = SvgNumber.ParseNumber(sBaselineShift);
            }

            var nodeType = XmlNodeType.None;

            var isVertical = false;
            var writingMode = _textElement.GetPropertyValue("writing-mode");
            if (!string.IsNullOrEmpty(writingMode) &&
                string.Equals(writingMode, "tb", StringComparison.OrdinalIgnoreCase))
                isVertical = true;

            if (_svgElement.ChildNodes.Count == 1)
            {
                var child = _svgElement.ChildNodes[0];
                nodeType = child.NodeType;
                if (nodeType == XmlNodeType.Text)
                {
                    if (isVertical)
                    {
                        ctp.X -= shiftBy;
                        RenderSingleLineTextV(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.X += shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        RenderSingleLineTextH(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.Y += shiftBy;
                    }
                }
                else if (nodeType == XmlNodeType.Element)
                {
                    var nodeName = child.Name;
                    if (string.Equals(nodeName, "tref"))
                        AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, true);
                    else if (string.Equals(nodeName, "tspan"))
                        AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, true);
                    else if (string.Equals(nodeName, "textPath"))
                        RenderTextPath(_textElement, (SvgTextPathElement)child, ref ctp,
                            rotate, placement);
                }
                else if (nodeType == XmlNodeType.Whitespace)
                {
                    if (isVertical)
                    {
                        ctp.X -= shiftBy;
                        RenderSingleLineTextV(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.X += shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        RenderSingleLineTextH(_textElement, ref ctp,
                            WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                        ctp.Y += shiftBy;
                    }
                }
            }
            else
            {
                var textAnchor = _textElement.GetPropertyValue("text-anchor");

                var anchor = WpfTextAnchor.None;

                if (textAnchor == "middle")
                    anchor = WpfTextAnchor.Middle;
                else if (textAnchor == "end")
                    anchor = WpfTextAnchor.End;

                var nodeList = _svgElement.ChildNodes;
                // This is a very simply hack to change centered text to left align, since for
                // text containing spans, different font weights may be applied to the spans...
                if (anchor == WpfTextAnchor.Middle)
                {
                    // Suspend the rendering...
                    IsMeasuring = true;

                    foreach (XmlNode child in nodeList)
                    {
                        nodeType = child.NodeType;
                        if (nodeType == XmlNodeType.Text)
                        {
                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                RenderTextRunV(_textElement, ref ctp,
                                    WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                RenderTextRunH(_textElement, ref ctp,
                                    WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                                ctp.Y += shiftBy;
                            }
                        }
                        else if (nodeType == XmlNodeType.Element)
                        {
                            var nodeName = child.Name;
                            if (string.Equals(nodeName, "tref"))
                                AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, false);
                            else if (string.Equals(nodeName, "tspan"))
                                AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false);
                            else if (string.Equals(nodeName, "textPath"))
                                RenderTextPath(_textElement, (SvgTextPathElement)child, ref ctp,
                                    rotate, placement);
                        }
                        else if (nodeType == XmlNodeType.Whitespace)
                        {
                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                //RenderTextRunV(_textElement, ref ctp, GetText(_textElement, child));
                                RenderTextRunV(_textElement, ref ctp, Whitespace, rotate, placement);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                //RenderTextRunH(_textElement, ref ctp, GetText(_textElement, child));
                                RenderTextRunH(_textElement, ref ctp, Whitespace, rotate, placement);
                                ctp.Y += shiftBy;
                            }
                        }
                    }

                    ctp.X -= TextWidth / 2d;

                    // Resume the rendering...
                    IsMeasuring = false;
                }

                var textRendered = false;

                for (var i = 0; i < nodeList.Count; i++)
                {
                    var child = nodeList[i];
                    nodeType = child.NodeType;
                    if (nodeType == XmlNodeType.Text)
                    {
                        if (isVertical)
                        {
                            ctp.X -= shiftBy;
                            RenderTextRunV(_textElement, ref ctp,
                                WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                            ctp.X += shiftBy;
                        }
                        else
                        {
                            ctp.Y -= shiftBy;
                            RenderTextRunH(_textElement, ref ctp,
                                WpfTextRenderer.GetText(_textElement, child), rotate, placement);
                            ctp.Y += shiftBy;
                        }

                        textRendered = true;
                    }
                    else if (nodeType == XmlNodeType.Element)
                    {
                        var nodeName = child.Name;
                        if (string.Equals(nodeName, "tref"))
                        {
                            AddTRefElementRun((SvgTRefElement)child, ref ctp, isVertical, false);

                            textRendered = true;
                        }
                        else if (string.Equals(nodeName, "tspan"))
                        {
                            AddTSpanElementRun((SvgTSpanElement)child, ref ctp, isVertical, false);

                            textRendered = true;
                        }
                        else if (string.Equals(nodeName, "textPath"))
                        {
                            RenderTextPath(_textElement, (SvgTextPathElement)child, ref ctp,
                                rotate, placement);

                            textRendered = false;
                        }
                    }
                    else if (nodeType == XmlNodeType.Whitespace)
                    {
                        if (textRendered)
                        {
                            if (isVertical)
                            {
                                ctp.X -= shiftBy;
                                //RenderTextRunV(_textElement, ref ctp, GetText(_textElement, child));
                                RenderTextRunV(_textElement, ref ctp, Whitespace, rotate, placement);
                                ctp.X += shiftBy;
                            }
                            else
                            {
                                ctp.Y -= shiftBy;
                                //RenderTextRunH(_textElement, ref ctp, GetText(_textElement, child));
                                RenderTextRunH(_textElement, ref ctp, Whitespace, rotate, placement);
                                ctp.Y += shiftBy;
                            }

                            textRendered = false;
                        }
                    }
                }
            }
        }

        #endregion

        #region AfterRender Method

        private static void ResetGuidelineSet(DrawingGroup group)
        {
            var drawings = group.Children;
            var itemCount = drawings.Count;
            for (var i = 0; i < itemCount; i++)
            {
                var childGroup = drawings[i] as DrawingGroup;
                if (childGroup != null)
                {
                    childGroup.GuidelineSet = null;

                    ResetGuidelineSet(childGroup);
                }
            }
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_horzRenderer != null)
            {
                _horzRenderer.Uninitialize();
                _horzRenderer = null;
            }

            if (_vertRenderer != null)
            {
                _vertRenderer.Uninitialize();
                _vertRenderer = null;
            }

            if (_pathRenderer != null)
            {
                _pathRenderer.Uninitialize();
                _pathRenderer = null;
            }

            if (_drawContext != null)
            {
                _drawContext.Close();
                _drawContext = null;
            }

            var context = renderer.Context;

            // TODO-PAUL: Testing this for validity...
            // Remove the GuidelineSet from the groups added by the FormattedText to reduced the 
            // size of output XAML...
            if (_drawGroup != null) ResetGuidelineSet(_drawGroup);

            if (context.IncludeRuntime)
                if (_drawGroup != null)
                {
                    // Add the element/object type...
                    SvgObject.SetType(_drawGroup, SvgObjectType.Text);

                    // Add title for tooltips, if any...
                    var titleElement = _svgElement.SelectSingleNode("title") as SvgTitleElement;
                    if (titleElement != null)
                    {
                        var titleValue = titleElement.InnerText;
                        if (!string.IsNullOrEmpty(titleValue)) SvgObject.SetTitle(_drawGroup, titleValue);
                    }
                }

            if (!_isGroupAdded)
            {
                if (_drawGroup != null)
                {
                    if (IsTextPath || _drawGroup.Transform != null || _drawGroup.ClipGeometry != null)
                    {
                        var curGroup = _context.Peek();
                        Debug.Assert(curGroup != null);
                        if (curGroup != null) curGroup.Children.Add(_drawGroup);
                    }
                    else if (_drawGroup.Children.Count != 0)
                    {
                        var firstGroup = _drawGroup.Children[0] as DrawingGroup;
                        if (firstGroup != null && firstGroup.Children.Count != 0)
                        {
                            //Drawing firstDrawing = firstGroup.Children[0];

                            var curGroup = _context.Peek();
                            Debug.Assert(curGroup != null);
                            if (curGroup != null) curGroup.Children.Add(_drawGroup);
                        }
                    }
                }
            }
            else
            {
                if (_drawGroup != null)
                {
                    var currentGroup = context.Peek();

                    if (currentGroup == null || currentGroup != _drawGroup)
                        throw new InvalidOperationException("An existing group is expected.");

                    context.Pop();
                }
            }

            _context = null;
            _drawGroup = null;

            base.AfterRender(renderer);
        }

        #endregion

        public void SetTextWidth(double textWidth)
        {
            TextWidth = textWidth;
        }

        public void AddTextWidth(double textWidth)
        {
            TextWidth += textWidth;
        }

        #endregion

        #region Private Methods

        #region Horizontal Render Methods

        private void RenderSingleLineTextH(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text) || _horzRenderer == null)
                return;

            var targetText = text.Trim();
            if (placement != null) placement.UpdatePositions(targetText);
            _horzRenderer.RenderSingleLineText(element, ref ctp, targetText, rotate, placement);
        }

        private void RenderTextRunH(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text) || _horzRenderer == null)
                return;

            if (placement != null) placement.UpdatePositions(text);
            _horzRenderer.RenderTextRun(element, ref ctp, text, rotate, placement);
        }

        #endregion

        #region Vertical Render Methods

        private void RenderSingleLineTextV(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text) || _vertRenderer == null)
                return;

            var targetText = text.Trim();
            if (placement != null) placement.UpdatePositions(targetText);
            _vertRenderer.RenderSingleLineText(element, ref ctp, targetText, rotate, placement);
        }

        private void RenderTextRunV(SvgTextContentElement element, ref Point ctp,
            string text, double rotate, WpfTextPlacement placement)
        {
            if (string.IsNullOrEmpty(text) || _vertRenderer == null)
                return;

            if (placement != null) placement.UpdatePositions(text);
            _vertRenderer.RenderTextRun(element, ref ctp, text, rotate, placement);
        }

        #endregion

        #region Text Path Methods

        private void RenderTextPath(SvgTextElement element, SvgTextPathElement textPath,
            ref Point ctp, double rotate, WpfTextPlacement placement)
        {
            if (_pathRenderer == null) return;

            _pathRenderer.RenderSingleLineText(textPath, ref ctp, string.Empty, rotate, placement);
        }

        #endregion

        #region TRef/TSpan Methods

        private void AddTRefElementRun(SvgTRefElement element, ref Point ctp,
            bool isVertical, bool isSingleLine)
        {
            var placement = WpfTextRenderer.GetCurrentTextPosition(element, ctp);
            ctp = placement.Location;
            var rotate = placement.Rotation;
            if (!placement.HasPositions) placement = null; // render it useless

            if (isVertical)
            {
                if (isSingleLine)
                    RenderSingleLineTextV(element, ref ctp,
                        WpfTextRenderer.GetTRefText(element), rotate, placement);
                else
                    RenderTextRunV(element, ref ctp,
                        WpfTextRenderer.GetTRefText(element), rotate, placement);
            }
            else
            {
                if (isSingleLine)
                    RenderSingleLineTextH(element, ref ctp,
                        WpfTextRenderer.GetTRefText(element), rotate, placement);
                else
                    RenderTextRunH(element, ref ctp,
                        WpfTextRenderer.GetTRefText(element), rotate, placement);
            }
        }

        private void AddTSpanElementRun(SvgTSpanElement element, ref Point ctp,
            bool isVertical, bool isSingleLine)
        {
            var placement = WpfTextRenderer.GetCurrentTextPosition(element, ctp);
            ctp = placement.Location;
            var rotate = placement.Rotation;
            if (!placement.HasPositions) placement = null; // render it useless

            var sBaselineShift = element.GetPropertyValue("baseline-shift").Trim();
            double shiftBy = 0;

            if (sBaselineShift.Length > 0)
            {
                var textFontSize = WpfTextRenderer.GetComputedFontSize(_textElement);
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
                    if (isVertical)
                    {
                        ctp.X += shiftBy;
                        if (isSingleLine)
                            RenderSingleLineTextV(element, ref ctp,
                                WpfTextRenderer.GetText(element, child), rotate, placement);
                        else
                            RenderTextRunV(element, ref ctp,
                                WpfTextRenderer.GetText(element, child), rotate, placement);
                        ctp.X -= shiftBy;
                    }
                    else
                    {
                        ctp.Y -= shiftBy;
                        if (isSingleLine)
                            RenderSingleLineTextH(element, ref ctp,
                                WpfTextRenderer.GetText(element, child), rotate, placement);
                        else
                            RenderTextRunH(element, ref ctp,
                                WpfTextRenderer.GetText(element, child), rotate, placement);
                        ctp.Y += shiftBy;
                    }
                }
        }

        #endregion

        #endregion
    }
}