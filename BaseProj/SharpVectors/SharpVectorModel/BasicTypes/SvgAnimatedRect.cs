// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    /// </summary>
    public sealed class SvgAnimatedRect : ISvgAnimatedRect
    {
        #region Private Fields

        private readonly SvgRect baseVal;
        private readonly SvgRect animVal;

        #endregion

        #region Constructors

        public SvgAnimatedRect(string str)
        {
            baseVal = new SvgRect(str);
            animVal = baseVal;
        }

        public SvgAnimatedRect(SvgRect rect)
        {
            baseVal = rect;
            animVal = baseVal;
        }

        #endregion

        #region ISvgAnimatedRect Interface

        public ISvgRect BaseVal => baseVal;

        public ISvgRect AnimVal => animVal;

        #endregion
    }
}