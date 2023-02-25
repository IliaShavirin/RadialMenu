// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Shapes;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Shapes
{
    /// <summary>
    ///     The SvgEllipseElement class corresponds to the 'ellipse' element.
    /// </summary>
    public sealed class SvgEllipseElement : SvgTransformableElement, ISvgEllipseElement
    {
        #region Constructors and Destructor

        public SvgEllipseElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region ISvgElement Members

        /// <summary>
        ///     Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        ///     An enumeration of the <see cref="SvgRenderingHint" /> specifying the rendering hint.
        ///     This will always return <see cref="SvgRenderingHint.Shape" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Shape;

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private ISvgAnimatedLength cx;
        private ISvgAnimatedLength cy;
        private ISvgAnimatedLength rx;
        private ISvgAnimatedLength ry;

        private readonly SvgTests svgTests;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

        #endregion

        #region Public Methods

        public void Invalidate()
        {
        }

        public override void HandleAttributeChange(XmlAttribute attribute)
        {
            if (attribute.NamespaceURI.Length == 0)
            {
                switch (attribute.LocalName)
                {
                    case "cx":
                        cx = null;
                        Invalidate();
                        return;
                    case "cy":
                        cy = null;
                        Invalidate();
                        return;
                    case "rx":
                        rx = null;
                        Invalidate();
                        return;
                    case "ry":
                        ry = null;
                        Invalidate();
                        return;
                    // Color.attrib, Paint.attrib 
                    case "color":
                    case "fill":
                    case "fill-rule":
                    case "stroke":
                    case "stroke-dasharray":
                    case "stroke-dashoffset":
                    case "stroke-linecap":
                    case "stroke-linejoin":
                    case "stroke-miterlimit":
                    case "stroke-width":
                    // Opacity.attrib
                    case "opacity":
                    case "stroke-opacity":
                    case "fill-opacity":
                    // Graphics.attrib
                    case "display":
                    case "image-rendering":
                    case "shape-rendering":
                    case "text-rendering":
                    case "visibility":
                        Invalidate();
                        break;
                    case "transform":
                        Invalidate();
                        break;
                }

                base.HandleAttributeChange(attribute);
            }
        }

        #endregion

        #region ISvgEllipseElement Members

        public ISvgAnimatedLength Cx
        {
            get
            {
                if (cx == null) cx = new SvgAnimatedLength(this, "cx", SvgLengthDirection.Horizontal, "0");
                return cx;
            }
        }

        public ISvgAnimatedLength Cy
        {
            get
            {
                if (cy == null) cy = new SvgAnimatedLength(this, "cy", SvgLengthDirection.Vertical, "0");
                return cy;
            }
        }

        public ISvgAnimatedLength Rx
        {
            get
            {
                if (rx == null) rx = new SvgAnimatedLength(this, "rx", SvgLengthDirection.Horizontal, "100");
                return rx;
            }
        }

        public ISvgAnimatedLength Ry
        {
            get
            {
                if (ry == null) ry = new SvgAnimatedLength(this, "ry", SvgLengthDirection.Vertical, "100");
                return ry;
            }
        }

        #endregion

        #region ISvgTests Members

        public ISvgStringList RequiredFeatures => svgTests.RequiredFeatures;

        public ISvgStringList RequiredExtensions => svgTests.RequiredExtensions;

        public ISvgStringList SystemLanguage => svgTests.SystemLanguage;

        public bool HasExtension(string extension)
        {
            return svgTests.HasExtension(extension);
        }

        #endregion
    }
}