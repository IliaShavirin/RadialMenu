// <developer>niklas@protocol7.com</developer>
// <completed>99</completed>

using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    /// <summary>
    ///     The SvgGElement interface corresponds to the 'g' element.
    /// </summary>
    public interface ISvgGElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable, IEventTarget
    {
    }
}