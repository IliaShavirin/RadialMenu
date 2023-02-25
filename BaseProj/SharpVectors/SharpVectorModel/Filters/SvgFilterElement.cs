using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Filters;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Filters
{
    /// <summary>
    ///     Summary description for SvgFilterElement.
    /// </summary>
    public sealed class SvgFilterElement : SvgStyleableElement, ISvgFilterElement
    {
        #region Constructors

        internal SvgFilterElement(string prefix, string localname, string ns, SvgDocument doc) : base(prefix, localname,
            ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        #region ISvgFilterElement Interface

        #region ISvgFilterElement Properties

        public ISvgAnimatedEnumeration FilterUnits => null;

        public ISvgAnimatedEnumeration PrimitiveUnits => null;

        public ISvgAnimatedLength X => null;

        public ISvgAnimatedLength Y => null;

        public ISvgAnimatedLength Width => null;

        public ISvgAnimatedLength Height => null;

        public ISvgAnimatedInteger FilterResX => null;

        public ISvgAnimatedInteger FilterResY => null;

        #endregion

        #region ISvgFilterElement Methods

        public void SetFilterRes(ulong filterResX, ulong filterResY)
        {
        }

        #endregion

        #endregion

        #region Implementation of ISvgURIReference

        private readonly SvgUriReference svgURIReference;

        public ISvgAnimatedString Href => svgURIReference.Href;

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion
    }
}