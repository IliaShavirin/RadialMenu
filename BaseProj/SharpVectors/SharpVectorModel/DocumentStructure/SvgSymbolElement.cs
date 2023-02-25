using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgSymbolElement : SvgStyleableElement, ISvgSymbolElement
    {
        #region Constructors and Destructor

        internal SvgSymbolElement(string prefix, string localname, string ns, SvgDocument doc) : base(prefix, localname,
            ns, doc)
        {
            svgExternalResourcesRequired = new SvgExternalResourcesRequired(this);
            svgFitToViewBox = new SvgFitToViewBox(this);
        }

        #endregion

        #region ISvgExternalResourcesRequired Members

        public ISvgAnimatedBoolean ExternalResourcesRequired => svgExternalResourcesRequired.ExternalResourcesRequired;

        #endregion

        #region Private Fields

        private readonly SvgFitToViewBox svgFitToViewBox;
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
        public override bool IsRenderable
        {
            get
            {
                var parentNode = ParentNode;
                if (parentNode != null && string.Equals(parentNode.LocalName, "use")) return true;

                return false;
            }
        }

        /// <summary>
        ///     Gets a value providing a hint on the rendering defined by this element.
        /// </summary>
        /// <value>
        ///     An enumeration of the <see cref="SvgRenderingHint" /> specifying the rendering hint.
        ///     This will always return <see cref="SvgRenderingHint.Containment" />
        /// </value>
        public override SvgRenderingHint RenderingHint => SvgRenderingHint.Containment;

        #endregion

        #region ISvgFitToViewBox Members

        public ISvgAnimatedRect ViewBox => svgFitToViewBox.ViewBox;

        public ISvgAnimatedPreserveAspectRatio PreserveAspectRatio => svgFitToViewBox.PreserveAspectRatio;

        #endregion
    }
}