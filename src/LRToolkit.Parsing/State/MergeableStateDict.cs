using LRToolkit.Utilities;
using PureMonads;

namespace LRToolkit.Parsing;

internal class MergeableStateDict<TSymbol> where TSymbol : notnull
{
    private readonly IDictionary<ItemSet<TSymbol>, State<TSymbol>> _mergeableStates;

    public MergeableStateDict(IItemSetMerger<TSymbol> itemSetMerger)
    {
        var comparer = new FuncEqualityComparer<ItemSet<TSymbol>>(itemSetMerger.IsMergeable, itemSetMerger.GetMergeableHashCode);
        _mergeableStates = new Dictionary<ItemSet<TSymbol>, State<TSymbol>>(comparer);
    }

    public Option<State<TSymbol>> GetMergeableOrNone(ItemSet<TSymbol> itemSet) => _mergeableStates.GetOrNone(itemSet);

    public void AddNew(State<TSymbol> state, ItemSet<TSymbol> itemSet) => _mergeableStates.Add(itemSet, state);
}