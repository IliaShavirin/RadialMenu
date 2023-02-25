// <developer>niklas@protocol7.com</developer>
// <completed>100</completed>

using BaseProj.SharpVectors.SharpVectorCore.Css;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    /// </summary>
    public interface ISvgStylable
    {
        ISvgAnimatedString ClassName { get; }
        ICssStyleDeclaration Style { get; }
        ICssValue GetPresentationAttribute(string name);
    }
}