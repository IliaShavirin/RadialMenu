// <developer>kevin@kevlindev.com</developer>
// <completed>99</completed>

using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Scripting
{
    /// <summary>
    ///     The SvgScriptElement interface corresponds to the 'script' element.
    /// </summary>
    public interface ISvgScriptElement : ISvgElement, ISvgUriReference,
        ISvgExternalResourcesRequired
    {
        string Type { get; set; }
    }
}