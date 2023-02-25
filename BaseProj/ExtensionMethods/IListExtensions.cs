using System;
using System.Collections.Generic;
using System.Linq;

namespace BaseProj.ExtensionMethods
{
    public static class IListExtensions
    {
        public static List<IList<T>> Split<T>(this IList<T> coll, Func<T, bool> condition)
        {
            var rList = new List<IList<T>>();
            var curList = new List<T>();

            foreach (var item in coll)
                if (condition.Invoke(item))
                {
                    if (curList.Any()) rList.Add(curList);
                    curList = new List<T>();
                }
                else
                {
                    curList.Add(item);
                }

            if (curList.Any()) rList.Add(curList);

            return rList;
        }

        public static void AddIfNotContains<T>(this IList<T> coll, T item)
        {
            if (coll.Contains(item))
                return;

            coll.Add(item);
        }
    }
}