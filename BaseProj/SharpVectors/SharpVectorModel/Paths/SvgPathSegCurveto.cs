using System;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Polynomials;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public abstract class SvgPathSegCurveto : SvgPathSeg
    {
        #region constructors

        protected SvgPathSegCurveto(SvgPathSegType type) : base(type)
        {
        }

        #endregion

        protected abstract SqrtPolynomial getArcLengthPolynomial();

        #region abstract properties

        public abstract override SvgPointF AbsXY { get; }
        public abstract SvgPointF CubicX1Y1 { get; }
        public abstract SvgPointF CubicX2Y2 { get; }

        #endregion

        #region public properties

        public override double Length => getArcLengthPolynomial().Simpson(0, 1);

        public override double StartAngle
        {
            get
            {
                var p1 = PreviousSeg.AbsXY;
                var p2 = CubicX1Y1;

                double dx = p2.X - p1.X;
                double dy = p2.Y - p1.Y;
                var a = Math.Atan2(dy, dx) * 180 / Math.PI;
                a += 270;
                a %= 360;
                return a;
            }
        }

        public override double EndAngle
        {
            get
            {
                var p1 = CubicX2Y2;
                var p2 = AbsXY;

                double dx = p1.X - p2.X;
                double dy = p1.Y - p2.Y;
                var a = Math.Atan2(dy, dx) * 180 / Math.PI;
                a += 270;
                a %= 360;
                return a;
            }
        }

        #endregion
    }
}