using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Fills
{
    public sealed class SvgPatternElement : SvgStyleableElement, ISvgPatternElement
    {
        #region Constructors and Destructor

        public SvgPatternElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgFitToViewBox = new SvgFitToViewBox(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region Public Properties

        public XmlNodeList Children
        {
            get
            {
                var children = SelectNodes("svg:*", OwnerDocument.NamespaceManager);
                if (children.Count > 0) return children;

                // check any eventually referenced gradient
                if (ReferencedElement == null)
                    // return an empty list
                    return children;
                return ReferencedElement.Children;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private ISvgAnimatedEnumeration patternUnits;
        private ISvgAnimatedEnumeration patternContentUnits;
        private ISvgAnimatedTransformList patternTransform;
        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private readonly SvgUriReference svgURIReference;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;
        private readonly SvgFitToViewBox svgFitToViewBox;
        private readonly SvgTests svgTests;

        #endregion

        #region ISvgElement Members

        /// <summary>
        ///     Gets a value indicating whether this SVG element is renderable.
        /// </summary>
        /// <value>
        ///     This is <see langword="'true" /> if the element is renderable; otherwise,
        ///     it is <see langword="false" />.
        /// </value>
        public override bool IsRenderable => false;

        /// <summary>
        ///     Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        ///     An enumeration of the <see cref="SvgRenderingHint" /> specifying the rendering hint.
        ///     This will always return <see cref="SvgRenderingHint.Containment" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Containment;

        #endregion

        #region ISvgPatternElement Members

        public ISvgAnimatedEnumeration PatternUnits
        {
            get
            {
                if (!HasAttribute("patternUnits") && ReferencedElement != null) return ReferencedElement.PatternUnits;

                if (patternUnits == null)
                {
                    SvgUnitType type;
                    switch (GetAttribute("patternUnits"))
                    {
                        case "userSpaceOnUse":
                            type = SvgUnitType.UserSpaceOnUse;
                            break;
                        default:
                            type = SvgUnitType.ObjectBoundingBox;
                            break;
                    }

                    patternUnits = new SvgAnimatedEnumeration((ushort)type);
                }

                return patternUnits;
            }
        }

        public ISvgAnimatedEnumeration PatternContentUnits
        {
            get
            {
                if (!HasAttribute("patternContentUnits") && ReferencedElement != null)
                    return ReferencedElement.PatternContentUnits;

                if (patternContentUnits == null)
                {
                    SvgUnitType type;
                    switch (GetAttribute("patternContentUnits"))
                    {
                        case "objectBoundingBox":
                            type = SvgUnitType.ObjectBoundingBox;
                            break;
                        default:
                            type = SvgUnitType.UserSpaceOnUse;
                            break;
                    }

                    patternContentUnits = new SvgAnimatedEnumeration((ushort)type);
                }

                return patternContentUnits;
            }
        }

        public ISvgAnimatedTransformList PatternTransform
        {
            get
            {
                if (!HasAttribute("patternTransform") && ReferencedElement != null)
                    return ReferencedElement.PatternTransform;

                if (patternTransform == null)
                    patternTransform = new SvgAnimatedTransformList(GetAttribute("patternTransform"));
                return patternTransform;
            }
        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (!HasAttribute("x") && ReferencedElement != null) return ReferencedElement.X;

                if (x == null) x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "0");
                return x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (!HasAttribute("y") && ReferencedElement != null) return ReferencedElement.Y;

                if (y == null) y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "0");
                return y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get
            {
                if (!HasAttribute("width") && ReferencedElement != null) return ReferencedElement.Width;

                if (width == null) width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Horizontal, "0");
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (!HasAttribute("height") && ReferencedElement != null) return ReferencedElement.Height;

                if (height == null) height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Vertical, "0");
                return height;
            }
        }

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => svgURIReference.Href;

        public SvgPatternElement ReferencedElement => svgURIReference.ReferencedNode as SvgPatternElement;

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedRect ViewBox => svgFitToViewBox.ViewBox;

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio => svgFitToViewBox.PreserveAspectRatio;

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