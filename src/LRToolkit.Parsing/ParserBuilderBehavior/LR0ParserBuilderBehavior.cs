namespace LRToolkit.Parsing;

public class LR0ParserBuilderBehavior<TSymbol> : ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;
    private readonly IItemSetMerger<TSymbol> _itemSetMerger;

    public LR0ParserBuilderBehavior()
    {
        _lookaheadFactory = new NoLookaheadFactory<TSymbol>();
        _itemSetMerger = new LR0ItemSetMerger<TSymbol>();
    }

    public ILookaheadFactory<TSymbol> GetLookaheadFactory() => _lookaheadFactory;

    public IItemSetMerger<TSymbol> GetItemSetMerger() => _itemSetMerger;
}