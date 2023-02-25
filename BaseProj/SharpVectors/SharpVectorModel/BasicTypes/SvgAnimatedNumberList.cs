// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedNumberList : ISvgAnimatedNumberList
    {
        #region Constructor

        public SvgAnimatedNumberList(string str)
        {
            baseVal = new SvgNumberList(str);
            animVal = baseVal;
        }

        #endregion

        #region Fields

        private readonly SvgNumberList baseVal;
        private readonly SvgNumberList animVal;

        #endregion

        #region ISvgAnimatedNumberList Interface

        public ISvgNumberList BaseVal => baseVal;

        public ISvgNumberList AnimVal => animVal;

        #endregion
    }
}