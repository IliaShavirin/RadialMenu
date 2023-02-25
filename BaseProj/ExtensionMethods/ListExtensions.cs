using System.Collections.Generic;

namespace BaseProj.ExtensionMethods
{
    public static class ListExtensions
    {
        public static void SetItemAt<T>(this List<T> coll, int index, T item)
        {
            while (coll.Count <= index) coll.Add(default);

            coll[index] = item;
        }
    }
}