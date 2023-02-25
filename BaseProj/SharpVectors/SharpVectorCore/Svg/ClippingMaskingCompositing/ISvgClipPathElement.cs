// <developer>tabascopete78@yahoo.com</developer>
// <completed>50</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.ClippingMaskingCompositing
{
    /// <summary>
    ///     Used by SvgClipPathElement.
    /// </summary>
    public interface ISvgClipPathElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, ISvgTransformable
    {
        ISvgAnimatedEnumeration ClipPathUnits { get; }
    }
}