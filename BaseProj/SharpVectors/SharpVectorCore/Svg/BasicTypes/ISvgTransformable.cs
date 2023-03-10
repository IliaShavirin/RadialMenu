// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    ///     Interface SvgTransformable contains properties and methods that apply to all elements which have attribute
    ///     transform.
    /// </summary>
    public interface ISvgTransformable : ISvgLocatable
    {
        ISvgAnimatedTransformList Transform { get; }
    }
}