using Optional;

namespace LRBee.Utilities.Extensions
{
    public static class OptionalExtensions
    {
        public static bool SomeEquals<TValue>(this Option<TValue> valueOpt, TValue equalsToValue) =>
            valueOpt.Match(
                value => EqualityComparer<TValue>.Default.Equals(value, equalsToValue),
                () => false);

        public static IEnumerable<TItem> SelectSome<TItem>(this IEnumerable<Option<TItem>> items) =>
            items
                .Where(item => item.Match(_ => true, () => false))
                .Select(item => item.Match(
                    value => value,
                    () => throw new InvalidOperationException("Item must be Some.")));
    }
}