using System.Windows;
using System.Windows.Media;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.Fills;

namespace BaseProj.SharpVectors.SharpVectorRenderingWpf.Wpf
{
    public sealed class WpfPatternFill : WpfFill
    {
        #region Constructors and Destructor

        public WpfPatternFill(SvgPatternElement patternElement)
        {
            _patternElement = patternElement;
        }

        #endregion

        #region Public Methods

        public override Brush GetBrush(Rect elementBounds, WpfDrawingContext context)
        {
            var bounds = new Rect(0, 0, 1, 1);
            var image = GetImage(context);
            var destRect = GetDestRect(bounds);

            var tb = new DrawingBrush(image);
            //tb.Viewbox = new Rect(0, 0, destRect.Width, destRect.Height);
            //tb.Viewport = new Rect(0, 0, destRect.Width, destRect.Height);
            tb.Viewbox = destRect;
            tb.Viewport = destRect;
            tb.ViewboxUnits = BrushMappingMode.Absolute;
            tb.ViewportUnits = BrushMappingMode.Absolute;
            tb.TileMode = TileMode.Tile;

            var transform = GetTransformMatrix(image.Bounds);
            if (transform != null && !transform.Matrix.IsIdentity) tb.Transform = transform;

            return tb;
        }

        #endregion

        #region Private Fields

        private XmlElement oldParent;
        private readonly SvgPatternElement _patternElement;

        #endregion

        #region Private Methods

        private SvgSvgElement MoveIntoSvgElement()
        {
            var doc = _patternElement.OwnerDocument;
            var svgElm = doc.CreateElement("", "svg", SvgDocument.SvgNamespace) as SvgSvgElement;

            var children = _patternElement.Children;
            if (children.Count > 0) oldParent = children[0].ParentNode as XmlElement;

            for (var i = 0; i < children.Count; i++) svgElm.AppendChild(children[i]);

            if (_patternElement.HasAttribute("viewBox"))
                svgElm.SetAttribute("viewBox", _patternElement.GetAttribute("viewBox"));
            //svgElm.SetAttribute("x", "0");
            //svgElm.SetAttribute("y", "0");
            svgElm.SetAttribute("x", _patternElement.GetAttribute("x"));
            svgElm.SetAttribute("y", _patternElement.GetAttribute("y"));
            svgElm.SetAttribute("width", _patternElement.GetAttribute("width"));
            svgElm.SetAttribute("height", _patternElement.GetAttribute("height"));

            if (_patternElement.PatternContentUnits.AnimVal.Equals(SvgUnitType.ObjectBoundingBox))
                svgElm.SetAttribute("viewBox", "0 0 1 1");

            _patternElement.AppendChild(svgElm);

            return svgElm;
        }

        private void MoveOutOfSvgElement(SvgSvgElement svgElm)
        {
            while (svgElm.ChildNodes.Count > 0) oldParent.AppendChild(svgElm.ChildNodes[0]);

            _patternElement.RemoveChild(svgElm);
        }

        private Drawing GetImage(WpfDrawingContext context)
        {
            var renderer = new WpfDrawingRenderer();
            renderer.Window = _patternElement.OwnerDocument.Window as SvgWindow;

            var settings = context.Settings.Clone();
            settings.TextAsGeometry = true;
            var patternContext = new WpfDrawingContext(true,
                settings);

            patternContext.Initialize(null, context.FontFamilyVisitor, null);

            var elm = MoveIntoSvgElement();

            renderer.Render(elm, patternContext);
            var img = renderer.Drawing;

            MoveOutOfSvgElement(elm);

            return img;
        }

        private double CalcPatternUnit(SvgLength length, SvgLengthDirection dir, Rect bounds)
        {
            if (_patternElement.PatternUnits.AnimVal.Equals(SvgUnitType.UserSpaceOnUse)) return length.Value;

            var calcValue = length.ValueInSpecifiedUnits;
            if (dir == SvgLengthDirection.Horizontal)
                calcValue *= bounds.Width;
            else
                calcValue *= bounds.Height;
            if (length.UnitType == SvgLengthType.Percentage) calcValue /= 100F;

            return calcValue;
        }

        private Rect GetDestRect(Rect bounds)
        {
            var result = new Rect(0, 0, 0, 0);

            result.X = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Y = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            result.Width = CalcPatternUnit(_patternElement.Width.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            result.Height = CalcPatternUnit(_patternElement.Height.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            return result;
        }

        private MatrixTransform GetTransformMatrix(Rect bounds)
        {
            var svgMatrix =
                ((SvgTransformList)_patternElement.PatternTransform.AnimVal).TotalMatrix;

            var transformMatrix = new MatrixTransform(svgMatrix.A, svgMatrix.B, svgMatrix.C,
                svgMatrix.D, svgMatrix.E, svgMatrix.F);

            var translateX = CalcPatternUnit(_patternElement.X.AnimVal as SvgLength,
                SvgLengthDirection.Horizontal, bounds);
            var translateY = CalcPatternUnit(_patternElement.Y.AnimVal as SvgLength,
                SvgLengthDirection.Vertical, bounds);

            transformMatrix.Matrix.TranslatePrepend(translateX, translateY);

            return transformMatrix;
        }

        #endregion
    }
}