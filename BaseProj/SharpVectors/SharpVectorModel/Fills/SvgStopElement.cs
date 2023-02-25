using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Fills;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Fills
{
    /// <summary>
    ///     Summary description for SvgStopElement.
    /// </summary>
    public sealed class SvgStopElement : SvgStyleableElement, ISvgStopElement
    {
        public SvgStopElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }

        public ISvgAnimatedNumber Offset
        {
            get
            {
                var attr = GetAttribute("offset").Trim();
                if (attr.EndsWith("%"))
                {
                    attr = attr.TrimEnd('%');
                }
                else
                {
                    var tmp = SvgNumber.ParseNumber(attr) * 100;
                    attr = tmp.ToString(SvgNumber.Format);
                }

                return new SvgAnimatedNumber(attr);
            }
        }
    }
}