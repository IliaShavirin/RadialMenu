using System.Text;
using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    /// <summary>
    ///     Summary description for SvgMoveToSeg.
    /// </summary>
    public abstract class SvgPathSegMoveto : SvgPathSeg
    {
        protected double x;

        protected double y;

        protected SvgPathSegMoveto(SvgPathSegType type, double x, double y)
            : base(type)
        {
            this.x = x;
            this.y = y;
        }

        public abstract override SvgPointF AbsXY { get; }

        public override double StartAngle => 0;

        public override double EndAngle => 0;

        public override string PathText
        {
            get
            {
                var sb = new StringBuilder();
                sb.Append(PathSegTypeAsLetter);
                sb.Append(x);
                sb.Append(",");
                sb.Append(y);

                return sb.ToString();
            }
        }

        public double X
        {
            get => x;
            set => x = value;
        }

        public double Y
        {
            get => y;
            set => y = value;
        }
    }
}