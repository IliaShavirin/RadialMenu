using System;
using System.Collections.Generic;

namespace BaseProj.ExtensionMethods
{
    public static class DictionaryExtensions
    {
        public static TValue TryGetValueEx<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key,
            bool createIfNotExists = false)
        {
            if (!dictionary.TryGetValue(key, out var t) && createIfNotExists)
            {
                t = (TValue)Activator.CreateInstance(typeof(TValue));
                dictionary[key] = t;
            }

            return t;
        }
    }
}