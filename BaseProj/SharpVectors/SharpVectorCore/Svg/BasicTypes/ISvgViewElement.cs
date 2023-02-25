namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    /// <summary>
    /// </summary>
    /// <developer>don@donxml.com</developer>
    /// <completed>100</completed>
    public interface ISvgViewElement :
        ISvgElement,
        ISvgExternalResourcesRequired,
        ISvgFitToViewBox,
        ISvgZoomAndPan
    {
        ISvgStringList ViewTarget { get; }
    }
}