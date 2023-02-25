using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.Text;
using BaseProj.SharpVectors.SharpVectorModel.BasicTypes;
using BaseProj.SharpVectors.SharpVectorModel.DocumentStructure;

namespace BaseProj.SharpVectors.SharpVectorModel.Text
{
    /// <summary>
    ///     The SvgTextPositioningElement interface is inherited by text-related interfaces:
    ///     SvgTextElement, SvgTSpanElement, SvgTRefElement and SvgAltGlyphElement.
    /// </summary>
    public abstract class SvgTextPositioningElement : SvgTextContentElement, ISvgTextPositioningElement
    {
        #region Constructors and Destructor

        public SvgTextPositioningElement(string prefix, string localname, string ns,
            SvgDocument doc) : base(prefix, localname, ns, doc)
        {
        }

        #endregion

        #region Public Properties

        public ISvgAnimatedLengthList X =>
            new SvgAnimatedLengthList("x", GetAttribute("x"), this,
                SvgLengthDirection.Horizontal);

        public ISvgAnimatedLengthList Y =>
            new SvgAnimatedLengthList("y", GetAttribute("y"), this,
                SvgLengthDirection.Vertical);

        public ISvgAnimatedLengthList Dx =>
            new SvgAnimatedLengthList("dx", GetAttribute("dx"), this,
                SvgLengthDirection.Horizontal);

        public ISvgAnimatedLengthList Dy =>
            new SvgAnimatedLengthList("dy", GetAttribute("dy"), this,
                SvgLengthDirection.Vertical);

        public ISvgAnimatedNumberList Rotate => new SvgAnimatedNumberList(GetAttribute("rotate"));

        #endregion
    }
}