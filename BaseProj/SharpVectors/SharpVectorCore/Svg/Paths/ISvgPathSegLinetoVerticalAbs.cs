// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoVerticalAbs interface corresponds to an "absolute vertical lineto" (V) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoVerticalAbs : ISvgPathSeg
    {
        double Y { get; set; }
    }
}