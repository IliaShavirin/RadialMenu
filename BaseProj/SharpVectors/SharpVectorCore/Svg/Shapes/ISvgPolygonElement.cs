using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Shapes
{
    /// <summary>
    ///     The SvgPolygonElement interface corresponds to the 'polygon' element
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>100</completed>
    public interface ISvgPolygonElement :
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