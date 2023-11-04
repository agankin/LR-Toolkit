namespace LRToolkit.Parsing;

public interface ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    ILookaheadFactory<TSymbol> GetLookaheadFactory();

    IItemSetMerger<TSymbol> GetItemSetMerger();
}