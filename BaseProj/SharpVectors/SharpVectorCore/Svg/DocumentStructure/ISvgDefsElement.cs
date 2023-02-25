// <developer>niklas@protocol7.com</developer>
// <completed>99</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    /// <summary>
    ///     The SvgDefsElement interface corresponds to the 'defs' element.
    /// </summary>
    public interface ISvgDefsElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable
    {
    }
}