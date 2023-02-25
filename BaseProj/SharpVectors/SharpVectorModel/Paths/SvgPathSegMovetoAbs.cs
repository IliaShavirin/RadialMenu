using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegMovetoAbs : SvgPathSegMoveto, ISvgPathSegMovetoAbs
    {
        public SvgPathSegMovetoAbs(double x, double y)
            : base(SvgPathSegType.MoveToAbs, x, y)
        {
        }

        public override SvgPointF AbsXY => new SvgPointF(X, Y);
    }
}