using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Shapes
{
    /// <summary>
    ///     The SvgPolylineElement interface corresponds to the 'polyline' element
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>100</completed>
    public interface ISvgPolylineElement :
        ISvgElement,
        ISvgTests,
        ISvgLangSpace,
        ISvgExternalResourcesRequired,
        ISvgStylable,
        ISvgTransformable,
        ISvgAnimatedPoints,
        IEventTarget
    {
    }
}