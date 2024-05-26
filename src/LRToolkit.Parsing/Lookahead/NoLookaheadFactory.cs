using PureMonads;

namespace LRToolkit.Parsing;

public class NoLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
{
    public ILookahead<TSymbol> GetForStart() => NoLookahead<TSymbol>.Instance;

    public Option<ILookahead<TSymbol>> GetAhead(Item<TSymbol> item) =>
        NoLookahead<TSymbol>.Instance.Some<ILookahead<TSymbol>>();

    public IEnumerable<ILookahead<TSymbol>> Produce(ILookahead<TSymbol> symbolLookahead)
    {
        yield return NoLookahead<TSymbol>.Instance;
    }
}