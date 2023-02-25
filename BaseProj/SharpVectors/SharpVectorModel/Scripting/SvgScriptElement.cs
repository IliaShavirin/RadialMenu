using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Scripting;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Scripting
{
    /// <summary>
    ///     The SVGScriptElement interface corresponds to the 'script' element.
    /// </summary>
    public sealed class SvgScriptElement : SvgElement, ISvgScriptElement
    {
        private readonly SvgExternalResourcesRequired svgExternalResourcesRequired;
        private readonly SvgUriReference svgURIReference;

        #region Constructors

        public SvgScriptElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
        }

        #endregion

        public string Type
        {
            get => GetAttribute("type");
            set => SetAttribute("type", value);
        }

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => svgURIReference.Href;

        #endregion

        #region Implementation of ISvgExternalResourcesRequired

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion
    }
}