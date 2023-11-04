namespace LRToolkit.Parsing;

public interface ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    public ILookaheadFactory<TSymbol> GetLookaheadFactory();

    public IItemSetMerger<TSymbol> GetItemSetMerger();
}