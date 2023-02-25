using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    public interface ISvgSymbolElement : ISvgElement, ISvgLangSpace, ISvgStylable,
        ISvgExternalResourcesRequired, ISvgFitToViewBox, IEventTarget
    {
    }
}