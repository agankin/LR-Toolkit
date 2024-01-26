using Optional;

namespace LRToolkit.Parsing;

public interface ILookaheadFactory<TSymbol> where TSymbol : notnull
{
    ILookahead<TSymbol> GetForStart();
    
    Option<ILookahead<TSymbol>> GetAhead(Item<TSymbol> item);

    IEnumerable<ILookahead<TSymbol>> Produce(ILookahead<TSymbol> symbolLookahead);
}