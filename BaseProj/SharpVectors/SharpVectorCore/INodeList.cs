namespace BaseProj.SharpVectors.SharpVectorCore
{
    /// <summary>
    ///     Summary description for INodeList.
    /// </summary>
    public interface INodeList
    {
        INode this[ulong index] { get; }

        ulong Count { get; }
    }
}