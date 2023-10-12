using Optional;
using Optional.Unsafe;

namespace LRToolkit.Utilities;

public static class OptionExtensions
{
    public static Option<TValue> MapNone<TValue>(this Option<TValue> valueOption, Func<Option<TValue>> mapNone) =>
        valueOption.Match(value => value.Some(), mapNone);

    public static bool SomeEquals<TValue>(this Option<TValue> valueOption, TValue valueToCompare, IEqualityComparer<TValue>? comparer = null)
    {
        comparer = comparer ?? EqualityComparer<TValue>.Default;
        return valueOption.Map(value => comparer.Equals(value, valueToCompare)).ValueOr(false);
    }

    public static IEnumerable<TResult> SelectOnlySome<TItem, TResult>(this IEnumerable<TItem> items, Func<TItem, Option<TResult>> selector) =>
        items.Select(selector).Where(option => option.HasValue).Select(item => item.ValueOrFailure());
}