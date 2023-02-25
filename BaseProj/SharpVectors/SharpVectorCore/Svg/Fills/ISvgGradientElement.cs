using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Fills
{
    /// <summary>
    ///     The SvgGradientElement interface is a base interface used by SvgLinearGradientElement and SvgRadialGradientElement.
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>20</completed>
    public interface ISvgGradientElement :
        ISvgElement,
        ISvgUriReference,
        ISvgExternalResourcesRequired,
        ISvgStylable
    {
        ISvgAnimatedEnumeration GradientUnits { get; }
        ISvgAnimatedTransformList GradientTransform { get; }
        ISvgAnimatedEnumeration SpreadMethod { get; }
    }
}