// <developer></developer>
// <completed>0</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Text
{
    /// <summary>
    ///     The SvgTextPathElement interface corresponds to the 'textPath' element.
    /// </summary>
    public interface ISvgTextPathElement : ISvgUriReference, ISvgTextContentElement
    {
        ISvgAnimatedLength StartOffset { get; }
        ISvgAnimatedEnumeration Method { get; }
        ISvgAnimatedEnumeration Spacing { get; }
    }
}