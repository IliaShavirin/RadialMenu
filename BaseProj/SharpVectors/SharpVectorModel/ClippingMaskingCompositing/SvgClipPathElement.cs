// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.ClippingMaskingCompositing;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.ClippingMaskingCompositing
{
    public sealed class SvgClipPathElement : SvgTransformableElement, ISvgClipPathElement
    {
        #region Constructors and Destructor

        public SvgClipPathElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgTests = new SvgTests(this);
        }

        #endregion

        #region ISvgClipPathElement Members

        public ISvgAnimatedEnumeration ClipPathUnits
        {
            get
            {
                if (clipPathUnits == null)
                {
                    var clipPath = SvgUnitType.UserSpaceOnUse;
                    if (GetAttribute("clipPathUnits") == "objectBoundingBox") clipPath = SvgUnitType.ObjectBoundingBox;

                    clipPathUnits = new SvgAnimatedEnumeration((ushort)clipPath);
                }

                return clipPathUnits;
            }
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private readonly SvgTests svgTests;
        private ISvgAnimatedEnumeration clipPathUnits;
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
        ///     This will always return <see cref="SvgRenderingHint.Clipping" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Clipping;

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