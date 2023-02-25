using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgColorProfileElement : SvgElement, ISvgColorProfileElement
    {
        #region Constructors and Destructors

        public SvgColorProfileElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            UriReference = new SvgUriReference(this);
        }

        #endregion

        #region ISvgColorProfileElement Members

        public string Local => string.Empty;

        #endregion

        #region Private Fields

        #endregion

        #region ISvgURIReference Members

        public ISvgAnimatedString Href => UriReference.Href;

        public SvgUriReference UriReference { get; }

        public XmlElement ReferencedElement => UriReference.ReferencedNode as XmlElement;

        #endregion
    }
}