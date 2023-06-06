using LRToolkit.GrammarDefinition;

namespace LRToolkit.Parsing
{
    internal class ClosureItemSetGenerator<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;
        private readonly ILookaheadFactory<TSymbol> _lookaheadFactory;

        public ClosureItemSetGenerator(Grammar<TSymbol> grammar, ILookaheadFactory<TSymbol> lookaheadFactory)
        {
            _grammar = grammar;
            _lookaheadFactory = lookaheadFactory;
        }

        public ItemSet<TSymbol> GetClosureItems(Item<TSymbol> kernelItem)
        {
            var symbolAheads = GetSymbolAhead(kernelItem);
            var closureItems = symbolAheads.SelectMany(GetSymbolProductions).ToHashSet();

            return new ItemSet<TSymbol>(closureItems);
        }

        public ItemSet<TSymbol> GetClosureItems(ItemSet<TSymbol> kernelItemSet)
        {
            var symbolsAhead = kernelItemSet.SelectMany(GetSymbolAhead).ToHashSet();
            var nextSymbolsItems = symbolsAhead.SelectMany(GetSymbolProductions).ToHashSet();

            return new ItemSet<TSymbol>(nextSymbolsItems);
        }

        private IEnumerable<Item<TSymbol>> GetSymbolProductions(SymbolLookahead symbolLookahead) =>
            GetSymbolProductions(symbolLookahead, new HashSet<SymbolLookahead>());

        private IEnumerable<Item<TSymbol>> GetSymbolProductions(SymbolLookahead symbolLookahead, ISet<SymbolLookahead> processedSymbols)
        {
            processedSymbols.Add(symbolLookahead);

            var (symbol, lookahead) = symbolLookahead;
            
            var symbolProductionRules = _grammar[symbol];
            var firstSymbols = symbolProductionRules
                .Select(productionRule => productionRule.Production)
                .SelectMany(production => GetFirstInProduction(production, lookahead))
                .Where(symbolLookahead => !processedSymbols.Contains(symbolLookahead))
                .ToHashSet();

            var items = symbolProductionRules.Select(rule => Item<TSymbol>.FromRule(rule, lookahead));
            var firstSymbolsItems = firstSymbols.SelectMany(next => GetSymbolProductions(next, processedSymbols));

            return items.Concat(firstSymbolsItems);
        }

        private IEnumerable<SymbolLookahead> GetSymbolAhead(Item<TSymbol> item)
        {
            if (!item.HasProductionSymbolAhead())
                return Enumerable.Empty<SymbolLookahead>();

            return _lookaheadFactory.GetAhead(item)
                .Map(symbolLookahead => 
                {
                    var (symbol, lookahead) = symbolLookahead;

                    return symbol.MapByType(
                        plainSymbol => GetAllLookaheads(plainSymbol, lookahead),
                        _ => throw new Exception("Unexpected invocation for lookahead symbol."),
                        () => Enumerable.Empty<SymbolLookahead>());
                })
                .ValueOr(Enumerable.Empty<SymbolLookahead>());
        }

        private IEnumerable<SymbolLookahead> GetFirstInProduction(Production<TSymbol> symbolProduction, ILookahead<TSymbol> symbolLookahead)
        {
            var first = symbolProduction.First;

            return _lookaheadFactory.GetFullSet(symbolLookahead).Select(CreateSymbolLookahead(first));
        }

        private IEnumerable<SymbolLookahead> GetAllLookaheads(TSymbol symbol, ILookahead<TSymbol> lookahead) =>
            _lookaheadFactory.GetFullSet(lookahead).Select(CreateSymbolLookahead(symbol));

        private Func<ILookahead<TSymbol>, SymbolLookahead> CreateSymbolLookahead(TSymbol symbol) =>
            lookahead => new SymbolLookahead(symbol, lookahead);

        private readonly record struct SymbolLookahead(TSymbol Symbol, ILookahead<TSymbol> Lookahead);
    }
}