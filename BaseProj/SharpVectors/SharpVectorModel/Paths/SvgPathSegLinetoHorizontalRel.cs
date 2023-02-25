using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegLinetoHorizontalRel : SvgPathSegLineto, ISvgPathSegLinetoHorizontalRel
    {
        public SvgPathSegLinetoHorizontalRel(double x)
            : base(SvgPathSegType.LineToHorizontalRel)
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
                return new SvgPointF(prevPoint.X + X, prevPoint.Y);
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