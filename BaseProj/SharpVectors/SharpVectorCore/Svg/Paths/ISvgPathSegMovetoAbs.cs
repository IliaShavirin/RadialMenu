// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegMovetoAbs interface corresponds to an "absolute moveto" (M) path data command.
    /// </summary>
    public interface ISvgPathSegMovetoAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
    }
}