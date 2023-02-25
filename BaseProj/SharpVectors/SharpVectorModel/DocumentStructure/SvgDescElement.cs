using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    /// <summary>
    ///     The SvgDescElement interface corresponds to the 'desc' element.
    /// </summary>
    public sealed class SvgDescElement : SvgStyleableElement, ISvgDescElement
    {
        public SvgDescElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }
    }
}