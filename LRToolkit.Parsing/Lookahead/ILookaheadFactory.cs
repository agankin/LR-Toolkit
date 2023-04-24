using LRToolkit.GrammarDefinition;

namespace LRToolkit.Parsing
{
    public interface ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        ILookahead<TSymbol> CreateForStart();

        ILookahead<TSymbol> CreateForSymbolAhead(Item<TSymbol> item);

        ILookahead<TSymbol> CreateForSymbolProduction(Production<TSymbol> symbolProduction, ILookahead<TSymbol> symbolLookahead);

        IReadOnlySet<ILookahead<TSymbol>> CreateFullSet(ILookahead<TSymbol> lookahead, Grammar<TSymbol> grammar);
    }
}