using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Views;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for TextEvent.
    /// </summary>
    public class TextEvent : UiEvent, ITextEvent
    {
        #region Private Fields

        private IAbstractView view;

        #endregion

        #region Constructors

        public TextEvent()
        {
        }

        public TextEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string data)
        {
            InitTextEvent(eventType, bubbles, cancelable, view, data);
        }

        public TextEvent(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string data)
        {
            InitTextEventNs(namespaceUri, eventType, bubbles, cancelable, view, data);
        }

        #endregion

        #region ITextEvent interface

        public string Data { get; private set; }

        public void InitTextEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string data)
        {
            InitEvent(eventType, bubbles, cancelable);

            this.view = view;
            Data = data;
        }

        public void InitTextEventNs(
            string namespaceUri,
            string type,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string data)
        {
            InitEventNs(namespaceUri, eventType, bubbles, cancelable);

            this.view = view;
            Data = data;
        }

        #endregion
    }
}