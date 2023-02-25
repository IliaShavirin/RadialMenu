using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Fills
{
    /// <summary>
    ///     The SvgLinearGradientElement interface corresponds to the 'linearGradient' element.
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>100</completed>
    public interface ISvgLinearGradientElement : ISvgGradientElement
    {
        ISvgAnimatedLength X1 { get; }
        ISvgAnimatedLength Y1 { get; }
        ISvgAnimatedLength X2 { get; }
        ISvgAnimatedLength Y2 { get; }
    }
}