using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     The SvgPathSegCurvetoQuadraticSmoothRel interface corresponds to an "relative smooth quadratic curveto" (t) path
    ///     data command.
    /// </summary>
    public sealed class SvgPathSegCurvetoQuadraticSmoothRel : SvgPathSegCurvetoQuadratic,
        ISvgPathSegCurvetoQuadraticSmoothRel
    {
        #region Constructors

        public SvgPathSegCurvetoQuadraticSmoothRel(double x, double y)
            : base(SvgPathSegType.CurveToQuadraticSmoothRel)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region SvgpathSegCurvetoQuadraticSmoothRel Members

        /// <summary>
        ///     The absolute X coordinate for the end point of this path segment.
        /// </summary>
        public double X { get; set; }

        /// <summary>
        ///     The absolute Y coordinate for the end point of this path segment.
        /// </summary>
        public double Y { get; set; }

        #endregion

        #region Public Methods

        public override SvgPointF QuadraticX1Y1
        {
            get
            {
                var prevSeg = PreviousSeg;
                if (prevSeg == null || !(prevSeg is SvgPathSegCurvetoQuadratic)) return prevSeg.AbsXY;

                var prevXY = prevSeg.AbsXY;
                var prevX1Y1 = ((SvgPathSegCurvetoQuadratic)prevSeg).QuadraticX1Y1;

                return new SvgPointF(2 * prevXY.X - prevX1Y1.X, 2 * prevXY.Y - prevX1Y1.Y);
            }
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
                    prevPoint = prevSeg.AbsXY;

                return new SvgPointF(prevPoint.X + X, prevPoint.Y + Y);
            }
        }

        public override SvgPointF CubicX1Y1
        {
            get
            {
                var prevPoint = PreviousSeg.AbsXY;
                var x1y1 = QuadraticX1Y1;

                double x1 = prevPoint.X + (x1y1.X - prevPoint.X) * 2 / 3;
                double y1 = prevPoint.Y + (x1y1.Y - prevPoint.Y) * 2 / 3;

                return new SvgPointF(x1, y1);
            }
        }

        public override SvgPointF CubicX2Y2
        {
            get
            {
                var xy = AbsXY;
                var x1y1 = QuadraticX1Y1;
                double x2 = x1y1.X + (xy.X - x1y1.X) / 3;
                double y2 = x1y1.Y + (xy.Y - x1y1.Y) / 3;

                return new SvgPointF(x2, y2);
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

        #endregion
    }
}