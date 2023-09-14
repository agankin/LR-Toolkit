using LRToolkit.Utilities;
using Optional;
using Optional.Unsafe;

namespace LRToolkit.Parsing;

public readonly record struct Symbol<TSymbol>(
    SymbolType Type,
    Option<TSymbol> Value
) where TSymbol : notnull
{
    public static Symbol<TSymbol> Create(TSymbol symbol) => new Symbol<TSymbol>(SymbolType.Symbol, symbol.Some());

    public static Symbol<TSymbol> CreateLookahead(TSymbol symbol) => new Symbol<TSymbol>(SymbolType.Lookahead, symbol.Some());

    public static Symbol<TSymbol> End() => new Symbol<TSymbol>(SymbolType.End, Option.None<TSymbol>());

    public bool Equals(Symbol<TSymbol> other) => Value == other.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string? ToString()
    {
        return Type switch
        {
            SymbolType.Symbol => Value.ValueOrFailure().ToString(),
            SymbolType.Lookahead => Value.ValueOrFailure().ToString(),
            SymbolType.End => "$",
            _ => throw new UnsupportedEnumValueException<SymbolType>(Type)
        };
    }
}