using LRToolkit.Grammaring;
using LRToolkit.Utilities;
using PureMonads;

namespace LRToolkit.Parsing;

public class LALRItemSetMerger<TSymbol> : IItemSetMerger<TSymbol> where TSymbol : notnull
{
    private readonly ClosureProducer<TSymbol> _closureProducer;

    public LALRItemSetMerger(Grammar<TSymbol> grammar, ILookaheadFactory<TSymbol> lookaheadFactory)
    {
        _closureProducer = new ClosureProducer<TSymbol>(grammar, lookaheadFactory);
    }

    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second)
    {
        var comparer = new FuncEqualityComparer<Item<TSymbol>>(IsMergeable, GetMergeableHashCode);
        
        var firstSet = new HashSet<Item<TSymbol>>(first.Kernels, comparer);
        var secondSet = new HashSet<Item<TSymbol>>(second.Kernels, comparer);
        
        return firstSet.SetEquals(secondSet);
    }

    public int GetMergeableHashCode(ItemSet<TSymbol> itemSet) => Hash.FNV(itemSet, GetMergeableHashCode);

    public Result<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second)
    {
        var kernels = first.Kernels.Concat(second.Kernels).ToHashSet();
        var closures = _closureProducer.Produce(kernels);

        return new ItemSet<TSymbol>(kernels, closures);
    }

    private static bool IsMergeable(Item<TSymbol> first, Item<TSymbol> second) =>
        first.Position == second.Position
            && first.ForSymbol.Equals(second.ForSymbol)
            && first.Production.Equals(second.Production);

    private static int GetMergeableHashCode(Item<TSymbol> item)
    {
        var hashComponents = new[] { item.Position, item.ForSymbol.GetHashCode(), item.Production.GetHashCode() };
        return Hash.FNV(hashComponents, getHashCode: hashCode => hashCode);
    }
}