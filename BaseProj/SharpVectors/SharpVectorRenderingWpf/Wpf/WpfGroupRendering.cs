using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfGroupRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfGroupRendering(SvgElement element)
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

            var elementId = GetElementName();
            if (!string.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
            {
                _drawGroup.SetValue(FrameworkElement.NameProperty, elementId);

                context.RegisterId(elementId);

                if (context.IncludeRuntime) SvgObject.SetId(_drawGroup, elementId);
            }

            var currentGroup = context.Peek();

            if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");

            currentGroup.Children.Add(_drawGroup);
            context.Push(_drawGroup);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            if (_drawGroup != null)
            {
                var clipGeom = ClipGeometry;
                if (clipGeom != null) _drawGroup.ClipGeometry = clipGeom;

                var transform = Transform;
                if (transform != null) _drawGroup.Transform = transform;

                float opacityValue = -1;

                var element = (SvgGElement)_svgElement;
                var opacity = element.GetAttribute("opacity");
                if (opacity != null && opacity.Length > 0)
                {
                    opacityValue = (float)SvgNumber.ParseNumber(opacity);
                    opacityValue = Math.Min(opacityValue, 1);
                    opacityValue = Math.Max(opacityValue, 0);
                }

                if (opacityValue >= 0) _drawGroup.Opacity = opacityValue;
            }

            base.Render(renderer);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            Debug.Assert(_drawGroup != null);

            var context = renderer.Context;

            var currentGroup = context.Peek();

            if (currentGroup == null || currentGroup != _drawGroup)
                throw new InvalidOperationException("An existing group is expected.");

            // Remove the added group from the stack...
            context.Pop();

            // If the group is empty, we simply remove it...
            if (_drawGroup.Children.Count == 0 && _drawGroup.ClipGeometry == null &&
                _drawGroup.Transform == null)
            {
                currentGroup = context.Peek();
                if (currentGroup != null) currentGroup.Children.Remove(_drawGroup);
            }

            base.AfterRender(renderer);
        }

        #endregion
    }
}