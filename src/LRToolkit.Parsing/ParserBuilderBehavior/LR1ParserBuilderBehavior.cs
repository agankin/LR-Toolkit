using LRToolkit.GrammarDefinition;
using Optional;

namespace LRToolkit.Parsing;

public class LR1ParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    public LR1ParserBuilderBehavior(Grammar<TSymbol> grammar)
    {
        LookaheadFactory = new OneLookaheadFactory<TSymbol>(grammar);
    }

    public ILookaheadFactory<TSymbol> LookaheadFactory { get; }

    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second) => first == second;

    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second) =>
        first.Some<ItemSet<TSymbol>, BuilderError>();
}