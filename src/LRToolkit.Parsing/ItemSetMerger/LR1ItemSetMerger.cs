using PureMonads;

namespace LRToolkit.Parsing;

public class LR1ItemSetMerger<TSymbol> : IItemSetMerger<TSymbol> where TSymbol : notnull
{
    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second) => first == second;

    public int GetMergeableHashCode(ItemSet<TSymbol> itemSet) => itemSet.GetHashCode();

    public Result<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second) => first;
}