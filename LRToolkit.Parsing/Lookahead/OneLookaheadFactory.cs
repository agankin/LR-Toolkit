using LRToolkit.GrammarDefinition;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing
{
    public class OneLookaheadFactory<TSymbol> : ILookaheadFactory<TSymbol> where TSymbol : notnull
    {
        private const int LookaheadIdx = 1;

        private readonly Grammar<TSymbol> _grammar;

        public OneLookaheadFactory(Grammar<TSymbol> grammar) => _grammar = grammar;

        public Option<SymbolLookahead<TSymbol>> GetAhead(Item<TSymbol> item) =>
            item.GetSymbolAhead()
                .FlatMap(symbol => item.GetSymbolAhead(LookaheadIdx).Map(lookahead => (symbol, lookahead)))
                .Map(symbolLookahead =>
                {
                    var (symbol, lookahead) = symbolLookahead;
                    return new SymbolLookahead<TSymbol>(symbol, new OneLookahead<TSymbol>(lookahead));
                });

        public IReadOnlySet<ILookahead<TSymbol>> GetFullSet(ILookahead<TSymbol> lookahead)
        {
            var lookaheadSymbol = lookahead[0].ValueOrFailure();

            IEnumerable<ILookahead<TSymbol>> SelectFirstSymbolsInProductions(TSymbol symbol) =>
                _grammar[symbol].Select(rule => new OneLookahead<TSymbol>(rule.Production.First));

            var lookaheadProductions = lookaheadSymbol.MapByType(
                mapSymbol: SelectFirstSymbolsInProductions,
                mapLookaheadSymbol: SelectFirstSymbolsInProductions,
                mapEnd: () => Enumerable.Empty<ILookahead<TSymbol>>());
                
            return lookaheadProductions.Prepend(lookahead).ToHashSet<ILookahead<TSymbol>>();
        }
    }
}