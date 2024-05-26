using System.Collections;
using LRToolkit.Utilities;
using PureMonads;

namespace LRToolkit.Grammaring;

public class Production<TSymbol> : IReadOnlyCollection<TSymbol>, IEquatable<Production<TSymbol>>
{
    private readonly IReadOnlyList<TSymbol> _productionSymbols;

    public Production(IReadOnlyList<TSymbol> productionSymbols) =>
        _productionSymbols = productionSymbols;

    public Production(params TSymbol[] productionSymbols) : this(productionSymbols.AsReadOnly()) { }

    public Option<TSymbol> this[int index] =>
        index < Count
            ? _productionSymbols[index].Some()
            : Option.None<TSymbol>();

    public TSymbol First => _productionSymbols[0];

    public int Count => _productionSymbols.Count;

    public bool Equals(Production<TSymbol>? other) => other != null && this.SequenceEqual(other);

    public IEnumerator<TSymbol> GetEnumerator() => _productionSymbols.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => _productionSymbols.GetEnumerator();

    public override bool Equals(object? obj) => obj is Production<TSymbol> other && Equals(other);

    public override int GetHashCode() => Hash.FNV(this);

    public override string ToString() => string.Join(", ", _productionSymbols);
}