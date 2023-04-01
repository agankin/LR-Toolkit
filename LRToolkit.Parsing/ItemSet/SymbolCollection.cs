using System.Collections;
using LRToolkit.Utilities;

namespace LRToolkit.Parsing
{
    public readonly struct SymbolCollection<TSymbol> : IEquatable<SymbolCollection<TSymbol>>, IReadOnlyList<Symbol<TSymbol>>
        where TSymbol : notnull
    {
        private readonly IReadOnlyList<Symbol<TSymbol>> _symbols;

        public SymbolCollection() => _symbols = Array.Empty<Symbol<TSymbol>>();

        public SymbolCollection(IReadOnlyList<Symbol<TSymbol>> symbols) =>
            _symbols = symbols ?? throw new ArgumentNullException(nameof(symbols));

        public Symbol<TSymbol> this[int index] => _symbols[index];

        public int Count => _symbols.Count;

        public bool Equals(SymbolCollection<TSymbol> other) => other._symbols.SequenceEqual(_symbols);

        public IEnumerator<Symbol<TSymbol>> GetEnumerator() => _symbols.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public override int GetHashCode() => FNVHash.Get(_symbols);
    }
}