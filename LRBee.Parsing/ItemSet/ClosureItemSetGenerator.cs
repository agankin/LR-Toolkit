using LRBee.GrammarDefinition;
using LRBee.Utilities;

namespace LRBee.Parsing
{
    public class ClosureItemSetGenerator<TSymbol> where TSymbol : notnull
    {
        private readonly Grammar<TSymbol> _grammar;

        public ClosureItemSetGenerator(Grammar<TSymbol> grammar) => _grammar = grammar;

        public ItemSet<TSymbol> GetClosureItems(Item<TSymbol> kernelItem)
        {
            var symbolAheadOption = kernelItem.GetSymbolAhead();
            var closureItems = symbolAheadOption
                .Map(GetSymbolItems)
                .ValueOr(Enumerable.Empty<Item<TSymbol>>());

            return new ItemSet<TSymbol>(closureItems);
        }

        public ItemSet<TSymbol> GetClosureItems(ItemSet<TSymbol> kernelItemSet)
        {
            var symbolsAhead = kernelItemSet.OnlySome(item => item.GetSymbolAhead()).Distinct();
            var nextSymbolsItems = symbolsAhead.SelectMany(GetSymbolItems).Distinct();

            return new ItemSet<TSymbol>(nextSymbolsItems);
        }

        private IEnumerable<Item<TSymbol>> GetSymbolItems(Symbol<TSymbol> symbol) =>
            symbol.MapSymbol(
                sym => GetSymbolItems(sym, new HashSet<TSymbol>()),
                Enumerable.Empty<Item<TSymbol>>);

        private IEnumerable<Item<TSymbol>> GetSymbolItems(TSymbol symbol, ISet<TSymbol> processedSymbols)
        {
            processedSymbols.Add(symbol);

            var rules = _grammar[symbol];
            var firstSymbols = rules.Select(rule => rule.Production.First)
                .Where(symbol => !processedSymbols.Contains(symbol));

            var items = rules.Select(Item<TSymbol>.FromRule);
            var nextSymbolsItems = firstSymbols.SelectMany(symbol => GetSymbolItems(symbol, processedSymbols));

            return items.Concat(nextSymbolsItems);
        }
    }
}