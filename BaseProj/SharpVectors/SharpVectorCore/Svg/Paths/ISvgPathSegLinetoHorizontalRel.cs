// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegLinetoHorizontalRel interface corresponds to a "relative horizontal lineto" (h) path data command.
    /// </summary>
    public interface ISvgPathSegLinetoHorizontalRel : ISvgPathSeg
    {
        double X { get; set; }
    }
}