using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Utils;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorRuntime;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfPathRendering : WpfRendering
    {
        #region Constructors and Destructor

        public WpfPathRendering(SvgElement element)
            : base(element)
        {
        }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null) return;

            var context = renderer.Context;

            SetQuality(context);
            SetTransform(context);
            SetMask(context);
        }

        public override void Render(WpfDrawingRenderer renderer)
        {
            var context = renderer.Context;

            var hint = _svgElement.RenderingHint;
            if (hint != SvgRenderingHint.Shape || hint == SvgRenderingHint.Clipping) return;
            // We do not directly render the contents of the clip-path, unless specifically requested...
            if (string.Equals(_svgElement.ParentNode.LocalName, "clipPath") &&
                !context.RenderingClipRegion)
                return;

            var styleElm = (SvgStyleableElement)_svgElement;

            var sVisibility = styleElm.GetPropertyValue("visibility");
            var sDisplay = styleElm.GetPropertyValue("display");
            if (string.Equals(sVisibility, "hidden") || string.Equals(sDisplay, "none")) return;

            var drawGroup = context.Peek();
            Debug.Assert(drawGroup != null);

            var geometry = CreateGeometry(_svgElement, context.OptimizePath);

            if (geometry != null && !geometry.IsEmpty())
            {
                SetClip(context);

                var fillPaint = new WpfSvgPaint(context, styleElm, "fill");

                var fileValue = styleElm.GetAttribute("fill");

                var brush = fillPaint.GetBrush(geometry);

                var strokePaint = new WpfSvgPaint(context, styleElm, "stroke");
                var pen = strokePaint.GetPen();

                if (brush != null || pen != null)
                {
                    var transform = Transform;
                    if (transform != null && !transform.Value.IsIdentity)
                    {
                        geometry.Transform = transform;
                        if (brush != null)
                        {
                            var brushTransform = brush.Transform;
                            if (brushTransform == null || brushTransform == Transform.Identity)
                            {
                                brush.Transform = transform;
                            }
                            else
                            {
                                var groupTransform = new TransformGroup();
                                groupTransform.Children.Add(brushTransform);
                                groupTransform.Children.Add(transform);
                                brush.Transform = groupTransform;
                            }
                        }
                    }
                    else
                    {
                        transform = null; // render any identity transform useless...
                    }

                    var drawing = new GeometryDrawing(brush, pen, geometry);

                    var elementId = GetElementName();
                    if (!string.IsNullOrEmpty(elementId) && !context.IsRegisteredId(elementId))
                    {
                        drawing.SetValue(FrameworkElement.NameProperty, elementId);

                        context.RegisterId(elementId);

                        if (context.IncludeRuntime) SvgObject.SetId(drawing, elementId);
                    }

                    var maskBrush = Masking;
                    var clipGeom = ClipGeometry;
                    if (clipGeom != null || maskBrush != null)
                    {
                        //Geometry clipped = Geometry.Combine(geometry, clipGeom,
                        //    GeometryCombineMode.Exclude, null);

                        //if (clipped != null && !clipped.IsEmpty())
                        //{
                        //    geometry = clipped;
                        //}
                        var clipMaskGroup = new DrawingGroup();

                        var geometryBounds = geometry.Bounds;

                        if (clipGeom != null)
                        {
                            clipMaskGroup.ClipGeometry = clipGeom;

                            var clipUnits = ClipUnits;
                            if (clipUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                var drawingBounds = geometryBounds;

                                if (transform != null) drawingBounds = transform.TransformBounds(drawingBounds);

                                var transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                transformGroup.Children.Add(
                                    new ScaleTransform(drawingBounds.Width, drawingBounds.Height));
                                transformGroup.Children.Add(
                                    new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                                clipGeom.Transform = transformGroup;
                            }
                            else
                            {
                                if (transform != null) clipGeom.Transform = transform;
                            }
                        }

                        if (maskBrush != null)
                        {
                            var maskUnits = MaskUnits;
                            if (maskUnits == SvgUnitType.ObjectBoundingBox)
                            {
                                var drawingBounds = geometryBounds;

                                if (transform != null) drawingBounds = transform.TransformBounds(drawingBounds);

                                var transformGroup = new TransformGroup();

                                // Scale the clip region (at (0, 0)) and translate to the top-left corner of the target.
                                transformGroup.Children.Add(
                                    new ScaleTransform(drawingBounds.Width, drawingBounds.Height));
                                transformGroup.Children.Add(
                                    new TranslateTransform(drawingBounds.X, drawingBounds.Y));

                                var maskGroup = ((DrawingBrush)maskBrush).Drawing as DrawingGroup;
                                if (maskGroup != null)
                                {
                                    var maskDrawings = maskGroup.Children;
                                    for (var i = 0; i < maskDrawings.Count; i++)
                                    {
                                        var maskDrawing = maskDrawings[i];
                                        var maskGeomDraw = maskDrawing as GeometryDrawing;
                                        if (maskGeomDraw != null)
                                        {
                                            if (maskGeomDraw.Brush != null) ConvertColors(maskGeomDraw.Brush);
                                            if (maskGeomDraw.Pen != null) ConvertColors(maskGeomDraw.Pen.Brush);
                                        }
                                    }
                                }

                                //if (transformGroup != null)
                                //{
                                //    drawingBounds = transformGroup.TransformBounds(drawingBounds);
                                //}

                                //maskBrush.Viewbox = drawingBounds;
                                //maskBrush.ViewboxUnits = BrushMappingMode.Absolute;

                                //maskBrush.Stretch = Stretch.Uniform;

                                //maskBrush.Viewport = drawingBounds;
                                //maskBrush.ViewportUnits = BrushMappingMode.Absolute;

                                maskBrush.Transform = transformGroup;
                            }
                            else
                            {
                                if (transform != null) maskBrush.Transform = transform;
                            }

                            clipMaskGroup.OpacityMask = maskBrush;
                        }

                        clipMaskGroup.Children.Add(drawing);
                        drawGroup.Children.Add(clipMaskGroup);
                    }
                    else
                    {
                        drawGroup.Children.Add(drawing);
                    }
                }
            }

            RenderMarkers(renderer, styleElm, context);
        }

        //==========================================================================
        private static float AlphaComposition(Color color)
        {
            var max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
            var min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

            return (min + max) / 2.0f;
        }

        //==========================================================================
        private static float AlphaComposition(Brush brush)
        {
            var alphaValue = 1.0f;

            if (brush != null)
            {
                if (brush is SolidColorBrush)
                {
                    var nextValue = AlphaComposition((brush as SolidColorBrush).Color);
                    if (nextValue > 0 && nextValue < 1) alphaValue = nextValue;
                }
                else if (brush is GradientBrush)
                {
                    foreach (var gradient_stop in (brush as GradientBrush).GradientStops)
                    {
                        var nextValue = AlphaComposition(gradient_stop.Color);
                        if (nextValue > 0 && nextValue < 1) alphaValue = nextValue;
                    }
                }
                //else if (brush is DrawingBrush)
                //{
                //    ConvertColors((brush as DrawingBrush).Drawing);
                //}
                else
                {
                    throw new NotSupportedException();
                }
            }

            return alphaValue;
        }

        //==========================================================================
        private static Color ConvertColor(Color color)
        {
            var max = Math.Max(Math.Max(color.ScR, color.ScG), color.ScB);
            var min = Math.Min(Math.Min(color.ScR, color.ScG), color.ScB);

            return Color.FromScRgb((min + max) / 2.0f, color.ScR, color.ScG, color.ScB);
        }

        //==========================================================================
        private static void ConvertColors(Brush brush)
        {
            if (brush != null)
            {
                SolidColorBrush solidBrush = null;
                GradientBrush gradientBrush = null;

                if (DynamicCast.Cast(brush, out solidBrush))
                {
                    solidBrush.Color = ConvertColor(solidBrush.Color);
                }
                else if (DynamicCast.Cast(brush, out gradientBrush))
                {
                    var stopColl = gradientBrush.GradientStops;

                    foreach (var stop in stopColl) stop.Color = ConvertColor(stop.Color);
                }
                //else if (brush is DrawingBrush)
                //{
                //    ConvertColors((brush as DrawingBrush).Drawing);
                //}
                else
                {
                    throw new NotSupportedException();
                }
            }
        }

        #endregion
    }
}