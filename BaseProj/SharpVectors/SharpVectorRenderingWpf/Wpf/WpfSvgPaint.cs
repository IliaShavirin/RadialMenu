using System;
using System.Windows;
using System.Windows.Media;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Painting;
using BaseProj.SharpVectors.SharpVectorModel;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Paint;
using BaseProj.SharpVectors.SharpVectorRenderingWpf.Utils;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfSvgPaint : SvgPaint
    {
        #region Constructors and Destructor

        public WpfSvgPaint(WpfDrawingContext context, SvgStyleableElement elm, string propName)
            : base(elm.GetComputedStyle("").GetPropertyValue(propName))
        {
            _propertyName = propName;
            _element = elm;
            _context = context;
        }

        #endregion

        #region Public Properties

        public WpfFill PaintServer { get; private set; }

        #endregion

        #region Private Fields

        private readonly string _propertyName;
        private readonly SvgStyleableElement _element;
        private readonly WpfDrawingContext _context;

        #endregion

        #region Public Methods

        public Brush GetBrush()
        {
            return GetBrush(null, "fill", true);
        }

        public Brush GetBrush(bool setOpacity)
        {
            return GetBrush(null, "fill", setOpacity);
        }

        public Brush GetBrush(Geometry geometry)
        {
            return GetBrush(geometry, "fill", true);
        }

        public Brush GetBrush(Geometry geometry, bool setOpacity)
        {
            return GetBrush(geometry, "fill", setOpacity);
        }

        public Pen GetPen()
        {
            return GetPen(null);
        }

        public Pen GetPen(Geometry geometry)
        {
            var strokeWidth = GetStrokeWidth();
            if (strokeWidth == 0)
                return null;

            WpfSvgPaint stroke;
            if (PaintType == SvgPaintType.None)
                return null;
            if (PaintType == SvgPaintType.CurrentColor)
                stroke = new WpfSvgPaint(_context, _element, "color");
            else
                stroke = this;

            var pen = new Pen(stroke.GetBrush(geometry, "stroke", true),
                strokeWidth);

            pen.StartLineCap = pen.EndLineCap = GetLineCap();
            pen.LineJoin = GetLineJoin();
            var miterLimit = GetMiterLimit(strokeWidth);
            if (miterLimit > 0) pen.MiterLimit = miterLimit;

            //pen.MiterLimit = 1.0f;

            var dashArray = GetDashArray(strokeWidth);
            if (dashArray != null && dashArray.Count != 0)
            {
                var isValidDashes = true;
                //Do not draw if dash array had a zero value in it
                for (var i = 0; i < dashArray.Count; i++)
                    if (dashArray[i] == 0)
                        isValidDashes = false;

                if (isValidDashes)
                {
                    var dashStyle = new DashStyle(dashArray, GetDashOffset(strokeWidth));

                    pen.DashStyle = dashStyle;
                    // This is the one that works well for the XAML, the default is not Flat as
                    // stated in the documentations...
                    pen.DashCap = PenLineCap.Flat;
                }
            }

            return pen;
        }

        #endregion

        #region Private Methods

        private double GetOpacity(string fillOrStroke)
        {
            double opacityValue = 1;

            var opacity = _element.GetPropertyValue(fillOrStroke + "-opacity");
            if (opacity != null && opacity.Length > 0) opacityValue *= SvgNumber.ParseNumber(opacity);

            opacity = _element.GetPropertyValue("opacity");
            if (opacity != null && opacity.Length > 0) opacityValue *= SvgNumber.ParseNumber(opacity);

            opacityValue = Math.Min(opacityValue, 1);
            opacityValue = Math.Max(opacityValue, 0);

            return opacityValue;
        }

        private PenLineCap GetLineCap()
        {
            switch (_element.GetPropertyValue("stroke-linecap"))
            {
                case "round":
                    return PenLineCap.Round;
                case "square":
                    return PenLineCap.Square;
                case "butt":
                    return PenLineCap.Flat;
                case "triangle":
                    return PenLineCap.Triangle;
                default:
                    return PenLineCap.Flat;
            }
        }

        private PenLineJoin GetLineJoin()
        {
            switch (_element.GetPropertyValue("stroke-linejoin"))
            {
                case "round":
                    return PenLineJoin.Round;
                case "bevel":
                    return PenLineJoin.Bevel;
                default:
                    return PenLineJoin.Miter;
            }
        }

        private double GetStrokeWidth()
        {
            var strokeWidth = _element.GetPropertyValue("stroke-width");
            if (strokeWidth.Length == 0) strokeWidth = "1px";

            var strokeWidthLength = new SvgLength(_element, "stroke-width",
                SvgLengthDirection.Viewport, strokeWidth);

            return strokeWidthLength.Value;
        }

        private double GetMiterLimit(double strokeWidth)
        {
            // Use this to prevent the default value of "4" being used...
            var miterLimitAttr = _element.GetAttribute("stroke-miterlimit");
            if (string.IsNullOrEmpty(miterLimitAttr))
            {
                var strokeLinecap = _element.GetAttribute("stroke-linecap");
                if (string.Equals(strokeLinecap, "round", StringComparison.OrdinalIgnoreCase)) return 1.0d;
                return -1.0d;
            }

            var miterLimitStr = _element.GetPropertyValue("stroke-miterlimit");
            if (string.IsNullOrEmpty(miterLimitStr) || (float)strokeWidth <= 0) return -1.0d;

            var miterLimit = SvgNumber.ParseNumber(miterLimitStr);
            if (miterLimit < 1)
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr,
                    "stroke-miterlimit can not be less then 1");

            //if (miterLimit < 1.0d)
            //{
            //    return -1.0d;
            //}

            var ratioLimit = miterLimit / strokeWidth;
            if (ratioLimit >= 1.8d)
                return miterLimit;
            return 1.0d;
        }

        private DoubleCollection GetDashArray(double strokeWidth)
        {
            var dashArrayText = _element.GetPropertyValue("stroke-dasharray");
            if (string.IsNullOrEmpty(dashArrayText)) return null;

            if (dashArrayText == "none") return null;

            var list = new SvgNumberList(dashArrayText);

            var len = list.NumberOfItems;
            //float[] fDashArray = new float[len];
            var dashArray = new DoubleCollection((int)len);

            for (uint i = 0; i < len; i++)
                //divide by strokeWidth to take care of the difference between Svg and WPF
                dashArray.Add(list.GetItem(i).Value / strokeWidth);

            return dashArray;
        }

        private double GetDashOffset(double strokeWidth)
        {
            var dashOffset = _element.GetPropertyValue("stroke-dashoffset");
            if (dashOffset.Length > 0)
            {
                //divide by strokeWidth to take care of the difference between Svg and GDI+
                var dashOffsetLength = new SvgLength(_element, "stroke-dashoffset",
                    SvgLengthDirection.Viewport, dashOffset);
                return dashOffsetLength.Value;
            }

            return 0;
        }

        private WpfFill GetPaintFill(string uri)
        {
            var absoluteUri = _element.ResolveUri(uri);

            if (_element.Imported && _element.ImportDocument != null &&
                _element.ImportNode != null)
            {
                // We need to determine whether the provided URI refers to element in the
                // original document or in the current document...
                var styleElm = _element.ImportNode as SvgStyleableElement;
                if (styleElm != null)
                {
                    var propertyValue =
                        styleElm.GetComputedStyle("").GetPropertyValue(_propertyName);

                    if (!string.IsNullOrEmpty(propertyValue))
                    {
                        var importFill = new WpfSvgPaint(_context, styleElm, _propertyName);
                        if (string.Equals(uri, importFill.Uri, StringComparison.OrdinalIgnoreCase))
                        {
                            var fill = WpfFill.CreateFill(_element.ImportDocument, absoluteUri);
                            if (fill != null) return fill;
                        }
                    }
                }
            }

            return WpfFill.CreateFill(_element.OwnerDocument, absoluteUri);
        }

        private Brush GetBrush(Geometry geometry, string propPrefix,
            bool setOpacity)
        {
            SvgPaint fill;
            if (PaintType == SvgPaintType.None)
                return null;
            if (PaintType == SvgPaintType.CurrentColor)
                fill = new WpfSvgPaint(_context, _element, "color");
            else
                fill = this;

            var paintType = fill.PaintType;
            if (paintType == SvgPaintType.Uri || paintType == SvgPaintType.UriCurrentColor ||
                paintType == SvgPaintType.UriNone || paintType == SvgPaintType.UriRgbColor ||
                paintType == SvgPaintType.UriRgbColorIccColor)
            {
                PaintServer = GetPaintFill(fill.Uri);
                if (PaintServer != null)
                {
                    Brush brush = null;
                    if (geometry != null)
                        brush = PaintServer.GetBrush(geometry.Bounds, _context);
                    else
                        brush = PaintServer.GetBrush(Rect.Empty, _context);

                    if (brush != null) brush.Opacity = GetOpacity(propPrefix);

                    return brush;
                }

                if (PaintType == SvgPaintType.UriNone || PaintType == SvgPaintType.Uri)
                    return null;
                if (PaintType == SvgPaintType.UriCurrentColor)
                    fill = new WpfSvgPaint(_context, _element, "color");
                else
                    fill = this;
            }

            if (fill == null || fill.RgbColor == null) return null;

            var solidColor = WpfConvert.ToColor(fill.RgbColor);
            if (solidColor == null) return null;

            var solidBrush = new SolidColorBrush(solidColor.Value);
            //int opacity = GetOpacity(propPrefix);
            //solidBrush.Color = Color.FromArgb(opacity, brush.Color);
            if (setOpacity) solidBrush.Opacity = GetOpacity(propPrefix);
            return solidBrush;
        }

        #endregion
    }
}