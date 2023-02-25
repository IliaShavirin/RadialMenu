using System.Xml;

namespace BaseProj.SharpVectors.SharpVectorCore
{
    /// <summary>
    ///     Summary description for IAttribute.
    /// </summary>
    public interface IAttribute : INode
    {
        XmlElement OwnerElement { get; }

        bool Specified { get; }
    }
}