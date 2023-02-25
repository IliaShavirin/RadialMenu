// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoRel interface corresponds to an "relative lineto" (l) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoRel : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
    }
}