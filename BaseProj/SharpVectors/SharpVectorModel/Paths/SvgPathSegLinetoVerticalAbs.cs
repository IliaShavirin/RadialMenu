using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegLinetoVerticalAbs : SvgPathSegLineto, ISvgPathSegLinetoVerticalAbs
    {
        internal SvgPathSegLinetoVerticalAbs(double y)
            : base(SvgPathSegType.LineToVerticalAbs)
        {
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
                return new SvgPointF(prevPoint.X, Y);
            }
        }

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(Y);

                return sb.ToString();
            }
        }

        public double Y { get; set; }
    }
}