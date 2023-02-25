using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore;

namespace BaseProj.SharpVectors.SharpVectorDom
{
    /// <summary>
    ///     Summary description for NodeListAdapter.
    /// </summary>
    public class NodeListAdapter
        : INodeList
    {
        #region Private Fields

        private readonly XmlNodeList nodeList;

        #endregion

        #region Constructors

        public NodeListAdapter(
            XmlNodeList nodeList)
        {
            this.nodeList = nodeList;
        }

        #endregion

        #region INodeList interface

        public INode this[
            ulong index] =>
            (INode)nodeList[(int)index];

        public ulong Count => (ulong)nodeList.Count;

        #endregion
    }
}