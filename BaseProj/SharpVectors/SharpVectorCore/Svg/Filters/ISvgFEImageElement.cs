using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Filters
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgFEImageElement :
        ISvgElement,
        ISvgUriReference,
        ISvgLangSpace,
        ISvgExternalResourcesRequired,
        ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedPreserveAspectRatio PreserveAspectRatio { get; }
    }
}