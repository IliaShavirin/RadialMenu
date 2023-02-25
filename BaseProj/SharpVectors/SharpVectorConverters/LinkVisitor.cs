using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorConverters
{
    public sealed class LinkVisitor : WpfLinkVisitor
    {
        #region Constructors and Destructor

        public LinkVisitor()
        {
            _dicLinks = new Dictionary<string, bool>();
        }

        #endregion

        #region Private Fields

        private bool _isAggregated;
        private GeometryCollection _aggregatedGeom;

        private SvgStyleableElement _aggregatedFill;

        private readonly Dictionary<string, bool> _dicLinks;

        #endregion

        #region Public Properties

        public override bool Aggregates => true;

        public override bool IsAggregate => _isAggregated;

        public override string AggregatedLayerName => SvgObject.LinksLayer;

        #endregion

        #region Public Methods

        public override bool Exists(string linkId)
        {
            if (string.IsNullOrEmpty(linkId)) return false;

            if (_dicLinks != null && _dicLinks.Count != 0) return _dicLinks.ContainsKey(linkId);

            return false;
        }

        public override void Initialize(DrawingGroup linkGroup, WpfDrawingContext context)
        {
            if (linkGroup != null) SvgLink.SetKey(linkGroup, SvgObject.LinksLayer);
        }

        public override void Visit(DrawingGroup group, SvgAElement element,
            WpfDrawingContext context, float opacity)
        {
            _isAggregated = false;

            if (group == null || element == null || context == null) return;

            AddExtraLinkInformation(group, element);

            //string linkId = element.GetAttribute("id");
            var linkId = GetElementName(element);
            if (string.IsNullOrEmpty(linkId)) return;
            SvgLink.SetKey(group, linkId);

            if (_dicLinks.ContainsKey(linkId))
            {
                _isAggregated = _dicLinks[linkId];
                return;
            }

            var linkAction = element.GetAttribute("onclick");
            if (string.IsNullOrEmpty(linkAction))
            {
                linkAction = element.GetAttribute("onmouseover");
                if (!string.IsNullOrEmpty(linkAction) &&
                    linkAction.StartsWith("parent.svgMouseOverName", StringComparison.OrdinalIgnoreCase))
                    SvgLink.SetAction(group, SvgLinkAction.LinkTooltip);
                else
                    SvgLink.SetAction(group, SvgLinkAction.LinkNone);
            }
            else
            {
                if (linkAction.StartsWith("parent.svgClick", StringComparison.OrdinalIgnoreCase))
                    SvgLink.SetAction(group, SvgLinkAction.LinkPage);
                else if (linkAction.StartsWith("parent.svgOpenHtml", StringComparison.OrdinalIgnoreCase))
                    SvgLink.SetAction(group, SvgLinkAction.LinkHtml);
                else
                    SvgLink.SetAction(group, SvgLinkAction.LinkNone);
            }

            if (!string.IsNullOrEmpty(linkAction))
            {
                if (linkAction.IndexOf("'Top'") > 0)
                    SvgLink.SetLocation(group, "Top");
                else if (linkAction.IndexOf("'Bottom'") > 0)
                    SvgLink.SetLocation(group, "Bottom");
                else
                    SvgLink.SetLocation(group, "Top");
            }

            AggregateChildren(element, context, opacity);
            if (_isAggregated)
            {
                Geometry drawGeometry = null;
                if (_aggregatedGeom.Count == 1)
                {
                    drawGeometry = _aggregatedGeom[0];
                }
                else
                {
                    var geomGroup = new GeometryGroup();
                    geomGroup.FillRule = FillRule.Nonzero;

                    for (var i = 0; i < _aggregatedGeom.Count; i++) geomGroup.Children.Add(_aggregatedGeom[i]);

                    drawGeometry = geomGroup;
                }

                var fillPaint = new WpfSvgPaint(context, _aggregatedFill, "fill");
                var brush = fillPaint.GetBrush(false);

                brush.SetValue(FrameworkElement.NameProperty, linkId + "_Brush");

                var drawing = new GeometryDrawing(brush, null, drawGeometry);

                group.Children.Add(drawing);
            }

            _dicLinks.Add(linkId, _isAggregated);
        }

        public static string GetElementName(SvgElement element)
        {
            if (element == null) return string.Empty;
            var elementId = element.Id;
            if (elementId != null) elementId = elementId.Trim();
            if (string.IsNullOrEmpty(elementId)) return string.Empty;
            elementId = elementId.Replace(':', '_');
            elementId = elementId.Replace(" ", string.Empty);
            elementId = elementId.Replace('.', '_');
            elementId = elementId.Replace('-', '_');

            return elementId;
        }

        #endregion

        #region Private Methods

        private void AddExtraLinkInformation(DrawingGroup group, SvgElement element)
        {
            var linkColor = element.GetAttribute("color");
            if (!string.IsNullOrEmpty(linkColor)) SvgLink.SetColor(group, linkColor);
            var linkPartsId = element.GetAttribute("partsid");
            if (!string.IsNullOrEmpty(linkPartsId)) SvgLink.SetPartsId(group, linkPartsId);
            var linkType = element.GetAttribute("type");
            if (!string.IsNullOrEmpty(linkType)) SvgLink.SetPartsId(group, linkType);
            var linkNumber = element.GetAttribute("num");
            if (!string.IsNullOrEmpty(linkNumber)) SvgLink.SetPartsId(group, linkNumber);
            var linkPin = element.GetAttribute("pin");
            if (!string.IsNullOrEmpty(linkPin)) SvgLink.SetPartsId(group, linkPin);
            var linkLineId = element.GetAttribute("lineid");
            if (!string.IsNullOrEmpty(linkLineId)) SvgLink.SetPartsId(group, linkLineId);
        }

        private void AggregateChildren(SvgAElement aElement, WpfDrawingContext context, float opacity)
        {
            _isAggregated = false;

            if (aElement == null || aElement.ChildNodes == null) return;

            var aggregatedFill = aElement.GetAttribute("fill");
            var isFillFound = !string.IsNullOrEmpty(aggregatedFill);

            SvgStyleableElement paintElement = null;
            if (isFillFound) paintElement = aElement;

            XmlNode targetNode = aElement;
            // Check if the children of the link are wrapped in a Group Element...
            if (aElement.ChildNodes.Count == 1)
            {
                var groupElement = aElement.ChildNodes[0] as SvgGElement;
                if (groupElement != null) targetNode = groupElement;
            }

            var settings = context.Settings;

            var geomColl = new GeometryCollection();

            foreach (XmlNode node in targetNode.ChildNodes)
            {
                if (node.NodeType != XmlNodeType.Element) continue;

                // Handle a case where the clip element has "use" element as a child...
                if (string.Equals(node.LocalName, "use"))
                {
                    var useElement = (SvgUseElement)node;

                    var refEl = useElement.ReferencedElement;
                    if (refEl != null)
                    {
                        var refElParent = (XmlElement)refEl.ParentNode;
                        useElement.OwnerDocument.Static = true;
                        useElement.CopyToReferencedElement(refEl);
                        refElParent.RemoveChild(refEl);
                        useElement.AppendChild(refEl);

                        foreach (XmlNode useChild in useElement.ChildNodes)
                        {
                            if (useChild.NodeType != XmlNodeType.Element) continue;

                            var element = useChild as SvgStyleableElement;
                            if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                            {
                                var childPath = WpfRendering.CreateGeometry(element,
                                    settings.OptimizePath);

                                if (childPath != null)
                                {
                                    if (isFillFound)
                                    {
                                        var elementFill = element.GetAttribute("fill");
                                        if (!string.IsNullOrEmpty(elementFill) &&
                                            !string.Equals(elementFill, aggregatedFill,
                                                StringComparison.OrdinalIgnoreCase))
                                            return;
                                    }
                                    else
                                    {
                                        aggregatedFill = element.GetAttribute("fill");
                                        isFillFound = !string.IsNullOrEmpty(aggregatedFill);
                                        if (isFillFound) paintElement = element;
                                    }

                                    geomColl.Add(childPath);
                                }
                            }
                        }

                        useElement.RemoveChild(refEl);
                        useElement.RestoreReferencedElement(refEl);
                        refElParent.AppendChild(refEl);
                        useElement.OwnerDocument.Static = false;
                    }
                }
                //else if (String.Equals(node.LocalName, "g"))
                //{   
                //}
                else
                {
                    var element = node as SvgStyleableElement;
                    if (element != null && element.RenderingHint == SvgRenderingHint.Shape)
                    {
                        var childPath = WpfRendering.CreateGeometry(element,
                            settings.OptimizePath);

                        if (childPath != null)
                        {
                            if (isFillFound)
                            {
                                var elementFill = element.GetAttribute("fill");
                                if (!string.IsNullOrEmpty(elementFill) &&
                                    !string.Equals(elementFill, aggregatedFill, StringComparison.OrdinalIgnoreCase))
                                    return;
                            }
                            else
                            {
                                aggregatedFill = element.GetAttribute("fill");
                                isFillFound = !string.IsNullOrEmpty(aggregatedFill);
                                if (isFillFound) paintElement = element;
                            }

                            geomColl.Add(childPath);
                        }
                    }
                }
            }

            if (geomColl.Count == 0 || paintElement == null) return;

            _aggregatedFill = paintElement;
            _aggregatedGeom = geomColl;

            _isAggregated = true;
        }

        #endregion
    }
}