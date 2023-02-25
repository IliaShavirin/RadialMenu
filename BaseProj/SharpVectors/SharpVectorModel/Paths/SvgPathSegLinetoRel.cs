using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegLinetoRel : SvgPathSegLineto, ISvgPathSegLinetoRel
    {
        public SvgPathSegLinetoRel(double x, double y)
            : base(SvgPathSegType.LineToRel)
        {
            X = x;
            Y = y;
        }

        public override SvgPointF AbsXY
        {
            get
            {
                var prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;
                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        public double X { get; set; }

        public double Y { get; set; }
    }
}