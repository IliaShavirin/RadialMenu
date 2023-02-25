using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Text;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Text
{
    /// <summary>
    ///     Summary description for SvgTRefElement.
    /// </summary>
    public sealed class SvgTRefElement : SvgTextPositioningElement, ISvgTRefElement
    {
        private readonly SvgUriReference svgURIReference;

        public SvgTRefElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
        }

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => svgURIReference.Href;

        public XmlElement ReferencedElement => svgURIReference.ReferencedNode as XmlElement;

        #endregion
    }
}