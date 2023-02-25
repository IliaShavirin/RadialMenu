using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfARendering : WpfRendering
    {
        #region Constructors and Destructor

        public WpfARendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Properties

        public override bool IsRecursive => _isAggregated;

        #endregion

        #region Private Fields

        private bool _isLayer;
        private bool _isAggregated;
        private DrawingGroup _drawGroup;

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            _isAggregated = false;

            if (_svgElement.FirstChild == _svgElement.LastChild)
            {
                var gElement = _svgElement.FirstChild as SvgGElement;
                if (gElement != null)
                {
                    var elementId = gElement.GetAttribute("id");
                    if (!string.IsNullOrEmpty(elementId) &&
                        string.Equals(elementId, "IndicateLayer", StringComparison.OrdinalIgnoreCase))
                    {
                        var context = renderer.Context;

                        var animationGroup = context.Links;
                        if (animationGroup != null) context.Push(animationGroup);

                        _isLayer = true;
                    }
                }
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            _isAggregated = false;

            if (_isLayer)
            {
                base.Render(renderer);

                return;
            }

            var context = renderer.Context;

            var clipGeom = ClipGeometry;
            var transform = Transform;

            float opacityValue = -1;

            var element = (SvgAElement)_svgElement;
            var opacity = element.GetPropertyValue("opacity");
            if (opacity != null && opacity.Length > 0)
            {
                opacityValue = (float)SvgNumber.ParseNumber(opacity);
                opacityValue = Math.Min(opacityValue, 1);
                opacityValue = Math.Max(opacityValue, 0);
            }

            var linkVisitor = context.LinkVisitor;

            if (linkVisitor != null || clipGeom != null || transform != null || opacityValue >= 0)
            {
                _drawGroup = new DrawingGroup();

                var elementId = GetElementName();
                if (!string.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
                {
                    _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);

                    context.RegisterId(elementId);

                    if (context.IncludeRuntime) SvgObject.SetId(_drawGroup, elementId);
                }

                var currentGroup = context.Peek();

                if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");

                if (linkVisitor != null && linkVisitor.Aggregates && context.Links != null)
                {
                    if (!linkVisitor.Exists(elementId)) context.Links.Children.Add(_drawGroup);
                }
                else
                {
                    currentGroup.Children.Add(_drawGroup);
                }

                context.Push(_drawGroup);

                if (clipGeom != null) _drawGroup.ClipGeometry = clipGeom;

                if (transform != null) _drawGroup.Transform = transform;

                if (opacityValue >= 0) _drawGroup.Opacity = opacityValue;

                if (linkVisitor != null)
                {
                    linkVisitor.Visit(_drawGroup, element, context, opacityValue);

                    _isAggregated = linkVisitor.IsAggregate;
                }
            }

            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_isLayer)
            {
                var context = renderer.Context;

                context.Pop();

                base.AfterRender(renderer);

                return;
            }

            if (_drawGroup != null)
            {
                var context = renderer.Context;

                if (context.IncludeRuntime)
                {
                    // Add the element/object type...
                    SvgObject.SetType(_drawGroup, SvgObjectType.Link);

                    // Add title for tooltips, if any...
                    var titleElement = GetTitleElement();
                    if (titleElement != null)
                    {
                        var titleValue = titleElement.InnerText;
                        if (!string.IsNullOrEmpty(titleValue)) SvgObject.SetTitle(_drawGroup, titleValue);
                    }
                }

                var currentGroup = context.Peek();

                if (currentGroup == null || currentGroup != _drawGroup)
                    throw new InvalidOperationException("An existing group is expected.");

                context.Pop();

                // If not aggregated by a link visitor, we remove it from the links/animation and 
                // add it to the main drawing stack...
                if (!_isAggregated)
                    if (context.Links.Children.Remove(_drawGroup))
                    {
                        currentGroup = context.Peek();

                        currentGroup.Children.Add(_drawGroup);
                    }
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}