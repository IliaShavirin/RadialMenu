using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    /// <summary>
    ///     The SvgSwitchElement interface corresponds to the 'switch' element.
    /// </summary>
    public sealed class SvgSwitchElement : SvgTransformableElement, ISvgSwitchElement
    {
        #region Constructors and Destructor

        public SvgSwitchElement(string prefix, string localname, string ns, SvgDocument doc)
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
        ///     This will always return <see cref="SvgRenderingHint.Containment" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Containment;

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private readonly SvgTests svgTests;
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

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