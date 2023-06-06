using LRToolkit.GrammarDefinition;
using Optional;

namespace LRToolkit.Parsing
{
    public class NoLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        public Option<SymbolLookahead<TSymbol>> GetAhead(Item<TSymbol> item) =>
            item.GetSymbolAhead()
                .Map(symbolAhead => new SymbolLookahead<TSymbol>(symbolAhead, NoLookahead<TSymbol>.Instance));

        public IReadOnlySet<ILookahead<TSymbol>> GetFullSet(ILookahead<TSymbol> symbolLookahead)
        {
            var lookahead = NoLookahead<TSymbol>.Instance;

            return new HashSet<ILookahead<TSymbol>> { lookahead };
        }
    }
}