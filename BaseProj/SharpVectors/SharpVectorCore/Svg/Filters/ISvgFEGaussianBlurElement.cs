using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Filters
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgFEGaussianBlurElement :
        ISvgElement,
        ISvgFilterPrimitiveStandardAttributes
    {
        ISvgAnimatedString In1 { get; }
        ISvgAnimatedNumber StdDeviationX { get; }
        ISvgAnimatedNumber StdDeviationY { get; }
        void SetStdDeviation(float stdDeviationX, float stdDeviationY);
    }
}