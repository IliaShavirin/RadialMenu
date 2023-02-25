using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Filters
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgFESpotLightElement :
        ISvgElement
    {
        ISvgAnimatedNumber X { get; }
        ISvgAnimatedNumber Y { get; }
        ISvgAnimatedNumber Z { get; }
        ISvgAnimatedNumber PointsAtX { get; }
        ISvgAnimatedNumber PointsAtY { get; }
        ISvgAnimatedNumber PointsAtZ { get; }
        ISvgAnimatedNumber SpecularExponent { get; }
        ISvgAnimatedNumber LimitingConeAngle { get; }
    }
}