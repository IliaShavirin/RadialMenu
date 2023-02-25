// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>

using System;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgMatrix.
    /// </summary>
    public sealed class SvgMatrix : ISvgMatrix
    {
        #region Public Fields

        public static readonly SvgMatrix Identity = new SvgMatrix();

        #endregion

        #region Private Fields

        private readonly bool _isIdentity;

        #endregion

        #region Additional operators

        public static SvgMatrix operator *(SvgMatrix a, SvgMatrix b)
        {
            return (SvgMatrix)a.Multiply(b);
        }

        #endregion

        #region Constructors

        public SvgMatrix()
            : this(1, 0, 0, 1, 0, 0)
        {
            _isIdentity = true;
        }

        public SvgMatrix(double a, double b, double c, double d, double e, double f)
        {
            A = a;
            B = b;
            C = c;
            D = d;
            E = e;
            F = f;

            _isIdentity = a == 1 && b == 0 && c == 0 && d == 1 && e == 0 && f == 0;
        }

        #endregion

        #region ISvgMatrix Members

        public bool IsIdentity
        {
            get
            {
                if (_isIdentity) return true;

                return A == 1 && B == 0 && C == 0 && D == 1 && E == 0 && F == 0;
            }
        }

        public double A { get; set; }

        public double B { get; set; }

        public double C { get; set; }

        public double D { get; set; }

        public double E { get; set; }

        public double F { get; set; }

        public ISvgMatrix Multiply(ISvgMatrix secondMatrix)
        {
            if (secondMatrix == null) secondMatrix = new SvgMatrix();

            var matrix = (SvgMatrix)secondMatrix;
            return new SvgMatrix(
                A * matrix.A + C * matrix.B,
                B * matrix.A + D * matrix.B,
                A * matrix.C + C * matrix.D,
                B * matrix.C + D * matrix.D,
                A * matrix.E + C * matrix.F + E,
                B * matrix.E + D * matrix.F + F
            );
        }

        public ISvgMatrix Inverse()
        {
            var det1 = A * D - B * C;

            if (det1 == 0)
                throw new SvgException(SvgExceptionType.SvgMatrixNotInvertable);

            var iDet = 1.0 / det1;
            var det2 = F * C - E * D;
            var det3 = E * B - F * A;

            return new SvgMatrix(
                D * iDet,
                -B * iDet,
                -C * iDet,
                A * iDet,
                det2 * iDet,
                det3 * iDet
            );
        }

        public ISvgMatrix Translate(double x, double y)
        {
            return new SvgMatrix(
                A,
                B,
                C,
                D,
                A * x + C * y + E,
                B * x + D * y + F
            );
        }

        public ISvgMatrix Scale(double scaleFactor)
        {
            return new SvgMatrix(
                A * scaleFactor,
                B * scaleFactor,
                C * scaleFactor,
                D * scaleFactor,
                E,
                F
            );
        }

        public ISvgMatrix ScaleNonUniform(double scaleFactorX, double scaleFactorY)
        {
            return new SvgMatrix(
                A * scaleFactorX,
                B * scaleFactorX,
                C * scaleFactorY,
                D * scaleFactorY,
                E,
                F
            );
        }

        public ISvgMatrix Rotate(double angle)
        {
            var radians = angle * (Math.PI / 180.0);
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            return new SvgMatrix(
                A * cos + C * sin,
                B * cos + D * sin,
                A * -sin + C * cos,
                B * -sin + D * cos,
                E,
                F
            );
        }

        public ISvgMatrix RotateFromVector(double x, double y)
        {
            if (x == 0 || y == 0)
                throw new SvgException(SvgExceptionType.SvgInvalidValueErr);

            var length = Math.Sqrt(x * x + y * y);
            var cos = x / length;
            var sin = y / length;

            return new SvgMatrix(
                A * cos + C * sin,
                B * cos + D * sin,
                A * -sin + C * cos,
                B * -sin + D * cos,
                E,
                F
            );
        }

        public ISvgMatrix FlipX()
        {
            return new SvgMatrix(
                -A,
                -B,
                C,
                D,
                E,
                F
            );
        }

        public ISvgMatrix FlipY()
        {
            return new SvgMatrix(
                A,
                B,
                -C,
                -D,
                E,
                F
            );
        }

        public ISvgMatrix SkewX(double angle)
        {
            var tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(
                A,
                B,
                A * tan + C,
                B * tan + D,
                E,
                F
            );
        }

        public ISvgMatrix SkewY(double angle)
        {
            var tan = Math.Tan(angle * (Math.PI / 180.0));

            return new SvgMatrix(
                A + C * tan,
                B + D * tan,
                C,
                D,
                E,
                F
            );
        }

        #endregion
    }
}