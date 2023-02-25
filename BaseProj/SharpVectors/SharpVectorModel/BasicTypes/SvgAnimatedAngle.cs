using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    public sealed class SvgAnimatedAngle : ISvgAnimatedAngle
    {
        #region Fields

        #endregion

        #region Constructor

        public SvgAnimatedAngle(string s, string defaultValue)
        {
            AnimVal = BaseVal = new SvgAngle(s, defaultValue, false);
        }

        public SvgAnimatedAngle(ISvgAngle angle)
        {
            AnimVal = BaseVal = angle;
        }

        #endregion

        #region Implementation of ISvgAnimatedAngle

        public ISvgAngle BaseVal { get; }

        public ISvgAngle AnimVal { get; }

        #endregion
    }
}