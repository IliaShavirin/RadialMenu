// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegCurvetoCubicAbs interface corresponds to an
    ///     "absolute cubic Bezier curveto" (C) path data command.
    /// </summary>
    public interface ISvgPathSegCurvetoCubicAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
        double X1 { get; set; }
        double Y1 { get; set; }
        double X2 { get; set; }
        double Y2 { get; set; }
    }
}