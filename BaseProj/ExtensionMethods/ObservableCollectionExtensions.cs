using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace BaseProj.ExtensionMethods
{
    public static class ObservableCollectionExtensions
    {
        public static int Remove<T>(this ObservableCollection<T> coll, Func<T, bool> condition)
        {
            var itemsToRemove = coll.Where(condition).ToList();

            foreach (var itemToRemove in itemsToRemove) coll.Remove(itemToRemove);

            return itemsToRemove.Count;
        }

        public static void ForEach<T>(this ObservableCollection<T> coll, Action<T> action)
        {
            foreach (var item in coll) action(item);
        }

        /// <summary>
        ///     Sets reqiured item at specified position. If count is lesser that index fills collaction dith default(T) and then
        ///     sets specific item at index
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="coll"></param>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public static void SetItemAt<T>(this ObservableCollection<T> coll, int index, T item)
        {
            while (coll.Count <= index) coll.Add(default);

            coll[index] = item;
        }
    }
}