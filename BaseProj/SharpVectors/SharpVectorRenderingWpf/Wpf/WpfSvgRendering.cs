using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfSvgRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfSvgRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            var context = renderer.Context;
            _drawGroup = new DrawingGroup();

            if (context.Count == 0)
            {
                context.Push(_drawGroup);
                context.Root = _drawGroup;
            }
            else if (context.Count == 1)
            {
                var currentGroup = context.Peek();

                if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");
                if (currentGroup == context.Root && !context.IsFragment)
                {
                    _drawGroup.SetValue(FrameworkElement.NameProperty, SvgObject.DrawLayer);
                    if (context.IncludeRuntime) SvgLink.SetKey(_drawGroup, SvgObject.DrawLayer);
                }

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);
            }
            else
            {
                var currentGroup = context.Peek();

                if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);
            }

            var svgElm = (SvgSvgElement)_svgElement;

            var x = Math.Round(svgElm.X.AnimVal.Value, 4);
            var y = Math.Round(svgElm.Y.AnimVal.Value, 4);
            var width = Math.Round(svgElm.Width.AnimVal.Value, 4);
            var height = Math.Round(svgElm.Height.AnimVal.Value, 4);

            var elmRect = new Rect(x, y, width, height);

            //if (element.ParentNode is SvgElement)
            //{
            //    // TODO: should it be moved with x and y?
            //}

            var parentNode = _svgElement.ParentNode;

            //if (parentNode.NodeType == XmlNodeType.Document)
            {
                ISvgFitToViewBox fitToView = svgElm;
                if (fitToView != null)
                {
                    var animRect = fitToView.ViewBox;
                    if (animRect != null)
                    {
                        var viewRect = animRect.AnimVal;
                        if (viewRect != null)
                        {
                            var wpfViewRect = WpfConvert.ToRect(viewRect);
                            if (!wpfViewRect.IsEmpty && wpfViewRect.Width > 0 && wpfViewRect.Height > 0)
                                elmRect = wpfViewRect;
                        }
                    }
                }
            }

            Transform transform = null;
            if (parentNode.NodeType != XmlNodeType.Document)
            {
                FitToViewbox(context, elmRect);

                transform = Transform;
                if (transform != null) _drawGroup.Transform = transform;
            }

            //if (height > 0 && width > 0)
            //{
            //    ClipGeometry = new RectangleGeometry(elmRect);
            //}
            //Geometry clipGeom = this.ClipGeometry;
            //if (clipGeom != null)
            //{
            //    _drawGroup.ClipGeometry = clipGeom;
            //}

            if ((float)elmRect.Width != 0 && (float)elmRect.Height != 0)
            {
                // Elements such as "pattern" are also rendered by this renderer, so we make sure we are
                // dealing with the root SVG element...
                if (parentNode != null && parentNode.NodeType == XmlNodeType.Document)
                {
                    _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                }
                else
                {
                    if (transform != null)
                    {
                        // We have already applied the transform, which will translate to the start point...
                        if (transform is TranslateTransform)
                            _drawGroup.ClipGeometry = new RectangleGeometry(
                                new Rect(0, 0, elmRect.Width, elmRect.Height));
                        else
                            _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                    }
                    else
                    {
                        _drawGroup.ClipGeometry = new RectangleGeometry(elmRect);
                    }
                }

                //DrawingGroup animationGroup = context.Links;
                //if (animationGroup != null)
                //{
                //    animationGroup.ClipGeometry = clipGeom;
                //}
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            base.AfterRender(renderer);

            Debug.Assert(_drawGroup != null);

            var context = renderer.Context;

            var currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
                throw new InvalidOperationException("An existing group is expected.");

            context.Pop();
        }

        #endregion
    }
}