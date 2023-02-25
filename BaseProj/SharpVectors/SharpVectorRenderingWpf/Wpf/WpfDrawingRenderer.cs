using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfDrawingRenderer : DependencyObject,
        ISvgRenderer, IDisposable
    {
        #region Private Fields

        private readonly WpfDrawingSettings _renderingSettings;

        private readonly WpfRenderingHelper _svgRenderer;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingRenderer()
        {
            _svgRenderer = new WpfRenderingHelper(this);
        }

        public WpfDrawingRenderer(WpfDrawingSettings settings)
        {
            _svgRenderer = new WpfRenderingHelper(this);
            _renderingSettings = settings;
        }

        ~WpfDrawingRenderer()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public Drawing Drawing
        {
            get
            {
                if (Context == null) return null;

                return Context.Root;
            }
        }

        public WpfDrawingContext Context { get; private set; }

        public WpfLinkVisitor LinkVisitor { get; set; }

        public WpfEmbeddedImageVisitor ImageVisitor { get; set; }

        public WpfFontFamilyVisitor FontFamilyVisitor { get; set; }

        #endregion

        #region ISvgRenderer Members

        /// <summary>
        ///     The renderer's <see cref="SvgWindow">SvgWindow</see> object.
        /// </summary>
        public ISvgWindow Window { get; set; }

        public void Render(ISvgElement node)
        {
            //throw new NotImplementedException();

            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //if (graphics != null && graphics.Graphics != null)
            //{
            //    _svgRenderer.Render(node);
            //}

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);

            Context = new WpfDrawingContext(true,
                _renderingSettings);

            Context.Initialize(null, FontFamilyVisitor, ImageVisitor);

            Context.BeginDrawing();

            _svgRenderer.Render(node);

            Context.EndDrawing();
        }

        public void Render(ISvgElement node, WpfDrawingContext context)
        {
            //throw new NotImplementedException();

            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //if (graphics != null && graphics.Graphics != null)
            //{
            //    _svgRenderer.Render(node);
            //}

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);

            if (context == null)
            {
                Context = new WpfDrawingContext(true,
                    _renderingSettings);

                Context.Initialize(null, FontFamilyVisitor, ImageVisitor);
            }
            else
            {
                Context = context;
            }

            Context.BeginDrawing();

            _svgRenderer.Render(node);

            Context.EndDrawing();
        }

        public void Render(ISvgDocument node)
        {
            //SvgRectF updatedRect;
            //if (invalidRect != SvgRectF.Empty)
            //    updatedRect = new SvgRectF(invalidRect.X, invalidRect.Y,
            //        invalidRect.Width, invalidRect.Height);
            //else
            //    updatedRect = SvgRectF.Empty;

            //RendererBeforeRender();

            //_renderingContext = new WpfDrawingContext(new DrawingGroup());
            Context = new WpfDrawingContext(false,
                _renderingSettings);

            Context.Initialize(LinkVisitor, FontFamilyVisitor, ImageVisitor);

            Context.BeginDrawing();

            _svgRenderer.Render(node);

            Context.EndDrawing();

            //RendererAfterRender();

            //if (onRender != null)
            //    OnRender(updatedRect);
        }

        public SvgRectF InvalidRect
        {
            get => SvgRectF.Empty;
            set { }
        }

        public void RenderChildren(ISvgElement node)
        {
            _svgRenderer.RenderChildren(node);
        }

        public void RenderMask(ISvgElement node, WpfDrawingContext context)
        {
            if (context == null)
            {
                Context = new WpfDrawingContext(true,
                    _renderingSettings);

                Context.Initialize(null, FontFamilyVisitor, ImageVisitor);
            }
            else
            {
                Context = context;
            }

            Context.BeginDrawing();

            _svgRenderer.RenderMask(node);

            Context.EndDrawing();
        }

        public void InvalidateRect(SvgRectF rect)
        {
        }

        public RenderEvent OnRender
        {
            get => null;
            set { }
        }

        public ISvgRect GetRenderedBounds(ISvgElement element, float margin)
        {
            return SvgRect.Empty;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
        }

        #endregion
    }
}