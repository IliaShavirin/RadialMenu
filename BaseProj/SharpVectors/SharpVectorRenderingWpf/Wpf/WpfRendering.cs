using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Css;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.ClippingMaskingCompositing;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Painting;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;
using BaseProj.SharpVectors.SharpVectorCore.Utils;
using BaseProj.SharpVectors.SharpVectorCss.Css;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.ClippingMaskingCompositing;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Paths;
using BaseProj.SharpVectors.SharpVectorModel.Shapes;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public class WpfRendering : WpfRenderingBase
    {
        #region Private Fields

        private static readonly Regex _reUrl = new Regex(@"^url\((?<uri>.+)\)$", RegexOptions.Compiled);

        #endregion

        #region Constructor and Destructor

        public WpfRendering(SvgElement element)
            : base(element)
        {
            MaskUnits = SvgUnitType.UserSpaceOnUse;
            ClipUnits = SvgUnitType.UserSpaceOnUse;
            MaskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region Public Properties

        public Transform Transform { get; set; }

        public Geometry ClipGeometry { get; set; }

        public SvgUnitType ClipUnits { get; private set; }

        public Brush Masking { get; set; }

        public SvgUnitType MaskUnits { get; private set; }

        public SvgUnitType MaskContentUnits { get; private set; }

        #endregion

        #region Public Methods

        public override void BeforeRender(WpfDrawingRenderer renderer)
        {
            if (renderer == null) return;

            MaskUnits = SvgUnitType.UserSpaceOnUse;
            ClipUnits = SvgUnitType.UserSpaceOnUse;
            MaskContentUnits = SvgUnitType.UserSpaceOnUse;

            var context = renderer.Context;

            SetQuality(context);
            SetTransform(context);
            SetClip(context);
            SetMask(context);
        }

        public override void AfterRender(WpfDrawingRenderer renderer)
        {
            ClipGeometry = null;
            Transform = null;
            Masking = null;

            MaskUnits = SvgUnitType.UserSpaceOnUse;
            ClipUnits = SvgUnitType.UserSpaceOnUse;
            MaskContentUnits = SvgUnitType.UserSpaceOnUse;
        }

        #endregion

        #region Public Static Methods

        public static WpfRendering Create(ISvgElement element)
        {
            if (element == null) return null;

            var hint = element.RenderingHint;
            // For the shapes and text contents...
            if (hint == SvgRenderingHint.Shape) return new WpfPathRendering((SvgElement)element);
            if (hint == SvgRenderingHint.Text) return new WpfTextRendering((SvgElement)element);

            var localName = element.LocalName;
            if (string.IsNullOrEmpty(localName)) return new WpfRendering((SvgElement)element);

            switch (localName)
            {
                case "svg":
                    return new WpfSvgRendering((SvgElement)element);
                case "g":
                    return new WpfGroupRendering((SvgElement)element);
                case "a":
                    return new WpfARendering((SvgElement)element);
                case "use":
                    return new WpfUseRendering((SvgElement)element);
                case "switch":
                    return new WpfSwitchRendering((SvgElement)element);
                case "image":
                    return new WpfImageRendering((SvgElement)element);
                case "marker":
                    return new WpfMarkerRendering((SvgElement)element);
            }

            return new WpfRendering((SvgElement)element);
        }

        /// <summary>
        ///     Generates a new <see cref="RenderingNode">RenderingNode</see> that
        ///     corresponds to the given Uri.
        /// </summary>
        /// <param name="baseUri">
        ///     The base Uri.
        /// </param>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <returns>
        ///     The generated <see cref="RenderingNode">RenderingNode</see> that
        ///     corresponds to the given Uri.
        /// </returns>
        public static WpfRendering CreateByUri(SvgDocument document, string baseUri, string url)
        {
            if (url.StartsWith("#"))
            {
                // do nothing
            }
            else if (baseUri != "")
            {
                var absoluteUri = new Uri(new Uri(baseUri), url);
                url = absoluteUri.AbsoluteUri;
            }

            // TODO: Handle xml:base here?        
            // Right now just skip this... it can't be resolved, must assume it is absolute
            var elm = document.GetNodeByUri(url) as ISvgElement;

            if (elm != null)
                return Create(elm);
            return null;
        }

        #endregion

        #region Protected Methods

        protected SvgTitleElement GetTitleElement()
        {
            if (_svgElement == null) return null;

            SvgTitleElement titleElement = null;
            foreach (XmlNode node in _svgElement.ChildNodes)
                if (node.NodeType == XmlNodeType.Element &&
                    string.Equals(node.LocalName, "title", StringComparison.OrdinalIgnoreCase))
                {
                    titleElement = node as SvgTitleElement;
                    break;
                }

            return titleElement;
        }

        protected void SetClip(WpfDrawingContext context)
        {
            ClipUnits = SvgUnitType.UserSpaceOnUse;

            if (_svgElement == null) return;

            #region Clip with clip

            // see http://www.w3.org/TR/SVG/masking.html#OverflowAndClipProperties 
            if (_svgElement is ISvgSvgElement || _svgElement is ISvgMarkerElement ||
                _svgElement is ISvgSymbolElement || _svgElement is ISvgPatternElement)
            {
                // check overflow property
                var overflow = _svgElement.GetComputedCssValue("overflow", string.Empty) as CssValue;
                // TODO: clip can have "rect(10 10 auto 10)"
                var clip = _svgElement.GetComputedCssValue("clip", string.Empty) as CssPrimitiveValue;

                string sOverflow = null;

                if (overflow != null || overflow.CssText == "")
                {
                    sOverflow = overflow.CssText;
                }
                else
                {
                    if (this is ISvgSvgElement)
                        sOverflow = "hidden";
                }

                if (sOverflow != null)
                    // "If the 'overflow' property has a value other than hidden or scroll, the property has no effect (i.e., a clipping rectangle is not created)."
                    if (sOverflow == "hidden" || sOverflow == "scroll")
                    {
                        var clipRect = Rect.Empty;
                        if (clip != null && clip.PrimitiveType == CssPrimitiveType.Rect)
                        {
                            if (_svgElement is ISvgSvgElement)
                            {
                                var svgElement = (ISvgSvgElement)_svgElement;
                                var viewPort = svgElement.Viewport as SvgRect;
                                clipRect = WpfConvert.ToRect(viewPort);
                                ICssRect clipShape = (CssRect)clip.GetRectValue();
                                if (clipShape.Top.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Y += clipShape.Top.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Left.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.X += clipShape.Left.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Right.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Width = clipRect.Right - clipRect.X -
                                                     clipShape.Right.GetFloatValue(CssPrimitiveType.Number);
                                if (clipShape.Bottom.PrimitiveType != CssPrimitiveType.Ident)
                                    clipRect.Height = clipRect.Bottom - clipRect.Y -
                                                      clipShape.Bottom.GetFloatValue(CssPrimitiveType.Number);
                            }
                        }
                        else if (clip == null || (clip.PrimitiveType == CssPrimitiveType.Ident &&
                                                  clip.GetStringValue() == "auto"))
                        {
                            if (_svgElement is ISvgSvgElement)
                            {
                                var svgElement = (ISvgSvgElement)_svgElement;
                                var viewPort = svgElement.Viewport as SvgRect;
                                clipRect = WpfConvert.ToRect(viewPort);
                            }
                            else if (_svgElement is ISvgMarkerElement ||
                                     _svgElement is ISvgSymbolElement ||
                                     _svgElement is ISvgPatternElement)
                            {
                                // TODO: what to do here?
                            }
                        }

                        if (clipRect != Rect.Empty) ClipGeometry = new RectangleGeometry(clipRect);
                        //gr.SetClip(clipRect);
                    }
            }

            #endregion

            #region Clip with clip-path

            var hint = _svgElement.RenderingHint;

            if (hint == SvgRenderingHint.Image)
            {
            }

            // see: http://www.w3.org/TR/SVG/masking.html#EstablishingANewClippingPath

            if (hint == SvgRenderingHint.Shape || hint == SvgRenderingHint.Text ||
                hint == SvgRenderingHint.Clipping || hint == SvgRenderingHint.Masking ||
                hint == SvgRenderingHint.Containment || hint == SvgRenderingHint.Image)
            {
                var clipPath = _svgElement.GetComputedCssValue("clip-path", string.Empty) as CssPrimitiveValue;

                if (clipPath != null && clipPath.PrimitiveType == CssPrimitiveType.Uri)
                {
                    var absoluteUri = _svgElement.ResolveUri(clipPath.GetStringValue());

                    var eClipPath = _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgClipPathElement;

                    if (eClipPath != null)
                    {
                        var geomColl = CreateClippingRegion(eClipPath, context);
                        if (geomColl == null || geomColl.Count == 0) return;
                        var gpClip = geomColl[0];
                        var geomCount = geomColl.Count;
                        if (geomCount > 1)
                            //GeometryGroup clipGroup = new GeometryGroup();
                            //clipGroup.Children.Add(gpClip);
                            for (var k = 1; k < geomCount; k++)
                                gpClip = Geometry.Combine(gpClip, geomColl[k],
                                    GeometryCombineMode.Union, null);
                        //clipGroup.Children.Add(geomColl[k]);
                        //clipGroup.Children.Reverse();
                        //gpClip = clipGroup;
                        if (gpClip == null || gpClip.IsEmpty()) return;

                        ClipUnits = (SvgUnitType)eClipPath.ClipPathUnits.AnimVal;

                        //if (_clipPathUnits == SvgUnitType.ObjectBoundingBox)
                        //{
                        //    SvgTransformableElement transElement = _svgElement as SvgTransformableElement;

                        //    if (transElement != null)
                        //    {
                        //        ISvgRect bbox = transElement.GetBBox();

                        //        // scale clipping path
                        //        gpClip.Transform = new ScaleTransform(bbox.Width, bbox.Height);
                        //        //gr.SetClip(gpClip);

                        //        // offset clip
                        //        //TODO--PAUL gr.TranslateClip((float)bbox.X, (float)bbox.Y);

                        //        _clipGeometry = gpClip;
                        //    }
                        //    else
                        //    {
                        //        throw new NotImplementedException("clip-path with SvgUnitType.ObjectBoundingBox "
                        //          + "not supported for this type of element: " + _svgElement.GetType());
                        //    }
                        //}
                        //else
                        {
                            //gr.SetClip(gpClip);

                            ClipGeometry = gpClip;
                        }
                    }
                }
            }

            #endregion
        }

        protected void SetMask(WpfDrawingContext context)
        {
            MaskUnits = SvgUnitType.UserSpaceOnUse;
            MaskContentUnits = SvgUnitType.UserSpaceOnUse;

            var maskPath = _svgElement.GetComputedCssValue(
                "mask", string.Empty) as CssPrimitiveValue;

            if (maskPath != null && maskPath.PrimitiveType == CssPrimitiveType.Uri)
            {
                var absoluteUri = _svgElement.ResolveUri(maskPath.GetStringValue());

                var maskElement =
                    _svgElement.OwnerDocument.GetNodeByUri(absoluteUri) as SvgMaskElement;

                if (maskElement != null)
                {
                    var renderer = new WpfDrawingRenderer();
                    renderer.Window = _svgElement.OwnerDocument.Window as SvgWindow;

                    var settings = context.Settings.Clone();
                    settings.TextAsGeometry = true;
                    var maskContext = new WpfDrawingContext(true,
                        settings);

                    //maskContext.Initialize(null, context.FontFamilyVisitor, null);
                    maskContext.Initialize(context.LinkVisitor,
                        context.FontFamilyVisitor, context.ImageVisitor);

                    renderer.RenderMask(maskElement, maskContext);
                    var image = renderer.Drawing;

                    var bounds = new Rect(0, 0, 1, 1);
                    //Rect destRect = GetMaskDestRect(maskElement, bounds);

                    //destRect = bounds;

                    //DrawingImage drawImage = new DrawingImage(image);

                    //DrawingVisual drawingVisual = new DrawingVisual();
                    //DrawingContext drawingContext = drawingVisual.RenderOpen();
                    //drawingContext.DrawDrawing(image);
                    //drawingContext.Close();

                    //RenderTargetBitmap drawImage = new RenderTargetBitmap((int)200,
                    //    (int)200, 96, 96, PixelFormats.Pbgra32);
                    //drawImage.Render(drawingVisual);

                    //ImageBrush imageBrush = new ImageBrush(drawImage);
                    //imageBrush.Viewbox = image.Bounds;
                    //imageBrush.Viewport = image.Bounds;
                    //imageBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    //imageBrush.ViewportUnits = BrushMappingMode.Absolute;
                    //imageBrush.TileMode = TileMode.None;
                    //imageBrush.Stretch = Stretch.None;

                    //this.Masking = imageBrush;

                    var maskBrush = new DrawingBrush(image);
                    //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
                    //tb.Viewport = new Rect(0, 0, destRect.Width, destRect.Height);
                    maskBrush.Viewbox = image.Bounds;
                    maskBrush.Viewport = image.Bounds;
                    maskBrush.ViewboxUnits = BrushMappingMode.Absolute;
                    maskBrush.ViewportUnits = BrushMappingMode.Absolute;
                    maskBrush.TileMode = TileMode.None;
                    maskBrush.Stretch = Stretch.Uniform;

                    ////maskBrush.AlignmentX = AlignmentX.Center;
                    ////maskBrush.AlignmentY = AlignmentY.Center;

                    Masking = maskBrush;

                    MaskUnits = (SvgUnitType)maskElement.MaskUnits.AnimVal;
                    MaskContentUnits = (SvgUnitType)maskElement.MaskContentUnits.AnimVal;
                }
            }
        }

        private static double CalcPatternUnit(SvgMaskElement maskElement, SvgLength length,
            SvgLengthDirection dir, Rect bounds)
        {
            if (maskElement.MaskUnits.AnimVal.Equals(SvgUnitType.UserSpaceOnUse)) return length.Value;

            var calcValue = length.ValueInSpecifiedUnits;
            if (dir == SvgLengthDirection.Horizontal)
                calcValue *= bounds.Width;
            else
                calcValue *= bounds.Height;
            if (length.UnitType == SvgLengthType.Percentage) calcValue /= 100F;

            return calcValue;
        }

        private static Rect GetMaskDestRect(SvgMaskElement maskElement, Rect bounds)
        {
            var result = new Rect(0, 0, 0, 0);

            result.X = CalcPatternUnit(maskElement, maskElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Y = CalcPatternUnit(maskElement, maskElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            result.Width = CalcPatternUnit(maskElement, maskElement.Width.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Height = CalcPatternUnit(maskElement, maskElement.Height.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            return result;
        }

        protected void SetQuality(WpfDrawingContext context)
        {
            //Graphics graphics = gr.Graphics;

            //string colorRendering = _svgElement.GetComputedStringValue("color-rendering", String.Empty);
            //switch (colorRendering)
            //{
            //    case "optimizeSpeed":
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
            //        break;
            //    case "optimizeQuality":
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            //        break;
            //    default:
            //        // "auto"
            //        // todo: could use AssumeLinear for slightly better
            //        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.Default;
            //        break;
            //}

            //if (element is SvgTextContentElement)
            //{
            //    // Unfortunately the text rendering hints are not applied because the
            //    // text path is recorded and painted to the Graphics object as a path
            //    // not as text.
            //    string textRendering = _svgElement.GetComputedStringValue("text-rendering", String.Empty);
            //    switch (textRendering)
            //    {
            //        case "optimizeSpeed":
            //            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixel;
            //            break;
            //        case "optimizeLegibility":
            //            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            //            break;
            //        case "geometricPrecision":
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            //            break;
            //        default:
            //            // "auto"
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            //graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            //            break;
            //    }
            //}
            //else
            //{
            //    string shapeRendering = _svgElement.GetComputedStringValue("shape-rendering", String.Empty);
            //    switch (shapeRendering)
            //    {
            //        case "optimizeSpeed":
            //            graphics.SmoothingMode = SmoothingMode.HighSpeed;
            //            break;
            //        case "crispEdges":
            //            graphics.SmoothingMode = SmoothingMode.None;
            //            break;
            //        case "geometricPrecision":
            //            graphics.SmoothingMode = SmoothingMode.HighQuality;
            //            break;
            //        default:
            //            // "auto"
            //            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            //            break;
            //    }
            //}
        }

        protected void SetTransform(WpfDrawingContext context)
        {
            Transform = null;

            var transElm = _svgElement as ISvgTransformable;
            if (transElm != null)
            {
                var svgTList = (SvgTransformList)transElm.Transform.AnimVal;
                var svgMatrix = ((SvgTransformList)transElm.Transform.AnimVal).TotalMatrix;

                if (svgMatrix.IsIdentity) return;

                Transform = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                    svgMatrix.D, svgMatrix.E, svgMatrix.F);
            }
        }

        protected void FitToViewbox(WpfDrawingContext context, Rect elementBounds)
        {
            var fitToView = _svgElement as ISvgFitToViewBox;
            if (fitToView == null) return;

            var spar = (SvgPreserveAspectRatio)fitToView.PreserveAspectRatio.AnimVal;

            var transformArray = spar.FitToViewBox((SvgRect)fitToView.ViewBox.AnimVal,
                new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height));

            var translateX = transformArray[0];
            var translateY = transformArray[1];
            var scaleX = transformArray[2];
            var scaleY = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            //if (translateX >= 0 && translateY >= 0)
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0f) scaleMatrix = new ScaleTransform(scaleX, scaleY);

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                Transform = transformGroup;
            }
            else if (translateMatrix != null)
            {
                Transform = translateMatrix;
            }
            else if (scaleMatrix != null)
            {
                Transform = scaleMatrix;
            }
        }

        protected void FitToViewbox(WpfDrawingContext context, SvgElement svgElement, Rect elementBounds)
        {
            if (svgElement == null) return;
            var fitToView = svgElement as ISvgFitToViewBox;
            if (fitToView == null) return;

            var spar = (SvgPreserveAspectRatio)fitToView.PreserveAspectRatio.AnimVal;

            var transformArray = spar.FitToViewBox((SvgRect)fitToView.ViewBox.AnimVal,
                new SvgRect(elementBounds.X, elementBounds.Y, elementBounds.Width, elementBounds.Height));

            var translateX = transformArray[0];
            var translateY = transformArray[1];
            var scaleX = transformArray[2];
            var scaleY = transformArray[3];

            Transform translateMatrix = null;
            Transform scaleMatrix = null;
            //if (translateX != 0 || translateY != 0)
            {
                translateMatrix = new TranslateTransform(translateX, translateY);
            }
            if ((float)scaleX != 1.0f && (float)scaleY != 1.0f) scaleMatrix = new ScaleTransform(scaleX, scaleY);

            if (translateMatrix != null && scaleMatrix != null)
            {
                // Create a TransformGroup to contain the transforms
                // and add the transforms to it.
                var transformGroup = new TransformGroup();
                transformGroup.Children.Add(scaleMatrix);
                transformGroup.Children.Add(translateMatrix);

                Transform = transformGroup;
            }
            else if (translateMatrix != null)
            {
                Transform = translateMatrix;
            }
            else if (scaleMatrix != null)
            {
                Transform = scaleMatrix;
            }
        }

        #endregion

        #region Private Methods

        private GeometryCollection CreateClippingRegion(SvgClipPathElement clipPath,
            WpfDrawingContext context)
        {
            var geomColl = new GeometryCollection();

            foreach (XmlNode node in clipPath.ChildNodes)
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
                                var childPath = CreateGeometry(element, context.OptimizePath);

                                if (childPath != null) geomColl.Add(childPath);
                            }
                        }

                        useElement.RemoveChild(refEl);
                        useElement.RestoreReferencedElement(refEl);
                        refElParent.AppendChild(refEl);
                        useElement.OwnerDocument.Static = false;
                    }
                }
                else
                {
                    var element = node as SvgStyleableElement;
                    if (element != null)
                    {
                        if (element.RenderingHint == SvgRenderingHint.Shape)
                        {
                            var childPath = CreateGeometry(element, context.OptimizePath);

                            if (childPath != null) geomColl.Add(childPath);
                        }
                        else if (element.RenderingHint == SvgRenderingHint.Text)
                        {
                            var textGeomColl = GetTextClippingRegion(element, context);
                            if (textGeomColl != null)
                                for (var i = 0; i < textGeomColl.Count; i++)
                                    geomColl.Add(textGeomColl[i]);
                        }
                    }
                }
            }

            return geomColl;
        }

        private GeometryCollection GetTextClippingRegion(SvgStyleableElement element,
            WpfDrawingContext context)
        {
            var geomColl = new GeometryCollection();

            var renderer = new WpfDrawingRenderer();
            renderer.Window = _svgElement.OwnerDocument.Window as SvgWindow;

            var settings = context.Settings.Clone();
            settings.TextAsGeometry = true;
            var clipContext = new WpfDrawingContext(true,
                settings);
            clipContext.RenderingClipRegion = true;

            clipContext.Initialize(null, context.FontFamilyVisitor, null);

            renderer.Render(element, clipContext);

            var rootGroup = renderer.Drawing as DrawingGroup;
            if (rootGroup != null && rootGroup.Children.Count == 1)
            {
                var textGroup = rootGroup.Children[0] as DrawingGroup;
                if (textGroup != null) ExtractGeometry(textGroup, geomColl);
            }

            return geomColl;
        }

        private static void ExtractGeometry(DrawingGroup group, GeometryCollection geomColl)
        {
            if (geomColl == null) return;

            var drawings = group.Children;
            var textItem = drawings.Count;
            for (var i = 0; i < textItem; i++)
            {
                var drawing = drawings[i];
                var aDrawing = drawing as GeometryDrawing;
                if (aDrawing != null)
                {
                    var aGeometry = aDrawing.Geometry;
                    if (aGeometry != null)
                    {
                        var geomGroup = aGeometry as GeometryGroup;
                        if (geomGroup != null)
                        {
                            var children = geomGroup.Children;
                            for (var j = 0; j < children.Count; j++) geomColl.Add(children[j]);
                        }
                        else
                        {
                            geomColl.Add(aGeometry);
                        }
                    }
                }
                else
                {
                    var innerGroup = drawing as DrawingGroup;
                    if (innerGroup != null) ExtractGeometry(innerGroup, geomColl);
                }
            }
        }

        #endregion

        #region Geometry Methods

        public static Geometry CreateGeometry(ISvgElement element, bool optimizePath)
        {
            if (element == null || element.RenderingHint != SvgRenderingHint.Shape) return null;

            var localName = element.LocalName;
            switch (localName)
            {
                case "ellipse":
                    return CreateGeometry((SvgEllipseElement)element);
                case "rect":
                    return CreateGeometry((SvgRectElement)element);
                case "line":
                    return CreateGeometry((SvgLineElement)element);
                case "path":
                    if (optimizePath)
                        return CreateGeometryEx((SvgPathElement)element);
                    return CreateGeometry((SvgPathElement)element);
                case "circle":
                    return CreateGeometry((SvgCircleElement)element);
                case "polyline":
                    return CreateGeometry((SvgPolylineElement)element);
                case "polygon":
                    return CreateGeometry((SvgPolygonElement)element);
            }

            return null;
        }

        #region SvgEllipseElement Geometry

        public static Geometry CreateGeometry(SvgEllipseElement element)
        {
            var _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            var _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            var _rx = Math.Round(element.Rx.AnimVal.Value, 4);
            var _ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (_rx <= 0 || _ry <= 0) return null;

            /*if (_cx <= 1 && _cy <= 1 && _rx <= 1 && _ry <= 1)
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2, _ry*2);
            }
            else
            {
                gp.AddEllipse(_cx-_rx, _cy-_ry, _rx*2 - 1, _ry*2 - 1);
            }*/
            //gp.AddEllipse(_cx - _rx, _cy - _ry, _rx * 2, _ry * 2);

            var geometry = new EllipseGeometry(new Point(_cx, _cy),
                _rx, _ry);

            return geometry;
        }

        #endregion

        #region SvgRectElement Geometry

        public static Geometry CreateGeometry(SvgRectElement element)
        {
            var dx = Math.Round(element.X.AnimVal.Value, 4);
            var dy = Math.Round(element.Y.AnimVal.Value, 4);
            var width = Math.Round(element.Width.AnimVal.Value, 4);
            var height = Math.Round(element.Height.AnimVal.Value, 4);
            var rx = Math.Round(element.Rx.AnimVal.Value, 4);
            var ry = Math.Round(element.Ry.AnimVal.Value, 4);

            if (width <= 0 || height <= 0) return null;
            if (rx <= 0 && ry > 0)
                rx = ry;
            else if (rx > 0 && ry <= 0) ry = rx;

            return new RectangleGeometry(new Rect(dx, dy, width, height), rx, ry);
        }

        #endregion

        #region SvgLineElement Geometry

        public static Geometry CreateGeometry(SvgLineElement element)
        {
            return new LineGeometry(new Point(Math.Round(element.X1.AnimVal.Value, 4),
                    Math.Round(element.Y1.AnimVal.Value, 4)),
                new Point(Math.Round(element.X2.AnimVal.Value, 4), Math.Round(element.Y2.AnimVal.Value, 4)));
        }

        #endregion

        #region SvgPathElement Geometry

        public static Geometry CreateGeometryEx(SvgPathElement element)
        {
            var geometry = new PathGeometry();

            var pathScript = element.PathScript;
            if (string.IsNullOrEmpty(pathScript)) return geometry;

            var fillRule = element.GetPropertyValue("fill-rule");
            var clipRule = element.GetAttribute("clip-rule");
            if ((!string.IsNullOrEmpty(clipRule) &&
                 string.Equals(clipRule, "evenodd")) || string.Equals(clipRule, "nonzero"))
                fillRule = clipRule;
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            try
            {
                geometry.Figures = PathFigureCollection.Parse(pathScript);
            }
            catch
            {
            }

            return geometry;
        }

        public static Geometry CreateGeometry(SvgPathElement element)
        {
            var geometry = new PathGeometry();

            var fillRule = element.GetPropertyValue("fill-rule");
            var clipRule = element.GetAttribute("clip-rule");
            if ((!string.IsNullOrEmpty(clipRule) &&
                 string.Equals(clipRule, "evenodd")) || string.Equals(clipRule, "nonzero"))
                fillRule = clipRule;
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            var initPoint = new SvgPointF(0, 0);
            var lastPoint = new SvgPointF(0, 0);

            ISvgPathSeg segment = null;
            SvgPathSegMoveto pathMoveTo = null;
            SvgPathSegLineto pathLineTo = null;
            SvgPathSegCurveto pathCurveTo = null;
            SvgPathSegArc pathArc = null;

            var segments = element.PathSegList;
            var nElems = segments.NumberOfItems;

            PathFigure pathFigure = null;

            for (var i = 0; i < nElems; i++)
            {
                segment = segments.GetItem(i);

                if (DynamicCast.Cast(segment, out pathMoveTo))
                {
                    if (pathFigure != null)
                    {
                        pathFigure.IsClosed = false;
                        pathFigure.IsFilled = true;
                        geometry.Figures.Add(pathFigure);
                        pathFigure = null;
                    }

                    lastPoint = initPoint = pathMoveTo.AbsXY;

                    pathFigure = new PathFigure();
                    pathFigure.StartPoint = new Point(initPoint.ValueX, initPoint.ValueY);
                }
                else if (DynamicCast.Cast(segment, out pathLineTo))
                {
                    var p = pathLineTo.AbsXY;
                    pathFigure.Segments.Add(new LineSegment(new Point(p.ValueX, p.ValueY), true));

                    lastPoint = p;
                }
                else if (DynamicCast.Cast(segment, out pathCurveTo))
                {
                    var xy = pathCurveTo.AbsXY;
                    var x1y1 = pathCurveTo.CubicX1Y1;
                    var x2y2 = pathCurveTo.CubicX2Y2;
                    pathFigure.Segments.Add(new BezierSegment(new Point(x1y1.ValueX, x1y1.ValueY),
                        new Point(x2y2.ValueX, x2y2.ValueY), new Point(xy.ValueX, xy.ValueY), true));

                    lastPoint = xy;
                }
                else if (DynamicCast.Cast(segment, out pathArc))
                {
                    var p = pathArc.AbsXY;
                    if (lastPoint.Equals(p))
                    {
                        // If the endpoints (x, y) and (x0, y0) are identical, then this
                        // is equivalent to omitting the elliptical arc segment entirely.
                    }
                    else if (pathArc.R1 == 0 || pathArc.R2 == 0)
                    {
                        // Ensure radii are valid
                        pathFigure.Segments.Add(new LineSegment(new Point(p.ValueX, p.ValueY), true));
                    }
                    else
                    {
                        var calcValues = pathArc.GetCalculatedArcValues();

                        pathFigure.Segments.Add(new ArcSegment(new Point(p.ValueX, p.ValueY),
                            new Size(pathArc.R1, pathArc.R2), pathArc.Angle, pathArc.LargeArcFlag,
                            pathArc.SweepFlag ? SweepDirection.Clockwise : SweepDirection.Counterclockwise,
                            true));
                    }

                    lastPoint = p;
                }
                else if (segment is SvgPathSegClosePath)
                {
                    if (pathFigure != null)
                    {
                        pathFigure.IsClosed = true;
                        pathFigure.IsFilled = true;
                        geometry.Figures.Add(pathFigure);
                        pathFigure = null;
                    }

                    lastPoint = initPoint;
                }
            }

            if (pathFigure != null)
            {
                pathFigure.IsClosed = false;
                pathFigure.IsFilled = true;
                geometry.Figures.Add(pathFigure);
            }

            return geometry;
        }

        #endregion

        #region SvgCircleElement Geometry

        public static Geometry CreateGeometry(SvgCircleElement element)
        {
            var _cx = Math.Round(element.Cx.AnimVal.Value, 4);
            var _cy = Math.Round(element.Cy.AnimVal.Value, 4);
            var _r = Math.Round(element.R.AnimVal.Value, 4);

            if (_r <= 0) return null;

            var geometry = new EllipseGeometry(new Point(_cx, _cy), _r, _r);

            return geometry;
        }

        #endregion

        #region SvgPolylineElement Geometry

        public static Geometry CreateGeometry(SvgPolylineElement element)
        {
            var list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0) return null;

            var points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                var point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }

            var polyline = new PolyLineSegment();
            polyline.Points = points;

            var polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed = false;
            polylineFigure.IsFilled = true;

            polylineFigure.Segments.Add(polyline);

            var geometry = new PathGeometry();

            var fillRule = element.GetPropertyValue("fill-rule");
            var clipRule = element.GetAttribute("clip-rule");
            if ((!string.IsNullOrEmpty(clipRule) &&
                 string.Equals(clipRule, "evenodd")) || string.Equals(clipRule, "nonzero"))
                fillRule = clipRule;
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #region SvgPolygonElement Geometry

        public static Geometry CreateGeometry(SvgPolygonElement element)
        {
            var list = element.AnimatedPoints;
            ulong nElems = list.NumberOfItems;
            if (nElems == 0) return null;

            var points = new PointCollection((int)nElems);

            for (uint i = 0; i < nElems; i++)
            {
                var point = list.GetItem(i);
                points.Add(new Point(Math.Round(point.X, 4), Math.Round(point.Y, 4)));
            }

            var polyline = new PolyLineSegment();
            polyline.Points = points;

            var polylineFigure = new PathFigure();
            polylineFigure.StartPoint = points[0];
            polylineFigure.IsClosed = true;
            polylineFigure.IsFilled = true;

            polylineFigure.Segments.Add(polyline);

            var geometry = new PathGeometry();

            var fillRule = element.GetPropertyValue("fill-rule");
            var clipRule = element.GetAttribute("clip-rule");
            if ((!string.IsNullOrEmpty(clipRule) &&
                 string.Equals(clipRule, "evenodd")) || string.Equals(clipRule, "nonzero"))
                fillRule = clipRule;
            if (fillRule == "evenodd")
                geometry.FillRule = FillRule.EvenOdd;
            else if (fillRule == "nonzero")
                geometry.FillRule = FillRule.Nonzero;

            geometry.Figures.Add(polylineFigure);

            return geometry;
        }

        #endregion

        #endregion

        #region Marker Methods

        protected static string ExtractMarkerUrl(string propValue)
        {
            var match = _reUrl.Match(propValue);
            if (match.Success)
                return match.Groups["uri"].Value;
            return string.Empty;
        }

        protected static void RenderMarkers(WpfDrawingRenderer renderer,
            SvgStyleableElement styleElm, WpfDrawingContext gr)
        {
            // OPTIMIZE

            if (styleElm is ISharpMarkerHost)
            {
                var markerStartUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-start", "marker"));
                var markerMiddleUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-mid", "marker"));
                var markerEndUrl = ExtractMarkerUrl(styleElm.GetPropertyValue("marker-end", "marker"));

                if (markerStartUrl.Length > 0)
                {
                    var markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerStartUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.Start, styleElm);
                }

                if (markerMiddleUrl.Length > 0)
                {
                    // TODO markerMiddleUrl != markerStartUrl
                    var markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerMiddleUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.Mid, styleElm);
                }

                if (markerEndUrl.Length > 0)
                {
                    // TODO: markerEndUrl != markerMiddleUrl
                    var markerRenderer = CreateByUri(styleElm.OwnerDocument,
                        styleElm.BaseURI, markerEndUrl) as WpfMarkerRendering;
                    if (markerRenderer != null)
                        markerRenderer.RenderMarker(renderer, gr, SvgMarkerPosition.End, styleElm);
                }
            }
        }

        #endregion
    }
}