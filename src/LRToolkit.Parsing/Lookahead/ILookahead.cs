using Optional;

namespace LRToolkit.Parsing;

public interface ILookahead<TSymbol> : IEnumerable<Symbol<TSymbol>> where TSymbol : notnull
{
    int Count { get; }

    Option<Symbol<TSymbol>> this[int index] { get; }
}