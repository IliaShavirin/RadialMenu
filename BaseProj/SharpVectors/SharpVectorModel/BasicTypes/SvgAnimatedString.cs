// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedString : ISvgAnimatedString
    {
        #region Constructor

        public SvgAnimatedString(string str)
        {
            BaseVal = str;
            AnimVal = BaseVal;
        }

        #endregion

        #region Private Fields

        #endregion

        #region ISvgAnimatedString Interface

        public string BaseVal { get; set; }

        public string AnimVal { get; }

        #endregion
    }
}