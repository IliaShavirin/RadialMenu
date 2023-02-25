using System;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public abstract class SvgPathSegLineto : SvgPathSeg
    {
        protected SvgPathSegLineto(SvgPathSegType type) : base(type)
        {
        }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle
        {
            get
            {
                var prevPoint = getPrevPoint();
                var curPoint = AbsXY;

                double dx = curPoint.X - prevPoint.X;
                double dy = curPoint.Y - prevPoint.Y;

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
                var a = StartAngle;
                a += 180;
                a %= 360;
                return a;
            }
        }

        public override double Length
        {
            get
            {
                var prevPoint = getPrevPoint();
                var thisPoint = AbsXY;

                double dx = thisPoint.X - prevPoint.X;
                double dy = thisPoint.Y - prevPoint.Y;

                return Math.Sqrt(dx * dx + dy * dy);
            }
        }

        private SvgPointF getPrevPoint()
        {
            var prevSeg = PreviousSeg;
            SvgPointF prevPoint;
            if (prevSeg == null)
                prevPoint = new SvgPointF(0, 0);
            else
                prevPoint = prevSeg.AbsXY;
            return prevPoint;
        }
    }
}