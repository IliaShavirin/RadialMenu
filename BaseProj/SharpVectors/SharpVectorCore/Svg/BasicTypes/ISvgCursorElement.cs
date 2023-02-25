namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgCursorElement :
        ISvgElement,
        ISvgUriReference,
        ISvgTests,
        ISvgExternalResourcesRequired
    {
        ISvgAnimatedLength X { get; }
        ISvgAnimatedLength Y { get; }
    }
}