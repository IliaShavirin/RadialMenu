using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    public interface ISvgColorProfileElement : ISvgElement, ISvgUriReference
    {
        string Local { get; }
    }
}