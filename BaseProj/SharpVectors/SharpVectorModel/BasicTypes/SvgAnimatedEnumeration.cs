// <developer>niklas@protocol7.com</developer>
// <developer>kevin@kevlindev.com</developer>
// <completed>100</completed>


using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorModel.BasicTypes
{
    /// <summary>
    ///     Summary description for SvgAnimatedEnumeration.
    /// </summary>
    public sealed class SvgAnimatedEnumeration : ISvgAnimatedEnumeration
    {
        #region Constructor

        public SvgAnimatedEnumeration(ushort val)
        {
            BaseVal = AnimVal = val;
        }

        #endregion

        #region Private Fields

        #endregion

        #region ISvgAnimatedEnumeration Interface

        public ushort BaseVal { get; set; }

        public ushort AnimVal { get; }

        #endregion
    }
}