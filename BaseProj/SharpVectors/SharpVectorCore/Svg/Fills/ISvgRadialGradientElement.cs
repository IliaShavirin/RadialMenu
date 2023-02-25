using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Fills
{
    /// <summary>
    ///     The ISvgRadialGradientElement interface corresponds to the 'radialGradient' element.
    /// </summary>
    /// <developer>Rick.Bullotta@lighthammer.com</developer>
    /// <completed>100</completed>
    public interface ISvgRadialGradientElement : ISvgGradientElement
    {
        ISvgAnimatedLength Cx { get; }
        ISvgAnimatedLength Cy { get; }
        ISvgAnimatedLength R { get; }
        ISvgAnimatedLength Fx { get; }
        ISvgAnimatedLength Fy { get; }
    }
}