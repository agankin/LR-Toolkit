using System.Collections;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing
{
    public class ItemSet<TSymbol> : IEquatable<ItemSet<TSymbol>>, IEnumerable<Item<TSymbol>> where TSymbol : notnull
    {
        private readonly IReadOnlySet<Item<TSymbol>> _items;

        public ItemSet(IReadOnlySet<Item<TSymbol>> items) => _items = items;

        public IEnumerable<Symbol<TSymbol>> GetSymbolsAhead()
        {
            var symbolsAhead = _items.OnlySome(item => item.GetSymbolAhead());
            return symbolsAhead;
        }

        public ItemSet<TSymbol> StepForward(Symbol<TSymbol> symbol)
        {
            var symbolAheadItems = _items.Where(item => item.GetSymbolAhead().SomeEquals(symbol));
            var items = symbolAheadItems.OnlySome(item => item.StepForward()).ToHashSet();

            return new ItemSet<TSymbol>(items);
        }

        public IEnumerator<Item<TSymbol>> GetEnumerator() => _items.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => _items.GetEnumerator();

        public bool Equals(ItemSet<TSymbol>? other) =>
            other != null && _items.SetEquals(other._items);

        public override bool Equals(object? obj) =>
            obj is ItemSet<TSymbol> other && Equals(other);

        public override int GetHashCode() => Hash.CalculateFNV(_items);

        public override string ToString() => string.Join(", ", _items.Select(item => $"[{item}]"));
    }
}