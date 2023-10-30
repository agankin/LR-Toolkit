using Optional;

namespace LRToolkit.Parsing;

public interface ILRParserBuilderBehavior<TSymbol> where TSymbol : notnull
{
    public ILookaheadFactory<TSymbol> GetLookaheadFactory();

    public bool IsMergeable(ItemSet<TSymbol> first, ItemSet<TSymbol> second);
    
    public Option<ItemSet<TSymbol>, BuilderError> Merge(ItemSet<TSymbol> first, ItemSet<TSymbol> second);
}