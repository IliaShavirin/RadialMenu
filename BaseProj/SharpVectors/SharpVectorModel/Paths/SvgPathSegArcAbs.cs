using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     Summary description for SvgPathSegLinetoAbs.
    /// </summary>
    public sealed class SvgPathSegArcAbs : SvgPathSegArc, ISvgPathSegArcAbs
    {
        internal SvgPathSegArcAbs(double x, double y, double r1, double r2, double angle,
            bool largeArcFlag, bool sweepFlag) :
            base(SvgPathSegType.ArcAbs, x, y, r1, r2, angle, largeArcFlag, sweepFlag)
        {
        }

        public override SvgPointF AbsXY => new SvgPointF(X, Y);
    }
}