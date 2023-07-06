using LRToolkit.GrammarDefinition;
using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing
{
    internal class ClosureProducer<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;
        private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;

        public ClosureProducer(Grammar<TSymbol> grammar, ILookaheadFactory<TSymbol> lookaheadFactory)
        {
            _grammar = grammar;
            _lookaheadFactory = lookaheadFactory;
        }

        public ItemSet<TSymbol> Produce(Item<TSymbol> kernelItem)
        {
            var symbolAheadOption = GetAhead(kernelItem);
            var producedClosures = symbolAheadOption.Map(symbolAhead => ProduceClosures(symbolAhead).ToHashSet())
                .ValueOr(new HashSet<Item<TSymbol>>());

            return new ItemSet<TSymbol>(producedClosures);
        }

        public ItemSet<TSymbol> Produce(ItemSet<TSymbol> kernelItemSet)
        {
            var symbolsAhead = kernelItemSet.OnlySome(GetAhead).Distinct();
            var nextSymbolsItems = symbolsAhead.SelectMany(ProduceClosures).ToHashSet();

            return new ItemSet<TSymbol>(nextSymbolsItems);
        }

        private Option<SymbolAhead> GetAhead(Item<TSymbol> item)
        {
            var symbolOption = item.GetSymbolAhead().FlatMap(symbolAhead => symbolAhead.Value);
            var lookaheadOption = _lookaheadFactory.GetAhead(item);

            return symbolOption.FlatMap(symbol => lookaheadOption.Map(lookahead => new SymbolAhead(symbol, lookahead)));
        }

        private IEnumerable<Item<TSymbol>> ProduceClosures(SymbolAhead symbolAhead) =>
            ProduceClosures(symbolAhead, new HashSet<TSymbol>());

        private IEnumerable<Item<TSymbol>> ProduceClosures(SymbolAhead symbolAhead, ISet<TSymbol> processedSymbols)
        {
            var (symbol, lookahead) = symbolAhead;
            processedSymbols.Add(symbol);
            
            var symbolProductions = _grammar[symbol];
            var producedLookaheads = _lookaheadFactory.Produce(lookahead).ToList();
            
            var closures = symbolProductions
                .SelectMany(rule => producedLookaheads.Select(lookahead => Item<TSymbol>.FromRule(rule, lookahead)));

            var producedSymbols = symbolProductions.Select(rule => rule.Production.First)
                .Where(symbol => !processedSymbols.Contains(symbol))
                .Distinct();
            var producedClosures = producedSymbols
                .SelectMany(symbol => producedLookaheads.Select(lookahead => new SymbolAhead(symbol, lookahead)))
                .SelectMany(symbolAhead => ProduceClosures(symbolAhead, processedSymbols));

            return closures.Concat(producedClosures);
        }

        private readonly record struct SymbolAhead(
            TSymbol Symbol,
            ILookahead<TSymbol> Lookahead
        );
    }
}