using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.ClippingMaskingCompositing;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.ClippingMaskingCompositing
{
    public sealed class SvgMaskElement : SvgStyleableElement, ISvgMaskElement
    {
        #region Constructors and Destructor

        public SvgMaskElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private ISvgAnimatedLength x;
        private ISvgAnimatedLength y;
        private ISvgAnimatedLength width;
        private ISvgAnimatedLength height;

        private ISvgAnimatedEnumeration maskUnits;
        private ISvgAnimatedEnumeration maskContentUnits;

        private readonly SvgTests svgTests;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

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
        ///     This will always return <see cref="SvgRenderingHint.Masking" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Masking;

        #endregion

        #region ISvgMaskElement Members

        public ISvgAnimatedEnumeration MaskUnits
        {
            get
            {
                if (maskUnits == null)
                {
                    var mask = SvgUnitType.ObjectBoundingBox;
                    if (GetAttribute("maskUnits") == "userSpaceOnUse")
                        mask = SvgUnitType.UserSpaceOnUse;
                    maskUnits = new SvgAnimatedEnumeration((ushort)mask);
                }

                return maskUnits;
            }
        }

        public ISvgAnimatedEnumeration MaskContentUnits
        {
            get
            {
                if (maskContentUnits == null)
                {
                    var maskContent = SvgUnitType.UserSpaceOnUse;
                    if (GetAttribute("maskContentUnits") == "objectBoundingBox")
                        maskContent = SvgUnitType.ObjectBoundingBox;
                    maskContentUnits = new SvgAnimatedEnumeration((ushort)maskContent);
                }

                return maskContentUnits;
            }
        }

        public ISvgAnimatedLength X
        {
            get
            {
                if (x == null) x = new SvgAnimatedLength(this, "x", SvgLengthDirection.Horizontal, "-10%");
                return x;
            }
        }

        public ISvgAnimatedLength Y
        {
            get
            {
                if (y == null) y = new SvgAnimatedLength(this, "y", SvgLengthDirection.Vertical, "-10%");
                return y;
            }
        }

        public ISvgAnimatedLength Width
        {
            get
            {
                if (width == null) width = new SvgAnimatedLength(this, "width", SvgLengthDirection.Viewport, "120%");
                return width;
            }
        }

        public ISvgAnimatedLength Height
        {
            get
            {
                if (height == null) height = new SvgAnimatedLength(this, "height", SvgLengthDirection.Viewport, "120%");
                return height;
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