namespace BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes
{
    public interface ISvgAnimatedAngle
    {
        ISvgAngle BaseVal { get; }
        ISvgAngle AnimVal { get; }
    }
}