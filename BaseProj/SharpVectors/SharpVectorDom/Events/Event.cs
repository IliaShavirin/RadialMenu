using System;
using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for Event.
    /// </summary>
    public class Event : IEvent
    {
        #region Private Fields

        internal bool stopped = false;
        internal IEventTarget eventTarget = null;
        internal IEventTarget currentTarget = null;
        internal EventPhase eventPhase = EventPhase.AtTarget;

        protected string eventType;

        #endregion

        #region Constructors

        public Event()
        {
        }

        public Event(
            string eventType,
            bool bubbles,
            bool cancelable)
        {
            InitEvent(eventType, bubbles, cancelable);
        }

        public Event(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable)
        {
            InitEventNs(namespaceUri, eventType, bubbles, cancelable);
        }

        #endregion

        #region Properties

        #region DOM Level 2

        public string Type => eventType;

        public IEventTarget Target => eventTarget;

        public IEventTarget CurrentTarget => currentTarget;

        public EventPhase EventPhase => eventPhase;

        public bool Bubbles { get; private set; }

        public bool Cancelable { get; private set; }

        public DateTime TimeStamp { get; } = DateTime.Now;

        #endregion

        #region DOM Level 3 Experimental

        public string NamespaceUri { get; private set; }

        public bool IsCustom => throw new NotImplementedException();

        public bool IsDefaultPrevented => throw new NotImplementedException();

        #endregion

        #endregion

        #region Methods

        #region DOM Level 2

        public void StopPropagation()
        {
            throw new NotImplementedException();
        }

        public void PreventDefault()
        {
            throw new NotImplementedException();
        }

        public void InitEvent(
            string eventType,
            bool bubbles,
            bool cancelable)
        {
            NamespaceUri = null;
            this.eventType = eventType;
            Bubbles = bubbles;
            Cancelable = cancelable;
        }

        #endregion

        #region DOM Level 3 Experimental

        public void InitEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable)
        {
            NamespaceUri = namespaceUri == "" ? null : namespaceUri;
            this.eventType = eventType;
            Bubbles = bubbles;
            Cancelable = cancelable;
        }

        public void StopImmediatePropagation()
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion
    }
}