using PureMonads;

namespace LRToolkit.Utilities
{
    public static class DictionaryExtensions
    {
        public static Option<TValue> GetOrNone<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var value) ? value : Option.None<TValue>();
        }

        public static Option<TValue> GetOrNone<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key)
        {
            return dict.TryGetValue(key, out var value) ? value : Option.None<TValue>();
        }

        public static Option<TValue> GetOrNone<TKey, TValue>(this Dictionary<TKey, TValue> dict, TKey key)
            where TKey : notnull
        {
            return GetOrNone((IReadOnlyDictionary<TKey, TValue>)dict, key);
        }
    }
}