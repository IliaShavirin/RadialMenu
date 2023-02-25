using BaseProj.SharpVectors.SharpVectorCore;
using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for MutationNameEvent.
    /// </summary>
    public class MutationNameEvent : MutationEvent, IMutationNameEvent
    {
        #region Private Fields

        #endregion

        #region IMutationEvent interface

        public string PrevNamespaceUri { get; private set; }

        public string PrevNodeName { get; private set; }

        public void InitMutationNameEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            INode relatedNode,
            string prevNamespaceUri,
            string prevNodeName)
        {
            InitMutationEvent(
                eventType, bubbles, cancelable,
                relatedNode, "", "", "", AttrChangeType.None);

            PrevNamespaceUri = prevNamespaceUri;
            PrevNodeName = prevNodeName;
        }

        public void InitMutationNameEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            INode relatedNode,
            string prevNamespaceUri,
            string prevNodeName)
        {
            InitMutationEventNs(
                namespaceUri, eventType, bubbles, cancelable,
                relatedNode, "", "", "", AttrChangeType.None);

            PrevNamespaceUri = prevNamespaceUri;
            PrevNodeName = prevNodeName;
        }

        #endregion
    }
}