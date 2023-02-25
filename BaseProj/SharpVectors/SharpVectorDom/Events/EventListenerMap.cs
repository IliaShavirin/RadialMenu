using System;
using BaseProj.SharpVectors.SharpVectorCore.Events;

namespace BaseProj.SharpVectors.SharpVectorDom.Events
{
    /// <summary>
    ///     Summary description for EventListenerMap.
    /// </summary>
    public class EventListenerMap
    {
        #region Private Helpers

        private EventListenerMapEntry[] GrowBy(
            int growth)
        {
            if (entries == null)
            {
                entries = new EventListenerMapEntry[
                    growth * GrowthFactor + GrowthBuffer];

                count = 0;
                locked = false;

                return entries;
            }

            var newCount = count + growth;

            if (newCount > entries.Length)
            {
                newCount = newCount * GrowthFactor + GrowthBuffer;

                var newEntries =
                    new EventListenerMapEntry[newCount];

                Array.Copy(entries, 0, newEntries, 0, entries.Length);

                entries = newEntries;
            }

            return entries;
        }

        #endregion

        #region Private Fields

        private const int GrowthBuffer = 8;
        private const int GrowthFactor = 2;

        private EventListenerMapEntry[] entries;
        private int count;
        private bool locked;

        #endregion

        #region Public Methods

        public void AddEventListener(
            string namespaceUri,
            string eventType,
            object eventGroup,
            EventListener listener)
        {
            var entries = GrowBy(1);

            for (var i = 0; i < count; i++)
            {
                if (namespaceUri != entries[i].NamespaceUri) continue;

                if (eventType != entries[i].Type) continue;

                if (listener == entries[i].Listener) return;
            }

            entries[count] = new EventListenerMapEntry(
                namespaceUri, eventType, eventGroup, listener, locked);

            count++;
        }

        public void RemoveEventListener(
            string namespaceUri,
            string eventType,
            EventListener listener)
        {
            if (entries == null) return;

            for (var i = 0; i < count; i++)
            {
                if (namespaceUri != entries[i].NamespaceUri) continue;

                if (eventType != entries[i].Type) continue;

                if (listener == entries[i].Listener)
                {
                    count--;
                    entries[i] = entries[count];
                    entries[count] = new EventListenerMapEntry();

                    return;
                }
            }
        }

        public void FireEvent(
            IEvent @event)
        {
            var namespaceUri = @event.NamespaceUri;
            var eventType = @event.Type;
            for (var i = 0; i < count; i++)
            {
                // Check if the entry was added during this phase
                if (entries[i].Locked)
                    continue;

                var entryNamespaceUri = entries[i].NamespaceUri;
                var entryEventType = entries[i].Type;

                if (entryNamespaceUri != null && namespaceUri != null)
                    if (entryNamespaceUri != namespaceUri)
                        continue;

                if (entryEventType != eventType) continue;

                entries[i].Listener(@event);
            }
        }

        public bool HasEventListenerNs(
            string namespaceUri,
            string eventType)
        {
            if (entries == null) return false;

            for (var i = 0; i < count; i++)
            {
                if (namespaceUri != entries[i].NamespaceUri) continue;

                if (eventType != entries[i].Type) continue;

                return true;
            }

            return false;
        }

        public void Lock()
        {
            locked = true;
        }

        public void Unlock()
        {
            // Unlock the map
            locked = false;

            // Unlock pending entries
            for (var i = 0; i < count; i++) entries[i].Locked = false;
        }

        #endregion
    }
}