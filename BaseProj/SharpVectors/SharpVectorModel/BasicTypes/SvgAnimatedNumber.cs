// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedNumber.
    /// </summary>
    public sealed class SvgAnimatedNumber : ISvgAnimatedNumber
    {
        #region Constructors

        public SvgAnimatedNumber(string str)
        {
            BaseVal = SvgNumber.ParseNumber(str);
            AnimVal = BaseVal;
        }

        #endregion

        #region Private Fields

        #endregion

        #region ISvgAnimatedNumber Interface

        public double BaseVal { get; set; }

        public double AnimVal { get; }

        #endregion
    }
}