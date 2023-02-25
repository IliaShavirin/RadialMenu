using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     Summary description for SvgPathSegCurvetoCubicAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticAbs : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticAbs
    {
        #region constructors

        public SvgPathSegCurvetoQuadraticAbs(double x, double y, double x1, double y1)
            : base(SvgPathSegType.CurveToQuadraticAbs)
        {
            X = x;
            Y = y;
            X1 = x1;
            Y1 = y1;
        }

        #endregion

        #region ISvgPathSegCurvetoQuadraticAbs Members

        public double X { get; set; }

        public double Y { get; set; }

        public double X1 { get; set; }

        public double Y1 { get; set; }

        #endregion

        #region Public Methods

        public override SvgPointF AbsXY => new SvgPointF(X, Y);

        public override SvgPointF QuadraticX1Y1 => new SvgPointF(X1, Y1);

        /*
        * Convert to cubic bezier using the algorithm from Math:Bezier:Convert in CPAN
        * $p0x+($p1x-$p0x)*2/3
        * $p0y+($p1y-$p0y)*2/3
        * $p1x+($p2x-$p1x)/3
        * $p1x+($p2x-$p1x)/3
        * */

        public override SvgPointF CubicX1Y1
        {
            get
            {
                var prevPoint = PreviousSeg.AbsXY;

                var x1 = prevPoint.X + (X1 - prevPoint.X) * 2 / 3;
                var y1 = prevPoint.Y + (Y1 - prevPoint.Y) * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                var x2 = X1 + (X - X1) / 3;
                var y2 = Y1 + (Y - Y1) / 3;

                return new SvgPointF(x2, y2);
            }
        }

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X1);
                sb.Append(",");
                sb.Append(Y1);
                sb.Append(",");
                sb.Append(X);
                sb.Append(",");
                sb.Append(Y);

                return sb.ToString();
            }
        }

        #endregion
    }
}