using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Views;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for MouseEvent.
    /// </summary>
    public class MouseEvent : UiEvent, IMouseEvent
    {
        #region Private Fields

        #endregion

        #region Constructors

        public MouseEvent()
        {
        }

        public MouseEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail,
            long screenX,
            long screenY,
            long clientX,
            long clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            ushort button,
            IEventTarget relatedTarget)
        {
            InitMouseEvent(
                eventType, bubbles, cancelable, view, detail,
                screenX, screenY, clientX, clientY,
                ctrlKey, altKey, shiftKey, metaKey, button,
                relatedTarget);
        }

        public MouseEvent(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail,
            long screenX,
            long screenY,
            long clientX,
            long clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            ushort button,
            IEventTarget relatedTarget,
            bool altGraphKey)
        {
            InitMouseEventNs(
                namespaceUri, eventType, bubbles, cancelable, view, detail,
                screenX, screenY, clientX, clientY,
                ctrlKey, altKey, shiftKey, metaKey, button,
                relatedTarget, altGraphKey);
        }

        public MouseEvent(
            EventType eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail,
            long screenX,
            long screenY,
            long clientX,
            long clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            ushort button,
            IEventTarget relatedTarget,
            bool altGraphKey)
        {
            InitMouseEventNs(
                eventType.NamespaceUri, eventType.Name,
                bubbles, cancelable, view, detail,
                screenX, screenY, clientX, clientY,
                ctrlKey, altKey, shiftKey, metaKey, button,
                relatedTarget, altGraphKey);
        }

        #endregion

        #region IMouseEvent interface

        #region Public Properties

        public long ScreenX { get; private set; }

        public long ScreenY { get; private set; }

        public long ClientX { get; private set; }

        public long ClientY { get; private set; }

        public bool CtrlKey { get; private set; }

        public bool ShiftKey { get; private set; }

        public bool AltKey { get; private set; }

        public bool MetaKey { get; private set; }

        public ushort Button { get; private set; }

        public IEventTarget RelatedTarget { get; private set; }

        public bool AltGraphKey { get; private set; }

        #endregion

        #region Public Methods

        public void InitMouseEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail,
            long screenX,
            long screenY,
            long clientX,
            long clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            ushort button,
            IEventTarget relatedTarget)
        {
            InitUiEvent(eventType, bubbles, cancelable, view, detail);

            ScreenX = screenX;
            ScreenY = screenY;
            ClientX = clientX;
            ClientY = clientY;
            CtrlKey = ctrlKey;
            ShiftKey = shiftKey;
            AltKey = altKey;
            MetaKey = metaKey;
            Button = button;
            RelatedTarget = relatedTarget;
            //this.altGraphKey   = altGraphKey;
        }

        public void InitMouseEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            long detail,
            long screenX,
            long screenY,
            long clientX,
            long clientY,
            bool ctrlKey,
            bool altKey,
            bool shiftKey,
            bool metaKey,
            ushort button,
            IEventTarget relatedTarget,
            bool altGraphKey)
        {
            InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, detail);

            ScreenX = screenX;
            ScreenY = screenY;
            ClientX = clientX;
            ClientY = clientY;
            CtrlKey = ctrlKey;
            ShiftKey = shiftKey;
            AltKey = altKey;
            MetaKey = metaKey;
            Button = button;
            RelatedTarget = relatedTarget;
            AltGraphKey = altGraphKey;
        }

        #endregion

        #endregion
    }
}