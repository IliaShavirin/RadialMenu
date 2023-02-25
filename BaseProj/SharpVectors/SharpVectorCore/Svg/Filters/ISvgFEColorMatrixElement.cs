using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Filters
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgFEColorMatrixElement :
        ISvgElement,
        ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedEnumeration Type { get; }
        ISvgAnimatedNumberList Values { get; }
    }
}