// <developer>niklas@protocol7.com</developer>
// <completed>0</completed>

namespace BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure
{
    /// <summary>
    ///     The SvgElementInstanceList interface provides the abstraction of an ordered collection of SvgElementInstance
    ///     objects, without defining or constraining how this collection is implemented.
    /// </summary>
    public interface ISvgElementInstanceList
    {
        ulong Length { get; }
        ISvgElementInstance Item(ulong index);
    }
}