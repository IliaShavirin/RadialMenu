using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Filters
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgFilterPrimitiveStandardAttributes :
        ISvgStylable
    {
        ISvgAnimatedLength X { get; }
        ISvgAnimatedLength Y { get; }
        ISvgAnimatedLength Width { get; }
        ISvgAnimatedLength Height { get; }
        ISvgAnimatedLength Result { get; }
    }
}