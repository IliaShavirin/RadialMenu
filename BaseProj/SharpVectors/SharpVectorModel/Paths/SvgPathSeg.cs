using BaseProj.SharpVectors.SharpVectorCore.Svg;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Paths;

namespace BaseProj.SharpVectors.SharpVectorModel.Paths
{
    public abstract class SvgPathSeg : ISvgPathSeg
    {
        #region Private Fields

        private SvgPathSegList _list;

        #endregion

        #region Constructors

        protected SvgPathSeg(SvgPathSegType type)
        {
            PathSegType = type;
        }

        #endregion

        #region Public Properties

        public abstract string PathText { get; }
        public abstract SvgPointF AbsXY { get; }
        public abstract double StartAngle { get; }
        public abstract double EndAngle { get; }

        public SvgPathSeg PreviousSeg => _list.GetPreviousSegment(this);

        public SvgPathSeg NextSeg => _list.GetNextSegment(this);

        public int Index { get; private set; }

        public virtual double Length => 0;

        #endregion

        #region Internal Methods

        internal void SetList(SvgPathSegList list)
        {
            _list = list;
        }

        internal void SetIndex(int index)
        {
            Index = index;
        }

        internal void SetIndexWithDiff(int diff)
        {
            Index += diff;
        }

        #endregion

        #region ISvgPathSeg Members

        public SvgPathSegType PathSegType { get; }

        public string PathSegTypeAsLetter
        {
            get
            {
                switch (PathSegType)
                {
                    case SvgPathSegType.ArcAbs:
                        return "A";
                    case SvgPathSegType.ArcRel:
                        return "a";
                    case SvgPathSegType.ClosePath:
                        return "z";
                    case SvgPathSegType.CurveToCubicAbs:
                        return "C";
                    case SvgPathSegType.CurveToCubicRel:
                        return "c";
                    case SvgPathSegType.CurveToCubicSmoothAbs:
                        return "S";
                    case SvgPathSegType.CurveToCubicSmoothRel:
                        return "s";
                    case SvgPathSegType.CurveToQuadraticAbs:
                        return "Q";
                    case SvgPathSegType.CurveToQuadraticRel:
                        return "q";
                    case SvgPathSegType.CurveToQuadraticSmoothAbs:
                        return "T";
                    case SvgPathSegType.CurveToQuadraticSmoothRel:
                        return "t";
                    case SvgPathSegType.LineToAbs:
                        return "L";
                    case SvgPathSegType.LineToHorizontalAbs:
                        return "H";
                    case SvgPathSegType.LineToHorizontalRel:
                        return "h";
                    case SvgPathSegType.LineToRel:
                        return "l";
                    case SvgPathSegType.LineToVerticalAbs:
                        return "V";
                    case SvgPathSegType.LineToVerticalRel:
                        return "v";
                    case SvgPathSegType.MoveToAbs:
                        return "M";
                    case SvgPathSegType.MoveToRel:
                        return "m";
                    default:
                        return string.Empty;
                }
            }
        }

        #endregion
    }
}