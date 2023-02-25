using BaseProj.SharpVectors.SharpVectorCore.Svg.Metadata;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Metadata
{
    public sealed class SvgMetadataElement : SvgElement, ISvgMetadataElement
    {
        internal SvgMetadataElement(string prefix, string localname, string ns, SvgDocument doc)
            : base(prefix, localname, ns, doc)
        {
        }
    }
}