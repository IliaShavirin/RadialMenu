using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public sealed class SvgPathSegCurvetoCubicAbs : SvgPathSegCurvetoCubic, ISvgPathSegCurvetoCubicAbs
    {
        #region constructors

        internal SvgPathSegCurvetoCubicAbs(double x, double y, double x1, double y1, double x2, double y2)
            : base(SvgPathSegType.CurveToCubicAbs)
        {
            X = x;
            Y = y;
            X1 = x1;
            Y1 = y1;
            X2 = x2;
            Y2 = y2;
        }

        #endregion

        #region ISvgPathSegCurvtoCubicAbs Members

        public double X { get; set; }

        public double Y { get; set; }

        public double X1 { get; set; }

        public double Y1 { get; set; }

        public double X2 { get; set; }

        public double Y2 { get; set; }

        #endregion

        #region Public Methods

        public override SvgPointF AbsXY => new SvgPointF(X, Y);

        public override SvgPointF CubicX1Y1 => new SvgPointF(X1, Y1);

        public override SvgPointF CubicX2Y2 => new SvgPointF(X2, Y2);

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