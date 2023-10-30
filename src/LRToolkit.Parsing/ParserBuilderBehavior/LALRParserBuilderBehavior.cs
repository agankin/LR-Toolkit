using LRToolkit.GrammarDefinition;
using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing;

public class LALRParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
    private readonly ClosureProducer<TSymbol> _closureProducer;

    public LALRParserBuilderBehavior(Grammar<TSymbol> grammar)
    {
        _lookaheadFactory = new OneLookaheadFactory<TSymbol>(grammar);
        _closureProducer = new ClosureProducer<TSymbol>(grammar, _lookaheadFactory);
    }

    public ILookaheadFactory<TSymbol> GetLookaheadFactory() => _lookaheadFactory;

    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second)
    {
        var comparer = new FuncEqualityComparer<Item<TSymbol>>(IsMergeable);
        
        var firstSet = new HashSet<Item<TSymbol>>(first.Kernels, comparer);
        var secondSet = new HashSet<Item<TSymbol>>(second.Kernels, comparer);
        
        return firstSet.SetEquals(secondSet);
    }

    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second)
    {
        var kernels = first.Kernels.Concat(second.Kernels).ToHashSet();
        var closures = _closureProducer.Produce(kernels);

        return new ItemSet<TSymbol>(kernels, closures).Some<ItemSet<TSymbol>, BuilderError>();
    }

    private static bool IsMergeable(Item<TSymbol> first, Item<TSymbol> second) =>
        first.Position == second.Position
            && first.ForSymbol.Equals(second.ForSymbol)
            && first.Production.Equals(second.Production);
}