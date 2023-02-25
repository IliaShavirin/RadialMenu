using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.Painting
{
    public interface ISvgMarkerElement : ISvgElement, ISvgLangSpace, ISvgExternalResourcesRequired, ISvgStylable,
        ISvgFitToViewBox
    {
        ISvgAnimatedLength RefX { get; }
        ISvgAnimatedLength RefY { get; }
        ISvgAnimatedEnumeration MarkerUnits { get; }
        ISvgAnimatedLength MarkerWidth { get; }
        ISvgAnimatedLength MarkerHeight { get; }
        ISvgAnimatedEnumeration OrientType { get; }
        ISvgAnimatedAngle OrientAngle { get; }
        void SetOrientToAuto();
        void SetOrientToAngle(ISvgAngle angle);
    }
}