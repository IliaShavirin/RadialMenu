// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Many of the SVG DOM interfaces refer to objects of class SvgPoint.
    ///     An SvgPoint is an (x,y) coordinate pair. When used in matrix
    ///     operations, an SvgPoint is treated as a vector of the form:
    ///     [x]
    ///     [y]
    ///     [1]
    /// </summary>
    public sealed class SvgPoint : ISvgPoint
    {
        #region Constructor

        public SvgPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region Fields

        #endregion

        #region ISvgPoint Members

        public double X { get; set; }

        public double Y { get; set; }

        public ISvgPoint MatrixTransform(ISvgMatrix matrix)
        {
            return new SvgPoint(matrix.A * X + matrix.C * Y + matrix.E,
                matrix.B * X + matrix.D * Y + matrix.F);
        }

        #endregion

        #region Additional operators

        public SvgPoint lerp(SvgPoint that, double percent)
        {
            return new SvgPoint(
                X + (that.X - X) * percent,
                Y + (that.Y - Y) * percent
            );
        }

        public static SvgPoint operator +(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(
                a.X + b.X,
                a.Y + b.Y
            );
        }

        public static SvgPoint operator -(SvgPoint a, SvgPoint b)
        {
            return new SvgPoint(
                a.X - b.X,
                a.Y - b.Y
            );
        }

        public static SvgPoint operator *(SvgPoint a, double scalar)
        {
            return new SvgPoint(
                a.X * scalar,
                a.Y * scalar
            );
        }

        public static SvgPoint operator *(double scalar, SvgPoint a)
        {
            return new SvgPoint(
                scalar * a.X,
                scalar * a.Y
            );
        }

        public static SvgPoint operator /(SvgPoint a, double scalar)
        {
            return new SvgPoint(
                a.X / scalar,
                a.Y / scalar
            );
        }

        public static SvgPoint operator /(double scalar, SvgPoint a)
        {
            return new SvgPoint(
                scalar / a.X,
                scalar / a.Y
            );
        }

        #endregion
    }
}