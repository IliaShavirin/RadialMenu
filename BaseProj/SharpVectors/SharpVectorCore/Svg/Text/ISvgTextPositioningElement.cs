// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Text
{
    /// <summary>
    ///     The SvgTextPositioningElement interface is inherited by text-related interfaces: SvgTextElement, SvgTSpanElement,
    ///     SvgTRefElement and SvgAltGlyphElement.
    /// </summary>
    public interface ISvgTextPositioningElement : ISvgTextContentElement
    {
        ISvgAnimatedLengthList X { get; }

        ISvgAnimatedLengthList Y { get; }

        ISvgAnimatedLengthList Dx { get; }

        ISvgAnimatedLengthList Dy { get; }

        ISvgAnimatedNumberList Rotate { get; }
    }
}