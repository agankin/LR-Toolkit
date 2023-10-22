using System.Collections;
using LRToolkit.Utilities;
using Optional;

namespace LRToolkit.Parsing;

internal class OneLookahead<TSymbol> : ILookahead<TSymbol>, IEquatable<OneLookahead<TSymbol>> where TSymbol : notnull
{
    private readonly Symbol<TSymbol> _symbol;

    public OneLookahead(TSymbol symbol) => _symbol = Symbol<TSymbol>.CreateLookahead(symbol);

    public OneLookahead(Symbol<TSymbol> symbol) => _symbol = symbol;

    public int Count => 1;

    public Option<Symbol<TSymbol>> this[int index] => index == 0
        ? _symbol.Some()
        : Option.None<Symbol<TSymbol>>();

    public IEnumerator<Symbol<TSymbol>> GetEnumerator() => new OneEnumerator<Symbol<TSymbol>>(_symbol);

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Equals(OneLookahead<TSymbol>? other) => other != null && _symbol == other._symbol;

    public override bool Equals(object? obj) => obj is OneLookahead<TSymbol> other && Equals(other);

    public override int GetHashCode() => _symbol.GetHashCode();
}