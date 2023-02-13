using LRBee.GrammarDefinition;
using LRBee.Utilities.Extensions;

namespace LRBee.Parsing.ItemBuilding
{
    public class ClosureItemSetBuilder<TSymbol>
    {
        private readonly Grammar<TSymbol> _grammar;

        public ClosureItemSetBuilder(Grammar<TSymbol> grammar)
        {
            _grammar = grammar;
        }

        public ItemSet<TSymbol> GetClosure(Item<TSymbol> item)
        {
            var (production, position) = item;
            var symbol = production[position];

            var closureItems = GetClosureItems(symbol).Prepend(item);

            return new ItemSet<TSymbol>(closureItems);
        }

        public ItemSet<TSymbol> GetClosure(ItemSet<TSymbol> itemSet)
        {
            var closureItems = GetClosureItems(itemSet);

            return new ItemSet<TSymbol>(closureItems);
        }

        private IEnumerable<Item<TSymbol>> GetClosureItems(IEnumerable<Item<TSymbol>> items)
        {
            var nextSymbols = items.Select(item => item.GetNextSymbol())
                .SelectSome()
                .Distinct();
            var nextSymbolsItems = nextSymbols.SelectMany(GetClosureItems).Distinct();

            return items.Concat(nextSymbolsItems);
        }

        private IEnumerable<Item<TSymbol>> GetClosureItems(TSymbol symbol) =>
            GetClosureItems(symbol, new HashSet<TSymbol>());

        private IEnumerable<Item<TSymbol>> GetClosureItems(TSymbol symbol, ISet<TSymbol> addedSymbolsSet)
        {
            addedSymbolsSet.Add(symbol);

            var symbolProductions = _grammar[symbol];
            var nextSymbols = symbolProductions
                .Select(production => production[0])
                .Where(symbol => !addedSymbolsSet.Contains(symbol));

            var items = symbolProductions.Select(Item<TSymbol>.Init);
            var nextSymbolsItems = nextSymbols
                .SelectMany(symbol => GetClosureItems(symbol, addedSymbolsSet));

            return items.Concat(nextSymbolsItems);
        }
    }
}