using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegLinetoAbs : SvgPathSegLineto, ISvgPathSegLinetoAbs
    {
        public SvgPathSegLinetoAbs(double x, double y)
            : base(SvgPathSegType.LineToAbs)
        {
            X = x;
            Y = y;
        }

        public override SvgPointF AbsXY => new SvgPointF(X, Y);

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