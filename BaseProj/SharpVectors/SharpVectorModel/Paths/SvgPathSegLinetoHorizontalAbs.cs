using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegLinetoHorizontalAbs : SvgPathSegLineto, ISvgPathSegLinetoHorizontalAbs
    {
        public SvgPathSegLinetoHorizontalAbs(double x)
            : base(SvgPathSegType.LineToHorizontalAbs)
        {
            X = x;
        }

        public override SvgPointF AbsXY
        {
            get
            {
                var prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(X, prevPoint.Y);
            }
        }

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X);

                return sb.ToString();
            }
        }

        public double X { get; set; }
    }
}