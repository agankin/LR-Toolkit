using LRToolkit.GrammarDefinition;
using Optional;

namespace LRToolkit.Parsing;

public class LR1ParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;

    public LR1ParserBuilderBehavior(Grammar<TSymbol> grammar) =>
        _lookaheadFactory = new OneLookaheadFactory<TSymbol>(grammar);

    public ILookaheadFactory<TSymbol> GetLookaheadFactory() => _lookaheadFactory;

    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second) => first == second;

    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second) =>
        first.Some<ItemSet<TSymbol>, BuilderError>();
}