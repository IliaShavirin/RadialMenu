// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    /// <summary>
    ///     The SvgSwitchElement interface corresponds to the 'switch' element.
    /// </summary>
    public interface ISvgSwitchElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable, IEventTarget
    {
    }
}