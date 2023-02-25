using System;

namespace BaseProj.SharpVectors.SharpVectorCore.Utils.Polynomials
{
    /// <summary>
    ///     Summary description for Polynomial.
    /// </summary>
    /// <developer>kevin@kevlindev.com</developer>
    /// <completed>100</completed>
    public class Polynomial
    {
        #region class methods

        /// <summary>
        ///     Interpolate - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        /// <param name="n"></param>
        /// <param name="offset"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static ValueWithError Interpolate(double[] xs, double[] ys, int n, int offset, double x)
        {
            double y;
            var dy = 0.0;
            var c = new double[n];
            var d = new double[n];
            var ns = 0;

            var diff = Math.Abs(x - xs[offset]);
            for (var i = 0; i < n; i++)
            {
                var dift = Math.Abs(x - xs[offset + i]);

                if (dift < diff)
                {
                    ns = i;
                    diff = dift;
                }

                c[i] = d[i] = ys[offset + i];
            }

            y = ys[offset + ns];
            ns--;

            for (var m = 1; m < n; m++)
            {
                for (var i = 0; i < n - m; i++)
                {
                    var ho = xs[offset + i] - x;
                    var hp = xs[offset + i + m] - x;
                    var w = c[i + 1] - d[i];
                    var den = ho - hp;

                    if (den == 0.0) return new ValueWithError(0, 0);

                    den = w / den;
                    d[i] = hp * den;
                    c[i] = ho * den;
                }

                dy = 2 * (ns + 1) < n - m ? c[ns + 1] : d[ns--];
                y += dy;
            }

            return new ValueWithError(y, dy);
        }

        #endregion

        #region protected methods

        /// <summary>
        ///     trapezoid - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        protected double trapezoid(double min, double max, int n)
        {
            var range = max - min;

            if (n == 1)
            {
                s = 0.5 * range * (Evaluate(min) + Evaluate(max));
            }
            else
            {
                var it = 1 << (n - 2);
                var delta = range / it;
                var x = min + 0.5 * delta;
                var sum = 0.0;

                for (var i = 0; i < it; i++)
                {
                    sum += Evaluate(x);
                    x += delta;
                }

                s = 0.5 * (s + range * sum / it);
            }

            return s;
        }

        #endregion

        #region fields

        private readonly double[] coefficients;
        private double s;

        #endregion

        #region properties

        public int Degree => coefficients.Length - 1;

        public double this[int index] => coefficients[index];

        #endregion

        #region constructors

        /// <summary>
        ///     Polynomial constuctor
        /// </summary>
        /// <param name="coefficients"></param>
        public Polynomial(params double[] coefficients)
        {
            var end = 0;
            var TOLERANCE = 1e-9;

            for (end = coefficients.Length; end > 0; end--)
                if (Math.Abs(coefficients[end - 1]) > TOLERANCE)
                    break;

            if (end > 0)
            {
                this.coefficients = new double[coefficients.Length - (coefficients.Length - end)];
                for (var i = 0; i < end; i++) this.coefficients[i] = coefficients[i];
            }
            else
            {
                this.coefficients = new double[0];
            }
        }

        public Polynomial(Polynomial that)
        {
            coefficients = that.coefficients;
        }

        #endregion

        #region public methods

        /// <summary>
        ///     Evaluate
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public virtual double Evaluate(double t)
        {
            var result = 0.0;

            for (var i = coefficients.Length - 1; i >= 0; i--) result = result * t + coefficients[i];

            return result;
        }

        /// <summary>
        ///     Simspon - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Simpson(double min, double max)
        {
            var s = 0.0;
            var st = 0.0;
            var os = 0.0;
            var ost = 0.0;
            var MAX = 20;
            var TOLERANCE = 1e-7;

            for (var j = 1; j <= MAX; j++)
            {
                st = trapezoid(min, max, j);
                s = (4.0 * st - ost) / 3.0;
                if (Math.Abs(s - os) < TOLERANCE * Math.Abs(os)) break;
                os = s;
                ost = st;
            }

            return s;
        }

        /// <summary>
        ///     Romberg - adapted from "Numerical Recipes in C"
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public double Romberg(double min, double max)
        {
            var MAX = 20;
            var TOLERANCE = 1e-7;
            var K = 4;
            var s = new double[MAX + 1];
            var h = new double[MAX + 1];
            var result = new ValueWithError(0, 0);

            h[0] = 1.0;
            for (var j = 1; j <= MAX; j++)
            {
                s[j - 1] = trapezoid(min, max, j);
                if (j >= K)
                {
                    result = Interpolate(h, s, K, j - K, 0.0);
                    if (Math.Abs(result.Error) < TOLERANCE * result.Value) break;
                }

                s[j] = s[j - 1];
                h[j] = 0.25 * h[j - 1];
            }

            return result.Value;
        }

        #endregion
    }
}