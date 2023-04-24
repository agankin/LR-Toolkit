using LRToolkit.GrammarDefinition;

namespace LRToolkit.Parsing
{
    public class NoLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        public ILookahead<TSymbol> CreateForStart() => NoLookahead<TSymbol>.Instance;

        public ILookahead<TSymbol> CreateForSymbolAhead(Item<TSymbol> item) => NoLookahead<TSymbol>.Instance;

        public ILookahead<TSymbol> CreateForSymbolProduction(Production<TSymbol> symbolProduction, ILookahead<TSymbol> symbolLookahead) =>
            NoLookahead<TSymbol>.Instance;

        public IReadOnlySet<ILookahead<TSymbol>> CreateFullSet(ILookahead<TSymbol> lookahead, Grammar<TSymbol> grammar) =>
            new HashSet<ILookahead<TSymbol>> { lookahead };
    }
}