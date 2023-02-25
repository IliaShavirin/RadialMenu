using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for EventListenerMapEntry.
    /// </summary>
    public class EventListenerMapEntry
    {
        public object Group;
        public EventListener Listener;
        public bool Locked;
        public string NamespaceUri;
        public string Type;

        public EventListenerMapEntry()
        {
        }

        public EventListenerMapEntry(string namespaceUri, string type, object group,
            EventListener listener, bool locked)
        {
            NamespaceUri = namespaceUri;
            Type = type;
            Group = group;
            Listener = listener;
            Locked = locked;
        }
    }
}