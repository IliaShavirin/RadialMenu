using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Text;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Text
{
    /// <summary>
    ///     The implementation of the ISvgTextPathElement interface corresponds to the 'textPath' element.
    /// </summary>
    public sealed class SvgTextPathElement : SvgTextContentElement, ISvgTextPathElement
    {
        #region Private Fields

        private readonly SvgUriReference svgURIReference;

        #endregion

        #region Constructors and Destructor

        public SvgTextPathElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
            svgURIReference = new SvgUriReference(this);
        }

        #endregion

        #region ISvgTextPathElement Members

        public ISvgAnimatedLength StartOffset =>
            new SvgAnimatedLength(this, "startOffset",
                SvgLengthDirection.Horizontal, "0%");

        public ISvgAnimatedEnumeration Method
        {
            get
            {
                var pathMethod = SvgTextPathMethod.Align;
                if (GetAttribute("method") == "stretch") pathMethod = SvgTextPathMethod.Stretch;

                return new SvgAnimatedEnumeration((ushort)pathMethod);
            }
        }

        public ISvgAnimatedEnumeration Spacing
        {
            get
            {
                var pathSpacing = SvgTextPathSpacing.Exact;
                if (GetAttribute("spacing") == "auto") pathSpacing = SvgTextPathSpacing.Auto;

                return new SvgAnimatedEnumeration((ushort)pathSpacing);
            }
        }

        #endregion

        #region ISvgUriReference Members

        public ISvgAnimatedString Href => svgURIReference.Href;

        public XmlElement ReferencedElement => svgURIReference.ReferencedNode as XmlElement;

        #endregion
    }
}