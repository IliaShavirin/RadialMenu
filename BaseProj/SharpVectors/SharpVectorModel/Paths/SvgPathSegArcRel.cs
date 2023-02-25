using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     Summary description for SvgPathSegLinetoAbs.
    /// </summary>
    public sealed class SvgPathSegArcRel : SvgPathSegArc, ISvgPathSegArcRel
    {
        internal SvgPathSegArcRel(double x, double y, double r1, double r2, double angle,
            bool largeArcFlag, bool sweepFlag)
            : base(SvgPathSegType.ArcRel, x, y, r1, r2, angle, largeArcFlag, sweepFlag)
        {
        }

        public override SvgPointF AbsXY
        {
            get
            {
                var prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null)
                    prevPoint = new SvgPointF(0, 0);
                else
                    prevPoint =
                        prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }
    }
}