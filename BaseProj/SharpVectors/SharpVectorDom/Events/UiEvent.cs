using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Views;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for UiEvent.
    /// </summary>
    public class UiEvent : Event, IUiEvent
    {
        #region Private Fields

        #endregion

        #region Constructors

        public UiEvent()
        {
        }

        public UiEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail)
        {
            InitUiEvent(eventType, bubbles, cancelable, view, detail);
        }

        public UiEvent(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail)
        {
            InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);
        }

        #endregion

        #region IUiEvent interface

        public IAbstractView View { get; private set; }

        public long Detail { get; private set; }

        public void InitUiEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail)
        {
            InitEvent(eventType, bubbles, cancelable);

            View = view;
            Detail = detail;
        }

        public void InitUiEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail)
        {
            InitEventNs(namespaceUri, eventType, bubbles, cancelable);

            View = view;
            Detail = detail;
        }

        #endregion
    }
}