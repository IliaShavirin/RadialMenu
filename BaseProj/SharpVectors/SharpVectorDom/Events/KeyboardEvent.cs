using BaseProj.SharpVectors.SharpVectorCore.Events;
using BaseProj.SharpVectors.SharpVectorCore.Views;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for KeyboardEvent.
    /// </summary>
    public class KeyboardEvent : UiEvent, IKeyboardEvent
    {
        #region Private Fields

        #endregion

        #region Constructors

        public KeyboardEvent()
        {
        }

        public KeyboardEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string keyIdentifier,
            KeyLocationCode keyLocation,
            bool ctrlKey,
            bool shiftKey,
            bool altKey,
            bool metaKey,
            bool altGraphKey)
        {
            InitKeyboardEvent(
                eventType, bubbles, cancelable, view,
                keyIdentifier, keyLocation,
                ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
        }

        public KeyboardEvent(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string keyIdentifier,
            KeyLocationCode keyLocation,
            bool ctrlKey,
            bool shiftKey,
            bool altKey,
            bool metaKey,
            bool altGraphKey)
        {
            InitKeyboardEventNs(
                namespaceUri, eventType, bubbles, cancelable, view,
                keyIdentifier, keyLocation,
                ctrlKey, shiftKey, altKey, metaKey, altGraphKey);
        }

        #endregion

        #region IKeyboardEvent interface

        #region Public Properties

        public string KeyIdentifier { get; private set; }

        public KeyLocationCode KeyLocation { get; private set; }

        public bool CtrlKey { get; private set; }

        public bool ShiftKey { get; private set; }

        public bool AltKey { get; private set; }

        public bool MetaKey { get; private set; }

        public bool AltGraphKey { get; private set; }

        #endregion

        #region Public Methods

        public void InitKeyboardEvent(
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string keyIdentifier,
            KeyLocationCode keyLocation,
            bool ctrlKey,
            bool shiftKey,
            bool altKey,
            bool metaKey,
            bool altGraphKey)
        {
            InitUiEvent(eventType, bubbles, cancelable, view, 0);

            KeyIdentifier = keyIdentifier;
            KeyLocation = keyLocation;
            CtrlKey = ctrlKey;
            ShiftKey = shiftKey;
            AltKey = altKey;
            MetaKey = metaKey;
            AltGraphKey = altGraphKey;
        }

        public void InitKeyboardEventNs(
            string namespaceUri,
            string eventType,
            bool bubbles,
            bool cancelable,
            IAbstractView view,
            string keyIdentifier,
            KeyLocationCode keyLocation,
            bool ctrlKey,
            bool shiftKey,
            bool altKey,
            bool metaKey,
            bool altGraphKey)
        {
            InitUiEventNs(namespaceUri, eventType, bubbles, cancelable, view, 0);

            KeyIdentifier = keyIdentifier;
            KeyLocation = keyLocation;
            CtrlKey = ctrlKey;
            ShiftKey = shiftKey;
            AltKey = altKey;
            MetaKey = metaKey;
            AltGraphKey = altGraphKey;
        }

        #endregion

        #endregion
    }
}