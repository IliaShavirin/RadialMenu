using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     Summary description for SvgPathSegCurvetoCubicAbs.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticRel : SvgPathSegCurvetoQuadratic, ISvgPathSegCurvetoQuadraticRel
    {
        #region constructors

        public SvgPathSegCurvetoQuadraticRel(double x, double y, double x1, double y1)
            : base(SvgPathSegType.CurveToQuadraticRel)
        {
            X = x;
            Y = y;
            X1 = x1;
            Y1 = y1;
        }

        #endregion

        #region SvgPathSegCurvetoQuadraticRel Members

        public double X { get; set; }

        public double Y { get; set; }

        public double X1 { get; set; }

        public double Y1 { get; set; }

        #endregion

        #region Pubic Methods

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

        public override SvgPointF QuadraticX1Y1
        {
            get
            {
                var prevSeg = PreviousSeg;
                SvgPointF prevPoint;
                if (prevSeg == null) prevPoint = new SvgPointF(0, 0);
                else prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X1, prevPoint.Y + Y1);
            }
        }

        public override SvgPointF CubicX1Y1
        {
            get
            {
                var prevPoint = PreviousSeg.AbsXY;

                var x1 = prevPoint.X + X1 * 2 / 3;
                var y1 = prevPoint.Y + Y1 * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                var prevPoint = PreviousSeg.AbsXY;

                var x2 = X1 + prevPoint.X + (X - X1) / 3;
                var y2 = Y1 + prevPoint.Y + (Y - Y1) / 3;

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