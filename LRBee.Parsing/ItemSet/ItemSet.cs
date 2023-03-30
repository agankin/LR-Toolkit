using System.Collections;
using LRBee.Utilities;

namespace LRBee.Parsing
{
    public class ItemSet<TSymbol> : IEquatable<ItemSet<TSymbol>>, IEnumerable<Item<TSymbol>> where TSymbol : notnull
    {
        private readonly IReadOnlySet<Item<TSymbol>> _items;

        public ItemSet(IEnumerable<Item<TSymbol>> items) =>
            _items = new HashSet<Item<TSymbol>>(items);

        public IReadOnlySet<Symbol<TSymbol>> GetSymbolsAhead()
        {
            var symbolsAhead = _items.OnlySome(item => item.GetSymbolAhead());
            return new HashSet<Symbol<TSymbol>>(symbolsAhead);
        }

        public ItemSet<TSymbol> StepForward(Symbol<TSymbol> symbol)
        {
            var symbolAheadItems = _items.Where(item => item.GetSymbolAhead().SomeEquals(symbol));
            var items = symbolAheadItems.OnlySome(item => item.StepForward());

            return new ItemSet<TSymbol>(items);
        }

        public IEnumerator<Item<TSymbol>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public bool Equals(ItemSet<TSymbol>? other) =>
            other != null && _items.SetEquals(other._items);

        public override bool Equals(object? obj) =>
            obj is ItemSet<TSymbol> other && Equals(other);

        public override int GetHashCode() => FNVHash.Get(_items);
    }
}