using BaseProj.SharpVectors.SharpVectorCore.Svg.Text;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Text
{
    /// <summary>
    ///     Summary description for SvgTextElement.
    /// </summary>
    public sealed class SvgTextElement : SvgTextPositioningElement, ISvgTextElement
    {
        public SvgTextElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }
    }
}