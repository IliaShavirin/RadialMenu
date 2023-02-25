// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedNumber.
    /// </summary>
    public sealed class SvgAnimatedBoolean : ISvgAnimatedBoolean
    {
        #region Constructor

        public SvgAnimatedBoolean(string str, bool defaultValue)
        {
            switch (str)
            {
                case "true":
                    BaseVal = true;
                    break;
                case "false":
                    BaseVal = false;
                    break;
                default:
                    BaseVal = defaultValue;
                    break;
            }

            AnimVal = BaseVal;
        }

        #endregion

        #region Private Fields

        #endregion

        #region ISvgAnimatedBoolean Interface

        public bool BaseVal { get; set; }

        public bool AnimVal { get; }

        #endregion
    }
}