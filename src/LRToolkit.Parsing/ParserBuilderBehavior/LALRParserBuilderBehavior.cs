using LRToolkit.Grammaring;

namespace LRToolkit.Parsing;

public class LALRParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
    private readonly IItemSetMerger<TSymbol> _itemSetMerger;

    public LALRParserBuilderBehavior(Grammar<TSymbol> grammar)
    {
        _lookaheadFactory = new OneLookaheadFactory<TSymbol>(grammar);
        _itemSetMerger = new LALRItemSetMerger<TSymbol>(grammar, _lookaheadFactory);
    }

    public ILookaheadFactory<TSymbol> GetLookaheadFactory() => _lookaheadFactory;

    public IItemSetMerger<TSymbol> GetItemSetMerger() => _itemSetMerger;
}