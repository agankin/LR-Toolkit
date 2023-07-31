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

        public ILookahead<TSymbol> GetStart() => new OneLookahead<TSymbol>(Symbol<TSymbol>.End());

        public Option<ILookahead<TSymbol>> GetAhead(Item<TSymbol> item) =>
            item.GetSymbolAhead(LookaheadIdx).Map<ILookahead<TSymbol>>(symbol => new OneLookahead<TSymbol>(symbol));

        public IEnumerable<ILookahead<TSymbol>> Produce(ILookahead<TSymbol> lookahead)
        {
            var lookaheadSymbol = lookahead[0].ValueOrFailure();

            IEnumerable<ILookahead<TSymbol>> SelectFirstSymbolsInProductions(TSymbol symbol) =>
                _grammar[symbol].Select(rule => new OneLookahead<TSymbol>(rule.Production.First));

            var lookaheadProductions = lookaheadSymbol.Value
                .Map(SelectFirstSymbolsInProductions)
                .ValueOr(() => Enumerable.Empty<ILookahead<TSymbol>>());
                
            return lookaheadProductions.Prepend(lookahead).Distinct();
        }
    }
}