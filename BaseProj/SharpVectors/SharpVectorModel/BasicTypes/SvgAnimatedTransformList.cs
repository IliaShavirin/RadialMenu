// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Used for the various attributes which specify a set of
    ///     transformations, such as the transform attribute which is
    ///     available for many of Svg's elements, and which can be animated.
    /// </summary>
    public sealed class SvgAnimatedTransformList : ISvgAnimatedTransformList
    {
        #region Constructors

        public SvgAnimatedTransformList(string transform)
        {
            baseVal = new SvgTransformList(transform);
            animVal = baseVal;
        }

        #endregion

        #region Private Fields

        private readonly SvgTransformList baseVal;
        private readonly SvgTransformList animVal;

        #endregion

        #region ISvgAnimagedTransformList Interface

        public ISvgTransformList BaseVal => baseVal;

        public ISvgTransformList AnimVal => animVal;

        #endregion
    }
}