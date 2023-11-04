using Optional;

namespace LRToolkit.Parsing;

public class LR1ItemSetMerger<TSymbol> : IItemSetMerger<TSymbol> where TSymbol : notnull
{
    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second) => first == second;

    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second) =>
        first.Some<ItemSet<TSymbol>, BuilderError>();
}