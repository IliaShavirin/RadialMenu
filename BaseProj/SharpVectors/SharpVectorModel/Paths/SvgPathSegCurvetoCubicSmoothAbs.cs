using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegCurvetoCubicSmoothAbs : SvgPathSegCurvetoCubic, ISvgPathSegCurvetoCubicSmoothAbs
    {
        #region constructors

        internal SvgPathSegCurvetoCubicSmoothAbs(double x, double y, double x2, double y2)
            : base(SvgPathSegType.CurveToCubicSmoothAbs)
        {
            X = x;
            Y = y;
            X2 = x2;
            Y2 = y2;
        }

        #endregion

        #region ISvgPathSegCurvetoCubicSmoothAbs Members

        public double X { get; set; }

        public double Y { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        #endregion

        #region Public Methods

        public override SvgPointF AbsXY => new SvgPointF(X, Y);

        public override SvgPointF CubicX1Y1
        {
            get
            {
                var prevSeg = PreviousSeg;
                if (prevSeg == null || !(prevSeg is SvgPathSegCurvetoCubic)) return prevSeg.AbsXY;

                var prevXY = prevSeg.AbsXY;
                var prevX2Y2 = ((SvgPathSegCurvetoCubic)prevSeg).CubicX2Y2;

                return new SvgPointF(2 * prevXY.X - prevX2Y2.X, 2 * prevXY.Y - prevX2Y2.Y);
            }
        }

        public override SvgPointF CubicX2Y2 => new SvgPointF(X2, Y2);

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(X2);
                sb.Append(",");
                sb.Append(Y2);
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