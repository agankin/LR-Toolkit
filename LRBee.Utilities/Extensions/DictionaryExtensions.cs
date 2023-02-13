namespace LRBee.Utilities.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(
            this IReadOnlyDictionary<TKey, TValue> dictionary,
            TKey key,
            Func<TValue> onNotFound) =>
            dictionary.ContainsKey(key) ? dictionary[key] : onNotFound();
    }
}