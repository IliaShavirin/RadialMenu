// <developer>niklas@protocol7.com</developer>
// <completed>80</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    ///     Interface SvgFitToViewBox defines DOM attributes that apply to
    ///     elements which have XML attributes viewBox and
    ///     preserveAspectRatio.
    /// </summary>
    public interface ISvgFitToViewBox
    {
        ISvgAnimatedRect ViewBox { get; }
        ISvgAnimatedPreserveAspectRatio PreserveAspectRatio { get; }
    }
}