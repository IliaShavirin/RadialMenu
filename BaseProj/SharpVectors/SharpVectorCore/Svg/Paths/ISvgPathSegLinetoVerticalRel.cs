// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoVerticalRel interface corresponds to a "relative vertical lineto" (v) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoVerticalRel : ISvgPathSeg
    {
        double Y { get; set; }
    }
}