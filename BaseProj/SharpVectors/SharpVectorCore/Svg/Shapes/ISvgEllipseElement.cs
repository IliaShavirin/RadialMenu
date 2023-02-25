using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Shapes
{
    /// <summary>
    ///     The SvgEllipseElement interface corresponds to the 'ellipse' element.
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>100</completed>
    public interface ISvgEllipseElement :
        ISvgElement,
        ISvgTests,
        ISvgLangSpace,
        ISvgExternalResourcesRequired,
        ISvgStylable,
        ISvgTransformable,
        IEventTarget
    {
        ISvgAnimatedLength Cx { get; }

        ISvgAnimatedLength Cy { get; }

        ISvgAnimatedLength Rx { get; }

        ISvgAnimatedLength Ry { get; }
    }
}