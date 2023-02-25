using System;
using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public abstract class SvgPathSegArc : SvgPathSeg
    {
        #region Constructors and Destructor

        protected SvgPathSegArc(SvgPathSegType type, double x, double y, double r1, double r2,
            double angle, bool largeArcFlag, bool sweepFlag)
            : base(type)
        {
            X = x;
            Y = y;
            R1 = r1;
            R2 = r2;
            Angle = angle;
            LargeArcFlag = largeArcFlag;
            SweepFlag = sweepFlag;
        }

        #endregion

        #region Public Methods

        public CalculatedArcValues GetCalculatedArcValues()
        {
            var calcVal = new CalculatedArcValues();

            /*
             *	This algorithm is taken from the Batik source. All cudos to the Batik crew.
             */

            var startPoint = PreviousSeg.AbsXY;
            var endPoint = AbsXY;

            double x0 = startPoint.X;
            double y0 = startPoint.Y;

            double x = endPoint.X;
            double y = endPoint.Y;

            // Compute the half distance between the current and the final point
            var dx2 = (x0 - x) / 2.0;
            var dy2 = (y0 - y) / 2.0;

            // Convert angle from degrees to radians
            var radAngle = Angle * Math.PI / 180;
            var cosAngle = Math.Cos(radAngle);
            var sinAngle = Math.Sin(radAngle);

            //
            // Step 1 : Compute (x1, y1)
            //
            var x1 = cosAngle * dx2 + sinAngle * dy2;
            var y1 = -sinAngle * dx2 + cosAngle * dy2;
            // Ensure radii are large enough

            var rx = Math.Abs(R1);
            var ry = Math.Abs(R2);

            var Prx = rx * rx;
            var Pry = ry * ry;
            var Px1 = x1 * x1;
            var Py1 = y1 * y1;

            // check that radii are large enough
            var radiiCheck = Px1 / Prx + Py1 / Pry;
            if (radiiCheck > 1)
            {
                rx = Math.Sqrt(radiiCheck) * rx;
                ry = Math.Sqrt(radiiCheck) * ry;
                Prx = rx * rx;
                Pry = ry * ry;
            }

            //
            // Step 2 : Compute (cx1, cy1)
            //
            double sign = LargeArcFlag == SweepFlag ? -1 : 1;
            var sq = (Prx * Pry - Prx * Py1 - Pry * Px1) / (Prx * Py1 + Pry * Px1);
            sq = sq < 0 ? 0 : sq;
            var coef = sign * Math.Sqrt(sq);
            var cx1 = coef * (rx * y1 / ry);
            var cy1 = coef * -(ry * x1 / rx);

            //
            // Step 3 : Compute (cx, cy) from (cx1, cy1)
            //
            var sx2 = (x0 + x) / 2.0;
            var sy2 = (y0 + y) / 2.0;
            var cx = sx2 + (cosAngle * cx1 - sinAngle * cy1);
            var cy = sy2 + (sinAngle * cx1 + cosAngle * cy1);

            //
            // Step 4 : Compute the angleStart (angle1) and the angleExtent (dangle)
            //
            var ux = x1 - cx1; // rx;
            var uy = y1 - cy1; // ry;
            var vx = -x1 - cx1; // rx;
            var vy = -y1 - cy1; // ry;
            double p, n;
            // Compute the angle start
            n = Math.Sqrt(ux * ux + uy * uy);
            p = ux; // (1 * ux) + (0 * uy)
            sign = uy < 0 ? -1d : 1d;
            var angleStart = sign * Math.Acos(p / n);
            angleStart = angleStart * 180 / Math.PI;

            // Compute the angle extent
            n = Math.Sqrt((ux * ux + uy * uy) * (vx * vx + vy * vy));
            p = ux * vx + uy * vy;
            sign = ux * vy - uy * vx < 0 ? -1d : 1d;
            var angleExtent = sign * Math.Acos(p / n);
            angleExtent = angleExtent * 180 / Math.PI;

            if (!SweepFlag && angleExtent > 0)
                angleExtent -= 360f;
            else if (SweepFlag && angleExtent < 0) angleExtent += 360f;
            angleExtent %= 360f;
            angleStart %= 360f;

            calcVal.CorrRx = rx;
            calcVal.CorrRy = ry;
            calcVal.Cx = cx;
            calcVal.Cy = cy;
            calcVal.AngleStart = angleStart;
            calcVal.AngleExtent = angleExtent;

            return calcVal;
        }

        #endregion

        #region Private Methods

        private double GetAngle(bool addExtent)
        {
            var calcValues = GetCalculatedArcValues();

            var radAngle = calcValues.AngleStart;
            if (addExtent) radAngle += calcValues.AngleExtent;

            radAngle *= Math.PI / 180;
            var cosAngle = Math.Cos(radAngle);
            var sinAngle = Math.Sin(radAngle);

            var denom = Math.Sqrt(
                calcValues.CorrRy * calcValues.CorrRy * cosAngle * cosAngle +
                calcValues.CorrRx * calcValues.CorrRx * sinAngle * sinAngle);

            var xt = -calcValues.CorrRx * sinAngle / denom;
            var yt = calcValues.CorrRy * cosAngle / denom;

            var a = Math.Atan2(yt, xt) * 180 / Math.PI;
            a += Angle;
            return a;
        }

        #endregion

        #region Private Fields

        #endregion

        #region Public Properties

        public double X { get; set; }

        public double Y { get; set; }

        public double R1 { get; set; }

        public double R2 { get; set; }

        public double Angle { get; set; }

        public bool LargeArcFlag { get; set; }

        public bool SweepFlag { get; set; }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle
        {
            get
            {
                var a = GetAngle(false);
                a += 270;
                a += 360;
                a = a % 360;
                return a;
            }
        }

        public override double EndAngle
        {
            get
            {
                var a = GetAngle(true);
                a += 90;
                a += 360;
                a = a % 360;

                return a;
            }
        }

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(R1);
                sb.Append(",");
                sb.Append(R2);
                sb.Append(",");
                sb.Append(Angle);
                sb.Append(",");

                if (LargeArcFlag)
                    sb.Append("1");
                else
                    sb.Append("0");

                sb.Append(",");

                if (SweepFlag)
                    sb.Append("1");
                else
                    sb.Append("0");

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