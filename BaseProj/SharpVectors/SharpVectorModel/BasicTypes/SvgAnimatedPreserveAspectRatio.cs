// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedPreserveAspectRatio.
    /// </summary>
    public sealed class SvgAnimatedPreserveAspectRatio : ISvgAnimatedPreserveAspectRatio
    {
        #region Constructor

        public SvgAnimatedPreserveAspectRatio(string attr, SvgElement ownerElement)
        {
            baseVal = new SvgPreserveAspectRatio(attr, ownerElement);
            animVal = baseVal;
        }

        #endregion

        #region Private Fields

        private readonly SvgPreserveAspectRatio baseVal;
        private readonly SvgPreserveAspectRatio animVal;

        #endregion

        #region ISvgAnimatedPreserveAspectRatio Interface

        public ISvgPreserveAspectRatio BaseVal => baseVal;

        public ISvgPreserveAspectRatio AnimVal => animVal;

        #endregion
    }
}