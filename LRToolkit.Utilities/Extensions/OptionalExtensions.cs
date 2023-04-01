using Optional;
using Optional.Unsafe;

namespace LRToolkit.Utilities
{
    public static class OptionalExtensions
    {
        public static bool SomeEquals<TValue>(
            this Option<TValue> valueOption,
            TValue valueToCompare,
            IEqualityComparer<TValue>? comparer = null)
        {
            comparer = comparer ?? EqualityComparer<TValue>.Default;

            return valueOption
                .Map(value => comparer.Equals(value, valueToCompare))
                .ValueOr(false);
        }

        public static IEnumerable<TResult> OnlySome<TItem, TResult>(
            this IEnumerable<TItem> items,
            Func<TItem, Option<TResult>> selector)
        {
            return items.Select(selector)
                .Where(option => option.HasValue)
                .Select(item => item.ValueOrFailure());
        }
    }
}