// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoHorizontalAbs interface corresponds to an "absolute horizontal lineto" (H) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoHorizontalAbs : ISvgPathSeg
    {
        double X { get; set; }
    }
}