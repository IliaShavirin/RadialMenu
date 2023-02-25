using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.BasicTypes;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorDom.Events;

namespace BaseProj.SharpVectors.SharpVectorModel.DocumentStructure
{
    public sealed class SvgElementInstance : IEventTargetSupport, ISvgElementInstance
    {
        #region Private Fields

        private readonly EventTarget eventTarget;

        #endregion

        private SvgElementInstanceList childNodes;

        public SvgElementInstance(XmlNode refNode, SvgUseElement useElement, SvgElementInstance parent)
        {
            CorrespondingUseElement = useElement;
            ParentNode = parent;
            CorrespondingElement = (ISvgElement)refNode;
            eventTarget = new EventTarget(this);
        }

        #region NON-DOM

        void IEventTargetSupport.FireEvent(IEvent eventObject)
        {
            eventTarget.FireEvent(eventObject);
        }

        #endregion

        public ISvgElement CorrespondingElement { get; }

        public ISvgUseElement CorrespondingUseElement { get; }

        public ISvgElementInstance ParentNode { get; }

        public ISvgElementInstanceList ChildNodes
        {
            get
            {
                if (childNodes == null)
                    childNodes = new SvgElementInstanceList((SvgUseElement)CorrespondingUseElement, this);
                return childNodes;
            }
        }

        public ISvgElementInstance FirstChild
        {
            get
            {
                var cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(0);
                return null;
            }
        }

        public ISvgElementInstance LastChild
        {
            get
            {
                var cn = ChildNodes;
                if (cn.Length < 0) return cn.Item(cn.Length);
                return null;
            }
        }

        public ISvgElementInstance PreviousSibling { get; private set; }

        public ISvgElementInstance NextSibling { get; private set; }

        #region IEventTarget Members

        #region Methods

        #region DOM Level 2

        void IEventTarget.AddEventListener(string type, EventListener listener, bool useCapture)
        {
            eventTarget.AddEventListener(type, listener, useCapture);
        }

        void IEventTarget.RemoveEventListener(string type, EventListener listener, bool useCapture)
        {
            eventTarget.RemoveEventListener(type, listener, useCapture);
        }

        bool IEventTarget.DispatchEvent(IEvent eventObject)
        {
            return eventTarget.DispatchEvent(eventObject);
        }

        #endregion

        #region DOM Level 3 Experimental

        void IEventTarget.AddEventListenerNs(string namespaceUri, string type, EventListener listener,
            bool useCapture, object eventGroup)
        {
            eventTarget.AddEventListenerNs(namespaceUri, type, listener, useCapture, eventGroup);
        }

        void IEventTarget.RemoveEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture)
        {
            eventTarget.RemoveEventListenerNs(namespaceUri, type, listener, useCapture);
        }

        bool IEventTarget.WillTriggerNs(string namespaceUri, string type)
        {
            return eventTarget.WillTriggerNs(namespaceUri, type);
        }

        bool IEventTarget.HasEventListenerNs(string namespaceUri, string type)
        {
            return eventTarget.HasEventListenerNs(namespaceUri, type);
        }

        #endregion

        #endregion

        #endregion

        #region Custom management functions

        internal void SetPreviousSibling(ISvgElementInstance instance)
        {
            PreviousSibling = instance;
        }

        internal void SetNextSibling(ISvgElementInstance instance)
        {
            NextSibling = instance;
        }

        #endregion
    }
}