// <developer>niklas@protocol7.com</developer>
// <completed>10</completed>

using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Coordinates;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Text
{
    /// <summary>
    ///     The SvgTextContentElement interface is inherited by various text-related interfaces, such as SvgTextElement,
    ///     SvgTSpanElement, SvgTRefElement, SvgAltGlyphElement and SvgTextPathElement.
    /// </summary>
    public interface ISvgTextContentElement : ISvgElement, ISvgTests, ISvgLangSpace,
        ISvgExternalResourcesRequired, ISvgStylable, IEventTarget
    {
        ISvgAnimatedLength TextLength { get; }
        ISvgAnimatedEnumeration LengthAdjust { get; }
        long GetNumberOfChars();
        float GetComputedTextLength();
        float GetSubStringLength(long charnum, long nchars);
        ISvgPoint GetStartPositionOfChar(long charnum);
        ISvgPoint GetEndPositionOfChar(long charnum);
        ISvgRect GetExtentOfChar(long charnum);
        float GetRotationOfChar(long charnum);
        long GetCharNumAtPosition(ISvgPoint point);
        void SelectSubString(long charnum, long nchars);
    }
}