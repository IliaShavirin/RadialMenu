using System;
using System.Collections;
using System.Xml;
using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Svg.DocumentStructure;
using BaseProj.SharpVectors.SharpVectorCore.Utils.Dom;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for EventTarget.
    /// </summary>
    public class EventTarget : IEventTargetSupport
    {
        #region Constructors

        public EventTarget(IEventTargetSupport eventTarget)
        {
            this.eventTarget = eventTarget;
            captureMap = new EventListenerMap();
            bubbleMap = new EventListenerMap();
            _ancestors = new ArrayList();
        }

        #endregion

        #region Private Fields

        private readonly IEventTargetSupport eventTarget;

        private readonly EventListenerMap captureMap;
        private readonly EventListenerMap bubbleMap;
        private readonly ArrayList _ancestors;

        #endregion

        #region IEventTarget interface

        #region Methods

        #region DOM Level 2

        public void AddEventListener(string type, EventListener listener,
            bool useCapture)
        {
            if (useCapture)
                captureMap.AddEventListener(null, type, null, listener);
            else
                bubbleMap.AddEventListener(null, type, null, listener);
        }

        public void RemoveEventListener(string type, EventListener listener,
            bool useCapture)
        {
            if (useCapture)
                captureMap.RemoveEventListener(null, type, listener);
            else
                bubbleMap.RemoveEventListener(null, type, listener);
        }

        public bool DispatchEvent(IEvent eventInfo)
        {
            if (eventInfo.Type == null || eventInfo.Type == "")
                throw new EventException(EventExceptionCode.UnspecifiedEventTypeErr);
            try
            {
                // The actual target may be an SvgElement or an SvgElementInstance from 
                // a conceptual tree <see href="http://www.w3.org/TR/SVG/struct.html#UseElement"/>
                XmlNode currNode = null;
                ISvgElementInstance currInstance = null;

                if (eventTarget is ISvgElementInstance)
                    currInstance = (ISvgElementInstance)eventTarget;
                else
                    currNode = (XmlNode)eventTarget;

                // We can't use an XPath ancestor axe because we must account for 
                // conceptual nodes
                _ancestors.Clear();

                // Build the ancestors from the conceptual tree
                if (currInstance != null)
                {
                    while (currInstance.ParentNode != null)
                    {
                        currInstance = currInstance.ParentNode;
                        _ancestors.Add(currInstance);
                    }

                    currNode = (XmlNode)currInstance.CorrespondingUseElement;
                    _ancestors.Add(currNode);
                }

                // Build actual ancestors
                while (currNode != null && currNode.ParentNode != null)
                {
                    currNode = currNode.ParentNode;
                    _ancestors.Add(currNode);
                }

                var realEvent = (Event)eventInfo;
                realEvent.eventTarget = eventTarget;

                if (!realEvent.stopped)
                {
                    realEvent.eventPhase = EventPhase.CapturingPhase;

                    for (var i = _ancestors.Count - 1; i >= 0; i--)
                    {
                        if (realEvent.stopped) break;

                        var ancestor = _ancestors[i] as IEventTarget;

                        if (ancestor != null)
                        {
                            realEvent.currentTarget = ancestor;

                            if (ancestor is IEventTargetSupport) ((IEventTargetSupport)ancestor).FireEvent(realEvent);
                        }
                    }
                }

                if (!realEvent.stopped)
                {
                    realEvent.eventPhase = EventPhase.AtTarget;
                    realEvent.currentTarget = eventTarget;
                    eventTarget.FireEvent(realEvent);
                }

                if (!realEvent.stopped)
                {
                    realEvent.eventPhase = EventPhase.BubblingPhase;

                    for (var i = 0; i < _ancestors.Count; i++)
                    {
                        if (realEvent.stopped) break;

                        var ancestor = _ancestors[i] as IEventTarget;

                        if (ancestor != null)
                        {
                            realEvent.currentTarget = ancestor;
                            ((IEventTargetSupport)ancestor).FireEvent(realEvent);
                        }
                    }
                }

                return realEvent.stopped;
            }
            catch (InvalidCastException)
            {
                throw new DomException(DomExceptionType.WrongDocumentErr);
            }
        }

        #endregion

        #region DOM Level 3 Experimental

        public void AddEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture, object evtGroup)
        {
            if (useCapture)
                captureMap.AddEventListener(namespaceUri, type, evtGroup, listener);
            else
                bubbleMap.AddEventListener(namespaceUri, type, evtGroup, listener);
        }

        public void RemoveEventListenerNs(string namespaceUri, string type,
            EventListener listener, bool useCapture)
        {
            if (useCapture)
                captureMap.RemoveEventListener(namespaceUri, type, listener);
            else
                bubbleMap.RemoveEventListener(namespaceUri, type, listener);
        }

        public bool WillTriggerNs(string namespaceUri, string type)
        {
            var node = (XmlNode)eventTarget;
            var ancestors = node.SelectNodes("ancestor::node()");

            for (var i = 0; i < ancestors.Count; i++)
            {
                var ancestor = ancestors[i] as IEventTarget;

                if (ancestor.HasEventListenerNs(namespaceUri, type)) return true;
            }

            return false;
        }

        public bool HasEventListenerNs(string namespaceUri, string eventType)
        {
            return captureMap.HasEventListenerNs(namespaceUri, eventType) ||
                   bubbleMap.HasEventListenerNs(namespaceUri, eventType);
        }

        #endregion

        #endregion

        #region NON-DOM

        public void FireEvent(IEvent eventInfo)
        {
            switch (eventInfo.EventPhase)
            {
                case EventPhase.AtTarget:
                case EventPhase.BubblingPhase:
                    bubbleMap.Lock();
                    bubbleMap.FireEvent(eventInfo);
                    bubbleMap.Unlock();
                    break;
                case EventPhase.CapturingPhase:
                    captureMap.Lock();
                    captureMap.FireEvent(eventInfo);
                    captureMap.Unlock();
                    break;
            }
        }

        public event EventListener OnMouseMove
        {
            add => throw new NotImplementedException();
            remove => throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}