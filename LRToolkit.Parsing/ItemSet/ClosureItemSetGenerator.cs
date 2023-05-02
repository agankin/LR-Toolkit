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
            var symbolAheads = GetSymbolAheadWithLookaheads(kernelItem);
            var closureItems = symbolAheads.SelectMany(GetSymbolProductions).ToHashSet();

            return new ItemSet<TSymbol>(closureItems);
        }

        public ItemSet<TSymbol> GetClosureItems(ItemSet<TSymbol> kernelItemSet)
        {
            var symbolsAhead = kernelItemSet.SelectMany(GetSymbolAheadWithLookaheads).ToHashSet();
            var nextSymbolsItems = symbolsAhead.SelectMany(GetSymbolProductions).ToHashSet();

            return new ItemSet<TSymbol>(nextSymbolsItems);
        }

        private IEnumerable<Item<TSymbol>> GetSymbolProductions(SymbolWithLookahead symbolWithLookahead) =>
            GetSymbolProductions(symbolWithLookahead, new HashSet<TSymbol>());

        private IEnumerable<Item<TSymbol>> GetSymbolProductions(SymbolWithLookahead symbolWithLookahead, ISet<TSymbol> processedSymbols)
        {
            var (symbol, symbolLookahead) = symbolWithLookahead;

            processedSymbols.Add(symbol);

            var symbolProductionRules = _grammar[symbol];
            var firstSymbols = symbolProductionRules
                .Select(productionRule => productionRule.Production)
                .Where(production => !processedSymbols.Contains(production.First))
                .SelectMany(production => GetFirstWithLookaheads(production, symbolLookahead))
                .ToHashSet();

            var items = symbolProductionRules.Select(rule => Item<TSymbol>.FromRule(rule, symbolLookahead));
            var firstSymbolsItems = firstSymbols.SelectMany(next => GetSymbolProductions(next, processedSymbols));

            return items.Concat(firstSymbolsItems);
        }

        private IEnumerable<SymbolWithLookahead> GetSymbolAheadWithLookaheads(Item<TSymbol> item)
        {
            if (!item.HasProductionSymbolAhead())
                return Enumerable.Empty<SymbolWithLookahead>();

            return item.GetSymbolAhead()
                .Map(symbol => symbol.MapByType(
                    symbolValue => GetWithFullLookaheadSet(symbolValue, GetLookahead(item)),
                    symbolValue => GetWithFullLookaheadSet(symbolValue, GetLookahead(item)),
                    () => Enumerable.Empty<SymbolWithLookahead>()))
                .ValueOr(Enumerable.Empty<SymbolWithLookahead>());
        }

        private IEnumerable<SymbolWithLookahead> GetFirstWithLookaheads(Production<TSymbol> symbolProduction, ILookahead<TSymbol> symbolLookahead)
        {
            var first = symbolProduction.First;
            var lookahead = _lookaheadFactory.CreateForSymbolProduction(symbolProduction, symbolLookahead);

            return GetWithFullLookaheadSet(first, lookahead);
        }

        private ILookahead<TSymbol> GetLookahead(Item<TSymbol> item) => _lookaheadFactory.CreateForSymbolAhead(item);

        private IEnumerable<SymbolWithLookahead> GetWithFullLookaheadSet(TSymbol symbol, ILookahead<TSymbol> lookahead) =>
            _lookaheadFactory.CreateFullSet(lookahead, _grammar)
                .Select(lookaheadFromFullSet => new SymbolWithLookahead(symbol, lookaheadFromFullSet));

        private record SymbolWithLookahead(TSymbol Symbol, ILookahead<TSymbol> Lookahead);
    }
}