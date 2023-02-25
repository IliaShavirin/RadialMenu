using BaseProj.SharpVectors.SharpVectorCore;
using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for MutationEvent.
    /// </summary>
    public class MutationEvent : Event, IMutationEvent
    {
        #region Constructors

        #endregion

        #region Private Fields

        #endregion

        #region IMutationEvent interface

        public INode RelatedNode { get; private set; }

        public string PrevValue { get; private set; }

        public string NewValue { get; private set; }

        public string AttrName { get; private set; }

        public AttrChangeType AttrChange { get; private set; }

        public void InitMutationEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            INode relatedNode,
            string prevValue,
            string newValue,
            string attrName,
            AttrChangeType attrChange)
        {
            InitEvent(eventType, bubbles, cancelable);

            RelatedNode = relatedNode;
            PrevValue = prevValue;
            NewValue = newValue;
            AttrName = attrName;
            AttrChange = attrChange;
        }

        public void InitMutationEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            INode relatedNode,
            string prevValue,
            string newValue,
            string attrName,
            AttrChangeType attrChange)
        {
            InitEventNs(namespaceUri, eventType, bubbles, cancelable);

            RelatedNode = relatedNode;
            PrevValue = prevValue;
            NewValue = newValue;
            AttrName = attrName;
            AttrChange = attrChange;
        }

        #endregion
    }
}