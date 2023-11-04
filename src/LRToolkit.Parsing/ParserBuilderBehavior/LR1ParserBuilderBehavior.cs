using LRToolkit.GrammarDefinition;

namespace LRToolkit.Parsing;

public class LR1ParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
    private readonly IItemSetMerger<TSymbol> _itemSetMerger;

    public LR1ParserBuilderBehavior(Grammar<TSymbol> grammar)
    {
        _lookaheadFactory = new OneLookaheadFactory<TSymbol>(grammar);
        _itemSetMerger = new LR1ItemSetMerger<TSymbol>();
    }

    public ILookaheadFactory<TSymbol> GetLookaheadFactory() => _lookaheadFactory;

    public IItemSetMerger<TSymbol> GetItemSetMerger() => _itemSetMerger;
}