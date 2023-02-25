using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Fills
{
    /// <summary>
    ///     The SvgStopElement interface corresponds to the 'stop' element.
    /// </summary>
    /// <developer>niklas@protocol7.com</developer>
    /// <completed>100</completed>
    public interface ISvgStopElement :
        ISvgElement,
        ISvgStylable
    {
        ISvgAnimatedNumber Offset { get; }
    }
}