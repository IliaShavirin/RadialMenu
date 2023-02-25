// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>


namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Paths
{
    /// <summary>
    ///     The SvgPathSegArcAbs interface corresponds to an "absolute arcto" (A) path data command.
    /// </summary>
    public interface ISvgPathSegArcAbs : ISvgPathSeg
    {
        double X { get; set; }
        double Y { get; set; }
        double R1 { get; set; }
        double R2 { get; set; }
        double Angle { get; set; }
        bool LargeArcFlag { get; set; }
        bool SweepFlag { get; set; }
    }
}