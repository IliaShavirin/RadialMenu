// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegCurvetoQuadraticAbs interface corresponds to an "absolute quadratic Bezier curveto" (Q) path data
    ///     command.
    /// </summary>
    public interface ISvgPathSegCurvetoQuadraticAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
        double X1 { get; set; }
        double Y1 { get; set; }
    }
}