using System.Collections.Immutable;

namespace LRToolkit.Parsing;

internal class StepItemSet<TSymbol> where TSymbol : notnull
{
    public StepItemSet(ItemSet<TSymbol> itemSet)
    {
        ItemSet = itemSet;

        (ShiftItems, ReduceItems) = itemSet.Kernels.Aggregate(
            ShiftReduceItems.Empty,
            (shiftReduce, item) => item.HasSymbolAhead() 
                ? shiftReduce with { ShiftItems = shiftReduce.ShiftItems.Add(item) }
                : shiftReduce with { ReduceItems = shiftReduce.ReduceItems.Add(item) });
    }

    public ItemSet<TSymbol> ItemSet { get; }

    public IReadOnlyList<Item<TSymbol>> ShiftItems { get; }

    public IReadOnlyList<Item<TSymbol>> ReduceItems { get; }

    private readonly record struct ShiftReduceItems(
        ImmutableList<Item<TSymbol>> ShiftItems,
        ImmutableList<Item<TSymbol>> ReduceItems
    )
    {
        public static readonly ShiftReduceItems Empty = new ShiftReduceItems(
            ImmutableList<Item<TSymbol>>.Empty,
            ImmutableList<Item<TSymbol>>.Empty);
    }
}