using PureMonads;

namespace LRToolkit.Utilities;

public static class EnumerableExtensions
{
    public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem> handler) => items.ForEach(AsNothingFunc(handler));

    public static void ForEach<TItem, THandlerResult>(this IEnumerable<TItem> items, Func<TItem, THandlerResult> handler) =>
        items.Select(handler).LastOrDefault();

    public static void ForEach<TItem>(this IEnumerable<TItem> items, Action<TItem, int> handler)
        => items.Aggregate(0, InvokeWithIndex(handler));

    private static Func<int, TItem, int> InvokeWithIndex<TItem>(Action<TItem, int> handler)
        => (index, item) =>
        {
            handler(item, index);
            return ++index;
        };

    private static Func<TArg, Nothing> AsNothingFunc<TArg>(Action<TArg> handler) =>
        arg =>
        {
            handler(arg);
            return new Nothing();
        };
}