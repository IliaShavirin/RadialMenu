// <developer>don@donxml.com</developer>
// <completed>100</completed>

using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    /// </summary>
    public interface ISvgAElement : ISvgElement, ISvgUriReference, ISvgTests,
        ISvgLangSpace, ISvgExternalResourcesRequired, ISvgStylable,
        ISvgTransformable, IEventTarget
    {
        ISvgAnimatedString Target { get; }
    }
}