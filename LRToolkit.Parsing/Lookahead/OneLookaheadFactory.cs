using LRToolkit.GrammarDefinition;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public class OneLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        private const int LookaheadIdx = 1;

        public ILookahead<TSymbol> CreateForSymbolAhead(Item<TSymbol> item) =>
            new OneLookahead<TSymbol>(item.GetSymbolAhead(LookaheadIdx).ValueOrFailure());

        public ILookahead<TSymbol> CreateForSymbolProduction(Production<TSymbol> symbolProduction, ILookahead<TSymbol> symbolLookahead)
        {
            var productionFirstLookahead = symbolProduction[LookaheadIdx]
                .Map(Symbol<TSymbol>.CreateLookahead)
                .ValueOr(() => GetSymbol(symbolLookahead));

            return new OneLookahead<TSymbol>(productionFirstLookahead);
        }

        public IReadOnlySet<ILookahead<TSymbol>> CreateFullSet(ILookahead<TSymbol> lookahead, Grammar<TSymbol> grammar) =>
            GetSymbol(lookahead).MapByType(
                symbol => GetWithFirstSymbolsInProductions(symbol, grammar)
                    .Select(possibleSymbol => new OneLookahead<TSymbol>(possibleSymbol))
                    .ToHashSet<ILookahead<TSymbol>>(),
                symbol => GetWithFirstSymbolsInProductions(symbol, grammar)
                    .Select(possibleSymbol => new OneLookahead<TSymbol>(possibleSymbol))
                    .ToHashSet<ILookahead<TSymbol>>(),
                () => new HashSet<ILookahead<TSymbol>> { lookahead });

        private Symbol<TSymbol> GetSymbol(ILookahead<TSymbol> lookahead) => lookahead[0].ValueOrFailure();

        private IReadOnlySet<TSymbol> GetWithFirstSymbolsInProductions(TSymbol symbol, Grammar<TSymbol> grammar) =>
            grammar[symbol]
                .Select(productionRule => productionRule.Production.First)
                .Prepend(symbol)
                .ToHashSet();
    }
}