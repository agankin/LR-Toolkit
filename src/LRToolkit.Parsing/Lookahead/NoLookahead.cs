using System.Collections;
using Optional;

namespace LRToolkit.Parsing;

internal class NoLookahead<TSymbol> : ILookahead<TSymbol> where TSymbol : notnull
{
    private NoLookahead() { }

    public static readonly NoLookahead<TSymbol> Instance = new NoLookahead<TSymbol>();

    public int Count => 0;

    public Option<Symbol<TSymbol>> this[int index] => Option.None<Symbol<TSymbol>>();

    public IEnumerator<Symbol<TSymbol>> GetEnumerator() =>
        Enumerable.Empty<Symbol<TSymbol>>().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}