// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedLengthList.
    /// </summary>
    public sealed class SvgAnimatedLengthList : ISvgAnimatedLengthList
    {
        #region Constructors

        public SvgAnimatedLengthList(string propertyName, string str, SvgElement ownerElement,
            SvgLengthDirection direction)
        {
            baseVal = new SvgLengthList(propertyName, str, ownerElement, direction);
            animVal = baseVal;
        }

        #endregion

        #region Fields

        private readonly SvgLengthList baseVal;
        private readonly SvgLengthList animVal;

        #endregion

        #region ISvgAnimatedLengthList Interface

        public ISvgLengthList BaseVal => baseVal;

        public ISvgLengthList AnimVal => animVal;

        #endregion
    }
}