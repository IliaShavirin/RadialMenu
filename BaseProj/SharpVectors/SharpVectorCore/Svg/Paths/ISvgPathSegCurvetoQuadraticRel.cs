// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegCurvetoCubicRel interface corresponds to a "relative cubic Bezier curveto" (c) path data command.
    /// </summary>
    public interface ISvgPathSegCurvetoQuadraticRel : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
        double X1 { get; set; }
        double Y1 { get; set; }
    }
}