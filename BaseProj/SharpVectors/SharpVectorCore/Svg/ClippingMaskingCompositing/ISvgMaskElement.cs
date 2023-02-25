// <developer>tabascopete78@yahoo.com</developer>
// <completed>10</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.ClippingMaskingCompositing
{
    /// <summary>
    ///     Used by SvgMaskElement.
    /// </summary>
    public interface ISvgMaskElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable
    {
        ISvgAnimatedEnumeration MaskUnits { get; }
        ISvgAnimatedEnumeration MaskContentUnits { get; }
        ISvgAnimatedLength X { get; }
        ISvgAnimatedLength Y { get; }
        ISvgAnimatedLength Width { get; }
        ISvgAnimatedLength Height { get; }
    }
}