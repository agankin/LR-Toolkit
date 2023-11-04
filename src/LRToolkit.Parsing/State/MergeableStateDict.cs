using LRToolkit.Utilities;
using Optional;
using Optional.Collections;

namespace LRToolkit.Parsing;

internal class MergeableStateDict<TSymbol> where TSymbol : notnull
{
    private readonly IDictionary<ItemSet<TSymbol>, State<TSymbol>> _mergeableStates;

    public MergeableStateDict(IItemSetMerger<TSymbol> itemSetMerger)
    {
        var comparer = new FuncEqualityComparer<ItemSet<TSymbol>>(itemSetMerger.IsMergeable);
        _mergeableStates = new Dictionary<ItemSet<TSymbol>, State<TSymbol>>(comparer);
    }

    public Option<State<TSymbol>> GetMergeableOrNone(ItemSet<TSymbol> itemSet) => _mergeableStates.GetValueOrNone(itemSet);

    public void AddNew(State<TSymbol> state, ItemSet<TSymbol> itemSet) => _mergeableStates.Add(itemSet, state);
}