// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgAnimatedPathData interface supports elements which have a 'd' attribute which holds Svg path data, and
    ///     supports the ability to animate that attribute.
    /// </summary>
    public interface ISvgAnimatedPathData
    {
        ISvgPathSegList PathSegList { get; }
        ISvgPathSegList NormalizedPathSegList { get; }
        ISvgPathSegList AnimatedPathSegList { get; }
        ISvgPathSegList AnimatedNormalizedPathSegList { get; }
    }
}