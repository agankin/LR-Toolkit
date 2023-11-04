using Optional;

namespace LRToolkit.Parsing;

public interface IItemSetMerger<TSymbol> where TSymbol : notnull
{
    bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second);

    int GetMergeableHashCode(ItemSet<TSymbol> itemSet);

    Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second);
}