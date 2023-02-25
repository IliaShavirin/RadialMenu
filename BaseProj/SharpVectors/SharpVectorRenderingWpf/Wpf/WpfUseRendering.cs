using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfUseRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfUseRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);

            var context = renderer.Context;

            var clipGeom = ClipGeometry;
            var transform = Transform;

            if (transform == null &&
                _svgElement.FirstChild != null && _svgElement.FirstChild == _svgElement.LastChild)
                try
                {
                    var useElement = (SvgUseElement)_svgElement;

                    // If none of the following attribute exists, an exception is thrown...
                    var x = useElement.X.AnimVal.Value;
                    var y = useElement.Y.AnimVal.Value;
                    var width = useElement.Width.AnimVal.Value;
                    var height = useElement.Height.AnimVal.Value;
                    if (width > 0 && height > 0)
                    {
                        var elementBounds = new Rect(x, y, width, height);

                        // Try handling the cases of "symbol" and "svg" sources within the "use"...
                        var childNode = _svgElement.FirstChild;
                        var childName = childNode.Name;
                        if (string.Equals(childName, "symbol", StringComparison.OrdinalIgnoreCase))
                        {
                            var symbolElement = (SvgSymbolElement)childNode;

                            FitToViewbox(context, symbolElement, elementBounds);
                        }
                    }

                    transform = Transform;
                }
                catch
                {
                }

            if (transform != null)
                try
                {
                    var useElement = (SvgUseElement)_svgElement;

                    // If none of the following attribute exists, an exception is thrown...
                    var x = useElement.X.AnimVal.Value;
                    var y = useElement.Y.AnimVal.Value;
                    var width = useElement.Width.AnimVal.Value;
                    var height = useElement.Height.AnimVal.Value;
                    if (width > 0 && height > 0)
                    {
                        var elementBounds = new Rect(x, y, width, height);

                        // Try handling the cases of "symbol" and "svg" sources within the "use"...
                        var childNode = _svgElement.FirstChild;
                        var childName = childNode.Name;
                        if (string.Equals(childName, "symbol", StringComparison.OrdinalIgnoreCase))
                        {
                            var symbolElement = (SvgSymbolElement)childNode;

                            FitToViewbox(context, symbolElement, elementBounds);
                        }
                    }

                    var symbolTransform = Transform;
                    if (symbolTransform != null && !symbolTransform.Value.IsIdentity)
                    {
                        var combinedTransform = new TransformGroup();

                        combinedTransform.Children.Add(transform);
                        combinedTransform.Children.Add(symbolTransform);

                        transform = combinedTransform;
                    }
                }
                catch
                {
                }

            if (clipGeom != null || transform != null)
            {
                _drawGroup = new DrawingGroup();

                var currentGroup = context.Peek();

                if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);

                if (clipGeom != null) _drawGroup.ClipGeometry = clipGeom;

                if (transform != null) _drawGroup.Transform = transform;
            }
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            if (_drawGroup != null)
            {
                var context = renderer.Context;

                var currentGroup = context.Peek();

                if (currentGroup == null || currentGroup != _drawGroup)
                    throw new InvalidOperationException("An existing group is expected.");

                context.Pop();
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}