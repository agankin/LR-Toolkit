using System.Collections;
using LRBee.Utilities.Extensions;

namespace LRBee.Parsing.ItemBuilding
{

    public class ItemSet<TSymbol> : IEnumerable<Item<TSymbol>>
    {
        private readonly IReadOnlySet<Item<TSymbol>> _items;

        public ItemSet(IEnumerable<Item<TSymbol>> items)
        {
            _items = new HashSet<Item<TSymbol>>(items);
        }

        public IReadOnlySet<TSymbol> GetNextSymbols() => new HashSet<TSymbol>(
            _items.Select(item => item.GetNextSymbol()).SelectSome());

        public ItemSet<TSymbol> AfterNextSymbol(TSymbol symbol)
        {
            var itemsAfterSymbol = _items
                .Where(item => item.GetNextSymbol().SomeEquals(symbol))
                .Select(item => item.StepPosition())
                .SelectSome();

            return new ItemSet<TSymbol>(itemsAfterSymbol);
        }

        public IEnumerator<Item<TSymbol>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();
    }
}