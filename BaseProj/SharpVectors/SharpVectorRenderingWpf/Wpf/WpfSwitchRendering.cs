using System;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfSwitchRendering : WpfRendering
    {
        #region Private Fields

        private DrawingGroup _drawGroup;

        #endregion

        #region Constructors and Destructor

        public WpfSwitchRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            base.BeforeRender(renderer);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            var clipGeom = ClipGeometry;
            var transform = Transform;

            if (clipGeom != null || transform != null)
            {
                var context = renderer.Context;
                _drawGroup = new DrawingGroup();

                var currentGroup = context.Peek();

                if (currentGroup == null) throw new InvalidOperationException("An existing group is expected.");

                currentGroup.Children.Add(_drawGroup);
                context.Push(_drawGroup);

                if (clipGeom != null) _drawGroup.ClipGeometry = clipGeom;

                if (transform != null) _drawGroup.Transform = transform;
            }

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