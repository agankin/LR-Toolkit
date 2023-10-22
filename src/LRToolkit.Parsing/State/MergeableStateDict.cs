using LRToolkit.Utilities;
using Optional;
using Optional.Collections;

namespace LRToolkit.Parsing;

internal class MergeableStateDict<TSymbol> where TSymbol : notnull
{
    private readonly IDictionary<ItemSet<TSymbol>, State<TSymbol>> _mergeableStates;

    public MergeableStateDict(ILRParserBuilderBehavior<TSymbol> builderBehavior)
    {
        var mergeableItemSetsComparer = new FuncEqualityComparer<ItemSet<TSymbol>>(builderBehavior.IsMergeable);
        _mergeableStates = new Dictionary<ItemSet<TSymbol>, State<TSymbol>>(mergeableItemSetsComparer);
    }

    public Option<State<TSymbol>> GetMergeableOrNone(ItemSet<TSymbol> itemSet) => _mergeableStates.GetValueOrNone(itemSet);

    public void AddNew(State<TSymbol> state, ItemSet<TSymbol> itemSet) => _mergeableStates.Add(itemSet, state);
}