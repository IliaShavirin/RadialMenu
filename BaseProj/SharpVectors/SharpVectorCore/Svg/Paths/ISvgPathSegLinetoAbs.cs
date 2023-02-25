// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoAbs interface corresponds to an "absolute lineto" (L) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
    }
}