using Optional;

namespace LRToolkit.Parsing;

public interface IItemSetMerger<TSymbol> where TSymbol : notnull
{
    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second);

    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second);
}